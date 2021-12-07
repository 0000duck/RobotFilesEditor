using RobotFilesEditor.Dialogs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RobotFilesEditor.Model.Operations.FANUC
{
    public class FanucFilesValidator
    {
        #region props
        public List<string> FilesList { get; set; }
        public IDictionary<string, FanucRobot> FilesAndContent { get; set; }
        public string RobotName { get; set; }
        #endregion

        #region fields
        private enum CurrentType { Undef, Motion, Spot, Glue, Search, ProcCall, FrameDef }
        Regex collzoneNumberRegex = new Regex(@"(?<=^\s*\d+\s*\:(.*PR_CALL.*CollZone.*ZoneNo\s*.\s*\=\s*|\s*!\s*Coll\s*))\d+", RegexOptions.IgnoreCase);
        Regex collDescrRegex = new Regex(@"(?<=^\s*\d+\s*\:(.*PR_CALL.*CollZone.*ZoneNo\s*.\s*\=.*,\s*'|\s*!\s*Coll.*\d+\s*-))[\w\d\s,-_]*", RegexOptions.IgnoreCase);
        Regex isJobRegex = new Regex(@"(?<=^\s*\d+\s*\:.*PR_CALL.*Job.*JobNo\s*.\s*\=\s*)\d+", RegexOptions.IgnoreCase);
        Regex jobDescrRegex = new Regex(@"(?<=^\s*\d+\s*\:.*PR_CALL.*Job.*JobNo\s*.\s*\=.*,\s*')[\w\d\s-_,]*", RegexOptions.IgnoreCase);
        string logContent;
        #endregion

        public FanucFilesValidator(List<string> filesList, out string logContentOut)
        {
            logContent = string.Empty;
            FilesList = filesList;
            GetRobotName();
            FilesAndContent = ReadFiles();
            FillGlobalData();
            FilesAndContent = AddHeader();
            CheckOpenAndCloseCommands();
            FilesAndContent = CheckJobsAndCollisions();
            FilesAndContent = AddSpaces();
            FilesAndContent = RenumberLines();
            logContentOut = logContent;
        }

        private IDictionary<string, FanucRobot> CheckJobsAndCollisions()
        {
            bool fillDescrs = false;
            IDictionary<string, FanucRobot> result = new Dictionary<string, FanucRobot>();
            IDictionary<int, string> collAndDescription = GetCollDescriptions();
            DialogResult dialogResult = MessageBox.Show("Would you like to fill the description in Collzone statement?\r\nYes - Fill Colldescr\r\nNo - leave it blank", "Fill descriptions", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
                fillDescrs = true;
            foreach (var file in FilesAndContent)
            {
                List<string> lines = RemoveCollComments(file.Value.ProgramSection);
                List<string> linesToAdd = new List<string>();
                foreach (var line in lines)
                {
                    if (collzoneNumberRegex.IsMatch(line))
                    {
                        int collZoneNumber = int.Parse(collzoneNumberRegex.Match(line).ToString());
                        linesToAdd.Add(" 666:  ! Coll " + collZoneNumber.ToString() + " - " + collAndDescription[collZoneNumber] + " ;");
                        string tempLine = string.Empty;
                        if (fillDescrs)
                            tempLine = collDescrRegex.Replace(line, collAndDescription[collZoneNumber]);
                        else
                            tempLine = collDescrRegex.Replace(line, "...");
                        linesToAdd.Add(tempLine);
                    }
                    else if (isJobRegex.IsMatch(line))
                    {
                        string templine = string.Empty;
                        int jobNum = int.Parse(isJobRegex.Match(line).ToString());
                        templine = jobDescrRegex.Replace(line, GlobalData.Jobs[jobNum]);
                        linesToAdd.Add(templine);
                    }
                    else   
                        linesToAdd.Add(line);
                }
                result.Add(file.Key, new FanucRobot(file.Value.InitialSection, linesToAdd, file.Value.DeclarationSection));
            }
            return result;
        }

        private List<string> RemoveCollComments(List<string> programSection)
        {
            List<string> result = new List<string>();
            Regex collzoneDescrLine = new Regex(@"(?<=^\s*\d+\s*\:\s*!\s*Coll\s*)\d+", RegexOptions.IgnoreCase);
            foreach (var line in programSection.Where(x => !collzoneDescrLine.IsMatch(x)))
                result.Add(line);
            return result;
        }

        private IDictionary<int, string> GetCollDescriptions()
        {
            IDictionary<int, string> result = new Dictionary<int, string>();
            IDictionary<int, List<string>> tempCollList = new SortedDictionary<int, List<string>>();
            foreach (var file in FilesAndContent)
            {
                foreach (var line in file.Value.ProgramSection.Where(x=>collzoneNumberRegex.IsMatch(x)))
                {
                    int collNum = int.Parse(collzoneNumberRegex.Match(line).ToString());
                    if (!tempCollList.Keys.Contains(collNum))
                        tempCollList.Add(collNum, new List<string>());
                    string descr = collDescrRegex.Match(line).ToString().Replace(";", "").Trim();
                    if (!tempCollList[collNum].Contains(descr))
                        tempCollList[collNum].Add(descr);
                }
            }
            foreach (var collision in tempCollList)
            {
                SelectColisionViewModel vm = new SelectColisionViewModel(collision, false);
                SelectCollisionFromDuplicate sW = new SelectCollisionFromDuplicate(vm);
                var dialogResult = sW.ShowDialog();
                result.Add(collision.Key, vm.RequestText);
            }

            return result;
        }

        private IDictionary<string, FanucRobot> AddSpaces()
        {
            Regex isMotionPoint = new Regex(@"^\s*\d+\s*\:\s*(J|L)\s*(PR|P)\s*\[\s*\d+\s*(|\:\s*[\w\d-_]+)\s*\]\s+\d+(\%|mm/sec)\s+(FINE|CNT\d+)\s*;", RegexOptions.IgnoreCase);
            Regex isSpotPoint = new Regex(@"^\s*\d+\s*\:\s*(J|L)\s*(PR|P)\s*\[\s*\d+.*SWP_P", RegexOptions.IgnoreCase);
            Regex isGluePoint = new Regex(@"^\s*\d+\s*\:\s*(J|L)\s*(PR|P)\s*\[\s*\d+.*GL_P", RegexOptions.IgnoreCase);
            Regex isSearchPoint = new Regex(@"^\s*\d+\s*\:\s*(J|L)\s*(PR|P)\s*\[\s*\d+.*SEARCH_S_P", RegexOptions.IgnoreCase);
            Regex isProcedureCall = new Regex(@"^\s*\d+\s*\:\s*PR_CALL", RegexOptions.IgnoreCase);
            Regex isFrameDef = new Regex(@"^\s*\d+\s*\:\s*(UFRAME|UTOOL|PAYLOAD)", RegexOptions.IgnoreCase);

            IDictionary<string, FanucRobot> result = new Dictionary<string, FanucRobot>();
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
                result.Add(file.Key, new FanucRobot(file.Value.InitialSection, currentFileContent, file.Value.DeclarationSection));
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

        private IDictionary<string, FanucRobot> RenumberLines()
        {
            IDictionary<string, FanucRobot> result = new Dictionary<string, FanucRobot>();
            foreach (var file in FilesAndContent)
            {
                List<string> renumberedProgramSection = RenumberLinesFanucMethods.GetRenumberedBody(file.Value.ProgramSection);
                result.Add(file.Key, new FanucRobot(file.Value.InitialSection, renumberedProgramSection, file.Value.DeclarationSection));
            }
            return result;
        }

        private IDictionary<string, FanucRobot> ReadFiles()
        {
            Regex isprogramSectionStart = new Regex(@"^\d+\s*\:(J|L|\s)", RegexOptions.IgnoreCase);
            Regex isDeclarationSectionStart = new Regex(@"^\/\s*POS\s*$", RegexOptions.IgnoreCase);
            IDictionary<string, FanucRobot> result = new Dictionary<string, FanucRobot>();
            foreach (var file in FilesList)
            {
                int sectionNum = 1;
                List<string> initialSection = new List<string>();
                List<string> programSection = new List<string>();
                List<string> declarationSection = new List<string>();
                StreamReader reader = new StreamReader(file);
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (sectionNum ==1 && isprogramSectionStart.IsMatch(line.Trim()) || sectionNum == 2 && isDeclarationSectionStart.IsMatch(line.Trim()))
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
                result.Add(file, new FanucRobot(initialSection,programSection,declarationSection));
            }
            return result;
        }

        private IDictionary<string, FanucRobot> AddHeader()
        {
            Regex isOldHeaderPart = new Regex(@"^\s*\d+\s*\:\s*!\s*\*", RegexOptions.IgnoreCase);
            Regex commentRegex = new Regex(@"^\s*\d+\s*\:\s*!", RegexOptions.IgnoreCase);
            IDictionary<string, FanucRobot> result = new Dictionary<string, FanucRobot>();
            foreach (var file in FilesAndContent)
            {
                int headreLinesCounter = 0;
                List<string> header = CreateHeader(Path.GetFileNameWithoutExtension(file.Key));
                List<string> currentFile = new List<string>();
                foreach (var line in file.Value.ProgramSection)
                {
                    if (!commentRegex.IsMatch(line))
                        headreLinesCounter = 2;
                    if (headreLinesCounter >= 2)
                        currentFile.Add(line);
                    if (isOldHeaderPart.IsMatch(line))
                        headreLinesCounter++;
                }
                List<string> newProgramSection = header;
                newProgramSection.AddRange(currentFile);
                result.Add(file.Key, new FanucRobot(file.Value.InitialSection, newProgramSection, file.Value.DeclarationSection));
            }
            return result;
        }

        internal static string GetFileContenetFANUC(FanucRobot files)
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
            header.Add("   1:  ! *********************************************;");
            header.Add("   2:  ! Prog: "+progname+";");
            header.Add("   3:  ! created on: "+DateTime.Now.ToString("yyyy-MM-dd")+";");
            header.Add("   4:  ! IR: "+RobotName+";");
            header.Add("   5:  ! Creator: "+ConfigurationManager.AppSettings["Ersteller"] + ";");
            header.Add("   6:  ! Last Update: "+ DateTime.Now.ToString("yyyy-MM-dd") + ";");
            header.Add("   7:  ! *********************************************;");

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
    }
}