using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class SPS_PROG_TYPE : Variable
	{
	#region fields
		private int pROG_NR;
		private string pROG_NAME;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "SPS_PROG_TYPE";} }
		public int PROG_NR { get { return pROG_NR; } set { Set(ref pROG_NR, value); } }
		public string PROG_NAME { get { return pROG_NAME; } set { Set(ref pROG_NAME, value); } }
	#endregion //properties

	#region constructors
		public SPS_PROG_TYPE(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("PROG_NR")) pROG_NR = int.Parse(dataItems["PROG_NR"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("PROG_NAME")) pROG_NAME = dataItems["PROG_NAME"].ToString().Trim('"');
		}

		public SPS_PROG_TYPE(int PROG_NR, string PROG_NAME, string valName="")
		{
			pROG_NR = PROG_NR;
			pROG_NAME = PROG_NAME;
			valName = ValName;
		}

		public SPS_PROG_TYPE(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["PROG_NR"] != null) pROG_NR = (int)mem["PROG_NR"];
			if (mem["PROG_NAME"] != null) pROG_NAME = (string)mem["PROG_NAME"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL SPS_PROG_TYPE " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				pROG_NR.ToString(CultureInfo.InvariantCulture),
				pROG_NAME,
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{PROG_NR {0},PROG_NAME[] \"{1}\"}}",
				pROG_NR, pROG_NAME
				);
		}

	#endregion //methods

	}
}
