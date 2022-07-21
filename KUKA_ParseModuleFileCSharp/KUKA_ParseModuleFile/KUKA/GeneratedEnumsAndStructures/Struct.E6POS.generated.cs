using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class E6POS : Variable
	{
	#region fields
		private double x;
		private double y;
		private double z;
		private double a;
		private double b;
		private double c;
		private int s;
		private int t;
		private double e1;
		private double e2;
		private double e3;
		private double e4;
		private double e5;
		private double e6;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "E6POS";} }
		public double X { get { return x; } set { Set(ref x, value); } }
		public double Y { get { return y; } set { Set(ref y, value); } }
		public double Z { get { return z; } set { Set(ref z, value); } }
		public double A { get { return a; } set { Set(ref a, value); } }
		public double B { get { return b; } set { Set(ref b, value); } }
		public double C { get { return c; } set { Set(ref c, value); } }
		public int S { get { return s; } set { Set(ref s, value); } }
		public int T { get { return t; } set { Set(ref t, value); } }
		public double E1 { get { return e1; } set { Set(ref e1, value); } }
		public double E2 { get { return e2; } set { Set(ref e2, value); } }
		public double E3 { get { return e3; } set { Set(ref e3, value); } }
		public double E4 { get { return e4; } set { Set(ref e4, value); } }
		public double E5 { get { return e5; } set { Set(ref e5, value); } }
		public double E6 { get { return e6; } set { Set(ref e6, value); } }
	#endregion //properties

	#region constructors
		public E6POS(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("X")) x = double.Parse(dataItems["X"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("Y")) y = double.Parse(dataItems["Y"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("Z")) z = double.Parse(dataItems["Z"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("A")) a = double.Parse(dataItems["A"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("B")) b = double.Parse(dataItems["B"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("C")) c = double.Parse(dataItems["C"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("S")) s = int.Parse(dataItems["S"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("T")) t = int.Parse(dataItems["T"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("E1")) e1 = double.Parse(dataItems["E1"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("E2")) e2 = double.Parse(dataItems["E2"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("E3")) e3 = double.Parse(dataItems["E3"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("E4")) e4 = double.Parse(dataItems["E4"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("E5")) e5 = double.Parse(dataItems["E5"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("E6")) e6 = double.Parse(dataItems["E6"].ToString(), CultureInfo.InvariantCulture);
		}

		public E6POS(double X, double Y, double Z, double A, double B, double C, int S, int T, double E1, double E2, double E3, double E4, double E5, double E6, string valName="")
		{
			x = X;
			y = Y;
			z = Z;
			a = A;
			b = B;
			c = C;
			s = S;
			t = T;
			e1 = E1;
			e2 = E2;
			e3 = E3;
			e4 = E4;
			e5 = E5;
			e6 = E6;
			valName = ValName;
		}

		public E6POS(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["X"] != null) x = (double)mem["X"];
			if (mem["Y"] != null) y = (double)mem["Y"];
			if (mem["Z"] != null) z = (double)mem["Z"];
			if (mem["A"] != null) a = (double)mem["A"];
			if (mem["B"] != null) b = (double)mem["B"];
			if (mem["C"] != null) c = (double)mem["C"];
			if (mem["S"] != null) s = (int)mem["S"];
			if (mem["T"] != null) t = (int)mem["T"];
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
			return "DECL E6POS " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				x.ToString(CultureInfo.InvariantCulture),
				y.ToString(CultureInfo.InvariantCulture),
				z.ToString(CultureInfo.InvariantCulture),
				a.ToString(CultureInfo.InvariantCulture),
				b.ToString(CultureInfo.InvariantCulture),
				c.ToString(CultureInfo.InvariantCulture),
				s.ToString(CultureInfo.InvariantCulture),
				t.ToString(CultureInfo.InvariantCulture),
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
				"{{X {0},Y {1},Z {2},A {3},B {4},C {5},S {6},T {7},E1 {8},E2 {9},E3 {10},E4 {11},E5 {12},E6 {13}}}",
				x, y, z, a, b, c, s, t, e1, e2, e3, e4, e5, e6
				);
		}

	#endregion //methods

	}
}
