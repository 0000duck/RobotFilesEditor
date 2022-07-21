using WarningHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.KUKA.Applications
{
    public class Area : Application
    {
        private bool request;
        private int number;
        private string abort;
        private string desc;

        public bool Request { get { return request; } set { request = value; } }
        public int Number { get { return number; } set { number = value; } }
        public string Abort { get { return abort; } set { abort = value; } }
        public string Desc { get { return desc; } set { desc = value; } }

        public override void Enumerate(oldFolds list, ProgramBaseInfo _baseinfo)
        {
            Dictionary<int, string> x = new Dictionary<int, string>();
            foreach (oldFold item in list)
            {
                if (item.Application == null)
                    continue;
                if (!(item.Application is Area))
                    continue;
                Area appl = (Area) item.Application;
                if (!x.ContainsKey(appl.Number))
                {
                    x.Add(appl.Number, appl.Desc);
                }
            }
            _baseinfo.areaList = x;
            if (x.Count == 0)
                return;
            _warnings.Add(-2, WarningType.Program_Applications, Level.Information, "Used area numbers", string.Join(",", x));
        }

        public override void Test(oldFolds list, ProgramBaseInfo _baseinfo)
        {
            List<int> openList = new List<int>();
            List<int> closeList = new List<int>();
            int open = 0;
            int close = 0;
            int count = 0;
            foreach (oldFold item in list)
            {
                if (item.Application == null)
                    continue;
                if (!(item.Application is Area))
                    continue;
                Area appl = (Area) item.Application;
                if (appl.Request == true)
                {
                    open += 1;
                    openList.Add(appl.Number);
                }
                else
                {
                    close += 1;
                    closeList.Add(appl.Number);
                }
                count += 1;
            }
            if (count % 2 == 1)
            {
                _warnings.Add(-2, WarningType.Program_Applications, Level.Warning, "Area request/release is not even");
                _baseinfo.areaListOK = false;
            }
            if (open != close)
            {
                _warnings.Add(-2, WarningType.Program_Applications, Level.Warning, "Area request count doesn't match area release count");
                _baseinfo.areaListOK = false;
            }
            foreach (int i in openList)
            {
                if (!closeList.Contains(i))
                {
                    _warnings.Add(-2, WarningType.Program_Applications, Level.Warning, "Area " + i.ToString() + " was not closed");
                    _baseinfo.areaListOK = false;
                }
            }
        }
        public Area(Warnings warnings)
        {
            _warnings = warnings;
        }
        public Area(oldFold fold)
        {
            _warnings = fold.Warnings;
            request = isTrue(fold.Name, "", " Request ", " Release ", fold);
            number = GetSingleInteger(fold.Name, "AreaNum", fold);
            desc = GetString(fold.Name, "Desc", fold, true);
            abort = "";
            OK = true;
            Dictionary<string, string> @params = GetParams(fold);
            if (@params != null)
            {
                if (@params.ContainsKey("Plc_AreaNum"))
                {
                    if ((@params["Plc_AreaNum"] != number.ToString()))
                    {
                        _warnings.Add(fold.LineStart, WarningType.Intern, Level.Failure, "Integrity check failed");
                        OK = false;
                    }
                }
                else
                {
                    _warnings.Add(fold.LineStart, WarningType.Intern, Level.Failure, "Could not find property Plc_AreaNum in \";Params\"");
                    OK = false;
                }
                if (@params.ContainsKey("Plc_AreaAbort"))
                {
                    abort = @params["Plc_AreaAbort"];
                }
                else
                {
                    if (request == true)
                    {
                        _warnings.Add(fold.LineStart, WarningType.Program_Applications, Level.Failure, "No abort action");
                        OK = false;
                    }
                }
            }
            else
            {
                _warnings.Add(fold.LineStart, WarningType.Intern, Level.Failure, "Could not read property \";Params\" in fold Area");
                OK = false;
            }
        }

        public override string ToString()
        {
            return "Area " + (Request ? "Request" : "Release" + " " + Number.ToString());
        }
    }
}
