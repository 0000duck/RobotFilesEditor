using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class INT : Variable
	{
	#region fields
		private int value;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "INT";} }
		public int Value { get { return value; } set { Set(ref value, value); } }
	#endregion //properties

	#region constructors
		public INT(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("value")) value = int.Parse(dataItems["value"].ToString(), CultureInfo.InvariantCulture);
		}

		public INT(int Value, string valName="")
		{
			value = Value;
			valName = ValName;
		}

		public INT(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["value"] != null) value = (int)mem["value"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL INT " + ToString();
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
			return value.ToString(CultureInfo.InvariantCulture);
		}

	#endregion //methods

	}
}
