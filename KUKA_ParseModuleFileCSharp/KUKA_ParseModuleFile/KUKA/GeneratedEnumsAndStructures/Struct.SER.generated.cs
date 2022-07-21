using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class SER : Variable
	{
	#region fields
		private int pROTO;
		private int pROC;
		private int rCO;
		private int bL;
		private int bAUD;
		private int nOC;
		private int nOS;
		private int pARITY;
		private int tRC;
		private int fLP;
		private int lLP;
		private int rT;
		private int pT;
		private int dSR;
		private int wCCXON;
		private int vXON;
		private int vNOFF;
		private int wEOBC;
		private int vEOBC;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "SER";} }
		public int PROTO { get { return pROTO; } set { Set(ref pROTO, value); } }
		public int PROC { get { return pROC; } set { Set(ref pROC, value); } }
		public int RCO { get { return rCO; } set { Set(ref rCO, value); } }
		public int BL { get { return bL; } set { Set(ref bL, value); } }
		public int BAUD { get { return bAUD; } set { Set(ref bAUD, value); } }
		public int NOC { get { return nOC; } set { Set(ref nOC, value); } }
		public int NOS { get { return nOS; } set { Set(ref nOS, value); } }
		public int PARITY { get { return pARITY; } set { Set(ref pARITY, value); } }
		public int TRC { get { return tRC; } set { Set(ref tRC, value); } }
		public int FLP { get { return fLP; } set { Set(ref fLP, value); } }
		public int LLP { get { return lLP; } set { Set(ref lLP, value); } }
		public int RT { get { return rT; } set { Set(ref rT, value); } }
		public int PT { get { return pT; } set { Set(ref pT, value); } }
		public int DSR { get { return dSR; } set { Set(ref dSR, value); } }
		public int WCCXON { get { return wCCXON; } set { Set(ref wCCXON, value); } }
		public int VXON { get { return vXON; } set { Set(ref vXON, value); } }
		public int VNOFF { get { return vNOFF; } set { Set(ref vNOFF, value); } }
		public int WEOBC { get { return wEOBC; } set { Set(ref wEOBC, value); } }
		public int VEOBC { get { return vEOBC; } set { Set(ref vEOBC, value); } }
	#endregion //properties

	#region constructors
		public SER(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("PROTO")) pROTO = int.Parse(dataItems["PROTO"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("PROC")) pROC = int.Parse(dataItems["PROC"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("RCO")) rCO = int.Parse(dataItems["RCO"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("BL")) bL = int.Parse(dataItems["BL"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("BAUD")) bAUD = int.Parse(dataItems["BAUD"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("NOC")) nOC = int.Parse(dataItems["NOC"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("NOS")) nOS = int.Parse(dataItems["NOS"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("PARITY")) pARITY = int.Parse(dataItems["PARITY"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("TRC")) tRC = int.Parse(dataItems["TRC"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("FLP")) fLP = int.Parse(dataItems["FLP"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("LLP")) lLP = int.Parse(dataItems["LLP"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("RT")) rT = int.Parse(dataItems["RT"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("PT")) pT = int.Parse(dataItems["PT"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("DSR")) dSR = int.Parse(dataItems["DSR"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("WCCXON")) wCCXON = int.Parse(dataItems["WCCXON"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("VXON")) vXON = int.Parse(dataItems["VXON"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("VNOFF")) vNOFF = int.Parse(dataItems["VNOFF"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("WEOBC")) wEOBC = int.Parse(dataItems["WEOBC"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("VEOBC")) vEOBC = int.Parse(dataItems["VEOBC"].ToString(), CultureInfo.InvariantCulture);
		}

		public SER(int PROTO, int PROC, int RCO, int BL, int BAUD, int NOC, int NOS, int PARITY, int TRC, int FLP, int LLP, int RT, int PT, int DSR, int WCCXON, int VXON, int VNOFF, int WEOBC, int VEOBC, string valName="")
		{
			pROTO = PROTO;
			pROC = PROC;
			rCO = RCO;
			bL = BL;
			bAUD = BAUD;
			nOC = NOC;
			nOS = NOS;
			pARITY = PARITY;
			tRC = TRC;
			fLP = FLP;
			lLP = LLP;
			rT = RT;
			pT = PT;
			dSR = DSR;
			wCCXON = WCCXON;
			vXON = VXON;
			vNOFF = VNOFF;
			wEOBC = WEOBC;
			vEOBC = VEOBC;
			valName = ValName;
		}

		public SER(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["PROTO"] != null) pROTO = (int)mem["PROTO"];
			if (mem["PROC"] != null) pROC = (int)mem["PROC"];
			if (mem["RCO"] != null) rCO = (int)mem["RCO"];
			if (mem["BL"] != null) bL = (int)mem["BL"];
			if (mem["BAUD"] != null) bAUD = (int)mem["BAUD"];
			if (mem["NOC"] != null) nOC = (int)mem["NOC"];
			if (mem["NOS"] != null) nOS = (int)mem["NOS"];
			if (mem["PARITY"] != null) pARITY = (int)mem["PARITY"];
			if (mem["TRC"] != null) tRC = (int)mem["TRC"];
			if (mem["FLP"] != null) fLP = (int)mem["FLP"];
			if (mem["LLP"] != null) lLP = (int)mem["LLP"];
			if (mem["RT"] != null) rT = (int)mem["RT"];
			if (mem["PT"] != null) pT = (int)mem["PT"];
			if (mem["DSR"] != null) dSR = (int)mem["DSR"];
			if (mem["WCCXON"] != null) wCCXON = (int)mem["WCCXON"];
			if (mem["VXON"] != null) vXON = (int)mem["VXON"];
			if (mem["VNOFF"] != null) vNOFF = (int)mem["VNOFF"];
			if (mem["WEOBC"] != null) wEOBC = (int)mem["WEOBC"];
			if (mem["VEOBC"] != null) vEOBC = (int)mem["VEOBC"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL SER " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				pROTO.ToString(CultureInfo.InvariantCulture),
				pROC.ToString(CultureInfo.InvariantCulture),
				rCO.ToString(CultureInfo.InvariantCulture),
				bL.ToString(CultureInfo.InvariantCulture),
				bAUD.ToString(CultureInfo.InvariantCulture),
				nOC.ToString(CultureInfo.InvariantCulture),
				nOS.ToString(CultureInfo.InvariantCulture),
				pARITY.ToString(CultureInfo.InvariantCulture),
				tRC.ToString(CultureInfo.InvariantCulture),
				fLP.ToString(CultureInfo.InvariantCulture),
				lLP.ToString(CultureInfo.InvariantCulture),
				rT.ToString(CultureInfo.InvariantCulture),
				pT.ToString(CultureInfo.InvariantCulture),
				dSR.ToString(CultureInfo.InvariantCulture),
				wCCXON.ToString(CultureInfo.InvariantCulture),
				vXON.ToString(CultureInfo.InvariantCulture),
				vNOFF.ToString(CultureInfo.InvariantCulture),
				wEOBC.ToString(CultureInfo.InvariantCulture),
				vEOBC.ToString(CultureInfo.InvariantCulture),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{PROTO {0},PROC {1},RCO {2},BL {3},BAUD {4},NOC {5},NOS {6},PARITY {7},TRC {8},FLP {9},LLP {10},RT {11},PT {12},DSR {13},WCCXON {14},VXON {15},VNOFF {16},WEOBC {17},VEOBC {18}}}",
				pROTO, pROC, rCO, bL, bAUD, nOC, nOS, pARITY, tRC, fLP, lLP, rT, pT, dSR, wCCXON, vXON, vNOFF, wEOBC, vEOBC
				);
		}

	#endregion //methods

	}
}
