using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class STOPMESS : Variable
	{
	#region fields
		private CAUSE_T cAUSE;
		private int cONFNO;
		private int gRO;
		private int mESSNO;
		private int sTATE;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "STOPMESS";} }
		public CAUSE_T CAUSE { get { return cAUSE; } set { Set(ref cAUSE, value); } }
		public int CONFNO { get { return cONFNO; } set { Set(ref cONFNO, value); } }
		public int GRO { get { return gRO; } set { Set(ref gRO, value); } }
		public int MESSNO { get { return mESSNO; } set { Set(ref mESSNO, value); } }
		public int STATE { get { return sTATE; } set { Set(ref sTATE, value); } }
	#endregion //properties

	#region constructors
		public STOPMESS(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("CAUSE")) cAUSE = (CAUSE_T)System.Enum.Parse(typeof(CAUSE_T), dataItems["CAUSE"].ToString().TrimStart('#'), true);
			if (dataItems.ContainsKey("CONFNO")) cONFNO = int.Parse(dataItems["CONFNO"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("GRO")) gRO = int.Parse(dataItems["GRO"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("MESSNO")) mESSNO = int.Parse(dataItems["MESSNO"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("STATE")) sTATE = int.Parse(dataItems["STATE"].ToString(), CultureInfo.InvariantCulture);
		}

		public STOPMESS(CAUSE_T CAUSE, int CONFNO, int GRO, int MESSNO, int STATE, string valName="")
		{
			cAUSE = CAUSE;
			cONFNO = CONFNO;
			gRO = GRO;
			mESSNO = MESSNO;
			sTATE = STATE;
			valName = ValName;
		}

		public STOPMESS(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["CAUSE"] != null) cAUSE = (CAUSE_T)mem["CAUSE"];
			if (mem["CONFNO"] != null) cONFNO = (int)mem["CONFNO"];
			if (mem["GRO"] != null) gRO = (int)mem["GRO"];
			if (mem["MESSNO"] != null) mESSNO = (int)mem["MESSNO"];
			if (mem["STATE"] != null) sTATE = (int)mem["STATE"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL STOPMESS " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				cAUSE.ToString(),
				cONFNO.ToString(CultureInfo.InvariantCulture),
				gRO.ToString(CultureInfo.InvariantCulture),
				mESSNO.ToString(CultureInfo.InvariantCulture),
				sTATE.ToString(CultureInfo.InvariantCulture),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{CAUSE {0},CONFNO {1},GRO {2},MESSNO {3},STATE {4}}}",
				"#" + cAUSE.ToString(), cONFNO, gRO, mESSNO, sTATE
				);
		}

	#endregion //methods

	}
}
