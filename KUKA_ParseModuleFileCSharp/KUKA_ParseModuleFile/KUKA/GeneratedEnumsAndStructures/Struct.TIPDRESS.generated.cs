using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class TIPDRESS : Variable
	{
	#region fields
		private int num;
		private double dressTime;
		private int dressForce;
		private int interval;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "TIPDRESS";} }
		public int Num { get { return num; } set { Set(ref num, value); } }
		public double DressTime { get { return dressTime; } set { Set(ref dressTime, value); } }
		public int DressForce { get { return dressForce; } set { Set(ref dressForce, value); } }
		public int Interval { get { return interval; } set { Set(ref interval, value); } }
	#endregion //properties

	#region constructors
		public TIPDRESS(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("Num")) num = int.Parse(dataItems["Num"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("DressTime")) dressTime = double.Parse(dataItems["DressTime"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("DressForce")) dressForce = int.Parse(dataItems["DressForce"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("Interval")) interval = int.Parse(dataItems["Interval"].ToString(), CultureInfo.InvariantCulture);
		}

		public TIPDRESS(int Num, double DressTime, int DressForce, int Interval, string valName="")
		{
			num = Num;
			dressTime = DressTime;
			dressForce = DressForce;
			interval = Interval;
			valName = ValName;
		}

		public TIPDRESS(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["Num"] != null) num = (int)mem["Num"];
			if (mem["DressTime"] != null) dressTime = (double)mem["DressTime"];
			if (mem["DressForce"] != null) dressForce = (int)mem["DressForce"];
			if (mem["Interval"] != null) interval = (int)mem["Interval"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL TIPDRESS " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				num.ToString(CultureInfo.InvariantCulture),
				dressTime.ToString(CultureInfo.InvariantCulture),
				dressForce.ToString(CultureInfo.InvariantCulture),
				interval.ToString(CultureInfo.InvariantCulture),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{Num {0},DressTime {1},DressForce {2},Interval {3}}}",
				num, dressTime, dressForce, interval
				);
		}

	#endregion //methods

	}
}
