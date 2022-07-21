using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class LDAT : Variable
	{
	#region fields
		private double vEL;
		private double aCC;
		private double aPO_DIST;
		private double aPO_FAC;
		private double aXIS_VEL;
		private double aXIS_ACC;
		private ORI_TYPE oRI_TYP;
		private CIRC_TYPE cIRC_TYP;
		private double jERK_FAC;
		private double gEAR_JERK;
		private int eXAX_IGN;
		private CIRC_MODE cB;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "LDAT";} }
		public double VEL { get { return vEL; } set { Set(ref vEL, value); } }
		public double ACC { get { return aCC; } set { Set(ref aCC, value); } }
		public double APO_DIST { get { return aPO_DIST; } set { Set(ref aPO_DIST, value); } }
		public double APO_FAC { get { return aPO_FAC; } set { Set(ref aPO_FAC, value); } }
		public double AXIS_VEL { get { return aXIS_VEL; } set { Set(ref aXIS_VEL, value); } }
		public double AXIS_ACC { get { return aXIS_ACC; } set { Set(ref aXIS_ACC, value); } }
		public ORI_TYPE ORI_TYP { get { return oRI_TYP; } set { Set(ref oRI_TYP, value); } }
		public CIRC_TYPE CIRC_TYP { get { return cIRC_TYP; } set { Set(ref cIRC_TYP, value); } }
		public double JERK_FAC { get { return jERK_FAC; } set { Set(ref jERK_FAC, value); } }
		public double GEAR_JERK { get { return gEAR_JERK; } set { Set(ref gEAR_JERK, value); } }
		public int EXAX_IGN { get { return eXAX_IGN; } set { Set(ref eXAX_IGN, value); } }
		public CIRC_MODE CB { get { return cB; } set { Set(ref cB, value); } }
	#endregion //properties

	#region constructors
		public LDAT(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("VEL")) vEL = double.Parse(dataItems["VEL"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("ACC")) aCC = double.Parse(dataItems["ACC"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("APO_DIST")) aPO_DIST = double.Parse(dataItems["APO_DIST"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("APO_FAC")) aPO_FAC = double.Parse(dataItems["APO_FAC"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("AXIS_VEL")) aXIS_VEL = double.Parse(dataItems["AXIS_VEL"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("AXIS_ACC")) aXIS_ACC = double.Parse(dataItems["AXIS_ACC"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("ORI_TYP")) oRI_TYP = (ORI_TYPE)System.Enum.Parse(typeof(ORI_TYPE), dataItems["ORI_TYP"].ToString().TrimStart('#'), true);
			if (dataItems.ContainsKey("CIRC_TYP")) cIRC_TYP = (CIRC_TYPE)System.Enum.Parse(typeof(CIRC_TYPE), dataItems["CIRC_TYP"].ToString().TrimStart('#'), true);
			if (dataItems.ContainsKey("JERK_FAC")) jERK_FAC = double.Parse(dataItems["JERK_FAC"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("GEAR_JERK")) gEAR_JERK = double.Parse(dataItems["GEAR_JERK"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("EXAX_IGN")) eXAX_IGN = int.Parse(dataItems["EXAX_IGN"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("CB")) cB = new CIRC_MODE(dataItems["CB"]);
		}

		public LDAT(double VEL, double ACC, double APO_DIST, double APO_FAC, double AXIS_VEL, double AXIS_ACC, ORI_TYPE ORI_TYP, CIRC_TYPE CIRC_TYP, double JERK_FAC, double GEAR_JERK, int EXAX_IGN, CIRC_MODE CB, string valName="")
		{
			vEL = VEL;
			aCC = ACC;
			aPO_DIST = APO_DIST;
			aPO_FAC = APO_FAC;
			aXIS_VEL = AXIS_VEL;
			aXIS_ACC = AXIS_ACC;
			oRI_TYP = ORI_TYP;
			cIRC_TYP = CIRC_TYP;
			jERK_FAC = JERK_FAC;
			gEAR_JERK = GEAR_JERK;
			eXAX_IGN = EXAX_IGN;
			cB = CB;
			valName = ValName;
		}

		public LDAT(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["VEL"] != null) vEL = (double)mem["VEL"];
			if (mem["ACC"] != null) aCC = (double)mem["ACC"];
			if (mem["APO_DIST"] != null) aPO_DIST = (double)mem["APO_DIST"];
			if (mem["APO_FAC"] != null) aPO_FAC = (double)mem["APO_FAC"];
			if (mem["AXIS_VEL"] != null) aXIS_VEL = (double)mem["AXIS_VEL"];
			if (mem["AXIS_ACC"] != null) aXIS_ACC = (double)mem["AXIS_ACC"];
			if (mem["ORI_TYP"] != null) oRI_TYP = (ORI_TYPE)mem["ORI_TYP"];
			if (mem["CIRC_TYP"] != null) cIRC_TYP = (CIRC_TYPE)mem["CIRC_TYP"];
			if (mem["JERK_FAC"] != null) jERK_FAC = (double)mem["JERK_FAC"];
			if (mem["GEAR_JERK"] != null) gEAR_JERK = (double)mem["GEAR_JERK"];
			if (mem["EXAX_IGN"] != null) eXAX_IGN = (int)mem["EXAX_IGN"];
			cB = new CIRC_MODE((DynamicMemory)mem["CB"]);
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL LDAT " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				vEL.ToString(CultureInfo.InvariantCulture),
				aCC.ToString(CultureInfo.InvariantCulture),
				aPO_DIST.ToString(CultureInfo.InvariantCulture),
				aPO_FAC.ToString(CultureInfo.InvariantCulture),
				aXIS_VEL.ToString(CultureInfo.InvariantCulture),
				aXIS_ACC.ToString(CultureInfo.InvariantCulture),
				oRI_TYP.ToString(),
				cIRC_TYP.ToString(),
				jERK_FAC.ToString(CultureInfo.InvariantCulture),
				gEAR_JERK.ToString(CultureInfo.InvariantCulture),
				eXAX_IGN.ToString(CultureInfo.InvariantCulture),
				cB.ToString(),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{VEL {0},ACC {1},APO_DIST {2},APO_FAC {3},AXIS_VEL {4},AXIS_ACC {5},ORI_TYP {6},CIRC_TYP {7},JERK_FAC {8},GEAR_JERK {9},EXAX_IGN {10},CB {11}}}",
				vEL, aCC, aPO_DIST, aPO_FAC, aXIS_VEL, aXIS_ACC, "#" + oRI_TYP.ToString(), "#" + cIRC_TYP.ToString(), jERK_FAC, gEAR_JERK, eXAX_IGN, cB
				);
		}

	#endregion //methods

	}
}
