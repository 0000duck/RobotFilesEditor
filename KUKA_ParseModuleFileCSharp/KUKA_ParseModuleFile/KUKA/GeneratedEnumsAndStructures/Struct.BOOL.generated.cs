using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class BOOL : Variable
	{
	#region fields
		private bool value;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "BOOL";} }
		public bool Value { get { return value; } set { Set(ref value, value); } }
	#endregion //properties

	#region constructors
		public BOOL(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("value")) value = bool.Parse(dataItems["value"].ToString());
		}

		public BOOL(bool Value, string valName="")
		{
			value = Value;
			valName = ValName;
		}

		public BOOL(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["value"] != null) value = (bool)mem["value"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL BOOL " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				value.ToString(CultureInfo.InvariantCulture),
			};
		}

		public override string ToString()
		{
			return BtoStr(value);
		}

	#endregion //methods

	}
}
