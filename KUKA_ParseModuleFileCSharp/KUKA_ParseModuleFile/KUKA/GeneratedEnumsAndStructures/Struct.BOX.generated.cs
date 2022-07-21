using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class BOX : Variable
	{
	#region fields
		private double x;
		private double y;
		private double z;
		private double a;
		private double b;
		private double c;
		private double x1;
		private double y1;
		private double z1;
		private double x2;
		private double y2;
		private double z2;
		private MODE mODE;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "BOX";} }
		public double X { get { return x; } set { Set(ref x, value); } }
		public double Y { get { return y; } set { Set(ref y, value); } }
		public double Z { get { return z; } set { Set(ref z, value); } }
		public double A { get { return a; } set { Set(ref a, value); } }
		public double B { get { return b; } set { Set(ref b, value); } }
		public double C { get { return c; } set { Set(ref c, value); } }
		public double X1 { get { return x1; } set { Set(ref x1, value); } }
		public double Y1 { get { return y1; } set { Set(ref y1, value); } }
		public double Z1 { get { return z1; } set { Set(ref z1, value); } }
		public double X2 { get { return x2; } set { Set(ref x2, value); } }
		public double Y2 { get { return y2; } set { Set(ref y2, value); } }
		public double Z2 { get { return z2; } set { Set(ref z2, value); } }
		public MODE MODE { get { return mODE; } set { Set(ref mODE, value); } }
	#endregion //properties

	#region constructors
		public BOX(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("X")) x = double.Parse(dataItems["X"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("Y")) y = double.Parse(dataItems["Y"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("Z")) z = double.Parse(dataItems["Z"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("A")) a = double.Parse(dataItems["A"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("B")) b = double.Parse(dataItems["B"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("C")) c = double.Parse(dataItems["C"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("X1")) x1 = double.Parse(dataItems["X1"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("Y1")) y1 = double.Parse(dataItems["Y1"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("Z1")) z1 = double.Parse(dataItems["Z1"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("X2")) x2 = double.Parse(dataItems["X2"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("Y2")) y2 = double.Parse(dataItems["Y2"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("Z2")) z2 = double.Parse(dataItems["Z2"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("MODE")) mODE = (MODE)System.Enum.Parse(typeof(MODE), dataItems["MODE"].ToString().TrimStart('#'), true);
		}

		public BOX(double X, double Y, double Z, double A, double B, double C, double X1, double Y1, double Z1, double X2, double Y2, double Z2, MODE MODE, string valName="")
		{
			x = X;
			y = Y;
			z = Z;
			a = A;
			b = B;
			c = C;
			x1 = X1;
			y1 = Y1;
			z1 = Z1;
			x2 = X2;
			y2 = Y2;
			z2 = Z2;
			mODE = MODE;
			valName = ValName;
		}

		public BOX(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["X"] != null) x = (double)mem["X"];
			if (mem["Y"] != null) y = (double)mem["Y"];
			if (mem["Z"] != null) z = (double)mem["Z"];
			if (mem["A"] != null) a = (double)mem["A"];
			if (mem["B"] != null) b = (double)mem["B"];
			if (mem["C"] != null) c = (double)mem["C"];
			if (mem["X1"] != null) x1 = (double)mem["X1"];
			if (mem["Y1"] != null) y1 = (double)mem["Y1"];
			if (mem["Z1"] != null) z1 = (double)mem["Z1"];
			if (mem["X2"] != null) x2 = (double)mem["X2"];
			if (mem["Y2"] != null) y2 = (double)mem["Y2"];
			if (mem["Z2"] != null) z2 = (double)mem["Z2"];
			if (mem["MODE"] != null) mODE = (MODE)mem["MODE"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL BOX " + ToString();
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
				x1.ToString(CultureInfo.InvariantCulture),
				y1.ToString(CultureInfo.InvariantCulture),
				z1.ToString(CultureInfo.InvariantCulture),
				x2.ToString(CultureInfo.InvariantCulture),
				y2.ToString(CultureInfo.InvariantCulture),
				z2.ToString(CultureInfo.InvariantCulture),
				mODE.ToString(),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{X {0},Y {1},Z {2},A {3},B {4},C {5},X1 {6},Y1 {7},Z1 {8},X2 {9},Y2 {10},Z2 {11},MODE {12}}}",
				x, y, z, a, b, c, x1, y1, z1, x2, y2, z2, "#" + mODE.ToString()
				);
		}

	#endregion //methods

	}
}
