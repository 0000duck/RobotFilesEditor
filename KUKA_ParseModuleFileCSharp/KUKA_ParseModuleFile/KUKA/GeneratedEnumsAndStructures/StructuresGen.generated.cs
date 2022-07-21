namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class MemorySelector
	{
		public static Variable Get(string type, ANTLR.DataItems dataItems)
		{
			switch(type)
			{
				case "IPO_M_T":
					return new EnumDataType<Enums.IPO_M_T>("IPO_M_T", dataItems);
				case "IPO_MODE":
					return new EnumDataType<Enums.IPO_MODE>("IPO_MODE", dataItems);
				case "APO_MODE_T":
					return new EnumDataType<Enums.APO_MODE_T>("APO_MODE_T", dataItems);
				case "GrpACT_Type":
					return new EnumDataType<Enums.GrpACT_Type>("GrpACT_Type", dataItems);
				case "Grp_STATE":
					return new EnumDataType<Enums.Grp_STATE>("Grp_STATE", dataItems);
				case "Grp_PP_STATE":
					return new EnumDataType<Enums.Grp_PP_STATE>("Grp_PP_STATE", dataItems);
				case "swp_GUN_MOUNTING":
					return new EnumDataType<Enums.swp_GUN_MOUNTING>("swp_GUN_MOUNTING", dataItems);
				case "tch_enum_pnio":
					return new EnumDataType<Enums.tch_enum_pnio>("tch_enum_pnio", dataItems);
				case "tch_enum_tchTool":
					return new EnumDataType<Enums.tch_enum_tchTool>("tch_enum_tchTool", dataItems);
				case "ESYS":
					return new EnumDataType<Enums.ESYS>("ESYS", dataItems);
				case "CIRC_MODE_args":
					return new EnumDataType<Enums.CIRC_MODE_args>("CIRC_MODE_args", dataItems);
				case "CIRC_TYPE":
					return new EnumDataType<Enums.CIRC_TYPE>("CIRC_TYPE", dataItems);
				case "MECH_TYPE":
					return new EnumDataType<Enums.MECH_TYPE>("MECH_TYPE", dataItems);
				case "ORI_TYPE":
					return new EnumDataType<Enums.ORI_TYPE>("ORI_TYPE", dataItems);
				case "OUT_MODETYPE":
					return new EnumDataType<Enums.OUT_MODETYPE>("OUT_MODETYPE", dataItems);
				case "INDIVIDUAL_MAMES":
					return new EnumDataType<Enums.INDIVIDUAL_MAMES>("INDIVIDUAL_MAMES", dataItems);
				case "MODE":
					return new EnumDataType<Enums.MODE>("MODE", dataItems);
				case "VAR_STATE":
					return new EnumDataType<Enums.VAR_STATE>("VAR_STATE", dataItems);
				case "CP_VEL_TYPE":
					return new EnumDataType<Enums.CP_VEL_TYPE>("CP_VEL_TYPE", dataItems);
				case "OPTION_CTL":
					return new EnumDataType<Enums.OPTION_CTL>("OPTION_CTL", dataItems);
				case "SSE_Cmd":
					return new EnumDataType<Enums.SSE_Cmd>("SSE_Cmd", dataItems);
				case "SSE_Mode":
					return new EnumDataType<Enums.SSE_Mode>("SSE_Mode", dataItems);
				case "Pal_Func":
					return new EnumDataType<Enums.Pal_Func>("Pal_Func", dataItems);
				case "Pal_Check":
					return new EnumDataType<Enums.Pal_Check>("Pal_Check", dataItems);
				case "REFERENCE":
					return new EnumDataType<Enums.REFERENCE>("REFERENCE", dataItems);
				case "CONTROL_PARAMETER":
					return new EnumDataType<Enums.CONTROL_PARAMETER>("CONTROL_PARAMETER", dataItems);
				case "AXIS_OF_COORDINATES":
					return new EnumDataType<Enums.AXIS_OF_COORDINATES>("AXIS_OF_COORDINATES", dataItems);
				case "SPLINE_PARA_VARIANT":
					return new EnumDataType<Enums.SPLINE_PARA_VARIANT>("SPLINE_PARA_VARIANT", dataItems);
				case "PARITY":
					return new EnumDataType<Enums.PARITY>("PARITY", dataItems);
				case "LOADREACTION":
					return new EnumDataType<Enums.LOADREACTION>("LOADREACTION", dataItems);
				case "TARGET_STATUS":
					return new EnumDataType<Enums.TARGET_STATUS>("TARGET_STATUS", dataItems);
				case "CP_STATMON":
					return new EnumDataType<Enums.CP_STATMON>("CP_STATMON", dataItems);
				case "COOP_UPDATE_T":
					return new EnumDataType<Enums.COOP_UPDATE_T>("COOP_UPDATE_T", dataItems);
				case "CAUSE_T":
					return new EnumDataType<Enums.CAUSE_T>("CAUSE_T", dataItems);
				case "KRLMSGPARTYPE_T":
					return new EnumDataType<Enums.KRLMSGPARTYPE_T>("KRLMSGPARTYPE_T", dataItems);
				case "SUPPLY_VOLTAGE":
					return new EnumDataType<Enums.SUPPLY_VOLTAGE>("SUPPLY_VOLTAGE", dataItems);
				case "KINCLASS":
					return new EnumDataType<Enums.KINCLASS>("KINCLASS", dataItems);
				case "MAIN_AXIS":
					return new EnumDataType<Enums.MAIN_AXIS>("MAIN_AXIS", dataItems);
				case "WRIST_AXIS":
					return new EnumDataType<Enums.WRIST_AXIS>("WRIST_AXIS", dataItems);
				case "EX_KIN_E":
					return new EnumDataType<Enums.EX_KIN_E>("EX_KIN_E", dataItems);
				case "ET_AX_E":
					return new EnumDataType<Enums.ET_AX_E>("ET_AX_E", dataItems);
				case "Approximate_Positioning":
					return new EnumDataType<Enums.Approximate_Positioning>("Approximate_Positioning", dataItems);
				case "GrpAction":
					return new EnumDataType<Enums.GrpAction>("GrpAction", dataItems);
				case "PlcComAction":
					return new EnumDataType<Enums.PlcComAction>("PlcComAction", dataItems);
				case "SwpAction":
					return new EnumDataType<Enums.SwpAction>("SwpAction", dataItems);
				case "JobType":
					return new EnumDataType<Enums.JobType>("JobType", dataItems);
				case "OLPType":
					return new EnumDataType<Enums.OLPType>("OLPType", dataItems);
				case "ProgramBaseInfoItemType":
					return new EnumDataType<Enums.ProgramBaseInfoItemType>("ProgramBaseInfoItemType", dataItems);
				case "ProgramType":
					return new EnumDataType<Enums.ProgramType>("ProgramType", dataItems);
				case "EAttrType":
					return new EnumDataType<Enums.EAttrType>("EAttrType", dataItems);
				case "ADAP_ACC":
					return new EnumDataType<Enums.ADAP_ACC>("ADAP_ACC", dataItems);
				case "MODEL_TYPE":
					return new EnumDataType<Enums.MODEL_TYPE>("MODEL_TYPE", dataItems);
				case "EKO_MODE":
					return new EnumDataType<Enums.EKO_MODE>("EKO_MODE", dataItems);
				case "INT":
					return new INT(dataItems);
				case "REAL":
					return new REAL(dataItems);
				case "BOOL":
					return new BOOL(dataItems);
				case "CHAR":
					return new CHAR(dataItems);
				case "DATE":
					return new DATE(dataItems);
				case "BASIS_SUGG_T":
					return new BASIS_SUGG_T(dataItems);
				case "GrpActuator":
					return new GrpActuator(dataItems);
				case "GripStruc":
					return new GripStruc(dataItems);
				case "GrpOutFlags":
					return new GrpOutFlags(dataItems);
				case "swp_limits":
					return new swp_limits(dataItems);
				case "SSE_Struc":
					return new SSE_Struc(dataItems);
				case "Palletizing_Struc":
					return new Palletizing_Struc(dataItems);
				case "CIRC_MODE_items":
					return new CIRC_MODE_items(dataItems);
				case "CIRC_MODE":
					return new CIRC_MODE(dataItems);
				case "PDAT":
					return new PDAT(dataItems);
				case "LDAT":
					return new LDAT(dataItems);
				case "FDAT":
					return new FDAT(dataItems);
				case "ODAT":
					return new ODAT(dataItems);
				case "E6AXIS":
					return new E6AXIS(dataItems);
				case "E6POS":
					return new E6POS(dataItems);
				case "FRAME":
					return new FRAME(dataItems);
				case "POS":
					return new POS(dataItems);
				case "FRA":
					return new FRA(dataItems);
				case "LOAD":
					return new LOAD(dataItems);
				case "TRIGGER_PARA":
					return new TRIGGER_PARA(dataItems);
				case "CONSTVEL_PARA":
					return new CONSTVEL_PARA(dataItems);
				case "CONDSTOP_PARA":
					return new CONDSTOP_PARA(dataItems);
				case "ADAT":
					return new ADAT(dataItems);
				case "SPS_PROG_TYPE":
					return new SPS_PROG_TYPE(dataItems);
				case "ACC_CAR":
					return new ACC_CAR(dataItems);
				case "CP":
					return new CP(dataItems);
				case "AXBOX":
					return new AXBOX(dataItems);
				case "CYLINDER":
					return new CYLINDER(dataItems);
				case "MACHINE_DEF_T":
					return new MACHINE_DEF_T(dataItems);
				case "MACHINE_TOOL_T":
					return new MACHINE_TOOL_T(dataItems);
				case "MACHINE_FRAME_T":
					return new MACHINE_FRAME_T(dataItems);
				case "DHART":
					return new DHART(dataItems);
				case "SPIN":
					return new SPIN(dataItems);
				case "TRPSPIN":
					return new TRPSPIN(dataItems);
				case "EX_KIN":
					return new EX_KIN(dataItems);
				case "ET_AX":
					return new ET_AX(dataItems);
				case "MAXTOOL":
					return new MAXTOOL(dataItems);
				case "INERTIA":
					return new INERTIA(dataItems);
				case "PRESET":
					return new PRESET(dataItems);
				case "ERR_MESS":
					return new ERR_MESS(dataItems);
				case "MODULEPARAM_T":
					return new MODULEPARAM_T(dataItems);
				case "monitoring":
					return new monitoring(dataItems);
				case "JERK_STRUC":
					return new JERK_STRUC(dataItems);
				case "TQM_TQDAT_T":
					return new TQM_TQDAT_T(dataItems);
				case "_calEqPressure":
					return new _calEqPressure(dataItems);
				case "TIPDRESS":
					return new TIPDRESS(dataItems);
				case "PRO_IO_T":
					return new PRO_IO_T(dataItems);
				case "SER":
					return new SER(dataItems);
				case "EXT_MOD_T":
					return new EXT_MOD_T(dataItems);
				case "BOX":
					return new BOX(dataItems);
				case "COOP_KRC":
					return new COOP_KRC(dataItems);
				case "WS_CONFIG":
					return new WS_CONFIG(dataItems);
				case "BIN_TYPE":
					return new BIN_TYPE(dataItems);
				case "LDC_REACTION":
					return new LDC_REACTION(dataItems);
				case "EMSTOP_PATH":
					return new EMSTOP_PATH(dataItems);
				case "STOPMESS":
					return new STOPMESS(dataItems);
				case "KRLMSGOPT_T":
					return new KRLMSGOPT_T(dataItems);
				case "KrlInputStruct":
					return new KrlInputStruct(dataItems);
				case "KRLMSGPAR_T":
					return new KRLMSGPAR_T(dataItems);
				case "TchData":
					return new TchData(dataItems);
				case "Gun":
					return new Gun(dataItems);
				case "SpotProcess":
					return new SpotProcess(dataItems);
				case "Home":
					return new Home(dataItems);
				case "LoadDataPLC":
					return new LoadDataPLC(dataItems);
				case "Base":
					return new Base(dataItems);
				case "Tool":
					return new Tool(dataItems);
				case "TchStation":
					return new TchStation(dataItems);
				case "TchTool_OptStation":
					return new TchTool_OptStation(dataItems);
				case "TchTool":
					return new TchTool(dataItems);
				case "TchTool_PNDevNo":
					return new TchTool_PNDevNo(dataItems);
				default:
					return null;
			}
		}

	}
}
