using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ABB_add_spaces.MirrorABBClasses;

namespace ABB_add_spaces
{
    class Methods
    {
        private static string resultString;
        private static Dictionary<char, string> map = new Dictionary<char, string>() {
              { 'ä', "ae" },
              { 'ö', "oe" },
              { 'ü', "ue" },
              { 'Ä', "Ae" },
              { 'Ö', "Oe" },
              { 'Ü', "Ue" },
              { 'ß', "ss" }
            };

        public static void FindComments(string[] files)
        {
            string message = "", dir = "";
            foreach (string file in files)
            {
                if (dir == "")
                    dir = Path.GetDirectoryName(file);
                StreamReader reader = new StreamReader(file);
                int linecounter = 0;
                List<Data> instructionsWithoutComments = new List<Data>();
                while (!reader.EndOfStream)
                {
                    linecounter++;
                    string line = reader.ReadLine();
                    if (!line.ToLower().Contains("proc ") && !line.ToLower().Contains("!* "))
                    {
                        if (line.ToLower().Contains("job") && !line.ToLower().Contains("clearall"))
                        {
                            if ((line.ToLower().Contains("started") || line.ToLower().Contains("done")) && !line.ToLower().Contains("text"))
                            {
                                instructionsWithoutComments.Add(new Data(linecounter, line, Path.GetFileName(file)));
                            }
                        }
                        if (line.ToLower().Contains("area") && !line.ToLower().Contains("clearall"))
                        {
                            if ((line.ToLower().Contains("areareq") || line.ToLower().Contains("arearel")) && !line.ToLower().Contains("text"))
                            {
                                instructionsWithoutComments.Add(new Data(linecounter, line, Path.GetFileName(file)));
                            }
                        }
                        if (line.ToLower().Contains("collzo") && !line.ToLower().Contains("clearall"))
                        {
                            if ((line.ToLower().Contains("collzoreq") || line.ToLower().Contains("collzorel")) && !line.ToLower().Contains("text"))
                            {
                                instructionsWithoutComments.Add(new Data(linecounter, line, Path.GetFileName(file)));
                            }
                        }
                    }
                }
                reader.Close();
                if (instructionsWithoutComments.Count > 0)
                {

                    //MessageBox.Show("Instructions without comments found. See log file for details and correct comments!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    foreach (var data in instructionsWithoutComments)
                    {
                        message += "No comment found in file " + data.File + ", line number: " + data.Line + ", line content: " + data.Content + "\r\n";
                    }
                }
            }

            if (message != "")
            {
                string filestring = "";
                if (File.Exists(dir + "\\log.txt"))
                {
                    if (!File.Exists(dir + "\\log_1.txt"))
                    {
                        filestring = "\\log_1.txt";
                    }
                    else
                    {
                        int i = 1;
                        while (true)
                        {
                            if (!File.Exists(dir + "\\log_" + i + ".txt"))
                            {
                                filestring = "\\log_" + i + ".txt";
                                break;
                            }
                            i++;
                        }
                    }
                }
                else
                {
                    filestring = "\\log.txt";
                }

                File.WriteAllText(dir + filestring, message);
                MessageBox.Show("Instructions without comments found. See log file for details and correct comments!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                System.Diagnostics.Process.Start(dir + filestring);
            }
        }

        public static bool TranslateOrgsAndAddComments(string[] files)
        {
            resultString = "";
            bool isSuccess = false;
            if (files.Length > 0)
            {
                isSuccess = true;
                foreach (string file in files)
                {
                    string fileWithCorrectLanguage = TranslateHeader(file);
                    if (fileWithCorrectLanguage != "")
                    {
                        fileWithCorrectLanguage = AddComments(fileWithCorrectLanguage);
                        if (Path.GetFileName(file).ToLower().Contains("a04_swp_user"))
                        {
                            fileWithCorrectLanguage = RemoveComments(fileWithCorrectLanguage);
                            fileWithCorrectLanguage = AddTipChangeComments(fileWithCorrectLanguage);
                        }
                        WriteFiles(file, fileWithCorrectLanguage);
                        if (Path.GetFileName(file).ToLower().Contains("a04_swp_user"))
                        {
                            AddSpaces(file, false, true);
                        }
                    }
                }
            }
            return isSuccess;
        }

        private static string RemoveComments(string fileWithCorrectLanguage)
        {
            string result = "";
            string header = "";
            bool addHeader = false;
            bool istipchangeordress = false;
            StringReader reader = new StringReader(fileWithCorrectLanguage);
            string proc = "";
            while (true)
            {
                string line = reader.ReadLine();
                if (line == null)
                    break;
                if (line.ToLower().Contains("proc swp_tipdress") || line.ToLower().Contains("proc swp_tipchange") || line.ToLower().Contains("proc swp_undockdress") || line.ToLower().Contains("proc swp_dockdress"))
                {
                    Regex regex = new Regex(@"(?<=PROC\s+)[a-zA-Z0-9_]*", RegexOptions.IgnoreCase);
                    proc = regex.Match(line).ToString();
                    istipchangeordress = true;
                    addHeader = true;
                }
                if (line.Length > 1)
                {
                    if (istipchangeordress && line.Trim().Substring(0, 1) == "!")
                    { }
                    else
                    {
                        if (addHeader)
                        {
                            result += line + GetHeader(proc, "A04_swp_User") + "\r\n";
                            addHeader = false;
                        }
                        else
                            result += line + "\r\n";
                    }
                }
                else
                    result += line + "\r\n";
                if (istipchangeordress && line.ToLower().Contains("endproc"))
                    istipchangeordress = false;
            }
            return result;
        }

        private static string AddTipChangeComments(string fileWithCorrectLanguage)
        {
            bool addEnter = true;
            bool istipchangeordress = false;
            bool motionStarted = false;
            bool emptyLine = false;
            string result = "";
            StringReader reader = new StringReader(fileWithCorrectLanguage);
            while (true)
            {
                string line = reader.ReadLine();
                if (line == null)
                    break;
                if (string.IsNullOrEmpty(line))
                    emptyLine = true;
                if (line.Contains("!*") && !(line.Contains("!* ") || line.Contains("!***") || line.Contains("!** Unlock")))
                    emptyLine = true;
                if (line.ToLower().Contains("proc swp_tipdress") || line.ToLower().Contains("proc swp_tipchange") || line.ToLower().Contains("proc swp_undockdress") || line.ToLower().Contains("proc swp_dockdress"))
                    istipchangeordress = true;

                if (istipchangeordress)
                {
                    if (line.ToLower().Contains("checkrobinhome"))
                        line = "\r\n!** Check robot is in Home 1\r\n" + line;
                    if (line.ToLower().Contains("job") && line.ToLower().Contains("started"))
                        line = "\r\n!** Set job started\r\n" + line;
                    if (line.ToLower().Contains("movegun"))
                    {
                        Regex regex = new Regex(@"(?<=,\s*)\d+", RegexOptions.IgnoreCase);
                        string gunOpening = regex.Match(line).ToString();
                        line = "\r\n!** Set gun opening to " + gunOpening + "mm\r\n" + line;
                    }
                    if ((line.ToLower().Contains("movejg") || line.ToLower().Contains("movelg")) && !motionStarted)
                    {
                        motionStarted = true;
                        line = "\r\n!** Move robot to dress station\r\n" + line;
                    }
                    if (line.ToLower().Contains("swp_do_vb_fsapplact") && line.ToLower().Contains("1"))
                        line = "\r\n!** Avoid air supply disconnection\r\n" + line;
                    if (line.ToLower().Contains("swp_tipdress "))
                    {
                        line = "\r\n!** Dress tips\r\n" + line + "\r\n\r\n!** Move to Reference Position\r\n";
                        addEnter = false;
                    }
                    if (line.ToLower().Contains("swp_gunreference "))
                        line = "\r\n!** Gun reference drive\r\n" + line;
                    if (line.ToLower().Contains("swp_measureresistance "))
                        line = "\r\n!** Resistor Measurement for gun\r\n" + line;
                    if (line.ToLower().Contains("swp_tipchange") && line.ToLower().Contains("motor_left"))
                        line = "\r\n!** Open/Clean dress station\r\n" + line;
                    if (line.ToLower().Contains("swp_do_vb_fsapplact") && line.ToLower().Contains("0"))
                        line = "\r\n!** Allow air supply disconnection\r\n" + line;
                    if (line.ToLower().Contains("job") && line.ToLower().Contains("done"))
                        line = "\r\n!** Set Job done to PLC\r\n" + line;
                    if (line.ToLower().Contains("jt_r1_home1") && motionStarted)
                        line = "\r\n!** Move robot to home\r\n" + line;
                    if (line.ToLower().Contains("swp_tipchange") && line.ToLower().Contains("wateroff"))
                        line = "\r\n!** Disable cooling water\r\n" + line;
                    if (line.ToLower().Contains("swp_tipchange") && line.ToLower().Contains("wateron"))
                        line = "\r\n!** Activate cooling water\r\n" + line;
                    if (line.ToLower().Contains("motionsup") && line.ToLower().Contains("on"))
                        line = "\r\n!** Set motion supervision value\r\n" + line;
                    if (line.ToLower().Contains("swp_tipchange") && line.ToLower().Contains("motor_right"))
                        line = "\r\n!** Clamp tip / vacuum on\r\n" + line;
                    if (line.ToLower().Contains("swp_tipchange") && line.ToLower().Contains("tip_check_low"))
                        line = "\r\n!** Check tip removed\r\n" + line;
                    if (line.ToLower().Contains("swp_tippress") && line.ToLower().Contains("800"))
                        line = "\r\n!** Press on new tip\r\n" + line;
                    if (line.ToLower().Contains("swp_tipchange") && line.ToLower().Contains("tip_check_high"))
                        line = "\r\n!** Check tip fitted\r\n" + line;
                    if (line.ToLower().Contains("swp_tippress") && !line.ToLower().Contains("800"))
                        line = "\r\n!** Press station\r\n" + line;
                    if (line.ToLower().Contains("swp_tipchange") && line.ToLower().Contains("counterreset"))
                        line = "\r\n!** Reset counter\r\n" + line;
                    if (line.ToLower().Contains("endproc"))
                        line = "\r\n" + line;
                    if (line.ToLower().Contains("toolchg_check"))
                        line = "\r\n!** Check if correct tool is on the robot\r\n" + line;
                    if (line.ToLower().Contains("swp_mobtipdress") && line.ToLower().Contains("pp_check_low"))
                        line = "\r\n!** Check mobile dresser is unoccupied\r\n" + line;
                    if (line.ToLower().Contains("swp_mobtipdress") && line.ToLower().Contains("gripopen"))
                        line = "\r\n!** Open the gripper from station\r\n" + line;
                    if (line.ToLower().Contains("swp_mobtipdress") && line.ToLower().Contains("gripclose"))
                        line = "\r\n!** Close the gripper\r\n" + line;
                    if (line.ToLower().Contains("swp_mobtipdress") && line.ToLower().Contains("boltopen"))
                        line = "\r\n!** Open the bolt cylinder\r\n" + line;
                    if (line.ToLower().Contains("swp_mobtipdress") && line.ToLower().Contains("pp_check_high"))
                        line = "\r\n!** Check mobile dresser is occupied\r\n" + line;
                    if (line.ToLower().Contains("swp_mobtipdress") && line.ToLower().Contains("boltclose"))
                        line = "\r\n!** Close the bolt from station\r\n" + line;
                    if (line.ToLower().Contains("gripload load0"))
                        line = "\r\n!** Deactivate load\r\n" + line;
                    if (line.ToLower().Contains("swp_dockdress_"))
                        line = "\r\n!** Dock mobile tip dresser\r\n" + line;
                    if (line.ToLower().Contains("swp_undockdress_"))
                        line = "\r\n!** Undock mobile tip dresser\r\n" + line;
                    if (line.ToLower().Contains("gripload") && !line.ToLower().Contains("load0"))
                        line = "\r\n!** Activate load\r\n" + line;
                }
                if (istipchangeordress && line.ToLower().Contains("endproc"))
                    istipchangeordress = false;
                if (!emptyLine && istipchangeordress || !istipchangeordress)
                {
                    if (addEnter)
                        result += line + "\r\n";
                    else
                        result += line;
                }
                addEnter = true;
                emptyLine = false;
            }

            return result;
        }

        public static bool AddSpacesToAll(string[] files, bool addHeader)
        {
            resultString = "";
            bool isSuccess = false;
            if (files.Length > 0)
            {
                isSuccess = true;
                foreach (string file in files)
                {
                    resultString = "";
                    string fileWithoutSpaces = RemoveSpaces(file);
                    AddSpaces(file, addHeader, false);
                    RemoveUselessData(file);
                    FixHomeSyntax(file);
                }
            }
            return isSuccess;
        }

        private static void WriteFiles(string file, string fileWithCorrectLanguage)
        {
            if (!Directory.Exists(Path.GetDirectoryName(file) + "\\Corrected Files"))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(file) + "\\Corrected Files");
            }
            if (File.Exists(Path.GetDirectoryName(file) + "\\Corrected Files\\" + Path.GetFileName(file)))
            {
                File.Delete(Path.GetDirectoryName(file) + "\\Corrected Files\\" + Path.GetFileName(file));
            }
            File.WriteAllText(Path.GetDirectoryName(file) + "\\Corrected Files\\" + Path.GetFileName(file), fileWithCorrectLanguage);


        }

        private static string AddComments(string fileWithCorrectLanguage)
        {
            string resultstring = "";
            StringReader reader = new StringReader(fileWithCorrectLanguage);
            while (true)
            {
                bool modifyLine = false;
                string modifiedLine = "";
                string line = reader.ReadLine();
                if (line.Contains("L?chen"))
                    line = line.Replace("L?chen", "Löschen");
                if (line.Contains("Nu?aum"))
                    line = line.Replace("Nu?aum", "Nußbaum");
                if (line.ToLower().Contains("grpinit"))
                {
                    modifyLine = true;
                    if (line.ToLower().Contains("text:="))
                        modifiedLine = line;
                    else
                    {
                        Regex regex = new Regex(@"(?<=GrpInit\s+)\d+", RegexOptions.IgnoreCase);
                        if (regex.Match(line).ToString() == "0")
                            modifiedLine = line.Replace(";", "\\Text:=\"Deactivate gripper\";");
                        else
                            modifiedLine = line.Replace(";", "\\Text:=\"Initialize gripper nr " + regex.Match(line).ToString() + "\";");
                    }
                }
                if (line.ToLower().Contains("plc_comm") && !line.ToLower().Contains("!") && !line.ToLower().Contains("const") && !line.ToLower().Contains("pers"))
                {
                    modifyLine = true;
                    if (line.ToLower().Contains("sendtypenum"))
                    {
                        if (line.ToLower().Contains("text:="))
                            modifiedLine = line;
                        else
                        {
                            Regex regex = new Regex(@"(?<=SendTypeNum\s*:=\s*)\d+", RegexOptions.IgnoreCase);
                            switch (int.Parse(regex.Match(line).ToString()))
                            {
                                case 1:
                                    modifiedLine = "PLC_Comm \\Text:=\"Sending type G20 to PLC\"\\SendTypeNum:=1;";
                                    break;
                                case 2:
                                    modifiedLine = "PLC_Comm \\Text:=\"Sending type G28 to PLC\"\\SendTypeNum:=2;";
                                    break;
                            }
                        }
                    }
                    else if (line.ToLower().Contains("sendusernum"))
                    {
                        if (line.ToLower().Contains("text:="))
                            modifiedLine = line;
                        else
                        {
                            Regex regex = new Regex(@"(?<=SendUserNum\s*:=\s*)\d+", RegexOptions.IgnoreCase);
                            modifiedLine = "PLC_Comm \\Text:=\"Sending user number " + regex.Match(line).ToString() + " to PLC\"\\SendUserNum:=" + regex.Match(line).ToString() + ";";
                        }
                    }
                    else if (line.ToLower().Contains("preoutp_") && !line.ToLower().Contains("waitdi") && !line.ToLower().Contains("postoutp_0"))
                    {
                        if (line.ToLower().Contains("text:="))
                            modifiedLine = line;
                        else
                        {
                            Regex regex1 = new Regex(@"(?<=PreOutp_\d\s*:=)[a-zA-Z0-9_]*", RegexOptions.IgnoreCase);
                            Regex regex4 = new Regex(@"(?<=PLC_Comm\s*).*", RegexOptions.IgnoreCase);
                            modifiedLine = "PLC_Comm \\Text:=\"Set " + regex1.Match(line).ToString() + "\"" + (regex4.Match(line).ToString().TrimStart());
                        }
                    }
                    else if (line.ToLower().Contains("preoutp_") && line.ToLower().Contains("waitdi") && !line.ToLower().Contains("postoutp_0"))
                    {
                        if (line.ToLower().Contains("text:="))
                            modifiedLine = line;
                        else
                        {
                            Regex regex1 = new Regex(@"(?<=PreOutp_\d\s*:=)[a-zA-Z0-9_]*", RegexOptions.IgnoreCase);
                            Regex regex2 = new Regex(@"(?<=WaitDI_1\s*:=)[a-zA-Z0-9_]*", RegexOptions.IgnoreCase);
                            Regex regex4 = new Regex(@"(?<=PLC_Comm\s*).*", RegexOptions.IgnoreCase);
                            modifiedLine = "PLC_Comm \\Text:=\"Set " + regex1.Match(line).ToString() + ", wait for " + regex2.Match(line).ToString() + "\"" + (regex4.Match(line).ToString().TrimStart());
                        }
                    }
                    else if (line.ToLower().Contains("preoutp_") && line.ToLower().Contains("waitdi") && line.ToLower().Contains("postoutp_0"))
                    {
                        if (line.ToLower().Contains("text"))
                            modifiedLine = line;
                        else
                        {
                            Regex regex1 = new Regex(@"(?<=PreOutp_\d\s*:=)[a-zA-Z0-9_]*", RegexOptions.IgnoreCase);
                            Regex regex2 = new Regex(@"(?<=WaitDI_1\s*:=)[a-zA-Z0-9_]*", RegexOptions.IgnoreCase);
                            Regex regex3 = new Regex(@"(?<=PostOutp_0\s*:=)[a-zA-Z0-9_]*", RegexOptions.IgnoreCase);
                            Regex regex4 = new Regex(@"(?<=PLC_Comm\s*).*", RegexOptions.IgnoreCase);
                            modifiedLine = "PLC_Comm \\Text:=\"Set " + regex1.Match(line).ToString() + ", wait for " + regex2.Match(line).ToString() + ", reset " + regex3.Match(line).ToString() + "\"" + (regex4.Match(line).ToString().TrimStart());
                        }
                    }
                    else if (!line.ToLower().Contains("preoutp_") && !line.ToLower().Contains("waitdi") && line.ToLower().Contains("postoutp_0"))
                    {
                        if (line.ToLower().Contains("text:="))
                            modifiedLine = line;
                        else
                        {
                            Regex regex3 = new Regex(@"(?<=PostOutp_0\s*:=)[a-zA-Z0-9_]*", RegexOptions.IgnoreCase);
                            Regex regex4 = new Regex(@"(?<=PLC_Comm\s*).*", RegexOptions.IgnoreCase);
                            modifiedLine = "PLC_Comm \\Text:=\"Reset " + regex3.Match(line).ToString() + "\"" + (regex4.Match(line).ToString().TrimStart());
                        }
                    }
                    else if (!line.ToLower().Contains("preoutp_") && line.ToLower().Contains("waitdi") && !line.ToLower().Contains("postoutp_0"))
                    {
                        if (line.ToLower().Contains("text:="))
                            modifiedLine = line;
                        else
                        {
                            Regex regex2 = new Regex(@"(?<=WaitDI_1\s*:=)[a-zA-Z0-9_]*", RegexOptions.IgnoreCase);
                            Regex regex4 = new Regex(@"(?<=PLC_Comm\s*).*", RegexOptions.IgnoreCase);
                            modifiedLine = "PLC_Comm \\Text:=\"Wait for " + regex2.Match(line).ToString() + "\"" + (regex4.Match(line).ToString().TrimStart());
                        }
                    }
                    else if (!line.ToLower().Contains("preoutp_") && line.ToLower().Contains("waitdi") && line.ToLower().Contains("postoutp_0"))
                    {

                        if (line.ToLower().Contains("text:="))
                            modifiedLine = line;
                        else
                        {
                            Regex regex2 = new Regex(@"(?<=WaitDI_1\s*:=)[a-zA-Z0-9_]*", RegexOptions.IgnoreCase);
                            Regex regex3 = new Regex(@"(?<=PostOutp_0\s*:=)[a-zA-Z0-9_]*", RegexOptions.IgnoreCase);
                            Regex regex4 = new Regex(@"(?<=PLC_Comm\s*).*", RegexOptions.IgnoreCase);
                            modifiedLine = "PLC_Comm \\Text:=\"Wait for " + regex2.Match(line).ToString() + ", Reset " + regex3.Match(line).ToString() + "\"" + (regex4.Match(line).ToString().TrimStart());
                        }
                    }

                }
                if (line.ToLower().Contains("grp ") && (line.ToLower().Contains("chk") || line.ToLower().Contains("pos") || line.ToLower().Contains("vacuum") || line.ToLower().Contains("grpl ") || line.ToLower().Contains("grpj ")))
                {

                    modifyLine = true;
                    if (line.ToLower().Contains("text:="))
                        modifiedLine = line;
                    else
                    {
                        string clamps = "";
                        string state = "";
                        if (line.ToLower().Contains("posadv"))
                            state = "set to advance";
                        else if (line.ToLower().Contains("posret"))
                            state = "set to retract";
                        else if (line.ToLower().Contains("chkret"))
                            state = "checked to retract";
                        else if (line.ToLower().Contains("chkadv"))
                            state = "checked to advance";
                        else if (line.ToLower().Contains("vacuumon"))
                            state = "set to Vacuum On";
                        else if (line.ToLower().Contains("vacuumoff"))
                            state = "set to Vacuum Off";
                        Regex regexFGs = new Regex(@"(FG|FC)[a-zA-Z0-9_]*");
                        int counter = 0;
                        foreach (Match match in regexFGs.Matches(line))
                        {
                            clamps += match.ToString() + ", ";
                            counter++;
                        }
                        clamps = clamps.Substring(0, clamps.Length - 2);
                        string text = "";
                        if (counter == 1)
                            text = "Valve " + clamps + " is " + state;
                        else
                            text = "Valves " + clamps + " are " + state;

                        if (text.Length > 80)
                        {
                            text = text.Replace("FG001_", "");
                            text = text.Replace("FG002_", "");
                            text = text.Replace("FC001_", "");
                            text = text.Replace("FC002_", "");
                        }
                        modifiedLine = line.Replace(";", "\\Text:=\"" + text + "\";");
                    }
                }

                if (line.ToLower().Contains("grppartchk") || line.ToLower().Contains("grplpartchk") || line.ToLower().Contains("grpjpartchk"))
                {
                    modifyLine = true;
                    if (line.ToLower().Contains("text:="))
                        modifiedLine = line;
                    else
                    {
                        Regex regexState = new Regex(@"(?<=(GrpPartChk\s+|GrpLPartChk\s+|GrpJPartChk\s+))[a-zA-Z0-9]*(?=,)", RegexOptions.IgnoreCase);
                        string state = regexState.Match(line).ToString();
                        string sensors = "";
                        Regex regexFGs = new Regex(@"(FG|FC)[a-zA-Z0-9_]*", RegexOptions.IgnoreCase);
                        int counter = 0;
                        foreach (Match match in regexFGs.Matches(line))
                        {
                            sensors += match.ToString() + ", ";
                            counter++;
                        }
                        sensors = sensors.Substring(0, sensors.Length - 2);
                        string text = "";
                        if (counter > 1)
                            text = "Sensors " + sensors + " are checked to " + state;
                        else
                            text = "Sensor " + sensors + " is checked to " + state;
                        if (text.Length > 79)
                        {
                            text = text.Replace("FG001_", "");
                            text = text.Replace("FG002_", "");
                            text = text.Replace("FC001_", "");
                            text = text.Replace("FC002_", "");
                        }
                        modifiedLine = line.Replace(";", "\\Text:=\"" + text + "\";");
                    }
                }
                if (line.ToLower().Contains("toolchg_cmd"))
                {
                    modifyLine = true;
                    if (line.ToLower().Contains("desc:="))
                        modifiedLine = line;
                    else
                    {
                        if (line.ToLower().Contains("opencover"))
                        {
                            Regex regex = new Regex(@"(?<=Station_)\d+", RegexOptions.IgnoreCase);
                            modifiedLine = line.Replace(";", "\\Desc:=\"Open cover on station " + regex.Match(line).ToString() + "\";");
                        }
                        else if (line.ToLower().Contains("closecover"))
                        {
                            Regex regex = new Regex(@"(?<=Station_)\d+", RegexOptions.IgnoreCase);
                            modifiedLine = line.Replace(";", "\\Desc:=\"Close cover on station " + regex.Match(line).ToString() + "\";");
                        }
                        else if (line.ToLower().Contains("statocc"))
                        {
                            Regex regex = new Regex(@"(?<=Station_)\d+", RegexOptions.IgnoreCase);
                            modifiedLine = line.Replace(";", "\\Desc:=\"Check if station " + regex.Match(line).ToString() + " is occupied\";");
                        }
                        else if (line.ToLower().Contains("statfree"))
                        {
                            Regex regex = new Regex(@"(?<=Station_)\d+", RegexOptions.IgnoreCase);
                            modifiedLine = line.Replace(";", "\\Desc:=\"Check if station " + regex.Match(line).ToString() + " is free\";");
                        }
                        else if (line.ToLower().Contains("lock") && !line.ToLower().Contains("unlock"))
                        {
                            Regex regex = new Regex(@"(?<=Station_)\d+", RegexOptions.IgnoreCase);
                            modifiedLine = line.Replace(";", "\\Desc:=\"Lock toolchanger in tool nr " + regex.Match(line).ToString() + "\";");
                        }
                        else if (line.ToLower().Contains("unlock"))
                        {
                            Regex regex = new Regex(@"(?<=Station_)\d+", RegexOptions.IgnoreCase);
                            modifiedLine = line.Replace(";", "\\Desc:=\"Unlock toolchanger in tool nr " + regex.Match(line).ToString() + "\";");
                        }
                        else if (line.ToLower().Contains("predock"))
                        {
                            modifiedLine = line.Replace(";", "\\Desc:=\"Unlock toolchanger in predock position\";");
                        }
                    }
                }
                if (line.ToLower().Contains("toolchg_check"))
                {
                    modifyLine = true;
                    if (line.ToLower().Contains("desc:="))
                        modifiedLine = line;
                    else
                    {
                        if (line.ToLower().Contains("tool_0"))
                        {
                            modifiedLine = "ToolChg_Check Tool_0\\Desc:=\"Empty toolchanger\";";
                        }
                        else if ((line.ToLower().Contains("tool_1")))
                        {
                            modifiedLine = "ToolChg_Check Tool_1\\Desc:=\"Gripper 1\";";
                        }
                        else if ((line.ToLower().Contains("tool_2")))
                        {
                            modifiedLine = "ToolChg_Check Tool_2\\Desc:=\"Gripper 2\";";
                        }
                    }
                }
                if (modifyLine)
                    resultstring += modifiedLine + "\r\n";
                else
                    resultstring += line + "\r\n";
                if (line.ToLower().Contains("endmodule"))
                    break;
            }

            reader.Close();
            return resultstring;
        }

        private static string TranslateHeader(string file)
        {

            List<RobotProgram> listOfPrograms = new List<RobotProgram>();
            RobotProgram currentProgram = new RobotProgram();
            string resultString = "";
            List<string> program = new List<string>();
            var reader = new StreamReader(file);
            bool isProgram = false, addLine = false, activerProc = false, firstProcFound = false;
            string currentString = "", programName = "", header = "", finalString = "";
            IDictionary<string, string> programOrder = new Dictionary<string, string>();
            while (!reader.EndOfStream)
            {
                bool helpBool = false;
                string line = reader.ReadLine();
                if (line.Contains("!scale "))
                { }
                if (!string.IsNullOrEmpty(line) && line.Trim() == "")
                    line = line.Trim();
                if (!firstProcFound)
                {
                    if (line.Length > 0 && (line.TrimStart().Substring(0, 1) == "!" && line.ToLower().Contains("proc ")))
                        helpBool = true;

                    if (!line.ToLower().Contains("proc ") && !line.ToLower().Contains("endproc") && !helpBool)
                    {
                        header += line + "\r\n";
                    }
                }
                if (line.ToLower().Contains("proc ") && !line.ToLower().Contains("endproc") && !helpBool)
                {
                    if (!activerProc && firstProcFound)
                        if (programOrder.Keys.Contains(programName))
                            MessageBox.Show("Multiple definitions of procedure " + programName + " found. Make sure only one definition of this procedure exist and run program again");
                        else
                            programOrder.Add(programName, currentString);
                    if (!firstProcFound)
                    {
                        programOrder.Add("Header", header);
                    }
                    firstProcFound = true;
                    activerProc = true;
                    addLine = true;
                    currentString = "";
                    Regex regex = new Regex(@"(?<=PROC\s+).*", RegexOptions.IgnoreCase);
                    if (line.Contains("!x"))
                        programName = "x" + regex.Match(line).ToString();
                    else
                        programName = regex.Match(line).ToString();
                }
                if (addLine)
                {
                    currentString += line + "\r\n";
                }
                if (line.ToLower().Contains("proc ") && !line.ToLower().Contains("endproc") && !helpBool)
                {
                    //if (!activerProc)
                    //{
                    //    addLine = false;
                    //    programOrder.Add(programName, currentString);
                    //}
                    activerProc = false;
                }
            }
            if (currentString.Length > 4 && currentString.Substring(0, 3) == "!x ")
                programName = "x" + programName;
            if (programName != "")
                programOrder.Add(programName, currentString);
            int linecounter = 1;
            reader.Close();
            reader = new StreamReader(file);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                //if (line.ToLower().Contains("proc program") && !line.ToLower().Contains("program_251"))
                if (line.ToLower().Contains("proc ") && !line.ToLower().Contains("endproc"))
                {
                    currentProgram = new RobotProgram();
                    Regex regex = new Regex(@"(?<=PROC\s+).*", RegexOptions.IgnoreCase);
                    currentProgram.Name = regex.Match(line).ToString();
                    currentProgram.StartLine = linecounter;
                    program = new List<string>();
                    isProgram = true;
                }
                if (isProgram && line != "")
                    program.Add(line);
                if (isProgram && line.ToLower().Contains("endproc"))
                {
                    //string currentString2 = Translate(program);
                    //resultString += currentString2 + "\r\n\r\n!***********************************************\r\n\r\n\r\n";
                    isProgram = false;
                    currentProgram.EndLine = linecounter;
                    currentProgram.Content = resultString;
                    listOfPrograms.Add(currentProgram);
                }
                linecounter++;
            }

            reader.Close();

            foreach (var item in programOrder.Where(x => x.Key.Substring(0, 1).Contains("x")))
                listOfPrograms.Add(new RobotProgram(item.Key, 0, 0, ""));


            IDictionary<string, string> copyOfProgramOrder = new Dictionary<string, string>(programOrder);
            foreach (var programAndContent in copyOfProgramOrder.Where(x => x.Key.ToLower().Contains("program_") && !x.Key.ToLower().Contains("xprogram_")))
            {
                List<string> lines = new List<string>();
                StringReader stringreader = new StringReader(programAndContent.Value);
                while (true)
                {
                    string line = stringreader.ReadLine();
                    if (line == null)
                        break;
                    lines.Add(line);
                }
                string input = Translate(lines);
                programOrder[programAndContent.Key] = input;
            }
            string result = "";
            foreach (string element in programOrder.Values)
                result += element;
            //string result = "";
            //reader = new StreamReader(file);
            //isProgram = false;
            //bool searchfornextproc = false;
            //while (!reader.EndOfStream)
            //{

            //    string line = reader.ReadLine();
            //    if (line.ToLower().Contains("proc "))
            //    {
            //        searchfornextproc = false;
            //    }

            //    if ((line.ToLower().Contains("proc program") || line.ToLower().Contains("endmodule")))
            //    {
            //        isProgram = true;
            //    }
            //    if (!isProgram && !searchfornextproc)
            //        result += line + "\r\n";
            //    if (isProgram && line.ToLower().Contains("endproc"))
            //    {
            //        searchfornextproc = true;
            //        isProgram = false;
            //    }
            //}
            //reader.Close();
            //result = result.TrimEnd();
            //result += "\r\n\r\n\r\n" +  resultString + "\r\n\r\nENDMODULE";

            return result;
        }

        private static string Translate(List<string> program)
        {
            string result = "";
            bool switchLine = false;
            string robot = "";
            int number = 0, home = 0;
            string lastCommentFound = "";

            List<string> trimmedList = new List<string>();
            foreach (string item in program)
            {
                if (item.ToLower().Contains("proc program"))
                {
                    Regex regex = new Regex(@"\d+");
                    number = int.Parse(regex.Match(item).ToString());
                }
                if (item.ToLower().Contains("checkrobinhome"))
                {
                    Regex regex = new Regex(@"(?<=Home)\d+");
                    home = int.Parse(regex.Match(item).ToString());
                }
                if (item.ToLower().Contains("roboter"))
                    robot = item.Replace("Roboter", "Robot  ");
                string tempstring = item.Trim();
                if (tempstring != "")
                    trimmedList.Add(tempstring);
            }
            List<string> templist = new List<string>();
            int counter = 0;
            string previousLine = "", previous2lines = "", next1Line = "", next2Lines = "", next3Lines = "";
            List<string> trimmedFilteredList = new List<string>();
            foreach (string item in trimmedList.Where(x => ((x.Substring(0, 1).Trim() != "!") | x.ToLower().Contains("wait for ") | x.ToLower().Contains("!* job ") | x.ToLower().Contains("programmablauf beendet"))))
            {
                trimmedFilteredList.Add(item);
            }
            foreach (string item in trimmedFilteredList)
            {
                if (counter < trimmedFilteredList.Count - 3)
                {
                    next1Line = trimmedFilteredList[counter + 1];
                    next2Lines = trimmedFilteredList[counter + 2];
                    next3Lines = trimmedFilteredList[counter + 3];
                }
                counter++;
                bool isReqTyp = false;
                bool addItem = true;
                string newstring = "";
                if (item.ToLower().Contains("wait for ") || item.ToLower().Contains("!* job "))
                {
                    Regex regex = new Regex(@"(?<=\(\s*).*(?=\s*\))", RegexOptions.IgnoreCase);
                    lastCommentFound = regex.Match(item).ToString();
                }
                bool addNewString = false;
                if (item.ToLower().Contains("job_request"))
                {
                    if (!item.ToLower().Contains("text"))
                    {
                        Regex regex = new Regex(@".*Job\d+", RegexOptions.IgnoreCase);
                        string tempstring = regex.Match(item).ToString();
                        regex = new Regex(@"Job\d+", RegexOptions.IgnoreCase);
                        string jobnumber = regex.Match(item).ToString();
                        regex = new Regex(@"(?<=" + jobnumber + ").*", RegexOptions.IgnoreCase);
                        newstring = tempstring + "\\Text:=\"" + lastCommentFound + "\"" + regex.Match(item).ToString();
                        addNewString = true;
                    }
                }

                if (item.ToLower().Contains("reqtypenum"))
                {
                    isReqTyp = true;
                    addItem = false;
                    //if (item.ToLower().Contains("anyjobnum"))
                    templist.Add("!* TYP Request");
                    //else
                    //    templist.Add("!* TYP Request;\r\n!***********************************************\r\n"+item);
                }
                if (item.ToLower().Contains("anyjobnum"))
                {
                    addItem = false;
                    string temp = item.Replace("Anfoderung", "Request");
                    temp = temp.Replace("od.", "or");
                    if (!isReqTyp)
                    {
                        Regex regex = new Regex(@"(?<=Text\s*:=\s*).*(?=\\)", RegexOptions.IgnoreCase);
                        string anyjobdescription = regex.Match(temp).ToString();
                        anyjobdescription = anyjobdescription.Replace("\"", "");
                        templist.Add("!* WAIT FOR ANYJOB " + anyjobdescription);

                    }
                    templist.Add(temp);
                }
                if (item.ToLower().Contains("programmaufruf"))
                {
                    addItem = false;
                    templist.Add(item.Replace("Programmaufruf", "Procedure call"));
                }
                if (item.ToLower().Contains("sps sendet"))
                {
                    addItem = false;
                    Regex regex = new Regex("(?<=.*)((Typ.*)|(User.*)|(Job.*))(?=\")", RegexOptions.IgnoreCase);
                    string test = regex.Match(item).ToString();
                    templist.Add("ProgramError \"PLC sent incorrect " + regex.Match(item).ToString() + "\";");
                }
                if (item.ToLower().Contains("programmablauf beendet"))
                {
                    addItem = false;
                    templist.Add("\r\n!***************** Program finished *********************\r\n");
                }
                if (item.ToLower().Contains("job\\clearall"))
                {
                    addItem = false;
                    templist.Add("Job\\ClearAll;\r\n");
                }
                if (addItem && !addNewString)
                {
                    if (item.ToLower().Contains("!* job "))
                        if (next1Line.ToLower().Contains("wait for job ") || next2Lines.ToLower().Contains("wait for job ") || next3Lines.ToLower().Contains("wait for job "))
                        { }
                        else
                            templist.Add("!***********************************************\r\n" + item + "\r\n!***********************************************");

                    else
                        templist.Add(item);
                }

                if (addNewString)
                    templist.Add(newstring);
                previous2lines = previousLine;
                previousLine = item;
            }
            List<string> tempList2 = new List<string>();
            counter = 0;
            foreach (string item in templist)
            {
                bool nextisTypreq = false;
                if (counter + 1 < templist.Count)
                    if (templist[counter + 1].Contains("TYP Req"))
                    {
                        nextisTypreq = true;
                    }
                if (item.ToLower().Contains("wait for "))
                {
                    string temp = "";
                    if (!nextisTypreq)
                        temp = "!***********************************************\r\n" + item + "\r\n!***********************************************";
                    else
                        temp = "!***********************************************\r\n" + item;
                    tempList2.Add(temp);
                }

                else
                {
                    tempList2.Add(item);
                }
                counter++;
            }
            List<string> list = new List<string>();
            foreach (string item in tempList2)
            {
                list.Add(item);
                if (item.ToLower().Contains("proc program"))
                {
                    string header = String.Join(Environment.NewLine,
                    "!***********************************************************",
                    "!* Company             : BMW ALB",
                    "!* Program             : " + number.ToString() + "",
                    "!* Created             : " + DateTime.Now,
                    robot,
                    "!* Created by          : SAS",
                    "!* Changes             : Translation to English by AIUT",
                    "!***********************************************************",
                    "");
                    if (home != 0)
                        header += "\r\n!* Robot should be started from HOME" + home.ToString();

                    list.Add(header);
                }
            }

            int testCounter = 0;
            List<string> finalList = new List<string>();
            int nrOfTypes = 0;
            bool isType = false;
            foreach (string item in list)
            {
                if (item.ToLower().Contains("cmn_gettypenum"))
                    isType = true;

                if (isType)
                {
                    if (item.ToLower().Contains("test "))
                        testCounter++;

                    if (item.ToLower().Contains("endtest"))
                        testCounter--;

                    if (item.ToLower().Contains("case") && testCounter == 1)
                    {
                        if (item.ToLower().Contains("case 1"))
                        {
                            nrOfTypes++;
                            string tempstring = item + "\r\n!***********************************************\r\n!**************    Start    ********************\r\n!************** G20 type\r\n!***********************************************";
                            finalList.Add(tempstring);
                        }
                        if (item.ToLower().Contains("case 2"))
                        {
                            nrOfTypes++;
                            string tempstring = item + "\r\n!***********************************************\r\n!**************    Start    ********************\r\n!************** G28 type\r\n!***********************************************";
                            finalList.Add(tempstring);
                        }
                    }

                    else
                    {
                        finalList.Add(item);
                    }
                }
                else
                {
                    finalList.Add(item);
                }

            }
            List<string> finalListWithCorrectedType = new List<string>();
            if (nrOfTypes == 1)
            {
                foreach (string item in finalList)
                {
                    if (item.Contains("G20 type"))
                        finalListWithCorrectedType.Add(item.Replace("G20", "G2x"));
                    else
                        finalListWithCorrectedType.Add(item);
                }
            }
            else
                finalListWithCorrectedType = finalList;
            counter = 0;
            bool typReqFound = false;
            foreach (string item in finalListWithCorrectedType)
            {
                if (item.ToLower().Contains("typ request"))
                {
                    typReqFound = true;
                    break;
                }
                counter++;
            }
            if (typReqFound)
            {
                string typ = finalListWithCorrectedType[counter];
                string job = finalListWithCorrectedType[counter - 1];
                Regex regexjob = new Regex(@"(?<=.*\r\n).*", RegexOptions.IgnoreCase);
                finalListWithCorrectedType[counter] = regexjob.Match(job).ToString() + "\r\n!***********************************************";
                finalListWithCorrectedType[counter - 1] = "!***********************************************\r\n" + typ;
            }
            counter = 0;
            string previousItem = "";
            foreach (string item in finalListWithCorrectedType)
            {
                string nextItem = "";
                if (item.ToLower().Contains("checkrobinhome") | item.ToLower().Contains("endtest") | item.ToLower().Contains("job_request") | item.ToLower().Contains("procedure call"))
                    result += item + "\r\n\r\n";
                //else if((item.ToLower().Contains("case") && !previousItem.ToLower().Contains("case")) || item.ToLower().Contains("default") || item.ToLower().Contains("endproc"))
                //    result += "\r\n" +item + "\r\n";
                else
                    result += item + "\r\n";
                previousItem = item;
            }

            result += "\r\n\r\n!***********************************************************\r\n\r\n\r\n";
            return result;
        }

        private static string RemoveSpaces(string file)
        {
            string resultString = "";
            var reader = new StreamReader(file,Encoding.Default,true);
            while (!reader.EndOfStream)
            {
                string line = (reader.ReadLine()).Aggregate(
                  new StringBuilder(),
                  (sb, c) => map.TryGetValue(c, out var r) ? sb.Append(r) : sb.Append(c)
                  ).ToString();
                if (line != "")
                    resultString += line + "\r\n";
            }
            reader.Close();
            return resultString;
        }

        private static void FixHomeSyntax(string file)
        {
            var line = "";
            string homenum = "";
            resultString = "";
            var reader = new StreamReader(file);
            while (!reader.EndOfStream)
            {
                homenum = "";
                line = reader.ReadLine();
                if (line.Contains("CheckRobInHome"))
                {
                    homenum = GetHomenum(line);
                    string lineToAdd = "CheckRobInHome\\Home:=Home" + homenum + "\\Text:=\"Robot in Home" + homenum + "\";";
                    resultString = resultString + lineToAdd + "\n";
                }
                else
                    resultString = resultString + line + "\n";
            }
            reader.Close();
            File.Delete(file);
            File.WriteAllText(file, resultString);
        }

        private static string GetHomenum(string line)
        {
            string resultString = "";
            Regex regName = new Regex(@"(?<=:\=Home).", RegexOptions.IgnoreCase);
            Match mResult = regName.Match(line);
            resultString = mResult.ToString();
            return resultString;
        }

        private static void RemoveUselessData(string writefile)
        {
            string previousLine = "";
            bool addCurrentLine = true;
            var line = "";
            bool addPreviuosLine = true;
            resultString = "";
            var reader = new StreamReader(writefile);
            while (!reader.EndOfStream)
            {
                previousLine = line;
                line = reader.ReadLine();
                if (line.Contains("SPEED DATA") | line.Contains("ZONE DATA") | line.Contains("CUSTOM DATA"))
                {
                    addPreviuosLine = false;
                    addCurrentLine = false;
                }
                else if (line == "")
                {
                    addPreviuosLine = true;
                }

                if (addPreviuosLine)
                {
                    if (!addCurrentLine)
                        addCurrentLine = true;
                    else
                    {

                        line = line + "\n";
                        if (!previousLine.Contains(" wobj0:="))
                            resultString = resultString + previousLine;
                    }
                }

            }
            resultString = resultString + line;
            reader.Close();
            File.Delete(writefile);
            File.WriteAllText(writefile, resultString);
        }

        private static void AddSpaces(string file, bool addHeader, bool a04)
        {
            string moduleName = "";
            bool firstLine = true;
            bool firstProc = true;
            bool addspace = true;
            string previousLine = "";
            string line = "";
            string procedureName = "";
            string header = "";
            bool existHeader = false;
            bool isMotionCommand = false, isPreviousMotionCommand = false;
            if (a04)
                file = Path.GetDirectoryName(file) + "\\Corrected Files\\" + Path.GetFileName(file);
            var reader = new StreamReader(file,Encoding.Default,true);
            while (!reader.EndOfStream)
            {

                previousLine = line;
                line = (reader.ReadLine()).Aggregate(
                  new StringBuilder(),
                  (sb, c) => map.TryGetValue(c, out var r) ? sb.Append(r) : sb.Append(c)
                  ).ToString();
                if (line.Contains("!** Move to Reference Position"))
                { }
                string lineWithoutSpaces = line.Trim();
                if (line.Contains("MODULE") & !line.Contains("ENDMODULE"))
                {
                    moduleName = GetModuleName(line);
                }
                if (lineWithoutSpaces.Contains("PROC ") & !lineWithoutSpaces.Contains("ENDPROC"))
                {
                    procedureName = GetProcedureName(lineWithoutSpaces);
                    if (addHeader)
                    {
                        existHeader = CheckNextLine(file, isHeader: true);
                        if (!existHeader)
                            header = GetHeader(procedureName, moduleName);
                    }
                    if (previousLine != "" | firstProc)
                        if (firstProc)
                            addspace = CheckNextLine(file, isEmpty: true);
                    if (addspace & !existHeader)
                        lineWithoutSpaces = lineWithoutSpaces + "\n";
                    firstProc = false;
                }


                if (lineWithoutSpaces.Contains("ENDPROC") & previousLine != "")
                    lineWithoutSpaces = lineWithoutSpaces + " !(" + procedureName + ")\n\n\n!*************************************************\n";

                if (line.Contains("MoveJ") | line.Contains("MoveL") | line.Contains("MoveJG") | line.Contains("MoveLG") | line.Contains("PROC"))
                    isMotionCommand = true;
                else
                    isMotionCommand = false;

                if (isMotionCommand & !isPreviousMotionCommand & previousLine != "")
                {
                    BuildResultString(lineWithoutSpaces, true, firstLine, header, existHeader);
                    firstLine = false;
                    header = "";
                }
                else
                {
                    BuildResultString(lineWithoutSpaces, false, firstLine, header, existHeader);
                    firstLine = false;
                    header = "";
                }


                isPreviousMotionCommand = isMotionCommand;
            }
            string writeFile = "";
            //if (!file.Contains(appendix))
            //   writeFile = file.TrimEnd().Substring(0, file.Length - 4) + appendix;
            //else
            //    writeFile = file;
            reader.Close();
            File.Delete(file);
            File.WriteAllText(file, resultString);
        }

        private static string GetModuleName(string line)
        {
            string result = "";
            Regex regName = new Regex(@"(?<=MODULE).*", RegexOptions.IgnoreCase);
            Match mResult = regName.Match(line);
            result = mResult.ToString();
            return result;
        }

        private static string GetHeader(string procedureName, string moduleName)
        {
            string header = "";
            DateTime currentTime = DateTime.Now;
            string procNameLine = "!* Program: " + procedureName;
            string moduleNameLine = "!* Location: " + moduleName;
            procNameLine = GetHeaderLine(procNameLine);
            moduleNameLine = GetHeaderLine(moduleNameLine);

            header = String.Join(
                    Environment.NewLine,
                    "",
                    "!***********************************************************",
                    procNameLine,
                    moduleNameLine,
                    "!***********************************************************",
                    "!*                   Update Section                        *",
                    "!*                  ================                       *",
                    "!* Date: " + currentTime.ToString() + "                               *",
                    "!* Engineer: AIUT Sp.z o.o.                                *",
                    "!* Changes:                                                *",
                    "!***********************************************************");
            return header;
        }

        private static string GetHeaderLine(string inputString)
        {
            string result = "";
            while (inputString.Length < 60)
            {
                if (inputString.Length < 59)
                    inputString = inputString + " ";
                else
                    inputString = inputString + "*";
            }
            result = inputString;

            return result;
        }

        private static string GetProcedureName(string lineWithoutSpaces)
        {
            string result = "";
            Regex regName = new Regex(@"(?<=PROC ).*", RegexOptions.IgnoreCase);
            Match mResult = regName.Match(lineWithoutSpaces);
            result = mResult.ToString();
            return result;
        }

        private static void BuildResultString(string line, bool addSpace, bool firstLine, string header, bool existHeader)
        {
            if (firstLine)
                resultString = resultString + line;
            else if (header != "")
            {
                resultString = resultString + "\n" + line;
                resultString = resultString + header;
            }
            else if (addSpace)
                resultString = resultString + "\n\n" + line;
            else if (!addSpace)
                resultString = resultString + "\n" + line;
            else
                resultString = resultString + line;
            //     else
            //         throw new InvalidDataException();
        }

        private static bool CheckNextLine(string file, bool isEmpty = false, bool isHeader = false)
        {
            string line = "";
            bool checkNextLine = false;
            bool result;
            var reader = new StreamReader(file);
            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();
                if (checkNextLine)
                    break;
                if (line.Contains("PROC "))
                {
                    checkNextLine = true;
                }
            }
            if ((line == "" & isEmpty) | (line.Contains("*****") & isHeader))
                result = true;
            else
                result = false;
            reader.Close();
            return result;
        }


        internal static bool FindLoads(string[] files, string dir)
        {
            List<string> loads = new List<string>();
            bool isSuccess = false;
            loads.Add(GetRobotType(dir));
            if (files.Length > 0)
            {
                isSuccess = true;

                List<string> tooldatas = FindLoaddata(files, "tooldata");
                List<string> loaddatas = FindLoaddata(files, "loaddata");
                List<string> armloads = FindArmLoads(dir + "\\SYSPAR\\MOC.cfg");
                loads.Add("TOOLDATAS:\r\n");
                loads.AddRange(tooldatas);
                loads.Add("\r\nLOADDATAS");
                loads.AddRange(loaddatas);
                loads.Add("\r\nARM LOADS");
                loads.AddRange(armloads);
            }
            string test = dir + "\\loads.txt";
            File.WriteAllLines(dir + "\\loads.txt", loads.ToArray());
            System.Diagnostics.Process.Start(dir + "\\loads.txt");
            return isSuccess;

        }

        private static string GetRobotType(string dir)
        {
            string robotNumber = "";
            StreamReader reader = new StreamReader(dir + "\\SYSPAR\\MMC.cfg");
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (line.ToLower().Contains("-backup_name"))
                {
                    Regex regex = new Regex("(?<=\")[a-zA-Z0-9_]*", RegexOptions.IgnoreCase);
                    robotNumber = regex.Match(line).ToString();
                    break;
                }
            }
            reader.Close();

            string strRegex = @"IRB\s+[a-zA-Z0-9\-_\./]*";
            reader = new StreamReader(dir + "\\system.xml");
            string match = Regex.Matches(reader.ReadToEnd(), strRegex)[1].ToString();
            reader.Close();
            return robotNumber + "\r\n" + match + "\r\n";
        }

        private static List<string> FindArmLoads(string mocFile)
        {
            List<string> foundValues = new List<string>();
            StreamReader reader = new StreamReader(mocFile);
            bool addLine = false;
            int couter = 0;
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (line.ToLower().Contains("name \"rob_1_load_1\"") || line.ToLower().Contains("name \"r1_load_1\""))
                    addLine = true;

                if (addLine)
                {
                    foundValues.Add(line);
                }
                if (line.ToLower().Contains("mass_centre_z"))
                    couter++;
                if (couter > 3)
                    break;

            }
            reader.Close();
            return foundValues;
        }



        private static List<string> FindLoaddata(string[] files, string input)
        {
            List<string> foundValues = new List<string>();
            foreach (string file in files)
            {
                StreamReader reader = new StreamReader(file);
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (line.ToLower().Contains(input) && !line.Contains("!*"))
                        foundValues.Add(line);
                }
                reader.Close();
            }

            return foundValues;
        }

        internal static void MirrorPaths(string dir)
        {
            try
            {
                if (!Directory.Exists(dir + "\\Mirrored"))
                    Directory.CreateDirectory(dir + "\\Mirrored");
                int spotModifier = 0;
                bool mirrorToolsAndBases = false;
                Regex isSpot = new Regex(@"(?<=\s+)(swp_|lsp)\d+_\w*\d*", RegexOptions.IgnoreCase);
                Regex spotNumRegex = new Regex(@"(?<=\s+(swp_|lsp))\d+", RegexOptions.IgnoreCase);
                Regex moveRegex = new Regex(@"^Move(J|L|JG|LG|J_CollReq|J_CollClr|J_JobRequest|J_JobFinished|L_CollReq|L_CollClr|L_JobRequest|L_JobFinished)\s+(\[|\d+)+.*", RegexOptions.IgnoreCase);
                Regex getRobtargetRegex = new Regex(@"(?<=(\[+|,))(-\d+\.\d+|\d+\.\d+|-\d+|(\d+(E\+\d+|\s*)))", RegexOptions.IgnoreCase);
                Regex prefixRegexRobtarget = new Regex(@"^.*\=", RegexOptions.IgnoreCase);
                Regex prefixRegexMove = new Regex(@"^\s*[a-zA-Z0-9_]*((\s+\d+(\\[a-zA-Z0-9_\:\=\\]*)\s*,*)|\s+\d+\s*,*|\s+)", RegexOptions.IgnoreCase);
                Regex suffixRegex = new Regex(@"(?<=\]\s*\]).*", RegexOptions.IgnoreCase);
                Regex robholdRegex = new Regex(@"(?<=\=.*)[a-zA-Z]+", RegexOptions.IgnoreCase);
                string[] files = Directory.GetFiles(dir, "*.mod", SearchOption.TopDirectoryOnly);

                DialogResult dialogResultLeftOrRight = MessageBox.Show("Spot points number will be modified\r\nTak - Left to right (spotNr + 1)\r\nNie - Right to left (spotNr - 1)\r\nAnuluj - don't modify number", "?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (dialogResultLeftOrRight == DialogResult.Yes)
                {
                    spotModifier = 1;
                }
                else if (dialogResultLeftOrRight == DialogResult.No)
                {
                    spotModifier = -1;
                }

                DialogResult dialogResult = MessageBox.Show("Mirror tooldata and wobjdata?", "?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    mirrorToolsAndBases = true;
                }
                foreach (var file in files)
                {
                    string resultFile = "";
                    StreamReader reader = new StreamReader(file,Encoding.Default);
                    while (!reader.EndOfStream)
                    {
                        string line = (reader.ReadLine()).Aggregate(
                              new StringBuilder(),
                              (sb, c) => map.TryGetValue(c, out var r) ? sb.Append(r) : sb.Append(c)
                              ).ToString();

                        if (line.Trim() == "")
                        {
                            resultFile += line + "\r\n";
                        }
                        else
                        {
                            if (isSpot.IsMatch(line))
                            {
                                int spotNum = (int.Parse(spotNumRegex.Match(line).ToString())) + spotModifier;
                                string stringToAdd = GetStringWithSpecifiedLenght(spotNum.ToString(), '0', 6, true);
                                line = Regex.Replace(line, spotNumRegex.ToString(), stringToAdd);
                            }
                            if (line.Length > 0 && line.Trim().Substring(0, 1) != "!" && line.ToLower().Contains("robtarget"))
                            {
                                string prefix = prefixRegexRobtarget.Match(line).ToString();
                                MatchCollection matches = getRobtargetRegex.Matches(line.Replace(" ", ""));
                                Robtarget currentPoint = new Robtarget(matches[0].ToString(), matches[1].ToString(), matches[2].ToString(), matches[3].ToString(), matches[4].ToString(), matches[5].ToString(), matches[6].ToString(), matches[7].ToString(), matches[8].ToString(), matches[9].ToString(), matches[10].ToString(), matches[11].ToString());
                                Robtarget mirroredPoint = MirrorRobtarget(currentPoint);
                                resultFile += prefix + "[[" + mirroredPoint.X + "," + mirroredPoint.Y + "," + mirroredPoint.Z + "],[" + mirroredPoint.Quat1 + "," + mirroredPoint.Quat2 + "," + mirroredPoint.Quat3 + "," + mirroredPoint.Quat4 + "],[" + mirroredPoint.Conf1 + "," + mirroredPoint.Conf2 + "," + mirroredPoint.Conf3 + "," + mirroredPoint.Conf4 + "],[" + mirroredPoint.E1 + ",9E+09,9E+09,9E+09,9E+09,9E+09]];\r\n";

                            }
                            else if (line.Length > 0 && line.Trim().Substring(0, 1) != "!" && line.ToLower().Contains("jointtarget"))
                            {
                                string prefix = prefixRegexRobtarget.Match(line).ToString();
                                MatchCollection matches = getRobtargetRegex.Matches(line.Replace(" ", ""));
                                Jointtarget currentPoitn = new Jointtarget(matches[0].ToString(), matches[1].ToString(), matches[2].ToString(), matches[3].ToString(), matches[4].ToString(), matches[5].ToString(), matches[6].ToString());
                                Jointtarget resultPoint = new Jointtarget(ReverseSign(currentPoitn.A1), currentPoitn.A2, currentPoitn.A3, ReverseSign(currentPoitn.A4), currentPoitn.A5, ReverseSign(currentPoitn.A6), currentPoitn.E1);
                                resultFile += prefix + "[[" + resultPoint.A1 + "," + resultPoint.A2 + "," + resultPoint.A3 + "," + resultPoint.A4 + "," + resultPoint.A5 + "," + resultPoint.A6 + "],[" + resultPoint.E1 + ",9E+09,9E+09,9E+09,9E+09,9E+09]];\r\n";
                            }
                            else if (line.Length > 0 && line.Trim().Substring(0, 1) != "!" && moveRegex.IsMatch(line.ToLower().Trim()))
                            {
                                string prefix = prefixRegexMove.Match(line).ToString();
                                string suffix = suffixRegex.Match(line).ToString();
                                MatchCollection matches = getRobtargetRegex.Matches(line.Replace(" ", ""));
                                if (matches.Count == 0)
                                    resultFile += line + "\r\n";
                                else
                                {
                                    Robtarget currentPoint = new MirrorABBClasses.Robtarget(matches[0].ToString(), matches[1].ToString(), matches[2].ToString(), matches[3].ToString(), matches[4].ToString(), matches[5].ToString(), matches[6].ToString(), matches[7].ToString(), matches[8].ToString(), matches[9].ToString(), matches[10].ToString(), matches[11].ToString());
                                    Robtarget mirroredPoint = MirrorRobtarget(currentPoint);
                                    resultFile += prefix + "[[" + mirroredPoint.X + "," + mirroredPoint.Y + "," + mirroredPoint.Z + "],[" + mirroredPoint.Quat1 + "," + mirroredPoint.Quat2 + "," + mirroredPoint.Quat3 + "," + mirroredPoint.Quat4 + "],[" + mirroredPoint.Conf1 + "," + mirroredPoint.Conf2 + "," + mirroredPoint.Conf3 + "," + mirroredPoint.Conf4 + "],[" + mirroredPoint.E1 + ",9E+09,9E+09,9E+09,9E+09,9E+09]]" + suffix + "\r\n";
                                }
                            }
                            else if (line.Length > 0 && line.Trim().Substring(0, 1) != "!" && mirrorToolsAndBases && line.ToLower().Contains("tooldata"))
                            {
                                string prefix = prefixRegexRobtarget.Match(line).ToString();
                                string robhold = robholdRegex.Match(line).ToString();
                                MatchCollection matches = getRobtargetRegex.Matches(line.Replace(" ", ""));
                                //if (robhold.ToLower() == "true")
                                resultFile += prefix + "[" + robhold + ",[[" + matches[0].ToString() + "," + ReverseSign(matches[1].ToString()) + "," + matches[2].ToString() + "],[" + matches[3].ToString() + "," + ReverseSign(matches[4].ToString()) + "," + matches[5].ToString() + "," + ReverseSign(matches[6].ToString()) + "]],[" + matches[7].ToString() + ",[" + matches[8].ToString() + "," + matches[9].ToString() + "," + matches[10].ToString() + "],[" + matches[11].ToString() + "," + matches[12].ToString() + "," + matches[13].ToString() + "," + matches[14].ToString() + "]," + matches[15].ToString() + "," + matches[16].ToString() + "," + matches[17].ToString() + "]];\r\n";
                                //else
                                //    resultFile += prefix + "[" + robhold + ",[[" + matches[0].ToString() + "," + ReverseSign(matches[1].ToString()) + "," + matches[2].ToString() + "],[" + matches[3].ToString() + "," + matches[4].ToString() + "," + matches[5].ToString() + "," + matches[6].ToString() + "]],[" + matches[7].ToString() + ",[" + matches[8].ToString() + "," + matches[9].ToString() + "," + matches[10].ToString() + "],[" + matches[11].ToString() + "," + matches[12].ToString() + "," + matches[13].ToString() + "," + matches[14].ToString() + "]," + matches[15].ToString() + "," + matches[16].ToString() + "," + matches[17].ToString() + "]];\r\n";
                            }
                            else if (line.Length > 0 && line.Trim().Substring(0, 1) != "!" && mirrorToolsAndBases && line.ToLower().Contains("wobjdata"))
                            {
                                string prefix = prefixRegexRobtarget.Match(line).ToString();
                                MatchCollection robholds = robholdRegex.Matches(line.Replace(" ", ""));
                                MatchCollection matches = getRobtargetRegex.Matches(line);
                                resultFile += prefix + "[" + robholds[0].ToString() + "," + robholds[1].ToString() + ",\"\",[[" + matches[0].ToString() + "," + ReverseSign(matches[1].ToString()) + "," + matches[2].ToString() + "],[" + matches[3].ToString() + "," + ReverseSign(matches[4].ToString()) + "," + matches[5].ToString() + "," + ReverseSign(matches[6].ToString()) + "]],[[" + matches[7].ToString() + "," + matches[8].ToString() + "," + matches[9].ToString() + "],[" + matches[10].ToString() + "," + matches[11].ToString() + "," + matches[12].ToString() + "," + matches[13].ToString() + "]]];\r\n";
                            }
                            else if (line.Length > 0 && line.Trim().Substring(0, 1) != "!" && !mirrorToolsAndBases && (line.ToLower().Contains("wobjdata") || line.ToLower().Contains("tooldata")))
                            {
                                //nic nie dodawaj gdy nie odbijamy tool ani basa i trafimy na jego definicje
                            }
                            else
                                resultFile += line + "\r\n";
                        }
                    }
                    reader.Close();
                    File.WriteAllText(dir + "\\Mirrored\\" + Path.GetFileName(file), resultFile);
                }
                MessageBox.Show("Files saved at " + dir + "\\Mirrored\\", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string GetStringWithSpecifiedLenght(string input, char signToAdd, int nrOfRepetitions, bool addToFront)
        {
            string stringToAdd = "";
            int inputLength = input.Length;
            if (inputLength >= nrOfRepetitions)
                return input;
            for (int i = inputLength; i < nrOfRepetitions; i++)
            {
                stringToAdd += signToAdd;
            }
            if (addToFront)
                return stringToAdd + input;
            else
                return input + stringToAdd;

        }

        private static Robtarget MirrorRobtarget(Robtarget currentPoint)
        {
            string[] mirroredConf = MirrorConfABB(currentPoint.Conf1, currentPoint.Conf2, currentPoint.Conf3, currentPoint.Conf4);
            return new Robtarget(currentPoint.X, ReverseSign(currentPoint.Y), currentPoint.Z, currentPoint.Quat1, ReverseSign(currentPoint.Quat2), currentPoint.Quat3, ReverseSign(currentPoint.Quat4), mirroredConf[0], mirroredConf[1], mirroredConf[2], mirroredConf[3], currentPoint.E1);
        }

        private static string ReverseSign(string input)
        {
            string result = "";
            if (input.Trim().Substring(0, 1) == "-")
                result = input.Replace("-", "");
            else
                result = "-" + input;
            return result;
        }

        private static string[] MirrorConfABB(string conf1, string conf2, string conf3, string conf4)
        {
            return new string[] { MirrorConfSingle(conf1), MirrorConfSingle(conf2), MirrorConfSingle(conf3), conf4 };
        }

        private static string MirrorConfSingle(string conf)
        {
            int result = 0;
            int inputConf = int.Parse(conf);
            result = (inputConf + 1) * (-1);
            return result.ToString();
        }

        public static Encoding GetEncoding(string filename)
        {
            // Read the BOM
            var bom = new byte[4];
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                file.Read(bom, 0, 4);
            }

            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;
            return Encoding.ASCII;
        }
    }
}