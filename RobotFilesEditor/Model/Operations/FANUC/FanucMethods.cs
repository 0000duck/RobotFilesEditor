using RobotFilesEditor.Dialogs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace RobotFilesEditor.Model.Operations.FANUC
{
    public class FanucFilesValidator
    {
        #region props
        public List<string> FilesList { get; set; }
        public IDictionary<string, FanucRobotPath> FilesAndContent { get; set; }
        public string RobotName { get; set; }
        #endregion

        #region fields
        private enum CurrentType { Undef, Motion, Spot, Glue, Search, ProcCall, FrameDef }
        Regex collzoneNumberRegex = new Regex(@"(?<=^\s*\d+\s*\:(.*PR_CALL.*CollZone.*ZoneNo\s*.\s*\=\s*|\s*!\s*Coll\s*))\d+", RegexOptions.IgnoreCase);
        Regex collDescrRegex = new Regex(@"(?<=^\s*\d+\s*\:(.*PR_CALL.*CollZone.*ZoneNo\s*.\s*\=.*,\s*'|\s*!\s*Coll.*\d+\s*-))[\w\d\s,-_]*", RegexOptions.IgnoreCase);
        Regex isJobRegex = new Regex(@"(?<=^\s*\d+\s*\:.*PR_CALL.*Job.*JobNo\s*.\s*\=\s*)\d+", RegexOptions.IgnoreCase);
        Regex jobDescrRegex = new Regex(@"(?<=^\s*\d+\s*\:.*PR_CALL.*Job.*JobNo\s*.\s*\=.*,\s*')[\w\d\s-_,]*", RegexOptions.IgnoreCase);
        Regex isCommentRegex = new Regex(@"(?<=^\s*\d+\s*:\s*!).*", RegexOptions.IgnoreCase);
        Regex isSeparatorLine = new Regex(@"^\s*\d+\s*:\s*!\s*\*+\s*;", RegexOptions.IgnoreCase);
        Regex isCollComment = new Regex(@"^\s*\d+\s*:\s*!\s*Coll\s+\d+", RegexOptions.IgnoreCase);
        string logContent;
        #endregion

        #region ctor
        public FanucFilesValidator(List<string> filesList, out string logContentOut)
        {
            logContent = string.Empty;
            FilesList = filesList;
            GetRobotName();
            FilesAndContent = ReadFiles();
            FillGlobalData();
            FilesAndContent = AddHeader();
            CheckOpenAndCloseCommands();
            FilesAndContent = CheckJobsAndCollisions(false);
            CheckCommentLength();
            DivideToLines();
            FilesAndContent = AddSpaces();
            FilesAndContent = RenumberLines();
            logContentOut = logContent;
        }

        public FanucFilesValidator(List<string> filesList)
        {
            logContent = string.Empty;
            FilesList = filesList;
            FilesAndContent = ReadFiles();
            FilesAndContent = CheckJobsAndCollisions(true);
            CheckCommentLength();
            DivideToLines();
            FilesAndContent = RenumberLines();
        }
        #endregion

        #region methods
        private void CheckCommentLength()
        {
            Regex lineContentRegex = new Regex(@"(?<=^\s*\d+\s*:\s*(!|PR_CALL CMN.*'))[^']+", RegexOptions.IgnoreCase);
            Regex procCallRegex = new Regex(@"(?<=^\s*\d+\s*:\s*PR_CALL\s+CMN.*').*(?=')", RegexOptions.IgnoreCase);
            foreach (var file in FilesAndContent)
            {
                List<string> tempList = new List<string>();
                foreach (var line in file.Value.ProgramSection)
                {
                    Regex isCommentRgx = new Regex(@"(?<=^\s*\d+\s*:\s*!).*(?=;)", RegexOptions.IgnoreCase);
                    if (!isCollComment.IsMatch(line) && (isCommentRgx.IsMatch(line) && isCommentRgx.Match(line).ToString().Length > 32) || (procCallRegex.IsMatch(line) && procCallRegex.Match(line).ToString().Length > 32))
                    {
                        if (isSeparatorLine.IsMatch(line))
                            tempList.Add(" 666:  ! ***************************** ;");
                        else
                        {
                            FanucCommentValidatorViewModel vm = new FanucCommentValidatorViewModel(line, Path.GetFileNameWithoutExtension(file.Key), 32);
                            var window = new FanucCommentValidatorWindow(vm);
                            var dialog = window.ShowDialog();
                            string tempLine = lineContentRegex.Replace(line, vm.OutputLine, 1);
                            if (!string.IsNullOrEmpty(tempLine) && !new Regex(@"^.*;$", RegexOptions.IgnoreCase).IsMatch(tempLine.Trim()))
                                tempLine += ";";
                            tempList.Add(tempLine);
                        }
                    }
                    else
                        tempList.Add(line);
                }
                file.Value.ProgramSection = tempList;
            }
        }

        private void DivideToLines()
        {
            foreach (var file in this.FilesAndContent)
            {
                List<string> tempList = new List<string>();
                foreach (var line in file.Value.ProgramSection)
                {
                    if (isCommentRegex.IsMatch(line) && line.Contains("\r\n"))
                    {
                        var lines = line.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                        tempList.AddRange(lines);
                    }
                    else
                        tempList.Add(line);
                }
                FilesAndContent.First(x => x.Key == file.Key).Value.ProgramSection = tempList;
            }
        }

        private IDictionary<string, FanucRobotPath> CheckJobsAndCollisions(bool onlyColls)
        {
            bool fillDescrs = false;
            IDictionary<string, FanucRobotPath> result = new Dictionary<string, FanucRobotPath>();
            DialogResult dialogResult = MessageBox.Show("Would you like to fill the description in Collzone statement?\r\nYes - Fill Colldescr\r\nNo - leave it blank", "Fill descriptions", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
                fillDescrs = true;
            IDictionary<int, string> collAndDescription = GetCollDescriptions(fillDescrs);
            foreach (var file in FilesAndContent)
            {
                List<string> lines = RemoveCollComments(file.Value.ProgramSection);
                List<string> linesToAdd = new List<string>();
                foreach (var line in lines)
                {
                    if (collzoneNumberRegex.IsMatch(line))
                    {
                        int collZoneNumber = int.Parse(collzoneNumberRegex.Match(line).ToString());
                        linesToAdd.Add(" 666:  !"+ collAndDescription[collZoneNumber] + ";");
                        string tempLine = string.Empty;
                        if (fillDescrs)
                        {
                            tempLine = collDescrRegex.Replace(line, new Regex(@"(?<=Coll\s+\d+\s*:\s*).*(?=\r\n)",RegexOptions.IgnoreCase).Match(collAndDescription[collZoneNumber]).ToString().Trim().Replace(";",""));
                        }
                        else
                            tempLine = collDescrRegex.Replace(line, "...");
                        linesToAdd.Add(tempLine);
                    }
                    else if (isJobRegex.IsMatch(line) && !onlyColls)
                    {
                        string templine = string.Empty;
                        int jobNum = int.Parse(isJobRegex.Match(line).ToString());
                        templine = jobDescrRegex.Replace(line, GlobalData.Jobs[jobNum]);
                        linesToAdd.Add(templine);
                    }
                    else   
                        linesToAdd.Add(line);
                }
                result.Add(file.Key, new FanucRobotPath(file.Value.InitialSection, linesToAdd, file.Value.DeclarationSection));
            }
            return result;
        }

        private List<string> RemoveCollComments(List<string> programSection)
        {
            //List<string> result = new List<string>();
            //Regex collzoneDescrLine = new Regex(@"(?<=^\s*\d+\s*\:\s*!\s*Coll\s*)\d+", RegexOptions.IgnoreCase);
            //foreach (var line in programSection.Where(x => !collzoneDescrLine.IsMatch(x)))
            //    result.Add(line);
            //return result;

            List<string> result = new List<string>();
            Regex collzoneDescrLine = new Regex(@"(?<=^\s*\d+\s*\:\s*!\s*Coll\s*)\d+", RegexOptions.IgnoreCase);
            bool isCollCommentActive = false;
            foreach (var line in programSection)
            {
                if (collzoneDescrLine.IsMatch(line) || isCollCommentActive && isCommentRegex.IsMatch(line))
                    isCollCommentActive = true;
                else
                    isCollCommentActive = false;

                if (!isCollCommentActive)
                    result.Add(line);
            }
                
            return result;
        }

        private IDictionary<int, string> GetCollDescriptions(bool fillDescr)
        {
            IDictionary<int, string> result = new Dictionary<int, string>();
            IDictionary<int, List<string>> tempCollList = new SortedDictionary<int, List<string>>();
            foreach (var file in FilesAndContent)
            {
                bool collCommentActive = false;
                int collNum = 0;
                string collComment = string.Empty;
                foreach (var line in file.Value.ProgramSection)
                {
                    if (collzoneNumberRegex.IsMatch(line) && !isCommentRegex.IsMatch(line))
                    {
                        int collZoneNr = int.Parse(collzoneNumberRegex.Match(line).ToString());
                        if (!tempCollList.Keys.Contains(collZoneNr))
                            tempCollList.Add(collZoneNr, new List<string>());
                        string descr = new Regex(@"(?<=^.*PR_CALL.*CollZone.*').*(?=')", RegexOptions.IgnoreCase).Match(line).ToString();
                        if (!tempCollList[collZoneNr].Contains(descr))
                            tempCollList[collZoneNr].Add(descr);
                    }
                    if (!collCommentActive && isCollComment.IsMatch(line))
                    {
                        collNum = int.Parse(collzoneNumberRegex.Match(line).ToString());
                        collCommentActive = true;
                        collComment = new Regex(@"(?<=^\s*\d+\s*:\s*!\s*Coll\s*\d+\s*(-|:)\s*)(.[^;])*", RegexOptions.IgnoreCase).Match(line).ToString().Trim();
                    }
                    else if (collCommentActive)
                    {
                        if (isCommentRegex.IsMatch(line))
                            collComment += "\r\n" + isCommentRegex.Match(line).ToString();
                        if (!tempCollList.Keys.Contains(collNum))
                            tempCollList.Add(collNum, new List<string>());
                        //string descr = collDescrRegex.Match(line).ToString().Replace(";", "").Trim();
                        if (!tempCollList[collNum].Contains(collComment))
                            tempCollList[collNum].Add(collComment);
                        collCommentActive = false;
                        collNum = 0;
                        collComment = string.Empty;
                    }

                }
            }
            foreach (var collision in tempCollList)
            {
                SelectColisionViewModel vm = new SelectColisionViewModel(collision, fillDescr,32, true, releaseVisible:false);
                SelectCollisionFromDuplicate sW = new SelectCollisionFromDuplicate(vm);
                var dialogResult = sW.ShowDialog();
                result.Add(collision.Key, (vm.Line2Visibility == System.Windows.Visibility.Visible && !string.IsNullOrEmpty(vm.RequestTextLine2) ? vm.RequestText + ";\r\n 666:  !" + vm.RequestTextLine2 : vm.RequestText));
            }

            return result;
        }

        private IDictionary<string, FanucRobotPath> AddSpaces()
        {
            Regex isMotionPoint = new Regex(@"^\s*\d+\s*\:\s*(J|L)\s*(PR|P)\s*\[\s*\d+\s*(|\:\s*[\w\d-_]+)\s*\]\s+\d+(\%|mm/sec)\s+(FINE|CNT\d+)\s*;", RegexOptions.IgnoreCase);
            Regex isSpotPoint = new Regex(@"^\s*\d+\s*\:\s*(J|L)\s*(PR|P)\s*\[\s*\d+.*SWP_P", RegexOptions.IgnoreCase);
            Regex isGluePoint = new Regex(@"^\s*\d+\s*\:\s*(J|L)\s*(PR|P)\s*\[\s*\d+.*GL_P", RegexOptions.IgnoreCase);
            Regex isSearchPoint = new Regex(@"^\s*\d+\s*\:\s*(J|L)\s*(PR|P)\s*\[\s*\d+.*SEARCH_S_P", RegexOptions.IgnoreCase);
            Regex isProcedureCall = new Regex(@"^\s*\d+\s*\:\s*PR_CALL", RegexOptions.IgnoreCase);
            Regex isFrameDef = new Regex(@"^\s*\d+\s*\:\s*(UFRAME|UTOOL|PAYLOAD)", RegexOptions.IgnoreCase);

            IDictionary<string, FanucRobotPath> result = new Dictionary<string, FanucRobotPath>();
            foreach (var file in FilesAndContent)
            {
                List<string> lines = RemoveSpaces(file.Value.ProgramSection);
                CurrentType previousType = CurrentType.Undef;
                CurrentType currentType = CurrentType.Undef;
                List<string> currentFileContent = new List<string>();
                foreach (var line in lines)
                {
                    if (isMotionPoint.IsMatch(line))
                        currentType = CurrentType.Motion;
                    else if (isSpotPoint.IsMatch(line))
                        currentType = CurrentType.Spot;
                    else if (isGluePoint.IsMatch(line))
                        currentType = CurrentType.Glue;
                    else if (isSearchPoint.IsMatch(line))
                        currentType = CurrentType.Search;
                    else if (isProcedureCall.IsMatch(line))
                        currentType = CurrentType.ProcCall;
                    else if (isFrameDef.IsMatch(line))
                        currentType = CurrentType.FrameDef;

                    if (currentType != previousType)
                        currentFileContent.Add("666:  ;");
                    currentFileContent.Add(line);
                    
                    previousType = currentType;
                }
                result.Add(file.Key, new FanucRobotPath(file.Value.InitialSection, currentFileContent, file.Value.DeclarationSection));
            }
            return result;
        }

        private List<string> RemoveSpaces(List<string> programSection)
        {
            Regex isBlankLineRegex = new Regex(@"^\s*\d+\s*\:\s*(|!)\s*;", RegexOptions.IgnoreCase);
            List<string> result = new List<string>();
            foreach (var line in programSection.Where(x => !isBlankLineRegex.IsMatch(x.Trim())))
                result.Add(line);
            return result;
        }

        private IDictionary<string, FanucRobotPath> RenumberLines()
        {
            IDictionary<string, FanucRobotPath> result = new Dictionary<string, FanucRobotPath>();
            foreach (var file in FilesAndContent.Where(x=> Path.GetExtension(x.Key).ToLower() == ".ls"))
            {
                List<string> renumberedProgramSection = CommonLibrary.CommonMethods.GetRenumberedBody(file.Value.ProgramSection);
                List<string> newInitialSection = GetLineCount(file.Value.InitialSection, renumberedProgramSection.Count);
                result.Add(file.Key, new FanucRobotPath(file.Value.InitialSection, renumberedProgramSection, file.Value.DeclarationSection));
            }
            return result;
        }

        private List<string> GetLineCount(List<string> initialSection, int count)
        {
            Regex getLineCountRegex = new Regex(@"\s*LINE_COUNT\s*\=\s*(?=\d+)", RegexOptions.IgnoreCase);
            int counter = 0;
            string lineToAdd = string.Empty;
            foreach (var line in initialSection)
            {
                if (getLineCountRegex.IsMatch(line))
                {
                    lineToAdd = getLineCountRegex.Match(line).ToString() + count + ";";
                    break;
                }
                counter++;
            }
            initialSection[counter] = lineToAdd;
            return initialSection;
        }

        private IDictionary<string, FanucRobotPath> ReadFiles()
        {
            IDictionary<string, FanucRobotPath> result = new Dictionary<string, FanucRobotPath>();
            foreach (var file in FilesList)
            {
                var fanucFileContent = ReadFanucFile(file);
                result.Add(file, fanucFileContent);
            }
            return result;
        }

        private FanucRobotPath ReadFanucFile(string file)
        {
            Regex isprogramSectionStart = new Regex(@"^\d+\s*\:(J|L|\s)", RegexOptions.IgnoreCase);
            Regex isDeclarationSectionStart = new Regex(@"^\/\s*POS\s*$", RegexOptions.IgnoreCase);
            int sectionNum = 1;
            List<string> initialSection = new List<string>();
            List<string> programSection = new List<string>();
            List<string> declarationSection = new List<string>();
            StreamReader reader = new StreamReader(file);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (sectionNum == 1 && isprogramSectionStart.IsMatch(line.Trim()) || sectionNum == 2 && isDeclarationSectionStart.IsMatch(line.Trim()))
                    sectionNum++;
                switch (sectionNum)
                {
                    case 1:
                        initialSection.Add(line);
                        break;
                    case 2:
                        programSection.Add(line);
                        break;
                    case 3:
                        declarationSection.Add(line);
                        break;
                }
            }
            reader.Close();
            return new FanucRobotPath(initialSection, programSection, declarationSection);
        }

        private IDictionary<string, FanucRobotPath> AddHeader()
        {
            Regex isOldHeaderPart = new Regex(@"^\s*\d+\s*\:\s*!\s*\*", RegexOptions.IgnoreCase);
            Regex commentRegex = new Regex(@"^\s*\d+\s*\:\s*!", RegexOptions.IgnoreCase);
            Regex isEmptyLineRegex = new Regex(@"^\s*\d+\s*\:\s*;\s*$", RegexOptions.IgnoreCase);
            IDictionary<string, FanucRobotPath> result = new Dictionary<string, FanucRobotPath>();
            foreach (var file in FilesAndContent)
            {
                int headreLinesCounter = 0;
                List<string> header = CreateHeader(Path.GetFileNameWithoutExtension(file.Key));
                List<string> currentFile = new List<string>();
                foreach (var line in file.Value.ProgramSection)
                {
                    if (!commentRegex.IsMatch(line) && ! isEmptyLineRegex.IsMatch(line))
                        headreLinesCounter = 2;
                    if (headreLinesCounter >= 2)
                        currentFile.Add(line);
                    if (isOldHeaderPart.IsMatch(line))
                        headreLinesCounter++;
                }
                List<string> newProgramSection = header;
                newProgramSection.AddRange(currentFile);
                result.Add(file.Key, new FanucRobotPath(file.Value.InitialSection, newProgramSection, file.Value.DeclarationSection));
            }
            return result;
        }

        internal static string GetFileContenetFANUC(FanucRobotPath files)
        {
            string result = string.Empty;
            foreach (var line in files.InitialSection)
                result += line + "\r\n";
            foreach (var line in files.ProgramSection)
                result += line + "\r\n";
            foreach (var line in files.DeclarationSection)
                result += line + "\r\n";
            return result;
        }

        private List<string> CreateHeader(string progname)
        {
            List<string> header = new List<string>();
            header.Add("   1:  ! ***************************** ;");
            header.Add("   2:  ! Prog: "+progname+";");
            header.Add("   3:  ! created on: "+DateTime.Now.ToString("yyyy-MM-dd")+";");
            header.Add("   4:  ! IR: "+RobotName+";");
            header.Add("   5:  ! Creator: "+ConfigurationManager.AppSettings["Ersteller"] + ";");
            header.Add("   6:  ! Last Update: "+ DateTime.Now.ToString("yyyy-MM-dd") + ";");
            header.Add("   7:  ! ***************************** ;");

            return header;
        }

        private void GetRobotName()
        {
            NameRoboterViewModel vm = new NameRoboterViewModel();
            var dialog = new NameRobot(vm);
            dialog.ShowDialog();
            RobotName = vm.RobotName == null ? string.Empty : vm.RobotName;
            GlobalData.RobotNameFanuc = RobotName;
        }

        private void CheckOpenAndCloseCommands()
        {
            string log = string.Empty;
            Regex collZoneRegex = new Regex(@"(?<=^\s*\d+\s*\:.*PR_CALL.*CollZone.*ZoneNo\s*.\s*\=\s*)\d+", RegexOptions.IgnoreCase);
            Regex areaRegex = new Regex(@"(?<=^\s*\d+\s*\:.*PR_CALL.*Area.*AreaNo\s*.\s*\=\s*)\d+", RegexOptions.IgnoreCase);
            Regex jobRegex = new Regex(@"(?<=^\s*\d+\s*\:.*PR_CALL.*Job.*JobNo\s*.\s*\=\s*)\d+", RegexOptions.IgnoreCase);
            foreach (var file in FilesAndContent)
            {
                IDictionary<int, int> collCounter = new SortedDictionary<int, int>();
                IDictionary<int, int> areaCounter = new SortedDictionary<int, int>();
                IDictionary<int, int> jobCounter = new SortedDictionary<int, int>();
                foreach (var line in file.Value.ProgramSection)
                {
                    if (collZoneRegex.IsMatch(line))
                    {
                        int collNumber = int.Parse(collZoneRegex.Match(line).ToString());
                        if (!collCounter.Keys.Contains(collNumber))
                            collCounter.Add(collNumber, 0);
                        if (line.ToLower().Contains("request"))
                            collCounter[collNumber]++;
                        if (line.ToLower().Contains("release"))
                            collCounter[collNumber]--;
                    }
                    if (areaRegex.IsMatch(line))
                    {
                        int areaNumber = int.Parse(areaRegex.Match(line).ToString());
                        if (!areaCounter.Keys.Contains(areaNumber))
                            areaCounter.Add(areaNumber, 0);
                        if (line.ToLower().Contains("request"))
                            areaCounter[areaNumber]++;
                        if (line.ToLower().Contains("release"))
                            areaCounter[areaNumber]--;
                    }
                    if (jobRegex.IsMatch(line))
                    {
                        int jobNumber = int.Parse(jobRegex.Match(line).ToString());
                        if (!jobCounter.Keys.Contains(jobNumber))
                            jobCounter.Add(jobNumber, 0);
                        if (line.ToLower().Contains("started"))
                            jobCounter[jobNumber]++;
                        if (line.ToLower().Contains("done"))
                            jobCounter[jobNumber]--;
                    }
                }
                foreach (var coll in collCounter.Where(x => x.Value > 0))
                    log += "Collision " + coll.Key + " in file " + Path.GetFileNameWithoutExtension(file.Key) + " is requested but not released.\r\n";
                foreach (var coll in collCounter.Where(x => x.Value < 0))
                    log += "Collision " + coll.Key + " in file " + Path.GetFileNameWithoutExtension(file.Key) + " is released but not requested.\r\n";
                foreach (var area in areaCounter.Where(x => x.Value > 0))
                    log += "Area " + area.Key + " in file " + Path.GetFileNameWithoutExtension(file.Key) + " is requested but not released.\r\n";
                foreach (var area in areaCounter.Where(x => x.Value < 0))
                    log += "Area " + area.Key + " in file " + Path.GetFileNameWithoutExtension(file.Key) + " is released but not requested.\r\n";
                foreach (var job in jobCounter.Where(x => x.Value > 0))
                    log += "Job " + job.Key + " in file " + Path.GetFileNameWithoutExtension(file.Key) + " is started but not done.\r\n";
                foreach (var job in jobCounter.Where(x => x.Value < 0))
                    log += "Job " + job.Key + " in file " + Path.GetFileNameWithoutExtension(file.Key) + " is done but not started.\r\n";
            }
            if (!string.IsNullOrEmpty(log))
            {
                MessageBox.Show("Area and collision count for some files are not equal. See log for details.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                logContent += log;
            }
        }

        private void FillGlobalData()
        {
            IDictionary<string, int> pathsAndJobs = new Dictionary<string, int>();
            IDictionary<int, List<string>> unfilteresJobs = new SortedDictionary<int, List<string>>();
            IDictionary<int, string> jobs = new Dictionary<int, string>();

            foreach (var file in FilesAndContent)
            {
                string fileName = Path.GetFileNameWithoutExtension(file.Key);
                int jobNum = 0;
                foreach (var line in file.Value.ProgramSection)
                {
                    if (isJobRegex.IsMatch(line))
                    {
                        jobNum = int.Parse(isJobRegex.Match(line).ToString());
                        string description = jobDescrRegex.Match(line).ToString();
                        if (!pathsAndJobs.Keys.Contains(fileName))
                            pathsAndJobs.Add(fileName, jobNum);
                        if (!unfilteresJobs.Keys.Contains(jobNum))
                            unfilteresJobs.Add(jobNum, new List<string>());
                        if (!unfilteresJobs[jobNum].Contains(description))
                            unfilteresJobs[jobNum].Add(description);
                    }
                }
            }
            foreach (var job in unfilteresJobs)
            {
                if (job.Value.Count != 1)
                {
                    SelectJobViewModel vm = new SelectJobViewModel(unfilteresJobs.First(x=>x.Key == job.Key), "");
                    SelectJob sW = new SelectJob(vm);
                    var dialogResult = sW.ShowDialog();
                    jobs.Add(job.Key, vm.ResultText);
                }
                else
                    jobs.Add(job.Key, job.Value[0]);
            }
           
            GlobalData.SrcPathsAndJobs = pathsAndJobs;
            GlobalData.Jobs = jobs;
        }
        #endregion
    }

    public class FanucCreateSOVBackup
    {
        public FanucCreateSOVBackup(bool isSov)
        {
            if (isSov)
            { 
            DialogResult dialogResult = MessageBox.Show("Would you like to create SOV backup?\r\n", "Create SOV backup?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    try
                    {
                        Regex isProg = new Regex(@"PROG\d+\.ls", RegexOptions.IgnoreCase);
                        List<string> programPaths = Directory.GetFiles(Path.Combine(GlobalData.DestinationPath, "Program"), "*.ls").ToList();
                        List<string> orgs = Directory.GetFiles(GlobalData.DestinationPath, "*.ls", SearchOption.AllDirectories).Where(x => isProg.IsMatch(x)).ToList();
                        if (orgs.Count == 0)
                            orgs = Directory.GetFiles(GlobalData.SourcePath, "*.ls", SearchOption.AllDirectories).Where(x => isProg.IsMatch(x)).ToList();
                        if (orgs.Count == 0)
                        {
                            DialogResult dialogResult2 = MessageBox.Show("No orgs found. Would you like to:\r\nYes - Create orgs\r\nNo - select folder with orgs\r\nCancel - abort", "Create orgs?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                            if (dialogResult2 == DialogResult.Yes)
                            {
                                bool isSuccessOrgs = CreateOrgsFanuc();
                                if (!isSuccessOrgs)
                                {
                                    MessageBox.Show("No orgs created. SOV backup was not created.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }
                                orgs = Directory.GetFiles(GlobalData.DestinationPath, "*.ls", SearchOption.AllDirectories).Where(x => isProg.IsMatch(x)).ToList();
                            }
                            else if (dialogResult2 == DialogResult.No)
                            {
                                string orgsFolder = CommonLibrary.CommonMethods.SelectDirOrFile(true);
                                if (Directory.Exists(orgsFolder))
                                    orgs = Directory.GetFiles(orgsFolder, "*.ls", SearchOption.AllDirectories).Where(x => isProg.IsMatch(x)).ToList();
                                if (orgs.Count == 0)
                                {
                                    MessageBox.Show("No orgs found. SOV backup was not created.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }
                            }
                            else
                            {
                                MessageBox.Show("No orgs created. SOV backup was not created.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                        string xvrFile = string.Empty;
                        if (Directory.GetFiles(GlobalData.SourcePath, "*.xvr", SearchOption.AllDirectories).Any())
                        {
                            xvrFile = Directory.GetFiles(GlobalData.SourcePath, "*.xvr", SearchOption.AllDirectories).First();
                        }
                        else
                        {
                            MessageBox.Show("Select XVR workbook file", "Select XVR", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            xvrFile = CommonLibrary.CommonMethods.SelectDirOrFile(false, "XVR file", "*.xvr");
                            if (string.IsNullOrEmpty(xvrFile))
                            {
                                MessageBox.Show("XVR file not selected. SOV backup won't be created", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                        IDictionary<int, List<double>> homes = GetHomes();
                        if (homes.Count == 0)
                        {
                            MessageBox.Show("No home positions found in olp files. SOV Backup will not be created", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        StreamReader reader = new StreamReader(xvrFile);
                        var xvrFileContent = reader.ReadToEnd();
                        reader.Close();

                        List<AppPair> foundPairs = FindPairs();
                        ApplicationSelectorViewModel vm = new ApplicationSelectorViewModel(foundPairs);
                        ApplicationSelector dialog = new ApplicationSelector(vm);
                        dialog.ShowDialog();

                        xvrFileContent = UpdateXvr(xvrFileContent, homes, vm.Data.ToList(), true);

                        string backdate = Properties.Resources.BACKDATE.Replace("{DATE_AND_TIME}", DateTime.Now.ToString("yy/MM/dd hh:mm:ss").Replace(".", "/"));
                        string pathOfSOVBackups = Path.Combine(GlobalData.DestinationPath, "SOV_Backup");
                        if (!Directory.Exists(pathOfSOVBackups))
                            Directory.CreateDirectory(pathOfSOVBackups);
                        var asciiFile = File.Create(Path.Combine(pathOfSOVBackups, "ASCII.zip"));
                        asciiFile.Close();

                        using (FileStream zipToOpen = new FileStream(Path.Combine(pathOfSOVBackups, "ASCII.zip"), FileMode.Open))
                        {
                            using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                            {
                                programPaths.ForEach(x => archive.CreateEntryFromFile(x, Path.GetFileName(x)));
                                orgs.ForEach(x => archive.CreateEntryFromFile(x, Path.GetFileName(x)));
                            }
                            zipToOpen.Close();
                        }
                        File.WriteAllText(Path.Combine(GlobalData.DestinationPath, "SOV_Backup", "BACKDATE.DT"), backdate);
                        File.WriteAllText(Path.Combine(GlobalData.DestinationPath, "SOV_Backup", "WORKBOOK.XVR"), xvrFileContent);
                        var sovBackupFile = File.Create(Path.Combine(pathOfSOVBackups, GlobalData.RobotNameFanuc + ".zip"));
                        sovBackupFile.Close();
                        using (FileStream zipToOpen = new FileStream(Path.Combine(pathOfSOVBackups, GlobalData.RobotNameFanuc + ".zip"), FileMode.Open))
                        {
                            using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                            {
                                archive.CreateEntryFromFile(Path.Combine(pathOfSOVBackups, "ASCII.zip"), "ASCII.zip");
                                archive.CreateEntryFromFile(Path.Combine(pathOfSOVBackups, "BACKDATE.DT"), "BACKDATE.DT");
                                archive.CreateEntryFromFile(Path.Combine(pathOfSOVBackups, "WORKBOOK.XVR"), "WORKBOOK.XVR");
                            }
                            zipToOpen.Close();
                        }
                        File.Delete(Path.Combine(pathOfSOVBackups, "ASCII.zip"));
                        File.Delete(Path.Combine(pathOfSOVBackups, "BACKDATE.DT"));
                        File.Delete(Path.Combine(pathOfSOVBackups, "WORKBOOK.XVR"));

                        MessageBox.Show("SOV Backup successfuly created.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private List<AppPair> FindPairs()
        {
            List<AppPair> result = new List<AppPair>();
            foreach (var path in GlobalData.AllFiles.Where(x=> Path.GetExtension(x).ToLower() == ".ls"))
            {
                string pathfiltered = Path.GetFileNameWithoutExtension(path).ToLower();
                if (pathfiltered.Contains("b02") || pathfiltered.Contains("dock") && !pathfiltered.Contains("a04"))
                    result.Add(new AppPair("B", 2, true));
                if (pathfiltered.Contains("a03") || pathfiltered.Contains("pick") && !pathfiltered.Contains("drop"))
                    result.Add(new AppPair("A", 3, true));
                if (pathfiltered.Contains("a04") || pathfiltered.Contains("spot"))
                    result.Add(new AppPair("A", 4, true));
                if (pathfiltered.Contains("a05"))
                    result.Add(new AppPair("A", 5, true));
                if (pathfiltered.Contains("a08") || pathfiltered.Contains("glue"))
                    result.Add(new AppPair("A", 8, true));
                if (pathfiltered.Contains("a10") || pathfiltered.Contains("search"))
                    result.Add(new AppPair("A", 10, true));
                if (pathfiltered.Contains("a12") || pathfiltered.Contains("stack"))
                    result.Add(new AppPair("A", 12, true));
                if (pathfiltered.Contains("a13") || pathfiltered.Contains("rivet") && !pathfiltered.Contains("blindrivet"))
                    result.Add(new AppPair("A", 13, true));
                if (pathfiltered.Contains("a15") || pathfiltered.Contains("laser"))
                    result.Add(new AppPair("A", 15, true));
                if (pathfiltered.Contains("a16") || pathfiltered.Contains("laser"))
                    result.Add(new AppPair("A", 16, true));
                if (pathfiltered.Contains("a19") || pathfiltered.Contains("clinch"))
                    result.Add(new AppPair("A", 19, true));
                if (pathfiltered.Contains("a20") || pathfiltered.Contains("stud"))
                    result.Add(new AppPair("A", 20, true));
                if (pathfiltered.Contains("a21") || pathfiltered.Contains("stud"))
                    result.Add(new AppPair("A", 21, true));
                if (pathfiltered.Contains("a22") || pathfiltered.Contains("flow"))
                    result.Add(new AppPair("A", 22, true));
                if (pathfiltered.Contains("a23") || pathfiltered.Contains("hem"))
                    result.Add(new AppPair("A", 23, true));
                if (pathfiltered.Contains("a24") || pathfiltered.Contains("inline"))
                    result.Add(new AppPair("A", 24, true));
                if (pathfiltered.Contains("a27") || pathfiltered.Contains("blindrivet"))
                    result.Add(new AppPair("A", 27, true));
                if (pathfiltered.Contains("a28") || pathfiltered.Contains("fasten"))
                    result.Add(new AppPair("A", 28, true));
                if (pathfiltered.Contains("a47"))
                    result.Add(new AppPair("A", 47, true));
            }
            result = FilterResult(result);
            return result;
        }

        private List<AppPair> FilterResult(List<AppPair> input)
        {
            List<AppPair> result = new List<AppPair>();
            foreach (var item in input)
            {
                //if (result.FirstOrDefault(x=> x.Prefix == item.Prefix) == null && !result.Where(x => x.Prefix == item.Prefix).Any(y => y.Suffix == item.Suffix))
                if (!result.Where(x => x.Prefix == item.Prefix).Any(y => y.Suffix == item.Suffix))
                    result.Add(item);
            }
            return result;
        }

        public string UpdateXvr(string xvrFileContent, IDictionary<int, List<double>> homes, List<AppPair> apps, bool isSOV)
        {
            IDictionary<string[], string> setupDatas = new Dictionary<string[], string>();
            Regex isPosArrayRegex = new Regex("^\\s*<\\s*ARRAY\\s+name\\s*\\=\\s*\"\\s*\\$POSREG\\s*\\[\\s*1\\s*\\]", RegexOptions.IgnoreCase);
            Regex isEndRegex = new Regex(@"^\s*<\s*/\s*XMLVAR\s*>", RegexOptions.IgnoreCase);
            Regex isProgRegex = new Regex(@"^\s*<\s*PROG\s+", RegexOptions.IgnoreCase);
            Regex isProgEndRegex = new Regex(@"^\s*<\s*/\s*PROG\s*>", RegexOptions.IgnoreCase);
            Regex isSetupDataRegx = new Regex(@"^\s*<\s*VAR\s+name.*SETUP_DATA", RegexOptions.IgnoreCase);
            Regex setupDataContentRegex = new Regex(@"(?<=^\s*<\s*ARRAY.*SETUP_DATA.*)\d+",RegexOptions.IgnoreCase);
            Regex setupDataFieldRegex = new Regex(@"(?<=^\s*<\s*FIELD.*COMMENT.*\>).*(?=\<)", RegexOptions.IgnoreCase);
            Regex isRefPosRegex = new Regex(@"^\s*<\s*VAR.*REFPOS1", RegexOptions.IgnoreCase);
            //Regex isSystemRegex = new Regex(@"^\s*<\s*PROG\s+.*\*SYSTEM\*", RegexOptions.IgnoreCase);
            string result = string.Empty;
            int progLevel = 0;
            bool isSetupData = false, setupDataFound = false, isMnuframe = false, isRefPos = false;
            MatchCollection currentSetupData = null;
            string refPosString = isSOV ? FindRefPosString(xvrFileContent, apps.Where(x=>x.IsSelected==true).ToList().Count, homes.First().Value[6] == -999999 ? 6 : 7) : string.Empty;
            
            StringReader reader = new StringReader(xvrFileContent);
            while (true)
            {
                string line = reader.ReadLine();
                if (line == null)
                    break;
                if (isProgRegex.IsMatch(line))
                    progLevel++;
                if (isProgEndRegex.IsMatch(line))
                    progLevel--;
                if (progLevel > 1)
                {
                    result += "</PROG>\r\n";
                    progLevel = 1;
                }
                if (line.ToLower().Contains("$mnuframe") && isSOV)
                    isMnuframe = true;
                if (isRefPosRegex.IsMatch(line) && isSOV)
                    isRefPos = true;
                if (isSetupDataRegx.IsMatch(line) && isSOV)
                    isSetupData = true;
                if (isEndRegex.IsMatch(line) && isSOV)
                {
                    string stringToAdd = " <PROG name=\"A01_CMN_VAR\">\r\n";
                    stringToAdd += "  <VAR name=\"AR_APPS\">\r\n";
                    foreach (var app in apps.Where(x=>x.IsSelected == true))
                    {
                        stringToAdd += "    <ARRAY name=\"AR_APPS[" + app.Suffix + "]\">\r\n";
                        stringToAdd += "      <FIELD name=\""+app.Prefix+"\">\r\n";
                        stringToAdd += "        <FIELD name=\"ST_PREFIX\" prot=\"RW\">___</FIELD>\r\n";
                        stringToAdd += "        <FIELD name=\"B_INSTALLED\" prot=\"RW\">TRUE</FIELD>\r\n";
                        stringToAdd += "        <FIELD name=\"ST_VERSION\" prot=\"RW\" />\r\n";
                        stringToAdd += "        <FIELD name=\"$DUMMY3\" prot=\"RW\">255</FIELD>\r\n";
                        stringToAdd += "        <FIELD name=\"$DUMMY4\" prot=\"RW\">255</FIELD>\r\n";
                        stringToAdd += "      </FIELD>\r\n";
                        stringToAdd += "    </ARRAY>\r\n";
                    }
                    stringToAdd += "  </VAR>\r\n</PROG>\r\n";
                    result += stringToAdd;
                }
                if (!isSetupData && !isRefPos)
                    result += line + "\r\n";
                if (isRefPos && line.ToLower().Contains("</var>") && isSOV)
                    isRefPos = false;
                if (isMnuframe && line.ToLower().Contains("</var>") && isSOV)
                {
                    isMnuframe = false;
                    result += refPosString;
                }
                if (isSetupData && setupDataContentRegex.IsMatch(line) && setupDataContentRegex.Matches(line).Count== 3 && isSOV)
                {
                    currentSetupData = setupDataContentRegex.Matches(line);
                    setupDataFound = true;
                }
                if (setupDataFound && setupDataFieldRegex.IsMatch(line) && isSOV)
                {
                    setupDatas.Add(new string[3] { currentSetupData[0].ToString(), currentSetupData[1].ToString(), currentSetupData[2].ToString() }, setupDataFieldRegex.Match(line).ToString());
                    currentSetupData = null;
                    setupDataFound = false;
                }
                if (isSetupData && line.ToLower().Contains("</var>") && isSOV)
                {
                    isSetupData = false;
                    result += "    <VAR name=\"$REFPOS1\">\r\n    </VAR>\r\n";
                    foreach (var item in setupDatas)
                    {
                        result += "    <VAR name=\"SETUP_DATA["+item.Key[0]+","+ item.Key[1] + ","+ item.Key[2] +"].$COMMENT\" prot=\"RO\">"+item.Value+ "</VAR>\r\n";
                    }
                }
                if (isPosArrayRegex.IsMatch(line))
                {
                    foreach (var home in homes)
                    {
                        string stringToAdd = "      <ARRAY name = \"$POSREG[1,"+home.Key+"]\" prot = \"RW\">  'HOME"+home.Key+"'\r\n";
                        stringToAdd += "gnum: 1 rep: 9 axes: "+(home.Value[6] == -999999 ? "6" : "7") +" utool: 253 uframe: 253 Config: \r\n";
                        stringToAdd += "J1 = "+home.Value[0]+" deg   J2 = "+ home.Value[1] + " deg   J3 = "+ home.Value[2] + " deg\r\n";
                        stringToAdd += "J4 = " + home.Value[3] + " deg   J5 = " + home.Value[4] + " deg   J6 = " + home.Value[5] + " deg\r\n";
                        if (home.Value[6] != -999999)
                            stringToAdd += "EXT1: "+home.Value[6]+" mm  \r\n";
                        stringToAdd += "      </ARRAY>\r\n";
                        result += stringToAdd;
                    }
                }
            }
            return result;
        }

        private string FindRefPosString(string xvrFileContent, int appcount, int axisCount)
        {
            Regex refposStartRegex = new Regex(@"^\s*<\s*ARRAY.*REFPOS1\s*\[\s*\d+", RegexOptions.IgnoreCase);
            Regex refposEndRegex = new Regex(@"^\s*<\s*/ARRAY\s*>", RegexOptions.IgnoreCase);
            string result = string.Empty;
            bool refposstarted = false;
            StringReader reader = new StringReader(xvrFileContent);
            while (true)
            {
                string line = reader.ReadLine();
                if (line == null)
                    break;
                if (refposStartRegex.IsMatch(line))
                    refposstarted = true;
                if (refposstarted)
                    result += line + "\r\n";
                if (refposstarted && refposEndRegex.IsMatch(line))
                    refposstarted = false;
            }
            reader.Close();
            result += "    <VAR name=\"$ROBOT_NAME\" prot=\"RW\">"+ GlobalData.RobotNameFanuc + "</VAR>\r\n";
            result += "    <ARRAY name=\"$APPLICATION["+ appcount + "]\" prot=\"RO\">VIRTUAL</ARRAY>\r\n";
            result += "    <VAR name=\"$SCR.$NUM_TOT_AXS\" prot=\"RO\">"+ axisCount + "</VAR>\r\n";
            return result;
        }

        public IDictionary<int, List<double>> GetHomes()
        {
            Regex homenumRegex = new Regex(@"(?<=^\s*\[\s*\d+\s*,\s*)\d+", RegexOptions.IgnoreCase);
            Regex joint123Regex = new Regex(@"(?<=J(1|2|3)\s*=\s*)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)",RegexOptions.IgnoreCase);
            Regex joint456Regex = new Regex(@"(?<=J(4|5|6)\s*=\s*)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);
            Regex ext1Regex = new Regex(@"(?<=EXT1\s*\:\s*)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);
            IDictionary<int, List<double>> result = new SortedDictionary<int, List<double>>();
            List<string> olpFiles = Directory.GetFiles(GlobalData.SourcePath, "*.olp", SearchOption.AllDirectories).ToList();
            foreach (var file in olpFiles)
            {
                int currentHomenum = 0;
                bool firtsHomeFound = false, j123Found = false, j456Found = false, ext1Found = false, emptyLineFound = false;
                StreamReader reader = new StreamReader(file);
                double[] joints = new double[7];
                while (!reader.EndOfStream)
                {                    
                    string line = reader.ReadLine();
                    if (homenumRegex.IsMatch(line))
                    {
                        currentHomenum = int.Parse(homenumRegex.Match(line).ToString());
                        if (currentHomenum > 5)
                            currentHomenum = 0;
                        else
                            firtsHomeFound = true;
                        
                    }
                    if (joint123Regex.IsMatch(line) && currentHomenum > 0)
                    {
                        j123Found = true;
                        var matches = joint123Regex.Matches(line);
                        joints[0] = double.Parse(matches[0].ToString(), CultureInfo.InvariantCulture);
                        joints[1] = double.Parse(matches[1].ToString(), CultureInfo.InvariantCulture);
                        joints[2] = double.Parse(matches[2].ToString(), CultureInfo.InvariantCulture);
                    }
                    if (joint456Regex.IsMatch(line) && currentHomenum > 0)
                    {
                        j456Found = true;
                        var matches = joint456Regex.Matches(line);
                        joints[3] = double.Parse(matches[0].ToString(), CultureInfo.InvariantCulture);
                        joints[4] = double.Parse(matches[1].ToString(), CultureInfo.InvariantCulture);
                        joints[5] = double.Parse(matches[2].ToString(), CultureInfo.InvariantCulture);
                    }
                    if (ext1Regex.IsMatch(line) && currentHomenum > 0)
                    {
                        ext1Found = true;
                        joints[6] = double.Parse(ext1Regex.Match(line).ToString(), CultureInfo.InvariantCulture);
                    }
                    if (string.IsNullOrEmpty(line.Trim()) && firtsHomeFound && currentHomenum > 0 && j123Found && j456Found)
                        emptyLineFound = true;
                    if (firtsHomeFound && currentHomenum > 0 && j123Found && j456Found && (ext1Found || emptyLineFound))
                    {
                        if (!ext1Found)
                            joints[6] = -999999;
                        if (!result.Keys.Contains(currentHomenum))
                        {
                            result.Add(currentHomenum, joints.ToList());
                        }
                        else
                        {
                            if (joints[0] != result[currentHomenum][0] || joints[1] != result[currentHomenum][1] || joints[2] != result[currentHomenum][2] || joints[3] != result[currentHomenum][3] || joints[4] != result[currentHomenum][4] || joints[5] != result[currentHomenum][5] || joints[6] != result[currentHomenum][6])
                                MessageBox.Show("Definition of home " + currentHomenum + " is ambiguous. Verify values in .olp files!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        currentHomenum = 0;
                        joints = new double[7];
                        j123Found = false;
                        j456Found = false;
                        ext1Found = false;
                        emptyLineFound = false;
                    }

                }
                if (firtsHomeFound && currentHomenum > 0 && j123Found && j456Found && !result.Keys.Contains(currentHomenum))
                {
                    if (!ext1Found)
                        joints[6] = -999999;
                    result.Add(currentHomenum, joints.ToList());
                }
                reader.Close();
            }
            return result;
        }

        private bool CreateOrgsFanuc()
        {
            var vm = new CreateOrgsViewModel(GlobalData.SrcPathsAndJobs, GlobalData.Jobs);
            CreateOrgs sW = new CreateOrgs(vm);
            var dialogResult = sW.ShowDialog();
            if ((bool)dialogResult)
            {
                FanucOrgs org = new FanucOrgs(vm);
                org.CreateOrgs();
                return true;
            }
            return false;
        }

        internal string DetectWorkbookFile(Dictionary<string, string>.KeyCollection keys)
        {
            try
            {
                string fileToRead = string.Empty;
                foreach (var file in keys)
                {
                    XDocument docu = XDocument.Load(file);
                    var xvrFile = docu.Element("XMLVAR").Elements("PROG").Attributes("name").Where(x=>x.Value == "TPFDEF");
                    if (xvrFile.ToList().Count > 0)
                    {
                        return file;
                    }
                }
                return fileToRead;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}