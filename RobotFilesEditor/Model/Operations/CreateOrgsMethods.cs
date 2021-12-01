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
using CommonLibrary;
using RobotFilesEditor.Dialogs;
using RobotFilesEditor.Model.DataInformations;
using RobotFilesEditor.Model.DataOrganization;
using static RobotFilesEditor.Dialogs.OrgsElementVM;
using RobotFilesEditor.ViewModel;
namespace RobotFilesEditor.Model.Operations
{

    public class CreateOrgsMethods
    {
        static string plcAndRobotName;

        static bool isSchweissen;
        static bool isKleben;
        static bool isHandling;
        static bool isRivet;
        static bool isArcWeld;
        static bool isHem;

        internal void CreateOrgs(Dictionary<int, ICollection<IOrgsElement>> data, int selectedToolsNumber, bool isSafeRobot, string line, int gunsNumber, string plcnum, string robotname, int startOrgNum, bool waitForInHome)
        {
            string ablaufName = SrcValidator.language == "DE" ? "_ablauf_" : "_sequence_";
            string tipchange = SrcValidator.language == "DE" ? "Kappenwechsel" : "Tipchange";
            string tipdress = SrcValidator.language == "DE" ? "Kappenfraesen" : "Tipdress";
            string braketest = SrcValidator.language == "DE" ? "Bremsentest" : "Braketest";
            string masref = SrcValidator.language == "DE" ? "Justagereferenzierung" : "Masterreference";
            string purge = SrcValidator.language == "DE" ? "Spulen" : "Purge";
            string diecheck = SrcValidator.language == "DE" ? "Matrix Prufen" : "Die check";
            string maintenance = SrcValidator.language == "DE" ? "Wartung" : "Maintenance";


            int controllerType = 0;
            if (GlobalData.ControllerType == "KRC2 L6")
                controllerType = 1;
            else if (GlobalData.ControllerType == "KRC2 V8")
                controllerType = 2;
            else if (GlobalData.ControllerType == "KRC4")
                controllerType = 3;
            data.Remove(0);
            data = RemoveEmptyTypes(data);
            plcAndRobotName = GetOrgName(line + plcnum);
            if (!string.IsNullOrEmpty(robotname))
                plcAndRobotName = line + plcnum + "_" + robotname;
            IDictionary<string, string> orgs = new Dictionary<string, string>();
            if (ValidOrgs(data) && data.Count > 0)
            {
                isSchweissen = FindProcess(data,"schw","spot");
                isKleben = FindProcess(data,"kleb", "glue");
                isHandling = FindProcess(data, "drop", "pick");
                isRivet = FindProcess(data, "rivet", "hsn");
                isArcWeld = FindProcess(data, "arc", "arc", notContain: "search");
                isHem = FindProcess(data, "hem", "hem");
                if (startOrgNum == 1)
                {
                    if (isSchweissen && gunsNumber <= 1)
                    {
                        if (controllerType == 1)
                            orgs.Add(plcAndRobotName + "_org29", CreateHeader(29, 1, waitForInHome) + CreateProcedure("tip_change_pr1_auto () ; Kappenwechsel", 0, 29, "Kappenwechsel"));
                        else if (controllerType == 2)
                        {
                            orgs.Add("prog030_tipchange", CreateHeader(30, 1, waitForInHome,comment: tipchange, progname: "prog030_tipchange", robot: robotname) + CreateProcedure("tip_change_pr1_auto () ; "+ tipchange, 0, 30, tipchange));
                            orgs.Add("prog031_tipdress", CreateHeader(31, 1, waitForInHome, comment: tipdress, progname: "prog031_tipdress", robot: robotname) + CreateProcedure("tip_dress_pr1 () ; " + tipdress, 0, 31, tipdress));
                        }
                        else if (controllerType == 3)
                        {
                            orgs.Add("prog063_TipChange", CreateHeader(63, 1, waitForInHome, comment: tipchange, progname: "prog063_TipChange", robot: robotname) + CreateProcedure("A04_swp_TipChange_G1 () ; "+tipchange, 0, 63, tipchange));
                            orgs.Add("Prog062_TipDress", CreateHeader(62, 1, waitForInHome, comment: tipdress, progname: "Prog062_TipDress", robot: robotname) + CreateProcedure("A04_swp_TipDress_G1 () ; "+ tipdress, 0, 62, tipdress));
                        }
                    }
                    else if (isSchweissen && gunsNumber > 1)
                    {
                        if (controllerType == 1)
                        {
                            for (int i = 1; i <= gunsNumber; i++)
                            {
                                int orgnum = 30 - i;
                                orgs.Add(plcAndRobotName + "_org" + orgnum, CreateHeader(orgnum, 1, waitForInHome) + CreateProcedure("tip_change_pr" + i + "_auto () ; "+ tipchange, 0, orgnum, tipchange));
                            }
                        }
                        else if (controllerType == 2)
                        {
                            orgs.Add("prog030_tipchange", CreateHeader(30, 1, waitForInHome, comment: tipchange, progname: "prog030_tipchange", robot: robotname) + CreateProcedure("tip_change_pr1_auto () ; "+tipchange, selectedToolsNumber, 30, descr: tipchange, gunsNumber: gunsNumber));
                            orgs.Add("prog031_tipdress", CreateHeader(31, 1, waitForInHome, comment: tipdress, progname: "prog031_tipdress", robot: robotname) + CreateProcedure("tip_dress_pr1 () ; "+ tipdress, selectedToolsNumber, 31, descr: tipdress, gunsNumber: gunsNumber));
                        }
                        else if (controllerType == 3)
                        {
                            orgs.Add("prog063_TipChange", CreateHeader(63, 1, waitForInHome, comment: tipchange, progname: "prog063_TipChange", robot: robotname) + CreateProcedure("A04_swp_TipChange_G1 () ; "+tipchange, selectedToolsNumber, 63, tipchange, gunsNumber: gunsNumber));
                            orgs.Add("Prog062_TipDress", CreateHeader(62, 1, waitForInHome, comment: tipdress, progname: "Prog062_TipDress", robot: robotname) + CreateProcedure("A04_swp_TipDress_G1 () ; "+tipdress, selectedToolsNumber, 62, tipdress, gunsNumber: gunsNumber));
                        }
                    }
                    if (isKleben)
                    {
                        if (controllerType == 1)
                            orgs.Add(plcAndRobotName + "_org25", CreateHeader(25, 1, waitForInHome) + CreateProcedure("gl_system_purge () ; "+purge, 0, 25, purge));
                        else if (controllerType == 2)
                            orgs.Add("prog050_autopurge", CreateHeader(50, 1, waitForInHome, comment: purge, progname: "prog050_autopurge", robot: robotname) + CreateProcedure("gl_system_purge () ; "+ purge, 0, 50, purge));
                        else if (controllerType == 3)
                        { }
                    }
                    if (isRivet)
                        if (controllerType == 2)
                            orgs.Add("prog025_MatrixCheckZN1", CreateHeader(25, 1, waitForInHome, comment: diecheck, progname: "prog025_MatrixCheckZN1", robot: robotname) + CreateProcedure("A13_rvt_DieCheck1 () ; "+diecheck, 0, 25, diecheck));
                        else if (controllerType == 3)
                            orgs.Add("prog050_MatrixCheck", CreateHeader(50, 1, waitForInHome, comment: diecheck, progname: "prog050_MatrixCheck", robot: robotname) + CreateProcedure("A13_rvt_DieCheck1 () ; "+diecheck, 0, 50, diecheck));
                    if (isArcWeld)
                        if (controllerType == 3)
                        {
                            orgs.Add("Prog050_BrennerWechsel", CreateHeader(50, 1, waitForInHome, comment: "Brenner wechseln", progname: "Prog050_BrennerWechsel", robot: robotname) + CreateProcedure("A14_Arc_TipChange () ; Brenner wechseln", 0, 50, "Brenner wechseln",addJobDone:true));
                            orgs.Add("Prog051_BrennerReinigung", CreateHeader(51, 1, waitForInHome, comment: "Brenner Reinigung", progname: "Prog051_BrennerReinigung", robot: robotname) + CreateProcedure("A14_arc_Torchcleaner () ; Brenner Reinigung", 0, 51, "Brenner Reinigung", addJobDone: true));
                            orgs.Add("Prog052_DrahtSchneiden", CreateHeader(52, 1, waitForInHome, comment: "Draht Schneiden", progname: "Prog052_DrahtSchneiden", robot: robotname) + CreateProcedure("A14_arc_wirecutter () ; Draht Schneiden", 0, 52, "Draht Schneiden", addJobDone: true));
                            orgs.Add("Prog053_TCP_Vermessung", CreateHeader(53, 1, waitForInHome, comment: "TCP Prufen", progname: "Prog053_TCP_Vermessung", robot: robotname) + CreateProcedure("A14_arc_TcpCheck () ; TCP Prufen\r\nA14_arc_TestTable () ; Pruftisch", 0, 53, "TCP Prufen", addJobDone: true));
                            orgs.Add("Prog054_DrahtWechsel", CreateHeader(54, 1, waitForInHome, comment: "Draht wechseln", progname: "Prog054_DrahtWechsel", robot: robotname) + CreateProcedure("A14_arc_wirechange () ; Draht wechseln", 0, 54, "Draht wechseln", addJobDone: true));
                        }
                    if (isSafeRobot)
                    {
                        if (controllerType == 1)
                            orgs.Add(plcAndRobotName + "_org31", CreateHeader(31, 1, waitForInHome) + CreateProcedure("masref ( ) ; Justage\r\nbraketest ( ) ; Bremsentest", 0, 31, "Bremsentest und Justage"));
                        else if (controllerType == 3)
                        {
                            orgs.Add("prog250_masterreference", CreateHeader(250, 1, waitForInHome, comment: masref, progname: "prog250_masterreference", robot: robotname) + CreateProcedure("A01_Masterreference () ; "+masref, 0, 250, masref));
                            orgs.Add("prog251_braketest", CreateHeader(251, 1, waitForInHome, comment: braketest, progname: "prog251_braketest", robot: robotname) + CreateProcedure("A01_BrakeTest () ; "+braketest, 0, 251, braketest));
                        }
                    }
                    else
                    {
                        if (controllerType == 1)
                            orgs.Add(plcAndRobotName + "_org31", CreateHeader(31, 1, waitForInHome) + CreateProcedure("braketest ( ) ; "+braketest, 0, 31, braketest));
                        else if (controllerType == 3)
                        {
                            orgs.Add("prog251_braketest", CreateHeader(251, 1, waitForInHome, comment: braketest, progname: "prog251_braketest", robot: robotname) + CreateProcedure("A01_BrakeTest () ; "+braketest, 0, 251, braketest));
                        }
                    }
                    if (controllerType == 2)
                    {
                        orgs.Add("prog061_masterreference", CreateHeader(61, 1, waitForInHome, comment: masref, progname: "prog061_masterreference", robot: plcAndRobotName) + CreateProcedure("MASREF ( ) ; "+masref, 0, 61, masref, addJobReq: false));
                        orgs.Add("prog062_braketest", CreateHeader(62, 1, waitForInHome, comment: braketest, progname: "prog062_braketest", robot: plcAndRobotName) + CreateProcedure("BRAKETEST () ; "+braketest, 0, 62, braketest, addJobReq: false));
                    }
                }  
                List<int> typesList = new List<int>();
                foreach (int type in data.Keys)
                    typesList.Add(type);
                bool isStartOrgGreaterThen1 = false;
                if (startOrgNum > 1)
                    isStartOrgGreaterThen1 = true;

                int orgLength = GetOrgLength(data);
                List<int> dummyTypes = CreateDummyTypes(line, data);
                for (int i = startOrgNum; i <= (orgLength + startOrgNum-1); i++)
                {
                    //string header = CreateHeader(i, GetHomeNum(i, data[GetCorrectHomeIndex(i, data)]));
                    if (controllerType == 1)
                    {
                        string header = CreateHeader(i, GetCorrectHomeIndex(i, data), waitForInHome);
                        string waitHome = CreateWaitTypBit(data, isSchweissen, i, typesList[0], gunsNumber);
                        string types = CreateAllTypes(data, (i - startOrgNum + 1), isSchweissen, line, typesList, dummyTypes, gunsNumber);
                        if (types == null)
                            return;
                        orgs.Add(plcAndRobotName + "_org" + i, header + waitHome + types);
                    }
                    else if (controllerType == 2)
                    {
                        string firstJobEnable = "", header = "";
                        if (isStartOrgGreaterThen1)
                        {
                            
                            header = CreateHeader(i, GetCorrectHomeIndex(i - startOrgNum + 1, data), waitForInHome, comment: CreateProgName(i) + ablaufName + i, progname: CreateProgName(i) + ablaufName + i, robot: robotname);
                            firstJobEnable = GetFirstJobEnable(data, i - startOrgNum + 1);
                        }
                        else
                        {
                            header = CreateHeader(i, GetCorrectHomeIndex(i, data), waitForInHome, comment: CreateProgName(i) + ablaufName + i, progname: CreateProgName(i) + ablaufName + i, robot: robotname);
                            firstJobEnable = GetFirstJobEnable(data, i);
                        }
                        //
                        string types = GetTypesKRC2_V8(data, (i - startOrgNum + 1), typesList,line);
                        orgs.Add(CreateProgName(i) + ablaufName + i, header + firstJobEnable + types);
                    }
                    else if (controllerType == 3)
                    {
                        string firstJobEnable = "", header = "";
                        if (isStartOrgGreaterThen1)
                        {
                            header = CreateHeader(i, GetCorrectHomeIndex(i - startOrgNum + 1, data), waitForInHome, comment: CreateProgName(i) + ablaufName + i, progname: CreateProgName(i) + ablaufName + i, robot: robotname);
                            firstJobEnable = GetFirstJobEnable(data, i - startOrgNum + 1);
                        }
                        else
                        {
                            header = CreateHeader(i, GetCorrectHomeIndex(i, data), waitForInHome, comment: CreateProgName(i) + ablaufName + i, progname: CreateProgName(i) + ablaufName + i, robot: robotname);
                            firstJobEnable = GetFirstJobEnable(data, i);
                        }
                        string types = GetTypesKRC4(data, (i-startOrgNum+1), typesList, line);
                        orgs.Add(CreateProgName(i) + ablaufName + i, header + firstJobEnable + types);
                    }
                }
                if (startOrgNum == 1)
                {
                    if (controllerType == 1)
                    {
                        orgs.Add(plcAndRobotName + "_org30", CreateHeader(30, 1, waitForInHome) + CreateProcedure("Service", selectedToolsNumber, 30, maintenance));
                        orgs.Add("cell", GetCell(plcAndRobotName, orgs.Keys));
                    }
                    else if (controllerType == 2)
                    {
                        orgs.Add("prog032_maintenance", CreateHeader(32, 1, waitForInHome, comment: maintenance, progname: "prog032_maintenance", robot: robotname) + CreateProcedure("Service_KRC2_V8", selectedToolsNumber, 32, maintenance, isSchweissen, isKleben, isHandling, isRivet, gunsNumber: gunsNumber));
                        orgs.Add("cell", GetCell(robotname, orgs.Keys));
                    }
                    else if (controllerType == 3)
                    {
                        orgs.Add("Prog064_Maintenance", CreateHeader(64, 1, waitForInHome, comment: maintenance, progname: "Prog064_Maintenance", robot: robotname) + CreateProcedure("Service_KRC4", selectedToolsNumber, 64, maintenance, isSchweissen, isKleben, isHandling, isRivet, gunsNumber: gunsNumber));
                        orgs.Add("cell", GetCell(robotname, orgs.Keys));
                    }
                }
                else
                    ModifyCell(GlobalData.DestinationPath + "\\Orgs", orgs, startOrgNum);
                
                string destPath = GlobalData.DestinationPath + "\\Orgs";
                if (!Directory.Exists(destPath))
                    Directory.CreateDirectory(destPath);
                WriteAllOrgs(destPath, orgs);
            }
        }

        private string CreateProgName(int i)
        {
            string progrString = i.ToString();
            switch (progrString.Length)
            {
                case 1:
                    { return "prog00" + i; }
                case 2:
                    { return "prog0" + i; }
                case 3:
                    { return "prog" + i; }
            }
            return "prog00 + i";
        }

        private static void ResetApps()
        {
            isSchweissen = false;
            isKleben = false;
            isHandling = false;
            isRivet= false;
            isArcWeld = false;
        }

        private static void ModifyCell(string v, IDictionary<string, string> orgs, int startOrgNum)
        {
            string startStrings = "", switchContent = "", endstring = "";
            bool switchFound = false, defaultfound = false;
            if (!File.Exists(v + "\\cell.src"))
                return;
            StreamReader reader = new StreamReader(v + "\\cell.src");
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (line.ToLower().Replace(" ","").Trim().Contains("case"))
                    switchFound = true;
                if (line.ToLower().Contains("default"))
                    defaultfound = true;
                if (!switchFound && !defaultfound)
                    startStrings += line + "\r\n";
                if (switchFound && !defaultfound)
                    switchContent += line + "\r\n";
                if (switchFound && defaultfound)
                    endstring += line + "\r\n";
            }
            reader.Close();
            string resultCases = ModifyCases(switchContent, orgs, startOrgNum);
            File.Delete(v + "\\cell.src");
            File.WriteAllText(v + "\\cell.src", startStrings + resultCases + endstring);

        }

        private static string ModifyCases(string switchContent, IDictionary<string, string> orgs, int startOrgNum)
        {
            Regex getCaseRegex = new Regex(@"(?<=CASE\s+)\d+", RegexOptions.IgnoreCase);
            Regex getOrgNum = new Regex(@"(?<=(ablauf_|sequence_|org))\d+", RegexOptions.IgnoreCase);
            SortedDictionary<int, string> foundCases = new SortedDictionary<int, string>();
            StringReader reader = new StringReader(switchContent);
            string currentString = "";
            while (true)
            {
                string line = reader.ReadLine();
                if (line == null)
                    break;
                if (line.ToLower().Contains ("case "))
                {
                    if (currentString != "")
                    {
                        foundCases.Add(int.Parse(getCaseRegex.Match(currentString).ToString()), currentString);
                        currentString = "";
                    }
                }
                currentString += line + "\r\n";
            }
            foundCases.Add(int.Parse(getCaseRegex.Match(currentString).ToString()), currentString);
            foreach (var org in orgs.Keys)
            {
                int currentOrgNumber = int.Parse(getOrgNum.Match(org).ToString());
                if (GlobalData.ControllerType == "KRC2 L6")
                {
                    if (!foundCases.Keys.Contains(currentOrgNumber))
                        foundCases.Add(currentOrgNumber, String.Join(Environment.NewLine, "    CASE " + currentOrgNumber, "       P00_BMW_PGNO_ACK ( M_PGNO ) ; Reset Progr.No.-Request", "       " + org + " ( )", "", ""));
                }
                else if (GlobalData.ControllerType == "KRC2 V8")
                {
                    if (!foundCases.Keys.Contains(currentOrgNumber))
                        foundCases.Add(currentOrgNumber, String.Join(Environment.NewLine, "CASE " + currentOrgNumber, "P00_BMW_PGNO_ACK ( M_PGNO ) ; Reset Progr.No.-Request", org + " ( )", "", ""));
                }
                else if (GlobalData.ControllerType == "KRC4")
                {
                    if (!foundCases.Keys.Contains(currentOrgNumber))
                        foundCases.Add(currentOrgNumber, String.Join(Environment.NewLine, "    CASE " + currentOrgNumber, "       Plc_P00_BMW_PGNO_ACK(plc_i_M_PGNO) ; Reset Progr.No.- Request", "       "+org+" ( ) ; Call User-Program", "", ""));
                }
            }
            string result = "";
            foreach (var org in foundCases)
            {
                result += org.Value;
            }

            return result;
        }

        private static int GetCorrectHomeIndex(int i, Dictionary<int, ICollection<IOrgsElement>> data)
        {
            if (i == 1)
            {
                if (data.FirstOrDefault().Value == null || data.FirstOrDefault().Value.FirstOrDefault() == null)
                    return 1;
                if (data.First().Value.First().OrgsElement.WithPart == "true")
                    return 2;
                return 1;
            }
            else
            {
                int homenum = 0;
                foreach (var type in data.Where(x => x.Value.Count >= 2))
                {
                    int counter = 0;
                    foreach (var job in type.Value)
                    {
                        if (counter == i - 1)
                        {
                            if (job.OrgsElement.WithPart == "true")
                                homenum = 2;
                            else
                                homenum = 1;
                            break;
                        }
                        counter++;
                    }

                }
                return homenum;
            }
            return 0;
        }

        private static int GetOrgLength(Dictionary<int, ICollection<IOrgsElement>> data)
        {
            int result = 0;
            foreach (var item in data)
            {
                if (item.Value.Count > result)
                    result = item.Value.Count;
            }

            return result;
        }

        private static Dictionary<int, ICollection<IOrgsElement>> RemoveEmptyTypes(Dictionary<int, ICollection<IOrgsElement>> data)
        {
            Dictionary<int, ICollection<IOrgsElement>> result = new Dictionary<int, ICollection<IOrgsElement>>();
            foreach (var type in data)
            {
               foreach (var job in type.Value)
                {
                    if (job.OrgsElement.Path != null)
                    {
                        result.Add(type.Key,type.Value);
                        break;
                    }
                }
            }

            return result;
        }

        private static void WriteAllOrgs(string destPath,IDictionary<string, string> orgs)
        {
            string orgsCreated = "";
            foreach (var org in orgs)
            {
                if (File.Exists (destPath + "\\" + org.Key + ".src"))
                {
                    DialogResult dialogResult = MessageBox.Show("File " + destPath + "\\" + org.Key + ".src already exists. Overwrite?", "Overwrite files?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        File.Delete(destPath + "\\" + org.Key + ".src");
                        File.WriteAllText(destPath + "\\" + org.Key + ".src", org.Value);
                        orgsCreated += destPath + "\\" + org.Key + ".src\r\n";
                    }
                }
                else
                {
                    File.WriteAllText(destPath + "\\" + org.Key + ".src", org.Value);
                    orgsCreated += destPath + "\\" + org.Key + ".src\r\n";
                }

            }
            MessageBox.Show("Following organization programs were created:\r\n" + orgsCreated, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static string CreateProcedure(string proc, int selectedToolsNumber, int jobNr, string descr, bool isSpot = false, bool isGlue = false, bool isHandling = false, bool isRivet = false, bool addJobReq = true, int gunsNumber = 0, bool addJobDone = false)
        {
            string maintenance = SrcValidator.language == "DE" ? "Wartung" : "Maintenance";
            string clean = SrcValidator.language == "DE" ? "Reinigung" : "Clean position";
            if (proc == "Service")
            {
                if (selectedToolsNumber == 1)
                {
                    return ";FOLD JOB.REQUEST JobNr=" + jobNr + ",  , DESC=" + descr + ";%{PE}%MKUKATPUSER\r\nJob_req (" + jobNr + ")\r\n;ENDFOLD\r\nService_pr1 () ; "+maintenance+"\r\n;FOLD Job.Finished\r\nJob_Finishwork(" + jobNr + ")\r\nJob_Finished(" + jobNr + ")\r\n;ENDFOLD\r\n\r\nEND";
                }
                else
                {
                    string result = "";
                    for (int i = 0; i <= selectedToolsNumber; i++)
                    {
                        string binary = ReverseString(Convert.ToString(i, 2));
                        while (binary.Length < 3)
                            binary += "0";
                        result += "IF " + GetLogicNot(binary[0]) + "$IN[I_TypBit[30] AND " + GetLogicNot(binary[1]) + "$IN[I_TypBit[31] AND " + GetLogicNot(binary[2]) + "$IN[I_TypBit[32] THEN\r\n;FOLD JOB.REQUEST JobNr=" + jobNr + ",  , DESC=" + descr + ";%{PE}%MKUKATPUSER\r\nJob_req (" + jobNr + ")\r\n;ENDFOLD\r\n\tService_pr" + i + " () ; "+maintenance+" \r\n;FOLD Job.Finished\r\nJob_Finishwork(" + jobNr + ")\r\nJob_Finished(" + jobNr + ")\r\n;ENDFOLD" + i + "\r\nELSE\r\n";
                    }
                    result = result.Substring(0, result.Length - 6);
                    for (int i = 0; i <= selectedToolsNumber; i++)
                    {
                        result += "ENDIF\r\n";
                    }
                    result += "\r\nEND";
                    return result;
                }
            }
            else if (proc == "Service_KRC2_V8")
            {
                string result = String.Join(Environment.NewLine,
                    ";-------------------------Job32-------------------------",
                    "WAIT FOR $IN[I_Enable_Job[32]] OR $IN[I_CANCLE_PROG_PLC]",
                    "CONTINUE",
                    "IF $IN[I_CANCLE_PROG_PLC] THEN",
                    "RETURN",
                    "ENDIF",
                    "",
                    ";-------------------------GETUSERNUM--------------------",
                    ";FOLD PLC_COM.Request TypNum=FALSE, UserNum=TRUE, JobNr=32, DESC="+maintenance+";%{PE}%MKUKATPUSER",
                    "PLC_ReqNum(FALSE, TRUE, 32)",
                    ";ENDFOLD",
                    "",
                    "SWITCH PLC_GETUSERNUM()",
                    "", "");
                if (isSpot)
                {
                    if (gunsNumber == 0)
                        gunsNumber = 1;
                    for (int i = 1; i <= gunsNumber; i++)
                        result += "CASE "+i+"\r\nA04_swp_Maintenan_"+i+" ()\r\n\r\n";
                }
                if (isGlue)
                    result += "CASE 1\r\nA08_gl_Maintenan_1 ()\r\n\r\n";
                if (isHandling)
                {
                    for (int i = 1; i <= selectedToolsNumber - gunsNumber; i++)
                        result += "CASE "+(4+i).ToString()+"\r\nA03_grp_Maintenan_Grp"+i+" ()\r\n\r\n";
                }
                if (isRivet)
                {
                    for (int i = 1; i <= selectedToolsNumber - gunsNumber; i++)
                        result += "CASE " + (i).ToString() + "\r\nB13_rvt_GunMaintenan_" + i + " ()\r\n\r\n";
                }
                if (GlobalData.HasToolchager)
                    result += "CASE 9\r\nA02_tch_Maintenan_PTM ()\r\n\r\n";
                result += ";--------------User15 "+clean+"---------------\r\nCASE 15\r\nmaintenance_Clean()\r\n\r\nDEFAULT\r\nWAIT FOR FALSE\r\nENDSWITCH\r\nEND";
                return result;
            }
            else if (proc == "Service_KRC4")
            {
                string result = String.Join(Environment.NewLine,
                    ";FOLD Job Request JobNum:64 UserNumReq Abort:Home1 Desc:"+maintenance,
                    ";FOLD ;%{h}",
                    ";Params IlfProvider=job; JobCmd=Request; JobNum=64; JobTypeNumReq=False; JobUserNumReq=True; JobAbort=1; JobCont=_; JobDesc="+maintenance,
                    ";ENDFOLD",
                    "Plc_JobReq(64,1,False,True,1,0,FALSE)",
                    ";ENDFOLD",
                    "",
                    "SWITCH PLC_GETUSERNUM()",
                    "",
                    "");
                if (isSpot)
                {
                    if (gunsNumber == 0)
                        gunsNumber = 1;
                    for (int i = 1; i <= gunsNumber; i++)
                        result += "CASE " + i + "\r\nA04_swp_Maintenan_G" + i + " ()\r\n\r\n";
                }
                if (isGlue)
                    result += "CASE 1\r\nA08_gl_Maintenan_1 ()\r\n\r\n";
                if (isRivet)
                    result += "CASE 1\r\nA13_rvt_GunMaintenan1 ()\r\n\r\n";
                if (isArcWeld)                    
                    result += "CASE 1\r\nA14_Arc_Maintenance1 ()\r\n\r\n";
                if (isHem)
                    result += "CASE 1\r\nA23_hem_Maintenan1 ()\r\n\r\n";
                if (isHandling)
                {
                    for (int i = 1; i <= selectedToolsNumber - gunsNumber; i++)
                        result += "CASE " + (4 + i).ToString() + "\r\nA03_grp_Maintenan_Grp" + i + " ()\r\n\r\n";
                }
                if (GlobalData.HasToolchager)
                    result += "CASE 9\r\nA02_tch_Maintenan_PTM ()\r\n\r\n";
                result += "CASE 100\r\nmaintenance_Clean()\r\n\r\nDEFAULT\r\nWAIT FOR FALSE\r\nENDSWITCH\r\nEND";
                return result;
            }
            else
            {
                string result = "";
                if (addJobReq)
                    //if (selectedToolsNumber > 1)
                    result = CreateMultipleToolsServicePath(jobNr, descr, proc,selectedToolsNumber, addJobDone, gunsNumber:gunsNumber);
                    //else
                    //    result = ";FOLD JOB.REQUEST JobNr=" + jobNr + ",  , DESC=" + descr + ";%{PE}%MKUKATPUSER\r\nJob_req (" + jobNr + ")\r\n;ENDFOLD\r\n" + proc + "\r\n;FOLD Job.Finished\r\nJob_Finishwork(" + jobNr + ")\r\nJob_Finished(" + jobNr + ")\r\n;ENDFOLD\r\n\r\nEND";
                else
                    result = proc + "\r\n\r\nEND";
                return result;
            }
        }

        private static string CreateMultipleToolsServicePath(int jobNr, string descr, string proc, int selectedToolsNumber, bool addJobDone, int gunsNumber = 0)
        {
            string error = SrcValidator.language == "DE" ? "FEHLER" : "ERROR";
            string result = "";
            if (GlobalData.ControllerType.Contains("KRC2"))
            {
                //Regex prRegex = new Regex(@"(pr\d+|gun\d+|gun_\d+)", RegexOptions.IgnoreCase);
                Regex replRegex = new Regex(@"\d+", RegexOptions.IgnoreCase);
                //string process = prRegex.Match(proc).ToString();                
                result = String.Join(Environment.NewLine,
                    ";-------------------------Job" + jobNr + "-------------------------",
                    "WAIT FOR $IN[I_Enable_Job[" + jobNr + "]] OR $IN[I_CANCLE_PROG_PLC]",
                    "CONTINUE",
                    "IF $IN[I_CANCLE_PROG_PLC] THEN",
                    "RETURN",
                    "ENDIF",
                    "");

                if (gunsNumber > 0)
                    result += String.Join(Environment.NewLine, ";-------------------------GETUSERNUM--------------------",
                     ";FOLD PLC_COM.Request TypNum=FALSE, UserNum=TRUE, JobNr=" + jobNr + ", DESC=" + descr + ";%{PE}%MKUKATPUSER",
                     "PLC_ReqNum(FALSE, TRUE, " + jobNr + ")",
                     ";ENDFOLD",
                     "",
                     "SWITCH PLC_GetUserNum ()",
                     "");
                else
                    result += String.Join(Environment.NewLine,
                        ";FOLD PLC_COM.Request TypNum=FALSE, UserNum=FALSE, JobNr=" + jobNr + ", DESC=" + descr + ";%{PE}%MKUKATPUSER",
                        "PLC_ReqNum(FALSE, FALSE, " + jobNr + ")",
                        ";ENDFOLD",
                        "");
                if (gunsNumber > 0)
                {
                    for (int i = 1; i <= selectedToolsNumber; i++)
                    {
                        string tempstring = String.Join(Environment.NewLine,
                            "",
                            "CASE " + i,
                            replRegex.Replace(proc, i.ToString())
                            , "");
                        result += tempstring;
                    }
                }
                else
                {
                    string tempstring = String.Join(Environment.NewLine,
                            "",
                            proc,
                            "");
                    result += tempstring;
                }
                if (gunsNumber > 1)
                    result += String.Join(Environment.NewLine, "", ";--------------"+error+"---------------", "DEFAULT", "WAIT FOR FALSE", "ENDSWITCH", "END");
                else
                    result += String.Join(Environment.NewLine, "","END");

            }
            else if (GlobalData.ControllerType == "KRC4")
            {
                if (jobNr == 250 || jobNr == 251)
                {
                    result = String.Join(Environment.NewLine,
                        proc,
                        "END");
                }
                else if (jobNr == 63 || jobNr == 62 || jobNr == 50)
                {
                    Regex replace = new Regex(@"(?<=G)\d+", RegexOptions.IgnoreCase);
                    if (gunsNumber > 1)
                    {
                        result = String.Join(Environment.NewLine,
                        ";FOLD Job Request JobNum:" + jobNr + " UserNumReq Abort:Home1 Desc:" + descr,
                        ";FOLD ;%{h}",
                        ";Params IlfProvider=job; JobCmd=Request; JobNum=" + jobNr + "; JobTypeNumReq=False; JobUserNumReq=True; JobAbort=1; JobCont=_; JobDesc=" + descr,
                        ";ENDFOLD",
                        "Plc_JobReq(" + jobNr + ",1,False,True,1,0,FALSE)",
                        ";ENDFOLD",
                        "",
                        "SWITCH PLC_GETUSERNUM()",
                        "",
                        "");
                        for (int i = 1; i <= gunsNumber; i++)
                        {
                            result += String.Join(Environment.NewLine, "CASE " + i, replace.Replace(proc, i.ToString()), "", "");
                        }

                        result += String.Join(Environment.NewLine, "DEFAULT", "PLC_DefaultError()", "ENDSWITCH", "");
                        if (addJobDone)
                        {
                            result += String.Join(Environment.NewLine, ";FOLD Job Done JobNum:" + jobNr + " Desc: " + descr,
                                ";FOLD;%{ h}",
                                ";Params IlfProvider=job; Plc_JobCmd=Done; Plc_JobMove=NoMove; Plc_JobNum=" + jobNr + "; Plc_JobDesc =" + descr,
                                ";ENDFOLD",
                                "Plc_Job(2, " + jobNr + ", False)",
                                ";ENDFOLD",
                                "END");
                        }
                        else
                            result += "\r\nEND";
                    }
                    else
                    {
                        result = String.Join(Environment.NewLine,
                        ";FOLD Job Request JobNum:" + jobNr + " Abort:Home1 Desc:" + descr,
                        ";FOLD ;%{h}",
                        ";Params IlfProvider=job; JobCmd=Request; JobNum=" + jobNr + "; JobTypeNumReq=False; JobUserNumReq=False; JobAbort=1; JobCont=_; JobDesc=" + descr,
                        ";ENDFOLD",
                        "Plc_JobReq(" + jobNr + ",1,False,False,1,0,FALSE)",
                        ";ENDFOLD",
                        "",
                        proc,
                        "");
                        if (addJobDone)
                        {
                            result += String.Join(Environment.NewLine, "", ";FOLD Job Done JobNum:" + jobNr + " Desc: " + descr,
                          ";FOLD;%{ h}",
                          ";Params IlfProvider=job; Plc_JobCmd=Done; Plc_JobMove=NoMove; Plc_JobNum=" + jobNr + "; Plc_JobDesc=" + descr,
                          ";ENDFOLD",
                          "Plc_Job(2, " + jobNr + ", False)",
                          ";ENDFOLD",
                          "END");
                        }
                        else
                            result += "\r\nEND";
                    }
                }
                else
                { }
            }
            return result;
        }

        private static string GetLogicNot(char v)
        {
            if (v == '0')
                return "NOT ";
            else
                return "";
        }

        public static string ReverseString(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        private static bool FindProcess(Dictionary<int, ICollection<IOrgsElement>> data, string arg1, string arg2, string notContain="")
        {
            bool notContainPresent = false;
            if (!String.IsNullOrEmpty(notContain))
                notContainPresent = true;
            
            foreach (var type in data)
            {
                foreach (var path in type.Value.Where(x=>!(x.OrgsElement.Path == "USERNUM" || x.OrgsElement.Path == "ANYJOB")))
                {
                    if (notContainPresent && path.OrgsElement.Path.ToLower().Contains(notContain))
                        return false;
                    if (path.OrgsElement.Path.ToLower().Contains(arg1) || path.OrgsElement.Path.ToLower().Contains(arg2))
                        return true;
                }
                foreach (var usernum in type.Value.Where(x => x.OrgsElement.Path == "USERNUM"))
                {
                    foreach (var path in usernum.OrgsElement.UserNumValue)
                    {
                        if (notContainPresent && path.Key.ToLower().Contains(notContain))
                            return false;
                        if (path.Key.ToLower().Contains(arg1) || path.Key.ToLower().Contains(arg2))
                            return true;
                    }
                }
                foreach (var anyjob in type.Value.Where(x => x.OrgsElement.Path == "ANYJOB"))
                {
                    foreach (var path in anyjob.OrgsElement.AnyJobValue)
                    {
                        if (notContainPresent && path.Key.ToLower().Contains(notContain))
                            return false;
                        if (path.Key.ToLower().Contains(arg1) || path.Key.ToLower().Contains(arg2))
                            return true;
                    }
                }

            }


            return false;
        }

        private static string GetOrgName(string line)
        {

            //List<string> plcNames = new List<string>();
            //foreach (var type in data)
            //{
            //    foreach (var path in type.Value)
            //    {
            //        Regex getPLC = new Regex(@"[a-zA-Z]+\d+[a-zA-Z]+\d+r\d+", RegexOptions.IgnoreCase);
            //        string plcAndRobotName = getPLC.Match(path.OrgsElement.Path).ToString();
            //        if (plcNames.Count == 0)
            //            plcNames.Add(plcAndRobotName);
            //        if (plcNames.Count == 1 && !plcNames.Contains(plcAndRobotName))
            //        {
            //            MessageBox.Show("Inconsistent PLC names of paths. Program will abort", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //            return null;
            //        }
            //    }
            //}
            Regex getRobot = new Regex(@"(\d+[a-zA-Z]*R\d+)|((?<=ir_*)\d+[a-zA-Z]+\d+)", RegexOptions.IgnoreCase);
            string robot = getRobot.Match(GlobalData.Roboter).ToString();
            if (string.IsNullOrEmpty(robot))
                robot = "UnknownRobot";

            return line+"_"+robot;
        }

        private static bool ValidOrgs(Dictionary<int, ICollection<IOrgsElement>> data)
        {
            if (data.Count == 0)
            {
                MessageBox.Show("No types found! Orgs generation failed!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            foreach (var type in data)
            {
                foreach (var job in type.Value)
                {
                    if (job.OrgsElement.Path == null)
                    {
                        MessageBox.Show("No paths selected for one of elements. Orgs generation failed!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    if (job.OrgsElement.Abort == null)
                    {
                        MessageBox.Show("Abort not selected for one of elements. Orgs generation failed!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    if (job.OrgsElement.WithPart == null)
                    {
                        MessageBox.Show("WithPart not selected for one of elements. Orgs generation failed!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }
            //TEMP
            return true;
            
        }

        private static string CreateAllTypes(Dictionary<int, ICollection<IOrgsElement>> data, int iteration, bool isSchweissen, string line, List<int> typeNums, List<int> dummyTypes, int gunsNumber)
        {
            IDictionary<int, ICollection<IOrgsElement>> copyOfData = new Dictionary<int, ICollection<IOrgsElement>>(data);
            List<OrgsElementVM> keysToRemove = new List<OrgsElementVM>();
            Regex getJobNumRegex = new Regex(@"(?<=Job\s+)\d+", RegexOptions.IgnoreCase);
            Regex getJobDescrRegex = new Regex(@"(?<=:\s*).*", RegexOptions.IgnoreCase);
            string result = "";
            bool shortenedOrg = false;
            if (iteration > 1)
                shortenedOrg = true;
            foreach (var type in data)
            {
                foreach (var job in type.Value.Where(x => x.OrgsElement.Id <= (iteration - 1)))
                {
                    keysToRemove.Add((OrgsElementVM)job);
                }
            }
            for (int i = 1; i <= typeNums.Count; i++)
            {
                foreach (var item in keysToRemove)
                    data[typeNums[i - 1]].Remove(item);
            }
            SortedDictionary<int, ICollection<IOrgsElement>> sortedData = new SortedDictionary<int, ICollection<IOrgsElement>>(data);

            foreach (int item in dummyTypes)
                sortedData.Add(item, null);
            
            bool firstCommand = true;
            foreach (var type in sortedData)
            {
                string typ = GetTypeName(type.Key, line);
                result += string.Join(Environment.NewLine,
                ";***********************************************",
                ";*       "+typ+" TypBit ("+type.Key+")",
                ";***********************************************",
                "",
                "IF $IN[I_TYPBIT[" + type.Key + "]] THEN",
                ";FOLD OUT "+(type.Key + 32)+" 'TYP "+ typ+"' State=TRUE ;%{PE}%R 5.6.16,%MKUKATPBASIS,%COUT,%VOUTX,%P 2:33, 3:TYP "+typ+", 5:TRUE, 6:",
                "$OUT[" + (type.Key + 32) + "]=TRUE",
                ";ENDFOLD",
                "",
                "");
                if (type.Value != null)
                {
                    if (type.Value.Count > 0)
                    {
                        foreach (var job in type.Value)
                        {
                            string abort = GetAbortLine(job, firstCommand);
                            string homeToCentral = GetHomeToCentral(job, firstCommand, shortenedOrg);
                            if ((job.OrgsElement.AnyJobValue == null || job.OrgsElement.AnyJobValue.Count == 0) && (job.OrgsElement.UserNumValue == null || job.OrgsElement.UserNumValue.Count == 0))
                            {
                                result += string.Join(Environment.NewLine,
                                   "WAIT FOR $IN[I_ENABLE_JOB[" + getJobNumRegex.Match(job.OrgsElement.JobAndDescription).ToString() + "]] OR $IN[I_CANCLE_PROG_PLC]",
                                   "   IF $IN[I_CANCLE_PROG_PLC] THEN",
                                   "     " + abort + "GOTO ABORT_PLC",
                                   "   ELSE",
                                   "     " + homeToCentral + job.OrgsElement.Path + " () ; " + getJobDescrRegex.Match(job.OrgsElement.JobAndDescription).ToString().Trim(),
                                   "   ENDIF",
                                   "",
                                   ""
                                   );
                            }

                            else if (job.OrgsElement.AnyJobValue != null && job.OrgsElement.AnyJobValue.Count > 0)
                            {
                                string waitcommand = string.Join(Environment.NewLine,
                                    "WAIT FOR " + GetAnyJobs(job.OrgsElement.AnyJobValue) + " OR $IN[I_CANCLE_PROG_PLC]",
                                    "   IF $IN[I_CANCLE_PROG_PLC] THEN",
                                    "     " + abort + "GOTO ABORT_PLC",
                                    "   ELSE",
                                    "");
                                string jobs = GetJobsInAnyJob(job.OrgsElement.AnyJobValue);
                                result += waitcommand + jobs + "   ENDIF\r\n\r\n";
                            }
                            else if (job.OrgsElement.UserNumValue != null && job.OrgsElement.UserNumValue.Count > 0)
                            {
                                if (ValidateUserNum(job.OrgsElement.UserNumValue))
                                {
                                    string waitcommand = string.Join(Environment.NewLine,
                                        "WAIT FOR $IN[I_ENABLE_JOB[" + job.OrgsElement.UserNumValue[0].Value + "]] OR $IN[I_CANCLE_PROG_PLC]",
                                        "   IF $IN[I_CANCLE_PROG_PLC] THEN",
                                        "     " + abort + "GOTO ABORT_PLC",
                                        "   ELSE",
                                        "");
                                    string jobs = GetJobsInUserNum(job.OrgsElement.UserNumValue);
                                    result += waitcommand + jobs + "   ENDIF\r\n\r\n";
                                }
                                else
                                    return null;
                            }

                            firstCommand = false;
                        }
                    }
                    else
                    {
                        string typnotused = SrcValidator.language == "DE" ? "Typ nicht verwendet" : "Type not used";
                        result += "; "+ typnotused + "\r\nWAIT FOR FALSE\r\n\r\n";
                    }
                }
                else
                {
                    string typnotused = SrcValidator.language == "DE" ? "leerer Typ fur zukunftige Modelle" : "Empty type for future models";
                    result += string.Join(Environment.NewLine,
                        "; "+ typnotused, "WAIT FOR FALSE", "", "");

                }
                result += string.Join(Environment.NewLine,
                    ";FOLD OUT " + (type.Key + 32) + " 'TYP " + typ + "' State = FALSE;%{ PE}% R 5.6.16,% MKUKATPBASIS,% COUT,% VOUTX,% P 2:33, 3:TYP " + typ + ", 5:FALSE, 6:",
                    "$OUT[" + (type.Key + 32) + "]=FALSE",
                    ";ENDFOLD","","");
                if (isSchweissen && gunsNumber <= 1)
                    result += "IF CHK_DRESS (1) THEN\r\n  TIP_DRESS_PR1 ( )\r\nENDIF\r\n\r\n";
                else if (isSchweissen && gunsNumber > 1)
                {
                    for (int i = 1; i <= gunsNumber; i++)
                    {
                        result += "IF CHK_DRESS ("+i+") THEN\r\n  TIP_DRESS_PR"+i+" ( )\r\nENDIF\r\n\r\n";
                    }
                }
                result += string.Join(Environment.NewLine,
                    "  GOTO ABORT_PLC",
                    "ENDIF",
                    "",
                    "");
            }
            result += "ABORT_PLC:\r\n\r\nEND";

            //Encoding ascii = Encoding.GetEncoding(1252);
            //Encoding utf8 = Encoding.UTF8;
            //byte[] utfBytes = utf8.GetBytes(result);
            //byte[] asciiBytes = Encoding.Convert(utf8, ascii, utfBytes);
            //string msg = ascii.GetString(asciiBytes);
            return result;

        }

        internal static void CreateOrgsV8(Dictionary<int, ICollection<IOrgsElement>> dictOrgsElements, int selectedToolsNumber, bool safeRobot, string selectedLine, int selectedGunsNumber, string selectedPLC)
        {
            throw new NotImplementedException();
        }

        private static string GetJobsInUserNum(ObservableCollection<KeyValuePair<string, int>> userNumValue)
        {
            string result = "WAIT FOR";
            int counter = 0;
            foreach (var item in userNumValue)
            {
                result += " $IN[" + (16 + counter) + "] OR";
                counter++;
            }
            counter = 0;
            result = result.Remove(result.Length - 3, 3) + "\r\n";
            foreach (var item in userNumValue)
            {
                result += "   IF $IN[" + (16 + counter) + "] THEN\r\n     " + item.Key + "()\r\n   ELSE\r\n";
                counter++;
            }
            result = result.Remove(result.Length - 8, 8);
            result += "\r\n";
            foreach (var item in userNumValue)
            {
                result += "   ENDIF\r\n";
            }
            
            return result;
        }

        private static bool ValidateUserNum(ObservableCollection<KeyValuePair<string, int>> userNumValue)
        {
            List<int> foundJobs = new List<int>();
            foreach (var item in userNumValue)
            {
                if (foundJobs.Count == 0)
                {
                    foundJobs.Add(item.Value);
                }
                if (!foundJobs.Contains(item.Value))
                {
                    MessageBox.Show("Usernumbers do not use the same job!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return true;
        }

        private static string GetJobsInAnyJob(ObservableCollection<KeyValuePair<string, int>> anyJobValue)
        {
            string result = "";
            int counter = 1;
            foreach (var job in anyJobValue)
            {
                result += "   IF $IN[I_ENABLE_JOB[" + job.Value + "]] THEN\r\n     " +  job.Key + "()\r\n   ELSE\r\n";
                counter++;
            }
            result = (result.Remove(result.Length - 7, 7)).TrimEnd() + "\r\n";

            for (int i = 1; i < counter; i++)
            {
                result += "   ENDIF\r\n";
            }
            return result;

        }

        private static string GetAnyJobs(ObservableCollection<KeyValuePair<string, int>> anyJobValue)
        {
            string result = "";
            foreach (var job in anyJobValue)
            {
                result += "$IN[I_ENABLE_JOB[" + job.Value + "]] OR ";
            }
            result = result.Remove(result.Length - 4,4);
            return result;
        }

        private static string GetHomeToCentral(IOrgsElement job, bool firstCommand, bool shortenedOrg)
        {
            if (!shortenedOrg || !firstCommand || job.OrgsElement.Abort == "Home")
                return "";

            string result = "";
            if (job.OrgsElement.WithPart == "true")
                result = "H2_";
            else
                result = "H1_";

            result += GetAbortPathName(job);

            if (job.OrgsElement.AbortNr > 0)
                result += job.OrgsElement.AbortNr;
            result += "()\r\n     ";
           
            return result;

        }

        private static string GetAbortPathName(IOrgsElement job)
        {
            string result = "";
            if (job.OrgsElement.Path.ToLower().Contains("anyjob"))
            {
                job.OrgsElement.Path = job.OrgsElement.AnyJobValue[0].Key; 
            }
            else if (job.OrgsElement.Path.ToLower().Contains("usernum"))
            {
                job.OrgsElement.Path = job.OrgsElement.UserNumValue[0].Key;
            }

            if (job.OrgsElement.Path.ToLower().Contains("spot"))
                result += "SpotPos";
            else if (job.OrgsElement.Path.ToLower().Contains("pick"))
                result += "PickPos";
            else if (job.OrgsElement.Path.ToLower().Contains("drop"))
                result += "DropPos";
            else if (job.OrgsElement.Path.ToLower().Contains("search"))
                result += "SearchPos";
            else if (job.OrgsElement.Path.ToLower().Contains("glue"))
                result += "GluePos";
            else if (job.OrgsElement.Path.ToLower().Contains("dock"))
                result += "DockPos";

            return result;
        }

        private static string GetTypeName(int key, string line)
        {
            List<string> types = ConfigurationManager.AppSettings["Line_"+line].Split(',').Select(s => s.Trim()).ToList();
            return types[key - 1];
        }

        internal static SortedDictionary<int, string> GetTypesWithDescr(string value, List<string> types)
        {
            SortedDictionary<int, string> result = new SortedDictionary<int, string>();
            List<string> typesOnLine = ConfigurationManager.AppSettings["Line_" + value].Split(',').Select(s => s.Trim()).ToList();
            int counter = 1;
            result.Add(0, "");
            foreach (string type in typesOnLine)
            {
                result.Add(counter,typesOnLine[counter-1]);
                counter++;
            }
            return result;
        }

        internal static List<string> GetTypesOrPLCs(string value,string typeOrPlc)
        {
            List<string> result = new List<string>();
            List<string> typesOnLine = ConfigurationManager.AppSettings[typeOrPlc + value].Split(',').Select(s => s.Trim()).ToList();
            Dictionary<int, string> typeNrAndName = new Dictionary<int, string>();
            int typeCounter = 1;

            foreach (var type in typesOnLine)
            {
                if (!string.IsNullOrEmpty(type))
                    typeNrAndName.Add(typeCounter, type);
                typeCounter++;
            }
            result.Add("0");
            int counter = 1;
            if (typeOrPlc == "Line_")
            {
                foreach (var type in typeNrAndName.Keys)
                {
                    result.Add(type.ToString());
                    counter++;
                }
            }
            else
            {
                foreach (string type in typesOnLine)
                    result.Add(type);
            }
            return result;
        }


        private static string GetAbortLine(IOrgsElement job, bool firstElement)
        {
            if (firstElement || job.OrgsElement.Abort == "Home")
                return "";
            else
            {
                string result = GetAbortPathName(job);
                string number = "";
                if (job.OrgsElement.AbortNr > 0)
                    number = "_" + job.OrgsElement.AbortNr.ToString();
                if (GlobalData.ControllerType == "KRC2 L6")
                {
                    if (job.OrgsElement.WithPart == "true")
                        return result + "_H2" + number + "()\r\n     ";
                    else
                        return result + "_H1" + number + "()\r\n     ";
                }
                else if (GlobalData.ControllerType == "KRC2 V8")
                {
                    if (job.OrgsElement.WithPart == "true")
                        return result + "_H2" + number + "()\r\n";
                    else
                        return result + "_H1" + number + "()\r\n";
                }
                else
                    return "";
            }
        }

        private static string CreateWaitTypBit(Dictionary<int, ICollection<IOrgsElement>> data, bool isSchweissen, int iteration, int firstTypeId, int gunsNumber)
        {
            List<int> types = new List<int>(data.Keys.ToList());
            int homenum = GetHomeNum(iteration, data[firstTypeId]);

            string result = "";
            if (GlobalData.ControllerType == "KRC2 L6")
            {
                result = "WAIT FOR bTypBit() ";
                if (homenum == 1 && isSchweissen && gunsNumber <= 1)
                    result += "OR CHK_DRESS (1) OR $IN[I_CANCLE_PROG_PLC]\r\n\r\nCONTINUE\r\nIF $IN[I_CANCLE_PROG_PLC] THEN\r\n RETURN; Halt nach Taktende\r\nENDIF\r\n\r\nIF CHK_DRESS (1) THEN\r\n  TIP_DRESS_PR1 ( )\r\n  GOTO ABORT_PLC\r\nENDIF\r\n\r\nCONTINUE\r\n";
                else if (homenum == 1 && isSchweissen && gunsNumber > 1)
                {
                    string chkDress = "";
                    string chkDressCall = "";
                    for (int i = 1; i <= gunsNumber; i++)
                    {
                        chkDress += "OR CHK_DRESS (" + i + ") ";
                        chkDressCall += "IF CHK_DRESS (" + i + ") THEN\r\n  TIP_DRESS_PR" + i + " ( )\r\n  GOTO ABORT_PLC\r\nENDIF\r\n\r\n";
                    }
                    result += chkDress + "OR $IN[I_CANCLE_PROG_PLC]\r\n\r\nCONTINUE\r\nIF $IN[I_CANCLE_PROG_PLC] THEN\r\n RETURN; Halt nach Taktende\r\nENDIF\r\n\r\n" + chkDressCall + "CONTINUE\r\n";
                }
                else
                    result += "OR $IN[I_CANCLE_PROG_PLC]\r\n\r\nCONTINUE\r\nIF $IN[I_CANCLE_PROG_PLC] THEN\r\n  RETURN; Halt nach Taktende\r\nENDIF\r\n\r\n";
            }
            else if (GlobalData.ControllerType == "KRC2 V8")
            {

            }
            return result;
        }

        private static List<OrganizationLists> FillLists(Dictionary<int, ICollection<IOrgsElement>> types)
        {
            List<OrganizationLists> result = new List<OrganizationLists>();
            foreach (var typ in types.Where(x => x.Key > 0))
            {
                List<string> paths = new List<string>();
                List<string> descriptions = new List<string>();
                List<string> aborts = new List<string>();
                List<string> abortNumbers = new List<string>();
                List<string> withParts = new List<string>();
                List<int> jobNumbers = new List<int>();
                result.Add(new OrganizationLists(typ.Key, paths, descriptions, aborts, abortNumbers, withParts, jobNumbers));
            }
            return result;
        }



        private static string CreateHeader(int progNum, int homenum, bool waitForInHome, string comment = "", string progname = "", string robot = "", int gunsNumber = 0)
        {
            string result = "";
            if (GlobalData.ControllerType == "KRC2 L6")
            {
                result = String.Join(Environment.NewLine,
                    "&ACCESS RVO1",
                    "&REL 16",
                    "&PARAM TEMPLATE = C:\\KRC\\Roboter\\Template\\vorgabe",
                    "&PARAM EDITMASK = *",
                    "DEF " + plcAndRobotName + "_org" + progNum + "( )",
                    ";FOLD INI",
                    ";FOLD BASISTECH INI",
                    "    IF CHK_INIT() THEN",
                    "    GLOBAL INTERRUPT DECL 3 WHEN $STOPMESS==TRUE DO IR_STOPM ( )",
                    "    INTERRUPT ON 3",
                    "    BAS (#INITMOV,0 )",
                    ";ENDFOLD (BASISTECH INI)",
                    ";FOLD USER INI",
                    ";Make your modifications here",
                    "    APPLICATION_INI ( )",
                    ";ENDFOLD (USER INI)",
                    "    ENDIF",
                    ";ENDFOLD (INI)",
                    ";FOLD INIT_OUT",
                    ";Make your modifications here",
                    "    INIT_OUT( )",
                    ";ENDFOLD (INIT_OUT)",
                    ";***********************************************************",
                    ConfigurationManager.AppSettings["Header2" + GlobalData.ControllerType.Replace(" ", "_") + SrcValidator.language] + plcAndRobotName + "_org" + progNum,
                    ConfigurationManager.AppSettings["Header4" + GlobalData.ControllerType.Replace(" ", "_") + SrcValidator.language] + plcAndRobotName,
                    ConfigurationManager.AppSettings["Header7" + GlobalData.ControllerType.Replace(" ", "_") + SrcValidator.language] + DateTime.Now,
                    ConfigurationManager.AppSettings["Header5" + GlobalData.ControllerType.Replace(" ", "_") + SrcValidator.language] + "AIUT",
                    ConfigurationManager.AppSettings["Header6" + GlobalData.ControllerType.Replace(" ", "_") + SrcValidator.language] + ConfigurationManager.AppSettings["Ersteller"],
                    ConfigurationManager.AppSettings["Header8" + GlobalData.ControllerType.Replace(" ", "_") + SrcValidator.language],
                    ";***********************************************************",
                    "",
                    "WAIT FOR $IN_HOME" + homenum, "", ""
                    );
            }
            else if (GlobalData.ControllerType == "KRC2 V8")
            {
                result = String.Join(Environment.NewLine,
                    "&ACCESS RVO1",
                    "&REL 1",
                    "&COMMENT " + comment,
                    "&PARAM TEMPLATE = C:\\KRC\\Roboter\\Template\\vorgabe",
                    "&PARAM EDITMASK = *",
                    "DEF " + progname + " ( )",
                    ";FOLD INI",
                    ";FOLD BASISTECH INI",
                    "    IF CHK_INIT() THEN",
                    "    GLOBAL INTERRUPT DECL 3 WHEN $STOPMESS==TRUE DO IR_STOPM ( )",
                    "    INTERRUPT ON 3",
                    "    BAS (#INITMOV,0 )",
                    ";ENDFOLD (BASISTECH INI)",
                    ";FOLD USER INI",
                    ";Make your modifications here",
                    "    APPLICATION_INI ( )",
                    ";ENDFOLD (USER INI)",
                    "    ENDIF",
                    ";ENDFOLD (INI)",
                    ";FOLD INIT_OUT",
                    ";Make your modifications here",
                    "    INIT_OUT( )",
                    ";ENDFOLD (INIT_OUT)",
                    ";***********************************************************",
                    ConfigurationManager.AppSettings["Header2" + GlobalData.ControllerType.Replace(" ", "_") + SrcValidator.language] + progname,
                    ConfigurationManager.AppSettings["Header4" + GlobalData.ControllerType.Replace(" ", "_") + SrcValidator.language] + robot,
                    ConfigurationManager.AppSettings["Header7" + GlobalData.ControllerType.Replace(" ", "_") + SrcValidator.language] + DateTime.Now,
                    ConfigurationManager.AppSettings["Header5" + GlobalData.ControllerType.Replace(" ", "_") + SrcValidator.language] + "AIUT",
                    ConfigurationManager.AppSettings["Header6" + GlobalData.ControllerType.Replace(" ", "_") + SrcValidator.language] + ConfigurationManager.AppSettings["Ersteller"],
                    ConfigurationManager.AppSettings["Header8" + GlobalData.ControllerType.Replace(" ", "_") + SrcValidator.language],
                    ";***********************************************************",
                    "",
                    "WAIT FOR $IN_HOME" + homenum, "", ""
                    );
            }
            else if (GlobalData.ControllerType == "KRC4")
            {

                if (progNum == 250 || progNum == 251)
                {
                    string homeString = waitForInHome ? "WAIT FOR $IN_HOME1" : String.Join(Environment.NewLine,
                        ";FOLD PlcCheckHome Home: 1",
                        ";FOLD ;%{h}",
                        ";Params IlfProvider=plchome; Plc_ChkHomeNum=1; Plc_ChkRehome=false; Plc_ChkDesc=",
                        ";ENDFOLD",
                        "Plc_CheckHome(1, false)",
                        ";ENDFOLD");
                    result = String.Join(Environment.NewLine,
                        "&ACCESS RVO1",
                        "&REL 42",
                        "&COMMENT " + (SrcValidator.language == "DE" ? "Ablauf " : "Sequence ")  + progNum,
                        "&PARAM EDITMASK = *",
                        "&PARAM TEMPLATE = C:\\KRC\\Roboter\\Template\\vorgabe",
                        "DEF " + progname + "( )",
                        ";###### do not delete this line ######",
                        ";FOLD INI",
                        ";FOLD USER INI",
                        ";Make your modifications here",
                        "IF PLC_CHK_INIT() THEN",
                        "GLOBAL INTERRUPT DECL 3 WHEN $STOPMESS==TRUE DO IR_STOPM ( )",
                        "INTERRUPT ON 3",
                        "BAS (#INITMOV,0 )",
                        ";FOLD APPLICATION_INI",
                        "APPLICATION_INI ( )",
                        ";ENDFOLD (APPLICATION_INI)",
                        "ENDIF",
                        ";ENDFOLD (USER INI)",
                        ";ENDFOLD (INI)",
                        "",
                        ";***********************************************************",
                        ConfigurationManager.AppSettings["Header2" + GlobalData.ControllerType.Replace(" ", "_") + SrcValidator.language] + progname,
                        ConfigurationManager.AppSettings["Header4" + GlobalData.ControllerType.Replace(" ", "_") + SrcValidator.language] + robot,
                        ConfigurationManager.AppSettings["Header7" + GlobalData.ControllerType.Replace(" ", "_") + SrcValidator.language] + DateTime.Now,
                        ConfigurationManager.AppSettings["Header5" + GlobalData.ControllerType.Replace(" ", "_") + SrcValidator.language] +"AIUT",
                        ConfigurationManager.AppSettings["Header6" + GlobalData.ControllerType.Replace(" ", "_") + SrcValidator.language] + ConfigurationManager.AppSettings["Ersteller"],
                        ConfigurationManager.AppSettings["Header8" + GlobalData.ControllerType.Replace(" ", "_") + SrcValidator.language],
                        ";***********************************************************",
                        ";FOLD ; 27/11/2018 10:51:00 NAME: SAS CHANGES: Erstellt;%{PE}%R 8.3.22,%MKUKATPBASIS,%CCOMMENT,%VSTAMP,%P 1:;, 2:27/11/2018 10:51:00, 3:NAME:, 4:SAS, 5:CHANGES:, 6:Erstellt",
                        ";ENDFOLD",
                        "",
                        homeString,
                        ";***********************************************************",
                        "; "+comment+"",
                        ";***********************************************************",
                        "");
                }
                else if (progNum == 64 || progNum == 63 || progNum == 62)
                {
                    string homeString = waitForInHome ? "WAIT FOR $IN_HOME1" : String.Join(Environment.NewLine,
                    ";FOLD PlcCheckHome Home: 1",
                    ";FOLD ;%{h}",
                    ";Params IlfProvider=plchome; Plc_ChkHomeNum=1; Plc_ChkRehome=false; Plc_ChkDesc=",
                    ";ENDFOLD",
                    "Plc_CheckHome(1, false)",
                    ";ENDFOLD");
                    result = String.Join(Environment.NewLine,
                        "&ACCESS RVO1",
                        "&REL 42",
                        "&COMMENT " + (SrcValidator.language == "DE" ? "Ablauf " : "Sequence ") + progNum,
                        "&PARAM EDITMASK = *",
                        "&PARAM TEMPLATE = C:\\KRC\\Roboter\\Template\\vorgabe",
                        "DEF "+progname+"( )",
                        ";FOLD Moduleparameters;%{h}",
                        ";Params Plc_JobCmd=Request; Plc_JobMove=NoMove; Plc_JobNum="+progNum+"; Plc_JobCont=_; Plc_JobTypeNumReq=False; Plc_JobUserNumReq=True",
                        ";ENDFOLD Moduleparameters",
                        ";FOLD Declaration",
                        "INT nAnswer",
                        ";ENDFOLD (Declaration)",
                        ";###### do not delete this line ######",
                        ";FOLD INI",
                        ";FOLD USER INI",
                        ";Make your modifications here",
                        "IF PLC_CHK_INIT() THEN",
                        "GLOBAL INTERRUPT DECL 3 WHEN $STOPMESS==TRUE DO IR_STOPM ( )",
                        "INTERRUPT ON 3",
                        "BAS (#INITMOV,0 )",
                        ";FOLD APPLICATION_INI",
                        "APPLICATION_INI ( )",
                        ";ENDFOLD (APPLICATION_INI)",
                        "ENDIF",
                        ";ENDFOLD (USER INI)",
                        ";ENDFOLD (INI)",
                        ";***********************************************************",
                        ConfigurationManager.AppSettings["Header2" + GlobalData.ControllerType.Replace(" ", "_") + SrcValidator.language] + progname,
                        ConfigurationManager.AppSettings["Header4" + GlobalData.ControllerType.Replace(" ", "_") + SrcValidator.language] + robot,
                        ConfigurationManager.AppSettings["Header7" + GlobalData.ControllerType.Replace(" ", "_") + SrcValidator.language] + DateTime.Now,
                        ConfigurationManager.AppSettings["Header5" + GlobalData.ControllerType.Replace(" ", "_") + SrcValidator.language] +"AIUT",
                        ConfigurationManager.AppSettings["Header6" + GlobalData.ControllerType.Replace(" ", "_") + SrcValidator.language] + ConfigurationManager.AppSettings["Ersteller"],
                        ConfigurationManager.AppSettings["Header8" + GlobalData.ControllerType.Replace(" ", "_") + SrcValidator.language],
                        ";***********************************************************",
                        "",
                        ";FOLD ; 27/11/2018 10:51:00 NAME: SAS CHANGES: Erstellt;%{PE}%R 8.3.22,%MKUKATPBASIS,%CCOMMENT,%VSTAMP,%P 1:;, 2:27/11/2018 10:51:00, 3:NAME:, 4:SAS, 5:CHANGES:, 6:Erstellt",
                        ";ENDFOLD",
                        "",
                        homeString,
                        "");
                    
                }
                else
                {
                    string homeString = waitForInHome ? "WAIT FOR $IN_HOME" + homenum : String.Join(Environment.NewLine,
                        ";FOLD PlcCheckHome Home:" + homenum,
                        ";FOLD ;%{h}",
                        ";Params IlfProvider=plchome; Plc_ChkHomeNum=" + homenum + "; Plc_ChkRehome=false; Plc_ChkDesc=",
                        ";ENDFOLD",
                        "Plc_CheckHome(" + homenum + ", false)",
                        ";ENDFOLD");
                    result = String.Join(Environment.NewLine,
                        "&ACCESS RVO1",
                        "&REL 42",
                        "&COMMENT " + (SrcValidator.language == "DE" ? "Ablauf " : "Sequence ") + progNum,
                        "&PARAM EDITMASK = *",
                        "&PARAM TEMPLATE = C:\\KRC\\Roboter\\Template\\vorgabe",
                        "DEF " + progname + "( )",
                        ";FOLD Moduleparameters;%{h}",
                        ";ENDFOLD Moduleparameters",
                        ";FOLD INI",
                        ";FOLD BASISTECH INI",
                        ";FOLD IF_PLC_CHK_INIT",
                        "IF PLC_CHK_INIT() THEN",
                        ";ENDFOLD (IF_PLC_CHK_INIT)",
                        "GLOBAL INTERRUPT DECL 3 WHEN $STOPMESS==TRUE DO IR_STOPM ( )",
                        "INTERRUPT ON 3",
                        "BAS (#INITMOV,0 )",
                        ";ENDFOLD (BASISTECH INI)",
                        ";FOLD USER INI",
                        ";Make your modifications here",
                        ";FOLD APPLICATION_INI",
                        "APPLICATION_INI ( )",
                        ";ENDFOLD (APPLICATION_INI)",
                        ";ENDFOLD (USER INI)",
                        ";FOLD ENDIF_PLC_CHK_INIT",
                        "ENDIF",
                        ";ENDFOLD (ENDIF_PLC_CHK_INIT)",
                        ";ENDFOLD (INI)",
                        ";***********************************************************",
                        ConfigurationManager.AppSettings["Header2" + GlobalData.ControllerType.Replace(" ", "_") + SrcValidator.language] + progname,
                        ConfigurationManager.AppSettings["Header4" + GlobalData.ControllerType.Replace(" ", "_") + SrcValidator.language] + robot,
                        ConfigurationManager.AppSettings["Header7" + GlobalData.ControllerType.Replace(" ", "_") + SrcValidator.language] + DateTime.Now,
                        ConfigurationManager.AppSettings["Header5" + GlobalData.ControllerType.Replace(" ", "_") + SrcValidator.language] + "AIUT",
                        ConfigurationManager.AppSettings["Header6" + GlobalData.ControllerType.Replace(" ", "_") + SrcValidator.language] + ConfigurationManager.AppSettings["Ersteller"],
                        ConfigurationManager.AppSettings["Header8" + GlobalData.ControllerType.Replace(" ", "_") + SrcValidator.language],
                        ";***********************************************************",
                        ";FOLD ; 27/11/2018 10:51:00 NAME: SAS CHANGES: Erstellt;%{PE}%R 8.3.22,%MKUKATPBASIS,%CCOMMENT,%VSTAMP,%P 1:;, 2:27/11/2018 10:51:00, 3:NAME:, 4:SAS, 5:CHANGES:, 6:Erstellt",
                        ";ENDFOLD",
                        "",
                        homeString,
                        "");

                }
            }
            return result;
        }

        private static int GetHomeNum(int progNum,  ICollection<IOrgsElement> types)
        {
             foreach (var type in types.Where(x => x.OrgsElement.Id == progNum))
            {
                if (type.OrgsElement.WithPart == "false")
                    return 1;
                else
                    return 2;
            }

            return 0;
        }

        private static string GetCell(string robotName,ICollection<string> keys)
        {
            string result = "";
            if (GlobalData.ControllerType.Contains("KRC2"))

                result = string.Join(Environment.NewLine,
                    "&ACCESS RVP",
                    "&REL 11",
                    "&COMMENT External automatic V_1_0_0",
                    "DEF  CELL ( )",
                    "  ;FOLD BASISTECH INI",
                    "  GLOBAL INTERRUPT DECL 3 WHEN $STOPMESS==TRUE DO IR_STOPM ( )",
                    "  INTERRUPT ON 3 ",
                    "  BAS (#INITMOV,0 )",
                    "  ;ENDFOLD (BASISTECH INI)",
                    "	;FOLD APPLICATION_INI",
                    "	  APPLICATION_INI ( )",
                    "	;ENDFOLD",
                    "  ;FOLD Set Cell startes",
                    "   $OUT[O_CELL_STARTED]=TRUE ",
                    "  ;ENDFOLD",
                    "  ;FOLD CHECK HOME",
                    "  IF CHECK_HOME==TRUE THEN",
                    "    PTP HOME_SELECTION ( ) ;Testing Home-Position ",
                    "  ENDIF",
                    "  ;ENDFOLD (CHECK HOME)",
                    ";FOLD INIT_OUT",
                    "INIT_OUT ()",
                    ";ENDFOLD",
                    "",
                    "  ;FOLD AUTOEXT INI",
                    "  P00_BMW_INIT_EXT ( ) ; Initialize extern mode",
                    "  ;ENDFOLD (AUTOEXT INI)",
                    "  LOOP",
                    "  P00_BMW_PGNO_GET ( )",
                    "    SWITCH  M_PGNO ; Select with Programnumber",
                    "",
                    "");
            else if (GlobalData.ControllerType.Contains("KRC4"))
            {
                result += String.Join(Environment.NewLine,
                    "&ACCESS RVP",
                    "&COMMENT V_1_1_0",
                    "&REL 26",
                    "&PARAM DISKPATH = KRC:\\R1",
                    "DEF Cell( )",
                    ";***********************************************************",
                    ConfigurationManager.AppSettings["Header5" + GlobalData.ControllerType.Replace(" ", "_") + SrcValidator.language] +"AIUT",
                    ConfigurationManager.AppSettings["Header2" + GlobalData.ControllerType.Replace(" ", "_") + SrcValidator.language] +"Cell",
                    SrcValidator.language == "DE" ? ";* Erstellung Datum    : " : ";* Date of creation    : " + DateTime.Now,
                    ConfigurationManager.AppSettings["Header4" + GlobalData.ControllerType.Replace(" ", "_") + SrcValidator.language] + robotName,
                    ConfigurationManager.AppSettings["Header6" + GlobalData.ControllerType.Replace(" ", "_") + SrcValidator.language] + ConfigurationManager.AppSettings["Ersteller"],
                    SrcValidator.language == "DE" ? ";* Aenderungen" : ";* Changes",
                    ";FOLD ; 08/04/2014 11:30:00 NAME: SAS CHANGES: Erstellt;%{PE}%R 8.3.22,%MKUKATPBASIS,%CCOMMENT,%VSTAMP,%P 1:;, 2:08/04/2014 11:30:00, 3:NAME:, 4:SAS, 5:CHANGES:, 6:Erstellt",
                    ";ENDFOLD",
                    ";***********************************************************",
                    "",
                    ";FOLD BASISTECH INI",
                    "GLOBAL INTERRUPT DECL 3 WHEN $STOPMESS==TRUE DO IR_STOPM ( )",
                    "INTERRUPT ON 3",
                    "BAS (#INITMOV,0 )",
                    ";ENDFOLD (BASISTECH INI)",
                    ";FOLD RESET ABORTFLAGS",
                    ";Reset variables from abort, which were set in submit",
                    ";must be reset on first time",
                    "PLC_b_AbortActive=FALSE",
                    "PLC_i_AbortState=0",
                    "PLC_b_CallAbortProg=FALSE",
                    ";ENDFOLD (RESET ABORTFLAGS)",
                    ";FOLD APPLICATION_INI",
                    "APPLICATION_INI ( )",
                    ";ENDFOLD (APPLICATION_INI)",
                    ";FOLD Init Variables from abort",
                    "IF ($MODE_OP<>#EX) THEN",
                    ";Reset member for switch by abortprog*********",
                    "plc_b_cellOverAbort=FALSE",
                    "ELSE",
                    "PTP $POS_ACT",
                    "WAIT FOR $NEAR_POSRET",
                    "ENDIF",
                    ";Reset variables for abort, which were setting in submit",
                    "PLC_do_PrgBreakFinished=FALSE",
                    ";ENDFOLD (Init Variables from abort)",
                    "",
                    "",
                    ";FOLD Set CellStarted",
                    "$OUT[PLC_do_CellStarted]=TRUE",
                    ";ENDFOLD (Set CellStarted)",
                    ";FOLD Init_BeforeHome",
                    "InitBeforeHome()",
                    ";ENDFOLD (Init_BeforeHome)",
                    ";FOLD cellOverAbort",
                    ";Switch by Abort-Prog*****************************************************************",
                    "IF (plc_b_cellOverAbort==FALSE) THEN",
                    ";Check is LoadVar set",
                    "PLC_CHECK_LOADVAR()",
                    "",
                    ";FOLD CHECK HOME",
                    "IF CHECK_HOME==TRUE THEN",
                    "PTP Plc_HomeSelection ( ) ;Testing Home-Position",
                    "ENDIF",
                    ";ENDFOLD (CHECK HOME)",
                    "ENDIF",
                    ";FOLD Init_AfterHome",
                    "InitInHome()",
                    "InitAppl()",
                    "InitProduction()",
                    ";ENDFOLD (Init_AfterHome)",
                    "IF (plc_b_cellOverAbort==FALSE) THEN",
                    ";FOLD AUTOEXT INI",
                    "Plc_P00_BMW_INIT_EXT ( ) ; Initialize extern mode",
                    ";ENDFOLD (AUTOEXT INI)",
                    "ENDIF ;****************************************************************************",
                    ";ENDFOLD (cellOverAbort)",
                    "LOOP",
                    "Plc_P00_BMW_PGNO_GET ( )",
                    ";FOLD INIT_ABORT",
                    "IF plc_b_cellOverAbort THEN",
                    "plc_b_AbortActive=FALSE",
                    "plc_b_cellOverAbort=FALSE",
                    "$RED_VEL=100",
                    "ENDIF",
                    ";ENDFOLD (INIT_ABORT)",
                    "SWITCH  plc_i_M_PGNO ; Select with Programnumber",
                    "",
                    "");
                
            }
            int counter = 1;
            List <int> keyList = GetOrgNums(keys.ToList());

            foreach (int program in keyList)
            {
                if (GlobalData.ControllerType == "KRC2 L6")
                {
                    result += string.Join(Environment.NewLine,
                        "    CASE " + program,
                        "       P00_BMW_PGNO_ACK ( M_PGNO ) ; Reset Progr.No.-Request",
                        "       " + robotName + "_org" + program + " ()",
                        "",
                        "");
                    counter++;
                }
                else if (GlobalData.ControllerType == "KRC2 V8")
                {
                    result += string.Join(Environment.NewLine,
                        "    CASE " + program,
                        "       P00_BMW_PGNO_ACK ( M_PGNO ) ; Reset Progr.No.-Request",
                        "       " + GetProgramNameV8(program) + " ()",
                        "",
                        "");
                }
                else if (GlobalData.ControllerType == "KRC4")
                {
                    result += string.Join(Environment.NewLine,
                        "    CASE " + program,
                        "       Plc_P00_BMW_PGNO_ACK (plc_i_M_PGNO ) ; Reset Progr.No.-Request",
                        "       " + GetProgramNameV8(program) + " ( ) ; Call User-Program",
                        "",
                        "");
                }
            }
            if (GlobalData.ControllerType.Contains ("KRC2"))
            result += string.Join(Environment.NewLine,
                    "    DEFAULT",
                    "      P00_BMW_PGNO_FAULT ( M_PGNO )",
                    "    ENDSWITCH",
                    "  ENDLOOP",
                    "",
                    "END");
            else if (GlobalData.ControllerType == "KRC4")
                result += string.Join(Environment.NewLine,
                        "    DEFAULT",
                        "      P00_BMW_PGNO_FAULT ( plc_i_M_PGNO )",
                        "    ENDSWITCH",
                        "",
                        "    ;befor a new program will select, initProduction",
                        "    InitProduction()",
                        "",
                        "  ENDLOOP",
                        "END");
            return result;
        }

        private static List<int> GetOrgNums(List<string> list)
        {
            List<int> result = new List<int>();
            foreach (string item in list)
            {
                string tempItem = "";
                if (GlobalData.ControllerType == "KRC2 L6")
                {
                    tempItem = item.Substring(item.Length - 4, 4);
                    Regex getOrgNum = new Regex(@"\d+", RegexOptions.IgnoreCase);
                    result.Add(int.Parse(getOrgNum.Match(tempItem).ToString()));
                }
                else if (GlobalData.ControllerType == "KRC2 V8" || GlobalData.ControllerType == "KRC4")
                {
                    string resultItem = "";
                    tempItem = item.Substring(4, 3);
                    foreach (char number in tempItem)
                    {
                        if (number == '0' && resultItem == "")
                        { }
                        else
                        {
                            resultItem += number;
                        }
                    }
                    result.Add(int.Parse(resultItem));
                }
                
            }
            result.Sort();
            return result;
        }


        private static List<int> CreateDummyTypes(string line, Dictionary<int, ICollection<IOrgsElement>> data)
        {
            List<int> result = new List<int>();
            int typesRequired=0;
            typesRequired = ConfigurationManager.AppSettings["Line_" + line].Split(',').Select(s => s.Trim()).ToList().Count;
            //if (line == "KG1")
            //    typesRequired = 6;
            //else if (line == "KG2")
            //    typesRequired = 17;
            //else if (line == "SR")
            //    typesRequired = 6;
            //else if (line == "FFG")
            //    typesRequired = 6;

            for (int i = 1; i <= typesRequired; i++)
            {
                if (!data.ContainsKey(i))
                {
                    result.Add(i);
                }
            }

            return result;
        }

        //internal static ObservableCollection<KeyValuePair<string, int>> CreateAnyJob(int? id)
        //{
        //    GlobalData.SelectedJobsForAnyJob = new ObservableCollection<KeyValuePair<string, int>>();
        //    CreateAnyJobViewModel vm = new CreateAnyJobViewModel(id);
        //    CreateAnyJob sW = new CreateAnyJob(vm);
        //    var dialogResult = sW.ShowDialog();

        //    return GlobalData.SelectedJobsForAnyJob;
        //}

        internal static ObservableCollection<KeyValuePair<string, int>> CreateUserNumOrAnyJob(int? id, ChooseType chooseType, int jobNr = 0)
        {
            switch (chooseType)
            {
                case ChooseType.Anyjob:
                    GlobalData.SelectedJobsForAnyJob = new ObservableCollection<KeyValuePair<string, int>>();
                    break;
                case ChooseType.Usernum:
                    GlobalData.SelectedUserNums = new ObservableCollection<KeyValuePair<string, int>>();
                    break;
            }                      
            CreateUserNumsViewModel2 vm = new CreateUserNumsViewModel2(id, chooseType, jobNr);
            CreateUserNum2 sW = new CreateUserNum2(vm);
            var dialogResult = sW.ShowDialog();

            switch (chooseType)
            {
                case ChooseType.Anyjob:
                    return GlobalData.SelectedJobsForAnyJob;
                case ChooseType.Usernum:
                    return GlobalData.SelectedUserNums;
                default:
                    return null;
            }
            
        }

        private static bool GetTypNumReq(Dictionary<int, ICollection<IOrgsElement>> data,int iteration)
        {
            bool isTypNumReq = false;
            if (data.Count == 0)
                return false;
            List<int> keys = data.Keys.ToList<int>();
            var type1 = data[keys[0]];
            int counter = 1;
            foreach (var item in type1)
            {
                if (item.OrgsElement.TypNumReq)
                    isTypNumReq = true;
                if (iteration == counter)
                    break;
                counter++;
            }
            return isTypNumReq;
        }


        private static string GetFirstJobEnable(Dictionary<int, ICollection<IOrgsElement>> data, int iteration)
        {
            bool isTypNmuReq = GetTypNumReq(data,iteration);
            List<int> keys = data.Keys.ToList<int>();
            var type1 = data[keys[0]];
            OrgsElementVM selectedelement = null;
            int counter = 1;
            string usernum = "False";
            string anyjob = "False";
            foreach (var item in type1)
            {
                if (item.OrgsElement.TypNumReq)
                    isTypNmuReq = true;
                selectedelement = (OrgsElementVM)item;
                if (iteration == counter)
                    break;
                counter++;
            }
            int jobNr = 0;
            string jobNrString = "";
            string description = "";
            if (selectedelement.JobAndDescription == "USERNUM")
            {
                jobNr = selectedelement.UserNumValue[0].Value;
                jobNrString = jobNr.ToString();
                description = GlobalData.Jobs[jobNr];
                usernum = "True";
            }
            else if (selectedelement.JobAndDescription == "ANYJOB")
            {
                jobNr = 255;
                jobNrString = "Anyjob";
                description = GetAnyJobDescription(selectedelement.AnyJobValue);
                anyjob = "True";
            }
            else if (selectedelement.JobAndDescription == "ANYJOB/USERNUM")
            {
                jobNr = 255;
                jobNrString = "Anyjob";
                ObservableCollection<KeyValuePair<string, int>> tempObsCollection = new ObservableCollection<KeyValuePair<string, int>>();
                foreach (var item in selectedelement.AnyJobUserNumValue.Values)
                    tempObsCollection.Add(item[0]);
                description = GetAnyJobDescription(tempObsCollection);
                anyjob = "True";
                usernum = "True";
            }
            else
            {
                string jobanddescr = selectedelement.JobAndDescription;
                Regex getJob = new Regex(@"(?<=Job\s+)\d+", RegexOptions.IgnoreCase);
                jobNr = int.Parse(getJob.Match(jobanddescr).ToString());
                jobNrString = jobNr.ToString();
                description = new Regex(@"(?<=\d:+\s+).*", RegexOptions.IgnoreCase).Match(selectedelement.JobAndDescription).ToString();
                usernum = "False";
            }
            
            string result = "";
            if (GlobalData.ControllerType == "KRC2 V8")
            {
                result = String.Join(Environment.NewLine,
                    ";-------------------------Job" + jobNr + "-------------------------",
                    "WAIT FOR $IN[I_Enable_job[" + jobNr + "]] OR $IN[I_CANCLE_PROG_PLC]",
                    "CONTINUE",
                    "IF $IN[I_CANCLE_PROG_PLC] THEN",
                    "RETURN",
                    "ENDIF",
                    "","");

                if (isTypNmuReq)
                    result += String.Join(Environment.NewLine,
                    ";-------------------------GETTYPNUM--------------------",
                    ";FOLD PLC_COM.Request TypNum=TRUE, UserNum=" + usernum + ", JobNr=" + jobNr + ", DESC=" + description + ";%{PE}%MKUKATPUSER",
                    "PLC_ReqNum(TRUE, " + usernum + ", " + jobNr + ")",
                    ";ENDFOLD",
                    "",
                    "MyTypNum = PLC_GETTYPNUM ()",
                    "SWITCH MyTypNum",
                    "",
                    "");
            }
            else if (GlobalData.ControllerType == "KRC4")
            {
                description = description.Trim();
                string typNumReq = "", userNumReq = "", typNumReqBool = "False";
                if (usernum == "True")
                    userNumReq = "UserNumReq ";
                //if (selectedelement.TypNumReq)
                if (isTypNmuReq)
                {
                    result = String.Join(Environment.NewLine,
                        ";***********************************************",
                        ";* "+ (SrcValidator.language == "DE" ? "TYP Auswahl" : "Type selection"), "");
                    typNumReq = "TypNumReq ";
                    typNumReqBool = "True";
                }
                else
                {
                    result = String.Join(Environment.NewLine,
                        ";***********************************************","");
                }


                if (anyjob == "True")
                    result += ";* WAIT FOR ANYJOB " + description + "\r\n";
                else
                    result += ";* JOB " + jobNr + " (" + description+ ")\r\n";
                result += String.Join(Environment.NewLine,
                    ";***********************************************","");
                    result += ";FOLD Job Request JobNum:" + jobNrString + " "+typNumReq+userNumReq+"Abort:Home" + GetCorrectHomeIndex(iteration,data) + " Desc:" + description+"\r\n";
                    result += String.Join(Environment.NewLine,
                    ";FOLD ;%{h}",
                    ";Params IlfProvider=job; Plc_JobCmd=Request; Plc_JobNum=" + jobNr + "; Plc_JobTypeNumReq="+typNumReqBool+"; Plc_JobUserNumReq=" + usernum + "; Plc_JobAbort=" + GetCorrectHomeIndex(iteration,data) + "; Plc_JobCont=_; Plc_JobDesc=" + description,
                    ";ENDFOLD","");
                //if (selectedelement.Abort == "Home")
                    result += "Plc_JobReq ("+jobNr+",1,"+typNumReqBool+","+usernum+","+GetCorrectHomeIndex(iteration,data)+",0,False)\r\n";
                //else
                //    result += "Plc_JobReq (" + jobNr + ",2,"+typNumReqBool+"," + usernum + ",0," + selectedelement.AbortNr + ",False)\r\n";

                result += String.Join(Environment.NewLine,
                    ";ENDFOLD",
                    "", "");
                if (isTypNmuReq)
                {
                    result += String.Join(Environment.NewLine,
                    "myTypNum = PLC_GETTYPNUM ( )",
                    "SWITCH myTypNum",
                    ";***********************************************","");
                }
            }

            return result;
        }


        private static string GetAnyJobDescription(ObservableCollection<KeyValuePair<string, int>> anyJobValue)
        {
            //"Job " + GlobalData.SrcPathsAndJobs[value].ToString() + ": " + GlobalData.Jobs[GlobalData.SrcPathsAndJobs[value]].ToString()
            string result = "";
            foreach (var job in anyJobValue)
            {
                result += GlobalData.Jobs[GlobalData.SrcPathsAndJobs[job.Key]].ToString().TrimEnd() + " \\ ";
            }
            result = result.Substring(0,result.Length - 3);
            return result;
        }

        private static string GetTypesKRC2_V8(Dictionary<int, ICollection<IOrgsElement>> data, int i, List<int> typesList, string line)
        {
            bool isTypNumReq = GetTypNumReq(data, i);
            List<string> types = ConfigurationManager.AppSettings["Line_" + line].Split(',').Select(s => s.Trim()).ToList();
            string result = "";
            Regex getJob = new Regex(@"(?<=Job\s+)\d+", RegexOptions.IgnoreCase);
            int jobNr = 0;
            string description = "";
            string usernum = "";
            for (int typeNr = 1; typeNr <= types.Count; typeNr ++)
            {
                bool firstTime = true;
                if (data.Keys.Contains(typeNr))
                {
                    ICollection<IOrgsElement> currentType = data[typeNr];
                    foreach (var operation in currentType.Where(x => x.OrgsElement.Id == 1))
                    {
                        if (operation.OrgsElement.JobAndDescription == "USERNUM")
                        {
                            jobNr = operation.OrgsElement.UserNumValue[0].Value;
                            description = GlobalData.Jobs[jobNr];
                        }
                        else
                        {
                            string jobanddescr = operation.OrgsElement.JobAndDescription;
                            jobNr = int.Parse(getJob.Match(jobanddescr).ToString());
                            description = new Regex(@"(?<=\d:+\s+).*", RegexOptions.IgnoreCase).Match(operation.OrgsElement.JobAndDescription).ToString();
                        }

                        if (isTypNumReq)
                        {
                            result += String.Join(Environment.NewLine, ";######################### " + GetTypeName(typeNr, line) + " #####################################",
                            "CASE " + typeNr,
                            "", "");
                        }
                        else
                        {
                            result += String.Join(Environment.NewLine, "", "");
                        }
                    }
                    for (int j = i; j <= currentType.Count; j++)
                    {
                        foreach (var operation in currentType.Where(x => x.OrgsElement.Id == j))
                        {
                            if (j == 1 || firstTime)
                            {
                                firstTime = false;
                                if (operation.OrgsElement.JobAndDescription == "USERNUM")
                                {
                                    result += "SWITCH PLC_GETUSERNUM ( )\r\n";
                                    int counter = 1;
                                    foreach (var path in operation.OrgsElement.UserNumValue)
                                    {
                                        result += "CASE " + counter + "\r\n" + path.Key + "() ; "+ (SrcValidator.language == "DE" ? "Programmaufruf" : "Program call")+"\r\n";
                                        counter++;
                                    }
                                    result += "DEFAULT\r\nWAIT FOR FALSE\r\nENDSWITCH\r\n\r\n";
                                }
                                else
                                    result += operation.OrgsElement.Path + "() ; " + (SrcValidator.language == "DE" ? "Programmaufruf" : "Program call") + "\r\n\r\n";
                            }
                            else
                            {
                                if (operation.OrgsElement.JobAndDescription == "USERNUM")
                                {
                                    int counter = 1;
                                    jobNr = operation.OrgsElement.UserNumValue[0].Value;
                                    description = GlobalData.Jobs[jobNr];
                                    usernum = "TRUE";
                                    result += String.Join(Environment.NewLine,
                                        ";-------------------------Job" + jobNr + "-------------------------",
                                        "WAIT FOR $IN[I_Enable_job[" + jobNr + "]] OR $IN[I_CANCLE_PROG_PLC]",
                                        "CONTINUE",
                                        "IF $IN[I_CANCLE_PROG_PLC] THEN",
                                        "  " + GetAbortLine(operation, false) + "  RETURN",
                                        "ENDIF",
                                        "",
                                        ";-------------------------GETJOBNUM--------------------",
                                        ";FOLD PLC_COM.Request TypNum=FALSE, UserNum="+usernum+", JobNr=" + jobNr + ", DESC=" + description + ";%{PE}%MKUKATPUSER",
                                        "PLC_ReqNum(FALSE, "+usernum+", " + jobNr + ")",
                                        ";ENDFOLD",
                                        "",
                                        "SWITCH PLC_GETUSERNUM ( )",
                                        "");
                                    foreach (var path in operation.OrgsElement.UserNumValue)
                                    {
                                        result += "CASE " + counter + "\r\n" + path.Key + "() ; " + (SrcValidator.language == "DE" ? "Programmaufruf" : "Program call") + "\r\n";
                                        counter++;
                                    }
                                    result += "DEFAULT\r\nWAIT FOR FALSE()\r\nENDSWITCH\r\n\r\n";
                                }
                                else
                                {
                                    jobNr = int.Parse(getJob.Match(operation.OrgsElement.JobAndDescription).ToString());
                                    description = new Regex(@"(?<=\d:+\s+).*", RegexOptions.IgnoreCase).Match(operation.OrgsElement.JobAndDescription).ToString();
                                    result += String.Join(Environment.NewLine,
                                        ";-------------------------Job" + jobNr + "-------------------------",
                                        "WAIT FOR $IN[I_Enable_job[" + jobNr + "]] OR $IN[I_CANCLE_PROG_PLC]",
                                        "CONTINUE",
                                        "IF $IN[I_CANCLE_PROG_PLC] THEN",
                                        "  " + GetAbortLine(operation, false) + "  RETURN",
                                        "ENDIF",
                                        "",
                                        ";-------------------------GETJOBNUM--------------------",
                                        ";FOLD PLC_COM.Request TypNum=FALSE, UserNum=FALSE, JobNr=" + jobNr + ", DESC=" + description + ";%{PE}%MKUKATPUSER",
                                        "PLC_ReqNum(FALSE, FALSE, " + jobNr + ")",
                                        ";ENDFOLD",
                                        "",
                                        operation.OrgsElement.Path + "() ; " + (SrcValidator.language == "DE" ? "Programmaufruf" : "Program call"),
                                        "", "");
                                }
                            }
                        }
                    }
                }
                else
                {
                    string typnotused = SrcValidator.language == "DE" ? "Typ nicht verwendet" : "Type not used";
                    result += String.Join(Environment.NewLine,
                        ";######################### " + GetTypeName(typeNr, line) + " #####################################",
                        "CASE " + typeNr,
                        "; "+typnotused, "WAIT FOR FALSE", "", "");
                }
            }
            string error = SrcValidator.language == "DE" ? "FEHLER" : "ERROR";

            if (isTypNumReq)
                result += String.Join(Environment.NewLine, ";--------------"+error+"---------------","DEFAULT","WAIT FOR FALSE","ENDSWITCH","END");
            else
                result += String.Join(Environment.NewLine, "", "END");
            return result;
        }

        private static string GetTypesKRC4(Dictionary<int, ICollection<IOrgsElement>> data, int i, List<int> typesList, string line)
        {
            bool isTypNumReq = GetTypNumReq(data,i);

            Regex getJob = new Regex(@"(?<=Job\s+)\d+", RegexOptions.IgnoreCase);
            data = RemoveOpFromData(data, i);
            List<string> types = ConfigurationManager.AppSettings["Line_" + line].Split(',').Select(s => s.Trim()).ToList();
            string result = "";
            for (int typeNr = 1; typeNr <= types.Count; typeNr++)
            {
                bool firstTime = true;
                if (data.Keys.Contains(typeNr))
                {
                    ICollection<IOrgsElement> currentType = data[typeNr];
                    if (isTypNumReq)
                    {
                        result += String.Join(Environment.NewLine,
                        ";***********************************************",
                        ";**************    Start    ********************",
                        ";************** " + GetTypeName(typeNr, line),
                        ";***********************************************",
                        "CASE " + typeNr,
                        "");
                    }
                    //foreach (var operation in currentType.Where(x => x.OrgsElement.Id == 1))
                    foreach (var operation in currentType)
                    {
                        string typNumReq = "", userNumReq = "", typNumReqBool = "False", userNumReqBool = "False";
                        if (operation.OrgsElement.TypNumReq)
                        {
                            typNumReq = "TypNumReq ";
                            typNumReqBool = "True";
                        }
                        if (operation.OrgsElement.JobAndDescription == "ANYJOB" || operation.OrgsElement.JobAndDescription == "ANYJOB/USERNUM")
                        {
                            int function = 0;
                            if (operation.OrgsElement.JobAndDescription == "ANYJOB")
                            {
                                function = 1;
                            }
                            else if (operation.OrgsElement.JobAndDescription == "ANYJOB/USERNUM")
                            {
                                function = 2;
                                userNumReq = "UserNumReq ";
                                userNumReqBool = "True";
                            }

                            ObservableCollection<KeyValuePair<string, int>> currentComment = new ObservableCollection<KeyValuePair<string, int>>();
                            foreach (var job in operation.OrgsElement.AnyJobValue)
                                currentComment.Add(job);

                            if (!firstTime)
                            {
                                result += ";***********************************************\r\n;* WAIT FOR ANYJOB " + GetAnyJobDescription(currentComment).Trim() + "\r\n;***********************************************\r\n";
                                result += ";FOLD Job Request JobNum:Anyjob " + typNumReq + userNumReq + "Abort:" + operation.OrgsElement.Abort + operation.OrgsElement.AbortNr + " Desc:" + GetAnyJobDescription(currentComment) + "\r\n";
                                result += String.Join(Environment.NewLine,
                                ";FOLD ;%{h}",
                                ";Params IlfProvider=job; Plc_JobCmd=Request; Plc_JobNum=255; Plc_JobTypeNumReq=" + typNumReqBool + "; Plc_JobUserNumReq=" + userNumReqBool + "; Plc_JobAbort=" + (operation.OrgsElement.Abort == "Home" ? "" : "P") + operation.OrgsElement.AbortNr + "; Plc_JobCont=_; Plc_JobDesc=" + GetAnyJobDescription(currentComment).Trim(),
                                ";ENDFOLD", "");
                                if (operation.OrgsElement.Abort == "Home")
                                    result += "Plc_JobReq (255,1," + typNumReqBool + "," + userNumReqBool + "," + operation.OrgsElement.AbortNr + ",0,False)\r\n";
                                else
                                    result += "Plc_JobReq (255,2," + typNumReqBool + "," + userNumReqBool + ",0," + operation.OrgsElement.AbortNr + ",False)\r\n";

                                result += String.Join(Environment.NewLine,
                                    ";ENDFOLD",
                                    "");
                            }
                            firstTime = false;
                            result += String.Join(Environment.NewLine,
                                ";***********************************************",
                                ";* SWITCH ANYJOB " + GetAnyJobDescription(currentComment),
                                ";***********************************************",
                                "SWITCH PLC_GetJobNum ( )",
                                "", "");

                            if (function == 1)
                            {
                                foreach (var job in operation.OrgsElement.AnyJobValue)
                                {
                                    result += String.Join(Environment.NewLine,
                                        ";***********************************************",
                                        ";* JOB " + job.Value + " (" + GlobalData.Jobs[job.Value].Trim() + ")",
                                        ";***********************************************",
                                        "CASE " + job.Value,
                                        job.Key + "  () ; " + (SrcValidator.language == "DE" ? "Programmaufruf" : "Program call"),
                                        "");
                                }
                            }
                            else if (function == 2)
                            {
                                foreach (var job in operation.OrgsElement.AnyJobUserNumValue)
                                {
                                    int counter = 1;
                                    result += String.Join(Environment.NewLine,
                                        ";***********************************************",
                                        ";* JOB " + job.Key + " (" + GlobalData.Jobs[job.Key].Trim() + ")",
                                        ";***********************************************",
                                        "CASE " + job.Key,
                                        "",
                                        "myUserNum = PLC_GetUserNum ()",
                                        "SWITCH myUserNum",
                                        "", "");
                                    foreach (var userNumPath in job.Value)
                                    {
                                        result += String.Join(Environment.NewLine,
                                            "CASE " + counter,
                                            userNumPath.Key + "  () ; " + (SrcValidator.language == "DE" ? "Programmaufruf" : "Program call"),
                                            "",
                                            "");
                                        counter++;
                                    }
                                    result += String.Join(Environment.NewLine,
                                        "DEFAULT", "PLC_DefaultError()", "ENDSWITCH", "", "");
                                }
                            }
                            result += String.Join(Environment.NewLine,
                                "DEFAULT", "PLC_DefaultError()", "ENDSWITCH", "", "");

                        }
                        else if (operation.OrgsElement.JobAndDescription == "USERNUM")
                        {
                            userNumReq = "UserNumReq";
                            userNumReqBool = "True";

                            if (!firstTime)
                            {
                                result += ";***********************************************\r\n;* WAIT FOR JOB " + operation.OrgsElement.UserNumValue[0].Value + " (" + GlobalData.Jobs[operation.OrgsElement.UserNumValue[0].Value].Trim() + ")\r\n;***********************************************\r\n";
                                result += ";FOLD Job Request JobNum:" + operation.OrgsElement.UserNumValue[0].Value + " " + typNumReq + userNumReq + " Abort:" + operation.OrgsElement.Abort + operation.OrgsElement.AbortNr + " Desc:" + GlobalData.Jobs[operation.OrgsElement.UserNumValue[0].Value];
                                result += String.Join(Environment.NewLine,
                                ";FOLD ;%{h}",
                                ";Params IlfProvider=job; Plc_JobCmd=Request; Plc_JobNum=" + operation.OrgsElement.UserNumValue[0].Value + "; Plc_JobTypeNumReq=" + typNumReqBool + "; Plc_JobUserNumReq=" + userNumReqBool + "; Plc_JobAbort=" + (operation.OrgsElement.Abort == "Home" ? "" : "P") + operation.OrgsElement.AbortNr + "; Plc_JobCont=_; Plc_JobDesc=" + GlobalData.Jobs[operation.OrgsElement.UserNumValue[0].Value].Trim(),
                                ";ENDFOLD","");
                                if (operation.OrgsElement.Abort == "Home")
                                    result += "Plc_JobReq (" + operation.OrgsElement.UserNumValue[0].Value + ",1," + typNumReqBool + "," + userNumReqBool + "," + operation.OrgsElement.AbortNr + ",0,False)\r\n";
                                else
                                    result += "Plc_JobReq (" + operation.OrgsElement.UserNumValue[0].Value + ",2," + typNumReqBool + "," + userNumReqBool + ",0," + operation.OrgsElement.AbortNr + ",False)\r\n";

                                result += String.Join(Environment.NewLine,
                                    ";ENDFOLD",
                                    "");
                            }
                            firstTime = false;
                            result += String.Join(Environment.NewLine,
                                ";***********************************************",
                                ";* SWITCH USERNUM " + operation.OrgsElement.UserNumValue[0].Value + " (" + GlobalData.Jobs[operation.OrgsElement.UserNumValue[0].Value].Trim() + ")",
                                ";***********************************************",
                                "myUserNum = PLC_GetUserNum ( )",
                                "SWITCH myUserNum",
                                "", "");
                            int counter = 1;
                            foreach (var job in operation.OrgsElement.UserNumValue)
                            {
                                result += String.Join(Environment.NewLine,
                                    "CASE " + counter,
                                    job.Key + "  () ; " + (SrcValidator.language == "DE" ? "Programmaufruf" : "Program call"),
                                    "", "");
                                counter++;
                            }
                            result += String.Join(Environment.NewLine,
                                "DEFAULT", "PLC_DefaultError()", "ENDSWITCH", "", "");
                        }
                        else
                        {
                            int jobNr = int.Parse(getJob.Match(operation.OrgsElement.JobAndDescription).ToString());
                            if (!firstTime)
                            {

                                result += ";***********************************************\r\n;* WAIT FOR JOB " + jobNr + " (" + GlobalData.Jobs[jobNr].Trim() + ")\r\n;***********************************************\r\n";
                                result += ";FOLD Job Request JobNum:" + jobNr + " " + typNumReq + userNumReq + "Abort:" + operation.OrgsElement.Abort + operation.OrgsElement.AbortNr + " Desc:" + GlobalData.Jobs[jobNr].Trim() + "\r\n";
                                result += String.Join(Environment.NewLine,
                                ";FOLD ;%{h}",
                                ";Params IlfProvider=job; Plc_JobCmd=Request; Plc_JobNum=" + jobNr + "; Plc_JobTypeNumReq=" + typNumReqBool + "; Plc_JobUserNumReq=" + userNumReqBool + "; Plc_JobAbort=" + (operation.OrgsElement.Abort == "AbortProg" ? "P" : "") + operation.OrgsElement.AbortNr + "; Plc_JobCont=_; Plc_JobDesc=" + GlobalData.Jobs[jobNr].Trim(),
                                ";ENDFOLD", "");
                                if (operation.OrgsElement.Abort == "Home")
                                    result += "Plc_JobReq (" + jobNr + ",1," + typNumReqBool + "," + userNumReqBool + "," + operation.OrgsElement.AbortNr + ",0,False)\r\n";
                                else
                                    result += "Plc_JobReq (" + jobNr + ",2," + typNumReqBool + "," + userNumReqBool + ",0," + operation.OrgsElement.AbortNr + ",False)\r\n";
                                result += String.Join(Environment.NewLine,
                                    ";ENDFOLD",
                                    "");
                            }
                            firstTime = false;
                            result += String.Join(Environment.NewLine, "",
                                operation.OrgsElement.Path + "() ; " + (SrcValidator.language == "DE" ? "Programmaufruf" : "Program call"), "", "");
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["Line_" + line].Split(',').Select(s => s.Trim()).ToList()[typeNr-1]))
                    {
                        string typnotused = SrcValidator.language == "DE" ? "Typ nicht verwendet" : "Type not used";
                        result += String.Join(Environment.NewLine,
                        ";***********************************************",
                        ";************** " + GetTypeName(typeNr, line),
                        ";***********************************************",
                        "CASE " + typeNr,
                        "; "+typnotused, "WAIT FOR FALSE", "", "");
                    }
                }
                
            }
            if (isTypNumReq)
                result += String.Join(Environment.NewLine, "DEFAULT", "PLC_DefaultError()", "ENDSWITCH", "", "END");
            else
                result += String.Join(Environment.NewLine, "", "END");
            return result;
        }


        private static Dictionary<int, ICollection<IOrgsElement>> RemoveOpFromData(Dictionary<int, ICollection<IOrgsElement>> data, int i)
        {
            if (i == 1)
                return data;
            //Dictionary<int, ICollection<IOrgsElement>> cloneOfData = new Dictionary<int, ICollection<IOrgsElement>>();
            //foreach(var typ in data)
            //{
            //    cloneOfData.Add(typ.Key,new Collection<IOrgsElement>());
            //    foreach (var item in typ.Value)
            //    {
            //        cloneOfData[typ.Key].Add((OrgsElementVM)item.Clone());
            //    }

            //}
            Dictionary<int, ICollection<IOrgsElement>> result = new Dictionary<int, ICollection<IOrgsElement>>();
            foreach (var typ in data)
            {
                bool addToOrg = false;
                int counter = 1;
                foreach (var jobForOrg in typ.Value)
                {
                    if (counter == i)
                    {
                        addToOrg = true;
                        result.Add(typ.Key, new Collection<IOrgsElement>());
                    }
                    if (addToOrg)
                    {
                        result[typ.Key].Add(jobForOrg);
                    }
                    counter++;
                }
            }

            return result;
        }

        private static string GetProgramNameV8(int program)
        {
            string ablaufName = SrcValidator.language == "DE" ? "_ablauf_" : "_sequence_";
            string result = "";
            string number = program.ToString();
            while (number.Length < 3)
            {
                number = 0 + number;
            }

            if (GlobalData.ControllerType == "KRC2 V8")
            {

                if (number == "030")
                    result = "prog030_tipchange";
                else if (number == "031")
                    result = "prog031_tipdress";
                else if (number == "025")
                {
                    if (isRivet)
                        result = "prog025_MatrixCheckZN1";
                }
                else if (number == "032")
                    result = "prog032_maintenance";
                else if (number == "050")
                    result = "prog050_autopurge";
                else if (number == "061")
                    result = "prog061_masterreference";
                else if (number == "062")
                    result = "prog062_braketest";
                else
                    result = "prog" + number + ablaufName + program.ToString();
            }
            else if (GlobalData.ControllerType == "KRC4")
            {
                if (number == "050")
                {
                    if (isRivet)
                        result = "prog050_MatrixCheck";
                    else if (isArcWeld)
                        result = "Prog050_BrennerWechsel";
                    else
                        result = "Unknown";
                }
                else if (number == "051")
                    result = "Prog051_BrennerReinigung";
                else if (number == "052")
                    result = "Prog052_DrahtSchneiden";
                else if (number == "053")
                {
                    if (isKleben)
                        result = "Prog053_AutoPurge";
                    else if (isArcWeld)
                        result = "Prog053_Tcp_Vermessung";
                    else
                        result = "Unknown";
                }
                else if (number == "054")
                    result = "Prog054_DrahtWechsel";
                else if (number == "062")
                    result = "Prog062_TipDress";
                else if (number == "063")
                    result = "Prog063_TipChange";
                else if (number == "064")
                    result = "Prog064_Maintenance";
                else if (number == "250")
                    result = "Prog250_MasterReference";
                else if (number == "251")
                    result = "Prog251_BrakeTest";
                else
                    result = "Prog" + number + ablaufName + program.ToString();
            }

            return result;
        }
    }
}
