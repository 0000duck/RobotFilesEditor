using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class _calEqPressure : Variable
	{
	#region fields
		private double min;
		private double max;
		private double stat;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "_calEqPressure";} }
		public double Min { get { return min; } set { Set(ref min, value); } }
		public double Max { get { return max; } set { Set(ref max, value); } }
		public double Stat { get { return stat; } set { Set(ref stat, value); } }
	#endregion //properties

	#region constructors
		public _calEqPressure(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("min")) min = double.Parse(dataItems["min"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("max")) max = double.Parse(dataItems["max"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("stat")) stat = double.Parse(dataItems["stat"].ToString(), CultureInfo.InvariantCulture);
		}

		public _calEqPressure(double Min, double Max, double Stat, string valName="")
		{
			min = Min;
			max = Max;
			stat = Stat;
			valName = ValName;
		}

		public _calEqPressure(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["min"] != null) min = (double)mem["min"];
			if (mem["max"] != null) max = (double)mem["max"];
			if (mem["stat"] != null) stat = (double)mem["stat"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL _calEqPressure " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				min.ToString(CultureInfo.InvariantCulture),
				max.ToString(CultureInfo.InvariantCulture),
				stat.ToString(CultureInfo.InvariantCulture),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{min {0},max {1},stat {2}}}",
				min, max, stat
				);
		}

	#endregion //methods

	}
}
