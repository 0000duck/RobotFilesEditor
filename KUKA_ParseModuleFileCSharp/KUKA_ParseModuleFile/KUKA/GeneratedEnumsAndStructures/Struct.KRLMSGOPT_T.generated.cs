using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class KRLMSGOPT_T : Variable
	{
	#region fields
		private bool vL_STOP;
		private bool cLEAR_P_RESET;
		private bool cLEAR_P_SAW;
		private bool lOG_TO_DB;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "KRLMSGOPT_T";} }
		public bool VL_STOP { get { return vL_STOP; } set { Set(ref vL_STOP, value); } }
		public bool CLEAR_P_RESET { get { return cLEAR_P_RESET; } set { Set(ref cLEAR_P_RESET, value); } }
		public bool CLEAR_P_SAW { get { return cLEAR_P_SAW; } set { Set(ref cLEAR_P_SAW, value); } }
		public bool LOG_TO_DB { get { return lOG_TO_DB; } set { Set(ref lOG_TO_DB, value); } }
	#endregion //properties

	#region constructors
		public KRLMSGOPT_T(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("VL_STOP")) vL_STOP = bool.Parse(dataItems["VL_STOP"].ToString());
			if (dataItems.ContainsKey("CLEAR_P_RESET")) cLEAR_P_RESET = bool.Parse(dataItems["CLEAR_P_RESET"].ToString());
			if (dataItems.ContainsKey("CLEAR_P_SAW")) cLEAR_P_SAW = bool.Parse(dataItems["CLEAR_P_SAW"].ToString());
			if (dataItems.ContainsKey("LOG_TO_DB")) lOG_TO_DB = bool.Parse(dataItems["LOG_TO_DB"].ToString());
		}

		public KRLMSGOPT_T(bool VL_STOP, bool CLEAR_P_RESET, bool CLEAR_P_SAW, bool LOG_TO_DB, string valName="")
		{
			vL_STOP = VL_STOP;
			cLEAR_P_RESET = CLEAR_P_RESET;
			cLEAR_P_SAW = CLEAR_P_SAW;
			lOG_TO_DB = LOG_TO_DB;
			valName = ValName;
		}

		public KRLMSGOPT_T(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["VL_STOP"] != null) vL_STOP = (bool)mem["VL_STOP"];
			if (mem["CLEAR_P_RESET"] != null) cLEAR_P_RESET = (bool)mem["CLEAR_P_RESET"];
			if (mem["CLEAR_P_SAW"] != null) cLEAR_P_SAW = (bool)mem["CLEAR_P_SAW"];
			if (mem["LOG_TO_DB"] != null) lOG_TO_DB = (bool)mem["LOG_TO_DB"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL KRLMSGOPT_T " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				vL_STOP.ToString(CultureInfo.InvariantCulture),
				cLEAR_P_RESET.ToString(CultureInfo.InvariantCulture),
				cLEAR_P_SAW.ToString(CultureInfo.InvariantCulture),
				lOG_TO_DB.ToString(CultureInfo.InvariantCulture),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{VL_STOP {0},CLEAR_P_RESET {1},CLEAR_P_SAW {2},LOG_TO_DB {3}}}",
				BtoStr(vL_STOP), BtoStr(cLEAR_P_RESET), BtoStr(cLEAR_P_SAW), BtoStr(lOG_TO_DB)
				);
		}

	#endregion //methods

	}
}
