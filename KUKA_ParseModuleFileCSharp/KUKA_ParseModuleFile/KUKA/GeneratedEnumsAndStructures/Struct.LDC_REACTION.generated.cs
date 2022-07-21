using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class LDC_REACTION : Variable
	{
	#region fields
		private LOADREACTION uNDERLOAD;
		private LOADREACTION oVERLOAD;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "LDC_REACTION";} }
		public LOADREACTION UNDERLOAD { get { return uNDERLOAD; } set { Set(ref uNDERLOAD, value); } }
		public LOADREACTION OVERLOAD { get { return oVERLOAD; } set { Set(ref oVERLOAD, value); } }
	#endregion //properties

	#region constructors
		public LDC_REACTION(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("UNDERLOAD")) uNDERLOAD = (LOADREACTION)System.Enum.Parse(typeof(LOADREACTION), dataItems["UNDERLOAD"].ToString().TrimStart('#'), true);
			if (dataItems.ContainsKey("OVERLOAD")) oVERLOAD = (LOADREACTION)System.Enum.Parse(typeof(LOADREACTION), dataItems["OVERLOAD"].ToString().TrimStart('#'), true);
		}

		public LDC_REACTION(LOADREACTION UNDERLOAD, LOADREACTION OVERLOAD, string valName="")
		{
			uNDERLOAD = UNDERLOAD;
			oVERLOAD = OVERLOAD;
			valName = ValName;
		}

		public LDC_REACTION(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["UNDERLOAD"] != null) uNDERLOAD = (LOADREACTION)mem["UNDERLOAD"];
			if (mem["OVERLOAD"] != null) oVERLOAD = (LOADREACTION)mem["OVERLOAD"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL LDC_REACTION " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				uNDERLOAD.ToString(),
				oVERLOAD.ToString(),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{UNDERLOAD {0},OVERLOAD {1}}}",
				"#" + uNDERLOAD.ToString(), "#" + oVERLOAD.ToString()
				);
		}

	#endregion //methods

	}
}
