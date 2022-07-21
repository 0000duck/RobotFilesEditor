using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarningHelper;
using ParseModuleFile.KUKA.Enums;

namespace ParseModuleFile.KUKA.Applications
{

    public class Swp : Application
    {

        private SwpAction action;
        private int number;
        private int gunOpen;
        private bool gunOpenControl;
        private int typeId;
        private int spotNo;
        private bool partEnd;
        private bool equalOffset;
        private bool fastClose;
        private bool leaveCtrl;
        private bool fixedTipUp;
        private bool waterOn;
        private bool rotationLeft;
        private bool tipCheckState;
        private int force;
        private string program;
        private int dresserNo;
        private string dresserState;

        public int Number { get { return number; } set { number = value; } }
        //Positioning and Spotpoint
        public int GunOpen { get { return gunOpen; } set { gunOpen = value; } }
        //Positioning
        public bool GunOpenControl { get { return gunOpenControl; } set { gunOpenControl = value; } }
        //Spotpoint
        public int TypeId { get { return typeId; } set { typeId = value; } }
        public int SpotNo { get { return spotNo; } set { spotNo = value; } }
        public bool PartEnd { get { return partEnd; } set { partEnd = value; } }
        public bool EqualOffset { get { return equalOffset; } set { equalOffset = value; } }
        public bool FastClose { get { return fastClose; } set { fastClose = value; } }
        public bool LeaveCtrl { get { return leaveCtrl; } set { leaveCtrl = value; } }
        public bool FixedTipUp { get { return fixedTipUp; } set { fixedTipUp = value; } }
        public bool WaterOn { get { return waterOn; } set { waterOn = value; } }
        public bool RotationLeft { get { return rotationLeft; } set { rotationLeft = value; } }
        public bool TipCheckState { get { return tipCheckState; } set { tipCheckState = value; } }
        public int Force { get { return force; } set { force = value; } }
        //ServiceProgram
        public string Program { get { return program; } set { program = value; } }
        //MobTipDresser
        public int DresserNo { get { return dresserNo; } set { dresserNo = value; } }
        public string DresserState { get { return dresserState; } set { dresserState = value; } }

        public SwpAction Action { get { return action; } set { action = value; } }

        public override void Enumerate(oldFolds list, ProgramBaseInfo _baseinfo)
        {
            List<int> x = new List<int>();
            foreach (oldFold item in list)
            {
                if (item.Application == null)
                    continue;
                if (!(item.Application is Swp))
                    continue;
                Swp appl = (Swp) item.Application;
                if (appl.Action == SwpAction.Spotpoint & !x.Contains(appl.SpotNo))
                    x.Add(appl.SpotNo);
            }
            x.Sort();
            _baseinfo.applicationPointList = x;
            if (x.Count == 0)
                return;
            _warnings.Add(-2, WarningType.Program_Applications, Level.Information, "Using Spotpoint numbers", string.Join(",", x));
        }

        public override void Test(oldFolds list, ProgramBaseInfo _baseinfo)
        {
            List<int> pointList = new List<int>();
            int spotpoints = 0;
            int swp_positioningCount = 0;
            int part_end = 0;
            foreach (oldFold item in list)
            {
                if (item.Application == null)
                    continue;
                if (!(item.Application is Swp))
                    continue;
                Swp appl = (Swp) item.Application;
                if (appl.Action == SwpAction.Positioning)
                {
                    swp_positioningCount += 1;
                    if (appl.GunOpenControl == false)
                    {
                        _warnings.Add(item.LineStart, WarningType.Program_Applications, Level.Warning, "Swp Positioning without GunOpenControl");
                        appl.OK = false;
                    }
                }
                else if (appl.Action == SwpAction.Spotpoint)
                {
                    if (appl.EqualOffset == true)
                    {
                        _warnings.Add(item.LineStart, WarningType.Program_Applications, Level.Warning, "EqualOffset used", appl.SpotNo.ToString());
                        appl.OK = false;
                    }
                    if (appl.FastClose == true)
                    {
                        _warnings.Add(item.LineStart, WarningType.Program_Applications, Level.Warning, "FastClose used", appl.SpotNo.ToString());
                        appl.OK = false;
                    }
                    if (appl.LeaveCtrl == false)
                    {
                        _warnings.Add(item.LineStart, WarningType.Program_Applications, Level.Failure, "No LeaveCtrl used", appl.SpotNo.ToString());
                        appl.OK = false;
                    }
                    if (appl.PartEnd)
                        part_end += 1;
                    if (appl.TypeId != 21)
                    {
                        _warnings.Add(item.LineStart, WarningType.Program_Applications, Level.Failure, "Wrong TypeId used", appl.SpotNo.ToString());
                        appl.OK = false;
                    }
                    spotpoints += 1;
                }
            }
            if (spotpoints > 0 & part_end != 1)
            {
                _baseinfo.applicationPointListOK = false;
                _warnings.Add(-2, WarningType.Program_Applications, Level.Failure, "Not used or too many PartEnd");
            }
            if (swp_positioningCount > 2)
            {
                _baseinfo.applicationPointListOK = false;
                _warnings.Add(-2, WarningType.Program_Applications, Level.Warning, "Swp Positioning is used " + swp_positioningCount.ToString() + " times.");
            }
        }

        public Swp(Warnings warnings)
        {
            _warnings = warnings;
        }

        public Swp(oldFold fold)
        {
            _warnings = fold.Warnings;
            program = "";
            dresserState = "";
            number = GetSingleInteger(fold.Name, "GunNo", fold);
            OK = true;
            //If _Movement.moveType <> MoveType.None Then Stop
            if (fold.Name.Contains(" Positioning "))
            {
                //Swp Positioning PTP via28 GunNo:1 GunOpen:200mm GunOpenCtrl:Ctrl CONT Vel:100% Pvia28 TOOL[1]:xGun1 BASE[1]:030FX001
                gunOpen = GetSingleInteger(fold.Name, "GunOpen", fold);
                gunOpenControl = isTrue(fold.Name, "GunOpenCtrl", "Ctrl", "None", fold);
                action = SwpAction.Positioning;
            }
            else if (fold.Name.Contains(" Spotpoint "))
            {
                //Swp Spotpoint PTP TypeId:22 SpotNo:30623 GunNo:1 GunOpen:200mm Vel:100%  TOOL[1]:xGun1 BASE[1]:030FX001 PartEnd:False EqualOffset:False FastClose:False LeaveCtrl:Ctrl
                typeId = GetSingleInteger(fold.Name, "TypeId", fold);
                spotNo = GetSingleInteger(fold.Name, "SpotNo", fold);
                gunOpen = GetSingleInteger(fold.Name, "GunOpen", fold);
                partEnd = isTrue(fold.Name, "PartEnd", "True", "False", fold);
                equalOffset = isNotFalse(fold.Name, "EqualOffset", "False", fold);
                leaveCtrl = isTrue(fold.Name, "LeaveCtrl", "Ctrl", "None", fold);
                action = SwpAction.Spotpoint;
            }
            else if (fold.Name.Contains(" Init "))
            {
                gunOpen = GetSingleInteger(fold.Name, "GunOpen", fold);
                action = SwpAction.Init;
            }
            else if (fold.Name.Contains(" EqualCalib "))
            {
                fixedTipUp = isTrue(fold.Name, "FixedTip", "Up", "Down", fold);
                action = SwpAction.EqualCalib;
            }
            else if (fold.Name.Contains(" Water "))
            {
                //Swp Water GunNo:1 Switch:Off
                waterOn = isTrue(fold.Name, "Switch", "On", "Off", fold);
                action = SwpAction.Water;
            }
            else if (fold.Name.Contains(" TipChangeUnit "))
            {
                //Swp TipChangeUnit GunNo:1 Rotation:Left
                rotationLeft = isTrue(fold.Name, "Rotation", "Left", "Right", fold);
                action = SwpAction.TipChangeUnit;
            }
            else if (fold.Name.Contains(" TipCheck "))
            {
                //Swp TipCheck GunNo:1 State:False
                tipCheckState = isTrue(fold.Name, "State", "True", "False", fold);
                action = SwpAction.TipCheck;
            }
            else if (fold.Name.Contains(" TipPress "))
            {
                //Swp TipCheck GunNo:1 State:False
                equalOffset = isTrue(fold.Name, "EqualOffset", "True", "False", fold);
                gunOpen = GetSingleInteger(fold.Name, "GunOpen", fold);
                force = GetSingleInteger(fold.Name, "Force", fold);
                action = SwpAction.TipPress;
            }
            else if (fold.Name.Contains(" QuitTipChange "))
            {
                //Swp QuitTipChange GunNo:1
                action = SwpAction.QuitTipChange;
            }
            else if (fold.Name.Contains(" GunReference "))
            {
                //Swp GunReference GunNo:1 GunOpen:150mm
                gunOpen = GetSingleInteger(fold.Name, "GunOpen", fold);
                action = SwpAction.GunReference;
            }
            else if (fold.Name.Contains(" Firstdress "))
            {
                //Swp Firstdress GunNo:1 GunOpen:150mm EqualOffset:False
                equalOffset = isTrue(fold.Name, "EqualOffset", "True", "False", fold);
                gunOpen = GetSingleInteger(fold.Name, "GunOpen", fold);
                action = SwpAction.Firstdress;
            }
            else if (fold.Name.Contains(" ResistorScale "))
            {
                //Swp ResistorScale GunNo:1 GunOpen:150mm
                gunOpen = GetSingleInteger(fold.Name, "GunOpen", fold);
                action = SwpAction.ResistorScale;
            }
            else if (fold.Name.Contains(" ResistorCheck "))
            {
                //Swp ResistorScale GunNo:1 GunOpen:150mm
                gunOpen = GetSingleInteger(fold.Name, "GunOpen", fold);
                action = SwpAction.ResistorCheck;
            }
            else if (fold.Name.Contains(" Dress "))
            {
                //Swp Dress GunNo:1 GunOpen:150mm EqualOffset:False
                equalOffset = isTrue(fold.Name, "EqualOffset", "True", "False", fold);
                gunOpen = GetSingleInteger(fold.Name, "GunOpen", fold);
                action = SwpAction.Dress;
            }
            else if (fold.Name.Contains(" ServiceProgram "))
            {
                //Swp ServiceProgram GunNo:1 Program:EqualCalib
                program = GetString(fold.Name, "Program", fold);
                action = SwpAction.ServiceProgram;
            }
            else if (fold.Name.Contains(" MobTipDresser "))
            {
                //Swp MobTipDresser DresserNo:1 DresserState:BoltOpen
                dresserNo = GetSingleInteger(fold.Name, "DresserNo", fold);
                dresserState = GetString(fold.Name, "DresserState", fold);
                action = SwpAction.MobTipDresser;
            }
            else if (fold.Name.Contains(" ServiceProgramMobDresser "))
            {
                //Swp ServiceProgramMobDresser DresserNo:1 Program:DockDresser
                dresserNo = GetSingleInteger(fold.Name, "DresserNo", fold);
                program = GetString(fold.Name, "Program", fold);
                action = SwpAction.ServiceProgramMobDresser;
            }
            else
            {
                action = SwpAction.UNKNOWN;
                OK = false;
                _warnings.Add(fold.LineStart, WarningType.Intern, Level.Failure, "Unknown command", fold.Name);
            }
        }

        public override string ToString()
        {
            if (Action == SwpAction.Spotpoint)
            {
                return "Swp Spotpoint " + SpotNo.ToString() + "_" + TypeId.ToString();
            }
            return "Swp " + action.ToString();
        }
    }
}
