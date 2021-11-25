using RobotFilesEditor.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace RobotFilesEditor.Model.Operations.ABB
{
    public static class ABB_Static_Methods
    {
        private static Regex isMovePointRegex = new Regex(@"^move(j|l|PTP|LIN)\s+", RegexOptions.IgnoreCase);
        private static Regex procRegex = new Regex(@"(?<=(^|\w+\s+)PROC\s+)[a-zA-Z0-9_-]*\(.*\)", RegexOptions.IgnoreCase);
        private static Regex endProcRegex = new Regex(@"^ENDPROC", RegexOptions.IgnoreCase);
        private static Regex robtargetNameRegex = new Regex(@"(?<=robtarget\s+).*(?=:\=)", RegexOptions.IgnoreCase);
        private static Regex isComment = new Regex(@"^!.*", RegexOptions.IgnoreCase);
        private static Regex robtargetWithRelToolOrOffs = new Regex(@"(?<=Move((L|J|PTP|LIN)\s+(Offs|RelTool)\s*\(\s*))[a-zA-Z0-9\-_+]*", RegexOptions.IgnoreCase);
        private static Regex robtargetWithoutRelTool = new Regex(@"(?<=Move(L|J|PTP|LIN)\s+)[a-zA-Z0-9\-_+]*", RegexOptions.IgnoreCase);
        private static Regex isInlinePointRegex = new Regex(@"Move((J|PTP)\s+(Offs|RelTool)\s*\(\s*|(L|LIN)\s+(Offs|RelTool)\s*\(\s*|(J|PTP)\s+|(L|LIN)\s+)\[\s*\[\s*(-\d+|\d+)", RegexOptions.IgnoreCase);
        private static Dictionary<string, ABBModule> oldModules;
        private static string backupfile;
        private static List<string> inlinePointsFound;


        internal static void Execute()
        {
            inlinePointsFound = new List<string>();
            bool isDir;
            var dialog = MessageBox.Show("Backups are in form of zip od directory?\r\nYes - zip\r\nNo - Directory","?",MessageBoxButton.YesNo,MessageBoxImage.Question);
            if (dialog == MessageBoxResult.Yes)
            {
                backupfile = CommonLibrary.CommonMethods.SelectDirOrFile(false, filter1Descr: "Zip File", filter1: "*.zip");
                isDir = false;
            }
            else
            {
                isDir = true;
                backupfile = CommonLibrary.CommonMethods.SelectDirOrFile(true);
            }
            if (string.IsNullOrEmpty(backupfile))
                return;

            Dictionary<string, ABBModule> modules = GetModulesFromBackup(backupfile, isDir);
            oldModules = modules.ToDictionary(kv => kv.Key, kv => kv.Value.Clone() as ABBModule);
            ABBRenameViewModel vm = new ABBRenameViewModel(modules);
            ABBRenameWindow sW = new ABBRenameWindow(vm);
            var dialogResult = sW.ShowDialog();

        }

        private static Dictionary<string, ABBModule> GetModulesFromBackup(string backupfile, bool isDir)
        {
            Dictionary<string, ABBModule> result = new Dictionary<string, ABBModule>();
            if (!isDir)
            {
                using (ZipArchive archive = ZipFile.Open(backupfile, ZipArchiveMode.Read))
                {
                    List<ZipArchiveEntry> entries = GetModFilesFromBackup(archive.Entries);
                    result = GetEntriesOrFiles(entries, archive);
                }
            }
            else
            {
                List<string> modFiles = Directory.GetFiles(backupfile, "*.mod", SearchOption.AllDirectories).ToList();
                result = GetEntriesOrFiles(modFiles);
            }
            if (inlinePointsFound.Count > 0)
            {
                string inlines = string.Empty;
                foreach (var mod in inlinePointsFound)
                    inlines += mod + "\r\n";
                MessageBox.Show("Inline points found in modules:\r\n" + inlines, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return result;
                
        }

        private static Dictionary<string, ABBModule> GetEntriesOrFiles(dynamic entries, ZipArchive archive = null)
        {
            Dictionary<string, List<string>> doublePointsDict = new Dictionary<string, List<string>>();
            string exceptionLine = string.Empty;
            try
            {
                Dictionary<string, ABBModule> result = new Dictionary<string, ABBModule>();
                foreach (var entry in entries)
                {
                    string currentProc = string.Empty;
                    dynamic modFile;
                    string currentModName = string.Empty;
                    if (archive != null)
                    {
                        modFile = archive.GetEntry(CommonLibrary.CommonMethods.GetEntryName(archive.Entries, entry.ToString()));
                        currentModName = modFile.FullName;
                    }
                    else
                    {
                        modFile = entry;
                        currentModName = Path.GetFileNameWithoutExtension((modFile as string));
                    }
                    
                    result.Add(currentModName, new ABBModule(currentModName));
                    StreamReader reader = new StreamReader(archive == null ? modFile : modFile.Open());
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        exceptionLine = line;
                        result[currentModName].Content.Add(line);
                        if (procRegex.IsMatch(line.Trim()) && !isComment.IsMatch(line.Trim()))
                        {
                            currentProc = procRegex.Match(line.Trim()).ToString();
                            result[currentModName].Procedures.Add(currentProc, new List<string>());
                        }
                        if (!string.IsNullOrEmpty(currentProc))
                        {
                            result[currentModName].Procedures[currentProc].Add(line);
                        }
                        if (!string.IsNullOrEmpty(currentProc) && isMovePointRegex.IsMatch(line.Trim()))
                        {
                            if (!result[currentModName].ProceduresWithRobtargets.Keys.Contains(currentProc))
                            {
                                result[currentModName].ProceduresWithRobtargets.Add(currentProc, new List<string>());
                            }
                            if (line.ToLower().Contains(" reltool") || line.ToLower().Contains(" offs"))
                            {
                                if (!isInlinePointRegex.IsMatch(line))
                                    result[currentModName].ProceduresWithRobtargets[currentProc].Add(robtargetWithRelToolOrOffs.Match(line).ToString());
                                else
                                {
                                    if (!inlinePointsFound.Contains(modFile.FullName))
                                        inlinePointsFound.Add(modFile.FullName);
                                }
                            }
                            else
                            {
                                if (!isInlinePointRegex.IsMatch(line))
                                        result[currentModName].ProceduresWithRobtargets[currentProc].Add(robtargetWithoutRelTool.Match(line).ToString());
                                else
                                {
                                    if (!inlinePointsFound.Contains(modFile.FullName))
                                        inlinePointsFound.Add(modFile.FullName);
                                }
                            }
                        }

                        if (endProcRegex.IsMatch(line.Trim()) && !isComment.IsMatch(line.Trim()))
                        {
                            currentProc = string.Empty;
                        }
                        if (!isComment.IsMatch(line.Trim()) && robtargetNameRegex.IsMatch(line) && !isInlinePointRegex.IsMatch(line))
                        {
                            string currentPoint = robtargetNameRegex.Match(line).ToString();
                            if (!result[currentModName].Robtargets.Keys.Contains(currentPoint))
                                result[currentModName].Robtargets.Add(currentPoint, line);
                            else
                            {
                                if (!doublePointsDict.Keys.Contains(currentModName))
                                    doublePointsDict.Add(currentModName, new List<string>());
                                doublePointsDict[currentModName].Add(currentPoint);
                            }
                        }

                    }
                    reader.Close();
                }
                if (doublePointsDict.Count > 0)
                {
                    string doubleMods = string.Empty;
                    doublePointsDict.Keys.ToList().ForEach(x=> doubleMods += Path.GetFileName(x) + "\r\n");
                    MessageBox.Show("Double point declaration found in mods:\r\n " + doubleMods,"Warning",MessageBoxButton.OK,MessageBoxImage.Warning);
                }
                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong with line:\r\n" + exceptionLine, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        private static List<ZipArchiveEntry> GetModFilesFromBackup(ReadOnlyCollection<ZipArchiveEntry> entries)
        {
            List<ZipArchiveEntry> result = new List<ZipArchiveEntry>();
            foreach (var entry in entries.Where(x => Path.GetExtension(x.FullName) == ".mod"))
            {
                result.Add(entry);
            }
            return result;
        }

        internal static void ExecuteChange(Dictionary<string, ABBModule> foundModules)
        {
            try
            {
                string message = string.Empty;
                foreach (var module in foundModules.Where(x => x.Value.ProceduresWithRobtargets.Count > 0))
                {
                    if (VerifyPath(module))
                    {
                        //create new robtarget list
                        List<string> correctedRobtargets = CreateRenamedRobtargets(module.Value.Robtargets, module.Value.ProceduresWithRobtargets, oldModules[module.Key].ProceduresWithRobtargets);
                        //rename points in movel/movej/relpos
                        List<string> correctedPaths = CreateCorrectedPaths(module.Value.Procedures, module.Value.ProceduresWithRobtargets, oldModules[module.Key].ProceduresWithRobtargets);
                        bool isSuccess = WriteFile(correctedRobtargets, correctedPaths, Path.GetFileName(module.Key));
                        if (isSuccess)
                            message += Path.GetFileName(module.Key) + "\r\n";
                    }
                }
                if (!string.IsNullOrEmpty(message))
                    MessageBox.Show("Files:\r\n" + message + "were saved at " + Path.GetDirectoryName(backupfile) + "\\RenamedModules", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static bool VerifyPath(KeyValuePair<string, ABBModule> module)
        {
            if (module.Key.Contains("M_WZ2"))
            { }
            List<string> addedRobtargets = new List<string>();
            List<string> multipoints = new List<string>();
            foreach (var point in module.Value.Robtargets)
            {
                if (!addedRobtargets.Contains(point.Key.ToLower()))
                    addedRobtargets.Add(point.Key.ToLower());
                else
                    multipoints.Add(point.Key);
                if (multipoints.Count > 0)
                {
                    MessageBox.Show("Multiple points in module " + module.Key + ". Module will NOT be renumbered","Warning",MessageBoxButton.OK,MessageBoxImage.Warning);
                    return false;
                }
            }
            return true;
        }

        private static bool WriteFile(List<string> correctedRobtargets, List<string> correctedPaths,string moduleName)
        {
            bool isSuccess = false;
            try
            {
                string result = string.Empty;
                foreach (var line in correctedRobtargets)
                    result += line + "\r\n";
                result += "\r\n!*************************************************************\r\n\r\n";
                foreach (var line in correctedPaths)
                    result += line + "\r\n";
                string path = Path.GetDirectoryName(backupfile);
                if (!Directory.Exists(path + "\\RenamedModules"))
                    Directory.CreateDirectory(path + "\\RenamedModules");
                if (moduleName.Substring(moduleName.Length - 4, 4).ToLower() != ".mod")
                    moduleName = moduleName + ".mod";
                File.WriteAllText(path + "\\RenamedModules\\" + moduleName, result);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return isSuccess;
        }

        private static List<string> CreateCorrectedPaths(Dictionary<string, List<string>> procedures, Dictionary<string, List<string>> newProceduresWithRobtargets, Dictionary<string, List<string>> oldProceduresWthiRobtargets)
        {
            Regex currentRegex;
            List<string> result = new List<string>();
            foreach (var procedure in procedures)
            {
                if (newProceduresWithRobtargets.Keys.Contains(procedure.Key) && newProceduresWithRobtargets[procedure.Key].Count > 0)
                {
                    if (procedure.Key.ToLower().Contains("bremsen"))
                    { }
                    foreach (var line in procedure.Value)
                    {
                        if (line.ToLower().Contains("reltool"))
                            currentRegex = robtargetWithRelToolOrOffs;
                        else
                            currentRegex = robtargetWithoutRelTool;
                        if (currentRegex.IsMatch(line) && !isInlinePointRegex.IsMatch(line))
                        {
                            string currentPointName = currentRegex.Match(line).ToString();
                            int pointIndex = GetPointIndex(oldProceduresWthiRobtargets[procedure.Key], currentPointName);
                            result.Add(line.Replace(currentPointName, newProceduresWithRobtargets[procedure.Key][pointIndex]));

                        }
                        else
                            result.Add(line);
                    }
                    result.Add(string.Empty);
                    result.Add("!*************************************************************");
                    result.Add(string.Empty);
                }
                
            }

            return result;
        }

        private static int GetPointIndex(List<string> list, string currentPointName)
        {
            int counter = 0;
            foreach (var point in list)
            {
                if (point == currentPointName)
                    return counter;
                counter++;
            }
            return 0;
        }

        private static List<string> CreateRenamedRobtargets(Dictionary<string, string> robtargets, Dictionary<string, List<string>> newProceduresWithRobtargets, Dictionary<string, List<string>> oldProceduresWthiRobtargets)
        {
            string lastRobtarget = string.Empty;
            Dictionary<string, List<string>> procedureAndRobtargets = new Dictionary<string, List<string>>(); 
            List<string> result = new List<string>();
            try
            {
                int counter = 0;
                foreach (var procedure in oldProceduresWthiRobtargets)
                {
                    procedureAndRobtargets.Add(procedure.Key, new List<string>());
                    foreach (var robtarget in procedure.Value)
                    {
                        lastRobtarget = robtarget;
                        if (!procedureAndRobtargets[procedure.Key].Contains(robtargets[robtarget].Replace(robtarget, newProceduresWithRobtargets[procedure.Key][counter])))
                            procedureAndRobtargets[procedure.Key].Add(robtargets[robtarget].Replace(robtarget, newProceduresWithRobtargets[procedure.Key][counter]));
                        counter++;
                    }
                    counter = 0;
                }

                foreach (var procedure in procedureAndRobtargets)
                {
                    result.Add("! " + procedure.Key);
                    foreach (var robtarget in procedure.Value)
                    {
                        result.Add(robtarget);
                    }
                    result.Add(string.Empty);
                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }
    }
}
