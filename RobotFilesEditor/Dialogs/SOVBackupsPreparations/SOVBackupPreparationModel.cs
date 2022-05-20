using CommonLibrary;
using RobotFilesEditor.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using RobotFilesEditor.Model.Operations.DataClass;
using RobotFilesEditor.Model.Operations;
using System.Collections.ObjectModel;
using RobotFilesEditor.Model.Operations.BackupSyntaxValidation;
using System.Configuration;

namespace RobotFilesEditor.Dialogs.SOVBackupsPreparations
{
    public class SOVBackupPreparationModel
    {
        #region fields
        string backup;
        string paths;
        enum UpdateSwitch { Abort, Home };
        #endregion

        #region properties
        public ObservableCollection<SovBackupsPreparationResult> LogContent { get; private set; }
        #endregion

        #region ctor
        public SOVBackupPreparationModel(string backupPath, string pathsDirPath, GlobalData.RobotController robottype)
        {
            LogContent = new ObservableCollection<SovBackupsPreparationResult>();
            backup = backupPath;
            paths = pathsDirPath;
            Execute();
        }
        #endregion

        #region methods
        internal void Execute()
        {
            try
            {
                string selectedBackup = backup;
                string selectedDir = paths;
                RemoveEntry(selectedBackup, "init_out.src");
                CopyToDir("BMW_Std", selectedBackup, selectedDir, false);
                CopyToDir("BMW_App", selectedBackup, selectedDir, false);
                CopyToDir("BMW_Init", selectedBackup, selectedDir, false);
                CopyToDir("BMW_Std_User", selectedBackup, selectedDir, false);
                CopyToDir("BMW_Utilities", selectedBackup, selectedDir, false);
                CopyToDir("BMW_Utilities\\Docking", selectedBackup, selectedDir, false, "BMW_Utilities\\Docking");
                CopyToDir("Init_out", selectedBackup, selectedDir, false, "BMW_Std_User");
                CopyToDir("Program", selectedBackup, selectedDir, true);
                CopyToDir("BMW_Program", selectedBackup, selectedDir, true);
                CopyToDir("BMW_Program\\Abort_Programs", selectedBackup, selectedDir, false);
                CopyToDir("BMW_Program\\Home_Programs", selectedBackup, selectedDir, false);
                CopyToDir("TP\\SafeRobot", selectedBackup, selectedDir, false, "TP\\SafeRobot");
                CopyOrgs(selectedBackup, selectedDir);
                ValidateSelectSwitches(selectedBackup);
                DataForSOV data = GetDataForSOV(selectedDir);
                if (data != null)
                {
                    ReplaceValuesInConfigDat(selectedBackup, data);
                    MessageBox.Show("Backup at " + selectedBackup + " has been updated", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                KUKASynataxValidator validationReport = new KUKASynataxValidator(selectedBackup);
                validationReport.ErrorsFound.ForEach(x => LogContent.Add(x));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ValidateSelectSwitches(string selectedBackup)
        {
            Regex abortProgRegex = new Regex(@"(?<=AbortProg)\d+", RegexOptions.IgnoreCase);
            Regex homeProgNumRegex = new Regex(@"(?<=Home)\d+", RegexOptions.IgnoreCase);
            Regex homeProgRegex = new Regex(@"Home\d+_to_Home\d+", RegexOptions.IgnoreCase);
            Regex selectAbortRegex = new Regex(@"select_abortprog", RegexOptions.IgnoreCase);
            Regex selectHomeRegex = new Regex(@"select_homeprog", RegexOptions.IgnoreCase);
            List<string> homeSelectStringList = new List<string>(), abortSelectStringList = new List<string>();
            string homeSelectChaged = string.Empty, abortSelectChanged = string.Empty;
            using (FileStream zipToOpen = new FileStream(selectedBackup, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    var homePrograms = archive.Entries.Where(x => homeProgRegex.IsMatch(x.FullName) && Path.GetExtension(x.FullName) != ".dat").ToList();
                    var abortPrograms = archive.Entries.Where(x => abortProgRegex.IsMatch(x.FullName) && Path.GetExtension(x.FullName) != ".dat").ToList();
                    if (homePrograms.Count > 0)
                    {
                        homeSelectStringList = ReadEntry("select_homeprog.src", selectHomeRegex, archive);
                        homeSelectChaged = FixSelectProgram(UpdateSwitch.Home, homePrograms, homeSelectStringList);
                        UpdateEntry(archive, selectHomeRegex, homeSelectChaged);

                    }
                    if (abortPrograms.Count > 0)
                    {
                        abortSelectStringList = ReadEntry("select_abortprog.src", selectAbortRegex, archive);
                        abortSelectChanged = FixSelectProgram(UpdateSwitch.Abort, abortPrograms, abortSelectStringList);
                        UpdateEntry(archive, selectAbortRegex, abortSelectChanged);
                    }
                    
                    archive.Dispose();
                } 
                zipToOpen.Close();
            }
        }

        private void UpdateEntry(ZipArchive archive, Regex changeRegex, string content)
        {
            var entryToDelete = archive.Entries.Single(x => changeRegex.IsMatch(x.FullName));
            string entryName = entryToDelete.FullName;
            DeleteEntry(entryToDelete);
            var entry = archive.CreateEntry(entryName);
            StreamWriter writer = new StreamWriter(entry.Open());
            writer.Write(content);
            writer.Close();
            LogContent.Add(new SovBackupsPreparationResult("Entry " + entryName + " created in backup!", GlobalData.SovLogContentInfoTypes.Information));
        }

        private string FixSelectProgram(UpdateSwitch switchtype, List<ZipArchiveEntry> programs, List<string> stringList)
        {
            Regex homeProgNumRegex = new Regex(@"(?<=Home)\d+", RegexOptions.IgnoreCase);
            Regex addSwitchRegex = new Regex(@"^\s*;\s*ENDFOLD.*Declaration", RegexOptions.IgnoreCase);
            Regex abortProgRegex = new Regex(@"(?<=AbortProg)\d+", RegexOptions.IgnoreCase);
            string result = string.Empty;
            foreach (var line in stringList)
            {
                result += line + "\r\n";
                if (addSwitchRegex.IsMatch(line))
                {
                    result += "\r\nSWITCH Nr\r\n";
                    foreach (var item in programs)
                    {
                        result += "CASE " + (switchtype.Equals(UpdateSwitch.Home) ? GetHomeProgName(item) : abortProgRegex.Match(item.Name).ToString()) + "\r\n";
                        result += Path.GetFileNameWithoutExtension(item.Name) + "()\r\n";
                    }
                    result += "DEFAULT\r\n" + (switchtype.Equals(UpdateSwitch.Home) ? "PLC_i_AutoHomeState = -1" : "WAIT FOR FALSE") + "\r\nENDSWITCH\r\n\r\nEND";
                    break;
                }
            }
            return result;
        }

        private string GetHomeProgName(ZipArchiveEntry item)
        {
            MatchCollection matches = new Regex(@"(?<=Home)\d+", RegexOptions.IgnoreCase).Matches(item.Name);
            string result = string.Empty;
            foreach (var match in matches)
                result += match.ToString();
            return result;
        }

        private List<string> ReadEntry(string fileName, Regex selectHomeRegex, ZipArchive archive)
        {
            ExtractToTempConfigFile(fileName, archive.Entries.Single(x => selectHomeRegex.IsMatch(x.Name)));
            StreamReader reader = new StreamReader(Path.GetDirectoryName(CommonGlobalData.ConfigurationFileName) + "\\TempConfigFile\\" + fileName);
            List<string> result = ReadLines(reader);
            reader.Close();
            File.Delete(Path.GetDirectoryName(CommonGlobalData.ConfigurationFileName) + "\\TempConfigFile\\" + fileName);
            return result;
        }

        private List<string> ReadLines(StreamReader reader)
        {
            List<string> result = new List<string>();
            while (!reader.EndOfStream)
                 result.Add(reader.ReadLine());
            return result;
        }

        private void CopyOrgs(string selectedBackup, string selectedDir)
        {
            Regex isOrgRegex = new Regex(@"(^prog\d+)|(cell\.src)", RegexOptions.IgnoreCase);
            List<string> filesInDirectory = Directory.GetFiles(selectedDir, "*.*", SearchOption.AllDirectories).ToList().Where(x => isOrgRegex.IsMatch(Path.GetFileName(x))).ToList();
            using (FileStream zipToOpen = new FileStream(selectedBackup, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    archive.Entries.ToList().Where(x => isOrgRegex.IsMatch(x.Name)).ToList().ForEach(y => DeleteEntry(y));
                    filesInDirectory.ForEach(x => CopyEntry(x, "KRC\\R1\\" + Path.GetFileName(x), archive));
                    archive.Dispose();
                }
                zipToOpen.Close();
            }
        }

        private void ReplaceValuesInConfigDat(string selectedBackup, DataForSOV data)
        {
            string directory = Path.GetDirectoryName(CommonGlobalData.ConfigurationFileName);
            Regex isHomeRegex = new Regex(@"XHOME\d+", RegexOptions.IgnoreCase);
            Regex getNumber = new Regex(@"(?<=^.*\[)\d+", RegexOptions.IgnoreCase);
            string outputstring = "";
            string userVarsContentOld = string.Empty;
            string userVarsContentNew = CreateUserVars(data);
            ZipArchiveEntry userVarEntry = null;
            using (FileStream zipToOpen = new FileStream(selectedBackup, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    if (archive.Entries.Any(x=> x.FullName.ToLower().Contains("uservariables.dat")))
                    {
                        userVarEntry = archive.Entries.Single(x => x.FullName.ToLower().Contains("uservariables.dat"));
                        StreamReader reader2 = new StreamReader(userVarEntry.Open());
                        userVarsContentOld = reader2.ReadToEnd();
                        reader2.Close();
                        userVarEntry.Delete();
                    }

                    ZipArchiveEntry foundEntry = null;
                    foreach (var entry in archive.Entries.Where(x => x.FullName.ToLower().Contains("$config.dat")))
                    {
                        foundEntry = entry;
                        ExtractToTempConfigFile("$config.dat", entry);
                    }
                    if (foundEntry != null)
                        foundEntry.Delete();

                    StreamReader reader = new StreamReader(directory + "\\TempConfigFile\\$config.dat");
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (isHomeRegex.IsMatch(line))
                        {
                            Regex homeNum = new Regex(@"(?<=XHOME)\d", RegexOptions.IgnoreCase);
                            int number = int.Parse(homeNum.Match(line).ToString());
                            if (data.HomesSOV.Keys.Contains(number))
                            {
                                outputstring += "E6AXIS " + data.HomesSOV[number].Content + "\r\n";
                                LogContent.Add(new SovBackupsPreparationResult("Home " + number + " updated in $config.dat", GlobalData.SovLogContentInfoTypes.Information));
                            }
                            else
                                outputstring += line + "\r\n";
                        }
                        else if (line.ToLower().Contains("tool_data"))
                        {
                            int number = int.Parse(getNumber.Match(line).ToString());
                            if (data.ToolDatasSOV.Keys.Contains(number))
                            {
                                outputstring += data.ToolDatasSOV[number].Content + "\r\n";
                                LogContent.Add(new SovBackupsPreparationResult("Tool data " + number + " updated in $config.dat", GlobalData.SovLogContentInfoTypes.Information));
                            }
                            else
                                outputstring += line + "\r\n";
                        }
                        else if (line.ToLower().Contains("tool_name"))
                        {
                            int number = int.Parse(getNumber.Match(line).ToString());
                            if (data.ToolDatasSOV.Keys.Contains(number))
                            {
                                outputstring += data.ToolDatasSOV[number].Name + "\r\n";
                                LogContent.Add(new SovBackupsPreparationResult("Tool name " + number + " updated in $config.dat", GlobalData.SovLogContentInfoTypes.Information));
                            }
                            else
                                outputstring += line + "\r\n";
                        }
                        else if (line.ToLower().Contains("tool_type"))
                        {
                            int number = int.Parse(getNumber.Match(line).ToString());
                            if (data.ToolDatasSOV.Keys.Contains(number))
                            {
                                outputstring += data.ToolDatasSOV[number].Type + "\r\n";
                                LogContent.Add(new SovBackupsPreparationResult("Tool type " + number + " updated in $config.dat", GlobalData.SovLogContentInfoTypes.Information));
                            }
                            else
                                outputstring += line + "\r\n";
                        }
                        else if (line.ToLower().Contains("base_data"))
                        {
                            int number = int.Parse(getNumber.Match(line).ToString());
                            if (data.BaseDatasSOV.Keys.Contains(number))
                            {
                                Regex uncommentRegex = new Regex(@"^\s*;");
                                outputstring += uncommentRegex.Replace(data.BaseDatasSOV[number].Content,"") + "\r\n";
                                LogContent.Add(new SovBackupsPreparationResult("Base data " + number + " updated in $config.dat", GlobalData.SovLogContentInfoTypes.Information));
                            }
                            else
                                outputstring += line + "\r\n";
                        }
                        else if (line.ToLower().Contains("base_name"))
                        {
                            int number = int.Parse(getNumber.Match(line).ToString());
                            if (data.BaseDatasSOV.Keys.Contains(number))
                            {
                                outputstring += data.BaseDatasSOV[number].Name + "\r\n";
                                LogContent.Add(new SovBackupsPreparationResult("Base name " + number + " updated in $config.dat", GlobalData.SovLogContentInfoTypes.Information));
                            }
                            else
                                outputstring += line + "\r\n";
                        }
                        else if (line.ToLower().Contains("base_type"))
                        {
                            int number = int.Parse(getNumber.Match(line).ToString());
                            if (data.BaseDatasSOV.Keys.Contains(number))
                            {
                                outputstring += data.BaseDatasSOV[number].Type + "\r\n";
                                LogContent.Add(new SovBackupsPreparationResult("Base type " + number + " updated in $config.dat", GlobalData.SovLogContentInfoTypes.Information));
                            }
                            else
                                outputstring += line + "\r\n";
                        }
                        //else if (line.ToLower().Contains(";fold user globals"))
                        //{
                        //    outputstring += line + "\r\n";
                        //    foreach (var global in data.GlobalPointsSOV)
                        //    {
                        //        outputstring += global.Value.Content + "\r\n";
                        //        LogContent.Add(new SovBackupsPreparationResult("User global " + global.Key + " updated in $config.dat", GlobalData.SovLogContentInfoTypes.Information));
                        //    }
                        //}
                        else
                            outputstring += line + "\r\n";
                    }
                    reader.Close();
                    ZipArchiveEntry readmeEntry = archive.CreateEntry(foundEntry.FullName);
                    using (StreamWriter writer = new StreamWriter(readmeEntry.Open()))
                    {
                        string[] alllines = outputstring.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        foreach (var line in alllines)
                            writer.WriteLine(line);
                        writer.Close();
                    }
                    archive.CreateEntry("KRC\\R1\\BMW_Utilities\\UserVariables.dat");
                    using (StreamWriter writer = new StreamWriter(archive.Entries.Single(x => x.FullName.ToLower().Contains("uservariables.dat")).Open()))
                    {
                        string[] alllines = userVarsContentNew.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        foreach (var line in alllines)
                            writer.WriteLine(line);
                        writer.Close();
                    }
                    archive.Dispose();
                }
                zipToOpen.Close();
            }
        }

        private string CreateUserVars(DataForSOV data)
        {
            string result = string.Empty;
            StringReader reader = new StringReader(Properties.Resources.UserVariables);
            while (true)
            {
                string line = reader.ReadLine();
                if (line == null)
                    break;
                if (!line.Contains("{LINES}"))
                {
                    if (!line.Contains("{ADD_OR_NOT}"))
                        result += line.Replace("{USER_NAME}", ConfigurationManager.AppSettings["Ersteller"]).Replace("{DATE}", DateTime.Now.ToString("yyyy.MM.dd"))+ "\r\n";
                    else
                    {
                        if (data.GlobalPointsSOV.Count > 0)
                            result += line.Replace("{ADD_OR_NOT}", "").Replace("{USER_NAME}", ConfigurationManager.AppSettings["Ersteller"]).Replace("{DATE}", DateTime.Now.ToString("yyyy.MM.dd")) + "\r\n";
                    }
                }
                else
                    foreach (var item in data.GlobalPointsSOV)
                        result += item.Value.Content + "\r\n";
            }
            return result;
        }

        private void ExtractToTempConfigFile(string file, ZipArchiveEntry entry)
        {
            string directory = Path.GetDirectoryName(CommonGlobalData.ConfigurationFileName);
            if (!Directory.Exists(directory + "\\TempConfigFile"))
                Directory.CreateDirectory(directory + "\\TempConfigFile");
            if (File.Exists(directory + "\\TempConfigFile\\" + file))
                File.Delete(directory + "\\TempConfigFile\\" + file);
            entry.ExtractToFile(directory + "\\TempConfigFile\\" + file);
        }

        private DataForSOV GetDataForSOV(string selectedDir)
        {
            try
            {
                IDictionary<int, ToolDataSOV> tooldatas = new Dictionary<int, ToolDataSOV>();
                IDictionary<int, BaseDataSOV> basedatas = new Dictionary<int, BaseDataSOV>();
                IDictionary<int, HomeSOV> homes = new Dictionary<int, HomeSOV>();
                IDictionary<string, GlobalPointsSOV> globals = new Dictionary<string, GlobalPointsSOV>();
                string inputdatapath = "", copytoconfigpath = "";
                Regex getPointName = new Regex(@"^[a-zA-Z0-9_-]*", RegexOptions.IgnoreCase);
                List<string> files = Directory.GetFiles(selectedDir, "*.*", SearchOption.AllDirectories).ToList();
                foreach (string file in files.Where(x => x.ToLower().ContainsAny(new string[] { "copy_to_$config", "copy_to_uservariables" })))
                {
                    copytoconfigpath = file;
                    break;
                }
                foreach (string file in files.Where(x => x.ToLower().Contains("inputdata")))
                {
                    inputdatapath = file;
                    break;
                }
                if (!String.IsNullOrEmpty(copytoconfigpath))
                {
                    StreamReader reader = new StreamReader(copytoconfigpath);
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        Regex currentRegex = new Regex(@"", RegexOptions.IgnoreCase);
                        if (line.ToLower().Contains("fdat") || line.ToLower().Contains("e6pos") || line.ToLower().Contains("e6axis"))
                            currentRegex = new Regex(@"((?<=(E6AXIS|E6POS|FDAT)\s+)).*", RegexOptions.IgnoreCase);
                        else
                            currentRegex = new Regex(@"^.*", RegexOptions.IgnoreCase);
                        string currentLine = currentRegex.Match(line).ToString();
                        string pointName = getPointName.Match(currentLine).ToString();
                        if (!string.IsNullOrEmpty(pointName))
                            globals.Add(pointName, GetCurrentLine(pointName, currentLine));
                    }
                    reader.Close();
                }
                Regex isHomeRegex = new Regex(@"XHOME\d+", RegexOptions.IgnoreCase);
                if (!String.IsNullOrEmpty(inputdatapath))
                {
                    Regex getNumber = new Regex(@"(?<=^.*\[)\d+", RegexOptions.IgnoreCase);
                    StreamReader reader = new StreamReader(inputdatapath);
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (isHomeRegex.IsMatch(line))
                        {
                            Regex homeNum = new Regex(@"(?<=XHOME)\d", RegexOptions.IgnoreCase);
                            int number = int.Parse(homeNum.Match(line).ToString());
                            homes.Add(number, new HomeSOV(number, line));
                        }
                        if (line.ToLower().Contains("tool_data"))
                        {
                            int number = int.Parse(getNumber.Match(line).ToString());
                            if (!tooldatas.Keys.Contains(number))
                                tooldatas.Add(number, new ToolDataSOV());
                            tooldatas[number].Content = line;
                        }
                        if (line.ToLower().Contains("tool_name"))
                        {
                            int number = int.Parse(getNumber.Match(line).ToString());
                            if (!tooldatas.Keys.Contains(number))
                                tooldatas.Add(number, new ToolDataSOV());
                            tooldatas[number].Name = line;
                        }
                        if (line.ToLower().Contains("tool_type"))
                        {
                            int number = int.Parse(getNumber.Match(line).ToString());
                            if (!tooldatas.Keys.Contains(number))
                                tooldatas.Add(number, new ToolDataSOV());
                            tooldatas[number].Type = line;
                        }
                        if (line.ToLower().Contains("base_data"))
                        {
                            int number = int.Parse(getNumber.Match(line).ToString());
                            if (!basedatas.Keys.Contains(number))
                                basedatas.Add(number, new BaseDataSOV());
                            basedatas[number].Content = line;
                        }
                        if (line.ToLower().Contains("base_name"))
                        {
                            int number = int.Parse(getNumber.Match(line).ToString());
                            if (!basedatas.Keys.Contains(number))
                                basedatas.Add(number, new BaseDataSOV());
                            basedatas[number].Name = line;
                        }
                        if (line.ToLower().Contains("base_type"))
                        {
                            int number = int.Parse(getNumber.Match(line).ToString());
                            if (!basedatas.Keys.Contains(number))
                                basedatas.Add(number, new BaseDataSOV());
                            basedatas[number].Type = line;
                        }
                        if (line.ToLower().Contains("fg_"))
                        {
                            Regex currentRegex = new Regex(@"", RegexOptions.IgnoreCase);
                            if (line.ToLower().Contains("fdat") || line.ToLower().Contains("e6pos") || line.ToLower().Contains("e6axis"))
                                currentRegex = new Regex(@"((?<=(E6POS|FDAT)\s+)).*", RegexOptions.IgnoreCase);
                            else
                                currentRegex = new Regex(@"^.*", RegexOptions.IgnoreCase);
                            string currentLine = currentRegex.Match(line).ToString();
                            string pointName = getPointName.Match(currentLine).ToString();
                            if (!string.IsNullOrEmpty(pointName))
                            {
                                if (!globals.Keys.Contains(pointName))
                                    globals.Add(pointName, GetCurrentLine(pointName, currentLine));
                                else
                                    MessageBox.Show("Double declaration of " + pointName, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                        if (line.ToLower().Contains("xg_"))
                        {
                            Regex currentRegex = new Regex(@"", RegexOptions.IgnoreCase);
                            if (line.ToLower().Contains("fdat") || line.ToLower().Contains("e6pos") || line.ToLower().Contains("e6axis"))
                                currentRegex = new Regex(@"((?<=(E6POS|FDAT)\s+)).*", RegexOptions.IgnoreCase);
                            else
                                currentRegex = new Regex(@"^.*", RegexOptions.IgnoreCase);
                            string currentLine = currentRegex.Match(line).ToString();
                            string pointName = getPointName.Match(currentLine).ToString();
                            if (!string.IsNullOrEmpty(pointName))
                            {
                                if (!globals.Keys.Contains(pointName))
                                    globals.Add(pointName, GetCurrentLine(pointName, currentLine));
                                else
                                    MessageBox.Show("Double declaration of " + pointName, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                    }
                    reader.Close();
                }
                DataForSOV result = new DataForSOV(tooldatas, basedatas, homes, globals);

                return result;
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                return null;
            }
        }

        private GlobalPointsSOV GetCurrentLine(string pointname, string currentLine)
        {
            string func = "";
            string firstLetter = currentLine.Substring(0, 1);
            if (firstLetter.ToLower() == "x")
                if (currentLine.ToLower().Contains("a1 "))
                    func = "E6AXIS";
                else
                    func = "E6POS";
            else
                func = "FDAT";
            return new GlobalPointsSOV(pointname, "DECL GLOBAL " + func + " " + currentLine);
        }

        private void RemoveEntry(string selectedBackup, string entrytodelete)
        {
            using (FileStream zipToOpen = new FileStream(selectedBackup, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    List<ZipArchiveEntry> entries = new List<ZipArchiveEntry>();
                    foreach (var item in archive.Entries)
                    {
                        if (item.FullName.ToLower().Contains(entrytodelete.ToLower()))
                            entries.Add(item);
                    }
                    foreach (var entry in entries)
                    {
                        ZipArchiveEntry ent = archive.GetEntry(entry.FullName);
                        if (ent != null)
                        {
                            DeleteEntry(ent);
                        }
                    }
                    archive.Dispose();
                }
                zipToOpen.Close();

            }
        }

        private void CopyToDir(string destinationDir, string selectedBackup, string selectedDir, bool removeAllContent, string destDirExtra = "")
        {
            List<string> files = new List<string>();
            if (!Directory.Exists(selectedDir + "\\" + destinationDir))
                return;
            files = Directory.GetFiles(selectedDir + "\\" + destinationDir).ToList();

            using (FileStream zipToOpen = new FileStream(selectedBackup, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    if (removeAllContent)
                    {
                        List<ZipArchiveEntry> entries = new List<ZipArchiveEntry>();
                        foreach (var item in archive.Entries)
                        {
                            if (item.FullName.ToLower().Contains(@"\" + destinationDir.ToLower() + @"\") || item.FullName.ToLower().Contains(@"/" + destinationDir.ToLower() + @"/"))
                            {
                                entries.Add(item);
                            }
                        }

                        foreach (var entry in entries)
                        {
                            ZipArchiveEntry ent = archive.GetEntry(entry.FullName);
                            if (ent != null && !ent.Name.ToLower().Contains("select_abortprog") && !ent.Name.ToLower().Contains("select_homeprog"))
                            {
                                DeleteEntry(ent);
                            }
                        }
                    }

                    foreach (var file in files)
                    {
                        if (string.IsNullOrEmpty(destDirExtra))
                        {
                            ZipArchiveEntry entry = archive.GetEntry("KRC\\R1\\" + destinationDir + "\\" + Path.GetFileName(file));
                            if (entry != null)
                            {
                                DeleteEntry(entry);
                            }
                            CopyEntry(file, "KRC\\R1\\" + destinationDir + "\\" + Path.GetFileName(file), archive);
                        }
                        else
                        {
                            ZipArchiveEntry entry = archive.GetEntry("KRC\\R1\\" + destDirExtra + "\\" + Path.GetFileName(file));
                            if (entry != null)
                                entry.Delete();
                            CopyEntry(file, "KRC\\R1\\" + destDirExtra + "\\" + Path.GetFileName(file), archive);
                        }
                    }
                    archive.Dispose();
                }
                zipToOpen.Close();
            }
        }

        private void CopyEntry(string file, string path, ZipArchive archive)
        {
            if (Path.GetExtension(file).ToLower() == ".src" || Path.GetExtension(file).ToLower() == ".dat")
            {
                ZipArchiveEntry readmeEntry = archive.CreateEntryFromFile(file, path);
                LogContent.Add(new SovBackupsPreparationResult("Entry " + readmeEntry.FullName + " copied to backup!", GlobalData.SovLogContentInfoTypes.Information));
            }
        }

        private void DeleteEntry(ZipArchiveEntry entry)
        {
            LogContent.Add(new SovBackupsPreparationResult("Entry " + entry.FullName + " deleted!", GlobalData.SovLogContentInfoTypes.Information));
            entry.Delete();
        }
        #endregion
    }
}
