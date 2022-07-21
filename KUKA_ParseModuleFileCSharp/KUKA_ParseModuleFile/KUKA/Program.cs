using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarningHelper;
using ParseModuleFile.KUKA.Enums;
using ParseModuleFile.KUKA.Applications;
using System.Windows.Data;
using ParseModuleFile.KUKA.DataTypes;
using ParseModuleFile.ViewModel;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA
{
    public class Program : MyNotifyPropertyChanged
    {
        private ProgramBaseInfo baseInfo = new ProgramBaseInfo();
        private Warnings warnings;
        private string fileName;
        private Parser parser;
        private ListCollectionView warningView;
        private VariablesContainer data;

        #region static
        static internal bool should_have_movements;
        public static ObservableDictionary<string, E6POS> GlobalE6pos { get; set; } // E6POS
        public static ObservableDictionary<string, E6AXIS> GlobalE6Axis { get; set; } // E6AXIS
        public static ObservableDictionary<string, FDAT> GlobalFDat { get; set; } // FDAT
        public static ObservableDictionary<string, LDAT> GlobalLDat { get; set; } // LDAT
        public static ObservableDictionary<string, PDAT> GlobalPDat { get; set; } // PDAT
        #endregion

        internal List<string> localVariableList;

        //public Dictionary<int, object> Data { get { return data; } set { Set(ref data, value); } }
        public string FileName { get { return fileName; } set { if (Set(ref fileName, value)) NotifyPropertyChanged("ShortFileName"); } }
        public string ShortFileName { get { return System.IO.Path.GetFileNameWithoutExtension(fileName); } }
        public ProgramBaseInfo BaseInfo { get { return baseInfo; } set { Set(ref baseInfo, value); } }
        public Warnings Warnings { get { return warnings; } set { Set(ref warnings, value); } }
        public ListCollectionView WarningView { get { return warningView; } set { Set(ref warningView, value); } }

        public object WarningsCount { get { return WarningView.Count; } }

        public Parser Parser { get { return parser; } set { Set(ref parser, value); } }

        #region constructors
        protected Program()
        {
            localVariableList = new List<string>();
            warnings = new Warnings();
        }

        public Program(string FileName, Parser SrcData, VariablesContainer Data, Warnings Warnings)
        {
            fileName = FileName;
            this.warnings = Warnings;
            this.data = Data;
            Warnings.ActFile = ShortFileName;
            parser = SrcData;

            EvaluateProgramType();

            ReadToEnd();
        }

        public Program(KeyValuePair<string, string> MemoryItem, Parser SrcData, VariablesContainer Data, Warnings Warnings)
        {
            fileName = MemoryItem.Key;
            this.warnings = Warnings;
            this.data = Data;
            Warnings.ActFile = ShortFileName;
            parser = SrcData;

            EvaluateProgramType();

            ReadToEnd();
        }

        #endregion // constructors

        public bool Filter(object sender)
        {
            try
            {
                Warning x = sender as Warning;
                RobotWarningListModel z = RobotWarningListModel.sharedInstance;
                if (
                    (x.Level == Level.Information && z.LevelInformation) || 
                    (x.Level == Level.Warning && z.LevelWarning) || 
                    (x.Level == Level.Failure && z.LevelFailure)
                    )
                {
                    if (
                        (x.Type == WarningType.UNKNOWN && z.TypeUnknown) || 
                        (x.Type == WarningType.Variables && z.TypeVariables) || 
                        (x.Type == WarningType.Program_Paths && z.TypeProgram_Paths) || 
                        (x.Type == WarningType.Program_Applications && z.TypeProgram_Applications) || 
                        (x.Type == WarningType.Program_Flow && z.TypeProgram_Flow) || 
                        (x.Type == WarningType.OrganizationPrograms && z.TypeOrganizationPrograms) || 
                        (x.Type == WarningType.Intern && z.TypeIntern) || 
                        (x.Type == WarningType.Deep_Intern && z.TypeDeep_Intern))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return true;
            }
        }
      
        public void ReadToEnd()
        {
            if (Parser.Src.Folds != null)
            {
                FilterFolds();
                Parser.Src.Folds.Sort(z => z.LineStart, System.ComponentModel.ListSortDirection.Ascending);
                bool has_movement = false;
                foreach (oldFold fold in Parser.Src.Folds)
                {
                    Application.SetApplication(fold);
                    fold.FindMovement(localVariableList, data);
                    if (fold.Movement != null)
                        has_movement = true;
                }
                if (!Program.should_have_movements & has_movement)
                    Warnings.Add(-1, WarningType.Program_Paths, Level.Warning, "Found at least one movement instruction");
                TestLists();
                CheckOrder();
                if (Program.should_have_movements)
                {
                    CheckNotNeededVariables(Parser.Dat);
                }
                GetBaseInfo();
            }
            WarningView = (ListCollectionView)CollectionViewSource.GetDefaultView(Warnings);
            WarningView.Filter = new Predicate<object>(this.Filter);
            WarningView.GroupDescriptions.Add(new PropertyGroupDescription("FileName"));
            WarningView.GroupDescriptions.Add(new PropertyGroupDescription("Level"));
        }

        public void TestLists()
        {
            Application apps = default(Application);
            apps = new Area(warnings);
            apps.EnumerateAndTest(parser.Src.Folds, baseInfo);
            apps = new CollZone(warnings);
            apps.EnumerateAndTest(parser.Src.Folds, baseInfo);
            apps = new Grp(warnings);
            apps.EnumerateAndTest(parser.Src.Folds, baseInfo);
            apps = new Job(warnings);
            apps.EnumerateAndTest(parser.Src.Folds, baseInfo);
            apps = new PlcCom(warnings);
            apps.EnumerateAndTest(parser.Src.Folds, baseInfo);
            apps = new Swp(warnings);
            apps.EnumerateAndTest(parser.Src.Folds, baseInfo);
            apps = new Tch(warnings);
            apps.EnumerateAndTest(parser.Src.Folds, baseInfo);
        }

        private void GetBaseInfo()
        {
            ProgramBaseInfo bInfo = new ProgramBaseInfo();
            if (BaseInfo.getTypeList(ProgramBaseInfoItemType.Home).Count < 2)
                bInfo.SetOK(ProgramBaseInfoItemType.Home, false);
        }

        private void EvaluateProgramType()
        {
            string lowerName = ShortFileName.ToLowerInvariant();
            if (lowerName == "cell" | lowerName.StartsWith("prog"))
            {
                Program.should_have_movements = false;
            }
            else
            {
                Program.should_have_movements = true;
            }

            // set the program type
            if (lowerName.StartsWith("pick_") || lowerName.StartsWith("pickdrop_") || lowerName.StartsWith("drop_") || lowerName.StartsWith("weldpos_") || lowerName.StartsWith("center_"))
            {
                baseInfo.ProgramType = ProgramType.Handling;
            }
            else if (lowerName.StartsWith("spot_"))
            {
                baseInfo.ProgramType = ProgramType.Spotwelding;
            }
            else if (lowerName.StartsWith("arplas_"))
            {
                baseInfo.ProgramType = ProgramType.Spotwelding;
            }
            else if (lowerName.StartsWith("destack_") || lowerName.StartsWith("stack_"))
            {
                baseInfo.ProgramType = ProgramType.Palletizing;
            }
            else if (lowerName.StartsWith("search_"))
            {
                baseInfo.ProgramType = ProgramType.Searching;
            }
            else if (lowerName.StartsWith("glue_"))
            {
                baseInfo.ProgramType = ProgramType.Glueing;
            }
            else if (lowerName.StartsWith("a01_braketest") || lowerName.StartsWith("a01_masterreference") || lowerName.StartsWith("bmwbraketestpark"))
            {
                baseInfo.ProgramType = ProgramType.Service;
            }
            else if (lowerName.StartsWith("a04_swp_equalcalib_g") || lowerName.StartsWith("a04_swp_gunchange_g"))
            {
                baseInfo.ProgramType = ProgramType.Service;
            }
            else if (lowerName.StartsWith("a04_swp_tipchange_g") || lowerName.StartsWith("a04_swp_tipdress_g"))
            {
                baseInfo.ProgramType = ProgramType.Service;
            }
            else if (lowerName.StartsWith("a04_swp_dockdresser_m") || lowerName.StartsWith("a04_swp_undockdresser_m"))
            {
                baseInfo.ProgramType = ProgramType.Service;
            }
            else if (lowerName.StartsWith("a04_swp_gunmaintenan_g"))
            {
                baseInfo.ProgramType = ProgramType.MaintenancePos;
            }
            else if (lowerName.StartsWith("a03_grp_service_grp") || lowerName.StartsWith("a04_swp_gunservice_g") || lowerName.StartsWith("a04_swp_gunserviceg"))
            {
                baseInfo.ProgramType = ProgramType.Other;
            }
            else if (lowerName.StartsWith("maintenance_clean") || lowerName.StartsWith("a03_grp_maintenan_grp"))
            {
                baseInfo.ProgramType = ProgramType.MaintenancePos;
            }
            else if (lowerName == "archiveinfo" || lowerName == "a03_grp_user" || lowerName == "a03_grp")
            {
                baseInfo.ProgramType = ProgramType.Other;
            }
            else if (lowerName == "a04_swp_user" || lowerName == "a04_swp_global")
            {
                baseInfo.ProgramType = ProgramType.Other;
            }
            else if (lowerName.StartsWith("a08_gl_camera") || lowerName.StartsWith("a08_gl_gluecontrol"))
            {
                baseInfo.ProgramType = ProgramType.Glueing;
            }
            else if (lowerName == "a01_plc_user" || lowerName == "bmwbraketestreq")
            {
                baseInfo.ProgramType = ProgramType.Other;
            }
            else if (lowerName == "a02_tch_user" || lowerName == "a02_tch_global" || lowerName == "a02_tch")
            {
                baseInfo.ProgramType = ProgramType.Other;
            }
            else if (lowerName == "$machine" || lowerName == "$config" || lowerName == "tmf_inbetriebnahme")
            {
                baseInfo.ProgramType = ProgramType.Other;
            }
            else if (lowerName.StartsWith("safetystop") || lowerName == "$en_ident" || lowerName == "$de_ident")
            {
                baseInfo.ProgramType = ProgramType.Other;
            }
            else if (lowerName.StartsWith("abortprog"))
            {
                baseInfo.ProgramType = ProgramType.Other;
            }
            else if (lowerName == "cell" || lowerName.StartsWith("prog"))
            {
                baseInfo.ProgramType = ProgramType.Organization;
            }
            else if (lowerName == "inputdata" || lowerName.StartsWith("fraesdata"))
            {
                baseInfo.ProgramType = ProgramType.UNKNOWN;
            }
            else if (lowerName.StartsWith("base_ini_mess_"))
            {
                baseInfo.ProgramType = ProgramType.Other;
            }
            else
            {
                //Stop
            }
        }

        public override string ToString()
        {
            return ShortFileName;
        }

        #region "Folds"

        public void CheckNotNeededVariables(File.Dat dat)
        {
            List<string> notNeeded = new List<string>();
            string[] variableTypes = new string[] { "E6AXIS", "E6POS", "LDAT", "PDAT", "FDAT" };
            //if (dat != null && dat.variables != null)
            //{
            //    foreach (KeyValuePair<string, Variable> item in dat.variables)
            //    {
            //        if (variableTypes.Contains(item.Value.Type) && !localVariableList.Contains(item.Key.ToUpperInvariant()))
            //        {
            //            notNeeded.Add(item.Key.ToUpperInvariant());
            //        }
            //    }
            //    notNeeded.Remove("SUCCESS");
            //    notNeeded.Remove("LAST_BASIS");
            //    foreach (string item in notNeeded)
            //    {
            //        Warnings.Add(-1, WarningType.Variables, Level.Warning, "Not needed variable in .dat file", item);
            //    }
            //}
        }

        public void CheckOrder()
        {
            bool area_was_requested = false;
            oldFold last_fold = new oldFold(warnings);
            int last_tool = -1;
            int last_base = -1;
            foreach (oldFold fold in parser.Src.Folds)
            {
                int act_tool = 0;
                int act_base = 0;

                if (fold.Application != null)
                {
                    // checking opening and closing of ares/collzones/jobs
                    if (fold.Application is Area)
                    {
                        Area area = (Area) fold.Application;
                        if (area.Request)
                            area_was_requested = true;
                    }
                    else if (fold.Application is CollZone)
                    {
                        CollZone coll = (CollZone) fold.Application;
                        if (coll.Request & !area_was_requested)
                            warnings.Add(fold.LineStart, WarningType.Program_Flow, Level.Warning, "Collision request before area request");
                    }
                    else if (fold.Application is Applications.Job)
                    {
                        Job job = (Job) fold.Application;
                        if (job.JobType == JobType.STARTED & !area_was_requested)
                            warnings.Add(fold.LineStart, WarningType.Program_Flow, Level.Warning, "Job started before area request");
                    }
                }


                string fold_pname = "";
                Movement x = fold.Movement;
                if (x != null)
                {
                    fold_pname = x.PointName;
                    act_tool = x.FDAT.TOOL_NO;
                    act_base = x.FDAT.BASE_NO;

                    if (last_tool == -1)
                        last_tool = act_tool;
                    if (last_base == -1)
                        last_base = act_base;
                    if (last_tool != act_tool)
                        warnings.Add(fold.LineStart, WarningType.Program_Flow, Level.Warning, "ToolNo changed from " + last_tool + " to " + act_tool, fold_pname);
                    if (last_base != act_base)
                        warnings.Add(fold.LineStart, WarningType.Program_Flow, Level.Warning, "BaseNo changed from " + last_base + " to " + act_base, fold_pname);
                    last_tool = act_tool;
                    last_base = act_base;
                }

                if (last_fold.Movement != null & (fold.Movement != null))
                {
                    if (last_fold.Movement.Approx != Enums.Approximate_Positioning.NONE & fold.Movement.Approx == Enums.Approximate_Positioning.NONE)
                    {
                        warnings.Add(fold.LineStart, WarningType.Program_Flow, Level.Warning, "Used CONT in NOMOVE fold after MOVE fold without CONT");
                    }
                    else if (last_fold.Movement.Approx == Enums.Approximate_Positioning.NONE & fold.Movement.Approx != Enums.Approximate_Positioning.NONE)
                    {
                        warnings.Add(fold.LineStart, WarningType.Program_Flow, Level.Warning, "Used NOCONT in NOMOVE fold after MOVE fold with CONT");
                    }
                }
                last_fold = fold;
            }
        }

        public void FilterFolds()
        {
            // remove all unneeded folds
            string[] unneddedList = { };
            string[] neededList = { };
            if (ShortFileName == "cell")
            {
                unneddedList = new List<string>{
					"cellOverAbort",
					"AUTOEXT INI",
					"Init_AfterHome",
					"CHECK HOME",
					"Init_BeforeHome",
					"Set CellStarted",
					"Init Variables from abort",
					"INIT_ABORT",
					"APPLICATION_INI",
					"BASISTECH INI",
					"Moduleparameters",
					";%{"
				}.ToArray();
            }
            else if (ShortFileName.ToLower() == "prog251_braketest")
            {
                unneddedList = new List<string> {
					"Call BRAKTESTREQ",
					"INI",
					"USER INI",
					"APPLICATION_INI",
					"Declaration",
					"BASISTECH INI",
					"IF_PLC_CHK_INIT",
					"ENDIF_PLC_CHK_INIT",
					"Moduleparameters",
					";%{"
				}.ToArray();
            }
            else if (ShortFileName.ToLower().StartsWith("a04_swp_gun_change_g"))
            {
                unneddedList = new List<string> {
					"Moduleparameters",
					"; ",
					";%{",
					"INI",
					"BASISTECH",
					"PLC_CHK_INIT",
					"IF_PLC_CHK_INIT",
					"ENDIF_PLC_CHK_INIT",
					"USER",
					"APPLICATION_INI",
					"WAIT",
					"Declaration",
					"MODE_OP=#T1"
				}.ToArray();
            }
            else
            {
                unneddedList = new List<string> {
					"Moduleparameters",
					"; ",
					";%{",
					"INI",
					"BASISTECH",
					"PLC_CHK_INIT",
					"IF_PLC_CHK_INIT",
					"ENDIF_PLC_CHK_INIT",
					"USER",
					"APPLICATION_INI",
					"WAIT",
					"Declaration"
				}.ToArray();
            }
            neededList = new List<string> {
			    "PTP",
			    "LIN",
			    "Area",
			    "Job",
			    "CollZone",
			    "Grp",
			    "Swp",
			    "Tch",
			    "PlcCom",
			    "Brt",
			    "Rvo",
			    "Gl",
			    "Swh"
		    }.ToArray();

            for (int i = parser.Src.Folds.Count - 1; i >= 0; i += -1)
            {
                bool removed = false;
                oldFold f = parser.Src.Folds[i];
                foreach (string unneeded in unneddedList)
                {
                    if (f.Name.StartsWith(unneeded))
                    {
                        parser.Src.Folds.RemoveAt(i);
                        removed = true;
                        break;
                    }
                }
                if (!removed)
                {
                    bool found = false;
                    foreach (string needed in neededList)
                    {
                        if (f.Name.StartsWith(needed))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        Warnings.Add(f.LineStart, WarningType.Intern, Level.Failure, "Unknown instruction " + f.Name);
                    }
                }
            }
        }
        #endregion


    }
}
