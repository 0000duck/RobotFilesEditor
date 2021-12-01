using RobotFilesEditor.Dialogs;
using RobotFilesEditor.Model.DataOrganization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static RobotFilesEditor.Model.Operations.FANUC.FanucRobot;

namespace RobotFilesEditor.Model.Operations.FANUC
{
    public class FanucOrgs
    {
        CreateOrgsViewModel orgsVM;
        enum JumpType { Type, JobNum, UserNum, AnyJobUserNum }
        Regex jobNumRegex = new Regex(@"(?<=Job\s+)\d+", RegexOptions.IgnoreCase);
        Regex jobDescrRegex = new Regex(@"(?<=Job\s+\d+\s*\:\s+)[\w\d\s_-]+", RegexOptions.IgnoreCase);
        List<int> alreadyFoundMaintenances;
        string typeSelection, typeString;

        public FanucOrgs(CreateOrgsViewModel _orgsVM)
        {
            orgsVM = _orgsVM;
            InitStrings();
        }

        private void InitStrings()
        {
            //companyName = SrcValidator.language == "DE" ? "Firma" : "Company";
            //dateOfCretion = SrcValidator.language == "DE" ? "Erstellungsdatum" : "Date";
            //robot = SrcValidator.language == "DE" ? "Roboter" : "Robot";
            //createdBy = SrcValidator.language == "DE" ? "Ersteller" : "Programmer";
            //changes = SrcValidator.language == "DE" ? "Aenderungen" : "Changes";
            typeSelection = SrcValidator.language == "DE" ? "TYP Auswahl" : "Type selection";
            typeString = SrcValidator.language == "DE" ? "TYP" : "Type";
        }

        public void CreateOrgs()
        {
            IDictionary<string, string> productionOrgs = CreateProductionOrgs();
            IDictionary<string, string> serviceOrgs = CreateServiceOrgs();
            serviceOrgs.ToList().ForEach(x => productionOrgs.Add(x));
            WriteOrgs(productionOrgs);
        }

        private IDictionary<string,string> CreateProductionOrgs()
        {
            IDictionary<string, string> result = new Dictionary<string, string>();
            List<int> orgNumbers = GetOrgNumbers();
            foreach (var orgNum in orgNumbers)
            {
                List<string> header = CreateHeader(orgNum);
                List<string> typeRequest = CreateFirstJobReq(orgNum);
                List<string> remainingOrgs = CreateRemainingJobs(orgNum);
                List<string> completeOrg = header;
                completeOrg.AddRange(typeRequest);
                completeOrg.AddRange(remainingOrgs);
                completeOrg = RenumberLines(completeOrg);
                result.Add("PROG" + orgNum,CombineOrg(completeOrg));

            }
            return result;
        }


        private IDictionary<string, string> CreateServiceOrgs()
        {
            alreadyFoundMaintenances = new List<int>();
            IDictionary<string, string> result = new Dictionary<string, string>();
            FanucRobotApps apps = FindApplications();
            Dictionary<string, int> maintenancePaths = new Dictionary<string, int>();
            GlobalData.SrcPathsAndJobs.ToList().Where(x => x.Value == 64).ToList().ForEach(y => maintenancePaths.Add(y.Key, AssignNumber(y.Key)));
            result.Add("PROG64", CreateMaintenance(maintenancePaths, 64));
            if (apps.isSpotWelding)
            {
                result.Add("PROG62", CreateMaintenance(new Dictionary<string, int>(), 62));
                result.Add("PROG63", CreateMaintenance(new Dictionary<string, int>(), 63));
            }
            if (apps.isGluing)
                result.Add("PROG50", CreateMaintenance(new Dictionary<string, int>(), 50,"glue"));
            if (apps.isLaser)
            {
                result.Add("PROG50", CreateMaintenance(new Dictionary<string, int>(), 50));
                result.Add("PROG51", CreateMaintenance(new Dictionary<string, int>(), 51));
                result.Add("PROG52", CreateMaintenance(new Dictionary<string, int>(), 52));
                result.Add("PROG53", CreateMaintenance(new Dictionary<string, int>(), 53));
                result.Add("PROG54", CreateMaintenance(new Dictionary<string, int>(), 54));
            }
            return result;
        }

        private string CreateMaintenance(Dictionary<string, int> maintenancePaths, int orgNum, string isGlue = "")
        {
            string result = string.Empty;
            List<string> header = CreateHeader(orgNum);
            List<string> maintenanceOrgBody = CreateMainteananceBody(orgNum, maintenancePaths, isGlue);
            List<string> completeOrg = header;
            completeOrg.AddRange(maintenanceOrgBody);
            completeOrg = RenumberLines(completeOrg);
            result = CombineOrg(completeOrg);

            return result;
        }

        private List<string> CreateMainteananceBody(int orgnum,Dictionary<string, int> maintenancePaths, string isGlue)
        {
            List<string> result = new List<string>();
            ObservableCollection<KeyValuePair<string, int>> usernums = new ObservableCollection<KeyValuePair<string, int>>();
            maintenancePaths.ToList().ForEach(x => usernums.Add(x));
            string jobAndDescription = string.Empty;
            switch (orgnum)
            {
                case 64:
                    { jobAndDescription = "Job 64: Maintenance"; break; }
                case 63:
                    {
                        jobAndDescription = "Job 63: Tipchange";
                        if (orgsVM.SelectedGunsNumber > 0)
                        {
                            for (int i = 1; i <= orgsVM.SelectedGunsNumber; i++)
                                usernums.Add(new KeyValuePair<string, int>("A04_SWP_TIPCHANGE_G"+i, i));
                        }
                        else
                            usernums.Add(new KeyValuePair<string, int>( "A04_SWP_TIPCHANGE_G1", 1));
                        break;
                    }
                case 62:
                    {
                        jobAndDescription = "Job 62: Tipdress";
                        if (orgsVM.SelectedGunsNumber > 0)
                        {
                            for (int i = 1; i <= orgsVM.SelectedGunsNumber; i++)
                                usernums.Add(new KeyValuePair<string, int>("A04_SWP_TIPDRESS_G" + i, i));
                        }
                        else
                            usernums.Add(new KeyValuePair<string, int>("A04_SWP_TIPDRESS_G1", 1));
                        break;
                    }
            }

            if (orgnum > 61)
            {
                OrgsElement element = new OrgsElement() { JobAndDescription = jobAndDescription, Abort = "Home", AbortNr = 1, UserNumValue = usernums };
                result.Add("   1:  WAIT (DO[417:PLC_do_InHome_1]=ON)    ;");
                result.Add("   2:  " + CreateJobReqest(element, false));
                result.Add("   3:   ;");
                result.AddRange(GenerateJump(JumpType.UserNum, element));
            }
            else
                switch (orgnum)
                {
                    case 50:
                        {
                            if (isGlue == "glue")
                                result.Add("   1:  CALL A08_System_Purge1    ;");
                            else
                                result.Add("   1:  CALL LJT_WIRE_CUT    ;");
                            break;
                        }
                    case 51:
                        { result.Add("   1:  CALL LJT_POWER_MEASURE    ;"); break; }
                    case 52:
                        { result.Add("   1:  CALL LJT_TEST_SHOT    ;"); break; }
                    case 53:
                        { result.Add("   1:  CALL LJT_MAINTENANCE    ;"); break; }
                    case 54:
                        { result.Add("   1:  CALL LJT_SAMPLE_PANEL    ;"); break; }
                }
            result.Add("/POS");
            result.Add("/END");
            return result;
        }

        private FanucRobotApps FindApplications()
        {
            FanucRobotApps result = new FanucRobotApps();
            result.isSpotWelding = GlobalData.SrcPathsAndJobs.Keys.Any(x => x.ToLower().Contains("spot_") ? true : false);
            result.isGluing = GlobalData.SrcPathsAndJobs.Keys.Any(x => x.ToLower().Contains("glue_") ? true : false);
            result.isLaser = GlobalData.SrcPathsAndJobs.Keys.Any(x => x.ToLower().Contains("laser_") ? true : false);
            result.isHandling = GlobalData.SrcPathsAndJobs.Keys.Any(x => x.ToLower().ContainsAny(new string[] { "pick_" , "drop_", "search_", "stack_", "center_"}) ? true : false);
            result.isRivet = GlobalData.SrcPathsAndJobs.Keys.Any(x => x.ToLower().Contains("rivet_") ? true : false);
            result.isFLS = GlobalData.SrcPathsAndJobs.Keys.Any(x => x.ToLower().Contains("flow_") ? true : false);
            result.isDocking = GlobalData.SrcPathsAndJobs.Keys.Any(x => x.ToLower().Contains("dock_") ? true : false);
            return result;
        }

        private List<int> GetOrgNumbers()
        {
            List<int> result = new List<int>();
            for (int i = orgsVM.SelectedStartOrgNum; i <= orgsVM.SelectedStartOrgNum + orgsVM.DictOrgsElements.First().Value.Count - 1; i++)
                result.Add(orgsVM.SelectedStartOrgNum + orgsVM.DictOrgsElements.First().Value.Count - (i - orgsVM.SelectedStartOrgNum + 1));
            result.Sort();
            return result;

        }

        private List<string> CreateHeader(int orgNum)
        {
            List<string> result = new List<string>();
            StringReader reader = new StringReader(SrcValidator.language == "DE" ? Properties.Resources.ProgHeaderFanucDE : Properties.Resources.ProgHeaderFanucEN);
            while (true)
            {
                string line = reader.ReadLine();
                if (line == null)
                    break;
                string tempLine = line.Replace("{ProgNum}", orgNum.ToString());
                tempLine = tempLine.Replace("{Date}", DateTime.Now.ToString("yy-MM-dd"));
                tempLine = tempLine.Replace("{Time}", DateTime.Now.ToString("hh:mm:ss"));
                tempLine = tempLine.Replace("{Date2}", DateTime.Now.ToString("dd/MM/yyyy"));
                tempLine = tempLine.Replace("{Robot}", orgsVM.RobotName);
                result.Add(tempLine);
            }
            reader.Close();
            return result;
        }

        private List<string> CreateFirstJobReq(int orgNum)
        {
            List<string> result = new List<string>();
            var initialJob = orgsVM.DictOrgsElements.First().Value.ToList()[orgNum-orgsVM.SelectedStartOrgNum];
            string jobDesrc = GetJobDescr(initialJob.OrgsElement);
            result.Add(initialJob.OrgsElement.WithPart == "false" ? "  11:  WAIT (DO[417:PLC_do_InHome_1]=ON)    ;" : "  11:  WAIT (DO[418:PLC_do_InHome_2]=ON)    ;");
            result.Add("  12:   ;");
            result.Add("  13:  !********************* ;");
            result.Add("  14:  !* "+typeSelection+" ;");
            result.Add("  15:  !* WAIT FOR " + jobDesrc + " ;");
            result.Add("  16:  !********************* ;");
            result.Add("  17:  " + CreateJobReqest(initialJob.OrgsElement,true));
            result.Add("  18:   ;");
            result.AddRange(GenerateJump(JumpType.Type, initialJob.OrgsElement));
            result.Add("  22:   ;");
            return result;
        }

        private List<string> CreateRemainingJobs(int orgNum)
        {            
            List<string> result = new List<string>();
            Dictionary<int, ICollection<IOrgsElement>> dataToProcess = GetDataToProcess(orgNum);
            int currentLabelToEscapeAnyjob = 900;
            //int currentLabelToEscapeAnyjobUsernum = 950;
            foreach (var typ in dataToProcess)
            {
                bool firstPathProcessed = false;
                bool isUserNum = false, isAnyJob = false;
                string typname = GetTypeName(typ.Key, orgsVM.SelectedLine);
                result.Add("  23:  !********************* ;");
                result.Add("  24:  !******  Start  ****** ;");
                result.Add("  25:  ! "+typeString+" "+ typname + " ;");
                result.Add("  26:  !********************* ;");
                result.Add("  27:  LBL[" + typ.Key + ": " + typname + "] ;");
                result.Add("  28:   ;");
                if (typ.Value.First().OrgsElement.Path.ToLower().Contains("anyjob"))
                {
                    isAnyJob = true;
                    currentLabelToEscapeAnyjob++;
                    result.AddRange(GenerateJump(JumpType.JobNum, typ.Value.First().OrgsElement, typ.Key));
                    result.Add("  18:   ;");
                    foreach (var job in typ.Value.First().OrgsElement.AnyJobValue)
                    {
                        result.Add("  28:  LBL[" + CreateAnyJobNumLabel(typ.Key, job.Value) + ": Job " + job.Value + " ] ;");
                        if (typ.Value.First().OrgsElement.JobAndDescription.ToLower().Contains("usernum"))
                        {
                            var anyjobusernum = typ.Value.First().OrgsElement.AnyJobUserNumValue[job.Value];
                            result.AddRange(GenerateJump(JumpType.AnyJobUserNum, typ.Value.First().OrgsElement, typ.Key,anyjobusernum.First().Value));
                            result.Add("  30:   ;");
                        }
                        else
                            result.Add("  28:  CALL " + job.Key + "    ;");
                        result.Add("  32:  JMP LBL["+currentLabelToEscapeAnyjob+"] ;");
                        result.Add("  18:   ;");
                    }
                    result.Add("  34:  LBL["+currentLabelToEscapeAnyjob+"] ;");
                }
                else if (typ.Value.First().OrgsElement.Path.ToLower().Contains("usernum"))                
                {
                    isUserNum = true;
                    result.AddRange(GenerateJump(JumpType.UserNum, typ.Value.First().OrgsElement, typ.Key));
                    result.Add("  30:   ;");
                }
                if (!isAnyJob && !isUserNum)
                {
                    result.Add("  29:  CALL " + typ.Value.First().OrgsElement.Path + "    ;");
                    result.Add("  30:   ;");
                }                
                foreach (var path in typ.Value)
                {
                    isUserNum = false;
                    isAnyJob = false;
                    if (firstPathProcessed)
                    {
                        string jobDesrc = GetJobDescr(path.OrgsElement);
                        result.Add("  31:  !********************* ;");
                        result.Add("  32:  !* WAIT FOR " + jobDesrc + " ;");
                        result.Add("  33:  !********************* ;");
                        result.Add("  34:  " + CreateJobReqest(path.OrgsElement, false));
                        result.Add("  35:   ;");
                        if (path.OrgsElement.JobAndDescription.ToLower().Contains("anyjob"))
                        {
                            isAnyJob = true;
                            currentLabelToEscapeAnyjob++;
                            result.AddRange(GenerateJump(JumpType.JobNum, path.OrgsElement, typ.Key));
                            result.Add("  36:   ;");
                            foreach (var job in path.OrgsElement.AnyJobValue)
                            {
                                result.Add("  28:  LBL[" + CreateAnyJobNumLabel(typ.Key, job.Value) + ": Job " + job.Value + " ] ;");
                                result.Add("  37:  !********************* ;");
                                result.Add("  38:  !* WAIT FOR JOB " + job.Value + " " + GlobalData.Jobs[job.Value] + " ;");
                                result.Add("  39:  !********************* ;");
                                result.Add("  39:  " + CreateJobReqest(path.OrgsElement, false, job.Value));
                                result.Add("  34:   ;");
                                if ((path.OrgsElement.JobAndDescription.ToLower().Contains("usernum")))
                                {
                                    var anyjobusernum = path.OrgsElement.AnyJobUserNumValue[job.Value];
                                    result.AddRange(GenerateJump(JumpType.AnyJobUserNum, path.OrgsElement, typ.Key, anyjobusernum.First().Value));
                                    result.Add("  30:   ;");
                                }
                                else
                                {
                                    result.Add("  35:  CALL " + job.Key + "    ;");
                                }
                                result.Add("  32:  JMP LBL[" + currentLabelToEscapeAnyjob + "] ;");
                                result.Add("  34:   ;");
                            }
                            result.Add("  34:  LBL[" + currentLabelToEscapeAnyjob + "] ;");
                        }
                        else if (path.OrgsElement.JobAndDescription.ToLower().Contains("usernum"))
                        {
                            isUserNum = true;
                            result.AddRange(GenerateJump(JumpType.UserNum, path.OrgsElement, typ.Key));
                            result.Add("  30:   ;");
                        }
                        if (!isAnyJob && !isUserNum)
                        {
                            result.Add("  36:  CALL " + path.OrgsElement.Path + "    ;");
                            result.Add("  37:   ;");
                        }
                    }
                    firstPathProcessed = true;
                }
                result.Add("  68:  JMP LBL[999] ;");
                result.Add("  37:   ;");
            }
            result.Add("  70:  !********************* ;");
            result.Add("  71:  LBL[999: End] ;");
            result.Add("  72:  !********************* ;");
            result.Add("/POS");
            result.Add("/END");

            return result;
        }

        private string BuildAnyJobString(ObservableCollection<KeyValuePair<string, int>> anyJobValue)
        {
            string result = "ANYJOB: ";
            foreach (var job in anyJobValue)
            {
                result += GlobalData.Jobs[job.Value] + " / ";
            }
            result = result.Remove(result.Length - 3, 3);
            return result;
        }

        private string CreateJobReqest(OrgsElement job, bool isInintialJob, int anyJobNumber = 0)
        {
            if (isInintialJob)
            {
                job.Abort = "Home";
                job.AbortNr = job.WithPart == "true" ? 2 : 1;
            }
            string result = string.Empty;
            bool isUserNum = false, isAnyJob = false;
            if (job.JobAndDescription == "ANYJOB")
                isAnyJob = true;
            else if (job.JobAndDescription == "ANYJOB/USERNUM")
            {
                isAnyJob = true;
                isUserNum = true;
            }
            else if (job.JobAndDescription == "USERNUM" || job.JobAndDescription.ContainsAny(new string[] { "Maintenance" , "Tipdress", "Tipchange"}))
                isUserNum = true;

            if (anyJobNumber > 0)
                result = "PR_CALL CMN(\"JobRequest\"=0,\"JobNo\"=" + anyJobNumber + "," + (isInintialJob ? "\"TypeReq\"=2" : "\"*\" = 1") + "," + (isUserNum ? "\"UserReq\"=2" : "\"*\"=1") + "," + (job.Abort == "Home" ? "\"AbortHome\"=2" : "\"AbortPath\"=3") + ",\"Arg\"=" + job.AbortNr + ",'" + GlobalData.Jobs[anyJobNumber] + "') ;";
            else
                result = "PR_CALL CMN(\"JobRequest\"=0,\"JobNo\"=" + (isAnyJob ? "0" : GetJobNrInJobReq(job)) + "," + (isInintialJob ? "\"TypeReq\"=2" : "\"*\"=1") + "," + (isUserNum ? "\"UserReq\"=2" : "\"*\"=1") + "," + (job.Abort == "Home" ? "\"AbortHome\"=2" : "\"AbortPath\"=3") + ",\"Arg\"=" + job.AbortNr + ",'" + GetJobDescr(job) + "') ;";

            return result;
        }

        private string GetJobNrInJobReq(OrgsElement orgsElement)
        {
           
            if (orgsElement.JobAndDescription.Contains("USERNUM"))
                return orgsElement.UserNumValue.First().Value.ToString();
            return jobNumRegex.Match(orgsElement.JobAndDescription).ToString();
        }

        private List<string> GenerateJump(JumpType jumptype, OrgsElement element, int typNum = 0, int anyjobusernumNr = 0)
        {
            string jumpRegister = string.Empty;
            string initialValue = string.Empty;
            List<int> orgElements = new List<int>();
            List<string> result = new List<string>();
            switch (jumptype)
            {
                case JumpType.Type:
                    {
                        jumpRegister = "186:Type Num";
                        initialValue = orgsVM.DictOrgsElements.First().Key.ToString();
                        orgElements = orgsVM.DictOrgsElements.Keys.ToList();
                        result.Add("  19:  SELECT R[" + jumpRegister + "]=" + initialValue + ",JMP LBL[" + initialValue + "] ;");
                        break;
                    }
                case JumpType.JobNum:
                    {
                        jumpRegister = "187:Job Num";
                        initialValue = CreateAnyJobNumLabel(typNum, element.AnyJobValue.First().Value);
                        element.AnyJobValue.ToList().ForEach(x => orgElements.Add(x.Value));
                        result.Add("  19:  SELECT R[" + jumpRegister + "]=" + element.AnyJobValue.First().Value + ",JMP LBL[" + initialValue + "] ;");
                        break;
                    }
                case JumpType.UserNum:
                    {
                        jumpRegister = "185:User Num";
                        element.UserNumValue.ToList().ForEach(x => orgElements.Add(x.Value));
                        result.Add("  19:  SELECT R[" + jumpRegister + "]=" + orgElements.First() + ",CALL " + element.UserNumValue.First().Key + " ;");
                        break;
                    }
                case JumpType.AnyJobUserNum:
                    {
                        jumpRegister = "185:User Num";
                        //element.OrgsElement.AnyJobUserNumValue.ToList().ForEach(x=>x.Value.ForEach(y=> orgElements.Add(y.Value)));
                        element.AnyJobUserNumValue.ToList().First(x => x.Key == anyjobusernumNr).Value.ForEach(y => orgElements.Add(y.Value));
                        result.Add("  19:  SELECT R[" + jumpRegister + "]="+ orgElements.First() + ",CALL " + element.AnyJobUserNumValue[anyjobusernumNr].First().Key + " ;");
                        break;
                    }
            }
            bool firstCycle = true;
            int usernumCounter = 1;
            foreach (var lblNum in orgElements)
            {
                if (!firstCycle)
                {
                    switch (jumptype)
                    {
                        case JumpType.Type:
                            { result.Add("  20:         =" + lblNum.ToString() + ",JMP LBL[" + lblNum.ToString() + "] ;"); break; }
                        case JumpType.JobNum:
                            { result.Add("  20:         =" + lblNum.ToString() + ",JMP LBL[" + CreateAnyJobNumLabel(typNum, lblNum) + "] ;"); break; }
                        case JumpType.UserNum:
                            {
                                result.Add("  20:         =" + lblNum + ",CALL " + element.UserNumValue.First(x => x.Value == lblNum).Key + " ;");
                                usernumCounter++;
                                break;
                            }
                        case JumpType.AnyJobUserNum:
                            {
                                result.Add("  20:         =" + lblNum + ",CALL " + element.AnyJobUserNumValue[anyjobusernumNr][usernumCounter].Key + " ;");
                                usernumCounter++;
                                break;
                            }
                    }
                    
                }
                firstCycle = false;
            }
            result.Add("  21:         ELSE,CALL WAITFOREVER ;");
            return result;
        }

        private string CreateAnyJobNumLabel(int typNum, int value)
        {
            string twoSignjob = value.ToString();
            if (value < 10)
                twoSignjob = "0" + value.ToString();            
            return "1" + typNum.ToString() + twoSignjob;
        }

        private string GetTwoSignJobNum(int value)
        {
            if (value >= 10)
                return value.ToString();
            return "0" + value;
        }

        private string GetTypeName(int key, string line)
        {
            List<string> types = ConfigurationManager.AppSettings["Line_" + line].Split(',').Select(s => s.Trim()).ToList();
            return types[key - 1];
        }

        private string CombineOrg(List<string> strings)
        {
            string result = string.Empty;
            strings.ForEach(x => result += x + "\r\n");
            return result;
        }


        private List<string> RenumberLines(List<string> completeOrg)
        {
            int counter = 0;
            Regex bodyStartRegex = new Regex(@"^\s*/MN", RegexOptions.IgnoreCase);
            Regex lineNumRegex = new Regex(@"^\s*\d+\s*\:", RegexOptions.IgnoreCase);
            bool beginingEnd = false;
            List<string> result = new List<string>();
            List<string> begining = new List<string>();
            List<string> body = new List<string>();
            foreach (var line in completeOrg)
            {
                if (!beginingEnd)
                    begining.Add(line);
                else
                    body.Add(line);
                if (bodyStartRegex.IsMatch(line))
                    beginingEnd = true;
            }
            body = RenumberLinesFanucMethods.GetRenumberedBody(body);
            foreach (var line in body.Where(x=> lineNumRegex.IsMatch(x)))
                counter++;
            var indexToChange = begining.FindIndex(x => x.Trim().ToLower().Contains("line_count"));
            begining[indexToChange] = "LINE_COUNT	= " + counter + ";";
            result = begining;
            result.AddRange(body);
            return result;
        }

        private void WriteOrgs(IDictionary<string, string> productionOrgs)
        {
            IDictionary<string, string> allOrgs = new Dictionary<string, string>();
            productionOrgs.ToList().ForEach(x => allOrgs.Add(x));

            string destPath = GlobalData.DestinationPath + "\\Orgs";
            if (!Directory.Exists(destPath))
                Directory.CreateDirectory(destPath);
            string orgsCreated = "";
            foreach (var org in allOrgs)
            {
                if (File.Exists(destPath + "\\" + org.Key + ".ls"))
                {
                    DialogResult dialogResult = MessageBox.Show("File " + destPath + "\\" + org.Key + ".ls already exists. Overwrite?", "Overwrite files?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        File.Delete(destPath + "\\" + org.Key + ".ls");
                        File.WriteAllText(destPath + "\\" + org.Key + ".ls", org.Value);
                        orgsCreated += destPath + "\\" + org.Key + ".ls\r\n";
                    }
                }
                else
                {
                    File.WriteAllText(destPath + "\\" + org.Key + ".ls", org.Value);
                    orgsCreated += destPath + "\\" + org.Key + ".ls\r\n";
                }

            }
            MessageBox.Show("Following organization programs were created:\r\n" + orgsCreated, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private Dictionary<int, ICollection<IOrgsElement>> GetDataToProcess(int orgNum)
        {
            Dictionary<int, ICollection<IOrgsElement>> result = new Dictionary<int, ICollection<IOrgsElement>>();
            foreach (var type in orgsVM.DictOrgsElements)
            {
                result.Add(type.Key, new List<IOrgsElement>());
                int iterationCounter = orgsVM.SelectedStartOrgNum;
                foreach (var element in type.Value)
                {
                    if (iterationCounter >= orgNum)
                        result[type.Key].Add(element);
                    iterationCounter++;
                }
            }

            return result;
        }

        private string GetJobDescr(OrgsElement orgsElement)
        {
            if (orgsElement.JobAndDescription.Contains("ANYJOB"))
                return BuildAnyJobString(orgsElement.AnyJobValue);
            if (orgsElement.JobAndDescription.Contains("USERNUM"))
                return GlobalData.Jobs[orgsElement.UserNumValue.First().Value];
            if (jobDescrRegex.IsMatch(orgsElement.JobAndDescription))
                return jobDescrRegex.Match(orgsElement.JobAndDescription).ToString();
            return orgsElement.JobAndDescription;
        }

        private int AssignNumber(string key)
        {
            if (key.ToLower().Contains("clean"))
                return 100;
            int startNum = 0, result = 0;
            if (key.ToLower().Contains("a03"))
                startNum = 5;
            else if (key.ToLower().ContainsAny(new string[] { "a02_", "b02_", "c02_" }))
                startNum = 9;
            else
                startNum = 1;
            while (true)
            {
                if (!alreadyFoundMaintenances.Contains(startNum))
                {
                    alreadyFoundMaintenances.Add(startNum);
                    return startNum;
                }
                else
                    startNum++;

            }
        }
    }
}
