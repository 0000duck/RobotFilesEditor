using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class PDAT : Variable
	{
	#region fields
		private double vEL;
		private double aCC;
		private double aPO_DIST;
		private APO_MODE_T aPO_MODE;
		private double gEAR_JERK;
		private int eXAX_IGN;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "PDAT";} }
		public double VEL { get { return vEL; } set { Set(ref vEL, value); } }
		public double ACC { get { return aCC; } set { Set(ref aCC, value); } }
		public double APO_DIST { get { return aPO_DIST; } set { Set(ref aPO_DIST, value); } }
		public APO_MODE_T APO_MODE { get { return aPO_MODE; } set { Set(ref aPO_MODE, value); } }
		public double GEAR_JERK { get { return gEAR_JERK; } set { Set(ref gEAR_JERK, value); } }
		public int EXAX_IGN { get { return eXAX_IGN; } set { Set(ref eXAX_IGN, value); } }
	#endregion //properties

	#region constructors
		public PDAT(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("VEL")) vEL = double.Parse(dataItems["VEL"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("ACC")) aCC = double.Parse(dataItems["ACC"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("APO_DIST")) aPO_DIST = double.Parse(dataItems["APO_DIST"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("APO_MODE")) aPO_MODE = (APO_MODE_T)System.Enum.Parse(typeof(APO_MODE_T), dataItems["APO_MODE"].ToString().TrimStart('#'), true);
			if (dataItems.ContainsKey("GEAR_JERK")) gEAR_JERK = double.Parse(dataItems["GEAR_JERK"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("EXAX_IGN")) eXAX_IGN = int.Parse(dataItems["EXAX_IGN"].ToString(), CultureInfo.InvariantCulture);
		}

		public PDAT(double VEL, double ACC, double APO_DIST, APO_MODE_T APO_MODE, double GEAR_JERK, int EXAX_IGN, string valName="")
		{
			vEL = VEL;
			aCC = ACC;
			aPO_DIST = APO_DIST;
			aPO_MODE = APO_MODE;
			gEAR_JERK = GEAR_JERK;
			eXAX_IGN = EXAX_IGN;
			valName = ValName;
		}

		public PDAT(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["VEL"] != null) vEL = (double)mem["VEL"];
			if (mem["ACC"] != null) aCC = (double)mem["ACC"];
			if (mem["APO_DIST"] != null) aPO_DIST = (double)mem["APO_DIST"];
			if (mem["APO_MODE"] != null) aPO_MODE = (APO_MODE_T)mem["APO_MODE"];
			if (mem["GEAR_JERK"] != null) gEAR_JERK = (double)mem["GEAR_JERK"];
			if (mem["EXAX_IGN"] != null) eXAX_IGN = (int)mem["EXAX_IGN"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL PDAT " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				vEL.ToString(CultureInfo.InvariantCulture),
				aCC.ToString(CultureInfo.InvariantCulture),
				aPO_DIST.ToString(CultureInfo.InvariantCulture),
				aPO_MODE.ToString(),
				gEAR_JERK.ToString(CultureInfo.InvariantCulture),
				eXAX_IGN.ToString(CultureInfo.InvariantCulture),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{VEL {0},ACC {1},APO_DIST {2},APO_MODE {3},GEAR_JERK {4},EXAX_IGN {5}}}",
				vEL, aCC, aPO_DIST, "#" + aPO_MODE.ToString(), gEAR_JERK, eXAX_IGN
				);
		}

	#endregion //methods

	}
}
