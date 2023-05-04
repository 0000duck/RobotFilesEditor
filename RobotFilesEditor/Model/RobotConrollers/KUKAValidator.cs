using RobotSoftwareManager.Model;
using CommonLibrary.DataClasses;
using GalaSoft.MvvmLight.Messaging;
using MS.WindowsAPICodePack.Internal;
using RobotFilesEditor.Dialogs.IBUSSegmentName;
using RobotFilesEditor.Dialogs.NameRobot;
using RobotFilesEditor.Dialogs.RenameTool;
using RobotFilesEditor.Dialogs.SelectLoadVar;
using RobotFilesEditor.Model.Operations;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static RobotFilesEditor.Model.DataInformations.FileValidationData;
using RobotFilesEditor.Dialogs.SelectJob;

namespace RobotFilesEditor.Model.RobotConrollers
{
    public class KUKAValidator : RobotControllerBase
    {

        #region fields
        private string roboter;
        #endregion fields

        #region ctor
        public KUKAValidator() : base()
        { }
        #endregion ctor


        #region overrides
        public override bool ValidateFiles(IDictionary<string, string> srcFilesIn, List<string> datFilesIn)
        {
            if (!base.ValidateFiles(srcFilesIn, datFilesIn))
                return false;
            srcFiles = srcFilesIn;
            datFiles = datFilesIn;
            IDictionary<string, List<string>> resultSrcFiles = DivideToFolds(srcFiles);
            List<string> GlobalFDATs = GetGlobalFDATs(GlobalData.AllFiles);
            resultSrcFiles = ClearEnters(resultSrcFiles);
            resultSrcFiles = ClearHeader(resultSrcFiles);
            GlobalData.Roboter = roboter;
            FillListOfOpsInGlobal(resultSrcFiles);
            SetFillDescr();
            FindDatFiles(srcFiles, datFiles);
            Helpers.UnusedDatsMethods.FindUnusedDataInDatFiles(resultSrcFiles, datFiles);
            CheckToolsAndBases(resultSrcFiles, datFiles);
            resultSrcFiles = CorrectFoldCommentIfZone0(resultSrcFiles);
            resultSrcFiles = RemoveHash(resultSrcFiles);
            FindRetrClo(resultSrcFiles);
            CheckChkAxisPos(resultSrcFiles);
            FindLocalHomes(datFiles);
            GlobalData.loadVars = FindLoadVars(resultSrcFiles);
            FillSrcPaths(resultSrcFiles);
            CheckOpeningAndClosingCommand(resultSrcFiles);
            resultSrcFiles = AddHeader(resultSrcFiles, false);
            if (GlobalData.CheckOrder)
            {
                var tempresult = Helpers.CheckOrderMethods.CheckOrder(resultSrcFiles);
                resultSrcFiles = DivideToFolds(tempresult);
                resultSrcFiles = ClearEnters(resultSrcFiles);
            }
            resultSrcFiles = CheckToolNames(resultSrcFiles);
            
            List<RobotCollision> robotCollisions = GetRobotCollisions(resultSrcFiles);
            List<RobotBase> bases = GetRobotBases(resultSrcFiles);
            List<RobotJob> jobs = GetRobotJobs(resultSrcFiles);

            resultSrcFiles = CheckContinous(resultSrcFiles);
            FindMaintenancePaths(resultSrcFiles);
            FindDockableTools(resultSrcFiles);
            FindUserBits(resultSrcFiles);

            RobotSoftwareManager.ViewModel.MainViewModel vm = new RobotSoftwareManager.ViewModel.MainViewModel(bases,jobs,robotCollisions);
            RobotSoftwareManager.MainWindow window = new RobotSoftwareManager.MainWindow(vm);
            window.ShowDialog();
            if (!vm.Result)
                return false;
            resultSrcFiles = CorrectCollisionComments(resultSrcFiles, robotCollisions);
            resultSrcFiles = CorrectJobsComments(resultSrcFiles, jobs);

            if (GlobalData.ControllerType == "KRC4" && resultSrcFiles.Any(x => Path.GetFileNameWithoutExtension(x.Key).ToLower() == "a01_braketest"))
                resultSrcFiles = AddnAnswerToBrakeTest(resultSrcFiles);

            if (GlobalData.UseOldFormatting)
                Result = KukaAddSpaces.AddSpaces(resultSrcFiles);
            else
                Result = KukaAddSpaces_v2.AddSpaces(resultSrcFiles);

            SrcValidator.Result = Result;

            return true;
        }
        #endregion overrides


        #region private methods
        private IDictionary<string, List<string>> DivideToFolds(IDictionary<string, string> filesAndContent)
        {
            Regex foldRegex = new Regex(@"^\s*;\s*FOLD\s+", RegexOptions.IgnoreCase);
            Regex endfoldRegex = new Regex(@"^\s*;\s*ENDFOLD", RegexOptions.IgnoreCase);
            IDictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            foreach (var file in filesAndContent.Where(item => item.Key.Contains(".src")))
            {
                List<string> folds = new List<string>();
                StringReader reader = new StringReader(file.Value);
                int foldlevel = 0;
                string currentFold = string.Empty;
                while (true)
                {
                    string line = reader.ReadLine();
                    if (line == null)
                        break;
                    if (!string.IsNullOrEmpty(line.Trim()))
                    {
                        if (foldRegex.IsMatch(line))
                            foldlevel++;
                        if (endfoldRegex.IsMatch(line))
                            foldlevel--;
                        currentFold += line + "\r\n";
                        if (foldlevel == 0)
                        {
                            folds.Add(currentFold);
                            currentFold = string.Empty;
                        }
                    }
                }
                result.Add(file.Key, folds);
            }
            return result;
        }

        private List<string> GetGlobalFDATs(List<string> allFiles)
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
                            if (!string.IsNullOrEmpty(getnameRegex.Match(line).ToString()) && getnameRegex.Match(line).ToString().Trim().Substring(0, 1).ToLower() == "x")
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

            foreach (var file in allFiles.Where(x => x.ToLower().Contains("global")))
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
                            fdats.Add(line.Replace("GLOBAL ", ""));
                    }
                }
                reader.Close();
            }
            GlobalData.GlobalFDATs = fdats;
            result = fdats;
            return result;
        }

        private IDictionary<string, List<string>> ClearEnters(IDictionary<string, List<string>> filteredFiles)
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


        private IDictionary<string, List<string>> ClearHeader(IDictionary<string, List<string>> filteredFiles)
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
                        SrcValidator.roboter = roboter;
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
        private void FillListOfOpsInGlobal(IDictionary<string, List<string>> resultSrcFiles)
        {
            GlobalData.Operations = new List<string>();
            foreach (var element in resultSrcFiles)
            {
                GlobalData.Operations.Add(Path.GetFileNameWithoutExtension(element.Key));
            }
        }

        private List<RobotCollision> GetRobotCollisions(IDictionary<string, List<string>> srcFiles)
        {
            Regex isCollReq = new Regex(@"(?<=^\s*Plc_CollSafetyReq1\s*\()\d+", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Regex isCollClr = new Regex(@"(?<=^\s*Plc_CollSafetyClear1\s*\()\d+", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            List<RobotCollision> robotCollisions = new List<RobotCollision>();
            string[] colldescrs = ConfigurationManager.AppSettings["CollisionDescriptions" + GlobalData.ControllerType].Split(',').Select(s => s.Trim()).ToArray();
            Regex getCollZoneCommentKRC4 = new Regex(@"(?<=Desc\s*\:\s*)[\d\w\s-_,\(\)]*", RegexOptions.IgnoreCase);
            foreach (var file in srcFiles)
            {
                string previousLine = string.Empty;
                foreach (var line in file.Value)
                {
                    GetCollisionDescriptions(isCollReq, robotCollisions, line, previousLine, true);
                    GetCollisionDescriptions(isCollClr, robotCollisions, line, previousLine, false);
                    previousLine = line;
                }
            }

            return robotCollisions;
        }

        private void GetCollisionDescriptions(Regex collRegex, List<RobotCollision> robotCollisions, string line, string previousLine, bool isReq)
        {
            string[] colldescrs = ConfigurationManager.AppSettings["CollisionDescriptions" + GlobalData.ControllerType].Split(',').Select(s => s.Trim()).ToArray();
            Regex getCollZoneCommentKRC4 = new Regex(@"(?<=Desc\s*\:\s*)[\d\w\s-_,\(\)]*", RegexOptions.IgnoreCase);
            if (collRegex.IsMatch(line))
            {
                int number = int.Parse(collRegex.Match(line).ToString());
                RobotCollision robotCollision;

                if (!robotCollisions.Any(x => x.Number == number))
                {
                    robotCollision = new RobotCollision(number);
                    robotCollisions.Add(robotCollision);
                }
                else
                    robotCollision = robotCollisions.FirstOrDefault(x => x.Number == number);
                var sectionToAdd = isReq ? robotCollision.FoundNames : robotCollision.FoundNamesRel;

                if (getCollZoneCommentKRC4.IsMatch(line) && !robotCollision.FoundNames.Contains(getCollZoneCommentKRC4.Match(line).ToString().Trim()))
                    sectionToAdd.Add(getCollZoneCommentKRC4?.Match(line)?.ToString().Trim());

                foreach (var descr in colldescrs)
                {
                    Regex isCollRegex = new Regex(descr, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    if (isCollRegex.IsMatch(previousLine))
                    {
                        Regex getCollDescr = new Regex(@"(?<=" + descr + ".*" + number.ToString() + @"\s*-*-s*).*", RegexOptions.IgnoreCase);
                        if (getCollDescr.IsMatch(previousLine))
                        {
                            var currDescr = getCollDescr?.Match(previousLine)?.ToString().Trim();
                            if (!sectionToAdd.Contains(currDescr))
                                sectionToAdd.Add(currDescr);
                        }
                    }
                }
            }
        }

        private void FindDatFiles(IDictionary<string, string> srcFiles, List<string> datFiles)
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
                    Messenger.Default.Send<LogResult>(new LogResult(message, LogResultTypes.Warning), "AddLog");
                }
            }
        }

        private void CheckToolsAndBases(IDictionary<string, List<string>> srcFiles, List<string> datFiles)
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
                        if (SrcValidator.UsedDats[file].FDAT.Contains(regex.Match(line).ToString()))
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

        private void CreateMultiToolOrBaseString(IDictionary<int, List<string>> toolOrBaseAndPoint, string file, bool isTool)
        {
            string[] toolOrBase = { "", "" };
            if (isTool)
                toolOrBase = new string[] { "tools", "tool" };
            else
                toolOrBase = new string[] { "bases", "base" };
            string currentString = "Mutiple " + toolOrBase[0] + " found in file " + (Path.GetFileName(file)).Replace(".dat", "") + ". Points with " + toolOrBase[1] + " number:";
            foreach (var toolorbase in toolOrBaseAndPoint)
            {

                string currentToolOrBase = " " + toolorbase.Key + " - points: ";
                {
                    foreach (string point in toolorbase.Value)
                    {
                        currentToolOrBase += point + ", ";
                    }
                }
                currentToolOrBase = currentToolOrBase.Substring(0, currentToolOrBase.Length - 2);
                currentString += currentToolOrBase;
            }
            Messenger.Default.Send<LogResult>(new LogResult(currentString, LogResultTypes.Warning), "AddLog");
        }

        private IDictionary<string, List<string>> CorrectFoldCommentIfZone0(IDictionary<string, List<string>> resultSrcFiles)
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

        private IDictionary<string, List<string>> RemoveHash(IDictionary<string, List<string>> resultSrcFiles)
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

        private void FindRetrClo(IDictionary<string, List<string>> resultSrcFiles)
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
                Messenger.Default.Send<LogResult>(new LogResult(message, LogResultTypes.Error), "AddLog");
            }
        }

        private void CheckChkAxisPos(IDictionary<string, List<string>> resultSrcFiles)
        {
            bool warningsFound = false;
            Regex findCentralPosRegex = new Regex(@"(?<=^\s*((WAIT\s+FOR)|(IF))\s+CHK_AXIS_POS\s*\(\s*).*(?=\))", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Regex findMovePointRegex = new Regex(@"(?<=^\s*(PTP|LIN)\s+)[\w_-]+", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            foreach (var file in resultSrcFiles)
            {
                bool firstPointFound = false, chkAxisPosFound = false;
                string chkAxisPointName = string.Empty, firstPointName = string.Empty;
                foreach (var line in file.Value)
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
                        warningsFound = true;
                        Messenger.Default.Send<LogResult>(new LogResult("Motion to central pos without CHK_AXIS_POS in path: " + Path.GetFileNameWithoutExtension(file.Key), LogResultTypes.Warning), "AddLog");
                        break;
                    }
                    if (firstPointFound && chkAxisPosFound)
                    {
                        if (chkAxisPointName.ToLower().Trim() != firstPointName.ToLower().Trim())
                        {
                            warningsFound = true;
                            Messenger.Default.Send<LogResult>(new LogResult("Central pos name is different than CHK_AXIS_POS argument in path: " + Path.GetFileNameWithoutExtension(file.Key), LogResultTypes.Warning), "AddLog");
                        }
                        break;
                    }
                }
            }
            if (warningsFound)
                MessageBox.Show("Inconsistency in CHK_AXIS_POS for central positions found.\r\nSee log file for details", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void FindLocalHomes(List<string> datFiles)
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
                Messenger.Default.Send<LogResult>(new LogResult(starter + proc + ender, LogResultTypes.Error), "AddLog");

            }
        }

        private IDictionary<int, string> FindLoadVars(IDictionary<string, List<string>> inputFiles)
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
                            MessageBox.Show("Loadvar's " + loadVarNum + " name is longer than 20 chars! It will be reduced to " + loadVarName + ".", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private IDictionary<string, List<string>> CorrectCollisionComments(IDictionary<string, List<string>> files, List<RobotCollision> robotCollisions)
        {
            files = RemoveCollisionComments(files);
            IDictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            Regex isCollDescrRegex = new Regex(@";\s*" + ConfigurationManager.AppSettings["CollisionDescription" + GlobalData.ControllerType.Replace(" ", "_") + language], RegexOptions.IgnoreCase);
            Regex collzoneCallRegex = new Regex(@"Plc_CollSafety(Req|Clear)", RegexOptions.IgnoreCase);
            foreach (var file in files)
            {
                List<string> commands = new List<string>();
                foreach (var item in file.Value)
                {
                    string collWithDescription = "";
                    string commentDescription = string.Empty;
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
                            var currentColl = robotCollisions?.FirstOrDefault(x => x.Number == number);
                            string descr = (item.ToLower().Replace(" ", "").Contains(ConfigurationManager.AppSettings["Collreq" + GlobalData.ControllerType.Replace(" ", "_")])) ? currentColl.CurrentName : currentColl.CurrentNameRel;
                            if (number > 0)
                            {
                                commentDescription = "; " + ConfigurationManager.AppSettings["CollisionDescription" + GlobalData.ControllerType.Replace(" ", "_") + language] + " " + number.ToString() + " - " + descr + "\r\n";
                                collWithDescription = item;
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
                    {
                        commands.Add(commentDescription);
                        commands.Add(collWithDescription);
                    }
                }
                result.Add(file.Key, commands);
            }
            return result;
        }
        private IDictionary<string, List<string>> RemoveCollisionComments(IDictionary<string, List<string>> filteredFiles)
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
                        if (line.Length > 6 && collregex.IsMatch(line.Substring(0, 6)))
                        {
                            clearedFile.Remove(line);
                        }
                        if (GlobalData.ControllerType == "KRC4" && (line.ToLower().Replace(" ", "").Contains("collzonerequest") || line.ToLower().Replace(" ", "").Contains("collzonerelease")))
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

        private IDictionary<string, List<string>> CorrectJobsComments(IDictionary<string, List<string>> files, List<RobotJob> jobsIn)
        {
            IDictionary<string, List<string>> jobs = new Dictionary<string, List<string>>();
            IDictionary<string, List<string>> copyOfFiles = new Dictionary<string, List<string>>();
            foreach (var item in files)
                copyOfFiles.Add(item);
            foreach (var file in files)
            {
                List<string> commands = new List<string>();
                foreach (var item in file.Value)
                {
                    if (item.ToLower().Contains("job_finishwork") || item.ToLower().Contains("job_req"))
                    {
                        Regex numberRegex = new Regex(@"(?<=JobNr\s*=\s*)\d*", RegexOptions.IgnoreCase);
                        int number = int.Parse(numberRegex.Match(item).ToString());
                        var job = jobsIn.FirstOrDefault(x => x.Number == number);
                        Regex descriptionRegex = new Regex(@"(?<=DESC\s*=\s*)[a-zA-Z0-9\s_]*", RegexOptions.IgnoreCase);
                        string description = descriptionRegex.Match(item).ToString();
                        if (description != job.CurrentName)
                        {
                            commands.Add(item.Replace("DESC=" + description, "DESC=" + job.CurrentName));
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
                            var job = jobsIn.FirstOrDefault(x => x.Number == number);
                            Regex descriptionRegex = new Regex(@"(?<=Desc\s*\:\s*)[a-zA-Z0-9\s_]*", RegexOptions.IgnoreCase);
                            string description = descriptionRegex.Match(item).ToString();
                            if (description != job.CurrentName)
                            {
                                Regex containDesc = new Regex(@"desc\s*\:", RegexOptions.IgnoreCase);
                                if (containDesc.IsMatch(item))
                                {
                                    string changedCommand = item.Replace("DESC=" + description.Trim(), "DESC=" + job.CurrentName);
                                    changedCommand = changedCommand.Replace("Desc:" + description.Trim(), "Desc:" + job.CurrentName);
                                    changedCommand = changedCommand.Replace("Plc_JobDesc=" + description.Trim(), "Plc_JobDesc=" + job.CurrentName);
                                    commands.Add(changedCommand);
                                }
                                else
                                {
                                    Regex replacementRegex = new Regex(@"JobNum\s*\:\s*\d+", RegexOptions.IgnoreCase);
                                    string commandToAdd = replacementRegex.Replace(item, "JobNum:" + number + " Desc:" + job.CurrentName);
                                    Regex paramRegex = new Regex(@"Plc_JobDesc\s*\=.*", RegexOptions.IgnoreCase);
                                    commandToAdd = paramRegex.Replace(commandToAdd, "Plc_JobDesc=" + job.CurrentName);
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
        private void FillSrcPaths(IDictionary<string, List<string>> resultSrcFiles)
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

        private void CheckOpeningAndClosingCommand(IDictionary<string, List<string>> srcFiles)
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
                            Messenger.Default.Send<LogResult>(new LogResult(message, LogResultTypes.Warning), "AddLog");
                        }

                        if (item.Value.StartLines.Count < item.Value.EndLines.Count)
                        {
                            message = "Path: " + file.Key.ToString() + ": " + item.Value.Type + " number: " + item.Value.Number.ToString() + " is released but not requested";
                            Messenger.Default.Send<LogResult>(new LogResult(message, LogResultTypes.Warning), "AddLog");
                        }
                        if (item.Value.StartLines.Count == item.Value.EndLines.Count)
                        {
                            int counter = 0;
                            foreach (int line in item.Value.StartLines)
                            {
                                if (item.Value.StartLines[counter] > item.Value.EndLines[counter])
                                {
                                    message = "Path: " + file.Key.ToString() + ": \r\n" + item.Value.Type + " number: " + item.Value.Number.ToString() + " is released before it is requested!";
                                    Messenger.Default.Send<LogResult>(new LogResult(message, LogResultTypes.Warning), "AddLog");
                                }
                                counter++;
                            }
                        }
                    }
                }
            }
        }

        private int FindJob(KeyValuePair<string, List<string>> file)
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
        private void FillJobsList(List<OpenAndCloseCommand> allOperations)
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

        private IDictionary<string, List<string>> AddHeader(IDictionary<string, List<string>> filteredFiles, bool isDat)
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
                    SrcValidator.roboter = roboter;
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
                        Messenger.Default.Send<LogResult>(new LogResult("Operation " + Path.GetFileName(file.Key) + " is of unknown type. Check if operation name is correct", LogResultTypes.Warning), "AddLog");
                    }
                }
                header = "";

                header = header + ConfigurationManager.AppSettings["Header1" + GlobalData.ControllerType.Replace(" ", "_") + language] + "\r\n";
                header = header + ConfigurationManager.AppSettings["Header2" + GlobalData.ControllerType.Replace(" ", "_") + language] + Path.GetFileNameWithoutExtension(file.Key) + "\r\n";
                header = header + ConfigurationManager.AppSettings["Header3" + GlobalData.ControllerType.Replace(" ", "_") + language] + ConfigurationManager.AppSettings[currentOpType.Replace("A04", "").Replace("A05", "") + GlobalData.ControllerType.Replace(" ", "_") + language] + "" + resultWerkzeug + "\r\n";
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
                    //header = header + Char.ConvertFromUtf32(160) + "\r\n" + Char.ConvertFromUtf32(160) + "\r\n" + ";# --------- START PATH : " + Path.GetFileNameWithoutExtension(file.Key) + " ---------\r\n\r\n";
                    header = header + "\r\n" + "\r\n" + ";# --------- START PATH : " + Path.GetFileNameWithoutExtension(file.Key) + " ---------\r\n\r\n";
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

        private IDictionary<string, List<string>> CheckToolNames(IDictionary<string, List<string>> resultSrcFiles)
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

        private List<RobotBase> GetRobotBases(IDictionary<string, List<string>> resultSrcFiles)
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
                        if (!foundBases.Keys.Contains(baseNr) && baseNr > 0)
                            foundBases.Add(baseNr, new List<string>());

                        string toolName = regexBaseName.Match(command).ToString();
                        if (baseNr > 0 && !foundBases[baseNr].Contains(toolName) && toolName != "")
                            foundBases[baseNr].Add(regexBaseName.Match(command).ToString());
                    }
                }
            }

            IDictionary<int, string> correctedBases = new Dictionary<int, string>();
            bool errorFound = false;
            foreach (var basee in foundBases)
            {
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

            
            }
            List<RobotBase> robotBasesrResult = new List<RobotBase>();
            foundBases.ToList().ForEach(x => robotBasesrResult.Add(new RobotBase(x.Key, x.Value, baseNamesFromStandard)));
            return robotBasesrResult;


            if (errorFound)
            {
                //List<RobotBase> robotBases = new List<RobotBase>();
                //foundBases.ToList().ForEach(x => robotBases.Add(new RobotBase(x.Key, x.Value, baseNamesFromStandard)));
                //RobotSoftwareManager.ViewModel.MainViewModel vm = new RobotSoftwareManager.ViewModel.MainViewModel(robotBases);
                //RobotSoftwareManager.MainWindow window = new RobotSoftwareManager.MainWindow(vm);
                //window.ShowDialog();
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
            //return result;
        }

        private IDictionary<string, List<string>> CheckContinous(IDictionary<string, List<string>> resultSrcFiles)
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
                                        string message = "Continous movement before command that requires stop detected. Path: " + Path.GetFileName(file.Key) + "Commands:" + previousCommand + "\r\n" + command;
                                        Messenger.Default.Send<LogResult>(new LogResult(message, LogResultTypes.Warning), "AddLog");
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

        private void FindMaintenancePaths(IDictionary<string, List<string>> resultSrcFiles)
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
                                Messenger.Default.Send<LogResult>(new LogResult("Missing maintenance operation: " + maintenanceOp, LogResultTypes.Warning), "AddLog");
                            }
                        }
                    }
                }
                else
                    MessageBox.Show("Maintenances for \"" + foundOp + "\" operation not found!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void FindDockableTools(IDictionary<string, List<string>> resultSrcFiles)
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
        private void FindUserBits(IDictionary<string, List<string>> resultSrcFiles)
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

        private IDictionary<string, List<string>> AddnAnswerToBrakeTest(IDictionary<string, List<string>> resultSrcFiles)
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

        private List<RobotJob> GetRobotJobs(IDictionary<string, List<string>> srcFiles)
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
                        descriptions.Add(description.Trim());
                        tempJobs.Add(number, descriptions);
                    }
                    else
                        if (!tempJobs[number].Contains(description.Trim()))
                        tempJobs[number].Add(description.Trim());

                }
            }
            var result = new List<RobotJob>();
            foreach (var job in tempJobs)
                result.Add(new RobotJob(job.Key, job.Value));
            return result;
        }
        #endregion private methods
    }
}
