using WarningHelper;
using ParseModuleFile.KUKA.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ParseModuleFile.KUKA.Applications
{
    public class PlcCom : Application
    {

        PlcComAction action1;
        PlcComAction action2;
        PlcComAction action3;

        private int number1;
        private int number2;
        private int number3;

        public int Number1 { get { return number1; } set { number1 = value; } }
        public int Number2 { get { return number2; } set { number2 = value; } }
        public int Number3 { get { return number3; } set { number3 = value; } }

        public PlcComAction Action1
        {
            get { return action1; }
            set { action1 = value; }
        }

        public PlcComAction Action2
        {
            get { return action2; }
            set { action2 = value; }
        }

        public PlcComAction Action3
        {
            get { return action3; }
            set { action3 = value; }
        }

        public override void Enumerate(oldFolds list, ProgramBaseInfo _baseinfo)
        {
            List<int> x = new List<int>();
            foreach (oldFold item in list)
            {
                if (item.Application == null)
                    continue;
                if (!(item.Application is PlcCom))
                    continue;
                PlcCom appl = (PlcCom) item.Application;
                if (!x.Contains(appl.Number1))
                    x.Add(appl.Number1);
                if (!x.Contains(appl.Number2))
                    x.Add(appl.Number2);
                if (!x.Contains(appl.Number3))
                    x.Add(appl.Number3);
            }
            x.Sort();
            _baseinfo.plcList = x;
            if (x.Count == 0)
                return;
            _warnings.Add(-2, WarningType.Program_Applications, Level.Information, "Using PlcCom numbers", string.Join(",", x));

        }

        public override void Test(oldFolds list, ProgramBaseInfo _baseinfo)
        {
            foreach (oldFold item in list)
            {
                if (item.Application == null)
                    continue;
                if (!(item.Application is PlcCom))
                    continue;
                PlcCom appl = (PlcCom) item.Application;
                if (appl.Action1 != PlcComAction.SET & appl.Action2 != PlcComAction.WAIT1 & appl.Action3 != PlcComAction.RESET)
                {
                    _warnings.Add(item.LineStart, WarningType.Program_Applications, Level.Warning, "PlcCom configuration not standard (standard is 1. set, 2. wait_di=1, 3. reset)");
                    _baseinfo.plclistOK = false;
                }
                if (appl.Number1 != appl.Number2 | appl.Number2 != appl.Number3)
                {
                    _warnings.Add(item.LineStart, WarningType.Program_Applications, Level.Warning, "PlcCom using not the same signals", " com1:" + appl.Number1.ToString() + ", com2:" + appl.Number2.ToString() + ", com3:" + appl.Number3.ToString());
                    _baseinfo.plclistOK = false;
                }
            }
        }

        public PlcCom(Warnings warnings)
        {
            _warnings = warnings;
        }

        public PlcCom(oldFold fold)
        {
            _warnings = fold.Warnings;
            //PlcCom PTP p_maintenance_Clean 1.Com:Set 422 '' 2.Com:Wait_In=1 422 '' 3.Com:Reset 422 '' Vel:100% Pp_maintenance_Clean TOOL[1]:xGun_1 BASE[0]:WorldFrame
            KeyValuePair<PlcComAction, int> x = default(KeyValuePair<PlcComAction, int>);
            x = GetPlcComData(fold.Name, "1.Com", fold);
            action1 = x.Key;
            number1 = x.Value;
            x = GetPlcComData(fold.Name, "2.Com", fold);
            action2 = x.Key;
            number2 = x.Value;
            x = GetPlcComData(fold.Name, "3.Com", fold);
            action3 = x.Key;
            number3 = x.Value;
            OK = true;
            //If _Movement.moveType <> MoveType.None Then Stop
            //;Params IlfProvider=plccom; ComMove=_; ComCmd1=SO; ComUserOut1=422; ComCmd2=WI1; ComUserIn2=422; ComCmd3=RO; ComUserOut3=422; ComCont=_; JobDesc=Wartungsposition erreicht
            Dictionary<string, string> @params = GetParams(fold);
            if (@params != null)
            {
                if (@params.ContainsKey("Plc_ComCmd1") & @params.ContainsKey("Plc_ComUserOut1") & @params.ContainsKey("Plc_ComCmd2") & @params.ContainsKey("Plc_ComUserIn2") & @params.ContainsKey("Plc_ComCmd3") & @params.ContainsKey("Plc_ComUserOut3"))
                {
                    if (
                        (@params["Plc_ComCmd1"] == "SO" & action1 == PlcComAction.SET) & 
                        (@params["Plc_ComUserOut1"] == number1.ToString()) & 
                        (@params["Plc_ComCmd2"] == "WI1" & action2 == PlcComAction.WAIT1) & 
                        (@params["Plc_ComUserIn2"] == number2.ToString()) & 
                        (@params["Plc_ComCmd3"] == "RO" & action3 == PlcComAction.RESET) & 
                        (@params["Plc_ComUserOut3"] == number3.ToString()))
                    {
                    }
                    else
                    {
                        _warnings.Add(fold.LineStart, WarningType.Intern, Level.Failure, "PlcCom Integrity check failed");
                        OK = false;
                    }
                }
                else
                {
                    _warnings.Add(fold.LineStart, WarningType.Intern, Level.Failure, "Missing correct attribute names in \";Params\"");
                    OK = false;
                }
            }
            else
            {
                _warnings.Add(fold.LineStart, WarningType.Intern, Level.Failure, "Could not read property \";Params\"");
                OK = false;
            }
        }

        private static KeyValuePair<PlcComAction, int> GetPlcComData(string to_check, string to_search, oldFold fold)
        {
            // PlcCom 1.Com:Set 1 'PLC_do_01' 2.Com:Wait_In=1 1 'PLC_di_01' 3.Com:Reset 1 'PLC_do_01' Desc:10fx21: 14, 13, 17 rueck
            // PlcCom  1.Com:_  2.Com:Send_UserNum 100  3.Com:_
            PlcComAction key = PlcComAction.UNKNOWN;
            int value = -1;
            if (to_check.Contains(to_search))
            {
                Regex reNum = new Regex(to_search + ":(?:(?:(?<in0>Wait_In=0)|(?<in1>Wait_In=1)|(?<set>Set)|(?<reset>Reset)|(?<send_usernum>Send_UserNum)|)\\s(?<num>\\d*)(?:\\s'(?<comment>.*?)'|)|(?<empty>_?))", RegexOptions.IgnoreCase);
                Match match = reNum.Match(to_check);
                if (!match.Groups["num"].Success & !match.Groups["empty"].Success)
                {
                    fold.Warnings.Add(fold.LineStart, WarningType.Intern, Level.Failure, "Could not parse " + to_search, to_check);
                    //Throw New NotImplementedException
                }
                else if (match.Groups["num"].Success)
                {
                    if (!int.TryParse(match.Groups["num"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture,out value))
                    {
                        fold.Warnings.Add(fold.LineStart, WarningType.Intern, Level.Failure, "Could not get integer from " + match.Groups["num"].Value, to_check);
                    }
                }
                if (!(match.Groups["in0"].Success | match.Groups["in1"].Success | match.Groups["set"].Success | match.Groups["reset"].Success | match.Groups["send_usernum"].Success | match.Groups["empty"].Success))
                {
                    key = PlcComAction.NOTHING;
                    fold.Warnings.Add(fold.LineStart, WarningType.Intern, Level.Failure, "did not found any match");
                    //Throw New NotImplementedException
                }
                else
                {
                    if(match.Groups["in0"].Success) key = PlcComAction.WAIT0;
                    else if (match.Groups["in1"].Success) key = PlcComAction.WAIT1;
                    else if (match.Groups["set"].Success) key = PlcComAction.SET;
                    else if (match.Groups["reset"].Success) key = PlcComAction.RESET;
                    else if (match.Groups["send_usernum"].Success) key = PlcComAction.SETUSERNUM;
                    else if (match.Groups["empty"].Success) key = PlcComAction.NOTHING;
                }
                return new KeyValuePair<PlcComAction, int>(key, value);
            }
            else
            {
                fold.Warnings.Add(fold.LineStart, WarningType.Intern, Level.Failure, "String " + to_check + " doesn't contain " + to_search);
                return new KeyValuePair<PlcComAction, int>(key, value);
            }
        }

        public override string ToString()
        {
            return "PlcCom";
        }
    }
}
