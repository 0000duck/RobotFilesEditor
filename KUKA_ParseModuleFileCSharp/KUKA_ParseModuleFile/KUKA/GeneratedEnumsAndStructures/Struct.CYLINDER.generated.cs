using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class CYLINDER : Variable
	{
	#region fields
		private double x;
		private double y;
		private double z;
		private double a;
		private double b;
		private double c;
		private double z1;
		private double z2;
		private double r;
		private MODE mODE;
		private REFERENCE rEFERENCE;
		private bool sTATE;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "CYLINDER";} }
		public double X { get { return x; } set { Set(ref x, value); } }
		public double Y { get { return y; } set { Set(ref y, value); } }
		public double Z { get { return z; } set { Set(ref z, value); } }
		public double A { get { return a; } set { Set(ref a, value); } }
		public double B { get { return b; } set { Set(ref b, value); } }
		public double C { get { return c; } set { Set(ref c, value); } }
		public double Z1 { get { return z1; } set { Set(ref z1, value); } }
		public double Z2 { get { return z2; } set { Set(ref z2, value); } }
		public double R { get { return r; } set { Set(ref r, value); } }
		public MODE MODE { get { return mODE; } set { Set(ref mODE, value); } }
		public REFERENCE REFERENCE { get { return rEFERENCE; } set { Set(ref rEFERENCE, value); } }
		public bool STATE { get { return sTATE; } set { Set(ref sTATE, value); } }
	#endregion //properties

	#region constructors
		public CYLINDER(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("X")) x = double.Parse(dataItems["X"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("Y")) y = double.Parse(dataItems["Y"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("Z")) z = double.Parse(dataItems["Z"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("A")) a = double.Parse(dataItems["A"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("B")) b = double.Parse(dataItems["B"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("C")) c = double.Parse(dataItems["C"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("Z1")) z1 = double.Parse(dataItems["Z1"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("Z2")) z2 = double.Parse(dataItems["Z2"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("R")) r = double.Parse(dataItems["R"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("MODE")) mODE = (MODE)System.Enum.Parse(typeof(MODE), dataItems["MODE"].ToString().TrimStart('#'), true);
			if (dataItems.ContainsKey("REFERENCE")) rEFERENCE = (REFERENCE)System.Enum.Parse(typeof(REFERENCE), dataItems["REFERENCE"].ToString().TrimStart('#'), true);
			if (dataItems.ContainsKey("STATE")) sTATE = bool.Parse(dataItems["STATE"].ToString());
		}

		public CYLINDER(double X, double Y, double Z, double A, double B, double C, double Z1, double Z2, double R, MODE MODE, REFERENCE REFERENCE, bool STATE, string valName="")
		{
			x = X;
			y = Y;
			z = Z;
			a = A;
			b = B;
			c = C;
			z1 = Z1;
			z2 = Z2;
			r = R;
			mODE = MODE;
			rEFERENCE = REFERENCE;
			sTATE = STATE;
			valName = ValName;
		}

		public CYLINDER(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["X"] != null) x = (double)mem["X"];
			if (mem["Y"] != null) y = (double)mem["Y"];
			if (mem["Z"] != null) z = (double)mem["Z"];
			if (mem["A"] != null) a = (double)mem["A"];
			if (mem["B"] != null) b = (double)mem["B"];
			if (mem["C"] != null) c = (double)mem["C"];
			if (mem["Z1"] != null) z1 = (double)mem["Z1"];
			if (mem["Z2"] != null) z2 = (double)mem["Z2"];
			if (mem["R"] != null) r = (double)mem["R"];
			if (mem["MODE"] != null) mODE = (MODE)mem["MODE"];
			if (mem["REFERENCE"] != null) rEFERENCE = (REFERENCE)mem["REFERENCE"];
			if (mem["STATE"] != null) sTATE = (bool)mem["STATE"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL CYLINDER " + ToString();
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
				z1.ToString(CultureInfo.InvariantCulture),
				z2.ToString(CultureInfo.InvariantCulture),
				r.ToString(CultureInfo.InvariantCulture),
				mODE.ToString(),
				rEFERENCE.ToString(),
				sTATE.ToString(CultureInfo.InvariantCulture),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{X {0},Y {1},Z {2},A {3},B {4},C {5},Z1 {6},Z2 {7},R {8},MODE {9},REFERENCE {10},STATE {11}}}",
				x, y, z, a, b, c, z1, z2, r, "#" + mODE.ToString(), "#" + rEFERENCE.ToString(), BtoStr(sTATE)
				);
		}

	#endregion //methods

	}
}
