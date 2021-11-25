using CommonLibrary;
using RobotPointsRenumber.View;
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

namespace RobotPointsRenumber.Model.Operations.ABB
{
    public static class ABB_Static_Methods
    {
        private static Regex isMovePointRegex = new Regex(@"^(SPZ_(|Fraesen|Messen)|move)(j|l|PTP(||_ROB|_FRG|_FB)|LIN(||_ROB|_FRG|_FB))\s+", RegexOptions.IgnoreCase);
        private static Regex procRegex = new Regex(@"(?<=(^|\w+\s+)PROC\s+)[a-zA-Z0-9_-]*\(.*\)", RegexOptions.IgnoreCase);
        private static Regex endProcRegex = new Regex(@"^ENDPROC", RegexOptions.IgnoreCase);
        private static Regex robtargetNameRegex = new Regex(@"(?<=robtarget\s+).*(?=:\=)", RegexOptions.IgnoreCase);
        private static Regex isComment = new Regex(@"^!.*", RegexOptions.IgnoreCase);
        private static Regex robtargetWithRelToolOrOffs = new Regex(@"(?<=(SPZ_(|Fraesen|Messen)|Move)((L|J|PTP(||_ROB|_FRG|_FB)|LIN(||_ROB|_FRG|_FB))\s+(Offs|RelTool)\s*\(\s*))[a-zA-Z0-9\-_+]*", RegexOptions.IgnoreCase);
        private static Regex robtargetWithoutRelTool = new Regex(@"(?<=(SPZ_(|Fraesen|Messen)|Move)(L|J|PTP(||_ROB|_FRG|_FB)|LIN(||_ROB|_FRG|_FB))\s+)[a-zA-Z0-9\-_+]*", RegexOptions.IgnoreCase);
        private static Regex isInlinePointRegex = new Regex(@"(SPZ_(|Fraesen|Messen)|Move)((J|PTP(||_ROB|_FRG|_FB))\s+(Offs|RelTool)\s*\(\s*|(L|LIN(||_ROB|_FRG|_FB))\s+(Offs|RelTool)\s*\(\s*|(J|PTP(||_ROB|_FRG|_FB))\s+|(L|LIN(||_ROB|_FRG|_FB))\s+)\[\s*\[\s*(-\d+|\d+)", RegexOptions.IgnoreCase);
        private static Regex kwLoesenRegex = new Regex(@"(?<=KW_Loesen(LIN|PTP)\s+).+", RegexOptions.IgnoreCase);
        private static Dictionary<string, ABBModule> oldModules;
        private static string backupfile;
        private static List<string> inlinePointsFound;


        internal static void Execute()
        {
            DateTime expirationDate = new DateTime(2021, 11, 30);
            int dateComp = DateTime.Compare(DateTime.Now, expirationDate);
            bool isAiutUser = GetAiutUser();
            if (dateComp > 0 && !isAiutUser)
            {
                MessageBox.Show("Application has expired and will close.", "Expired", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            inlinePointsFound = new List<string>();
            bool isDir;
            var dialog = MessageBox.Show("Backups are in form of zip od directory?\r\nYes - zip\r\nNo - Directory", "?", MessageBoxButton.YesNo, MessageBoxImage.Question);
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
            RenameWindowViewModel vm = new RenameWindowViewModel(modules);
            RenameWindow sW = new RenameWindow(vm);
            var dialogResult = sW.ShowDialog();

        }

        private static bool GetAiutUser()
        {
            string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            if (userName.Length >= 4 && userName.ToLower().Substring(0, 4) == "aiut" || userName.Length >= 6 && userName.ToLower().Substring(0, 6) == "233-ws")
                return true;
            return false;

        }

        private static Dictionary<string, ABBModule> GetModulesFromBackup(string backupfile, bool isDir)
        {
            List<string> excludedMods = new List<string>();
            Dictionary<string, ABBModule> tempDict = new Dictionary<string, ABBModule>();
            if (!isDir)
            {
                using (ZipArchive archive = ZipFile.Open(backupfile, ZipArchiveMode.Read))
                {
                    List<ZipArchiveEntry> entries = GetModFilesFromBackup(archive.Entries);
                    tempDict = GetEntriesOrFiles(entries, out excludedMods, archive: archive);
                }
            }
            else
            {
                List<string> modFiles = Directory.GetFiles(backupfile, "*.mod", SearchOption.AllDirectories).ToList();
                tempDict = GetEntriesOrFiles(modFiles, out excludedMods);
            }
            if (inlinePointsFound.Count > 0)
            {
                string inlines = string.Empty;
                foreach (var mod in inlinePointsFound)
                    inlines += mod + "\r\n";
                MessageBox.Show("Inline points found in modules:\r\n" + inlines, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            foreach (var item in excludedMods.Where(x => tempDict.Keys.Contains(x)))
                tempDict.Remove(item);
            Dictionary<string, ABBModule> result = new Dictionary<string, ABBModule>();
            foreach (var item in tempDict.Where(x => x.Value.ProceduresWithRobtargets.Count > 0))
            {
                item.Value.AssignPointsOldAndNew();
                result.Add(item.Key, item.Value);
            }
            return result;

        }

        //private static Dictionary<string, ABBModule> GetEntriesOrFiles(dynamic entries, ZipArchive archive = null, out Dictionary<string,List<string>>.KeyCollection excludedMods)
        private static Dictionary<string, ABBModule> GetEntriesOrFiles(dynamic entries, out List<string> excludedMods, ZipArchive archive = null)
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
                    StreamReader reader = new StreamReader(archive == null ? modFile : modFile.Open(),Encoding.Default);
                    bool isProcedureActive = false;
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        exceptionLine = line;
                        result[currentModName].Content.Add(line);
                        if (procRegex.IsMatch(line.Trim()) && !isComment.IsMatch(line.Trim()))
                        {
                            currentProc = procRegex.Match(line.Trim()).ToString();
                            result[currentModName].Procedures.Add(currentProc, new List<string>());
                            isProcedureActive = true;
                            result[currentModName].ProceduresList.Add(new ABBProcedure(currentProc));
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
                                {
                                    result[currentModName].ProceduresWithRobtargets[currentProc].Add(robtargetWithRelToolOrOffs.Match(line).ToString());
                                    result[currentModName].ProceduresList.First(x => x.Name.ToLower() == currentProc.ToLower()).PointsOldAndNew.Add(new KeyValuePair<string, string>(robtargetWithRelToolOrOffs.Match(line).ToString(), string.Empty));
                                }
                                else
                                {
                                    if (!inlinePointsFound.Contains(modFile.FullName))
                                        inlinePointsFound.Add(modFile.FullName);
                                }
                            }
                            else
                            {
                                if (!isInlinePointRegex.IsMatch(line))
                                {
                                    result[currentModName].ProceduresWithRobtargets[currentProc].Add(robtargetWithoutRelTool.Match(line).ToString());
                                    result[currentModName].ProceduresList.First(x => x.Name.ToLower() == currentProc.ToLower()).PointsOldAndNew.Add(new KeyValuePair<string, string>(robtargetWithoutRelTool.Match(line).ToString(),string.Empty));
                                }
                                else
                                {
                                    if (!inlinePointsFound.Contains(modFile.FullName))
                                        inlinePointsFound.Add(modFile.FullName);
                                }
                            }
                        }
                        if (isProcedureActive)
                            result[currentModName].ProceduresList.First(x=>x.Name.ToLower() == currentProc.ToLower()).Content.Add(line);
                        if (endProcRegex.IsMatch(line.Trim()) && !isComment.IsMatch(line.Trim()))
                        {
                            currentProc = string.Empty;
                            isProcedureActive = false;
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
                    doublePointsDict.Keys.ToList().ForEach(x => doubleMods += Path.GetFileName(x) + "\r\n");
                    MessageBox.Show("Double point declaration found in mods:\r\n " + doubleMods, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                excludedMods = doublePointsDict.Keys.ToList();
                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong with line:\r\n" + exceptionLine, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                excludedMods = null;
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
                foundModules = FillRenamedPoints(foundModules);
                if (foundModules == null)
                    return;
                string dirname = Path.GetFileName(backupfile) +  "_RenamedModules_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss");
                string message = string.Empty;
                foreach (var module in foundModules.Where(x => x.Value.ProceduresWithRobtargets.Count > 0 && x.Value.Modified))
                {
                    if (VerifyPath(module))
                    {
                        string moduleContent = string.Empty;
                        foreach (var line in module.Value.Content)
                        {
                            string lineToAdd = line;
                            if (!isComment.IsMatch(line.Trim()) && robtargetNameRegex.IsMatch(line) && !isInlinePointRegex.IsMatch(line))
                            {
                                string pointName = robtargetNameRegex.Match(line).ToString();
                                lineToAdd = RenameRobtarget(line, module.Value.PointsOldAndNew.First(x=>x.Key.ToLower() == pointName.ToLower()));
                            }
                            if (isMovePointRegex.IsMatch(line.Trim()))
                            {
                                if (robtargetWithRelToolOrOffs.IsMatch(line))
                                {
                                    string pointName = robtargetWithRelToolOrOffs.Match(line).ToString();
                                    lineToAdd = robtargetWithRelToolOrOffs.Replace(line, module.Value.PointsOldAndNew.First(x => x.Key.ToLower() == pointName.ToLower()).Value);
                                }
                                else if (robtargetWithoutRelTool.IsMatch(line))
                                {
                                    string pointName = robtargetWithoutRelTool.Match(line).ToString();
                                    lineToAdd = robtargetWithoutRelTool.Replace(line, module.Value.PointsOldAndNew.First(x => x.Key.ToLower() == pointName.ToLower()).Value);
                                }
                                else
                                { }//error
                                
                            }
                            if (lineToAdd != "POINT NOT FOUND")
                                moduleContent += lineToAdd + "\r\n";
                        }


                        //create new robtarget list
                        //List<string> correctedRobtargets = CreateRenamedRobtargets(module.Value.Robtargets, module.Value.ProceduresWithRobtargets, oldModules[module.Key].ProceduresWithRobtargets);
                        //rename points in movel/movej/relpos
                        //List<string> correctedPaths = CreateCorrectedPaths(module.Value.Procedures, module.Value.ProceduresWithRobtargets, oldModules[module.Key].ProceduresWithRobtargets);
                        moduleContent = SortRobtargets(moduleContent, module.Key);
                        bool isSuccess = WriteFile(moduleContent, Path.GetFileName(module.Key),dirname);
                        if (isSuccess)
                            message += Path.GetFileName(module.Key) + "\r\n";
                    }
                }
                if (!string.IsNullOrEmpty(message))
                {
                    var dialog =  MessageBox.Show("Files:\r\n" + message + "were saved at " + Path.GetDirectoryName(backupfile) + "\\"+dirname+"\r\nWould you like to open this directory?", "Success", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (dialog == MessageBoxResult.Yes)
                    {
                        if (Directory.Exists(Path.GetDirectoryName(backupfile) + "\\" + dirname.Trim()))
                            System.Diagnostics.Process.Start(Path.GetDirectoryName(backupfile) + "\\" + dirname.Trim());
                        else
                            MessageBox.Show("Destination folder does not exist", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static string SortRobtargets(string moduleContent, string modulename)
        {
            Regex pointNameRegex = new Regex(@"(?<=robtarget\s+)[\w\d_-]+(?=\s*\:\s*\=\s*\[)", RegexOptions.IgnoreCase);
            IDictionary<string, string> points = new SortedDictionary<string, string>();
            string result = string.Empty, tempPossibleRobtarget = string.Empty, tempDefinitiveRobtarget = string.Empty;
            string previousLine = string.Empty;
            bool possibleRobtargetSection = false;
            bool definitiveRobtargetSection = false;
            bool robtargetSectionFinished = false, robtargetSectionFound = false, lastCycle = false;
            string robtargetSection = string.Empty;
            var dialog = MessageBox.Show("Would you like to sort robtargets in module " + modulename + "?", "Sort robtargets?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (dialog == MessageBoxResult.No)
                return moduleContent;
            StringReader reader = new StringReader(moduleContent);
            while (true)
            {
                string line = reader.ReadLine();
                if (line == null)
                    break;
                if (line.Contains("p320"))
                { }
                if (!robtargetSectionFinished)
                {
                    if (possibleRobtargetSection && definitiveRobtargetSection && line.Trim().Length >= 1 && line.Trim().Substring(0, 1) != "!" && !pointNameRegex.IsMatch(line))
                        lastCycle = true;
                    if (line.Trim() == string.Empty || line.Trim().Length >= 1 && ((line.Trim().Substring(0, 1) == "!") || lastCycle))
                    {
                        possibleRobtargetSection = true;
                        if (definitiveRobtargetSection)
                        {
                            robtargetSectionFound = true;
                            //if (previousLine.Trim() == string.Empty && line.Trim().Length > 1 && (line.Trim().Substring(0, 1) == "!") && definitiveRobtargetSection)
                            if (previousLine.Trim() == string.Empty && line.Trim().Length >= 1 && definitiveRobtargetSection)
                            {
                                result += SortRobtargetsInSection(tempPossibleRobtarget, points, moduleContent) + "\r\n";
                                tempDefinitiveRobtarget = string.Empty;
                                tempPossibleRobtarget = string.Empty;
                                definitiveRobtargetSection = false;
                                points.Clear();
                                //tempDefinitiveRobtarget += line + "\r\n";
                                tempPossibleRobtarget += line + "\r\n";
                                //if (line.Length >= 1 && line.Substring(0, 1) != "!")
                                //    possibleRobtargetSection = false;
                            }
                        }
                        else
                            tempPossibleRobtarget += line + "\r\n";
                    }
                    else if (pointNameRegex.IsMatch(line) && possibleRobtargetSection)
                    {
                        definitiveRobtargetSection = true;
                        points.Add(pointNameRegex.Match(line).ToString(), line);
                    }
                    else
                    {
                        possibleRobtargetSection = false;
                        definitiveRobtargetSection = false;
                    }
                    if (lastCycle)
                    {
                        //lastCycle = false;
                        possibleRobtargetSection = false;
                        definitiveRobtargetSection = false;
                    }
                }
                if (!possibleRobtargetSection && !definitiveRobtargetSection)
                {
                    if (robtargetSectionFound && !robtargetSectionFinished && line.Trim().Length > 1 && line.Trim().Substring(0, 1) != "!")
                    {
                        robtargetSectionFinished = true;
                        if (!lastCycle)
                            result += tempPossibleRobtarget;
                    }
                    result += line + "\r\n";

                }
                previousLine = line;
                
            }
            reader.Close();
            return result;
        }

        private static string SortRobtargetsInSection(string tempPossibleRobtarget, IDictionary<string, string> points, string moduleContent)
        {
            List<string> alreadyAddedRobtargets = new List<string>();
            string result = tempPossibleRobtarget;
            StringReader reader = new StringReader(moduleContent);
            while (true)
            {
                string line = reader.ReadLine();
                if (line == null)
                    break;
                if (robtargetWithoutRelTool.IsMatch(line.Trim()))
                {
                    string pointName = robtargetWithoutRelTool.Match(line).ToString();
                    if (points.Keys.Contains(pointName))
                    {
                        if (!alreadyAddedRobtargets.Contains(pointName.ToLower()))
                        {
                            alreadyAddedRobtargets.Add(pointName.ToLower());
                            result += points.First(x => x.Key == pointName).Value + "\r\n";
                        }
                        
                    }
                }
                else if (kwLoesenRegex.IsMatch(line))
                {
                    string tempstring = kwLoesenRegex.Match(line).ToString();
                    var substrings = tempstring.Split(',');
                    for (int i = 0; i < 2; i++)
                    {
                        if (!alreadyAddedRobtargets.Contains(substrings[i].ToLower()) && points.FirstOrDefault(x => x.Key == substrings[i]).Value != null)
                        {
                            alreadyAddedRobtargets.Add(substrings[i].ToLower());
                            result += points.First(x => x.Key == substrings[i]).Value + "\r\n";
                        }
                    }
                }
            }
            return result;
        }

        private static string RenameRobtarget(string line, KeyValuePair<string, string> pointOldAndNew)
        {
            Regex renameRegex = new Regex(@"(?<=robtarget\s+)[\w\d_-]+", RegexOptions.IgnoreCase);
            if (pointOldAndNew.Value == "POINT NOT FOUND")
                return "POINT NOT FOUND";
            return renameRegex.Replace(line,pointOldAndNew.Value);
        }

        private static Dictionary<string, ABBModule> FillRenamedPoints(Dictionary<string, ABBModule> foundModules)
        {
            bool doublesFound = false;
            List<string> alreadyfoundPoints = new List<string>();
            Dictionary<string, ABBModule> result = new Dictionary<string, ABBModule>();
            foreach (var module in foundModules.Where(x=>x.Value.Modified == true))
            {
                alreadyfoundPoints = new List<string>();
                result.Add(module.Key, module.Value.Clone() as ABBModule);
                result[module.Key].PointsOldAndNew.Clear();
                foreach (var point in module.Value.Robtargets)
                {
                    ABBProcedure procWithPoint = module.Value.ProceduresList.FirstOrDefault(x => x.PointsOldAndNew.Any(y => y.Key.ToLower() == point.Key.ToLower()));
                    if (procWithPoint == null)
                    {
                        Regex isLoesenRegex = new Regex(@"Kappe.+(|_)Loesen", RegexOptions.IgnoreCase);
                        if (isLoesenRegex.IsMatch(point.Key))
                        {
                            result[module.Key].PointsOldAndNew.Add(new KeyValuePair<string, string>(point.Key, point.Key));
                            if (!alreadyfoundPoints.Contains(point.Key.ToLower()))
                                alreadyfoundPoints.Add(point.Key.ToLower());
                            else
                                doublesFound = true;
                        }
                        else
                            result[module.Key].PointsOldAndNew.Add(new KeyValuePair<string, string>(point.Key, "POINT NOT FOUND"));
                    }
                    else
                    {
                        var newName = procWithPoint.PointsOldAndNew.First(x => x.Key.ToLower() == point.Key.ToLower());
                        if (string.IsNullOrEmpty(newName.Value))
                        {
                            result[module.Key].PointsOldAndNew.Add(new KeyValuePair<string, string>(newName.Key, newName.Key));
                            if (!alreadyfoundPoints.Contains(newName.Key.ToLower()))
                                alreadyfoundPoints.Add(newName.Key.ToLower());
                            else
                                doublesFound = true;
                        }
                        else
                        {
                            result[module.Key].PointsOldAndNew.Add(new KeyValuePair<string, string>(newName.Key, newName.Value));
                            if (!alreadyfoundPoints.Contains(newName.Value.ToLower()))
                                alreadyfoundPoints.Add(newName.Value.ToLower());
                            else
                                doublesFound = true;
                        }
                    }
                }
            }
            if (doublesFound)
            {
                MessageBox.Show("Double declarations of points found. Program will abort!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            return result;
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
                    MessageBox.Show("Multiple points in module " + module.Key + ". Module will NOT be renumbered", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }
            return true;
        }

        private static bool WriteFile(string moduleContent, string moduleName, string dirName)
        {
            bool isSuccess = false;
            try
            {
                string path = Path.GetDirectoryName(backupfile);
                if (!Directory.Exists(path + "\\" + dirName))
                    Directory.CreateDirectory(path + "\\" + dirName);
                if (moduleName.Substring(moduleName.Length - 4, 4).ToLower() != ".mod")
                    moduleName = moduleName + ".mod";
                File.WriteAllText(path + "\\" + dirName + "\\" + moduleName, moduleContent);
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
