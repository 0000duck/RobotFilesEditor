using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class Gun : Variable
	{
	#region fields
		private int num;
		private swp_GUN_MOUNTING mountingWeld;
		private int weldTimerContr;
		private int tipDresserContr;
		private int tipDrRelayState;
		private int electrodeNo;
		private OPTION_CTL fastCloseOpt;
		private OPTION_CTL fastLeaveOpt;
		private bool equalBack;
		private bool equalBackFl;
		private double manMvSTEP0;
		private double manMvSTEP1;
		private double manMvSTEP2;
		private double lastOpenM;
		private double minOpenM;
		private double maxOpenM;
		private double homeEqualPr;
		private bool polishWOpen;
		private bool polishOption;
		private double gunOpenTime;
		private bool tipDressListOption;
		private int dressCounter;
		private int dressCounterMax;
		private double polishTime;
		private double polishForce;
		private ObservableDictionary<int,TIPDRESS> dressVar;
		private double calEqPressure_min;
		private double calEqPressure_max;
		private double calEqPressure_stat;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "Gun";} }
		public int Num { get { return num; } set { Set(ref num, value); } }
		public swp_GUN_MOUNTING MountingWeld { get { return mountingWeld; } set { Set(ref mountingWeld, value); } }
		public int WeldTimerContr { get { return weldTimerContr; } set { Set(ref weldTimerContr, value); } }
		public int TipDresserContr { get { return tipDresserContr; } set { Set(ref tipDresserContr, value); } }
		public int TipDrRelayState { get { return tipDrRelayState; } set { Set(ref tipDrRelayState, value); } }
		public int ElectrodeNo { get { return electrodeNo; } set { Set(ref electrodeNo, value); } }
		public OPTION_CTL FastCloseOpt { get { return fastCloseOpt; } set { Set(ref fastCloseOpt, value); } }
		public OPTION_CTL FastLeaveOpt { get { return fastLeaveOpt; } set { Set(ref fastLeaveOpt, value); } }
		public bool EqualBack { get { return equalBack; } set { Set(ref equalBack, value); } }
		public bool EqualBackFl { get { return equalBackFl; } set { Set(ref equalBackFl, value); } }
		public double ManMvSTEP0 { get { return manMvSTEP0; } set { Set(ref manMvSTEP0, value); } }
		public double ManMvSTEP1 { get { return manMvSTEP1; } set { Set(ref manMvSTEP1, value); } }
		public double ManMvSTEP2 { get { return manMvSTEP2; } set { Set(ref manMvSTEP2, value); } }
		public double LastOpenM { get { return lastOpenM; } set { Set(ref lastOpenM, value); } }
		public double MinOpenM { get { return minOpenM; } set { Set(ref minOpenM, value); } }
		public double MaxOpenM { get { return maxOpenM; } set { Set(ref maxOpenM, value); } }
		public double HomeEqualPr { get { return homeEqualPr; } set { Set(ref homeEqualPr, value); } }
		public bool PolishWOpen { get { return polishWOpen; } set { Set(ref polishWOpen, value); } }
		public bool PolishOption { get { return polishOption; } set { Set(ref polishOption, value); } }
		public double GunOpenTime { get { return gunOpenTime; } set { Set(ref gunOpenTime, value); } }
		public bool TipDressListOption { get { return tipDressListOption; } set { Set(ref tipDressListOption, value); } }
		public int DressCounter { get { return dressCounter; } set { Set(ref dressCounter, value); } }
		public int DressCounterMax { get { return dressCounterMax; } set { Set(ref dressCounterMax, value); } }
		public double PolishTime { get { return polishTime; } set { Set(ref polishTime, value); } }
		public double PolishForce { get { return polishForce; } set { Set(ref polishForce, value); } }
		public ObservableDictionary<int,TIPDRESS> DressVar { get { return dressVar; } set { Set(ref dressVar, value); } }
		public double CalEqPressure_min { get { return calEqPressure_min; } set { Set(ref calEqPressure_min, value); } }
		public double CalEqPressure_max { get { return calEqPressure_max; } set { Set(ref calEqPressure_max, value); } }
		public double CalEqPressure_stat { get { return calEqPressure_stat; } set { Set(ref calEqPressure_stat, value); } }
	#endregion //properties

	#region constructors
		public Gun(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("Num")) num = int.Parse(dataItems["Num"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("MountingWeld")) mountingWeld = (swp_GUN_MOUNTING)System.Enum.Parse(typeof(swp_GUN_MOUNTING), dataItems["MountingWeld"].ToString().TrimStart('#'), true);
			if (dataItems.ContainsKey("WeldTimerContr")) weldTimerContr = int.Parse(dataItems["WeldTimerContr"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("TipDresserContr")) tipDresserContr = int.Parse(dataItems["TipDresserContr"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("TipDrRelayState")) tipDrRelayState = int.Parse(dataItems["TipDrRelayState"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("ElectrodeNo")) electrodeNo = int.Parse(dataItems["ElectrodeNo"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("FastCloseOpt")) fastCloseOpt = (OPTION_CTL)System.Enum.Parse(typeof(OPTION_CTL), dataItems["FastCloseOpt"].ToString().TrimStart('#'), true);
			if (dataItems.ContainsKey("FastLeaveOpt")) fastLeaveOpt = (OPTION_CTL)System.Enum.Parse(typeof(OPTION_CTL), dataItems["FastLeaveOpt"].ToString().TrimStart('#'), true);
			if (dataItems.ContainsKey("EqualBack")) equalBack = bool.Parse(dataItems["EqualBack"].ToString());
			if (dataItems.ContainsKey("EqualBackFl")) equalBackFl = bool.Parse(dataItems["EqualBackFl"].ToString());
			if (dataItems.ContainsKey("manMvSTEP0")) manMvSTEP0 = double.Parse(dataItems["manMvSTEP0"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("manMvSTEP1")) manMvSTEP1 = double.Parse(dataItems["manMvSTEP1"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("manMvSTEP2")) manMvSTEP2 = double.Parse(dataItems["manMvSTEP2"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("LastOpenM")) lastOpenM = double.Parse(dataItems["LastOpenM"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("MinOpenM")) minOpenM = double.Parse(dataItems["MinOpenM"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("MaxOpenM")) maxOpenM = double.Parse(dataItems["MaxOpenM"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("HomeEqualPr")) homeEqualPr = double.Parse(dataItems["HomeEqualPr"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("PolishWOpen")) polishWOpen = bool.Parse(dataItems["PolishWOpen"].ToString());
			if (dataItems.ContainsKey("PolishOption")) polishOption = bool.Parse(dataItems["PolishOption"].ToString());
			if (dataItems.ContainsKey("GunOpenTime")) gunOpenTime = double.Parse(dataItems["GunOpenTime"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("TipDressListOption")) tipDressListOption = bool.Parse(dataItems["TipDressListOption"].ToString());
			if (dataItems.ContainsKey("DressCounter")) dressCounter = int.Parse(dataItems["DressCounter"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("DressCounterMax")) dressCounterMax = int.Parse(dataItems["DressCounterMax"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("PolishTime")) polishTime = double.Parse(dataItems["PolishTime"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("PolishForce")) polishForce = double.Parse(dataItems["PolishForce"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("DressVar")) dressVar = ObservableDictionary<int,TIPDRESS>.Parse(dataItems["DressVar"].ToString());
			if (dataItems.ContainsKey("calEqPressure_min")) calEqPressure_min = double.Parse(dataItems["calEqPressure_min"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("calEqPressure_max")) calEqPressure_max = double.Parse(dataItems["calEqPressure_max"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("calEqPressure_stat")) calEqPressure_stat = double.Parse(dataItems["calEqPressure_stat"].ToString(), CultureInfo.InvariantCulture);
		}

		public Gun(int Num, swp_GUN_MOUNTING MountingWeld, int WeldTimerContr, int TipDresserContr, int TipDrRelayState, int ElectrodeNo, OPTION_CTL FastCloseOpt, OPTION_CTL FastLeaveOpt, bool EqualBack, bool EqualBackFl, double ManMvSTEP0, double ManMvSTEP1, double ManMvSTEP2, double LastOpenM, double MinOpenM, double MaxOpenM, double HomeEqualPr, bool PolishWOpen, bool PolishOption, double GunOpenTime, bool TipDressListOption, int DressCounter, int DressCounterMax, double PolishTime, double PolishForce, ObservableDictionary<int,TIPDRESS> DressVar, double CalEqPressure_min, double CalEqPressure_max, double CalEqPressure_stat, string valName="")
		{
			num = Num;
			mountingWeld = MountingWeld;
			weldTimerContr = WeldTimerContr;
			tipDresserContr = TipDresserContr;
			tipDrRelayState = TipDrRelayState;
			electrodeNo = ElectrodeNo;
			fastCloseOpt = FastCloseOpt;
			fastLeaveOpt = FastLeaveOpt;
			equalBack = EqualBack;
			equalBackFl = EqualBackFl;
			manMvSTEP0 = ManMvSTEP0;
			manMvSTEP1 = ManMvSTEP1;
			manMvSTEP2 = ManMvSTEP2;
			lastOpenM = LastOpenM;
			minOpenM = MinOpenM;
			maxOpenM = MaxOpenM;
			homeEqualPr = HomeEqualPr;
			polishWOpen = PolishWOpen;
			polishOption = PolishOption;
			gunOpenTime = GunOpenTime;
			tipDressListOption = TipDressListOption;
			dressCounter = DressCounter;
			dressCounterMax = DressCounterMax;
			polishTime = PolishTime;
			polishForce = PolishForce;
			dressVar = DressVar;
			calEqPressure_min = CalEqPressure_min;
			calEqPressure_max = CalEqPressure_max;
			calEqPressure_stat = CalEqPressure_stat;
			valName = ValName;
		}

		public Gun(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["Num"] != null) num = (int)mem["Num"];
			if (mem["MountingWeld"] != null) mountingWeld = (swp_GUN_MOUNTING)mem["MountingWeld"];
			if (mem["WeldTimerContr"] != null) weldTimerContr = (int)mem["WeldTimerContr"];
			if (mem["TipDresserContr"] != null) tipDresserContr = (int)mem["TipDresserContr"];
			if (mem["TipDrRelayState"] != null) tipDrRelayState = (int)mem["TipDrRelayState"];
			if (mem["ElectrodeNo"] != null) electrodeNo = (int)mem["ElectrodeNo"];
			if (mem["FastCloseOpt"] != null) fastCloseOpt = (OPTION_CTL)mem["FastCloseOpt"];
			if (mem["FastLeaveOpt"] != null) fastLeaveOpt = (OPTION_CTL)mem["FastLeaveOpt"];
			if (mem["EqualBack"] != null) equalBack = (bool)mem["EqualBack"];
			if (mem["EqualBackFl"] != null) equalBackFl = (bool)mem["EqualBackFl"];
			if (mem["manMvSTEP0"] != null) manMvSTEP0 = (double)mem["manMvSTEP0"];
			if (mem["manMvSTEP1"] != null) manMvSTEP1 = (double)mem["manMvSTEP1"];
			if (mem["manMvSTEP2"] != null) manMvSTEP2 = (double)mem["manMvSTEP2"];
			if (mem["LastOpenM"] != null) lastOpenM = (double)mem["LastOpenM"];
			if (mem["MinOpenM"] != null) minOpenM = (double)mem["MinOpenM"];
			if (mem["MaxOpenM"] != null) maxOpenM = (double)mem["MaxOpenM"];
			if (mem["HomeEqualPr"] != null) homeEqualPr = (double)mem["HomeEqualPr"];
			if (mem["PolishWOpen"] != null) polishWOpen = (bool)mem["PolishWOpen"];
			if (mem["PolishOption"] != null) polishOption = (bool)mem["PolishOption"];
			if (mem["GunOpenTime"] != null) gunOpenTime = (double)mem["GunOpenTime"];
			if (mem["TipDressListOption"] != null) tipDressListOption = (bool)mem["TipDressListOption"];
			if (mem["DressCounter"] != null) dressCounter = (int)mem["DressCounter"];
			if (mem["DressCounterMax"] != null) dressCounterMax = (int)mem["DressCounterMax"];
			if (mem["PolishTime"] != null) polishTime = (double)mem["PolishTime"];
			if (mem["PolishForce"] != null) polishForce = (double)mem["PolishForce"];
			if (mem["DressVar"] != null) dressVar = (ObservableDictionary<int,TIPDRESS>)mem["DressVar"];
			if (mem["calEqPressure_min"] != null) calEqPressure_min = (double)mem["calEqPressure_min"];
			if (mem["calEqPressure_max"] != null) calEqPressure_max = (double)mem["calEqPressure_max"];
			if (mem["calEqPressure_stat"] != null) calEqPressure_stat = (double)mem["calEqPressure_stat"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL Gun " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				num.ToString(CultureInfo.InvariantCulture),
				mountingWeld.ToString(),
				weldTimerContr.ToString(CultureInfo.InvariantCulture),
				tipDresserContr.ToString(CultureInfo.InvariantCulture),
				tipDrRelayState.ToString(CultureInfo.InvariantCulture),
				electrodeNo.ToString(CultureInfo.InvariantCulture),
				fastCloseOpt.ToString(),
				fastLeaveOpt.ToString(),
				equalBack.ToString(CultureInfo.InvariantCulture),
				equalBackFl.ToString(CultureInfo.InvariantCulture),
				manMvSTEP0.ToString(CultureInfo.InvariantCulture),
				manMvSTEP1.ToString(CultureInfo.InvariantCulture),
				manMvSTEP2.ToString(CultureInfo.InvariantCulture),
				lastOpenM.ToString(CultureInfo.InvariantCulture),
				minOpenM.ToString(CultureInfo.InvariantCulture),
				maxOpenM.ToString(CultureInfo.InvariantCulture),
				homeEqualPr.ToString(CultureInfo.InvariantCulture),
				polishWOpen.ToString(CultureInfo.InvariantCulture),
				polishOption.ToString(CultureInfo.InvariantCulture),
				gunOpenTime.ToString(CultureInfo.InvariantCulture),
				tipDressListOption.ToString(CultureInfo.InvariantCulture),
				dressCounter.ToString(CultureInfo.InvariantCulture),
				dressCounterMax.ToString(CultureInfo.InvariantCulture),
				polishTime.ToString(CultureInfo.InvariantCulture),
				polishForce.ToString(CultureInfo.InvariantCulture),
				dressVar.ToString(),
				calEqPressure_min.ToString(CultureInfo.InvariantCulture),
				calEqPressure_max.ToString(CultureInfo.InvariantCulture),
				calEqPressure_stat.ToString(CultureInfo.InvariantCulture),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{Num {0},MountingWeld {1},WeldTimerContr {2},TipDresserContr {3},TipDrRelayState {4},ElectrodeNo {5},FastCloseOpt {6},FastLeaveOpt {7},EqualBack {8},EqualBackFl {9},manMvSTEP0 {10},manMvSTEP1 {11},manMvSTEP2 {12},LastOpenM {13},MinOpenM {14},MaxOpenM {15},HomeEqualPr {16},PolishWOpen {17},PolishOption {18},GunOpenTime {19},TipDressListOption {20},DressCounter {21},DressCounterMax {22},PolishTime {23},PolishForce {24},DressVar {25},calEqPressure_min {26},calEqPressure_max {27},calEqPressure_stat {28}}}",
				num, "#" + mountingWeld.ToString(), weldTimerContr, tipDresserContr, tipDrRelayState, electrodeNo, "#" + fastCloseOpt.ToString(), "#" + fastLeaveOpt.ToString(), BtoStr(equalBack), BtoStr(equalBackFl), manMvSTEP0, manMvSTEP1, manMvSTEP2, lastOpenM, minOpenM, maxOpenM, homeEqualPr, BtoStr(polishWOpen), BtoStr(polishOption), gunOpenTime, BtoStr(tipDressListOption), dressCounter, dressCounterMax, polishTime, polishForce, dressVar, calEqPressure_min, calEqPressure_max, calEqPressure_stat
				);
		}

	#endregion //methods

	}
}
