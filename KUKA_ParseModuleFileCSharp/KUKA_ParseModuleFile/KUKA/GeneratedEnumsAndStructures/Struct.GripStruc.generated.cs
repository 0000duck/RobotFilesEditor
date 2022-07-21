using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class GripStruc : Variable
	{
	#region fields
		private int num;
		private string name;
		private int pP1;
		private int pP2;
		private int pP3;
		private int pP4;
		private int pP5;
		private int pP6;
		private int pP7;
		private int pP8;
		private int pP9;
		private int pP10;
		private int pP11;
		private int pP12;
		private int pP13;
		private int pP14;
		private int pP15;
		private int pP16;
		private bool i_PrSwUsed;
		private int i_PrSw;
		private int o_SafeValve;
		private bool isTimeOut;
		private bool isConfig;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "GripStruc";} }
		public int Num { get { return num; } set { Set(ref num, value); } }
		public string Name { get { return name; } set { Set(ref name, value); } }
		public int PP1 { get { return pP1; } set { Set(ref pP1, value); } }
		public int PP2 { get { return pP2; } set { Set(ref pP2, value); } }
		public int PP3 { get { return pP3; } set { Set(ref pP3, value); } }
		public int PP4 { get { return pP4; } set { Set(ref pP4, value); } }
		public int PP5 { get { return pP5; } set { Set(ref pP5, value); } }
		public int PP6 { get { return pP6; } set { Set(ref pP6, value); } }
		public int PP7 { get { return pP7; } set { Set(ref pP7, value); } }
		public int PP8 { get { return pP8; } set { Set(ref pP8, value); } }
		public int PP9 { get { return pP9; } set { Set(ref pP9, value); } }
		public int PP10 { get { return pP10; } set { Set(ref pP10, value); } }
		public int PP11 { get { return pP11; } set { Set(ref pP11, value); } }
		public int PP12 { get { return pP12; } set { Set(ref pP12, value); } }
		public int PP13 { get { return pP13; } set { Set(ref pP13, value); } }
		public int PP14 { get { return pP14; } set { Set(ref pP14, value); } }
		public int PP15 { get { return pP15; } set { Set(ref pP15, value); } }
		public int PP16 { get { return pP16; } set { Set(ref pP16, value); } }
		public bool I_PrSwUsed { get { return i_PrSwUsed; } set { Set(ref i_PrSwUsed, value); } }
		public int I_PrSw { get { return i_PrSw; } set { Set(ref i_PrSw, value); } }
		public int O_SafeValve { get { return o_SafeValve; } set { Set(ref o_SafeValve, value); } }
		public bool IsTimeOut { get { return isTimeOut; } set { Set(ref isTimeOut, value); } }
		public bool IsConfig { get { return isConfig; } set { Set(ref isConfig, value); } }
	#endregion //properties

	#region constructors
		public GripStruc(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("Num")) num = int.Parse(dataItems["Num"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("Name")) name = dataItems["Name"].ToString().Trim('"');
			if (dataItems.ContainsKey("PP1")) pP1 = int.Parse(dataItems["PP1"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("PP2")) pP2 = int.Parse(dataItems["PP2"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("PP3")) pP3 = int.Parse(dataItems["PP3"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("PP4")) pP4 = int.Parse(dataItems["PP4"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("PP5")) pP5 = int.Parse(dataItems["PP5"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("PP6")) pP6 = int.Parse(dataItems["PP6"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("PP7")) pP7 = int.Parse(dataItems["PP7"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("PP8")) pP8 = int.Parse(dataItems["PP8"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("PP9")) pP9 = int.Parse(dataItems["PP9"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("PP10")) pP10 = int.Parse(dataItems["PP10"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("PP11")) pP11 = int.Parse(dataItems["PP11"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("PP12")) pP12 = int.Parse(dataItems["PP12"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("PP13")) pP13 = int.Parse(dataItems["PP13"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("PP14")) pP14 = int.Parse(dataItems["PP14"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("PP15")) pP15 = int.Parse(dataItems["PP15"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("PP16")) pP16 = int.Parse(dataItems["PP16"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("I_PrSwUsed")) i_PrSwUsed = bool.Parse(dataItems["I_PrSwUsed"].ToString());
			if (dataItems.ContainsKey("I_PrSw")) i_PrSw = int.Parse(dataItems["I_PrSw"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("O_SafeValve")) o_SafeValve = int.Parse(dataItems["O_SafeValve"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("IsTimeOut")) isTimeOut = bool.Parse(dataItems["IsTimeOut"].ToString());
			if (dataItems.ContainsKey("IsConfig")) isConfig = bool.Parse(dataItems["IsConfig"].ToString());
		}

		public GripStruc(int Num, string Name, int PP1, int PP2, int PP3, int PP4, int PP5, int PP6, int PP7, int PP8, int PP9, int PP10, int PP11, int PP12, int PP13, int PP14, int PP15, int PP16, bool I_PrSwUsed, int I_PrSw, int O_SafeValve, bool IsTimeOut, bool IsConfig, string valName="")
		{
			num = Num;
			name = Name;
			pP1 = PP1;
			pP2 = PP2;
			pP3 = PP3;
			pP4 = PP4;
			pP5 = PP5;
			pP6 = PP6;
			pP7 = PP7;
			pP8 = PP8;
			pP9 = PP9;
			pP10 = PP10;
			pP11 = PP11;
			pP12 = PP12;
			pP13 = PP13;
			pP14 = PP14;
			pP15 = PP15;
			pP16 = PP16;
			i_PrSwUsed = I_PrSwUsed;
			i_PrSw = I_PrSw;
			o_SafeValve = O_SafeValve;
			isTimeOut = IsTimeOut;
			isConfig = IsConfig;
			valName = ValName;
		}

		public GripStruc(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["Num"] != null) num = (int)mem["Num"];
			if (mem["Name"] != null) name = (string)mem["Name"];
			if (mem["PP1"] != null) pP1 = (int)mem["PP1"];
			if (mem["PP2"] != null) pP2 = (int)mem["PP2"];
			if (mem["PP3"] != null) pP3 = (int)mem["PP3"];
			if (mem["PP4"] != null) pP4 = (int)mem["PP4"];
			if (mem["PP5"] != null) pP5 = (int)mem["PP5"];
			if (mem["PP6"] != null) pP6 = (int)mem["PP6"];
			if (mem["PP7"] != null) pP7 = (int)mem["PP7"];
			if (mem["PP8"] != null) pP8 = (int)mem["PP8"];
			if (mem["PP9"] != null) pP9 = (int)mem["PP9"];
			if (mem["PP10"] != null) pP10 = (int)mem["PP10"];
			if (mem["PP11"] != null) pP11 = (int)mem["PP11"];
			if (mem["PP12"] != null) pP12 = (int)mem["PP12"];
			if (mem["PP13"] != null) pP13 = (int)mem["PP13"];
			if (mem["PP14"] != null) pP14 = (int)mem["PP14"];
			if (mem["PP15"] != null) pP15 = (int)mem["PP15"];
			if (mem["PP16"] != null) pP16 = (int)mem["PP16"];
			if (mem["I_PrSwUsed"] != null) i_PrSwUsed = (bool)mem["I_PrSwUsed"];
			if (mem["I_PrSw"] != null) i_PrSw = (int)mem["I_PrSw"];
			if (mem["O_SafeValve"] != null) o_SafeValve = (int)mem["O_SafeValve"];
			if (mem["IsTimeOut"] != null) isTimeOut = (bool)mem["IsTimeOut"];
			if (mem["IsConfig"] != null) isConfig = (bool)mem["IsConfig"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL GripStruc " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				num.ToString(CultureInfo.InvariantCulture),
				name,
				pP1.ToString(CultureInfo.InvariantCulture),
				pP2.ToString(CultureInfo.InvariantCulture),
				pP3.ToString(CultureInfo.InvariantCulture),
				pP4.ToString(CultureInfo.InvariantCulture),
				pP5.ToString(CultureInfo.InvariantCulture),
				pP6.ToString(CultureInfo.InvariantCulture),
				pP7.ToString(CultureInfo.InvariantCulture),
				pP8.ToString(CultureInfo.InvariantCulture),
				pP9.ToString(CultureInfo.InvariantCulture),
				pP10.ToString(CultureInfo.InvariantCulture),
				pP11.ToString(CultureInfo.InvariantCulture),
				pP12.ToString(CultureInfo.InvariantCulture),
				pP13.ToString(CultureInfo.InvariantCulture),
				pP14.ToString(CultureInfo.InvariantCulture),
				pP15.ToString(CultureInfo.InvariantCulture),
				pP16.ToString(CultureInfo.InvariantCulture),
				i_PrSwUsed.ToString(CultureInfo.InvariantCulture),
				i_PrSw.ToString(CultureInfo.InvariantCulture),
				o_SafeValve.ToString(CultureInfo.InvariantCulture),
				isTimeOut.ToString(CultureInfo.InvariantCulture),
				isConfig.ToString(CultureInfo.InvariantCulture),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{Num {0},Name[] \"{1}\",PP1 {2},PP2 {3},PP3 {4},PP4 {5},PP5 {6},PP6 {7},PP7 {8},PP8 {9},PP9 {10},PP10 {11},PP11 {12},PP12 {13},PP13 {14},PP14 {15},PP15 {16},PP16 {17},I_PrSwUsed {18},I_PrSw {19},O_SafeValve {20},IsTimeOut {21},IsConfig {22}}}",
				num, name, pP1, pP2, pP3, pP4, pP5, pP6, pP7, pP8, pP9, pP10, pP11, pP12, pP13, pP14, pP15, pP16, BtoStr(i_PrSwUsed), i_PrSw, o_SafeValve, BtoStr(isTimeOut), BtoStr(isConfig)
				);
		}

	#endregion //methods

	}
}
