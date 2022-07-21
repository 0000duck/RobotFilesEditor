using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarningHelper;
using System.Text.RegularExpressions;
using System.Globalization;
using ParseModuleFile.KUKA.Applications;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA
{
    public abstract class Application : MyNotifyPropertyChanged
    {
        private bool ok = false;
        internal Warnings _warnings;
        public bool OK
        {
            get { return ok; }
            set { Set(ref ok, value); }
        }

        public void EnumerateAndTest(oldFolds list, ProgramBaseInfo _baseinfo)
        {
            Enumerate(list, _baseinfo);
            Test(list, _baseinfo);
        }

        public abstract void Enumerate(oldFolds list, ProgramBaseInfo _baseinfo);

        public abstract void Test(oldFolds list, ProgramBaseInfo _baseinfo);

        //Public MustOverride Sub Parse(ByRef fold As Fold)

        static internal string getReValue(string pattern, string search)
        {
            Match match = RegexHelper.Match(pattern, search);
            //Match match = RegexHelper.Get(pattern).Match(search);
            if (match.Groups[1].Success)
            {
                return match.Groups[1].Value;
            }
            else
            {
                return null;
            }
        }
        static internal int getReInteger(string pattern, string search, oldFold fold)
        {
            //Match match = RegexHelper.Get(pattern).Match(search);
            Match match = RegexHelper.Match(pattern, search);
            if (match.Groups[1].Success)
            {
                int x = 0;
                if (!int.TryParse(match.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out x))
                {
                    fold.Warnings.Add(fold.LineStart, WarningType.Intern, Level.Failure, "Could not get integer from " + match.Groups[1].Value + " in pattern " + pattern, search);
                    return -1;
                }
                return x;
            }
            else
            {
                fold.Warnings.Add(fold.LineStart, WarningType.Intern, Level.Failure, "Could not find pattern " + pattern, search);
                return -1;
            }
        }
        static internal double getReDouble(string pattern, string search, oldFold fold)
        {
            //Match match = RegexHelper.Get(pattern).Match(search);
            Match match = RegexHelper.Match(pattern, search);
            if (match.Groups[1].Success)
            {
                double x = 0;
                if (!double.TryParse(match.Groups[1].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out x))
                {
                    fold.Warnings.Add(fold.LineStart, WarningType.Intern, Level.Failure, "Could not get double from " + match.Groups[1].Value + " in pattern " + pattern, search);
                    return -1;
                }
                return x;
            }
            else
            {
                fold.Warnings.Add(fold.LineStart, WarningType.Intern, Level.Failure, "Could not find pattern " + pattern, search);
                return -1;
            }
        }
        static internal bool isTrue(string to_check, string start, string _true, string _false, oldFold fold)
        {
            if (string.IsNullOrEmpty(start))
            {
                if (to_check.Contains(_true))
                {
                    return true;
                }
                else if (to_check.Contains(_false))
                {
                    return false;
                }
                else
                {
                    fold.Warnings.Add(fold.LineStart, WarningType.Intern, Level.Failure, "String " + to_check + " doesn't contain " + start + ":" + _true + " nor " + start + ":" + _false);
                    return false;
                }
            }
            if (to_check.Contains(start + ":" + _true))
            {
                return true;
            }
            else if (to_check.Contains(start + ":" + _false))
            {
                return false;
            }
            else
            {
                fold.Warnings.Add(fold.LineStart, WarningType.Intern, Level.Failure, "String " + to_check + " doesn't contain " + start + ":" + _true + " nor " + start + ":" + _false);
                return false;
            }
        }

        static internal bool isNotFalse(string to_check, string start, string _false, oldFold fold)
        {
            if (string.IsNullOrEmpty(start))
            {
                return !to_check.Contains(_false); 
            }
            if (to_check.Contains(start + ":" + _false))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        static internal string GetString(string to_check, string to_search, oldFold fold, bool _optional = false)
        {
            if (to_check.Contains(to_search))
            {
                Match match = RegexHelper.Match(to_search + ":(?<str>.*)(\\w*:|)", to_check);
                //Match match = RegexHelper.Get(to_search + ":(?<str>.*)(\\w*:|)").Match(to_check);
                if (match.Groups["str"].Success)
                {
                    return match.Groups["str"].Value;
                }
                else
                {
                    fold.Warnings.Add(fold.LineStart, WarningType.Intern, Level.Failure, "Could find pattern " + to_search + ":(?<str>.*)(\\w*:|)", to_check);
                    return "";
                }
            }
            else
            {
                if (!_optional)
                    fold.Warnings.Add(fold.LineStart, WarningType.Intern, Level.Failure, "Could not find " + to_search, to_check);
                return "";
            }
        }

        static internal int GetSingleInteger(string to_check, string to_search, oldFold fold)
        {
            if (to_check.Contains(to_search))
            {
                Match match = RegexHelper.Match(to_search + ":(?<num>\\d*)", to_check);
                //Match match = RegexHelper.Get(to_search + ":(?<num>\\d*)").Match(to_check);
                if (match.Groups["num"].Success)
                {
                    int x = 0;
                    if (!int.TryParse(match.Groups["num"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out x))
                    {
                        fold.Warnings.Add(fold.LineStart, WarningType.Intern, Level.Failure, "Could not get integer from " + match.Groups["num"].Value + " in pattern " + to_search + ":(?<num>\\d*)", to_check);
                        return -1;
                    }
                    return x;
                }
                else
                {
                    fold.Warnings.Add(fold.LineStart, WarningType.Intern, Level.Failure, "Could not find pattern " + to_search + ":(?<num>\\d*)", to_check);
                    return -1;
                }
            }
            else
            {
                fold.Warnings.Add(fold.LineStart, WarningType.Intern, Level.Failure, "Could not find " + to_search, to_check);
                return -1;
            }
        }

        static internal List<int> GetListInteger(string to_check, string to_search, oldFold fold)
        {
            List<int> functionReturnValue = default(List<int>);
            functionReturnValue = new List<int>();
            Match match = RegexHelper.Match(to_search + ":(?<list>[0-9,]*)", to_check);
            //Match match = RegexHelper.Get(to_search + ":(?<list>[0-9,]*)").Match(to_check);
            if (match.Groups["list"].Success)
            {
                string[] parts = match.Groups["list"].Value.Split(',');
                foreach (string part in parts)
                {
                    int x = 0;
                    if (!int.TryParse(part, NumberStyles.Integer, CultureInfo.InvariantCulture, out x))
                    {
                        fold.Warnings.Add(fold.LineStart, WarningType.Intern, Level.Failure, "Could not get integer from " + part + " in pattern " + to_search + ":(?<list>[0-9,]*)", to_check);
                        continue;
                    }
                    functionReturnValue.Add(x);
                }
            }
            return functionReturnValue;
        }

        static internal Dictionary<string, string> GetParams(oldFold fold)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            bool found = false;
            string @params = "";
            foreach (string line in fold.Contents.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
            {
                if (line.Trim().StartsWith(";Params "))
                {
                    @params = line;
                    found = true;
                    break;
                }
            }
            if (!found)
                return null;
            @params = @params.Trim();
            @params = @params.Substring(9);
            string[] alist = @params.Split(';');
            foreach (string item in alist)
            {
                string[] x = item.Trim().Split('=');
                if (x.Length == 2)
                {
                    ret.Add(x[0], x[1]);
                }
            }
            return ret;
        }
        public static void SetApplication(oldFold fold)
        {
            if (fold.Name.StartsWith("Area"))
                fold.Application = new Area(fold);
            else if (fold.Name.StartsWith("Job"))
                fold.Application = new Job(fold);
            else if (fold.Name.StartsWith("CollZone"))
                fold.Application = new CollZone(fold);
            else if (fold.Name.StartsWith("Grp"))
                fold.Application = new Grp(fold);
            else if (fold.Name.StartsWith("Swp"))
                fold.Application = new Swp(fold);
            else if (fold.Name.StartsWith("Tch"))
                fold.Application = new Tch(fold);
            else if (fold.Name.StartsWith("PlcCom"))
                fold.Application = new PlcCom(fold);
            else if (fold.Name.StartsWith("PTP"))
            { }
            else if (fold.Name.StartsWith("LIN"))
            { }
            else
                fold.Warnings.Add(fold.LineStart, WarningType.Program_Applications, Level.Warning, "Unknown fold type", fold.Name);
        }
    }
}
