using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class SPIN : Variable
	{
	#region fields
		private double sPIN_A;
		private double sPIN_RAD_G;
		private double sPIN_RAD_H;
		private int sPIN_SG;
		private double sPIN_BETA;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "SPIN";} }
		public double SPIN_A { get { return sPIN_A; } set { Set(ref sPIN_A, value); } }
		public double SPIN_RAD_G { get { return sPIN_RAD_G; } set { Set(ref sPIN_RAD_G, value); } }
		public double SPIN_RAD_H { get { return sPIN_RAD_H; } set { Set(ref sPIN_RAD_H, value); } }
		public int SPIN_SG { get { return sPIN_SG; } set { Set(ref sPIN_SG, value); } }
		public double SPIN_BETA { get { return sPIN_BETA; } set { Set(ref sPIN_BETA, value); } }
	#endregion //properties

	#region constructors
		public SPIN(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("SPIN_A")) sPIN_A = double.Parse(dataItems["SPIN_A"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("SPIN_RAD_G")) sPIN_RAD_G = double.Parse(dataItems["SPIN_RAD_G"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("SPIN_RAD_H")) sPIN_RAD_H = double.Parse(dataItems["SPIN_RAD_H"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("SPIN_SG")) sPIN_SG = int.Parse(dataItems["SPIN_SG"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("SPIN_BETA")) sPIN_BETA = double.Parse(dataItems["SPIN_BETA"].ToString(), CultureInfo.InvariantCulture);
		}

		public SPIN(double SPIN_A, double SPIN_RAD_G, double SPIN_RAD_H, int SPIN_SG, double SPIN_BETA, string valName="")
		{
			sPIN_A = SPIN_A;
			sPIN_RAD_G = SPIN_RAD_G;
			sPIN_RAD_H = SPIN_RAD_H;
			sPIN_SG = SPIN_SG;
			sPIN_BETA = SPIN_BETA;
			valName = ValName;
		}

		public SPIN(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["SPIN_A"] != null) sPIN_A = (double)mem["SPIN_A"];
			if (mem["SPIN_RAD_G"] != null) sPIN_RAD_G = (double)mem["SPIN_RAD_G"];
			if (mem["SPIN_RAD_H"] != null) sPIN_RAD_H = (double)mem["SPIN_RAD_H"];
			if (mem["SPIN_SG"] != null) sPIN_SG = (int)mem["SPIN_SG"];
			if (mem["SPIN_BETA"] != null) sPIN_BETA = (double)mem["SPIN_BETA"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL SPIN " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				sPIN_A.ToString(CultureInfo.InvariantCulture),
				sPIN_RAD_G.ToString(CultureInfo.InvariantCulture),
				sPIN_RAD_H.ToString(CultureInfo.InvariantCulture),
				sPIN_SG.ToString(CultureInfo.InvariantCulture),
				sPIN_BETA.ToString(CultureInfo.InvariantCulture),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{SPIN_A {0},SPIN_RAD_G {1},SPIN_RAD_H {2},SPIN_SG {3},SPIN_BETA {4}}}",
				sPIN_A, sPIN_RAD_G, sPIN_RAD_H, sPIN_SG, sPIN_BETA
				);
		}

	#endregion //methods

	}
}
