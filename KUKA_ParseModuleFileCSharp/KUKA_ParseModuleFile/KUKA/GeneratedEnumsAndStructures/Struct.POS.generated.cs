using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class POS : Variable
	{
	#region fields
		private double x;
		private double y;
		private double z;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "POS";} }
		public double X { get { return x; } set { Set(ref x, value); } }
		public double Y { get { return y; } set { Set(ref y, value); } }
		public double Z { get { return z; } set { Set(ref z, value); } }
	#endregion //properties

	#region constructors
		public POS(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("X")) x = double.Parse(dataItems["X"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("Y")) y = double.Parse(dataItems["Y"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("Z")) z = double.Parse(dataItems["Z"].ToString(), CultureInfo.InvariantCulture);
		}

		public POS(double X, double Y, double Z, string valName="")
		{
			x = X;
			y = Y;
			z = Z;
			valName = ValName;
		}

		public POS(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["X"] != null) x = (double)mem["X"];
			if (mem["Y"] != null) y = (double)mem["Y"];
			if (mem["Z"] != null) z = (double)mem["Z"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL POS " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				x.ToString(CultureInfo.InvariantCulture),
				y.ToString(CultureInfo.InvariantCulture),
				z.ToString(CultureInfo.InvariantCulture),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{X {0},Y {1},Z {2}}}",
				x, y, z
				);
		}

	#endregion //methods

	}
}
