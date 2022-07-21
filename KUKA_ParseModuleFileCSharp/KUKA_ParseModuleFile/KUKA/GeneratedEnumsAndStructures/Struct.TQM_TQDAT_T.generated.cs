using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class TQM_TQDAT_T : Variable
	{
	#region fields
		private int t11;
		private int t12;
		private int t13;
		private int t14;
		private int t15;
		private int t16;
		private int t21;
		private int t22;
		private int t23;
		private int t24;
		private int t25;
		private int t26;
		private int k1;
		private int k2;
		private int k3;
		private int k4;
		private int k5;
		private int k6;
		private int o1;
		private int o2;
		private int iD;
		private int oVM;
		private double tMF;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "TQM_TQDAT_T";} }
		public int T11 { get { return t11; } set { Set(ref t11, value); } }
		public int T12 { get { return t12; } set { Set(ref t12, value); } }
		public int T13 { get { return t13; } set { Set(ref t13, value); } }
		public int T14 { get { return t14; } set { Set(ref t14, value); } }
		public int T15 { get { return t15; } set { Set(ref t15, value); } }
		public int T16 { get { return t16; } set { Set(ref t16, value); } }
		public int T21 { get { return t21; } set { Set(ref t21, value); } }
		public int T22 { get { return t22; } set { Set(ref t22, value); } }
		public int T23 { get { return t23; } set { Set(ref t23, value); } }
		public int T24 { get { return t24; } set { Set(ref t24, value); } }
		public int T25 { get { return t25; } set { Set(ref t25, value); } }
		public int T26 { get { return t26; } set { Set(ref t26, value); } }
		public int K1 { get { return k1; } set { Set(ref k1, value); } }
		public int K2 { get { return k2; } set { Set(ref k2, value); } }
		public int K3 { get { return k3; } set { Set(ref k3, value); } }
		public int K4 { get { return k4; } set { Set(ref k4, value); } }
		public int K5 { get { return k5; } set { Set(ref k5, value); } }
		public int K6 { get { return k6; } set { Set(ref k6, value); } }
		public int O1 { get { return o1; } set { Set(ref o1, value); } }
		public int O2 { get { return o2; } set { Set(ref o2, value); } }
		public int ID { get { return iD; } set { Set(ref iD, value); } }
		public int OVM { get { return oVM; } set { Set(ref oVM, value); } }
		public double TMF { get { return tMF; } set { Set(ref tMF, value); } }
	#endregion //properties

	#region constructors
		public TQM_TQDAT_T(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("T11")) t11 = int.Parse(dataItems["T11"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("T12")) t12 = int.Parse(dataItems["T12"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("T13")) t13 = int.Parse(dataItems["T13"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("T14")) t14 = int.Parse(dataItems["T14"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("T15")) t15 = int.Parse(dataItems["T15"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("T16")) t16 = int.Parse(dataItems["T16"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("T21")) t21 = int.Parse(dataItems["T21"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("T22")) t22 = int.Parse(dataItems["T22"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("T23")) t23 = int.Parse(dataItems["T23"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("T24")) t24 = int.Parse(dataItems["T24"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("T25")) t25 = int.Parse(dataItems["T25"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("T26")) t26 = int.Parse(dataItems["T26"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("K1")) k1 = int.Parse(dataItems["K1"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("K2")) k2 = int.Parse(dataItems["K2"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("K3")) k3 = int.Parse(dataItems["K3"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("K4")) k4 = int.Parse(dataItems["K4"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("K5")) k5 = int.Parse(dataItems["K5"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("K6")) k6 = int.Parse(dataItems["K6"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("O1")) o1 = int.Parse(dataItems["O1"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("O2")) o2 = int.Parse(dataItems["O2"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("ID")) iD = int.Parse(dataItems["ID"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("OVM")) oVM = int.Parse(dataItems["OVM"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("TMF")) tMF = double.Parse(dataItems["TMF"].ToString(), CultureInfo.InvariantCulture);
		}

		public TQM_TQDAT_T(int T11, int T12, int T13, int T14, int T15, int T16, int T21, int T22, int T23, int T24, int T25, int T26, int K1, int K2, int K3, int K4, int K5, int K6, int O1, int O2, int ID, int OVM, double TMF, string valName="")
		{
			t11 = T11;
			t12 = T12;
			t13 = T13;
			t14 = T14;
			t15 = T15;
			t16 = T16;
			t21 = T21;
			t22 = T22;
			t23 = T23;
			t24 = T24;
			t25 = T25;
			t26 = T26;
			k1 = K1;
			k2 = K2;
			k3 = K3;
			k4 = K4;
			k5 = K5;
			k6 = K6;
			o1 = O1;
			o2 = O2;
			iD = ID;
			oVM = OVM;
			tMF = TMF;
			valName = ValName;
		}

		public TQM_TQDAT_T(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["T11"] != null) t11 = (int)mem["T11"];
			if (mem["T12"] != null) t12 = (int)mem["T12"];
			if (mem["T13"] != null) t13 = (int)mem["T13"];
			if (mem["T14"] != null) t14 = (int)mem["T14"];
			if (mem["T15"] != null) t15 = (int)mem["T15"];
			if (mem["T16"] != null) t16 = (int)mem["T16"];
			if (mem["T21"] != null) t21 = (int)mem["T21"];
			if (mem["T22"] != null) t22 = (int)mem["T22"];
			if (mem["T23"] != null) t23 = (int)mem["T23"];
			if (mem["T24"] != null) t24 = (int)mem["T24"];
			if (mem["T25"] != null) t25 = (int)mem["T25"];
			if (mem["T26"] != null) t26 = (int)mem["T26"];
			if (mem["K1"] != null) k1 = (int)mem["K1"];
			if (mem["K2"] != null) k2 = (int)mem["K2"];
			if (mem["K3"] != null) k3 = (int)mem["K3"];
			if (mem["K4"] != null) k4 = (int)mem["K4"];
			if (mem["K5"] != null) k5 = (int)mem["K5"];
			if (mem["K6"] != null) k6 = (int)mem["K6"];
			if (mem["O1"] != null) o1 = (int)mem["O1"];
			if (mem["O2"] != null) o2 = (int)mem["O2"];
			if (mem["ID"] != null) iD = (int)mem["ID"];
			if (mem["OVM"] != null) oVM = (int)mem["OVM"];
			if (mem["TMF"] != null) tMF = (double)mem["TMF"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL TQM_TQDAT_T " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				t11.ToString(CultureInfo.InvariantCulture),
				t12.ToString(CultureInfo.InvariantCulture),
				t13.ToString(CultureInfo.InvariantCulture),
				t14.ToString(CultureInfo.InvariantCulture),
				t15.ToString(CultureInfo.InvariantCulture),
				t16.ToString(CultureInfo.InvariantCulture),
				t21.ToString(CultureInfo.InvariantCulture),
				t22.ToString(CultureInfo.InvariantCulture),
				t23.ToString(CultureInfo.InvariantCulture),
				t24.ToString(CultureInfo.InvariantCulture),
				t25.ToString(CultureInfo.InvariantCulture),
				t26.ToString(CultureInfo.InvariantCulture),
				k1.ToString(CultureInfo.InvariantCulture),
				k2.ToString(CultureInfo.InvariantCulture),
				k3.ToString(CultureInfo.InvariantCulture),
				k4.ToString(CultureInfo.InvariantCulture),
				k5.ToString(CultureInfo.InvariantCulture),
				k6.ToString(CultureInfo.InvariantCulture),
				o1.ToString(CultureInfo.InvariantCulture),
				o2.ToString(CultureInfo.InvariantCulture),
				iD.ToString(CultureInfo.InvariantCulture),
				oVM.ToString(CultureInfo.InvariantCulture),
				tMF.ToString(CultureInfo.InvariantCulture),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{T11 {0},T12 {1},T13 {2},T14 {3},T15 {4},T16 {5},T21 {6},T22 {7},T23 {8},T24 {9},T25 {10},T26 {11},K1 {12},K2 {13},K3 {14},K4 {15},K5 {16},K6 {17},O1 {18},O2 {19},ID {20},OVM {21},TMF {22}}}",
				t11, t12, t13, t14, t15, t16, t21, t22, t23, t24, t25, t26, k1, k2, k3, k4, k5, k6, o1, o2, iD, oVM, tMF
				);
		}

	#endregion //methods

	}
}
