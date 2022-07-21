using WarningHelper;
using ParseModuleFile.KUKA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.KUKA.Applications
    {

        public class CollZone : Application
        {
            private bool request;
            private int number;
            private string desc;
            private bool external;

            public bool Request { get { return request; } set { request = value; } }
            public int Number { get { return number; } set { number = value; } }
            public string Desc { get { return desc; } set { desc = value; } }
            public bool External { get { return external; } set { external = value; } }

            public override void Enumerate(oldFolds list, ProgramBaseInfo _baseinfo)
            {
                Dictionary<int, string> localX = new Dictionary<int, string>();
                Dictionary<int, string> externX = new Dictionary<int, string>();
                foreach (oldFold item in list)
                {
                    if (item.Application == null)
                        continue;
                    if (!(item.Application is CollZone))
                        continue;
                    CollZone appl = (CollZone) item.Application;
                    if (appl.External)
                    {
                        if (!externX.ContainsKey(appl.Number))
                        {
                            externX.Add(appl.Number, appl.Desc);
                        }
                        else
                        {
                            if (appl.Desc.Trim() != externX[appl.Number].Trim())
                            {
                                _warnings.Add(item.LineStart, WarningType.Program_Applications, Level.Warning, "Description differs between the same numbers of collisions", "this=\"" + appl.Desc + "\", previous=\"" + externX[appl.Number] + "\"");
                                _baseinfo.colllistOK = false;
                            }
                        }
                    }
                    else
                    {
                        if (!localX.ContainsKey(appl.Number))
                        {
                            localX.Add(appl.Number, appl.Desc);
                        }
                        else
                        {
                            if (appl.Desc.Trim() != localX[appl.Number].Trim())
                            {
                                _warnings.Add(item.LineStart, WarningType.Program_Applications, Level.Warning, "Description differs between the same numbers of collisions", "this=\"" + appl.Desc + "\", previous=\"" + localX[appl.Number] + "\"");
                                _baseinfo.colllistOK = false;
                            }
                        }
                    }
                }
                _baseinfo.localCollList = localX;
                _baseinfo.extCollList = externX;
                if (localX.Count > 0)
                {
                    _warnings.Add(-2, WarningType.Program_Applications, Level.Information, "Used CollZone numbers", string.Join(",", localX));
                }
                if (externX.Count > 0)
                {
                    _warnings.Add(-2, WarningType.Program_Applications, Level.Information, "Used External CollZone numbers", string.Join(",", externX));
                }
            }

            public override void Test(oldFolds list, ProgramBaseInfo _baseinfo)
            {
                int count = 0;
                List<int> openList = new List<int>();
                List<int> closeList = new List<int>();
                int open = 0;
                int close = 0;
                foreach (oldFold item in list)
                {
                    if (item.Application == null)
                        continue;
                    if (!(item.Application is CollZone))
                        continue;
                    CollZone appl = (CollZone) item.Application;
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
                    _warnings.Add(-2, WarningType.Program_Applications, Level.Failure, "CollZone request/release is not even");
                    _baseinfo.colllistOK = false;
                }
                if (open != close)
                {
                    _warnings.Add(-2, WarningType.Program_Applications, Level.Failure, "CollZone request count doesn't match CollZone release count");
                    _baseinfo.colllistOK = false;
                }
                foreach (int i in openList)
                {
                    if (!closeList.Contains(i))
                    {
                        _baseinfo.colllistOK = false;
                        _warnings.Add(-2, WarningType.Program_Applications, Level.Failure, "CollZone " + i.ToString() + " was not closed");
                    }
                }
            }

            public CollZone(Warnings warnings)
            {
                _warnings = warnings;
            }

            public CollZone(oldFold fold)
            {
                _warnings = fold.Warnings;
                request = isTrue(fold.Name, "", " Request ", " Release ", fold);
                number = GetSingleInteger(fold.Name, "ZoneNum", fold);
                desc = GetString(fold.Name, "Desc", fold, true);
                external = isTrue(fold.Name, "", " ExtPlc ", "", fold);
                OK = true;
                //If _Movement.moveType <> MoveType.None Then Stop
                if (desc.Contains(";"))
                {
                    _warnings.Add(fold.LineStart, WarningType.Program_Applications, Level.Failure, "Description contains \";\": " + (request ? "Request" : "Release" + " " + number.ToString()));
                    OK = false;
                }
                Dictionary<string, string> @params = GetParams(fold);
                if (@params != null)
                {
                    if (@params.ContainsKey("Plc_CollZoneNum"))
                    {
                        if ((@params["Plc_CollZoneNum"] != number.ToString()))
                        {
                            _warnings.Add(fold.LineStart, WarningType.Intern, Level.Failure, "Integrity check failed!: " + (request ? "Request" : "Release" + " " + number.ToString()));
                            OK = false;
                        }
                    }
                    else
                    {
                        _warnings.Add(fold.LineStart, WarningType.Intern, Level.Failure, "Could not find property Plc_CollZoneNum in \";Params\"!: " + (request ? "Request" : "Release" + " " + number.ToString()));
                        OK = false;
                    }
                }
                else
                {
                    _warnings.Add(fold.LineStart, WarningType.Intern, Level.Failure, "FoldCollZone: could not read property \";Params\"!: " + (request ? "Request" : "Release" + " " + number.ToString() + " in fold CollZone"));
                    OK = false;
                }
            }

            public override string ToString()
            {
                return "CollZone " + (request ? "Request" : "Release" + " " + number.ToString());
            }
        }
    }
