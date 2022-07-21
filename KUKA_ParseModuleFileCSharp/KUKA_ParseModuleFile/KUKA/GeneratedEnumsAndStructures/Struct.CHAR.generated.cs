using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class CHAR : Variable
	{
	#region fields
		private string value;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "CHAR";} }
		public string Value { get { return value; } set { Set(ref value, value); } }
	#endregion //properties

	#region constructors
		public CHAR(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("value")) value = dataItems["value"].ToString().Trim('"');
		}

		public CHAR(string Value, string valName="")
		{
			value = Value;
			valName = ValName;
		}

		public CHAR(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["value"] != null) value = (string)mem["value"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL CHAR " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				value,
			};
		}

		public override string ToString()
		{
			return "\"" + value + "\"";
		}

	#endregion //methods

	}
}
