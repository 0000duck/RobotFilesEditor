using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class BASIS_SUGG_T : Variable
	{
	#region fields
		private string pOINT1;
		private string pOINT2;
		private string cP_PARAMS;
		private string pTP_PARAMS;
		private string cONT;
		private string cP_VEL;
		private string pTP_VEL;
		private string sYNC_PARAMS;
		private string sPL_NAME;
		private string a_PARAMS;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "BASIS_SUGG_T";} }
		public string POINT1 { get { return pOINT1; } set { Set(ref pOINT1, value); } }
		public string POINT2 { get { return pOINT2; } set { Set(ref pOINT2, value); } }
		public string CP_PARAMS { get { return cP_PARAMS; } set { Set(ref cP_PARAMS, value); } }
		public string PTP_PARAMS { get { return pTP_PARAMS; } set { Set(ref pTP_PARAMS, value); } }
		public string CONT { get { return cONT; } set { Set(ref cONT, value); } }
		public string CP_VEL { get { return cP_VEL; } set { Set(ref cP_VEL, value); } }
		public string PTP_VEL { get { return pTP_VEL; } set { Set(ref pTP_VEL, value); } }
		public string SYNC_PARAMS { get { return sYNC_PARAMS; } set { Set(ref sYNC_PARAMS, value); } }
		public string SPL_NAME { get { return sPL_NAME; } set { Set(ref sPL_NAME, value); } }
		public string A_PARAMS { get { return a_PARAMS; } set { Set(ref a_PARAMS, value); } }
	#endregion //properties

	#region constructors
		public BASIS_SUGG_T(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("POINT1")) pOINT1 = dataItems["POINT1"].ToString().Trim('"');
			if (dataItems.ContainsKey("POINT2")) pOINT2 = dataItems["POINT2"].ToString().Trim('"');
			if (dataItems.ContainsKey("CP_PARAMS")) cP_PARAMS = dataItems["CP_PARAMS"].ToString().Trim('"');
			if (dataItems.ContainsKey("PTP_PARAMS")) pTP_PARAMS = dataItems["PTP_PARAMS"].ToString().Trim('"');
			if (dataItems.ContainsKey("CONT")) cONT = dataItems["CONT"].ToString().Trim('"');
			if (dataItems.ContainsKey("CP_VEL")) cP_VEL = dataItems["CP_VEL"].ToString().Trim('"');
			if (dataItems.ContainsKey("PTP_VEL")) pTP_VEL = dataItems["PTP_VEL"].ToString().Trim('"');
			if (dataItems.ContainsKey("SYNC_PARAMS")) sYNC_PARAMS = dataItems["SYNC_PARAMS"].ToString().Trim('"');
			if (dataItems.ContainsKey("SPL_NAME")) sPL_NAME = dataItems["SPL_NAME"].ToString().Trim('"');
			if (dataItems.ContainsKey("A_PARAMS")) a_PARAMS = dataItems["A_PARAMS"].ToString().Trim('"');
		}

		public BASIS_SUGG_T(string POINT1, string POINT2, string CP_PARAMS, string PTP_PARAMS, string CONT, string CP_VEL, string PTP_VEL, string SYNC_PARAMS, string SPL_NAME, string A_PARAMS, string valName="")
		{
			pOINT1 = POINT1;
			pOINT2 = POINT2;
			cP_PARAMS = CP_PARAMS;
			pTP_PARAMS = PTP_PARAMS;
			cONT = CONT;
			cP_VEL = CP_VEL;
			pTP_VEL = PTP_VEL;
			sYNC_PARAMS = SYNC_PARAMS;
			sPL_NAME = SPL_NAME;
			a_PARAMS = A_PARAMS;
			valName = ValName;
		}

		public BASIS_SUGG_T(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["POINT1"] != null) pOINT1 = (string)mem["POINT1"];
			if (mem["POINT2"] != null) pOINT2 = (string)mem["POINT2"];
			if (mem["CP_PARAMS"] != null) cP_PARAMS = (string)mem["CP_PARAMS"];
			if (mem["PTP_PARAMS"] != null) pTP_PARAMS = (string)mem["PTP_PARAMS"];
			if (mem["CONT"] != null) cONT = (string)mem["CONT"];
			if (mem["CP_VEL"] != null) cP_VEL = (string)mem["CP_VEL"];
			if (mem["PTP_VEL"] != null) pTP_VEL = (string)mem["PTP_VEL"];
			if (mem["SYNC_PARAMS"] != null) sYNC_PARAMS = (string)mem["SYNC_PARAMS"];
			if (mem["SPL_NAME"] != null) sPL_NAME = (string)mem["SPL_NAME"];
			if (mem["A_PARAMS"] != null) a_PARAMS = (string)mem["A_PARAMS"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL BASIS_SUGG_T " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				pOINT1,
				pOINT2,
				cP_PARAMS,
				pTP_PARAMS,
				cONT,
				cP_VEL,
				pTP_VEL,
				sYNC_PARAMS,
				sPL_NAME,
				a_PARAMS,
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{POINT1[] \"{0}\",POINT2[] \"{1}\",CP_PARAMS[] \"{2}\",PTP_PARAMS[] \"{3}\",CONT[] \"{4}\",CP_VEL[] \"{5}\",PTP_VEL[] \"{6}\",SYNC_PARAMS[] \"{7}\",SPL_NAME[] \"{8}\",A_PARAMS[] \"{9}\"}}",
				pOINT1, pOINT2, cP_PARAMS, pTP_PARAMS, cONT, cP_VEL, pTP_VEL, sYNC_PARAMS, sPL_NAME, a_PARAMS
				);
		}

	#endregion //methods

	}
}
