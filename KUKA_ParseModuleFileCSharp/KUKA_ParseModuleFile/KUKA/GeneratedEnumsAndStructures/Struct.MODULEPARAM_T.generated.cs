using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class MODULEPARAM_T : Variable
	{
	#region fields
		private string pARAMS;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "MODULEPARAM_T";} }
		public string PARAMS { get { return pARAMS; } set { Set(ref pARAMS, value); } }
	#endregion //properties

	#region constructors
		public MODULEPARAM_T(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("PARAMS")) pARAMS = dataItems["PARAMS"].ToString().Trim('"');
		}

		public MODULEPARAM_T(string PARAMS, string valName="")
		{
			pARAMS = PARAMS;
			valName = ValName;
		}

		public MODULEPARAM_T(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["PARAMS"] != null) pARAMS = (string)mem["PARAMS"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL MODULEPARAM_T " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				pARAMS,
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{PARAMS[] \"{0}\"}}",
				pARAMS
				);
		}

	#endregion //methods

	}
}
