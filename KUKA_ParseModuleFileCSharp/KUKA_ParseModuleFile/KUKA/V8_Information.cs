using ParseModuleFile.KUKA.DataTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.KUKA
{
    public struct SAppl
    {
        public bool A01_Plc;
        public bool A02_Tch;
        public bool A03_Grp;
        public bool A04_Swp;
    }

    public class V8_Information : NotifyPropertyChanged
    {
        #region fields
        private ObservableDictionary<string, object> madaConfig = new ObservableDictionary<string, object>();
        private ObservableDictionary<int, Home> homes = new ObservableDictionary<int, Home>();
        private NumOfHomes numHomes;
        private ObservableDictionary<int, LoadDataPLC> loadsPLC = new ObservableDictionary<int, LoadDataPLC>();
        private ObservableDictionary<int, ObservableDictionary<int, GrpActuator>> grpActs = new ObservableDictionary<int, ObservableDictionary<int, GrpActuator>>();
        private Grippers grippers = new Grippers();
        private SpotProcess swp = new SpotProcess();
        private TchData tch;
        private LOAD lOAD_A1_DATA;
        private LOAD lOAD_A2_DATA;
        private LOAD lOAD_A3_DATA;
        private ObservableDictionary<int, Base> bases = new ObservableDictionary<int, Base>();
        private ObservableDictionary<int, Tool> tools = new ObservableDictionary<int, Tool>();
        private SAppl _appl;
        #endregion // fields

        #region properties
        public ObservableDictionary<string, object> MadaConfig { get { return madaConfig; } set { Set(ref madaConfig, value); } }
        public ObservableDictionary<int, Home> Homes { get { return homes; } set { Set(ref homes, value); } }
        public NumOfHomes NumHomes { get { return numHomes; } set { Set(ref numHomes, value); } }
        public ObservableDictionary<int, LoadDataPLC> LoadsPLC { get { return loadsPLC; } set { Set(ref loadsPLC, value); } }
        public ObservableDictionary<int, ObservableDictionary<int, GrpActuator>> GrpActs { get { return grpActs; } set { Set(ref grpActs, value); } }
        public Grippers Grippers { get { return grippers; } set { Set(ref grippers, value); } }
        public SpotProcess Swp { get { return swp; } set { Set(ref swp, value); } }
        public TchData Tch { get { return tch; } set { Set(ref tch, value); } }
        public LOAD LOAD_A1_DATA { get { return lOAD_A1_DATA; } set { Set(ref lOAD_A1_DATA, value); } }
        public LOAD LOAD_A2_DATA { get { return lOAD_A2_DATA; } set { Set(ref lOAD_A2_DATA, value); } }
        public LOAD LOAD_A3_DATA { get { return lOAD_A3_DATA; } set { Set(ref lOAD_A3_DATA, value); } }
        public ObservableDictionary<int, Base> Bases { get { return bases; } set { Set(ref bases, value); } }
        public ObservableDictionary<int, Tool> Tools { get { return tools; } set { Set(ref tools, value); } }
        public SAppl Appl { get { return _appl; } }
        #endregion // properties

        #region "Get contents from dat files"
        //public void PrepareDatFiles(Program prg, string file)
        //{
        //    string ext = Path.GetExtension(file).ToLowerInvariant();
        //    if (!ext.Equals(".dat"))
        //        return;
        //    switch (Path.GetFileNameWithoutExtension(file).ToLower())
        //    {
        //        case "$config":
        //            _appl.A01_Plc = true;
        //            DatConfig(prg);
        //            break;
        //        case "$machine":
        //            _appl.A01_Plc = true;
        //            DatMashine(prg);
        //            break;
        //        case "a01_plc_user":
        //            _appl.A01_Plc = true;
        //            DatPlcUser(prg);
        //            break;
        //        case "a02_tch_user":
        //            _appl.A02_Tch = true;
        //            DatTchUser(prg);
        //            break;
        //        case "a02_tch":
        //            _appl.A02_Tch = true;
        //            break;
        //        case "a02_tch_global":
        //            _appl.A02_Tch = true;
        //            DatGlobal(prg, Path.GetFileName(file));
        //            break;
        //        case "a03_grp":
        //            _appl.A03_Grp = true;
        //            DatGrp(prg);
        //            break;
        //        case "a03_grp_user":
        //            _appl.A03_Grp = true;
        //            DatGrpUser(prg);
        //            break;
        //        case "a04_swp_global":
        //            _appl.A04_Swp = true;
        //            DatGlobal(prg, Path.GetFileName(file));
        //            break;
        //        case "a04_swp_user":
        //            _appl.A04_Swp = true;
        //            DatSwpUser(prg);
        //            break;
        //        default:
        //            string afile = Path.GetFileNameWithoutExtension(file).ToLower();
        //            break;
        //        //Throw New NotImplementedException
        //    }
        //}

        //private void DatMashine(Program prg)
        //{
        //    if (prg.Parser.Dat.variables == null)
        //    {
        //        prg.Warnings.Add(-1, WarningType.Variables, Level.Failure, "No variables found in $mashine.dat");
        //        return;
        //    }
        //    string[] neededvars = new string[] {
        //    "$EX_AX_NUM",
        //    "$ET1_TA1KR",
        //    "$ET1_TFLA3",
        //    "$RAT_MOT_AX",
        //    "$ET1_NAME"
        //};
        //    foreach (string item in neededvars)
        //    {
        //        if (prg.Parser.Dat.variables.ContainsKey(item))
        //        {
        //            if (prg.Parser.Dat.variables[item].IsArray)
        //            {
        //                if (prg.Parser.Dat.variables[item].Type == "FRA")
        //                {
        //                    int i = 1;
        //                    foreach (DynamicMemory am in prg.Parser.Dat.variables[item].ListOfValue)
        //                    {
        //                        MadaConfig.Add(item + "[" + i.ToString() + "]", new FRA(am, item + "[" + i.ToString() + "]"));
        //                        i += 1;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                DynamicMemory am = prg.Parser.Dat.variables[item].Value;
        //                switch (prg.Parser.Dat.variables[item].Type)
        //                {
        //                    case "INT":
        //                        MadaConfig.Add(item, Convert.ToInt32(am["val"]));
        //                        break;
        //                    case "FRAME":
        //                        MadaConfig.Add(item, new FRAME(am, item));
        //                        break;
        //                    case "CHAR":
        //                        MadaConfig.Add(item + "[]", Convert.ToString(am["val"]));
        //                        break;
        //                    default:
        //                        System.Diagnostics.Debugger.Break();
        //                        break;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            prg.Warnings.Add(-1, WarningType.Variables, Level.Failure, "Did not found variable " + item + " in $mashine.dat");
        //        }
        //    }
        //}

        //private void DatConfig(Program prg)
        //{
        //    if (prg.Parser.Dat.variables == null)
        //    {
        //        prg.Warnings.Add(-1, WarningType.Variables, Level.Failure, "No variables found in $config.dat");
        //        return;
        //    }
        //    for (int i = 1; i <= 128; i++)
        //    {
        //        tools.Add(i, new Tool(i));
        //        bases.Add(i, new Base(i));
        //    }
        //    foreach (KeyValuePair<string, Variable> var in prg.Parser.Dat.variables)
        //    {
        //        DynamicMemory am = var.Value.Value;
        //        if (var.Value.Type == "E6AXIS")
        //        {
        //            Program.GlobalE6Axis.Add(var.Key, new E6AXIS(am));
        //        }
        //        else if (var.Value.Type == "FRAME")
        //        {
        //            if (var.Key == "TOOL_DATA")
        //            {
        //                for (int i = 1; i <= 128; i++)
        //                {
        //                    Tool b = tools[i];
        //                    b.Frame = new FRAME(var.Value[i]);
        //                    tools[i] = b;
        //                }
        //            }
        //            else if (var.Key == "BASE_DATA")
        //            {
        //                for (int i = 1; i <= 128; i++)
        //                {
        //                    Base b = bases[i];
        //                    b.Frame = new FRAME(var.Value[i]);
        //                    bases[i] = b;
        //                }
        //            }
        //        }
        //        else if (var.Value.Type == "LOAD")
        //        {
        //            if (var.Key == "LOAD_DATA")
        //            {
        //                for (int i = 1; i <= 128; i++)
        //                {
        //                    Tool b = tools[i];
        //                    b.Load = new LOAD(var.Value[i]);
        //                    tools[i] = b;
        //                }
        //            }
        //            else if (var.Key == "LOAD_A1_DATA")
        //            {
        //                lOAD_A1_DATA = new LOAD(var.Value.Value);
        //            }
        //            else if (var.Key == "LOAD_A2_DATA")
        //            {
        //                lOAD_A2_DATA = new LOAD(var.Value.Value);
        //            }
        //            else if (var.Key == "LOAD_A3_DATA")
        //            {
        //                lOAD_A3_DATA = new LOAD(var.Value.Value);
        //            }
        //        }
        //        else if (var.Value.Type == "CHAR")
        //        {
        //            if (var.Key == "TOOL_NAME")
        //            {
        //                for (int i = 1; i <= 128; i++)
        //                {
        //                    Tool b = tools[i];
        //                    b.Description = (string) var.Value[i]["val"];
        //                    tools[i] = b;
        //                }
        //            }
        //            else if (var.Key == "BASE_NAME")
        //            {
        //                for (int i = 1; i <= 128; i++)
        //                {
        //                    Base b = bases[i];
        //                    b.Description = (string) var.Value[i]["val"];
        //                    bases[i] = b;
        //                }
        //            }
        //        }
        //        else if (var.Value.Type == "IPO_M_T")
        //        {
        //            if (var.Key == "BASE_TYPE")
        //            {
        //                for (int i = 1; i <= 128; i++)
        //                {
        //                    Base b = bases[i];
        //                    b.Type = (IPO_M_T) var.Value[i]["val"];
        //                    bases[i] = b;
        //                }
        //            }
        //            else if (var.Key == "TOOL_TYPE")
        //            {
        //                for (int i = 1; i <= 128; i++)
        //                {
        //                    Tool b = tools[i];
        //                    b.Type = (IPO_M_T) var.Value[i]["val"];
        //                    tools[i] = b;
        //                }
        //            }
        //        }
        //    }
        //}

        //private void DatGlobal(Program prg, string filename)
        //{
        //    if (prg.Parser.Dat.variables == null)
        //    {
        //        prg.Warnings.Add(-1, WarningType.Variables, Level.Failure, "No variables found in " + filename);
        //        return;
        //    }
        //    foreach (KeyValuePair<string, Variable> var in prg.Parser.Dat.variables)
        //    {
        //        DynamicMemory am = var.Value.Value;
        //        if (var.Value.Type == "E6POS")
        //        {
        //            Program.GlobalE6pos.Add(var.Key, new E6POS(am));
        //        }
        //        else if (var.Value.Type == "E6AXIS")
        //        {
        //            Program.GlobalE6Axis.Add(var.Key, new E6AXIS(am));
        //        }
        //        else if (var.Value.Type == "PDAT")
        //        {
        //            Program.GlobalPDat.Add(var.Key, new PDAT(am));
        //        }
        //        else if (var.Value.Type == "FDAT")
        //        {
        //            Program.GlobalFDat.Add(var.Key, new FDAT(am));
        //        }
        //        else if (var.Value.Type == "LDAT")
        //        {
        //            Program.GlobalLDat.Add(var.Key, new LDAT(am));
        //        }
        //        else
        //        {
        //            prg.Warnings.Add(-1, WarningType.Variables, Level.Failure, "Variable of type " + var.Value.Type + " should not be in " + filename);
        //        }
        //    }
        //}

        //private void DatGrpUser(Program prg)
        //{
        //    if (prg.Parser.Dat.variables == null)
        //    {
        //        prg.Warnings.Add(-1, WarningType.Variables, Level.Failure, "No variables found in a03_grp_user.dat");
        //        return;
        //    }
        //    // actuators
        //    if (!prg.Parser.Dat.variables.ContainsKey("ACT"))
        //    {
        //        prg.Warnings.Add(-1, WarningType.Variables, Level.Failure, "Not found Act in a03_grp_user.dat");
        //    }
        //    else
        //    {
        //        grpActs = new ObservableDictionary<int, ObservableDictionary<int, GrpActuator>>();
        //        for (int i = 1; i <= 15; i++)
        //        {
        //            ObservableDictionary<int, GrpActuator> actGrpAct = new ObservableDictionary<int, GrpActuator>();
        //            for (int j = 1; j <= 16; j++)
        //            {
        //                DynamicMemory am = prg.Parser.Dat.variables["ACT"][i, j];
        //                actGrpAct.Add(j, new GrpActuator(am));
        //            }
        //            grpActs.Add(i, actGrpAct);
        //        }
        //    }
        //    // pp's
        //    if (!prg.Parser.Dat.variables.ContainsKey("GRIPPER"))
        //    {
        //        prg.Warnings.Add(-1, WarningType.Variables, Level.Failure, "Not found Gripper in a03_grp_user.dat");
        //    }
        //    else
        //    {
        //        grippers.Clear();
        //        for (int i = 1; i <= 15; i++)
        //        {
        //            DynamicMemory am = prg.Parser.Dat.variables["GRIPPER"][i];
        //            grippers.Add(new GripStruc(am));
        //        }
        //    }
        //}

        //private void DatGrp(Program prg)
        //{
        //    // nothing here yet
        //    return;
        //}

        //private void DatPlcUser(Program prg)
        //{
        //    if (prg.Parser.Dat.variables == null)
        //    {
        //        prg.Warnings.Add(-1, WarningType.Variables, Level.Failure, "No variables found in a01_plc_user.dat");
        //        return;
        //    }
        //    // homes
        //    if (!prg.Parser.Dat.variables.ContainsKey("PLC_ACTIVE_HOMES"))
        //    {
        //        prg.Warnings.Add(-1, WarningType.Variables, Level.Failure, "Not found PLC_ACTIVE_HOMES in a01_plc_user.dat");
        //    }
        //    else
        //    {
        //        //numHomes = new NumOfHomes(prg.Parser.dat.variables["PLC_ACTIVE_HOMES"].Value);
        //        homes = new ObservableDictionary<int, Home>();
        //        for (int i = 1; i <= 5; i++)
        //        {
        //            if (!prg.Parser.Dat.variables.ContainsKey(("plc_ch_Home" + i.ToString() + "Name").ToUpperInvariant()))
        //            {
        //                prg.Warnings.Add(-1, WarningType.Variables, Level.Failure, "Not found plc_ch_Home" + i.ToString() + "Name in a01_plc_user.dat");
        //            }
        //            else
        //            {
        //                //homes.Add(i, new Home(i, prg.Parser.dat.variables[("plc_ch_Home" + i.ToString() + "Name").ToUpperInvariant()].Value));
        //            }
        //        }
        //    }
        //    loadsPLC = new ObservableDictionary<int, LoadDataPLC>();
        //    for (int i = 1; i <= 128; i++)
        //    {
        //        if (!prg.Parser.Dat.variables.ContainsKey("LOAD_VAR_NAME" + i.ToString()))
        //        {
        //            prg.Warnings.Add(-1, WarningType.Variables, Level.Failure, "Not found LOAD_VAR_NAME" + i.ToString() + " in a01_plc_user.dat");
        //        }
        //        else
        //        {
        //            LoadDataPLC actload = new LoadDataPLC(prg.Parser.Dat.variables["LOAD_VAR_NAME" + i.ToString()].Value);
        //            //actload.Num = i;
        //            if (!prg.Parser.Dat.variables.ContainsKey("LOAD_VAR"))
        //            {
        //                prg.Warnings.Add(-1, WarningType.Variables, Level.Failure, "Not found LOAD_VAR in a01_plc_user.dat");
        //            }
        //            else
        //            {
        //                DynamicMemory am = prg.Parser.Dat.variables["LOAD_VAR"][i];
        //                actload.Load = new LOAD(prg.Parser.Dat.variables["LOAD_VAR"][i]);
        //            }
        //            loadsPLC.Add(i, actload);
        //        }
        //    }
        //    // load_saves
        //    // loads
        //}

        //private void DatTchUser(Program prg)
        //{
        //    if (prg.Parser.Dat.variables == null)
        //    {
        //        prg.Warnings.Add(-1, WarningType.Variables, Level.Failure, "No variables found in a02_tch_user.dat");
        //        return;
        //    }
        //    TchData Tch = new TchData();
        //    Tch.Stations = new ObservableDictionary<int, object>();
        //    Tch.Tools = new ObservableDictionary<int, object>();

        //    Tch.Tch_I_MaxStation = (int) ValFromVariable(prg, "tch_I_MaxStation");
        //    Tch.Tch_I_tchToolMax = (int)ValFromVariable(prg, "tch_I_tchToolMax");
        //    Tch.Tch_I_MaxExtStation = (int)ValFromVariable(prg, "tch_I_MaxExtStation");
        //    Tch.Tch_i_tchToolNo = (int)ValFromVariable(prg, "tch_i_tchToolNo");
        //    TchStation.tch_I_MaxStation = Tch.Tch_I_MaxStation;
        //    TchStation.tch_I_MaxExtStation = Tch.Tch_I_MaxExtStation;
        //    TchTool_OptStation.tch_I_MaxStation = Tch.Tch_I_MaxStation;
        //    TchTool_OptStation.tch_I_MaxExtStation = Tch.Tch_I_MaxExtStation;
        //    TchTool.tch_I_tchToolMax = Tch.Tch_I_tchToolMax;

        //    for (int i = 1; i <= 23; i++)
        //    {
        //        TchStation station = new TchStation(i);
        //        station.Num = i;
        //        station.Tch_b_Cover_present = (bool) ValFromVariable(prg, "tch_b_Cover_present", i);
        //        station.Tch_i_Station_BASE = (int) ValFromVariable(prg, "tch_i_Station_BASE", i);
        //        station.Tch_i_TimeOutCover = (int) ValFromVariable(prg, "tch_i_TimeOutCover", i);
        //        station.Tch_i_rel_Dis_Undocked = (int) ValFromVariable(prg, "tch_i_rel_Dis_Undocked", i);
        //        station.Tch_i_rel_Dis_Docked = (int) ValFromVariable(prg, "tch_i_rel_Dis_Docked", i);
        //        station.Tch_i_rel_Dis_Sensor = (int) ValFromVariable(prg, "tch_i_rel_Dis_Sensor", i);
        //        Tch.Stations.Add(i, station);
        //    }

        //    for (int i = 1; i <= 15; i++)
        //    {
        //        TchTool tool = new TchTool(i);
        //        tool.Num = i;
        //        tool.Tch_I_TOOL_DATA = (int) ValFromVariable(prg, "tch_I_TOOL_DATA", i);
        //        tool.Tch_i_After_Lock_Time = (int) ValFromVariable(prg, "tch_i_After_Lock_Time", i);
        //        tool.Tch_i_PN_Dev_count = (int) ValFromVariable(prg, "tch_i_PN_Dev_count", i);
        //        TchTool_PNDevNo.tch_i_PN_Dev_count = tool.Tch_i_PN_Dev_count;

        //        ObservableDictionary<int, TchTool_OptStation> optStation = new ObservableDictionary<int, TchTool_OptStation>();
        //        for (int j = 1; j <= 23; j++)
        //        {
        //            TchTool_OptStation @bool = new TchTool_OptStation(j);
        //            @bool.Value = (bool) ValFromVariable(prg, "tch_b_option_station", i, j);
        //            optStation.Add(j, @bool);
        //        }
        //        tool.Tch_b_option_station = optStation;

        //        ObservableDictionary<int, TchTool_PNDevNo> devNo = new ObservableDictionary<int, TchTool_PNDevNo>();
        //        for (int j = 1; j <= 64; j++)
        //        {
        //            TchTool_PNDevNo @int = new TchTool_PNDevNo(i, j);
        //            @int.PNDevNo = (int) ValFromVariable(prg, "tch_i_PN_DevNo", i, j);
        //            devNo.Add(j, @int);
        //        }
        //        tool.Tch_i_PN_DevNo = devNo;

        //        Tch.Tools.Add(i, tool);
        //    }
        //    tch = Tch;
        //}

        //private void DatSwpUser(Program prg)
        //{
        //    if (prg.Parser.Dat.variables == null)
        //    {
        //        prg.Warnings.Add(-1, WarningType.Variables, Level.Failure, "No variables found in a04_swp_user.dat");
        //        return;
        //    }
        //    SpotProcess Swp = new SpotProcess();
        //    Swp.Guns = new ObservableDictionary<int, Gun>();

        //    Swp.MaxNoGuns = (int) ValFromVariable(prg, "swp_i_MaxNoGuns");
        //    Swp.MaxNoWeldTimer = (int) ValFromVariable(prg, "swp_i_MaxNoWeldTimer");
        //    Swp.MaxNoWeldTimer = (int) ValFromVariable(prg, "swp_i_MaxNoDresser", Swp.MaxNoDresser);
        //    Gun.maxNoGuns = Swp.MaxNoGuns;
        //    TIPDRESS.maxNoDresser = Swp.MaxNoDresser;

        //    for (int i = 1; i <= 14; i++)
        //    {
        //        Gun gun = new Gun(i);
        //        ObservableDictionary<int, TIPDRESS> dress_pars = new ObservableDictionary<int, TIPDRESS>();
        //        gun.MountingWeld = (swp_GUN_MOUNTING) ValFromVariable(prg, "swp_enm_MountingWeld_G", i);
        //        gun.WeldTimerContr = (int) ValFromVariable(prg, "swp_i_TimerControl_G", i);
        //        gun.ElectrodeNo = (int) ValFromVariable(prg, "swp_i_ElectrodeNo_G", i);
        //        gun.FastCloseOpt = (swp_OPTION_CTL) ValFromVariable(prg, "swp_enm_FastCloseOpt_G", i);
        //        gun.FastLeaveOpt = (swp_OPTION_CTL) ValFromVariable(prg, "swp_enm_FastLeaveOpt_G", i);

        //        gun.ManMvSTEP0 = (double)ValFromVariable(prg, "swp_r_manMvSTEP0", i);
        //        gun.ManMvSTEP1 = (double)ValFromVariable(prg, "swp_r_manMvSTEP1", i);
        //        gun.ManMvSTEP2 = (double)ValFromVariable(prg, "swp_r_manMvSTEP2", i);

        //        gun.EqualBack = (bool) ValFromVariable(prg, "swp_b_EqualBack_G", i);
        //        gun.EqualBackFl = (bool) ValFromVariable(prg, "swp_b_EqualBackFl_G", i);
        //        gun.LastOpenM = (int) ValFromVariable(prg, "swp_i_LastOpenM_G", i);
        //        gun.LastOpenM = (double) ValFromVariable(prg, "swp_r_MinOpenM_G", i);
        //        gun.MaxOpenM = (double) ValFromVariable(prg, "swp_r_MaxOpenM_G", i);
        //        gun.HomeEqualPr = (double) ValFromVariable(prg, "swp_r_HomeEqualPr_G", i);

        //        gun.TipDresserContr = (int) ValFromVariable(prg, "swp_i_TipDresserContr", i);
        //        gun.TipDrRelayState = (int) ValFromVariable(prg, "swp_i_tipDrRelayState", i);

        //        gun.TipDressListOption = (bool) ValFromVariable(prg, "swp_b_TipDressListOption", i);
        //        gun.DressCounter = (int) ValFromVariable(prg, "swp_i_TipDressCounter", i);
        //        gun.DressCounterMax = (int) ValFromVariable(prg, "swp_i_DressCounterMax", i);
        //        gun.PolishWOpen = (bool) ValFromVariable(prg, "swp_b_PolishWopen", i);
        //        gun.PolishOption = (bool) ValFromVariable(prg, "swp_b_PolishOption", i);
        //        gun.PolishTime = (double) ValFromVariable(prg, "swp_r_PolishTime", i);
        //        gun.PolishForce = (double) ValFromVariable(prg, "swp_r_PolishForce", i);
        //        gun.GunOpenTime = (double) ValFromVariable(prg, "swp_r_GunOpenTime", i);
        //        for (int j = 1; j <= 30; j++)
        //        {
        //            TIPDRESS dresspar = new TIPDRESS(j);
        //            dresspar.DressTime = (double) ValFromVariable(prg, "swp_strc_DressVar", i, j, "DressTime");
        //            dresspar.DressForce = (int) ValFromVariable(prg, "swp_strc_DressVar", i, j, "DressForce");
        //            dresspar.Interval = (int) ValFromVariable(prg, "swp_strc_DressVar", i, j, "Interval");
        //            dress_pars.Add(j, dresspar);
        //        }

        //        gun.CalEqPressure_min = (double) ValFromVariable(prg, "swp_calEqPressure", i, 0, "min");
        //        gun.CalEqPressure_max = (double) ValFromVariable(prg, "swp_calEqPressure", i, 0, "max");
        //        gun.CalEqPressure_stat = (double) ValFromVariable(prg, "swp_calEqPressure", i, 0, "stat");
        //        gun.DressVar = dress_pars;

        //        Swp.Guns.Add(i, gun);
        //    }

        //    swp = Swp;
        //}
        #endregion

        #region methods
        //private object ValFromVariable(Program prg, string name, int dim1 = 0, int dim2 = 0, string item = "val")
        //{
        //    name = name.ToUpperInvariant();
        //    if (!prg.Parser.Dat.variables.ContainsKey(name))
        //    {
        //        prg.Warnings.Add(-1, WarningType.Variables, Level.Failure, "Not found " + name + " in " + Path.GetFileName(prg.Parser.Dat.FileName));
        //        return null;
        //    }
        //    else
        //    {
        //        if (dim2 != 0)
        //        {
        //            return prg.Parser.Dat.variables[name][dim1, dim2][item];
        //        }
        //        else if (dim1 != 0)
        //        {
        //            return prg.Parser.Dat.variables[name][dim1][item];
        //        }
        //        else
        //        {
        //            return prg.Parser.Dat.variables[name].Value[item];
        //        }
        //    }
        //}
        #endregion // methods
    }
}
