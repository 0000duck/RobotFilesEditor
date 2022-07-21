using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class REAL : Variable
	{
	#region fields
		private double value;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "REAL";} }
		public double Value { get { return value; } set { Set(ref value, value); } }
	#endregion //properties

	#region constructors
		public REAL(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("value")) value = double.Parse(dataItems["value"].ToString(), CultureInfo.InvariantCulture);
		}

		public REAL(double Value, string valName="")
		{
			value = Value;
			valName = ValName;
		}

		public REAL(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["value"] != null) value = (double)mem["value"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL REAL " + ToString();
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
