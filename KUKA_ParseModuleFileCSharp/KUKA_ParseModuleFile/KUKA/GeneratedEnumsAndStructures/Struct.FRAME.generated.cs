using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class FRAME : Variable
	{
	#region fields
		private double x;
		private double y;
		private double z;
		private double a;
		private double b;
		private double c;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "FRAME";} }
		public double X { get { return x; } set { Set(ref x, value); } }
		public double Y { get { return y; } set { Set(ref y, value); } }
		public double Z { get { return z; } set { Set(ref z, value); } }
		public double A { get { return a; } set { Set(ref a, value); } }
		public double B { get { return b; } set { Set(ref b, value); } }
		public double C { get { return c; } set { Set(ref c, value); } }
	#endregion //properties

	#region constructors
		public FRAME(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("X")) x = double.Parse(dataItems["X"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("Y")) y = double.Parse(dataItems["Y"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("Z")) z = double.Parse(dataItems["Z"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("A")) a = double.Parse(dataItems["A"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("B")) b = double.Parse(dataItems["B"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("C")) c = double.Parse(dataItems["C"].ToString(), CultureInfo.InvariantCulture);
		}

		public FRAME(double X, double Y, double Z, double A, double B, double C, string valName="")
		{
			x = X;
			y = Y;
			z = Z;
			a = A;
			b = B;
			c = C;
			valName = ValName;
		}

		public FRAME(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["X"] != null) x = (double)mem["X"];
			if (mem["Y"] != null) y = (double)mem["Y"];
			if (mem["Z"] != null) z = (double)mem["Z"];
			if (mem["A"] != null) a = (double)mem["A"];
			if (mem["B"] != null) b = (double)mem["B"];
			if (mem["C"] != null) c = (double)mem["C"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL FRAME " + ToString();
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
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{X {0},Y {1},Z {2},A {3},B {4},C {5}}}",
				x, y, z, a, b, c
				);
		}

	#endregion //methods

	}
}
