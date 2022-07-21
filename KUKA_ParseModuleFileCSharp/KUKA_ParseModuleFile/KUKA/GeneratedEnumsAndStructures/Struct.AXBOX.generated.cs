using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class AXBOX : Variable
	{
	#region fields
		private double a1_N;
		private double a1_P;
		private double a2_N;
		private double a2_P;
		private double a3_N;
		private double a3_P;
		private double a4_N;
		private double a4_P;
		private double a5_N;
		private double a5_P;
		private double a6_N;
		private double a6_P;
		private double e1_N;
		private double e1_P;
		private double e2_N;
		private double e2_P;
		private double e3_N;
		private double e3_P;
		private double e4_N;
		private double e4_P;
		private double e5_N;
		private double e5_P;
		private double e6_N;
		private double e6_P;
		private MODE mODE;
		private bool sTATE;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "AXBOX";} }
		public double A1_N { get { return a1_N; } set { Set(ref a1_N, value); } }
		public double A1_P { get { return a1_P; } set { Set(ref a1_P, value); } }
		public double A2_N { get { return a2_N; } set { Set(ref a2_N, value); } }
		public double A2_P { get { return a2_P; } set { Set(ref a2_P, value); } }
		public double A3_N { get { return a3_N; } set { Set(ref a3_N, value); } }
		public double A3_P { get { return a3_P; } set { Set(ref a3_P, value); } }
		public double A4_N { get { return a4_N; } set { Set(ref a4_N, value); } }
		public double A4_P { get { return a4_P; } set { Set(ref a4_P, value); } }
		public double A5_N { get { return a5_N; } set { Set(ref a5_N, value); } }
		public double A5_P { get { return a5_P; } set { Set(ref a5_P, value); } }
		public double A6_N { get { return a6_N; } set { Set(ref a6_N, value); } }
		public double A6_P { get { return a6_P; } set { Set(ref a6_P, value); } }
		public double E1_N { get { return e1_N; } set { Set(ref e1_N, value); } }
		public double E1_P { get { return e1_P; } set { Set(ref e1_P, value); } }
		public double E2_N { get { return e2_N; } set { Set(ref e2_N, value); } }
		public double E2_P { get { return e2_P; } set { Set(ref e2_P, value); } }
		public double E3_N { get { return e3_N; } set { Set(ref e3_N, value); } }
		public double E3_P { get { return e3_P; } set { Set(ref e3_P, value); } }
		public double E4_N { get { return e4_N; } set { Set(ref e4_N, value); } }
		public double E4_P { get { return e4_P; } set { Set(ref e4_P, value); } }
		public double E5_N { get { return e5_N; } set { Set(ref e5_N, value); } }
		public double E5_P { get { return e5_P; } set { Set(ref e5_P, value); } }
		public double E6_N { get { return e6_N; } set { Set(ref e6_N, value); } }
		public double E6_P { get { return e6_P; } set { Set(ref e6_P, value); } }
		public MODE MODE { get { return mODE; } set { Set(ref mODE, value); } }
		public bool STATE { get { return sTATE; } set { Set(ref sTATE, value); } }
	#endregion //properties

	#region constructors
		public AXBOX(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("A1_N")) a1_N = double.Parse(dataItems["A1_N"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("A1_P")) a1_P = double.Parse(dataItems["A1_P"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("A2_N")) a2_N = double.Parse(dataItems["A2_N"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("A2_P")) a2_P = double.Parse(dataItems["A2_P"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("A3_N")) a3_N = double.Parse(dataItems["A3_N"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("A3_P")) a3_P = double.Parse(dataItems["A3_P"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("A4_N")) a4_N = double.Parse(dataItems["A4_N"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("A4_P")) a4_P = double.Parse(dataItems["A4_P"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("A5_N")) a5_N = double.Parse(dataItems["A5_N"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("A5_P")) a5_P = double.Parse(dataItems["A5_P"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("A6_N")) a6_N = double.Parse(dataItems["A6_N"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("A6_P")) a6_P = double.Parse(dataItems["A6_P"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("E1_N")) e1_N = double.Parse(dataItems["E1_N"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("E1_P")) e1_P = double.Parse(dataItems["E1_P"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("E2_N")) e2_N = double.Parse(dataItems["E2_N"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("E2_P")) e2_P = double.Parse(dataItems["E2_P"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("E3_N")) e3_N = double.Parse(dataItems["E3_N"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("E3_P")) e3_P = double.Parse(dataItems["E3_P"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("E4_N")) e4_N = double.Parse(dataItems["E4_N"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("E4_P")) e4_P = double.Parse(dataItems["E4_P"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("E5_N")) e5_N = double.Parse(dataItems["E5_N"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("E5_P")) e5_P = double.Parse(dataItems["E5_P"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("E6_N")) e6_N = double.Parse(dataItems["E6_N"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("E6_P")) e6_P = double.Parse(dataItems["E6_P"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("MODE")) mODE = (MODE)System.Enum.Parse(typeof(MODE), dataItems["MODE"].ToString().TrimStart('#'), true);
			if (dataItems.ContainsKey("STATE")) sTATE = bool.Parse(dataItems["STATE"].ToString());
		}

		public AXBOX(double A1_N, double A1_P, double A2_N, double A2_P, double A3_N, double A3_P, double A4_N, double A4_P, double A5_N, double A5_P, double A6_N, double A6_P, double E1_N, double E1_P, double E2_N, double E2_P, double E3_N, double E3_P, double E4_N, double E4_P, double E5_N, double E5_P, double E6_N, double E6_P, MODE MODE, bool STATE, string valName="")
		{
			a1_N = A1_N;
			a1_P = A1_P;
			a2_N = A2_N;
			a2_P = A2_P;
			a3_N = A3_N;
			a3_P = A3_P;
			a4_N = A4_N;
			a4_P = A4_P;
			a5_N = A5_N;
			a5_P = A5_P;
			a6_N = A6_N;
			a6_P = A6_P;
			e1_N = E1_N;
			e1_P = E1_P;
			e2_N = E2_N;
			e2_P = E2_P;
			e3_N = E3_N;
			e3_P = E3_P;
			e4_N = E4_N;
			e4_P = E4_P;
			e5_N = E5_N;
			e5_P = E5_P;
			e6_N = E6_N;
			e6_P = E6_P;
			mODE = MODE;
			sTATE = STATE;
			valName = ValName;
		}

		public AXBOX(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["A1_N"] != null) a1_N = (double)mem["A1_N"];
			if (mem["A1_P"] != null) a1_P = (double)mem["A1_P"];
			if (mem["A2_N"] != null) a2_N = (double)mem["A2_N"];
			if (mem["A2_P"] != null) a2_P = (double)mem["A2_P"];
			if (mem["A3_N"] != null) a3_N = (double)mem["A3_N"];
			if (mem["A3_P"] != null) a3_P = (double)mem["A3_P"];
			if (mem["A4_N"] != null) a4_N = (double)mem["A4_N"];
			if (mem["A4_P"] != null) a4_P = (double)mem["A4_P"];
			if (mem["A5_N"] != null) a5_N = (double)mem["A5_N"];
			if (mem["A5_P"] != null) a5_P = (double)mem["A5_P"];
			if (mem["A6_N"] != null) a6_N = (double)mem["A6_N"];
			if (mem["A6_P"] != null) a6_P = (double)mem["A6_P"];
			if (mem["E1_N"] != null) e1_N = (double)mem["E1_N"];
			if (mem["E1_P"] != null) e1_P = (double)mem["E1_P"];
			if (mem["E2_N"] != null) e2_N = (double)mem["E2_N"];
			if (mem["E2_P"] != null) e2_P = (double)mem["E2_P"];
			if (mem["E3_N"] != null) e3_N = (double)mem["E3_N"];
			if (mem["E3_P"] != null) e3_P = (double)mem["E3_P"];
			if (mem["E4_N"] != null) e4_N = (double)mem["E4_N"];
			if (mem["E4_P"] != null) e4_P = (double)mem["E4_P"];
			if (mem["E5_N"] != null) e5_N = (double)mem["E5_N"];
			if (mem["E5_P"] != null) e5_P = (double)mem["E5_P"];
			if (mem["E6_N"] != null) e6_N = (double)mem["E6_N"];
			if (mem["E6_P"] != null) e6_P = (double)mem["E6_P"];
			if (mem["MODE"] != null) mODE = (MODE)mem["MODE"];
			if (mem["STATE"] != null) sTATE = (bool)mem["STATE"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL AXBOX " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				a1_N.ToString(CultureInfo.InvariantCulture),
				a1_P.ToString(CultureInfo.InvariantCulture),
				a2_N.ToString(CultureInfo.InvariantCulture),
				a2_P.ToString(CultureInfo.InvariantCulture),
				a3_N.ToString(CultureInfo.InvariantCulture),
				a3_P.ToString(CultureInfo.InvariantCulture),
				a4_N.ToString(CultureInfo.InvariantCulture),
				a4_P.ToString(CultureInfo.InvariantCulture),
				a5_N.ToString(CultureInfo.InvariantCulture),
				a5_P.ToString(CultureInfo.InvariantCulture),
				a6_N.ToString(CultureInfo.InvariantCulture),
				a6_P.ToString(CultureInfo.InvariantCulture),
				e1_N.ToString(CultureInfo.InvariantCulture),
				e1_P.ToString(CultureInfo.InvariantCulture),
				e2_N.ToString(CultureInfo.InvariantCulture),
				e2_P.ToString(CultureInfo.InvariantCulture),
				e3_N.ToString(CultureInfo.InvariantCulture),
				e3_P.ToString(CultureInfo.InvariantCulture),
				e4_N.ToString(CultureInfo.InvariantCulture),
				e4_P.ToString(CultureInfo.InvariantCulture),
				e5_N.ToString(CultureInfo.InvariantCulture),
				e5_P.ToString(CultureInfo.InvariantCulture),
				e6_N.ToString(CultureInfo.InvariantCulture),
				e6_P.ToString(CultureInfo.InvariantCulture),
				mODE.ToString(),
				sTATE.ToString(CultureInfo.InvariantCulture),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{A1_N {0},A1_P {1},A2_N {2},A2_P {3},A3_N {4},A3_P {5},A4_N {6},A4_P {7},A5_N {8},A5_P {9},A6_N {10},A6_P {11},E1_N {12},E1_P {13},E2_N {14},E2_P {15},E3_N {16},E3_P {17},E4_N {18},E4_P {19},E5_N {20},E5_P {21},E6_N {22},E6_P {23},MODE {24},STATE {25}}}",
				a1_N, a1_P, a2_N, a2_P, a3_N, a3_P, a4_N, a4_P, a5_N, a5_P, a6_N, a6_P, e1_N, e1_P, e2_N, e2_P, e3_N, e3_P, e4_N, e4_P, e5_N, e5_P, e6_N, e6_P, "#" + mODE.ToString(), BtoStr(sTATE)
				);
		}

	#endregion //methods

	}
}
