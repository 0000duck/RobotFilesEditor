using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class TchTool_OptStation : Variable
	{
	#region fields
		private int num;
		private bool value;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "TchTool_OptStation";} }
		public int Num { get { return num; } set { Set(ref num, value); } }
		public bool Value { get { return value; } set { Set(ref value, value); } }
	#endregion //properties

	#region constructors
		public TchTool_OptStation(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("Num")) num = int.Parse(dataItems["Num"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("Value")) value = bool.Parse(dataItems["Value"].ToString());
		}

		public TchTool_OptStation(int Num, bool Value, string valName="")
		{
			num = Num;
			value = Value;
			valName = ValName;
		}

		public TchTool_OptStation(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["Num"] != null) num = (int)mem["Num"];
			if (mem["Value"] != null) value = (bool)mem["Value"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL TchTool_OptStation " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				num.ToString(CultureInfo.InvariantCulture),
				value.ToString(CultureInfo.InvariantCulture),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{Num {0},Value {1}}}",
				num, BtoStr(value)
				);
		}

	#endregion //methods

	}
}
