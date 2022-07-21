using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class KRLMSGPAR_T : Variable
	{
	#region fields
		private bool pAR_BOOL;
		private int pAR_INT;
		private double pAR_REAL;
		private string pAR_TXT;
		private KRLMSGPARTYPE_T pAR_TYPE;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "KRLMSGPAR_T";} }
		public bool PAR_BOOL { get { return pAR_BOOL; } set { Set(ref pAR_BOOL, value); } }
		public int PAR_INT { get { return pAR_INT; } set { Set(ref pAR_INT, value); } }
		public double PAR_REAL { get { return pAR_REAL; } set { Set(ref pAR_REAL, value); } }
		public string PAR_TXT { get { return pAR_TXT; } set { Set(ref pAR_TXT, value); } }
		public KRLMSGPARTYPE_T PAR_TYPE { get { return pAR_TYPE; } set { Set(ref pAR_TYPE, value); } }
	#endregion //properties

	#region constructors
		public KRLMSGPAR_T(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("PAR_BOOL")) pAR_BOOL = bool.Parse(dataItems["PAR_BOOL"].ToString());
			if (dataItems.ContainsKey("PAR_INT")) pAR_INT = int.Parse(dataItems["PAR_INT"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("PAR_REAL")) pAR_REAL = double.Parse(dataItems["PAR_REAL"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("PAR_TXT")) pAR_TXT = dataItems["PAR_TXT"].ToString().Trim('"');
			if (dataItems.ContainsKey("PAR_TYPE")) pAR_TYPE = (KRLMSGPARTYPE_T)System.Enum.Parse(typeof(KRLMSGPARTYPE_T), dataItems["PAR_TYPE"].ToString().TrimStart('#'), true);
		}

		public KRLMSGPAR_T(bool PAR_BOOL, int PAR_INT, double PAR_REAL, string PAR_TXT, KRLMSGPARTYPE_T PAR_TYPE, string valName="")
		{
			pAR_BOOL = PAR_BOOL;
			pAR_INT = PAR_INT;
			pAR_REAL = PAR_REAL;
			pAR_TXT = PAR_TXT;
			pAR_TYPE = PAR_TYPE;
			valName = ValName;
		}

		public KRLMSGPAR_T(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["PAR_BOOL"] != null) pAR_BOOL = (bool)mem["PAR_BOOL"];
			if (mem["PAR_INT"] != null) pAR_INT = (int)mem["PAR_INT"];
			if (mem["PAR_REAL"] != null) pAR_REAL = (double)mem["PAR_REAL"];
			if (mem["PAR_TXT"] != null) pAR_TXT = (string)mem["PAR_TXT"];
			if (mem["PAR_TYPE"] != null) pAR_TYPE = (KRLMSGPARTYPE_T)mem["PAR_TYPE"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL KRLMSGPAR_T " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				pAR_BOOL.ToString(CultureInfo.InvariantCulture),
				pAR_INT.ToString(CultureInfo.InvariantCulture),
				pAR_REAL.ToString(CultureInfo.InvariantCulture),
				pAR_TXT,
				pAR_TYPE.ToString(),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{PAR_BOOL {0},PAR_INT {1},PAR_REAL {2},PAR_TXT[] \"{3}\",PAR_TYPE {4}}}",
				BtoStr(pAR_BOOL), pAR_INT, pAR_REAL, pAR_TXT, "#" + pAR_TYPE.ToString()
				);
		}

	#endregion //methods

	}
}
