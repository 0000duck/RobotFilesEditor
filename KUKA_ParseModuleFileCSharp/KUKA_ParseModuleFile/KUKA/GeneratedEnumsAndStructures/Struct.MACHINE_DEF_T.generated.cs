using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class MACHINE_DEF_T : Variable
	{
	#region fields
		private string nAME;
		private int cOOP_KRC_INDEX;
		private string pARENT;
		private FRAME rOOT;
		private ESYS mECH_TYPE;
		private string gEOMETRY;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "MACHINE_DEF_T";} }
		public string NAME { get { return nAME; } set { Set(ref nAME, value); } }
		public int COOP_KRC_INDEX { get { return cOOP_KRC_INDEX; } set { Set(ref cOOP_KRC_INDEX, value); } }
		public string PARENT { get { return pARENT; } set { Set(ref pARENT, value); } }
		public FRAME ROOT { get { return rOOT; } set { Set(ref rOOT, value); } }
		public ESYS MECH_TYPE { get { return mECH_TYPE; } set { Set(ref mECH_TYPE, value); } }
		public string GEOMETRY { get { return gEOMETRY; } set { Set(ref gEOMETRY, value); } }
	#endregion //properties

	#region constructors
		public MACHINE_DEF_T(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("NAME")) nAME = dataItems["NAME"].ToString().Trim('"');
			if (dataItems.ContainsKey("COOP_KRC_INDEX")) cOOP_KRC_INDEX = int.Parse(dataItems["COOP_KRC_INDEX"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("PARENT")) pARENT = dataItems["PARENT"].ToString().Trim('"');
			if (dataItems.ContainsKey("ROOT")) rOOT = new FRAME(dataItems["ROOT"]);
			if (dataItems.ContainsKey("MECH_TYPE")) mECH_TYPE = (ESYS)System.Enum.Parse(typeof(ESYS), dataItems["MECH_TYPE"].ToString().TrimStart('#'), true);
			if (dataItems.ContainsKey("GEOMETRY")) gEOMETRY = dataItems["GEOMETRY"].ToString().Trim('"');
		}

		public MACHINE_DEF_T(string NAME, int COOP_KRC_INDEX, string PARENT, FRAME ROOT, ESYS MECH_TYPE, string GEOMETRY, string valName="")
		{
			nAME = NAME;
			cOOP_KRC_INDEX = COOP_KRC_INDEX;
			pARENT = PARENT;
			rOOT = ROOT;
			mECH_TYPE = MECH_TYPE;
			gEOMETRY = GEOMETRY;
			valName = ValName;
		}

		public MACHINE_DEF_T(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["NAME"] != null) nAME = (string)mem["NAME"];
			if (mem["COOP_KRC_INDEX"] != null) cOOP_KRC_INDEX = (int)mem["COOP_KRC_INDEX"];
			if (mem["PARENT"] != null) pARENT = (string)mem["PARENT"];
			rOOT = new FRAME((DynamicMemory)mem["ROOT"]);
			if (mem["MECH_TYPE"] != null) mECH_TYPE = (ESYS)mem["MECH_TYPE"];
			if (mem["GEOMETRY"] != null) gEOMETRY = (string)mem["GEOMETRY"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL MACHINE_DEF_T " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				nAME,
				cOOP_KRC_INDEX.ToString(CultureInfo.InvariantCulture),
				pARENT,
				rOOT.ToString(),
				mECH_TYPE.ToString(),
				gEOMETRY,
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{NAME[] \"{0}\",COOP_KRC_INDEX {1},PARENT[] \"{2}\",ROOT {3},MECH_TYPE {4},GEOMETRY[] \"{5}\"}}",
				nAME, cOOP_KRC_INDEX, pARENT, rOOT, "#" + mECH_TYPE.ToString(), gEOMETRY
				);
		}

	#endregion //methods

	}
}
