using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class E6AXIS : Variable
	{
	#region fields
		private double a1;
		private double a2;
		private double a3;
		private double a4;
		private double a5;
		private double a6;
		private double e1;
		private double e2;
		private double e3;
		private double e4;
		private double e5;
		private double e6;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "E6AXIS";} }
		public double A1 { get { return a1; } set { Set(ref a1, value); } }
		public double A2 { get { return a2; } set { Set(ref a2, value); } }
		public double A3 { get { return a3; } set { Set(ref a3, value); } }
		public double A4 { get { return a4; } set { Set(ref a4, value); } }
		public double A5 { get { return a5; } set { Set(ref a5, value); } }
		public double A6 { get { return a6; } set { Set(ref a6, value); } }
		public double E1 { get { return e1; } set { Set(ref e1, value); } }
		public double E2 { get { return e2; } set { Set(ref e2, value); } }
		public double E3 { get { return e3; } set { Set(ref e3, value); } }
		public double E4 { get { return e4; } set { Set(ref e4, value); } }
		public double E5 { get { return e5; } set { Set(ref e5, value); } }
		public double E6 { get { return e6; } set { Set(ref e6, value); } }
	#endregion //properties

	#region constructors
		public E6AXIS(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("A1")) a1 = double.Parse(dataItems["A1"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("A2")) a2 = double.Parse(dataItems["A2"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("A3")) a3 = double.Parse(dataItems["A3"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("A4")) a4 = double.Parse(dataItems["A4"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("A5")) a5 = double.Parse(dataItems["A5"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("A6")) a6 = double.Parse(dataItems["A6"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("E1")) e1 = double.Parse(dataItems["E1"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("E2")) e2 = double.Parse(dataItems["E2"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("E3")) e3 = double.Parse(dataItems["E3"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("E4")) e4 = double.Parse(dataItems["E4"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("E5")) e5 = double.Parse(dataItems["E5"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("E6")) e6 = double.Parse(dataItems["E6"].ToString(), CultureInfo.InvariantCulture);
		}

		public E6AXIS(double A1, double A2, double A3, double A4, double A5, double A6, double E1, double E2, double E3, double E4, double E5, double E6, string valName="")
		{
			a1 = A1;
			a2 = A2;
			a3 = A3;
			a4 = A4;
			a5 = A5;
			a6 = A6;
			e1 = E1;
			e2 = E2;
			e3 = E3;
			e4 = E4;
			e5 = E5;
			e6 = E6;
			valName = ValName;
		}

		public E6AXIS(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["A1"] != null) a1 = (double)mem["A1"];
			if (mem["A2"] != null) a2 = (double)mem["A2"];
			if (mem["A3"] != null) a3 = (double)mem["A3"];
			if (mem["A4"] != null) a4 = (double)mem["A4"];
			if (mem["A5"] != null) a5 = (double)mem["A5"];
			if (mem["A6"] != null) a6 = (double)mem["A6"];
			if (mem["E1"] != null) e1 = (double)mem["E1"];
			if (mem["E2"] != null) e2 = (double)mem["E2"];
			if (mem["E3"] != null) e3 = (double)mem["E3"];
			if (mem["E4"] != null) e4 = (double)mem["E4"];
			if (mem["E5"] != null) e5 = (double)mem["E5"];
			if (mem["E6"] != null) e6 = (double)mem["E6"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL E6AXIS " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				a1.ToString(CultureInfo.InvariantCulture),
				a2.ToString(CultureInfo.InvariantCulture),
				a3.ToString(CultureInfo.InvariantCulture),
				a4.ToString(CultureInfo.InvariantCulture),
				a5.ToString(CultureInfo.InvariantCulture),
				a6.ToString(CultureInfo.InvariantCulture),
				e1.ToString(CultureInfo.InvariantCulture),
				e2.ToString(CultureInfo.InvariantCulture),
				e3.ToString(CultureInfo.InvariantCulture),
				e4.ToString(CultureInfo.InvariantCulture),
				e5.ToString(CultureInfo.InvariantCulture),
				e6.ToString(CultureInfo.InvariantCulture),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{A1 {0},A2 {1},A3 {2},A4 {3},A5 {4},A6 {5},E1 {6},E2 {7},E3 {8},E4 {9},E5 {10},E6 {11}}}",
				a1, a2, a3, a4, a5, a6, e1, e2, e3, e4, e5, e6
				);
		}

	#endregion //methods

	}
}
