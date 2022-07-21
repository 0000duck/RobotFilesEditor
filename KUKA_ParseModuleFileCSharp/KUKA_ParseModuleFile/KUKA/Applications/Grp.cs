using WarningHelper;
using ParseModuleFile.KUKA.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ParseModuleFile.KUKA.Applications
{
    public class Grp : Application
    {
        private GrpAction action;
        private int number;
        private bool chk;
        private List<int> partControl;
        private bool plcControl;
        private List<int> actuator;
        private int loadVariante;

        public int Number { get { return number; } set { number = value; } }
        // PartCheck
        public bool Chk { get { return chk; } set { chk = value; } }
        public List<int> PartControl { get { return partControl; } set { partControl = value; } }
        public bool PLCControl { get { return plcControl; } set { plcControl = value; } }
        // GrpAdv, GrpRet
        public List<int> Actuator { get { return actuator; } set { actuator = value; } }
        public int LoadVariante { get { return loadVariante; } set { loadVariante = value; } }

        public GrpAction Action
        {
            get { return action; }
            set { action = value; }
        }


        public override void Enumerate(oldFolds list, ProgramBaseInfo _baseinfo)
        {
        }


        public override void Test(oldFolds list, ProgramBaseInfo _baseinfo)
        {
        }

        public Grp(Warnings warnings)
        {
            _warnings = warnings;
        }

        public Grp(oldFold fold)
        {
            _warnings = fold.Warnings;
            OK = true;
            if (fold.Name.Contains(" PartChk "))
            {
                //Chk:True PartControl:1,2,3 PlcControl:noControl
                chk = isTrue(fold.Name, " Chk", "True", "False", fold);
                plcControl = isTrue(fold.Name, " PlcControl", "Control", "noControl", fold);
                partControl = GetListInteger(fold.Name, "PartControl", fold);
                action = GrpAction.PartChk;
            }
            else if (fold.Name.Contains(" PosRet ") | fold.Name.Contains(" PosAdv ") | fold.Name.Contains(" PosRetNoChk ") | fold.Name.Contains(" PosAdvNoChk "))
            {
                //GrpNo:1 Actuator:1,2 LoadVariante:1
                actuator = GetListInteger(fold.Name, "Actuator", fold);
                loadVariante = GetSingleInteger(fold.Name, "LoadVariante", fold);
                if (fold.Name.Contains(" PosRet "))
                {
                    action = GrpAction.PosRet;
                }
                else if (fold.Name.Contains(" PosRetNoChk "))
                {
                    action = GrpAction.PosRetNoChk;
                }
                else if (fold.Name.Contains(" PosAdvNoChk "))
                {
                    action = GrpAction.PosAdvNoChk;
                }
                else if (fold.Name.Contains(" PosAdv "))
                {
                    action = GrpAction.PosAdv;
                }
            }
            else if (fold.Name.Contains(" Init "))
            {
                action = GrpAction.Init;
            }
            else
            {
                action = GrpAction.UNKNOWN;
                OK = false;
                _warnings.Add(fold.LineStart, WarningType.Intern, Level.Failure, "Unknown command");
            }
            number = GetSingleInteger(fold.Name, "GrpNo", fold);
        }

        public override string ToString()
        {
            return "Grp " + action.ToString();
        }
    }
}
