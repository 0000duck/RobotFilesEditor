using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using RobotFilesEditor.Model.DataInformations;
using static RobotFilesEditor.Model.DataInformations.FileValidationData;
using System.Data;
using System.Windows.Forms;
using RobotFilesEditor.Dialogs;
using RobotFilesEditor.Dialogs.IBUSSegmentName;
using RobotFilesEditor.Dialogs.SelectRobotType;
using RobotFilesEditor.Dialogs.ChangeName;
using RobotFilesEditor.Dialogs.TypeCollisionComment;
using RobotFilesEditor.Dialogs.SelectCollision;
using RobotFilesEditor.Dialogs.AppTypeSelect;
using RobotFilesEditor.Dialogs.RenameBase;
using RobotFilesEditor.Dialogs.RenameTool;
using RobotFilesEditor.Dialogs.SelectLoadVar;
using RobotFilesEditor.Dialogs.NameRobot;
using RobotFilesEditor.Dialogs.SelectJob;
using System.Reflection;
using System.Globalization;
using Microsoft.VisualBasic.FileIO;
using RobotFilesEditor.Model.DataOrganization;
using System.Diagnostics;
using CommonLibrary;
using System.Collections.ObjectModel;

namespace RobotFilesEditor.Model.Operations
{
    public static class SrcValidator
    {
        public static string language;
        public static IDictionary<string, Dats> UnusedDats { get; set; }
        public static IDictionary<string, Dats> UsedDats { get; set; }
        public static List<string> AlreadyContain { get; set; }
        public static List<ResultInfo> GlobalFiles { get; set; }
        public static bool CopiedFiles { get; set; }
        public static string logFileContent;
        static IDictionary<int, List<string>> collisionsWithDescription;
        static IDictionary<int, string> jobsWithDescription;
        static List<CollisionWithoutDescr> collisions;
        static string roboter;
        public static List<string> UnclassifiedPaths { get; set; }
        public static ValidationData DataToProcess { get; set; }
        public static IDictionary<string, string> Result { get; set; }
        private static bool UnusedDataPresent { get; set; }
        private static string matchRoboter;
        private static bool fillDescrs;

        internal static bool ValidateFile(IDictionary<string, string> files)
        {
            if (GlobalData.ControllerType != "FANUC")
            {
                GlobalData.loadVars = new Dictionary<int, string>();
                GlobalData.LocalHomesFound = false;
                matchRoboter = string.Empty;
                GlobalData.HasToolchager = false;
                if (DataToProcess.SrcFiles.Count == 0)
                    return false;
                GlobalData.GlobalDatsList = new List<string>();
                GlobalData.InputDataList = new List<string>();
                IDictionary<string, string> srcFiles = DataToProcess.SrcFiles;
                srcFiles = FilterSRC(srcFiles);
                GlobalData.isWeldingRobot = FindWelding(srcFiles);
                GlobalData.isHandlingRobot = FindHandling(srcFiles);
                string opereationName = DataToProcess.OpereationName;
                List<string> datFiles = DataToProcess.DatFiles;
                GlobalData.Roboter = roboter;
                logFileContent = "";
                if (srcFiles == null | datFiles == null | !DetectDuplicates(srcFiles))
                    return false;
                GlobalData.ToolchangerType = DetectApp(srcFiles.Keys, "a02", "b02", new string[3] { "dock", "dockdresser", "_tch_" }, "toolchanger");
                GlobalData.WeldingType = DetectApp(srcFiles.Keys, "a04", "a05", new string[3] { "spot", "notUsed", "_swx_" }, "welding");
                //GlobalData.RivetingType = DetectApp(srcFiles.Keys, "a13", "c13", new string[3] { "rivet", "notUsed", "notUsed" }, "riveting");
                IDictionary<string, List<string>> resultSrcFiles = DivideToFolds(srcFiles);
                //TEMP
                List<string> GlobalFDATs = GetGlobalFDATs(GlobalData.AllFiles);
                //if (GlobalFDATs.Count > 0)

                resultSrcFiles = ClearEnters(resultSrcFiles);
                resultSrcFiles = ClearHeader(resultSrcFiles);
                GlobalData.Roboter = roboter;
                FillListOfOpsInGlobal(resultSrcFiles);
                SetFillDescr();
                if (GlobalData.ControllerType != "KRC4 Not BMW")
                {
                    collisions = GetCollisions(srcFiles);
                    collisionsWithDescription = GetCollisionsWithDescription(srcFiles);
                    jobsWithDescription = GetJobsWithDescr(resultSrcFiles);
                }//TEMP
                FindDatFiles(srcFiles, datFiles);
                FindUnusedDataInDatFiles(resultSrcFiles, datFiles);
                CheckToolsAndBases(resultSrcFiles, datFiles);
                resultSrcFiles = CorrectFoldCommentIfZone0(resultSrcFiles);
                resultSrcFiles = RemoveHash(resultSrcFiles);
                FindRetrClo(resultSrcFiles);
                CheckChkAxisPos(resultSrcFiles);
                FindLocalHomes(datFiles);
                if (GlobalData.ControllerType != "KRC4 Not BMW")
                {
                    GlobalData.loadVars = FindLoadVars(resultSrcFiles);
                    resultSrcFiles = CorrectCollisionComments(resultSrcFiles);
                    resultSrcFiles = CorrectJobsComments(resultSrcFiles);
                    FillSrcPaths(resultSrcFiles);
                    CheckOpeningAndClosingCommand(resultSrcFiles);
                    resultSrcFiles = AddHeader(resultSrcFiles, false);
                    if (GlobalData.CheckOrder)
                        resultSrcFiles = CheckOrder(resultSrcFiles);
                    resultSrcFiles = CheckToolNames(resultSrcFiles);
                    resultSrcFiles = CheckBaseNames(resultSrcFiles);
                    resultSrcFiles = CheckContinous(resultSrcFiles);
                    FindMaintenancePaths(resultSrcFiles);
                    FindDockableTools(resultSrcFiles);
                    FindUserBits(resultSrcFiles);

                }
                else
                {
                    resultSrcFiles = CheckToolNames(resultSrcFiles);
                    resultSrcFiles = CheckBaseNames(resultSrcFiles);
                }
                //CommonLibrary.CommonMethods.CreateLogFile(logFileContent, "\\log.txt");
                if (GlobalData.ControllerType == "KRC4" && resultSrcFiles.Any(x => Path.GetFileNameWithoutExtension(x.Key).ToLower() == "a01_braketest"))
                    resultSrcFiles = AddnAnswerToBrakeTest(resultSrcFiles);
                Result = AddSpaces(resultSrcFiles);
            }
            else
            {
                Result = new Dictionary<string, string>();
                string log;
                FANUC.FanucFilesValidator fanucFiles = new FANUC.FanucFilesValidator(files.Keys.ToList(), out log);
                GlobalData.LaserType = DetectApp(fanucFiles.FilesList, "b15", "b16", new string[3] { "laser", "notUsed", "notUsed" }, "laser");
                foreach (var file in fanucFiles.FilesAndContent)
                {
                    Result.Add(file.Key, FANUC.FanucFilesValidator.GetFileContenetFANUC(file.Value));
                    logFileContent = log;
                }
                //files = FANUC.FanucMethods.CheckCollsOpe
            }

            return true;

        }

        private static void CheckChkAxisPos(IDictionary<string, List<string>> resultSrcFiles)
        {
            Regex findCentralPosRegex = new Regex(@"(?<=^\s*((WAIT\s+FOR)|(IF))\s+CHK_AXIS_POS\s*\(\s*).*(?=\))", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Regex findMovePointRegex = new Regex(@"(?<=^\s*(PTP|LIN)\s+)[\w_-]+", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string currentLogFileContent = string.Empty;
            foreach (var file in resultSrcFiles)
            {
                bool firstPointFound = false, chkAxisPosFound = false;
                string chkAxisPointName = string.Empty, firstPointName = string.Empty;
                foreach(var line in file.Value)
                {
                    if (findCentralPosRegex.IsMatch(line))
                    {
                        chkAxisPosFound = true;
                        chkAxisPointName = findCentralPosRegex.Match(line).ToString();
                    }
                    if (findMovePointRegex.IsMatch(line))
                    {
                        firstPointFound = true;
                        firstPointName = findMovePointRegex.Match(line).ToString();
                        if (firstPointName.ToLower().Trim() == "xhome1" || firstPointName.ToLower().Trim() == "xhome2")
                            break;
                    }
                    if (firstPointFound && !chkAxisPosFound)
                    {
                        currentLogFileContent += "Motion to central pos without CHK_AXIS_POS in path: " + Path.GetFileNameWithoutExtension(file.Key) + "\r\n";
                        break;
                    }
                    if (firstPointFound && chkAxisPosFound)
                    {
                        if (chkAxisPointName.ToLower().Trim() != firstPointName.ToLower().Trim())
                            currentLogFileContent += "Central pos name is different than CHK_AXIS_POS argument in path: " + Path.GetFileNameWithoutExtension(file.Key) + "\r\n";
                        break;
                    }
                }
            }
            if (!string.IsNullOrEmpty(currentLogFileContent))
            {
                MessageBox.Show("Inconsistency in CHK_AXIS_POS for central positions found.\r\nSee log file for details", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                logFileContent += currentLogFileContent;
            }
        }

        private static IDictionary<int, string> FindLoadVars(IDictionary<string, List<string>> inputFiles)
        {
            IDictionary<int, List<string>> loadVarDict = new SortedDictionary<int, List<string>>();
            foreach (var file in inputFiles)
            {
                foreach (var command in file.Value)
                {
                    if (command.ToLower().Contains("loadvariante"))
                    {
                        Regex loadVarNumRegex = new Regex(@"(?<=LoadVariante\s*(\:|\=)\s*)\d+", RegexOptions.IgnoreCase);
                        Regex loadVarNameRegex = new Regex(@"(?<=LoadVariante\s*\:\s*\d+\s+'\s*)\w+", RegexOptions.IgnoreCase);
                        int loadVarNum = int.Parse(loadVarNumRegex.Match(command).ToString());
                        string loadVarName = loadVarNameRegex.Match(command).ToString();
                        if (loadVarName.Length > 20)
                        {
                            loadVarName = loadVarName.Substring(0, 20);
                            MessageBox.Show("Loadvar's " + loadVarNum + " name is longer than 20 chars! It will be reduced to "+ loadVarName + ".", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);                            
                        }
                        //while (loadVarName.Length < 20)
                        //{
                        //    loadVarName += " ";
                        //}
                        if (!loadVarDict.Keys.Contains(loadVarNum))
                            loadVarDict.Add(loadVarNum, new List<string>() { loadVarName });
                        else
                        {
                            if (!loadVarDict[loadVarNum].Contains(loadVarName))
                                loadVarDict[loadVarNum].Add(loadVarName);
                        }
                    }
                }
            }
            IDictionary<int, string> result = new Dictionary<int, string>();
            foreach (var loadVar in loadVarDict)
            {
                if (loadVar.Value.Count == 1)
                    result.Add(loadVar.Key, loadVar.Value[0]);
                else
                {
                    SelectLoadVarViewModel vm = new SelectLoadVarViewModel(loadVar);
                    SelectLoadVar sW = new SelectLoadVar(vm);
                    var dialogResult = sW.ShowDialog();
                    result.Add(loadVar.Key, loadVar.Value[vm.SelectedIndex]);
                }
            }
            
            return result;
        }

        internal static string RemoveLocalHomes(string value)
        {
            string result = string.Empty;
            List<string> listStrLineElements = value.Split('\n').ToList();
            foreach(var line in listStrLineElements)
            {
                if (!((line.ToLower().Contains("decl") && line.ToLower().Contains("e6axis") && line.ToLower().Contains("xhome")) || (GlobalData.ControllerType=="KRC4 Not BMW" && line.Length >= 3 && (line.Substring(0,3) == ";* " || line.Substring(0, 3) == ";**"))))
                    result += line + "\n";           
                
            }
            return result;
        }

        private static string DetectApp(ICollection<string> keys, string variantA, string variantB, string[] type, string label)
        {
            if (keys.Any(x => x.ToLower().Contains(type[0]) && !x.ToLower().Contains(type[1])) || keys.Any(x => x.ToLower().Contains(type[2].Replace("x","p"))) || keys.Any(x => x.ToLower().Contains(type[2].Replace("x", "i"))))
            {
                if (keys.Any(x => Path.GetFileNameWithoutExtension(x).ToLower().Contains(variantA)))
                    return variantA.ToUpper();
                else if (keys.Any(x => Path.GetFileNameWithoutExtension(x).ToLower().Contains(variantB)))
                    return variantB.ToUpper();
                else
                {
                    var vm = new AppTypeSelectViewModel(variantA, variantB, "Select "+label+" type");
                    AppTypeSelect sW = new AppTypeSelect(vm);
                    var dialogResult = sW.ShowDialog();
                    return vm.ResultType;
                }
            }
            else
                return string.Empty;

        }

        private static IDictionary<string, List<string>> AddnAnswerToBrakeTest(IDictionary<string, List<string>> resultSrcFiles)
        {
            List<string> resultList = new List<string>();
            var result = new Dictionary<string, List<string>>();
            var element = resultSrcFiles.Where(x => Path.GetFileNameWithoutExtension(x.Key).ToLower() == "a01_braketest").FirstOrDefault();
            foreach (var line in element.Value)
            {
                if (line.ToLower().Replace(" ", "").Contains("intnanswer"))
                    return resultSrcFiles;
                if (line.ToLower().Replace(" ", "").Contains(";foldini"))
                    resultList.Add(";FOLD DECLARATIONS\r\nINT nAnswer\r\n;ENDFOLD\r\n");
                resultList.Add(line);
            }
            result.Add(element.Key, resultList);
            foreach (var item in resultSrcFiles.Where(x => Path.GetFileNameWithoutExtension(x.Key).ToLower() != "a01_braketest"))
                result.Add(item.Key, item.Value);
            return result;
        }

        private static IDictionary<string, string> FilterSRC(IDictionary<string, string> srcFiles)
        {
            IDictionary<string, string> result = srcFiles;
            IDictionary<string, string> copyOfSrcFiles = new Dictionary<string, string>();
            foreach (var item in result)
                copyOfSrcFiles.Add(item);
            foreach (var item in ConfigurationManager.AppSettings["SystemFile" + GlobalData.ControllerType.Replace(" ", "_")].Split(',').ToArray())
            {
                foreach (var item2 in copyOfSrcFiles.Where(x => Path.GetFileName(x.Key.ToLower()).Contains(item)))
                    result.Remove(item2);
            }
            return result;

        }

        private static void FindLocalHomes(List<string> datFiles)
        {
            List<string> proceduresWithLocalHomes = new List<string>();
            Regex isHome = new Regex(@"E6AXIS\s+XHOME\d+", RegexOptions.IgnoreCase);
            foreach (var datfile in datFiles)
            {
                StreamReader reader = new StreamReader(datfile);
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (isHome.IsMatch(line))
                        proceduresWithLocalHomes.Add(Path.GetFileNameWithoutExtension(datfile));

                }
                reader.Close();
            }
            if (proceduresWithLocalHomes.Count > 0)
            {
                GlobalData.LocalHomesFound = true;
                string starter = "Local Home definitions found in procedures:\r\n";
                string proc = string.Empty;
                foreach (string procedure in proceduresWithLocalHomes)
                {
                    proc += procedure + "\r\n";
                }
                string ender = "Download paths again with correct template!";
                MessageBox.Show(starter + proc + ender, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                logFileContent += starter + proc + ender + "\r\n\r\n";
            }
        }

        private static IDictionary<string, List<string>> CorrectFoldCommentIfZone0(IDictionary<string, List<string>> resultSrcFiles)
        {
            IDictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            foreach (var file in resultSrcFiles)
            {
                List<string> currentFileFolds = new List<string>();
                foreach (var fold in file.Value)
                {
                    if (fold.ToLower().Contains("ptp ") || fold.ToLower().Contains("lin "))
                    {
                        Regex isCdisRegex = new Regex(@"(PTP|LIN)\s+[a-zA-Z0-9_\-]*\s+C_(DIS|PTP)", RegexOptions.IgnoreCase);

                        if (isCdisRegex.IsMatch(fold))
                        {
                            Regex isNormalMoveInstruction = new Regex(@";\s*FOLD\s+(PTP|LIN)\s+[a-zA-Z0-9_\-]*\s+", RegexOptions.IgnoreCase);
                            if (isNormalMoveInstruction.IsMatch(fold))
                            {
                                if (!fold.ToLower().Contains(" cont "))
                                {
                                    string beginPath = isNormalMoveInstruction.Match(fold).ToString();
                                    string correctedFold = fold.Replace(beginPath, beginPath + "CONT ");
                                    currentFileFolds.Add(correctedFold);
                                }
                                else
                                    currentFileFolds.Add(fold);
                            }
                            else
                                currentFileFolds.Add(fold);
                        }
                        else
                            currentFileFolds.Add(fold);
                    }
                    else
                        currentFileFolds.Add(fold);
                }
                result.Add(file.Key, currentFileFolds);
            }

            return result;
        }

        private static void FindUserBits(IDictionary<string, List<string>> resultSrcFiles)
        {
            Userbits result = new Userbits(new SortedDictionary<int, List<string>>(), new SortedDictionary<int, List<string>>());
            foreach (var file in resultSrcFiles)
            {
                foreach (string command in file.Value.Where((x => x.ToLower().Contains("wait for") && x.ToLower().Contains("$in[") || x.ToLower().Contains("$out["))))
                {
                    if (command.ToLower().Contains("wait for"))
                    {
                        int number = 0;
                        Regex getUserBit = new Regex(@"(?<=\$IN\[\s*)\d+", RegexOptions.IgnoreCase);
                        if (getUserBit.Match(command).ToString() != "")
                            number = int.Parse(getUserBit.Match(command).ToString());
                        else
                        {
                            getUserBit = new Regex(@"(?<=\$in\[\s*)[a-zA-Z0-9_]*", RegexOptions.IgnoreCase);
                            foreach (var signal in getUserBit.Matches(command))
                            {
                                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings[signal.ToString() + GlobalData.ControllerType.Replace(" ", "_")]))
                                {
                                    if (!result.Inputs.Keys.Contains(int.Parse(ConfigurationManager.AppSettings[signal.ToString() + GlobalData.ControllerType.Replace(" ", "_")])))
                                        result.Inputs.Add(int.Parse(ConfigurationManager.AppSettings[signal.ToString() + GlobalData.ControllerType.Replace(" ", "_")]), new List<string>());
                                }
                                else
                                    MessageBox.Show("Signal " + signal.ToString() + " name is invalid. Check spelling", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        if (!result.Inputs.Keys.Contains(number))
                        {
                            result.Inputs.Add(number, new List<string>());
                        }
                        Regex getInputBitComment = new Regex(@"(?<=IN\s+" + number.ToString() + @"\s+')[a-zA-Z0-9\s_\.]*", RegexOptions.IgnoreCase);
                        string comment = getInputBitComment.Match(command).ToString();
                        if (!result.Inputs[number].Contains(comment))
                            result.Inputs[number].Add(comment);
                    }

                    if (command.ToLower().Contains("$out[") && !command.ToLower().Contains("typbit"))
                    {
                        int number = 0;
                        Regex getUserBit = new Regex(@"(?<=\$OUT\[\s*)\d+", RegexOptions.IgnoreCase);
                        if (getUserBit.Match(command).ToString() != "")
                            number = int.Parse(getUserBit.Match(command).ToString());
                        else
                        {
                            getUserBit = new Regex(@"(?<=\$OUT\[\s*)[a-zA-Z0-9_]*", RegexOptions.IgnoreCase);
                            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings[getUserBit.Match(command).ToString() + GlobalData.ControllerType.Replace(" ", "_")]))
                                number = int.Parse(ConfigurationManager.AppSettings[getUserBit.Match(command).ToString() + GlobalData.ControllerType.Replace(" ", "_")]);
                        }
                        if (!result.Outputs.Keys.Contains(number))
                        {
                            result.Outputs.Add(number, new List<string>());
                        }
                        //Regex getOutputBitComment = new Regex(@"(?<=3:)[a-zA-Z0-9\s_]*(?=,)", RegexOptions.IgnoreCase);
                        Regex getOutputBitComment = new Regex(@"(?<=')[a-zA-Z0-9\s_.]*(?=')", RegexOptions.IgnoreCase);
                        string comment = getOutputBitComment.Match(command).ToString();
                        if (!result.Outputs[number].Contains(comment))
                            result.Outputs[number].Add(comment);
                    }
                }
            }
            GlobalData.SignalNames = result;
        }

        private static void FindRetrClo(IDictionary<string, List<string>> resultSrcFiles)
        {
            IDictionary<string, bool> result = new Dictionary<string, bool>();
            foreach (var file in resultSrcFiles)
            {

                if (!result.Keys.Contains(file.Key))
                    result.Add(file.Key, false);
                foreach (string command in file.Value)
                {
                    if (command.ToLower().Contains("retr clo") || command.ToLower().Contains("retr opn"))
                    {
                        result[file.Key] = true;
                        break;
                    }
                }
            }
            string foundErrors = "";
            foreach (var item in result.Where(x => x.Value == true))
            {
                foundErrors = item.Key + "\r\n";
            }
            if (foundErrors != "")
            {
                string message = "RETR CLO commands found in files:\r\n" + foundErrors + "Make sure gun position in PS is not set in via and home positions!";
                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                logFileContent += message + "\r\n\r\n";
            }
        }


        private static IDictionary<string, List<string>> RemoveHash(IDictionary<string, List<string>> resultSrcFiles)
        {
            IDictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            foreach (var file in resultSrcFiles)
            {
                if (!result.Keys.Contains(file.Key))
                    result.Add(file.Key, new List<string>());
                foreach (string command in file.Value)
                {
                    if (!string.IsNullOrEmpty(command.Trim()))
                        if (!(command.Contains(";# ") || (command.Contains(";SIM") || command.ToLower().Contains(";#end_header") || command.ToLower().Contains(";#start_trailer")) || command.Trim().Replace(" ", "").Substring(0, 1) == "#"))
                            result[file.Key].Add(command);
                }
            }
            return result;
        }


        public static void CheckDestinationEmpty(string destFolder)
        {
            if (!Directory.Exists(destFolder))
            {
                Directory.CreateDirectory(destFolder);
                MessageBox.Show("Destination folder has been created", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            string[] files = Directory.GetFiles(destFolder);
            string[] subfolders = Directory.GetDirectories(destFolder);
            if (files.Length > 0 || subfolders.Length > 0)
            {
                DialogResult dialogResult = MessageBox.Show("Destination folder not empty, this can cause errors.\r\nRemove content from destination folder?\r\nOrgs and GripperConfig folders will not necessarily be deleted", "Clear destination folder?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes)
                {
                    bool copyOrgs = CopyFilesToAppData("Orgs", destFolder);
                    bool copyGripper = CopyFilesToAppData("GripperConfig", destFolder);
                    FileSystem.DeleteDirectory(destFolder, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
                    Directory.CreateDirectory(destFolder);
                    if (copyOrgs)
                        RetrieveFolder("Orgs",destFolder);
                    if (copyGripper)
                        RetrieveFolder("GripperConfig", destFolder);
                }
            }
        }

        private static void RetrieveFolder(string folder, string destFolder)
        {
            Directory.CreateDirectory(Path.Combine(destFolder, folder));
            string workdir = Path.Combine(Path.GetDirectoryName(CommonLibrary.CommonGlobalData.ConfigurationFileName),folder);
            foreach (var file in Directory.GetFiles(workdir,"*.*"))
            {
                File.Copy(file, Path.Combine(destFolder, folder, Path.GetFileName(file)));
            }
        }

        private static bool CopyFilesToAppData(string folderName, string destFolder)
        {
            bool result = false;
            List<string> founfolders = new List<string>();
            Directory.GetDirectories(destFolder).ToList().ForEach(x => founfolders.Add(x));
            if (founfolders.Any(x => x.Split('\\')[(x.Split('\\').Length - 1)].ToLower() == folderName.ToLower()))
            {
                DialogResult dialogResult = MessageBox.Show("Remove " + folderName+ " folder?", "Remove folder?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes)
                    return false;
                string workdir = Path.GetDirectoryName(CommonLibrary.CommonGlobalData.ConfigurationFileName);
                result = true;

                if (Directory.GetDirectories(workdir).Any(x => x.Contains(folderName)))
                {
                    Directory.GetFiles(Path.Combine(workdir, folderName)).ToList().ForEach(x => File.Delete(x));
                    Directory.Delete(Path.Combine(workdir, folderName));
                }
                Directory.CreateDirectory(Path.Combine(workdir, folderName));
                var dirFromCopy = founfolders.Where(x => x.Split('\\')[(x.Split('\\').Length - 1)].ToLower() == folderName.ToLower()).First();
                List<string> allFiles = Directory.GetFiles(dirFromCopy,"*.*").ToList();
                foreach (var file in allFiles)
                    File.Copy(file, Path.Combine(workdir, folderName, Path.GetFileName(file)));
            }

            return result;
        }

        private static bool FindWelding(IDictionary<string, string> srcFiles)
        {
            foreach (var item in srcFiles.Where(x => x.Key.ToLower().Contains("spot")))
                return true;
            return false;
        }

        private static bool FindHandling(IDictionary<string, string> srcFiles)
        {
            foreach (var item in srcFiles.Where(x => x.Key.ToLower().Contains("pick") || x.Key.ToLower().Contains("drop")))
                return true;
            return false;
        }

        private static void FillSrcPaths(IDictionary<string, List<string>> resultSrcFiles)
        {
            GlobalData.SrcPathsAndJobs = new Dictionary<string, int>();
            foreach (var file in resultSrcFiles)
            {
                foreach (string command in file.Value.Where(x => x.ToLower().Contains(ConfigurationManager.AppSettings["JobStarted" + GlobalData.ControllerType.Replace(" ", "_")]) || x.ToLower().Contains(ConfigurationManager.AppSettings["JobDone" + GlobalData.ControllerType.Replace(" ", "_")])))
                {
                    Regex numberRegex = new Regex(@"(?<=(JobNr\s*=|JobNum\s*:)\s*)\d*", RegexOptions.IgnoreCase);
                    int number = int.Parse(numberRegex.Match(command).ToString());
                    if (!GlobalData.SrcPathsAndJobs.Keys.Contains(Path.GetFileNameWithoutExtension(file.Key)))
                        GlobalData.SrcPathsAndJobs.Add(Path.GetFileNameWithoutExtension(file.Key), number);
                    break;
                }
            }
        }

        public static IDictionary<string, List<string>> DivideToFolds(IDictionary<string, string> filesAndContent)
        {
            IDictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            foreach (var file in filesAndContent.Where(item => item.Key.Contains(".src")))
            {
                bool isHFold = false;
                bool isPartOfFold = false;
                String[] lines = file.Value.ToString().Split('\n');
                string resultString = "";
                List<string> resultFolds = new List<string>();
                foreach (string line in lines.Where(x => x != "\n" && !string.IsNullOrEmpty(x)))
                {
                    if (line.ToLower().Contains(";fold"))
                        isPartOfFold = true;
                    if (line.ToLower().Replace(" ", "").Contains(";fold;%{h}") || line.ToLower().Replace(" ", "").Contains(";foldparameters;%{h}"))
                        isHFold = true;
                    if (isPartOfFold)
                        resultString = resultString + line + "\n";
                    else
                    {
                        if (!line.ToLower().Trim().Replace(" ", "").Contains(";company:") && !line.ToLower().Trim().Replace(" ", "").Contains(";programmer:") && !line.ToLower().Trim().Replace(" ", "").Contains(";date:") && !line.ToLower().Trim().Replace(" ", "").Contains(";changes:"))
                        {
                            resultFolds.Add(line + "\n");
                            resultString = "";
                        }
                    }

                    if (line.ToLower().Contains(";endfold"))
                    {
                        if (!isHFold)
                        {
                            resultFolds.Add(resultString);
                            resultString = "";
                            isPartOfFold = false;
                        }
                        isHFold = false;

                    }

                }
                result.Add(file.Key, resultFolds);
            }
            return result;
        }

        private static IDictionary<int, string> GetJobsWithDescr(IDictionary<string, List<string>> srcFiles)
        {
            IDictionary<int, List<string>> tempJobs = new Dictionary<int, List<string>>();
            IDictionary<int, string> jobsWithDescription = new Dictionary<int, string>();
            foreach (var file in srcFiles)
            {
                foreach (string command in file.Value.Where(x => x.ToLower().Contains(ConfigurationManager.AppSettings["JobStarted" + GlobalData.ControllerType.Replace(" ", "_")]) || x.ToLower().Contains(ConfigurationManager.AppSettings["JobDone" + GlobalData.ControllerType.Replace(" ", "_")])))
                {
                    Regex numberRegex = new Regex(@"(?<=(JobNr\s*=|JobNum\s*:)\s*)\d*", RegexOptions.IgnoreCase);
                    int number = int.Parse(numberRegex.Match(command).ToString());
                    Regex descriptionRegex = new Regex(@"(?<=DESC\s*=\s*)[a-zA-Z0-9\s_]*", RegexOptions.IgnoreCase);
                    string description = "";
                    if (command.ToLower().Contains("desc"))
                        description = descriptionRegex.Match(command).ToString();
                    if (!tempJobs.Keys.Contains(number))
                    {
                        List<string> descriptions = new List<string>();
                        descriptions.Add(description);
                        tempJobs.Add(number, descriptions);
                    }
                    else
                        if (!tempJobs[number].Contains(description))
                        tempJobs[number].Add(description);

                }
            }
            foreach (var job in tempJobs.Where(x => x.Value.Count > 1))
            {
                string selectedJob = null;
                while (selectedJob == null)
                {
                    var vm = new SelectJobViewModel(job, "current robot");
                    SelectJob sW = new SelectJob(vm);
                    var dialogResult = sW.ShowDialog();
                    selectedJob = vm.ResultText.Trim();
                }
                jobsWithDescription.Add(job.Key, selectedJob);
            }
            foreach (var job in tempJobs.Where(x => x.Value.Count == 1))
                jobsWithDescription.Add(job.Key, job.Value[0]);

            Dictionary<int, string> result = new Dictionary<int, string>();
            foreach (var item in jobsWithDescription.OrderBy(i => i.Key))
                result.Add(item.Key, item.Value);
            return result;
        }

        internal static List<string> SortGlobalFile(List<string> resultToWrite)
        {
            string[] pointExtensions = ConfigurationManager.AppSettings["SortableExtensions"].Split(',').Select(s => s.Trim()).ToArray();
            string[] specialExtensions = ConfigurationManager.AppSettings["SpecialExtensions"].Split(',').Select(s => s.Trim()).ToArray();
            IDictionary<string, PointInDat> points = new Dictionary<string, PointInDat>();
            List<string> result = new List<string>();
            string header = GetHeaderFromFile(resultToWrite);

            foreach (string extension in pointExtensions)
            {
                foreach (string line in resultToWrite)
                {
                    if (!(line.ToLower().Contains("e6pos ") || line.ToLower().Contains("deltamfg ") || line.ToLower().Contains("e6axis ") || line.ToLower().Contains("fdat ") || line.ToLower().Contains("fdat")) && !line.ToLower().Contains("enddat") && !result.Contains(line))
                    { }
                    //else if (line.ToLower().Contains("defdat"))
                    //    result.Add(line);
                    else
                    {
                        if (!line.ToLower().Contains("enddat"))
                        {
                            Regex pointNameRegex = new Regex(@"");
                            if (specialExtensions.Contains(extension))
                                pointNameRegex = new Regex(@"(?<=(E6POS|FDAT|PDAT|LDAT|E6AXIS)\s+.).*(?=\=)", RegexOptions.IgnoreCase);
                            else
                                pointNameRegex = new Regex(extension + @"_*\d+_\d+", RegexOptions.IgnoreCase);

                            //Regex pointNameRegex = new Regex(extension + @"_*\d+", RegexOptions.IgnoreCase);
                            string pointName = pointNameRegex.Match(line).ToString();
                            if (!string.IsNullOrEmpty(pointName))
                            {
                                if (pointName.ToLower().Contains("swp1887"))
                                { }
                                if (!points.Keys.Contains(pointName))
                                    points.Add(pointName, new PointInDat());
                                if (line.ToLower().Contains(" fdat "))
                                    points[pointName].Fdat = line;
                                if (line.ToLower().Contains(" e6pos "))
                                    points[pointName].Xpos = line;
                                if (line.ToLower().Contains(" e6axis "))
                                    points[pointName].XAxis = line;
                                if (line.ToLower().Contains(";deltamfg "))
                                    points[pointName].Delta = line;
                            }
                        }
                    }
                }
            }
            result.Add(header);
            foreach (var item in points)
            {
                if (!String.IsNullOrEmpty(item.Value.Xpos))
                    result.Add(item.Value.Xpos);
                if (!String.IsNullOrEmpty(item.Value.XAxis))
                    result.Add(item.Value.XAxis);
                if (!String.IsNullOrEmpty(item.Value.Fdat))
                    result.Add(item.Value.Fdat);
                if (!String.IsNullOrEmpty(item.Value.Delta))
                    result.Add(item.Value.Delta);
                result.Add("");
            }
            result.Add("\r\nENDDAT");

            return result;
        }

        private static string GetHeaderFromFile(List<string> resultToWrite)
        {
            string result = "";
            bool separatorFound = false;

            foreach (string line in resultToWrite)
            {
                result += line + "\r\n";
                if ((line.Contains(";***") || line.Contains("; ***")) && separatorFound)
                    break;
                if (line.Contains(";***") || line.Contains("; ***"))
                    separatorFound = true;

            }
            return result;
        }

        internal static void GetInputGlobalDataString(List<ResultInfo> result)
        {
            foreach (ResultInfo line in result.Where(x => x.Content.ToLower().Contains("e6pos") || x.Content.ToLower().Contains("e6axis") || x.Content.ToLower().Contains("fdat ") || (GlobalData.ControllerType== "KRC4 Not BMW" && x.Content.ToLower().Contains("xhome"))))
            {
                //if (GlobalData.ControllerType == "KRC4 Not BMW" && line.Content.Substring(0,5).ToLower().Equals("xhome"))
                //    GlobalData.GlobalDatsList.Add("DECL E6AXIS " + line.Content);
                //else
                    GlobalData.GlobalDatsList.Add(line.Content);
            }
        }

        internal static void GetInputDataString(List<ResultInfo> result)
        {
            List<string> inputDataList = new List<string>();
            string tempstring = "";
            foreach (ResultInfo item in result)
            {
                tempstring += item.Content + "\r\n";
                inputDataList.Add(item.Content);
            }
            GlobalData.InputData = tempstring;
            GlobalData.InputDataList = inputDataList;
        }

        public static IDictionary<int, List<string>> GetCollisionsWithDescription(IDictionary<string, string> srcFiles, string contrType = "")
        {
            bool isConrtTypePreset = contrType == "" ? false : true;
            Regex getCollZoneNumberKRC4 = new Regex(@"(?<=ZoneNum\s*\:\s*)\d+", RegexOptions.IgnoreCase);
            Regex getCollZoneCommentKRC4 = new Regex(@"(?<=Desc\s*\:\s*)[\d\w\s-_,\(\)]*", RegexOptions.IgnoreCase);
            if (contrType == "")
                contrType = GlobalData.ControllerType.Replace(" ", "_");
            string[] colldescrs = ConfigurationManager.AppSettings["CollisionDescriptions" + contrType].Split(',').Select(s => s.Trim()).ToArray();
            IDictionary<int, List<string>> result = new Dictionary<int, List<string>>();
            List<CollisionWithDescr> notFilteredCollisions = new List<CollisionWithDescr>();
            string line = string.Empty;
            foreach (string file in srcFiles.Keys)
            {
                var reader = new StreamReader(file);
                for (int i = 0; i < colldescrs.Length; i++)
                {
                    Regex isCollRegex = new Regex(colldescrs[i], RegexOptions.IgnoreCase);
                    reader = new StreamReader(file);
                    
                    while (!reader.EndOfStream)
                    {
                        int number = 0;
                        string description = "";
                        line = reader.ReadLine();
                        if (isCollRegex.IsMatch(line))
                        {
                            Regex getCollNr = new Regex(@"(?<=" + colldescrs[i] + @"[a-zA-Z\s-_]*)\d+", RegexOptions.IgnoreCase);
                            Match matchCollNr = getCollNr.Match(line);
                            int.TryParse(matchCollNr.ToString(), out number);
                            Regex getCollDescr = new Regex(@"(?<=" + number.ToString() + @"\s*).*", RegexOptions.IgnoreCase);
                            description = (getCollDescr.Match(line)).ToString();
                            description = description.Trim();
                            if (!string.IsNullOrEmpty(description) && description.Substring(0, 1) == "-")
                                description = description.Substring(1, description.Length - 1).Trim();
                            if (!notFilteredCollisions.Any(x => x.Number == number))
                                notFilteredCollisions.Add(new CollisionWithDescr(number, new List<string>() { description }));
                            else
                                notFilteredCollisions.Where(x => x.Number == number).FirstOrDefault().Description.Add(description);
                        }
                    }
                    if (reader.EndOfStream)
                        reader.Close();
                }
                if (contrType == "KRC4")
                {
                    reader = new StreamReader(file);
                    while (!reader.EndOfStream)
                    {
                        line = reader.ReadLine();
                        if (line.ToLower().Contains("collzone"))
                        { }
                        if (line.Trim().Replace(" ", "").Length > 5 && line.ToLower().Trim().Replace(" ", "").Substring(0, 5) == ";fold" && line.ToLower().Contains("collzone") && (line.ToLower().Contains("request") || line.ToLower().Contains("release")))
                        {
                            if (getCollZoneNumberKRC4.IsMatch(line))
                            {
                                int number = int.Parse(getCollZoneNumberKRC4.Match(line).ToString());
                                string description = getCollZoneCommentKRC4.Match(line).ToString().Trim();
                                if (description != string.Empty)
                                {
                                    if (!notFilteredCollisions.Any(x => x.Number == number))
                                        notFilteredCollisions.Add(new CollisionWithDescr(number, new List<string>() { description.Trim() }));
                                    else
                                        notFilteredCollisions.Where(x => x.Number == number).FirstOrDefault().Description.Add(description.Trim());
                                }
                            }
                        }
                    }
                    reader.Close();
                }

            }
            List<CollisionWithDescr> collisionsWithoutDuplicates = RemoveDuplicates(notFilteredCollisions);
            collisionsWithoutDuplicates = RemoveUselessSings(collisionsWithoutDuplicates);

            foreach (CollisionWithDescr collision in collisionsWithoutDuplicates)
            {
                if (!result.Keys.Contains(collision.Number))
                {
                    result.Add(collision.Number, collision.Description);
                }
            }

            foreach (var coll in isConrtTypePreset ? CollisionsForPSMethods.collisions : collisions)
            {
                if (!result.Keys.Contains(coll.Number))
                {
                    var vm = new TypeCollisionCommentViewModel(coll.Number);
                    TypeCollisionComment sW = new TypeCollisionComment(vm);
                    var dialogResult = sW.ShowDialog();
                    result.Add(coll.Number, new List<string>() { vm.Description, vm.Description });
                }
            }
            foreach (var item in result)
            {
                if (item.Value.Count < 2)
                {
                    result[item.Key].Add(item.Value[0]);
                }
            }

            return result;
        }

        internal static void GetCopyOperationsCount(ObservableCollection<ControlItem> allOperations)
        {
            int result = 0;
            if (GlobalData.ControllerType == "KRC4")            
                result = 1;
            foreach (var operation in allOperations)
            {
                if (operation.Operations[0].ActionType.ToString() == "Copy")
                    result++;
            }
            GlobalData.AllOperations = result;
        }

        private static List<CollisionWithDescr> RemoveUselessSings(List<CollisionWithDescr> collisionsWithoutDuplicates)
        {
            List<CollisionWithDescr> result = new List<CollisionWithDescr>();
            Regex removeData = new Regex(@"[a-zA-Z0-9].*", RegexOptions.IgnoreCase);
            foreach (var coll in collisionsWithoutDuplicates)
            {
                foreach (var item in coll.Description)
                {
                    string filteredDescr = removeData.Match(item).ToString();
                    if (!result.Any(x=>x.Number == coll.Number))
                        result.Add(new CollisionWithDescr(coll.Number, new List<string>()));
                    result.Where(x => x.Number == coll.Number).FirstOrDefault().Description.Add(filteredDescr);
                }
            }
            return result;
        }

        private static List<CollisionWithDescr> RemoveDuplicates(List<CollisionWithDescr> notFilteredCollisions)
        {
            List<CollisionWithDescr> resultBeforeFiltering = new List<CollisionWithDescr>();
            foreach (CollisionWithDescr collision in notFilteredCollisions)
            {
                if (!(resultBeforeFiltering.Exists(x => x.Number == collision.Number) && resultBeforeFiltering.Exists(x => x.Description == collision.Description)))
                    resultBeforeFiltering.Add(collision);
            }
            IDictionary<int, List<string>> inconsistentCollDescriptions = new Dictionary<int, List<string>>();
            foreach (CollisionWithDescr collision in resultBeforeFiltering)
            {
                if (!inconsistentCollDescriptions.Keys.Contains(collision.Number))
                {
                    List<string> listOfDescriptions = new List<string>();
                    foreach (var item in collision.Description.Where(x => !listOfDescriptions.Contains(x)))
                        listOfDescriptions.Add(item);
                    inconsistentCollDescriptions.Add(collision.Number, listOfDescriptions);
                }
                if (inconsistentCollDescriptions.Keys.Contains(collision.Number))
                {
                    foreach (var item in collision.Description)
                    {
                        if (!inconsistentCollDescriptions[collision.Number].Contains(item))
                            inconsistentCollDescriptions[collision.Number].Add(item);
                    }
                }
            }
            List<CollisionWithDescr> result = new List<CollisionWithDescr>();

            foreach (var item in inconsistentCollDescriptions)
            {
                string chosenItemReq = null, chosenItemClr = null;
                while (chosenItemReq == null || chosenItemClr == null)
                {
                    var vm = new SelectColisionViewModel(item,fillDescrs,40, false); // ViewModel Creation, parameter - KeyValue payr
                    SelectCollisionFromDuplicate sW = new SelectCollisionFromDuplicate(vm); // DialogView Creation - parameter ViewModel
                    var dialogResult = sW.ShowDialog();
                    chosenItemReq = vm.RequestText;
                    chosenItemClr = vm.ReleaseText;  // tmp = Selected value of ListBox
                }
                result.Add(new CollisionWithDescr(item.Key, new List<string>() { chosenItemReq, chosenItemClr }));
            }

            foreach (var item in inconsistentCollDescriptions.Where(x => x.Value.Count == 1))
            {
                result.Add(new CollisionWithDescr(item.Key, new List<string>() { item.Value[0] }));
            }
            return result;
        }

        private static void SetFillDescr()
        {
            fillDescrs = false;
            if (GlobalData.ControllerType == "KRC4")
            {
                DialogResult dialogResult = MessageBox.Show("Would you like to fill the description in Collzone statement?\r\nYes - Fill Colldescr\r\nNo - leave it blank", "Fill descriptions", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                    fillDescrs = true;
            }
        }
         
        private static IDictionary<string, List<string>> CorrectCollisionComments(IDictionary<string, List<string>> filteredFiles)
        {
            filteredFiles = RemoveCollisionComments(filteredFiles);
            IDictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            Regex isCollDescrRegex = new Regex(@";\s*" + ConfigurationManager.AppSettings["CollisionDescription" + GlobalData.ControllerType.Replace(" ", "_") + language], RegexOptions.IgnoreCase);
            Regex collzoneCallRegex = new Regex(@"Plc_CollSafety(Req|Clear)", RegexOptions.IgnoreCase);
            foreach (var file in filteredFiles)
            {
                List<string> commands = new List<string>();
                foreach (var item in file.Value)
                {
                    string collWithDescription = "";
                    int number = 0;
                    bool addItem = true, correctedCollision = false;
                    if (isCollDescrRegex.IsMatch(item) && !collzoneCallRegex.IsMatch(item))
                        addItem = false;
                    if (item.Contains(ConfigurationManager.AppSettings["CollisionZone" + GlobalData.ControllerType.Replace(" ", "_")]))
                    {
                        Regex getTempString = new Regex("(?<=" + ConfigurationManager.AppSettings["ZoneNumber" + GlobalData.ControllerType.Replace(" ", "_")] + ")[0-9]*");
                        Regex replaceRegex1 = new Regex(@"(Desc\s*:).*", RegexOptions.IgnoreCase);
                        Regex replaceRegex2 = new Regex(@"(Plc_CollDesc\s*=).*", RegexOptions.IgnoreCase);
                        Match match = getTempString.Match(item);
                        if (int.TryParse(match.ToString(), out number))
                        {
                            string descr = (item.ToLower().Replace(" ", "").Contains(ConfigurationManager.AppSettings["Collreq" + GlobalData.ControllerType.Replace(" ", "_")])) ? collisionsWithDescription[number][0].ToString() : collisionsWithDescription[number][1].ToString();
                            if (number > 0)
                            {
                                collWithDescription = "; " + ConfigurationManager.AppSettings["CollisionDescription" + GlobalData.ControllerType.Replace(" ", "_") + language] + " " + number.ToString() + " - " + descr + "\r\n" + item;
                                if (GlobalData.ControllerType == "KRC4")
                                {
                                    if (fillDescrs)
                                    {
                                        if (replaceRegex1.IsMatch(collWithDescription))
                                            collWithDescription = replaceRegex1.Replace(collWithDescription, "Desc:" + descr);
                                        else
                                        {
                                            Regex replaceRegex3 = new Regex(@"ZoneNum\:\d+", RegexOptions.IgnoreCase);
                                            string tobereplaced = replaceRegex3.Match(collWithDescription).ToString();
                                            collWithDescription = replaceRegex3.Replace(collWithDescription, tobereplaced + " Desc:" + descr);
                                        }
                                        collWithDescription = replaceRegex2.Replace(collWithDescription, "Plc_CollDesc=" + descr);
                                    }
                                    else
                                    {
                                        collWithDescription = replaceRegex1.Replace(collWithDescription, "");
                                        collWithDescription = replaceRegex2.Replace(collWithDescription, "Plc_CollDesc=");

                                    }
                                }
                                correctedCollision = true;
                            }
                        }
                    }
                    if (addItem && !correctedCollision)
                        commands.Add(item);
                    if (addItem && correctedCollision)
                        commands.Add(collWithDescription);
                }
                result.Add(file.Key, commands);
            }
            return result;
        }

        private static IDictionary<string, List<string>> RemoveCollisionComments(IDictionary<string, List<string>> filteredFiles)
        {
            Regex getDescrRegex = new Regex(@"((?<=Desc\s*\:)[\w\s:\(\)]*(?=\b))|(?<=Plc_CollDesc\=)[\w\s:\(\)]*(?=\b)", RegexOptions.IgnoreCase);
            IDictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            string[] collDescrs = ConfigurationManager.AppSettings["CollisionDescriptions" + GlobalData.ControllerType.Replace(" ", "_")].Split(',').Select(s => s.Trim()).ToArray();
            foreach (var file in filteredFiles)
            {
                List<string> clearedFile = new List<string>(file.Value);
                foreach (string comment in collDescrs)
                {
                    Regex collregex = new Regex(comment, RegexOptions.IgnoreCase);
                    foreach (string line in file.Value)
                    {
                        //if (line.ToLower().Contains(comment))
                        if (line.Length > 6 && collregex.IsMatch(line.Substring(0,6)))
                        {
                            clearedFile.Remove(line);
                        }
                        if (GlobalData.ControllerType == "KRC4" && (line.ToLower().Replace(" ","").Contains("collzonerequest") || line.ToLower().Replace(" ", "").Contains("collzonerelease")))
                        {
                            int lineIndex = clearedFile.IndexOf(line);
                            if (lineIndex != -1)
                                clearedFile[lineIndex] = getDescrRegex.Replace(line, string.Empty);
                        }
                    }
                }
                result.Add(file.Key, clearedFile);
            }

            return result;
        }

        internal static void DeleteOldConfigFile()
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to reset all application data?", "Are you sure?", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                Regex regex = new Regex(@"(?<=.*\\).*", RegexOptions.IgnoreCase);
                Match match = regex.Match(userName);
                if (File.Exists("C:\\Users\\" + match.ToString() + "\\AppData\\Local\\RobotFilesHarvester\\Application.config"))
                {
                    MessageBox.Show("Application has to close. Data will be refreshed after restart.", "Application closes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    App.Current.MainWindow.Close();
                    File.Delete("C:\\Users\\" + match.ToString() + "\\AppData\\Local\\RobotFilesHarvester\\Application.config");
                }
            }
            else if (dialogResult == DialogResult.No)
            {
                return;
            }

        }

        internal static void MirrorPaths()
        {
            if (DataToProcess == null || DataToProcess.DatFiles.Count == 0)
            {
                MessageBox.Show("No files found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool writeFiles = true;
            string directory = Path.GetDirectoryName(DataToProcess.DatFiles[0]) + "\\Mirrored";
            if (!Directory.Exists(Path.GetDirectoryName(DataToProcess.DatFiles[0]) + "\\Mirrored"))
            {
                directory = Path.GetDirectoryName(DataToProcess.DatFiles[0]) + "\\Mirrored";
                Directory.CreateDirectory(Path.GetDirectoryName(DataToProcess.DatFiles[0]) + "\\Mirrored");
            }
            else
            {
                if (Directory.GetFiles(Path.GetDirectoryName(DataToProcess.DatFiles[0]) + "\\Mirrored").Length > 0)
                {
                    DialogResult dialog = MessageBox.Show("Folder not empty!\r\nRemove files?", "Mirror folder not empty", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialog == DialogResult.Yes)
                        foreach (string file in Directory.GetFiles(Path.GetDirectoryName(DataToProcess.DatFiles[0]) + "\\Mirrored"))
                            File.Delete(file);
                    else
                        writeFiles = false;
                }

            }
            if (writeFiles)
            {
                foreach (var datfile in DataToProcess.DatFiles)
                {
                    List<string> datelements = new List<string>();

                    StreamReader reader = new StreamReader(datfile);
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (line.ToLower().Contains("e6axis") || line.ToLower().Contains("e6pos") || line.ToLower().Contains("fdat "))
                        {
                            line = MirrorLine(line, Path.GetFileNameWithoutExtension(datfile));
                        }
                        datelements.Add(line);
                    }


                    File.WriteAllText(Path.GetDirectoryName(datfile) + "\\Mirrored\\" + Path.GetFileName(datfile), ConvertListToString(datelements));
                    string srcFile = Path.GetDirectoryName(datfile) + "\\" + Path.GetFileNameWithoutExtension(datfile) + ".src";
                    if (File.Exists(srcFile))
                        File.Copy(srcFile, Path.GetDirectoryName(datfile) + "\\Mirrored\\" + Path.GetFileNameWithoutExtension(datfile) + ".src");
                    else
                        MessageBox.Show("File " + srcFile + " does not exist!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    MirrorInputData(directory + "\\" + Path.GetFileNameWithoutExtension(datfile) + ".olp");
                    //
                }

                MirrorGlobalsData(directory);
                MessageBox.Show("Mirroring successful", "Mirror", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private static void MirrorGlobalsData(string directory)
        {
            List<string> resultFile = new List<string>();
            string resultString = "";
            resultFile.Add("&COMMENT Globale Punkteliste\r\n& REL 33\r\nDEFDAT A02_tch_Maintenan_PTM_global PUBLIC");
            resultFile.AddRange(GlobalData.GlobalDatsList);
            resultFile.Add("ENDDAT");
            foreach (string line in resultFile)
            {
                string mirroredLine = MirrorLine(line, "");
                resultString += mirroredLine + "\r\n";
            }
            File.WriteAllText(directory + "\\globalsMirrored.dat", resultString);
        }

        private static void MirrorInputData(string destFile)
        {
            List<string> resultList = new List<string>();
            resultList.Add(";***********************************************************\r\n; *\r\n; *Default olp Template for Kuka - Krc - Bmw\r\n; *Created 27 / 10 / 2011 by Siemens PLM\r\n; *\r\n; *Study               : ST006_ST012_G22_G23\r\n; *Program             : Spot_06WZ01_G22\r\n; *Generation Date: 09 / 08 / 2018 at 10:31:32\r\n; *Robot               : 6IR1_kr2240_2\r\n; *User                : Administrator\r\n; *Tecnomatix Software: Process Simulate Disconnected 14.1\r\n; *Olp                 : Kuka - Krc - Bmw 2.50.4\r\n; *\r\n; ***********************************************************");
            string modifiedLine = "";
            string resultText = "";
            foreach (string line in GlobalData.InputDataList)
            {
                if (line.ToLower().Contains("e6axis") || line.ToLower().Contains("e6pos") || line.ToLower().Contains("xhome"))
                {
                    string templine = "";
                    //line = MirrorLine(line);
                    if (line.ToLower().Contains("xhome"))
                        templine = "DECL E6AXIS " + line;
                    else
                        templine = line;
                    modifiedLine = MirrorLine(templine, "");
                    resultList.Add(modifiedLine);
                }
                else if (line.ToLower().Contains("tool_data") || line.ToLower().Contains("base_data"))
                {
                    //line = MirrorLine(line);
                    modifiedLine = MirrorToolOrBase(line);
                    resultList.Add(modifiedLine);
                }
                else if (line.ToLower().Contains("inputdata"))
                { }
                else
                {
                    resultList.Add(line);
                }
            }
            //resultList.Add("END");
            foreach (string line in resultList)
                resultText += line + "\r\n";
            File.WriteAllText(destFile, resultText);
        }

        private static string MirrorToolOrBase(string line)
        {
            string result = "";
            if (line.ToLower().Contains("tool"))
                result += "TOOL_DATA[";
            else
                result += "BASE_DATA[";
            Regex numberRegex = new Regex(@"(?<=\[)\d+", RegexOptions.IgnoreCase);
            result += numberRegex.Match(line).ToString() + "]={X ";
            Regex xRegex = new Regex(@"(?<=\{\s*X\s*)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);
            result += double.Parse(xRegex.Match(line).ToString(), CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture) + ",Y ";
            Regex yRegex = new Regex(@"(?<=,\s*Y\s*)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);
            result += (-double.Parse(yRegex.Match(line).ToString(), CultureInfo.InvariantCulture)).ToString(CultureInfo.InvariantCulture) + ",Z ";
            Regex zRegex = new Regex(@"(?<=,\s*Z\s*)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);
            result += double.Parse(zRegex.Match(line).ToString(), CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture) + ",A ";
            Regex aRegex = new Regex(@"(?<=,\s*A\s*)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);
            result += (-double.Parse(aRegex.Match(line).ToString(), CultureInfo.InvariantCulture)).ToString(CultureInfo.InvariantCulture) + ",B ";
            Regex bRegex = new Regex(@"(?<=,\s*B\s*)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);
            result += double.Parse(bRegex.Match(line).ToString(), CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture) + ",C ";
            Regex cRegex = new Regex(@"(?<=,\s*C\s*)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);
            result += (-double.Parse(cRegex.Match(line).ToString(), CultureInfo.InvariantCulture)).ToString(CultureInfo.InvariantCulture) + "}";

            return result;
        }

        private static string ConvertListToString(List<string> datelements)
        {
            string result = "";
            foreach (string line in datelements)
                result += line + "\r\n";
            return result;
        }

        private static string MirrorLine(string line, string file)
        {
            if (line.ToLower().Contains("e6pos"))
            {
                Regex pointRegex = new Regex(@"(?<=E6POS\s+)[\w\d_-]+", RegexOptions.IgnoreCase);
                string point = pointRegex.Match(line).ToString();
                Regex xRegex = new Regex(@"(?<=\{\s*X\s*)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);
                double xValue = double.Parse(xRegex.Match(line).ToString(), CultureInfo.InvariantCulture);
                Regex yRegex = new Regex(@"(?<=,\s*Y\s*)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);
                double yValue = double.Parse(yRegex.Match(line).ToString(), CultureInfo.InvariantCulture);
                Regex zRegex = new Regex(@"(?<=,\s*Z\s*)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);
                double zValue = double.Parse(zRegex.Match(line).ToString(), CultureInfo.InvariantCulture);
                Regex aRegex = new Regex(@"(?<=,\s*A\s*)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);
                double aValue = double.Parse(aRegex.Match(line).ToString(), CultureInfo.InvariantCulture);
                Regex bRegex = new Regex(@"(?<=,\s*B\s*)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);
                double bValue = double.Parse(bRegex.Match(line).ToString(), CultureInfo.InvariantCulture);
                Regex cRegex = new Regex(@"(?<=,\s*C\s*)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);
                double cValue = double.Parse(cRegex.Match(line).ToString(), CultureInfo.InvariantCulture);
                Regex sRegex = new Regex(@"(?<=,\s*S\s*)\d+", RegexOptions.IgnoreCase);
                int sValue = 0;
                if (sRegex.Match(line).Success)
                    sValue = int.Parse(sRegex.Match(line).ToString());
                else
                    MessageBox.Show("Configuration not found for point " + point + ", path: " + file, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Regex tRegex = new Regex(@"(?<=,\s*T\s*)\d+", RegexOptions.IgnoreCase);
                int tValue = 0;
                if (tRegex.Match(line).Success)
                    tValue = int.Parse(tRegex.Match(line).ToString());
                Regex restRegex = new Regex(@",\s*E1.*", RegexOptions.IgnoreCase);
                string rest = restRegex.Match(line).ToString();
                if (string.IsNullOrEmpty(rest))
                    rest = "}";

                string binary = Convert.ToString(tValue, 2);
                string tempBinary = binary;
                for (int i = binary.Length; i < 6; i++)
                {
                    tempBinary = "0" + tempBinary;
                }
                string pos5 = "", pos2 = "", pos0 = "";
                if (tempBinary[5].ToString() == "0")
                    pos5 = "1";
                else
                    pos5 = "0";

                if (tempBinary[2].ToString() == "0")
                    pos2 = "1";
                else
                    pos2 = "0";

                if (tempBinary[0].ToString() == "0")
                    pos0 = "1";
                else
                    pos0 = "0";

                string convertedTurn = pos0 + tempBinary[1] + pos2 + tempBinary[3] + tempBinary[4] + pos5;
                string convertedTurnDec = Convert.ToInt32(convertedTurn, 2).ToString();
                yValue = -yValue;
                aValue = -aValue;
                cValue = -cValue;

                line = "DECL E6POS " + point + "={X " + xValue.ToString(CultureInfo.InvariantCulture) + ",Y " + yValue.ToString(CultureInfo.InvariantCulture) + ",Z " + zValue.ToString(CultureInfo.InvariantCulture) + ",A " + aValue.ToString(CultureInfo.InvariantCulture) + ",B " + bValue.ToString(CultureInfo.InvariantCulture) + ",C " + cValue.ToString(CultureInfo.InvariantCulture) + ",S " + sValue + ",T " + convertedTurnDec.ToString(CultureInfo.InvariantCulture) + rest;
            }
            else if (line.ToLower().Contains("e6axis"))
            {
                Regex a1Regex = new Regex(@"(?<=\{\s*A1\s*)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);
                string a1Value = a1Regex.Match(line).ToString();
                Regex a2Regex = new Regex(@"(?<=,\s*A2\s*)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);
                string a2Value = a2Regex.Match(line).ToString();
                Regex a3Regex = new Regex(@"(?<=,\s*A3\s*)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);
                string a3Value = a3Regex.Match(line).ToString();
                Regex a4Regex = new Regex(@"(?<=,\s*A4\s*)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);
                string a4Value = a4Regex.Match(line).ToString();
                Regex a5Regex = new Regex(@"(?<=,\s*A5\s*)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);
                string a5Value = a5Regex.Match(line).ToString();
                Regex a6Regex = new Regex(@"(?<=,\s*A6\s*)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);
                string a6Value = a6Regex.Match(line).ToString();
                Regex e1Regex = new Regex(@"(?<=,\s*E1\s*)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);
                string e1Value = e1Regex.Match(line).ToString();
                Regex pointRegex = new Regex(@"(?<=E6AXIS\s+)[\w\d_-]+", RegexOptions.IgnoreCase);
                string point = pointRegex.Match(line).ToString();
                Regex restRegex = new Regex(@",\s*E2.*", RegexOptions.IgnoreCase);
                string rest = restRegex.Match(line).ToString();

                float a1Valuefloat = float.Parse(a1Value, CultureInfo.InvariantCulture);
                a1Valuefloat = -a1Valuefloat;
                a1Value = a1Valuefloat.ToString();
                float a4Valuefloat = float.Parse(a4Value, CultureInfo.InvariantCulture);
                a4Valuefloat = -a4Valuefloat;
                a4Value = a4Valuefloat.ToString();
                float a6Valuefloat = float.Parse(a6Value, CultureInfo.InvariantCulture);
                a6Valuefloat = -a6Valuefloat;
                a6Value = a6Valuefloat.ToString();

                if (!string.IsNullOrEmpty(e1Value))
                    line = "DECL E6AXIS " + point + "={A1 " + a1Value.ToString(CultureInfo.InvariantCulture) + ",A2 " + a2Value.ToString(CultureInfo.InvariantCulture) + ",A3 " + a3Value.ToString(CultureInfo.InvariantCulture) + ",A4 " + a4Value.ToString(CultureInfo.InvariantCulture) + ",A5 " + a5Value.ToString(CultureInfo.InvariantCulture) + ",A6 " + a6Value.ToString(CultureInfo.InvariantCulture) + ",E1 " + e1Value.ToString(CultureInfo.InvariantCulture) + rest;
                else
                    line = "DECL E6AXIS " + point + "={A1 " + a1Value.ToString(CultureInfo.InvariantCulture) + ",A2 " + a2Value.ToString(CultureInfo.InvariantCulture) + ",A3 " + a3Value.ToString(CultureInfo.InvariantCulture) + ",A4 " + a4Value.ToString(CultureInfo.InvariantCulture) + ",A5 " + a5Value.ToString(CultureInfo.InvariantCulture) + ",A6 " + a6Value.ToString(CultureInfo.InvariantCulture) + "}";
            }
            else if (line.ToLower().Contains("fdat"))
            { }
            return line;
        }

        private static IDictionary<string, List<string>> CheckOrder(IDictionary<string, List<string>> filteredFiles)
        {
            IDictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            IDictionary<int, string> orderOfOperations = new Dictionary<int, string>();
            List<FilesWithPriorities> resultList = new List<FilesWithPriorities>();
            string[] commandsOrder = ConfigurationManager.AppSettings["OrderOfOperations" + GlobalData.ControllerType.Replace(" ", "_")].Split(',').Select(s => s.Trim()).ToArray();
            for (int i = 0; i < commandsOrder.Length; i++)
                orderOfOperations.Add(i, commandsOrder[i]);

            foreach (var file in filteredFiles)
            {
                int jobNr = FindJob(file);
                List<OperationsPriority> orderOfOperationsFromFile = new List<OperationsPriority>();
                KeyValuePair<string, List<string>> switchGrops = FindSwitchGroups(file);
                bool isFirstPTPpointFound = false;
                bool isHomeAlreadyFound = false;
                foreach (string operation in switchGrops.Value)
                {
                    if (operation.Contains(" PTP ") && !operation.Contains(ConfigurationManager.AppSettings["PTPHome" + GlobalData.ControllerType.Replace(" ", "_")]))
                        isFirstPTPpointFound = true;
                    OperationsPriority operationFromFile = new OperationsPriority();
                    foreach (var op in orderOfOperations)
                    {
                        string tempOp = operation.ToLower().Replace(" ", "");
                        if (tempOp.Contains((ConfigurationManager.AppSettings[op.Value + GlobalData.ControllerType.Replace(" ", "_")]).Replace("USERBITNR", (33 - jobNr).ToString()).ToLower()))
                        {
                            operationFromFile = new OperationsPriority(op.Key, operation);
                            break;
                        }

                        if (operation.ToLower().Contains("endswitch"))
                        {
                            operationFromFile = new OperationsPriority(99, operation);
                            break;
                        }
                        if (operation.Contains(ConfigurationManager.AppSettings[op.Value + GlobalData.ControllerType.Replace(" ", "_")]))
                        {
                            if (operation.Contains(ConfigurationManager.AppSettings["PTPHome" + GlobalData.ControllerType.Replace(" ", "_")]) && isHomeAlreadyFound)
                                operationFromFile = new OperationsPriority(99, operation);
                            else
                                operationFromFile = new OperationsPriority(op.Key, operation);

                            if (operation.Contains(ConfigurationManager.AppSettings["PTPHome" + GlobalData.ControllerType.Replace(" ", "_")]))
                                isHomeAlreadyFound = true;
                        }
                    }
                    if (operationFromFile.Command == null || isFirstPTPpointFound)
                        operationFromFile = new OperationsPriority(99, operation);
                    orderOfOperationsFromFile.Add(operationFromFile);
                }
                resultList.Add(new FilesWithPriorities(file.Key, orderOfOperationsFromFile));
            }

            result = SortSrcFile(resultList);
            IDictionary<string, string> resultDividedToString = new Dictionary<string, string>();
            foreach (var file in result)
            {
                string resultString = "";
                foreach (string item in file.Value)
                {
                    resultString += item + "\r\n";
                }
                resultDividedToString.Add(file.Key, resultString);
            }
            result = DivideToFolds(resultDividedToString);
            result = ClearEnters(result);
            return result;
        }

        private static int FindJob(KeyValuePair<string, List<string>> file)
        {

            foreach (string command in file.Value)
            {
                if (command.ToLower().Replace(" ", "").Contains(ConfigurationManager.AppSettings["JobReq" + GlobalData.ControllerType.Replace(" ", "_")]))
                {
                    Regex jobNrRegex = new Regex(@"(?<=(Job_req[a-zA-Z]*|Plc_Job)\s*\(\s*)\d+", RegexOptions.IgnoreCase);
                    return int.Parse(jobNrRegex.Match(command).ToString());
                }
            }
            return 0;
        }

        private static IDictionary<string, List<string>> SortSrcFile(List<FilesWithPriorities> inputList)
        {
            IDictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            foreach (var file in inputList)
            {
                IDictionary<int, OperationsPriority> positionAndCommand = new Dictionary<int, OperationsPriority>();
                IDictionary<int, OperationsPriority> positionAndCommandWithPriority = new Dictionary<int, OperationsPriority>();
                IDictionary<int, OperationsPriority> positionAndCommandWithoutPriority = new Dictionary<int, OperationsPriority>();
                int counter = 0;
                foreach (var item in file.Commands)
                {
                    positionAndCommand.Add(counter, item);
                    counter++;
                }
                foreach (var item in positionAndCommand.Where(x => x.Value.Priority == 99))
                {
                    positionAndCommandWithoutPriority.Add(item);
                }
                foreach (var item in positionAndCommand.Where(x => x.Value.Priority != 99))
                {
                    positionAndCommandWithPriority.Add(item);
                }
                IDictionary<int, OperationsPriority> commandsWithCorrectPosition = new Dictionary<int, OperationsPriority>();
                for (int priority = 0; priority < 20; priority++)
                {
                    foreach (var item in positionAndCommandWithPriority)
                    {
                        if (item.Value.Priority == priority)
                            commandsWithCorrectPosition.Add(item);
                    }
                }
                List<int> positions = new List<int>();
                foreach (var item in commandsWithCorrectPosition)
                    positions.Add(item.Key);
                List<int> positionsNotSorted = new List<int>(positions);
                positions.Sort();

                IDictionary<int, string> sortedCommands = new Dictionary<int, string>();
                int j = 0;
                foreach (int item in positions)
                {
                    sortedCommands.Add(item, commandsWithCorrectPosition[positionsNotSorted[j]].Command);
                    j++;
                }
                foreach (var item in positionAndCommandWithoutPriority)
                {
                    sortedCommands.Add(item.Key, item.Value.Command);
                }
                List<string> resultList = new List<string>();
                for (int i = 0; i < sortedCommands.Count; i++)
                {
                    resultList.Add(sortedCommands[i]);
                }
                result.Add(file.Filename, resultList);
            }

            return result;
        }

        private static IDictionary<string, List<string>> AddHeader(IDictionary<string, List<string>> filteredFiles, bool isDat)
        {
            IDictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            string currentOpType = "";
            string[] operationTypes = ConfigurationManager.AppSettings["OperationTypes" + GlobalData.ControllerType.Replace(" ", "_")].Split(',').Select(s => s.Trim()).ToArray();
            string[] maintenanceTypes = ConfigurationManager.AppSettings["MaintenanceTypes" + GlobalData.ControllerType.Replace(" ", "_")].Split(',').Select(s => s.Trim()).ToArray();

            //string matchRoboter = string.Empty;
            Regex getRoboter = new Regex(@"(?<=:\s*).*");
            if (!string.IsNullOrEmpty(roboter) && getRoboter.IsMatch(roboter) && !roboter.ToLower().Contains("&param"))
                matchRoboter = getRoboter.Match(roboter).ToString();
            else
            {
                if (string.IsNullOrEmpty(matchRoboter))
                {
                    NameRoboterViewModel vm = new NameRoboterViewModel();
                    var dialog = new NameRobot(vm);
                    dialog.ShowDialog();
                    roboter = vm.RobotName == null ? string.Empty : vm.RobotName;
                    matchRoboter = vm.RobotName == null ? string.Empty : vm.RobotName;
                }
            }
            string header = "";
            string resultWerkzeug = "";
            Match matchWerkzeug;
            //if (roboter == null)
            //    throw new ArgumentException("Robot name not found");

            foreach (var file in filteredFiles)
            {
                resultWerkzeug = "";
                currentOpType = "";
                foreach (string opType in operationTypes)
                {
                    string filename = string.Empty;
                    if (opType == "SpotA04" && GlobalData.WeldingType == "A04")
                    {
                        string tempFileName = Path.GetFileName(file.Key).ToLower().Replace("spot_", "SpotA04_");
                        filename = file.Key.Replace(Path.GetFileName(file.Key), tempFileName);
                    }
                    else if (opType == "SpotA05" && GlobalData.WeldingType == "A05")
                    {
                        string tempFileName = Path.GetFileName(file.Key).ToLower().Replace("spot_", "SpotA05_");
                        filename = file.Key.Replace(Path.GetFileName(file.Key), tempFileName);
                    }
                    else
                        filename = file.Key;

                    Regex central = new Regex(opType, RegexOptions.IgnoreCase);
                    string match = central.Match(filename).ToString();
                    if (!string.IsNullOrEmpty(match))
                    {
                        currentOpType = opType;
                        break;
                    }
                    if (Path.GetFileNameWithoutExtension(file.Key).ToLower().Contains(opType.ToLower()))
                    {
                        currentOpType = opType;
                        break;
                    }
                }
                if (currentOpType != "")
                {
                    if (currentOpType == ".*pos\\d*_h\\d+")
                    {
                        currentOpType = "CentralToHome";
                        resultWerkzeug = "";
                    }
                    else if (currentOpType == "h\\d+_.*pos\\d*")
                    {
                        currentOpType = "HomeToCentral";
                        resultWerkzeug = "";
                    }
                    else
                    {
                        string filename = Path.GetFileNameWithoutExtension(file.Key);
                        filename = filename.ToUpper().Replace("_ST", "_");
                        Regex getWerkzeug = new Regex(@"(?<=_|ST)\d+[a-zA-Z]+\d+");
                        matchWerkzeug = getWerkzeug.Match(filename);
                        resultWerkzeug = matchWerkzeug.ToString();
                    }
                }

                else
                {
                    foreach (string maintType in maintenanceTypes)
                    {
                        if (file.Key.ToLower().Contains(maintType.ToLower()))
                        {
                            currentOpType = maintType;
                            break;
                        }
                    }
                }
                if (currentOpType == "")
                {
                    string[] omitedDataInCheck = ConfigurationManager.AppSettings["OmitedDataInCheck" + GlobalData.ControllerType.Replace(" ", "_")].Split(',').Select(s => s.Trim()).ToArray();

                    bool flag = false;
                    foreach (string item in omitedDataInCheck)
                    {
                        if (file.Key.Contains(ConfigurationManager.AppSettings[item]))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        string message = "Operation " + Path.GetFileName(file.Key) + " is of unknown type. Check if operation name is correct";
                        logFileContent = logFileContent + message + "\r\n";
                        //MessageBox.Show(message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                header = "";

                header = header + ConfigurationManager.AppSettings["Header1" + GlobalData.ControllerType.Replace(" ", "_") + language] + "\r\n";
                header = header + ConfigurationManager.AppSettings["Header2" + GlobalData.ControllerType.Replace(" ", "_") + language] + Path.GetFileNameWithoutExtension(file.Key) + "\r\n";
                header = header + ConfigurationManager.AppSettings["Header3" + GlobalData.ControllerType.Replace(" ", "_") + language] + ConfigurationManager.AppSettings[currentOpType.Replace("A04","").Replace("A05","") + GlobalData.ControllerType.Replace(" ", "_") + language] + "" + resultWerkzeug + "\r\n";
                header = header + ConfigurationManager.AppSettings["Header4" + GlobalData.ControllerType.Replace(" ", "_") + language] + matchRoboter.Trim() + "\r\n";
                header = header + ConfigurationManager.AppSettings["Header5" + GlobalData.ControllerType.Replace(" ", "_") + language] + "AIUT" + "\r\n";
                header = header + ConfigurationManager.AppSettings["Header6" + GlobalData.ControllerType.Replace(" ", "_") + language] + ConfigurationManager.AppSettings["Ersteller"] + "\r\n";
                header = header + ConfigurationManager.AppSettings["Header7" + GlobalData.ControllerType.Replace(" ", "_") + language] + DateTime.Now.ToString() + "\r\n";
                header = header + ConfigurationManager.AppSettings["Header8" + GlobalData.ControllerType.Replace(" ", "_") + language] + "\r\n";
                header = header + ConfigurationManager.AppSettings["Header9" + GlobalData.ControllerType.Replace(" ", "_") + language] + "\r\n";
                if (!isDat)
                {
                    // Char.ConvertFromUtf32(160) = twarda spacja
                    //header = header + Char.ConvertFromUtf32(160) + "\r\n" + Char.ConvertFromUtf32(160) + "\r\n" + ";# --------- START PATH : " + Path.GetFileNameWithoutExtension(file.Key) + " ---------\r\n" + Char.ConvertFromUtf32(160);
                    header = header + Char.ConvertFromUtf32(160) + "\r\n" + Char.ConvertFromUtf32(160) + "\r\n" + ";# --------- START PATH : " + Path.GetFileNameWithoutExtension(file.Key) + " ---------\r\n\r\n";
                }
                List<string> resultStrings = new List<string>();
                Regex isFoldStart = new Regex(@"^\s*;\s*FOLD\s+", RegexOptions.IgnoreCase);
                Regex isFoldEnd = new Regex(@"^\s*;\s*ENDFOLD\s+", RegexOptions.IgnoreCase);
                Regex notCommentRegex = new Regex(@"^\s*(?!;).*", RegexOptions.IgnoreCase);
                int foldCount = 0;
                bool alreadyAdded = false;
                foreach (string currentLine in file.Value)
                {
                    if (!isDat)
                    {
                        resultStrings.Add(currentLine);
                        if ((currentLine.Contains(";ENDFOLD (INI)") && !isDat))
                            resultStrings.Add(header);
                    }
                    else
                    {
                        if (isFoldStart.IsMatch(currentLine))
                            foldCount++;
                        if (isFoldEnd.IsMatch(currentLine))
                            foldCount--;
                        if (!alreadyAdded && foldCount == 0 && !string.IsNullOrEmpty(currentLine) && notCommentRegex.IsMatch(currentLine) && !currentLine.ToLower().Contains("defdat"))
                        {
                            alreadyAdded = true;
                            resultStrings.Add(header);
                            resultStrings.Add(";================================================\r\n; Positions(if any)\r\n;================================================ \r\n");
                        }
                        resultStrings.Add(currentLine);

                    }

                }
                result.Add(file.Key, resultStrings);
            }
            return result;
        }

        private static IDictionary<string, List<string>> ClearEnters(IDictionary<string, List<string>> filteredFiles)
        {
            IDictionary<string, List<string>> result = new Dictionary<string, List<string>>();

            foreach (var file in filteredFiles)
            {
                List<string> filteredStrings = new List<string>();
                foreach (string tempstring in file.Value)
                {
                    if (!(tempstring == "" | tempstring == "\r\n" | tempstring == "\n"))
                        filteredStrings.Add(tempstring);
                }
                result.Add(file.Key, filteredStrings);
            }

            return result;
        }


        private static IDictionary<string, List<string>> ClearHeader(IDictionary<string, List<string>> filteredFiles)
        {
            Regex notCommentRegex = new Regex(@"^\s*(?!;).*", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Regex headerStartRegex = new Regex(@"^\s*;\s*\*", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            GlobalData.RoboterFound = false;
            List<string> header = new List<string>();
            bool roboterFound = false;
            IDictionary<string, List<string>> result = new Dictionary<string, List<string>>();

            foreach (var file in filteredFiles)
            {
                bool headerFinished = false, headerStarted = false, isMeaningfulFold = false;
                List<string> contentWithoutHeader = new List<string>();
                foreach (string line in file.Value)
                {

                    string lineWithoutSpaces = line.Replace(" ", "");
                    if ((lineWithoutSpaces.Contains("Roboter") || (lineWithoutSpaces.Contains("Robot") && lineWithoutSpaces.Contains(";*"))) && !GlobalData.RoboterFound)
                    {
                        roboter = line;
                        GlobalData.RoboterFound = true;
                        roboterFound = true;
                    }
                    else
                    {
                        if (!roboterFound)
                            roboter = "";
                    }

                    isMeaningfulFold = notCommentRegex.IsMatch(line) && !string.IsNullOrEmpty(notCommentRegex.Match(line).ToString());
                    if (!isMeaningfulFold && !headerFinished && headerStartRegex.IsMatch(line))
                        headerStarted = true;
                    if (isMeaningfulFold && headerStarted && !string.IsNullOrEmpty(line))
                        headerFinished = true;

                    // jeszcze nie było headera lub już się skończył
                    if (!headerStarted || headerFinished)
                        contentWithoutHeader.Add(line);
                    

                }
                result.Add(file.Key, contentWithoutHeader);
            }
            return result;
        }

        public static IDictionary<string, string> AddSpaces(IDictionary<string, List<string>> filesAndContent)
        {
            IDictionary<string, string> result = new Dictionary<string, string>();
            foreach (var file in filesAndContent)
            {
                string resultString = string.Empty;
                DataClass.KukaCommandClass previousCommand = null;
                foreach (var command in file.Value)
                {
                    DataClass.KukaCommandClass currentItem = new DataClass.KukaCommandClass(command);
               
                    if (CheckAddSpaceBeforeCurrentKuka(currentItem,previousCommand))
                        resultString += "\r\n" + currentItem.Content;
                    else
                        resultString += currentItem.Content;
                    previousCommand = currentItem;
                }
                result.Add(file.Key, resultString);
            }

            return result;
        }

        private static bool CheckAddSpaceBeforeCurrentKuka(DataClass.KukaCommandClass currentItem, DataClass.KukaCommandClass previousCommand)
        {
            bool addSpaceBeforeCurrent = false;
            if (previousCommand != null)
            {
                if (currentItem.IsComment && previousCommand.IsMeaningfulFold)
                    addSpaceBeforeCurrent = true;
                if ((previousCommand.IsSingleInstruction || previousCommand.IsMeaningfulFold) && !previousCommand.IsTriggeredAction && !previousCommand.IsMotionFoldFold && currentItem.IsMotionFoldFold)
                    addSpaceBeforeCurrent = true;
                if (currentItem.IsCollisionReqRel && !previousCommand.IsCollisionReqRel)
                    addSpaceBeforeCurrent = true;
                if (currentItem.IsEnd)
                    addSpaceBeforeCurrent = true;
                if (currentItem.IsMeaningfulFold && !currentItem.IsCollisionReqRel && previousCommand.IsCollisionReqRel)
                    addSpaceBeforeCurrent = true;
                if (currentItem.IsMotionFoldFold && previousCommand.IsTriggeredAction)
                    addSpaceBeforeCurrent = true;
            }
            return addSpaceBeforeCurrent;
        }

        private static bool DetectAreaGroupL6(string fileContent, string previousItem)
        {
            bool fileContentContainsArea = false;
            for (int i = 21; i <= 32; i++)
            {
                if (fileContent.ToLower().Contains("wait for") && fileContent.ToLower().Contains("$in[" + i + "]"))
                {
                    fileContentContainsArea = true;
                    break;
                }
            }
            if (previousItem.ToLower().Contains("xhome") && fileContentContainsArea)
                return true;
            return false;
        }

        private static bool DetectCompleteHandshake(string fileContent, string previousLine, string v2)
        {
            for (int i = 21; i <= 32; i++)
            {
                if (previousLine.ToLower().Contains("wait for") && previousLine.ToLower().Contains("$in[" + i + "]"))
                {
                    return false;
                }
            }
            bool result = false;
            if (fileContent.Contains("$OUT"))
            {
                if (v2.Contains("$OUT") || !previousLine.Contains("$IN"))
                    result = true;
            }
            return result;
        }

        private static void CheckOpeningAndClosingCommand(IDictionary<string, List<string>> srcFiles)
        {
            string directory = "";
            IDictionary<int, Operation> operations = new Dictionary<int, Operation>();
            List<IDictionary<string, List<OpenAndCloseCommand>>> result = new List<IDictionary<string, List<OpenAndCloseCommand>>>();
            IDictionary<string, List<OpenAndCloseCommand>> fileAndCommands = new Dictionary<string, List<OpenAndCloseCommand>>();
            int nrOfPairs = int.Parse(ConfigurationManager.AppSettings["NrOfFilterPairs" + GlobalData.ControllerType.Replace(" ", "_")]);
            List<OpenAndCloseCommand> allOperations = new List<OpenAndCloseCommand>();
            for (int i = 1; i <= nrOfPairs; i++)
            {
                string[] filterPair = ConfigurationManager.AppSettings["FiltersPair" + i.ToString() + GlobalData.ControllerType.Replace(" ", "_")].Split(',').Select(s => s.Trim()).ToArray();
                fileAndCommands = new Dictionary<string, List<OpenAndCloseCommand>>();
                foreach (var file in srcFiles)
                {
                    int job = FindJob(file);
                    if (directory == "")
                        directory = Path.GetDirectoryName(file.Key);
                    List<OpenAndCloseCommand> foundValues = new List<OpenAndCloseCommand>();
                    int lineCounter = 1;
                    foreach (string command in file.Value)
                    {
                        bool isStart = false;
                        string name = "";
                        //if (command.Contains((ConfigurationManager.AppSettings[filterPair[0] + GlobalData.ControllerType.Replace(" ", "_")]).Replace("USERBITNR",(33-1).ToString())) | command.Contains((ConfigurationManager.AppSettings[filterPair[1] + GlobalData.ControllerType.Replace(" ", "_")]).Replace("USERBITNR", (33 - 1).ToString())))
                        //POPRAWIC JAK NAJSZYBCIEJ!!!!!
                        if (command.ToLower().Replace(" ", "").Contains((ConfigurationManager.AppSettings[filterPair[0] + GlobalData.ControllerType.Replace(" ", "_")]).Replace("USERBITNR", (33 - 1).ToString())) | command.ToLower().Replace(" ", "").Contains((ConfigurationManager.AppSettings[filterPair[1] + GlobalData.ControllerType.Replace(" ", "_")]).Replace("USERBITNR", (33 - 1).ToString())))
                        {
                            if (command.ToLower().Replace(" ", "").Contains((ConfigurationManager.AppSettings[filterPair[0] + GlobalData.ControllerType.Replace(" ", "_")]).Replace("USERBITNR", (33 - 1).ToString())))
                            {
                                isStart = true;
                                name = filterPair[0];
                            }
                            else
                            {
                                isStart = false;
                                name = filterPair[1];
                            }
                            int number = 0;
                            string regex = "";
                            if (command.Contains("PTP") | command.Contains("LIN"))
                                regex = @"(?<=" + (ConfigurationManager.AppSettings[filterPair[2] + GlobalData.ControllerType.Replace(" ", "_")]).Replace(":", "") + @"\s*(=|:)\s*)\d+";
                            //regex = @"(?<=" + ConfigurationManager.AppSettings[filterPair[2] + GlobalData.ControllerType.Replace(" ", "_")] + @"\s*=\s*)\d+(?=\,)";
                            else
                                regex = @"\d+";
                            foreach (Match match in Regex.Matches(command, regex, RegexOptions.IgnoreCase))
                            {
                                number = int.Parse(match.ToString());
                                Regex descriptionRegex = new Regex(@"(?<=DESC\s*=\s*)[a-zA-Z0-9\s_/\\\.\,]*", RegexOptions.IgnoreCase);
                                string description = "";
                                if (command.ToLower().Contains("desc"))
                                    description = descriptionRegex.Match(command).ToString();
                                OpenAndCloseCommand currentCommand = new OpenAndCloseCommand(file.Key, filterPair[3], number, lineCounter, isStart, description);
                                allOperations.Add(currentCommand);
                                if (match != null & match.ToString() != "")
                                {
                                    foundValues.Add(currentCommand);
                                    break;
                                }
                            }

                        }
                        lineCounter++;
                    }
                    if (!fileAndCommands.Keys.Contains(Path.GetFileName(file.Key)))
                        fileAndCommands.Add(Path.GetFileName(file.Key), foundValues);
                }
                result.Add(fileAndCommands);
            }

            FillJobsList(allOperations);
            foreach (var filter in result)
            {
                foreach (KeyValuePair<string, List<OpenAndCloseCommand>> file in filter)
                {
                    operations = new Dictionary<int, Operation>();
                    foreach (var item in file.Value)
                    {
                        if (!operations.ContainsKey(item.Number))
                        {
                            List<int> intList = new List<int>();
                            intList.Add(item.Line);
                            if (item.IsStart)
                                operations.Add(item.Number, new Operation(item.Number, item.Operation, intList, new List<int>()));
                            else
                                operations.Add(item.Number, new Operation(item.Number, item.Operation, new List<int>(), intList));
                        }
                        else
                        {
                            if (item.IsStart)
                                operations[item.Number].StartLines.Add(item.Line);
                            else
                                operations[item.Number].EndLines.Add(item.Line);
                        }
                    }

                    foreach (var item in operations)
                    {
                        string message = "";
                        if (item.Value.StartLines.Count > item.Value.EndLines.Count)
                        {
                            message = "Path: " + file.Key.ToString() + ": " + item.Value.Type + " number: " + item.Value.Number.ToString() + " is requested but not relesed";
                            //MessageBox.Show(message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            logFileContent = logFileContent + message + "\r\n";
                        }

                        if (item.Value.StartLines.Count < item.Value.EndLines.Count)
                        {
                            message = "Path: " + file.Key.ToString() + ": " + item.Value.Type + " number: " + item.Value.Number.ToString() + " is released but not requested";
                            //MessageBox.Show(message, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            logFileContent = logFileContent + message + "\r\n";
                        }
                        if (item.Value.StartLines.Count == item.Value.EndLines.Count)
                        {
                            int counter = 0;
                            foreach (int line in item.Value.StartLines)
                            {
                                if (item.Value.StartLines[counter] > item.Value.EndLines[counter])
                                {
                                    message = "Path: " + file.Key.ToString() + ": \r\n" + item.Value.Type + " number: " + item.Value.Number.ToString() + " is released before it is requested!";
                                    //MessageBox.Show(message, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    logFileContent = logFileContent + " " + message + "\r\n";
                                }
                                counter++;
                            }
                        }
                    }
                }
            }
        }

        private static void FillJobsList(List<OpenAndCloseCommand> allOperations)
        {
            List<string> files = new List<string>();
            foreach (var item in allOperations)
            {
                if (!files.Contains(item.File))
                    files.Add(item.File);
            }

            IDictionary<string, IDictionary<int, string>> values = new Dictionary<string, IDictionary<int, string>>();
            IDictionary<int, string> result = new Dictionary<int, string>();

            foreach (string item in files)
                foreach (var operation in allOperations.Where(x => x.File == item && x.Operation == "Job"))
                {
                    if (!values.Keys.Contains(Path.GetFileNameWithoutExtension(operation.File)))
                    {
                        IDictionary<int, string> currentJob = new Dictionary<int, string>();
                        currentJob.Add(operation.Number, operation.Description);
                        values.Add(Path.GetFileNameWithoutExtension(operation.File), currentJob);
                    }
                    else
                    {
                        if (!values[Path.GetFileNameWithoutExtension(operation.File)].Keys.Contains(operation.Number))
                            values[Path.GetFileNameWithoutExtension(operation.File)].Add(operation.Number, operation.Description);
                    }

                }
            foreach (var item in allOperations.Where(x => x.Operation == "Job"))
            {
                if (!result.Keys.Contains(item.Number))
                    result.Add(item.Number, item.Description);
            }
            Dictionary<int, string> jobs = new Dictionary<int, string>();
            jobs.Add(0, "");
            foreach (var item in result.OrderBy(i => i.Key))
                jobs.Add(item.Key, item.Value);
            GlobalData.Jobs = jobs;
        }

        private static IDictionary<string, List<string>> CorrectJobsComments(IDictionary<string, List<string>> resultSrcFiles)
        {
            IDictionary<string, List<string>> jobs = new Dictionary<string, List<string>>();
            IDictionary<string, List<string>> copyOfFiles = new Dictionary<string, List<string>>();
            foreach (var item in resultSrcFiles)
                copyOfFiles.Add(item);
            foreach (var file in resultSrcFiles)
            {
                List<string> commands = new List<string>();
                foreach (var item in file.Value)
                {
                    if (item.ToLower().Contains("job_finishwork") || item.ToLower().Contains("job_req"))
                    {
                        Regex numberRegex = new Regex(@"(?<=JobNr\s*=\s*)\d*", RegexOptions.IgnoreCase);
                        int number = int.Parse(numberRegex.Match(item).ToString());
                        Regex descriptionRegex = new Regex(@"(?<=DESC\s*=\s*)[a-zA-Z0-9\s_]*", RegexOptions.IgnoreCase);
                        string description = descriptionRegex.Match(item).ToString();
                        if (description != jobsWithDescription[number])
                        {
                            commands.Add(item.Replace("DESC=" + description, "DESC=" + jobsWithDescription[number]));
                        }
                        else
                            commands.Add(item);
                    }
                    else if (item.ToLower().Contains("plc_job"))
                    {
                        Regex numberRegex = new Regex(@"(?<=JobNum\s*\:\s*)\d+", RegexOptions.IgnoreCase);
                        if (numberRegex.IsMatch(item))
                        {
                            int number = int.Parse(numberRegex.Match(item).ToString());
                            Regex descriptionRegex = new Regex(@"(?<=Desc\s*\:\s*)[a-zA-Z0-9\s_]*", RegexOptions.IgnoreCase);
                            string description = descriptionRegex.Match(item).ToString();
                            if (description != jobsWithDescription[number])
                            {
                                Regex containDesc = new Regex(@"desc\s*\:", RegexOptions.IgnoreCase);
                                if (containDesc.IsMatch(item))
                                {
                                    string changedCommand = item.Replace("DESC=" + description.Trim(), "DESC=" + jobsWithDescription[number]);
                                    changedCommand = changedCommand.Replace("Desc:" + description.Trim(), "Desc:" + jobsWithDescription[number]);
                                    changedCommand = changedCommand.Replace("Plc_JobDesc=" + description.Trim(), "Plc_JobDesc=" + jobsWithDescription[number]);
                                    commands.Add(changedCommand);
                                }
                                else
                                {
                                    Regex replacementRegex = new Regex(@"JobNum\s*\:\s*\d+", RegexOptions.IgnoreCase);
                                    string commandToAdd = replacementRegex.Replace(item, "JobNum:" + number + " Desc:" + jobsWithDescription[number]);
                                    Regex paramRegex = new Regex(@"Plc_JobDesc\s*\=.*", RegexOptions.IgnoreCase);
                                    commandToAdd = paramRegex.Replace(commandToAdd, "Plc_JobDesc=" + jobsWithDescription[number]);
                                    commands.Add(commandToAdd);
                                }
                            }
                            else
                                commands.Add(item);
                        }
                    }
                    else
                        commands.Add(item);
                }
                jobs.Add(file.Key, commands);
            }
            return jobs;
        }



        private static void FindDatFiles(IDictionary<string, string> srcFiles, List<string> datFiles)
        {
            IDictionary<string, string> currentsrcFiles = new Dictionary<string, string>();
            IDictionary<string, string> copyOfCurrentFiles = new Dictionary<string, string>();
            foreach (var item in srcFiles)
                currentsrcFiles.Add(item);
            foreach (var item in srcFiles)
            {
                if (item.Key.ToLower().Contains(ConfigurationManager.AppSettings["Org" + GlobalData.ControllerType.Replace(" ", "_")]) | item.Key.ToLower().Contains(ConfigurationManager.AppSettings["Sub" + GlobalData.ControllerType.Replace(" ", "_")]) | item.Key.ToLower().Contains(ConfigurationManager.AppSettings["Special" + GlobalData.ControllerType.Replace(" ", "_")]) | item.Key.ToLower().Contains(ConfigurationManager.AppSettings["tch_auto" + GlobalData.ControllerType.Replace(" ", "_")]) | item.Key.ToLower().Contains(ConfigurationManager.AppSettings["Init" + GlobalData.ControllerType.Replace(" ", "_")]))
                    currentsrcFiles.Remove(item);
            }
            foreach (var item in currentsrcFiles)
                copyOfCurrentFiles.Add(item);

            foreach (var item in ConfigurationManager.AppSettings["SystemFile" + GlobalData.ControllerType.Replace(" ", "_")].Split(',').ToArray())
            {
                foreach (var item2 in copyOfCurrentFiles.Where(x => x.Key.ToLower().Contains(item)))
                    currentsrcFiles.Remove(item2);
            }
            foreach (string srcFile in currentsrcFiles.Keys)
            {
                string message = "";
                if (!datFiles.Contains(srcFile.Replace(".src", ".dat")))
                {
                    message = "Path: " + Path.GetFileName(srcFile) + ": \nNo Dat file found!";
                    logFileContent = logFileContent + " " + message + "\r\n";
                    MessageBox.Show("Path: " + Path.GetFileName(srcFile) + " \nNo .dat file found!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private static void CheckToolsAndBases(IDictionary<string, List<string>> srcFiles, List<string> datFiles)
        {
            bool multiToolsOrBasesFoundInDatFile = false;
            foreach (var file in datFiles.Where(x => !x.Contains("global")))
            {
                IDictionary<int, List<string>> toolAndPoint = new Dictionary<int, List<string>>();
                IDictionary<int, List<string>> baseAndPoint = new Dictionary<int, List<string>>();
                List<int> tools = new List<int>();
                List<int> bases = new List<int>();
                string line = "";
                var reader = new StreamReader(file);
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    if (line.Contains(" FDAT") && !line.Contains("FDAT_"))
                    {
                        Regex regex = new Regex(@"(?<=FDAT\s+).*(?=\s*\=)", RegexOptions.IgnoreCase);
                        if (UsedDats[file].FDAT.Contains(regex.Match(line).ToString()))
                        {
                            Regex regexTool = new Regex(@"(?<=TOOL_NO\s+)[0-9]*");
                            Match matchTool = regexTool.Match(line);
                            Regex regexBase = new Regex(@"(?<=BASE_NO\s+)[0-9]*");
                            Match matchBase = regexBase.Match(line);
                            if (!tools.Contains(int.Parse(matchTool.ToString())))
                                tools.Add(int.Parse(matchTool.ToString()));
                            if (!toolAndPoint.Keys.Contains(int.Parse(matchTool.ToString())))
                                toolAndPoint[int.Parse(matchTool.ToString())] = new List<string>();
                            toolAndPoint[int.Parse(matchTool.ToString())].Add(regex.Match(line).ToString());
                            if (!bases.Contains(int.Parse(matchBase.ToString())))
                                bases.Add(int.Parse(matchBase.ToString()));
                            if (!baseAndPoint.Keys.Contains(int.Parse(matchBase.ToString())))
                                baseAndPoint[int.Parse(matchBase.ToString())] = new List<string>();
                            baseAndPoint[int.Parse(matchBase.ToString())].Add(regex.Match(line).ToString());
                        }
                    }
                }
                reader.Close();

                if (tools.Count > 1)
                    CreateMultiToolOrBaseString(toolAndPoint, file, true);
                if (bases.Count > 1)
                    CreateMultiToolOrBaseString(baseAndPoint, file, false);

            }
            if (multiToolsOrBasesFoundInDatFile)
                MessageBox.Show("Multiple tools or bases in paths found. See log for details.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private static void CreateMultiToolOrBaseString(IDictionary<int, List<string>> toolOrBaseAndPoint, string file, bool isTool)
        {
            string[] toolOrBase = { "", "" };
            if (isTool)
                toolOrBase = new string[] { "tools", "tool" };
            else
                toolOrBase = new string[] { "bases", "base" };
            string currentString = "Mutiple " + toolOrBase[0] + " found in file " + (Path.GetFileName(file)).Replace(".dat", "") + ". Points with " + toolOrBase[1] + " number:\r\n";
            foreach (var toolorbase in toolOrBaseAndPoint)
            {

                string currentToolOrBase = toolorbase.Key + " - points: ";
                {
                    foreach (string point in toolorbase.Value)
                    {
                        currentToolOrBase += point + ", ";
                    }
                }
                currentToolOrBase = currentToolOrBase.Substring(0, currentToolOrBase.Length - 2);
                currentString += currentToolOrBase + "\r\n";
            }
            logFileContent = logFileContent + currentString + "\r\n";
        }

        public static void FindUnusedDataInDatFiles(IDictionary<string, List<string>> srcFiles, List<string> datFiles)
        {
            string currentline = "", currentfile = "";
            string errorLine = "";
            try
            {
                IDictionary<string, FileValidationData.Dats> usedDats = new Dictionary<string, FileValidationData.Dats>();
                IDictionary<string, FileValidationData.Dats> unusedDats = new Dictionary<string, FileValidationData.Dats>();
                IDictionary<string, FileValidationData.Dats> usedDatsClone = new Dictionary<string, FileValidationData.Dats>();
                IDictionary<string, FileValidationData.Dats> unusedDatsClone = new Dictionary<string, FileValidationData.Dats>();
                UnusedDats = new Dictionary<string, Dats>();
                UsedDats = new Dictionary<string, Dats>();
                IDictionary<string, List<Point>> pointsInSrcs = new Dictionary<string, List<Point>>();
                IDictionary<string, Dats> pointsInDats = new Dictionary<string, Dats>();
                foreach (var file in srcFiles)
                {
                    currentfile = file.Key;

                    List<Point> pointsInSrc = new List<Point>();

                    foreach (string command in file.Value)
                    {
                        currentline = command;
                        if (command.ToLower().Contains("startpos"))
                        { }
                        if (command.ToLower().Contains("ptp ") || command.ToLower().Contains("lin ") || command.ToLower().Contains("search_way") || command.ToLower().Contains("swr_reloadpos"))
                        {
                            Point currentPoint = new Point();
                            if (command.ToLower().Contains("home"))
                            {
                                currentPoint.IsHome = true;
                                currentPoint.ToolNr = 999;
                                currentPoint.ToolName = "ToolNotFound";
                                currentPoint.BaseNr = 999;
                                currentPoint.BaseName = "BaseNotFound";
                            }
                            else
                            {
                                currentPoint.IsHome = false;
                                if (command.ToLower().Contains("job.") || command.ToLower().Contains("collision") || command.ToLower().Contains("spot."))
                                {
                                    Regex regexToolNr = new Regex(@"(?<=Tool\s*(\[|=)\s*)\d*", RegexOptions.IgnoreCase);
                                    currentPoint.ToolNr = int.Parse((regexToolNr.Match(command)).ToString());
                                    Regex regexBaseNr = new Regex(@"(?<=Base\s*(\[|=)\s*)\d*", RegexOptions.IgnoreCase);
                                    currentPoint.BaseNr = int.Parse((regexBaseNr.Match(command)).ToString());
                                    currentPoint.BaseName = "";
                                    currentPoint.ToolName = "";
                                }
                                else
                                {
                                    if (command.ToLower().Contains("tool") && !command.Contains(";#"))
                                    {
                                        int toolNr = 999;
                                        errorLine = command;
                                        Regex regexToolNr = new Regex(@"(?<=Tool\s*(\[|\=)\s*)\d*", RegexOptions.IgnoreCase);
                                        if (int.TryParse(regexToolNr.Match(command).ToString(), out toolNr))
                                        {
                                            currentPoint.ToolNr = toolNr;
                                            Regex regexToolName = new Regex(@"(?<=Tool\s*\[\s*[0-9]*\s*\]\s*:\s*)[a-zA-Z0-9_]*", RegexOptions.IgnoreCase);
                                            currentPoint.ToolName = (regexToolName.Match(command)).ToString();
                                        }
                                    }
                                    else
                                    {
                                        currentPoint.ToolNr = 999;
                                        currentPoint.ToolName = "ToolNotFound";
                                    }
                                    if (command.ToLower().Contains("base") && !command.ToLower().Contains("search_") && !command.Contains(";#"))
                                    {
                                        Regex regexBaseNr = new Regex(@"(?<=Base\s*(\[|\=)\s*)\d*", RegexOptions.IgnoreCase);
                                        if (!command.ToLower().Replace(" ", "").Contains("palposrel"))
                                        {
                                            if (regexBaseNr.IsMatch(command))
                                            {
                                                if (!string.IsNullOrEmpty(regexBaseNr.Match(command).ToString()))
                                                {
                                                    currentPoint.BaseNr = int.Parse((regexBaseNr.Match(command)).ToString());
                                                    Regex regexBaseName = new Regex(@"(?<=Base\s*\[\s*\d*\s*\]\s*:\s*).*(?=\s*;)", RegexOptions.IgnoreCase);
                                                    currentPoint.BaseName = (regexBaseName.Match(command)).ToString();
                                                }
                                            }
                                            else
                                            { }
                                        }
                                    }
                                    else
                                    {
                                        currentPoint.BaseNr = 999;
                                        currentPoint.BaseName = "BaseNotFound";
                                    }
                                }

                            }
                            //Regex regexName = new Regex(@"(?<=(PTP|LIN)\s+)X[a-zA-Z0-9_]*", RegexOptions.IgnoreCase);
                            Regex regexName = new Regex(@"(?<=PTP\s*|LIN\s*|Search\s*\(\s*\d+\s*,\s*|Search\s*\(\s*\d+\s*,\s*[$a-zA-Z0-9_]*\s*,\s*|Reload_(PTP|LIN)\s*,\s*|Pal_PosRel(Ptp|Lin)\s+\(\s*\d+\s*,\s*#\w+\s*,\s*\d+\s*,(|-)\d+\s*,\s*|Pal_StartPos\s+\(\s*\d+\s*,\s*#\w+\s*,\s*)X[a-zA-Z0-9_\-]*", RegexOptions.IgnoreCase);
                            currentPoint.Name = (regexName.Match(command)).ToString();
                            Regex regexFDAT = new Regex(@"(?<=FDAT_ACT\s*\=\s*)[a-zA-Z0-9_]+", RegexOptions.IgnoreCase);
                            currentPoint.FDAT = (regexFDAT.Match(command)).ToString();
                            Regex regexLDAT = new Regex(@"(?<=LDAT_ACT\s*\=\s*)[a-zA-Z0-9_]+", RegexOptions.IgnoreCase);
                            currentPoint.LDAT = (regexLDAT.Match(command)).ToString();
                            Regex regexPDAT = new Regex(@"(?<=PDAT_ACT\s*\=\s*)[a-zA-Z0-9_]+", RegexOptions.IgnoreCase);
                            currentPoint.PDAT = (regexPDAT.Match(command)).ToString();

                            pointsInSrc.Add(currentPoint);
                        }
                    }
                    pointsInSrcs.Add(file.Key, pointsInSrc);
                }
                foreach (var file in datFiles)
                {
                    Dats pointsInDat = new Dats();
                    var reader = new StreamReader(file);
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (line.ToLower().Contains("decl fdat"))
                        {
                            Regex matchFDAT = new Regex(@"(?<=FDAT\s+).*(?=\s*\=)", RegexOptions.IgnoreCase);
                            pointsInDat.FDAT.Add((matchFDAT.Match(line)).ToString());
                        }
                        if (line.ToLower().Contains("decl pdat"))
                        {
                            Regex matchPDAT = new Regex(@"(?<=PDAT\s+).*(?=\s*\=)", RegexOptions.IgnoreCase);
                            pointsInDat.PDAT.Add((matchPDAT.Match(line)).ToString());
                        }
                        if (line.ToLower().Contains("decl ldat"))
                        {
                            Regex matchLDAT = new Regex(@"(?<=LDAT\s+).*(?=\s*\=)", RegexOptions.IgnoreCase);
                            pointsInDat.LDAT.Add((matchLDAT.Match(line)).ToString());
                        }
                        if (line.ToLower().Contains("decl e6pos"))
                        {
                            Regex matchE6POS = new Regex(@"(?<=E6POS\s+).*(?=\s*\=)", RegexOptions.IgnoreCase);
                            pointsInDat.E6POS.Add((matchE6POS.Match(line)).ToString());
                        }
                        if (line.ToLower().Contains("decl e6axis"))
                        {
                            Regex matchE6AXIS = new Regex(@"(?<=E6AXIS\s+).*(?=\s*\=)", RegexOptions.IgnoreCase);
                            pointsInDat.E6AXIS.Add((matchE6AXIS.Match(line)).ToString());
                        }
                    }

                    pointsInDats.Add(file, pointsInDat);
                }
                foreach (var item in pointsInDats)
                    unusedDats.Add(item);
                foreach (var item in pointsInDats)
                    usedDats.Add(item.Key, new Dats());
                //foreach (var item in unusedDats)
                unusedDatsClone = new Dictionary<string, FileValidationData.Dats>(unusedDats);
                //foreach (var item in usedDats)
                usedDatsClone = new Dictionary<string, FileValidationData.Dats>(usedDats);
                foreach (var dat in pointsInDats)
                {                   
                    foreach (var src in pointsInSrcs.Where(x => x.Key.Replace(".src", "").Contains(dat.Key.Replace(".dat", ""))))
                    {
                        foreach (Point point in src.Value)
                        {
                            if (point.Name.ToLower().Contains("xservice"))
                            { }
                            //foreach(var item in dat.Value.E6POS.Where(x=>x.ToLower().Contains(point.Name.ToLower())))
                            //if (dat.Value.E6POS.Contains(point.Name))
                            if (dat.Value.E6POS.Any(s => s.Equals(point.Name, StringComparison.OrdinalIgnoreCase)))                            
                            {
                                usedDats[dat.Key].E6POS.Add(point.Name);
                                var itemToRemove = unusedDats[dat.Key].E6POS.Where(s => s.Equals(point.Name, StringComparison.OrdinalIgnoreCase)).ToList();
                                if (itemToRemove.Count > 0)
                                    unusedDats[dat.Key].E6POS.Remove(itemToRemove[0]);
                            }
                            //foreach (var item in dat.Value.E6AXIS.Where(x => x.ToLower().Contains(point.Name.ToLower())))
                            //if (dat.Value.E6AXIS.Contains(point.Name))
                            if (dat.Value.E6AXIS.Any(s => s.Equals(point.Name, StringComparison.OrdinalIgnoreCase)))
                            {
                                usedDats[dat.Key].E6AXIS.Add(point.Name);
                                var itemToRemove = unusedDats[dat.Key].E6AXIS.Where(s => s.Equals(point.Name, StringComparison.OrdinalIgnoreCase)).ToList();
                                if (itemToRemove.Count > 0)
                                    unusedDats[dat.Key].E6AXIS.Remove(itemToRemove[0]);
                            }
                            //foreach (var item in dat.Value.FDAT.Where(x => x.ToLower().Contains(point.Name.ToLower())))
                            //if (dat.Value.FDAT.Contains(point.FDAT))
                            if (dat.Value.FDAT.Any(s => s.Equals(point.FDAT, StringComparison.OrdinalIgnoreCase)))
                            {
                                usedDats[dat.Key].FDAT.Add(point.FDAT);
                                var itemToRemove = unusedDats[dat.Key].FDAT.Where(s => s.Equals(point.FDAT, StringComparison.OrdinalIgnoreCase)).ToList();
                                if (itemToRemove.Count > 0)
                                    unusedDats[dat.Key].FDAT.Remove(itemToRemove[0]);
                            }
                            //foreach (var item in dat.Value.PDAT.Where(x => x.ToLower().Contains(point.Name.ToLower())))
                            //if (dat.Value.PDAT.Contains(point.PDAT))
                            if (dat.Value.PDAT.Any(s => s.Equals(point.PDAT, StringComparison.OrdinalIgnoreCase)))
                            {
                                usedDats[dat.Key].PDAT.Add(point.PDAT);
                                var itemToRemove = unusedDats[dat.Key].PDAT.Where(s => s.Equals(point.PDAT, StringComparison.OrdinalIgnoreCase)).ToList();
                                if (itemToRemove.Count > 0)
                                    unusedDats[dat.Key].PDAT.Remove(itemToRemove[0]);
                            }
                            //foreach (var item in dat.Value.LDAT.Where(x => x.ToLower().Contains(point.Name.ToLower())))
                            //if (dat.Value.LDAT.Contains(point.LDAT))
                            if (dat.Value.LDAT.Any(s => s.Equals(point.LDAT, StringComparison.OrdinalIgnoreCase)))
                            {
                                usedDats[dat.Key].LDAT.Add(point.LDAT);
                                var itemToRemove = unusedDats[dat.Key].LDAT.Where(s => s.Equals(point.LDAT, StringComparison.OrdinalIgnoreCase)).ToList();
                                if (itemToRemove.Count > 0)
                                    unusedDats[dat.Key].LDAT.Remove(itemToRemove[0]);
                            }
                        }
                    }
                }
                bool unusedDataFound = false;

                CreateUnusedDatsLog(unusedDats);

                foreach (var dat in unusedDats.Where(x => (x.Value.E6AXIS.Count > 0) | (x.Value.E6POS.Count > 0) | (x.Value.FDAT.Count > 0) | (x.Value.LDAT.Count > 0) | (x.Value.PDAT.Count > 0)))
                {
                    unusedDataFound = true;
                    if (dat.Value.E6AXIS.Count > 0)
                        WriteUnusedDataToLogFile(dat, "E6AXIS");
                    if (dat.Value.E6POS.Count > 0)
                        WriteUnusedDataToLogFile(dat, "E6POS");
                    if (dat.Value.FDAT.Count > 0)
                        WriteUnusedDataToLogFile(dat, "FDAT");
                    if (dat.Value.PDAT.Count > 0)
                        WriteUnusedDataToLogFile(dat, "PDAT");
                    if (dat.Value.LDAT.Count > 0)
                        WriteUnusedDataToLogFile(dat, "LDAT");
                }
                if (unusedDats != null)
                    UnusedDats = unusedDats;
                if (usedDats != null)
                    UsedDats = usedDats;
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex, errorLine);
            }
        }

        public class CloneableDictionary<TKey, TValue> : Dictionary<TKey, TValue> where TValue : ICloneable
        {
            public IDictionary<TKey, TValue> Clone()
            {
                CloneableDictionary<TKey, TValue> clone = new CloneableDictionary<TKey, TValue>();

                foreach (KeyValuePair<TKey, TValue> pair in this)
                {
                    clone.Add(pair.Key, (TValue)pair.Value.Clone());
                }

                return clone;
            }
        }

        private static void CreateUnusedDatsLog(IDictionary<string, Dats> unusedDats)
        {
            string unusedDatsString = "";
            UnusedDataPresent = false;

            foreach (var unusedElement in unusedDats.Where(x => ((x.Value.E6AXIS.Count + x.Value.E6POS.Count + x.Value.FDAT.Count + x.Value.LDAT.Count + x.Value.PDAT.Count) > 0)))
            {
                unusedDatsString += "File: " + Path.GetFileName(unusedElement.Key) + "\r\n";
                foreach (string element in unusedElement.Value.E6AXIS)
                {
                    unusedDatsString += "Not used E6AXIS: " + element + "\r\n";
                    UnusedDataPresent = true;
                }
                foreach (string element in unusedElement.Value.E6POS)
                {
                    unusedDatsString += "Not used E6POS: " + element + "\r\n";
                    UnusedDataPresent = true;
                }
                foreach (string element in unusedElement.Value.FDAT)
                {
                    unusedDatsString += "Not used FDAT: " + element + "\r\n";
                    UnusedDataPresent = true;
                }
                foreach (string element in unusedElement.Value.PDAT)
                {
                    unusedDatsString += "Not used PDAT: " + element + "\r\n";
                    UnusedDataPresent = true;
                }
                foreach (string element in unusedElement.Value.LDAT)
                {
                    unusedDatsString += "Not used LDAT: " + element + "\r\n";
                    UnusedDataPresent = true;
                }
            }

            if (!string.IsNullOrEmpty(unusedDatsString))
            {
                string directory = Path.GetDirectoryName(CommonLibrary.CommonGlobalData.ConfigurationFileName);
                File.Delete(directory + "\\unusedDats.txt");
                using (StreamWriter sw = File.AppendText(directory + "\\unusedDats.txt"))
                {
                    sw.Write(unusedDatsString);
                    sw.Close();
                    MessageBox.Show("Log with not used data has been created at " + directory + "\\unusedDats.txt");
                    System.Diagnostics.Process.Start(directory + "\\unusedDats.txt");
                }
            }
        }

        private static void WriteUnusedDataToLogFile(KeyValuePair<string, Dats> dat, string v)
        {
            return;
            Type myType = typeof(Dats);
            PropertyInfo myPropInfo = myType.GetProperty(v);
            string foundFiles = "";

            foreach (string item in dat.Value.E6AXIS)
                foundFiles += item + " ";
            string message = "Not used E6AXIS definitions found in file " + Path.GetFileName(dat.Key) + ". Points found: " + foundFiles;
            logFileContent = logFileContent + DateTime.Now.ToString() + " " + message + "\r\n";
        }

        public static List<CollisionWithoutDescr> GetCollisions(IDictionary<string, string> srcFiles)
        {
            List<CollisionWithoutDescr> result = new List<CollisionWithoutDescr>();
            int number = 0;
            Regex regexNr = new Regex(@"(?<=(COLL_SAFETY_|Plc_CollSafetyReq\d*|Plc_CollSafetyClear\d*)[a-zA-Z_]*\s*\(\s*)[0-9]*(?=\s*\))", RegexOptions.IgnoreCase);
            foreach (var file in srcFiles)
            {
                var reader = new StreamReader(file.Key);
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (line.ToLower().Contains("coll_safety_") || line.ToLower().Contains("collsafety") && !line.ToLower().Contains("init"))
                        if (line.ToLower().Trim().Replace(" ", "").Substring(0, 1) != ";" && !line.ToLower().Replace(" ", "").Contains("coll_nr:in") && !line.ToLower().Replace(" ", "").Contains("(coll_nr)"))
                        {
                            Match match = regexNr.Match(line);
                            int.TryParse(match.ToString(), out number);
                            if (number == 0)
                            {
                                MessageBox.Show("Coll 0 found!!!!!!");
                            }
                            if (number != 255)
                            {
                                if (line.ToLower().Contains("coll_safety_req") || line.ToLower().Contains("collsafetyreq"))
                                    result.Add(new CollisionWithoutDescr(number, "Request"));
                                else
                                    result.Add(new CollisionWithoutDescr(number, "Release"));
                            }
                        }

                }
            }
            return result;
        }

        internal static IDictionary<string, List<string>> PrepareDatFiles(Dictionary<string, string> filesToCopy)
        {
            List<string> pointNames = GetPointNamesFromFDATs();
            List<string> listOfGlobals = new List<string>();
            List<string> filteredListOfGlobals = new List<string>();
            List<string> globalsNames = new List<string>();
            List<string> listOfE6AXIS = new List<string>();
            if (GlobalFiles != null)
                GlobalFiles.ForEach(x => listOfGlobals.Add(x.Content));
            foreach (string item in listOfGlobals.Where(x => x.Contains("DECL")))
                filteredListOfGlobals.Add(item);

            foreach (string item in filteredListOfGlobals.Where(x => x.Contains("E6AXIS")))
            {
                Regex regex = new Regex(@"(?<=DECL\s+(E6AXIS|GLOBAL E6AXIS)\s+).*(?=\=.*)");
                Match match = regex.Match(item);
                string str = "F" + match.ToString().Remove(0, 1);

                listOfE6AXIS.Add(str);
            }



            foreach (string item in filteredListOfGlobals)
            {
                Regex regex = new Regex(@"(?<=DECL\s+(E6AXIS|E6POS|FDAT|GLOBAL E6AXIS|GLOBAL E6POS|GLOBAL FDAT)\s+).*(?=\=.*)");
                Match match = regex.Match(item);
                globalsNames.Add(match.ToString());
            }

            IDictionary<string, List<string>> result = new Dictionary<string, List<string>>();

            foreach (var file in filesToCopy.Where(x => x.Key.Contains(".dat")))
            {
                ToolAndBase toolAndBase = GetToolAndBaseFromFile(file);
                List<string> lines = new List<string>();
                IDictionary<string, List<string>> currentDatFile = new Dictionary<string, List<string>>();
                StreamReader reader = new StreamReader(file.Key);
                while (!reader.EndOfStream)
                {
                    bool addLine = true;
                    string line = reader.ReadLine();
                    foreach (string name in globalsNames)
                    {
                        if (line.Contains(name))
                        {
                            addLine = false;
                            break;
                        }
                    }
                    if (line.ToLower().Contains("pdat"))
                    {
                        foreach (string point in pointNames)
                        {
                            if (line.ToLower().Contains(point.ToLower()))
                            {
                                foreach (var item in GlobalData.GlobalFDATs.Where(x => x.Contains(point)))
                                {
                                    if (toolAndBase.Tool == (GetToolAndBase(item)).Tool && toolAndBase.Base == (GetToolAndBase(item)).Base)
                                    {
                                        line += "\r\n" + item;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (addLine)
                        lines.Add(line);
                }

                result.Add(file.Key, lines);
            }
            //TEMP
            if (GlobalData.ControllerType != "KRC4 Not BMW")
            {
                result = ClearHeader(result);
                result = AddHeader(result, true);
            }
            return result;
        }

        private static ToolAndBase GetToolAndBaseFromFile(KeyValuePair<string, string> file)
        {
            string pdatLine = "";
            ToolAndBase result = new ToolAndBase();
            StreamReader reader = new StreamReader(file.Key);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (line.ToLower().Contains("pdat"))
                    pdatLine = line;
                if (line.ToLower().Contains("fdat") && !line.ToLower().Contains("defdat"))
                {
                    if (!pdatLine.ToLower().Contains("pos"))
                    {
                        result = GetToolAndBase(line);
                        break;
                    }
                    else
                        result = GetToolAndBase(line);
                }
            }
            return result;
        }

        private static FileValidationData.ToolAndBase GetToolAndBase(string line)
        {
            ToolAndBase result = new ToolAndBase() { Tool = 0, Base = 0};
            Regex toolRegex = new Regex(@"(?<=TOOL_NO\s+)\d+", RegexOptions.IgnoreCase);
            Regex baseRegex = new Regex(@"(?<=BASE_NO\s+)\d+", RegexOptions.IgnoreCase);
            if (toolRegex.IsMatch(line))
                result.Tool = int.Parse(toolRegex.Match(line).ToString());
            if (baseRegex.IsMatch(line))
                result.Base = int.Parse(baseRegex.Match(line).ToString());
            return result;
        }

        private static List<string> GetPointNamesFromFDATs()
        {
            List<string> result = new List<string>();
            Regex getpointName = new Regex(@"(?<=FDAT\s+).*(?=\=)", RegexOptions.IgnoreCase);
            foreach (var item in GlobalData.GlobalFDATs)
            {
                if (!result.Contains(getpointName.Match(item).ToString().Remove(0, 1)))
                    result.Add(getpointName.Match(item).ToString().Remove(0, 1));
            }
            return result;
        }

        internal static void FilterDataFromBackup(bool isSrc, IDictionary<string, string> srcFiles = null, List<string> datFiles = null)
        {
            IDictionary<string, string> result = new Dictionary<string, string>();
            if (isSrc)
                result = srcFiles;
            else
                datFiles.ForEach(x => result.Add(x, ""));
            foreach (var item in result)
            {
                string[] omitedDataInCheck = ConfigurationManager.AppSettings["OmitedDataInCheck" + GlobalData.ControllerType.Replace(" ", "_")].Split(',').Select(s => s.Trim()).ToArray();
                bool flag = false;
                foreach (string omitedData in omitedDataInCheck)
                {
                    if (ConfigurationManager.AppSettings[omitedData]!=null && item.Key.Contains(ConfigurationManager.AppSettings[omitedData]))
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    if (isSrc)
                    {
                        if (!DataToProcess.SrcFiles.Contains(item))
                        {
                            DataToProcess.SrcFiles.Add(item);
                            //Result = DataToProcess.SrcFiles;
                        }
                    }
                    else
                    {
                        if (!DataToProcess.DatFiles.Contains(item.Key))
                            DataToProcess.DatFiles.Add(item.Key);
                    }

                }
            }
        }

        internal static Dictionary<string, string> RemoveNotUsedFilesFromFilteredFiles(Dictionary<string, string> filteredFiles)
        {
            string[] omitedDataInCheck = ConfigurationManager.AppSettings["OmitedDataInCheck" + GlobalData.ControllerType.Replace(" ", "_")].Split(',').Select(s => s.Trim()).ToArray();
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (var item in filteredFiles)
            {
                string itemToLower = item.Key.ToLower();
                bool flag = false;
                foreach (string omitedData in omitedDataInCheck)
                {
                    if (ConfigurationManager.AppSettings[omitedData] != null && itemToLower.Contains(ConfigurationManager.AppSettings[omitedData].ToLower()))
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    result.Add(item.Key, item.Value);
                }
            }

            return result;
        }

        public static Dictionary<string, string> ReplaceDataContent(IDictionary<string, string> filesAndContent, IDictionary<string, List<string>> datFilesToCopy)
        {
            bool replaceDats = false, unusedDataFound = false;
            foreach (var item in UnusedDats)
                if (item.Value.E6AXIS.Count > 0 | item.Value.E6POS.Count > 0 | item.Value.FDAT.Count > 0 | item.Value.PDAT.Count > 0 | item.Value.LDAT.Count > 0)
                {
                    unusedDataFound = true;
                    break;
                }
            //if (unusedDataFound)
            //{
            //    DialogResult dialog = MessageBox.Show("Do you want to remove not used elements from dat files?", "Remove not used dats?", MessageBoxButtons.YesNo);
            //    if (dialog == DialogResult.Yes)
            //        replaceDats = true;
            //}


            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (var item in filesAndContent.Where(x => x.Key.Contains(".src") && Result.Keys.Contains(x.Key)))
                result.Add(item.Key, Result[item.Key]);

            if (replaceDats)
            {
                IDictionary<string, List<string>> datFiles = new Dictionary<string, List<string>>();
                foreach (var item in datFilesToCopy)
                {
                    List<string> currentString = new List<string>();
                    foreach (string line in item.Value)
                        currentString.Add(line);
                    datFiles[item.Key] = currentString;
                }
                ICollection<Dats> props = UnusedDats.Values;
                foreach (var file in UnusedDats)
                {
                    foreach (PropertyInfo prop in typeof(FileValidationData.Dats).GetProperties())
                    {
                        var currentList = prop.GetValue(file.Value);
                        foreach (string item in (List<string>)currentList)
                        {
                            if (datFilesToCopy.Keys.Contains(file.Key))
                            {
                                foreach (string line in datFilesToCopy[file.Key].Where(x => x.Contains(item)))
                                {
                                    Regex regex = new Regex(@"(?<=\s+)[a-zA-Z0-9_]*(?=\s*=)");
                                    string match = regex.Match(line).ToString();
                                    if (item == match)
                                        datFiles[file.Key].Remove(line);
                                }
                            }
                        }
                    }
                }
                datFilesToCopy = datFiles;
            }


            foreach (var item in filesAndContent.Where(x => x.Key.Contains(".dat")))
            {
                bool addToFile = true;
                string currentString = "";
                foreach (string line in datFilesToCopy[item.Key])
                {
                    if (GlobalData.E6axisGlobalsfound != null && line.ToLower().Contains("e6axis"))
                    {
                        foreach (var position in GlobalData.E6axisGlobalsfound)
                        {
                            string posName = new Regex(@"(?<=E6AXIS\s+)[a-zA-Z0-9_]*", RegexOptions.IgnoreCase).Match(position).ToString();
                            if (line.ToLower().Contains(posName.ToLower()))
                            {
                                addToFile = false;
                                break;
                            }
                            else
                                addToFile = true;
                        }
                    }
                    else
                        addToFile = true;

                    if (addToFile)
                        currentString += line + "\r\n";
                }
                result.Add(item.Key, currentString);
            }
            return result;
        }

        private static KeyValuePair<string, List<string>> FindSwitchGroups(KeyValuePair<string, List<string>> file)
        {
            bool addLine = false;
            Regex switchRegex = new Regex(@"switch\s+[a-zA-Z0-9_\$]*", RegexOptions.IgnoreCase);
            List<string> switchString = new List<string>();
            //List<List<string>> list = new List<List<string>>();
            List<string> copyoffile = new List<string>();
            foreach (string command in file.Value)
            {
                if (switchRegex.IsMatch(command.ToLower()) && !command.ToLower().Contains("endswitch"))
                {
                    addLine = true;
                }
                if (addLine)
                    switchString.Add(command);
                else
                    copyoffile.Add(command);
                if (command.ToLower().Contains("endswitch"))
                {
                    addLine = false;
                    string currentString = "";
                    foreach (string item in switchString)
                    {
                        currentString += item;

                    }
                    copyoffile.Add(currentString);
                    //list.Add(switchString);
                    switchString = new List<string>();
                }
            }
            KeyValuePair<string, List<string>> switchStrings = new KeyValuePair<string, List<string>>(file.Key, copyoffile);

            return switchStrings;
        }

        public static void ChangeNameIfDefault()
        {
            
            var vm = new ChangeNameViewModel();
            while (vm.Name.ToLower().Contains("default"))
            {
                ChangeName sW = new ChangeName(vm);
                var dialogResult = sW.ShowDialog();
            }
        }

        private static IDictionary<string, List<string>> CheckToolNames(IDictionary<string, List<string>> resultSrcFiles)
        {
            IDictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            List<string> toolnames = ConfigurationManager.AppSettings["ToolNames" + GlobalData.ControllerType.Replace(" ", "_")].Split(',').Select(s => s.Trim()).ToList();
            IDictionary<int, string> toolNrAndName = new Dictionary<int, string>();
            int counter = 0;
            foreach (string tool in toolnames)
            {
                toolNrAndName.Add(counter, tool);
                counter++;
            }
            GlobalData.ToolsAndNamesFromStandar = toolNrAndName;
            IDictionary<int, List<string>> foundTools = new SortedDictionary<int, List<string>>();

            foreach (var srcFile in resultSrcFiles.Where(x => x.Key.Contains(".src")))
            {
                foreach (string command in srcFile.Value.Where(x => x.ToLower().Contains("ptp") || x.ToLower().Contains("lin")))
                {
                    Regex regexToolNr = new Regex(@"(?<=Tool\s*(\[|=)\s*)\d*", RegexOptions.IgnoreCase);
                    if (regexToolNr.Match(command).ToString() != "")
                    {
                        int toolNr = int.Parse(regexToolNr.Match(command).ToString());
                        Regex regexToolName = new Regex(@"(?<=Tool\s*\[\d+\s*\]\s*:\s*)[a-zA-Z0-9_\-]*", RegexOptions.IgnoreCase);
                        if (!foundTools.Keys.Contains(toolNr))
                            foundTools.Add(toolNr, new List<string>());

                        string toolName = regexToolName.Match(command).ToString();
                        if (!foundTools[toolNr].Contains(toolName) && toolName != "")
                            foundTools[toolNr].Add(regexToolName.Match(command).ToString());
                    }
                }
            }

            IDictionary<int, string> correctedTools = new Dictionary<int, string>();
            foreach (var tool in foundTools)
            {
                if (tool.Value.Count == 0)
                    tool.Value.Add("Unknown tool");
                string selectedToolName = tool.Value[0].ToLower();
                Regex toolRegex = new Regex(@toolNrAndName[tool.Key], RegexOptions.IgnoreCase);
                if (!toolRegex.IsMatch(selectedToolName))
                //if (!(selectedToolName == toolnames[tool.Key].Replace("[x|c]", "x").ToLower() || selectedToolName == toolnames[tool.Key].Replace("[x|c]", "c").ToLower()))
                {
                    var vm = new RenameToolViewModel(tool);
                    //if (toolnames[tool.Key].Contains("|"))
                    //{
                    //    vm.Pair.Value.Remove(toolnames[tool.Key]);
                    //    Regex findAlternatives = new Regex(@"\[.*\]", RegexOptions.IgnoreCase);
                    //    Regex findOptionsRegex = new Regex(@"(?<=(\[|\|))\w*", RegexOptions.IgnoreCase);
                    //    MatchCollection matches = findOptionsRegex.Matches(toolnames[tool.Key]);
                    //    foreach (Match match in matches)
                    //    {
                    //        vm.Pair.Value.Add(toolnames[tool.Key].ToString().Replace(findAlternatives.Match(toolnames[tool.Key]).ToString(), match.ToString()));
                    //    }
                    //}
                    RenameTool sW = new RenameTool(vm);
                    if (GlobalData.ControllerType != "KRC4 Not BMW")
                    {
                        var dialogResult = sW.ShowDialog();
                        correctedTools.Add(tool.Key, vm.CorrectedName);
                    }
                    else
                        correctedTools.Add(tool.Key, tool.Value[0]);
                }
                else
                    correctedTools.Add(tool.Key, tool.Value[0]);
            }

            foreach (var srcFile in resultSrcFiles.Where(x => x.Key.Contains(".src")))
            {
                result.Add(srcFile.Key, new List<string>());
                foreach (string command in srcFile.Value)
                {
                    if (command.ToLower().Contains("ptp") || command.ToLower().Contains("lin"))
                    {
                        Regex regexToolNr = new Regex(@"(?<=Tool\s*(\[)\s*)\d*", RegexOptions.IgnoreCase);
                        if (regexToolNr.Match(command).ToString() != "")
                        {
                            int toolnumber = int.Parse(regexToolNr.Match(command).ToString());
                            Regex regexToolName = new Regex(@"(?<=Tool\s*\[\d+\s*\]\s*:\s*)[a-zA-Z0-9_\-]*", RegexOptions.IgnoreCase);
                            string correctedstring = regexToolName.Replace(command, correctedTools[toolnumber]);
                            result[srcFile.Key].Add(correctedstring);
                        }
                        else
                            result[srcFile.Key].Add(command);
                    }
                    else
                        result[srcFile.Key].Add(command);
                }
            }
            GlobalData.Tools = correctedTools;
            return result;
        }

        private static IDictionary<string, List<string>> CheckBaseNames(IDictionary<string, List<string>> resultSrcFiles)
        {
            IDictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            List<string> baseNamesFromStandard = ConfigurationManager.AppSettings["BaseNames" + GlobalData.ControllerType.Replace(" ", "_")].Split(',').Select(s => s.Trim()).ToList();
            IDictionary<int, string> baseNrAndName = new Dictionary<int, string>();
            int counter = 1;
            foreach (string tool in baseNamesFromStandard)
            {
                baseNrAndName.Add(counter, tool);
                counter++;
            }

            IDictionary<int, List<string>> foundBases = new Dictionary<int, List<string>>();

            foreach (var srcFile in resultSrcFiles.Where(x => x.Key.Contains(".src")))
            {
                foreach (string command in srcFile.Value.Where(x => x.ToLower().Contains("ptp") || x.ToLower().Contains("lin")))
                {
                    Regex regexBaseNr = new Regex(@"(?<=Base\s*(\[|=)\s*)\d*", RegexOptions.IgnoreCase);
                    if (regexBaseNr.Match(command).ToString() != "")
                    {
                        int baseNr = int.Parse(regexBaseNr.Match(command).ToString());
                        Regex regexBaseName = new Regex(@"(?<=Base\s*\[\d+\s*\]\s*:\s*)[a-zA-Z0-9_\-\+]*", RegexOptions.IgnoreCase);
                        if (!foundBases.Keys.Contains(baseNr))
                            foundBases.Add(baseNr, new List<string>());

                        string toolName = regexBaseName.Match(command).ToString();
                        if (!foundBases[baseNr].Contains(toolName) && toolName != "")
                            foundBases[baseNr].Add(regexBaseName.Match(command).ToString());
                    }
                }
            }

            IDictionary<int, string> correctedBases = new Dictionary<int, string>();
            foreach (var basee in foundBases)
            {
                bool errorFound = false;
                if ((basee.Key > 0 && basee.Key < 21 || basee.Key > 52 && basee.Key < 57) && basee.Value.Count > 0)
                {
                    Regex regexBaseName = new Regex(baseNamesFromStandard[basee.Key], RegexOptions.IgnoreCase);
                    string matchBaseName = regexBaseName.Match(basee.Value[0]).ToString();
                    if (matchBaseName != basee.Value[0])
                        errorFound = true;
                }
                else
                {
                    if ((basee.Value.Count > 0 && basee.Value[0] != baseNamesFromStandard[basee.Key]) && baseNamesFromStandard[basee.Key] != "")
                        errorFound = true;
                }

                if (errorFound && GlobalData.ControllerType != "KRC4 Not BMW")
                {
                    var vm = new RenameBaseViewModel(basee);
                    RenameBase sW = new RenameBase(vm);
                    var dialogResult = sW.ShowDialog();
                    correctedBases.Add(basee.Key, vm.CorrectedName);
                }
                else
                    if (basee.Value.Count > 0)
                    correctedBases.Add(basee.Key, basee.Value[0]);
                else
                    correctedBases.Add(basee.Key, "");
            }

            foreach (var srcFile in resultSrcFiles.Where(x => x.Key.Contains(".src")))
            {
                result.Add(srcFile.Key, new List<string>());
                foreach (string command in srcFile.Value)
                {
                    if ((command.ToLower().Contains("ptp") || command.ToLower().Contains("lin")) && !command.ToLower().Contains(" search"))
                    {
                        Regex regexBaseNr = new Regex(@"(?<=Base\s*(\[)\s*)\d*", RegexOptions.IgnoreCase);
                        if (regexBaseNr.Match(command).ToString() != "")
                        {
                            int toolnumber = int.Parse(regexBaseNr.Match(command).ToString());
                            Regex regexBaseName = new Regex(@"(?<=Base\s*\[\d+\s*\]\s*:\s*)[a-zA-Z0-9_\-\+]*", RegexOptions.IgnoreCase);
                            string correctedstring = regexBaseName.Replace(command, correctedBases[toolnumber]);
                            result[srcFile.Key].Add(correctedstring);
                        }
                        else
                            result[srcFile.Key].Add(command);
                    }
                    else
                        result[srcFile.Key].Add(command);
                }
            }
            GlobalData.Bases = correctedBases;
            return result;
        }

        private static IDictionary<string, List<string>> CheckContinous(IDictionary<string, List<string>> resultSrcFiles)
        {
            string[] excludedStrings = { "DeltaMFG", "IF ", "ELSE", "ENDIF", "ENDSWITCH", "REPEAT", "_CAMERA_STOP", "_CAMERA_START", "GLUE_GUN._OPEN", "CONTINUE", "GLUE_NUMBER", "GLUE_APPLICATION_CYCLE" };
            string[] movementTypes = { "ptp", "lin" };
            string[] apprroxTypes = { "c_ptp", "c_lin" };
            foreach (var file in resultSrcFiles)
            {
                string previousCommand = "";
                foreach (string command in file.Value)
                {
                    if (!(command.Substring(0, 1) == ";" && !command.ToLower().Contains("fold")))
                    {
                        if (!command.ContainsAny(movementTypes))
                        {
                            if (previousCommand.ContainsAny(movementTypes))
                            {
                                if (previousCommand.ContainsAny(apprroxTypes))
                                {
                                    if (!command.ContainsAny(excludedStrings))
                                    {
                                        string message = "Continous movement before command that requires stop detected.\r\nPlease correct the path!\r\nPath: " + Path.GetFileName(file.Key) + "\r\nCommands:\r\n\r\n" + previousCommand + "\r\n" + command;
                                        logFileContent = logFileContent + message + "\r\n";
                                        MessageBox.Show("\r\n" + message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                }
                            }
                        }
                        previousCommand = command;
                    }

                }
            }

            return resultSrcFiles;
        }

        public static bool ContainsAny(this string currentString, string[] excludedStrings)
        {
            foreach (string substring in excludedStrings)
            {
                if (currentString.ToLower().Contains(substring.ToLower()))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool ContainsAnyRet(this string currentString, string[] excludedStrings, out string foundstring)
        {
            foreach (string substring in excludedStrings)
            {
                if (currentString.ToLower().Contains(substring.ToLower()))
                {
                    foundstring = substring;
                    return true;
                }
            }
            foundstring = "";
            return false;
        }

        private static void FindMaintenancePaths(IDictionary<string, List<string>> resultSrcFiles)
        {
            List<string> opertaionTypes = ConfigurationManager.AppSettings["OperationTypes" + GlobalData.ControllerType.Replace(" ", "_")].Split(',').Select(s => s.Trim()).ToList();
            List<string> opertaionTypesNotContains = ConfigurationManager.AppSettings["NotContainOpTypes" + GlobalData.ControllerType.Replace(" ", "_")].Split(',').Select(s => s.Trim()).ToList();
            opertaionTypes.Add("Dock");
            opertaionTypes.Add("Clean");
            List<string> foundOperations = new List<string>();
            foreach (string opType in opertaionTypes)
            {
                foreach (string operation in resultSrcFiles.Keys)
                {
                    bool addToOpType = true;
                    foreach (var op in opertaionTypesNotContains)
                    {
                        if (operation.ToLower().Contains(op.ToLower()))
                        {
                            addToOpType = false;
                            break;
                        }
                    }
                    if (operation.Contains(opType) && !foundOperations.Contains(opType) && addToOpType)
                        foundOperations.Add(opType);
                }
            }
            List<string> foundFiles = new List<string>();
            foreach (var item in resultSrcFiles.Keys)
                foundFiles.Add(Path.GetFileNameWithoutExtension(item).ToLower());

            string message = "";
            foreach (string foundOp in foundOperations)
            {
                if (ConfigurationManager.AppSettings[foundOp + "_Maintenances" + (GlobalData.ControllerType.Replace(" ", "_"))] != null)
                {
                    List<string> maintenanceOps = ConfigurationManager.AppSettings[foundOp + "_Maintenances" + (GlobalData.ControllerType.Replace(" ", "_"))].Split(',').Select(s => s.Trim()).ToList();
                    foreach (string maintenanceOp in maintenanceOps)
                    {
                        if (!string.IsNullOrEmpty(maintenanceOp))
                        {
                            bool maintenanceContain = false;
                            foreach (string file in foundFiles)
                            {
                                if (file.ToLower().Contains(maintenanceOp.ToLower()))
                                {
                                    maintenanceContain = true;
                                    break;
                                }
                            }
                            if (!maintenanceContain)
                            {
                                message += "Missing maintenance operation: " + maintenanceOp + "\r\n";
                            }
                        }
                    }
                }
                else
                    MessageBox.Show("Maintenances for \"" + foundOp + "\" operation not found!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            logFileContent += message;
            //if (!string.IsNullOrEmpty(message))
            //    MessageBox.Show("Missing maintenance operations found!\r\n\r\n" + message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static string GetPayload(List<string> files)
        {
            string robotname = GlobalData.RobotType;
            if (String.IsNullOrEmpty(robotname))
            {
                try
                {
                    Regex robotnumber = new Regex(@"\d+r\d+|\d+ir\d+", RegexOptions.IgnoreCase);
                    //if (robotnumber.Match(roboter).ToString().Length > 0)
                    //    roboter = roboter.Replace(robotnumber.Match(roboter).ToString(), "");
                    roboter = roboter.Replace("ST_", "");
                    roboter = roboter.Replace("st_", "");
                    Regex getpayload = new Regex(@"(?<=kr)\d+", RegexOptions.IgnoreCase);
                    Regex getrange = new Regex(@"(?<=(l|\d+r))\d+", RegexOptions.IgnoreCase);
                    Regex getgeneration = new Regex(@"(?<=_\s*)\d+", RegexOptions.IgnoreCase);
                    bool noRangedefined = false;
                    if (getrange.Match(roboter).ToString() == "")
                    {
                        noRangedefined = true;
                    }
                    bool isWeldingRobot = false;
                    // sprawdzenie czy jest zgrzewanie na robocie
                    foreach (var item in files)
                    {
                        string filename = Path.GetFileNameWithoutExtension(item);
                        if (filename.ToLower().Contains("spot"))
                        {
                            isWeldingRobot = true;
                            break;
                        }
                    }
                    // sprawdzenie czy zgrzewarka stacjonarna czy na robocie/
                    if (isWeldingRobot && !GlobalData.Tools.Keys.Contains(1))
                        isWeldingRobot = false;
                    Roboter robot = null;

                    if (roboter.ToLower().Contains("kr200_comp"))
                    {
                        robot = new Roboter(200, 0, 2, true);
                        robotname = "kr200_2";
                    }
                    else if (noRangedefined)
                    {
                        if (getgeneration.Match(roboter).ToString() == "")
                        {
                            robot = new Roboter(int.Parse(getpayload.Match(roboter).ToString()), 0, 2, isWeldingRobot);
                        }
                        else
                        {
                            robot = new Roboter(int.Parse(getpayload.Match(roboter).ToString()), 0, int.Parse(getgeneration.Match(roboter).ToString()), isWeldingRobot);
                        }
                        robotname = "kr" + robot.Payload + "_" + robot.Generation;
                    }
                    else
                    {
                        if (getgeneration.Match(roboter).ToString() != "")
                        {
                            robot = new Roboter(int.Parse(getpayload.Match(roboter).ToString()), int.Parse(getrange.Match(roboter).ToString()), int.Parse(getgeneration.Match(roboter).ToString()), isWeldingRobot);
                            robotname = "kr" + robot.Payload + "l" + robot.Range + "_" + robot.Generation;
                        }
                        else
                        {
                            robot = new Roboter(int.Parse(getpayload.Match(roboter).ToString()), int.Parse(getrange.Match(roboter).ToString()), 0, isWeldingRobot);
                            robotname = "kr" + robot.Payload + "l" + robot.Range + "_2";
                        }
                    }
                    string robotmodel = robotname;
                    if (robot.IsWeldingRobot)
                        robotname += "_weld";
                    else
                        robotname += "_noweld";
                    if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings[robotname]))
                    {
                        GlobalData.RobotType = robotname;
                        List<string> payloadValues = ConfigurationManager.AppSettings[robotname].Split(',').Select(s => s.Trim()).ToList();
                        return "\r\n; 3RD AXIS LOAD\r\nLOAD_A3_DATA={M " + payloadValues[0] + ",CM {X " + payloadValues[1] + ",Y " + payloadValues[2] + ",Z " + payloadValues[3] + ",A 0.0,B 0.0,C 0.0},J {X 0.0,Y 0.0,Z 0.0}}";
                    }
                    else
                    {
                        Exception ex = new Exception();
                        throw ex;
                    }
                }
                catch (Exception ex)
                {
                    var vm = new SelectRobotTypeVM();
                    SelectRobotType sW = new SelectRobotType(vm);
                    var dialogResult = sW.ShowDialog();
                    if (dialogResult.Value == true)
                    {
                        if (!String.IsNullOrEmpty(vm.SelectedRobot))
                        {
                            string robotnameVM = vm.SelectedRobot;
                            Regex replaceRegex = new Regex(@"(?<=\d+)r(?=\d+)", RegexOptions.IgnoreCase);
                            robotnameVM = replaceRegex.Replace(robotnameVM, "l");
                            if (vm.IsWelding)
                                robotnameVM += "_weld";
                            else
                                robotnameVM += "_noweld";
                            GlobalData.RobotType = robotnameVM;
                            List<string> payloadValues = ConfigurationManager.AppSettings[robotnameVM].Split(',').Select(s => s.Trim()).ToList();
                            return "\r\n; 3RD AXIS LOAD\r\nLOAD_A3_DATA={M " + payloadValues[0] + ",CM {X " + payloadValues[1] + ",Y " + payloadValues[2] + ",Z " + payloadValues[3] + ",A 0.0,B 0.0,C 0.0},J {X 0.0,Y 0.0,Z 0.0}}";
                        }
                    }
                    else
                    {
                        GlobalData.RobotType = "nopayload";
                        return "";
                    }
                }
            }
            if (robotname == "nopayload")
                return "";
            else
            {
                List<string> payloadValues = ConfigurationManager.AppSettings[robotname].Split(',').Select(s => s.Trim()).ToList();
                return "\r\n; 3RD AXIS LOAD\r\nLOAD_A3_DATA={M " + payloadValues[0] + ",CM {X " + payloadValues[1] + ",Y " + payloadValues[2] + ",Z " + payloadValues[3] + ",A 0.0,B 0.0,C 0.0},J {X 0.0,Y 0.0,Z 0.0}}";
            }
        }

        private static void FillListOfOpsInGlobal(IDictionary<string, List<string>> resultSrcFiles)
        {
            GlobalData.Operations = new List<string>();
            foreach (var element in resultSrcFiles)
            {
                GlobalData.Operations.Add(Path.GetFileNameWithoutExtension(element.Key));
            }
        }


        private static bool DetectDuplicates(IDictionary<string, string> srcFiles)
        {
            List<string> files = new List<string>();
            foreach (var file in srcFiles)
                files.Add(Path.GetFileNameWithoutExtension(file.Key));

            List<string> testlist = new List<string>();
            foreach(var file in files)
            {
                if (testlist.Contains(file))
                {
                    MessageBox.Show("Multiple paths with same name found. Program will abort!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                else
                    testlist.Add(file);
            }
            return true;
        }

        private static void FindDockableTools(IDictionary<string, List<string>> resultSrcFiles)
        {
            List<int> foundtools = new List<int>();
            foreach (var file in resultSrcFiles)
            {
                foreach (string command in file.Value.Where(x => x.ToLower().Contains("check_alternative") || x.ToLower().Contains("tch_checktool")))
                {
                    Regex findtoolnumber = new Regex(@"(?<=\(\s*)\d+");
                    int toolnumber = int.Parse(findtoolnumber.Match(command).ToString());
                    if (!foundtools.Contains(toolnumber) && toolnumber > 0)
                        foundtools.Add(toolnumber);
                }
            }

            if (GlobalData.ControllerType.Contains("KRC2"))
            {
                string ibus = "";
                foundtools.Sort();
                if (foundtools.Count > 1)
                {
                    GlobalData.HasToolchager = true;
                    foreach (int tool in foundtools)
                    {
                        var vm = new IBUSSegmentNameViewModel(tool);
                        while (string.IsNullOrEmpty(vm.NameTool))
                        {
                            IBUSSegmentName sW = new IBUSSegmentName(vm);
                            var dialogResult = sW.ShowDialog();
                        }
                        if (ibus == "")
                            ibus = "SEG_SK[" + tool + "]={SK_TYPE #VALUE,SK_TXT[] \"" + vm.NameTool + "   \"}";
                        else
                            ibus += "\r\nSEG_SK[" + tool + "]={SK_TYPE #VALUE,SK_TXT[] \"" + vm.NameTool + "   \"}";
                    }
                }
                GlobalData.IBUSSegments = ibus;
            }
        }

        internal static List<FileLineProperties> FilterGlobalFDATs(List<FileLineProperties> linesToAddToFile)
        {
            List<FileLineProperties> result = new List<FileLineProperties>();
            List<string> foundE6AXIS = new List<string>();
            foreach (var line in linesToAddToFile.Where(x=>x.LineContent.ToLower().Contains("e6axis")))
            {
                foundE6AXIS.Add(line.Variable.Remove(0, 1));
            }
            foreach (var line in linesToAddToFile)
            {
                if (line.LineContent.Contains("FDAT"))
                {
                    bool addLine = true;
                    foreach (var item in foundE6AXIS)
                    {
                        if (line.LineContent.Contains(item))
                        {
                            addLine = false;
                            break;
                        }
                    }
                    if (addLine)
                        result.Add(line);
                    //else
                        //if (!GlobalData.GlobalFDATs.Contains(line.LineContent))
                           // GlobalData.GlobalFDATs.Add(line.LineContent);

                }
                else
                    result.Add(line);
            }

            return result;
        }

        private static List<string> GetGlobalFDATs(List<string> allFiles)
        {
            List<string> e6axis = new List<string>();
            List<string> fdats = new List<string>();
            List<string> result = new List<string>();
            foreach (var file in allFiles.Where(x => x.ToLower().Contains(".dat")))
            {
                StreamReader reader = new StreamReader(file);
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (line.ToLower().Contains("e6axis") && !e6axis.Contains(line))
                    {
                        Regex getnameRegex = new Regex(@"(?<=E6AXIS\s*).*(?=\=)", RegexOptions.IgnoreCase);
                        if (getnameRegex.IsMatch(line))
                        {
                            string element = string.Empty;
                            if (!string.IsNullOrEmpty(getnameRegex.Match(line).ToString()) && getnameRegex.Match(line).ToString().Trim().Substring(0,1).ToLower() =="x")
                                element = getnameRegex.Match(line).ToString().Trim().Remove(0, 1);
                            else
                                element = getnameRegex.Match(line).ToString().Trim();
                            if (!e6axis.Contains(element))
                                e6axis.Add(element);
                        }
                    }
                        
                }
                reader.Close();
            }

            foreach (var file in allFiles.Where(x=>x.ToLower().Contains("global")))
            {
                StreamReader reader = new StreamReader(file);
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (line.ToLower().Contains("fdat") && line.ToLower().Contains("global") && !line.ToLower().Contains("defdat"))
                    {
                        bool addline = false;
                        foreach (string position in e6axis)
                        {
                            if (line.ToLower().Contains(position.ToLower()))
                            {
                                addline = true;
                                break;
                            }
                        }
                        if (!fdats.Contains(line.Replace("GLOBAL ", "")) && addline)
                            //fdats.Add(line);
                            fdats.Add(line.Replace("GLOBAL ",""));
                    }
                }
                reader.Close();
            }
            GlobalData.GlobalFDATs = fdats;
            result = fdats;
            return result;
        }

        public static List<FileLineProperties> CorrectHomeRound(List<FileLineProperties> listToCheck)
        {
            List<FileLineProperties> result = new List<FileLineProperties>();
            Regex getValuesRegex = new Regex(@"(?<=(A|E)\d+\s+)((-\d+\.\d+)|(\d+\.\d+)|(-\d+)|(\d+))", RegexOptions.IgnoreCase);
            Regex getHomeName = new Regex(@".*(?={)", RegexOptions.IgnoreCase);
            foreach (var line in listToCheck)
            {
                string currentHomeString = getHomeName.Match(line.LineContent).ToString() + "{";
                MatchCollection matches = getValuesRegex.Matches(line.LineContent);
                for (int i = 1; i <= matches.Count; i++)
                {
                    string tempstring = "";
                    if (i < 7)
                        tempstring = "A" + i;
                    else
                        tempstring = "E" + (i - 6);
                    double roundedValue = Math.Round(double.Parse(matches[i - 1].Value.Replace(".",",")), 0);
                    currentHomeString += tempstring + " " + roundedValue + ",";
                }
                currentHomeString = currentHomeString.Substring(0, currentHomeString.Length-1) + "}";

                line.LineContent = currentHomeString;
                result.Add(line);
            }
            return result;
        }

        public static List<FileLineProperties> FixMissingExternalAxis(List<FileLineProperties> listToCheck)
        {
            GlobalData.Has7thAxis = false;
            Regex getValuesRegex = new Regex(@"(?<=(A|E)\d+\s+)((-\d+\.\d+)|(\d+\.\d+)|(-\d+)|(\d+))", RegexOptions.IgnoreCase);
            List<FileLineProperties> tempList = new List<FileLineProperties>();
            foreach (var line in listToCheck)
            {
                MatchCollection matches = getValuesRegex.Matches(line.LineContent);
                if (!line.LineContent.ToLower().Contains("e1 "))
                {
                    string currentLine = line.LineContent.Trim();
                    currentLine = currentLine.Replace("}", ",E1 0.0, E2 0.0, E3 0.0, E4 0.0, E5 0.0, E6 0.0}");
                    line.LineContent = currentLine;
                }
                if (!GlobalData.Has7thAxis && line.LineContent.ToLower().Contains("e1 ") && matches.Count >= 7)
                    GlobalData.Has7thAxis = true;
                else if (line.LineContent.ToLower().Contains("e1 ") && matches.Count < 12)
                {
                    int firstMissingExt = (6 - matches.Count) * (-1) + 1;
                    string tempstring = "";
                    for (int i = firstMissingExt; i <= 6; i++)
                    {
                        tempstring += "E" + i + " 0,";
                    }
                    string currentLine = line.LineContent.Trim();
                    currentLine = currentLine.Replace("}", "," + tempstring);
                    currentLine = currentLine.Substring(0, currentLine.Length - 1) + "}";
                    line.LineContent = currentLine;
                }
                tempList.Add(line);
            }
            return tempList;
        }

        public static void GetExceptionLine(Exception ex, string line = "")
        {
            MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
            Trace.TraceError(ex.StackTrace);
        }

        internal static IEnumerable<IGrouping<string, FileLineProperties>> FilterDuplicates(IEnumerable<IGrouping<string, FileLineProperties>> duplicatesGroups)
        {
            Regex punktRegex = new Regex(@"(?<=E6POS.*\=.*)(-\d+\.\d+|\d+\.\d+|-\d+|\d+)", RegexOptions.IgnoreCase);
            IEnumerable<IGrouping<string, FileLineProperties>> result = new List<IGrouping<string, FileLineProperties>>();
            foreach (var duplicates in duplicatesGroups.ToList())
            {
                DataClass.PointKUKA previouspoint = null;
                foreach (var duplicate in duplicates)
                {
                    if (punktRegex.IsMatch(duplicate.LineContent))
                    {
                        MatchCollection matches = punktRegex.Matches(duplicate.LineContent);
                        DataClass.PointKUKA point = new DataClass.PointKUKA(Math.Round(double.Parse(matches[0].ToString(), CultureInfo.InvariantCulture), 2), Math.Round(double.Parse(matches[1].ToString(), CultureInfo.InvariantCulture), 2), Math.Round(double.Parse(matches[2].ToString(), CultureInfo.InvariantCulture), 2), Math.Round(double.Parse(matches[3].ToString(), CultureInfo.InvariantCulture), 2), Math.Round(double.Parse(matches[4].ToString(), CultureInfo.InvariantCulture), 2), Math.Round(double.Parse(matches[5].ToString(), CultureInfo.InvariantCulture), 2));
                        if (previouspoint == null)
                            previouspoint = point;
                        else
                        {
                            if (NotSamePoint(point, previouspoint))
                                (result as List<IGrouping<string, FileLineProperties>>).Add(duplicates);
                        }
                    }
                    else
                    {
                        (result as List<IGrouping<string, FileLineProperties>>).Add(duplicates);
                        break;
                    }
                }
            }
            return result;
        }

        private static bool NotSamePoint(DataClass.PointKUKA point, DataClass.PointKUKA previouspoint)
        {
            double deltaPos = 2.0, deltaAngle = 0.5;
            if (Math.Abs(point.Xpos - previouspoint.Xpos) < deltaPos)
                if (Math.Abs(point.Ypos - previouspoint.Ypos) < deltaPos)
                    if (Math.Abs(point.Zpos - previouspoint.Zpos) < deltaPos)
                        if (Math.Abs(point.A - previouspoint.A) < deltaAngle)
                            if (Math.Abs(point.B - previouspoint.B) < deltaAngle)
                                if (Math.Abs(point.C - previouspoint.C) < deltaAngle)
                                    return false;
            return true;
        }
    }
}
