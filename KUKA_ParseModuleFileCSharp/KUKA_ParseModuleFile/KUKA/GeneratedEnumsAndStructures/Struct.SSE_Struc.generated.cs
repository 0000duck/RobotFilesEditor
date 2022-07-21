using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class SSE_Struc : Variable
	{
	#region fields
		private int grp;
		private string name;
		private int startOffSet;
		private int lastOffSet;
		private int counter;
		private int searchType;
		private int trigType;
		private bool isFirstSrc;
		private int in1;
		private int in2;
		private int in3;
		private int in4;
		private bool inMode;
		private int inFast1;
		private int inFast2;
		private ORI_TYPE oriType;
		private double vel;
		private double vel_Slow;
		private double vel_Fast;
		private int waitTime;
		private bool isConfig;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "SSE_Struc";} }
		public int Grp { get { return grp; } set { Set(ref grp, value); } }
		public string Name { get { return name; } set { Set(ref name, value); } }
		public int StartOffSet { get { return startOffSet; } set { Set(ref startOffSet, value); } }
		public int LastOffSet { get { return lastOffSet; } set { Set(ref lastOffSet, value); } }
		public int Counter { get { return counter; } set { Set(ref counter, value); } }
		public int SearchType { get { return searchType; } set { Set(ref searchType, value); } }
		public int TrigType { get { return trigType; } set { Set(ref trigType, value); } }
		public bool IsFirstSrc { get { return isFirstSrc; } set { Set(ref isFirstSrc, value); } }
		public int In1 { get { return in1; } set { Set(ref in1, value); } }
		public int In2 { get { return in2; } set { Set(ref in2, value); } }
		public int In3 { get { return in3; } set { Set(ref in3, value); } }
		public int In4 { get { return in4; } set { Set(ref in4, value); } }
		public bool InMode { get { return inMode; } set { Set(ref inMode, value); } }
		public int InFast1 { get { return inFast1; } set { Set(ref inFast1, value); } }
		public int InFast2 { get { return inFast2; } set { Set(ref inFast2, value); } }
		public ORI_TYPE OriType { get { return oriType; } set { Set(ref oriType, value); } }
		public double Vel { get { return vel; } set { Set(ref vel, value); } }
		public double Vel_Slow { get { return vel_Slow; } set { Set(ref vel_Slow, value); } }
		public double Vel_Fast { get { return vel_Fast; } set { Set(ref vel_Fast, value); } }
		public int WaitTime { get { return waitTime; } set { Set(ref waitTime, value); } }
		public bool IsConfig { get { return isConfig; } set { Set(ref isConfig, value); } }
	#endregion //properties

	#region constructors
		public SSE_Struc(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("Grp")) grp = int.Parse(dataItems["Grp"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("Name")) name = dataItems["Name"].ToString().Trim('"');
			if (dataItems.ContainsKey("StartOffSet")) startOffSet = int.Parse(dataItems["StartOffSet"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("LastOffSet")) lastOffSet = int.Parse(dataItems["LastOffSet"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("Counter")) counter = int.Parse(dataItems["Counter"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("SearchType")) searchType = int.Parse(dataItems["SearchType"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("TrigType")) trigType = int.Parse(dataItems["TrigType"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("IsFirstSrc")) isFirstSrc = bool.Parse(dataItems["IsFirstSrc"].ToString());
			if (dataItems.ContainsKey("In1")) in1 = int.Parse(dataItems["In1"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("In2")) in2 = int.Parse(dataItems["In2"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("In3")) in3 = int.Parse(dataItems["In3"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("In4")) in4 = int.Parse(dataItems["In4"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("InMode")) inMode = bool.Parse(dataItems["InMode"].ToString());
			if (dataItems.ContainsKey("InFast1")) inFast1 = int.Parse(dataItems["InFast1"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("InFast2")) inFast2 = int.Parse(dataItems["InFast2"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("OriType")) oriType = (ORI_TYPE)System.Enum.Parse(typeof(ORI_TYPE), dataItems["OriType"].ToString().TrimStart('#'), true);
			if (dataItems.ContainsKey("Vel")) vel = double.Parse(dataItems["Vel"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("Vel_Slow")) vel_Slow = double.Parse(dataItems["Vel_Slow"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("Vel_Fast")) vel_Fast = double.Parse(dataItems["Vel_Fast"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("WaitTime")) waitTime = int.Parse(dataItems["WaitTime"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("IsConfig")) isConfig = bool.Parse(dataItems["IsConfig"].ToString());
		}

		public SSE_Struc(int Grp, string Name, int StartOffSet, int LastOffSet, int Counter, int SearchType, int TrigType, bool IsFirstSrc, int In1, int In2, int In3, int In4, bool InMode, int InFast1, int InFast2, ORI_TYPE OriType, double Vel, double Vel_Slow, double Vel_Fast, int WaitTime, bool IsConfig, string valName="")
		{
			grp = Grp;
			name = Name;
			startOffSet = StartOffSet;
			lastOffSet = LastOffSet;
			counter = Counter;
			searchType = SearchType;
			trigType = TrigType;
			isFirstSrc = IsFirstSrc;
			in1 = In1;
			in2 = In2;
			in3 = In3;
			in4 = In4;
			inMode = InMode;
			inFast1 = InFast1;
			inFast2 = InFast2;
			oriType = OriType;
			vel = Vel;
			vel_Slow = Vel_Slow;
			vel_Fast = Vel_Fast;
			waitTime = WaitTime;
			isConfig = IsConfig;
			valName = ValName;
		}

		public SSE_Struc(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["Grp"] != null) grp = (int)mem["Grp"];
			if (mem["Name"] != null) name = (string)mem["Name"];
			if (mem["StartOffSet"] != null) startOffSet = (int)mem["StartOffSet"];
			if (mem["LastOffSet"] != null) lastOffSet = (int)mem["LastOffSet"];
			if (mem["Counter"] != null) counter = (int)mem["Counter"];
			if (mem["SearchType"] != null) searchType = (int)mem["SearchType"];
			if (mem["TrigType"] != null) trigType = (int)mem["TrigType"];
			if (mem["IsFirstSrc"] != null) isFirstSrc = (bool)mem["IsFirstSrc"];
			if (mem["In1"] != null) in1 = (int)mem["In1"];
			if (mem["In2"] != null) in2 = (int)mem["In2"];
			if (mem["In3"] != null) in3 = (int)mem["In3"];
			if (mem["In4"] != null) in4 = (int)mem["In4"];
			if (mem["InMode"] != null) inMode = (bool)mem["InMode"];
			if (mem["InFast1"] != null) inFast1 = (int)mem["InFast1"];
			if (mem["InFast2"] != null) inFast2 = (int)mem["InFast2"];
			if (mem["OriType"] != null) oriType = (ORI_TYPE)mem["OriType"];
			if (mem["Vel"] != null) vel = (double)mem["Vel"];
			if (mem["Vel_Slow"] != null) vel_Slow = (double)mem["Vel_Slow"];
			if (mem["Vel_Fast"] != null) vel_Fast = (double)mem["Vel_Fast"];
			if (mem["WaitTime"] != null) waitTime = (int)mem["WaitTime"];
			if (mem["IsConfig"] != null) isConfig = (bool)mem["IsConfig"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL SSE_Struc " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				grp.ToString(CultureInfo.InvariantCulture),
				name,
				startOffSet.ToString(CultureInfo.InvariantCulture),
				lastOffSet.ToString(CultureInfo.InvariantCulture),
				counter.ToString(CultureInfo.InvariantCulture),
				searchType.ToString(CultureInfo.InvariantCulture),
				trigType.ToString(CultureInfo.InvariantCulture),
				isFirstSrc.ToString(CultureInfo.InvariantCulture),
				in1.ToString(CultureInfo.InvariantCulture),
				in2.ToString(CultureInfo.InvariantCulture),
				in3.ToString(CultureInfo.InvariantCulture),
				in4.ToString(CultureInfo.InvariantCulture),
				inMode.ToString(CultureInfo.InvariantCulture),
				inFast1.ToString(CultureInfo.InvariantCulture),
				inFast2.ToString(CultureInfo.InvariantCulture),
				oriType.ToString(),
				vel.ToString(CultureInfo.InvariantCulture),
				vel_Slow.ToString(CultureInfo.InvariantCulture),
				vel_Fast.ToString(CultureInfo.InvariantCulture),
				waitTime.ToString(CultureInfo.InvariantCulture),
				isConfig.ToString(CultureInfo.InvariantCulture),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{Grp {0},Name[] \"{1}\",StartOffSet {2},LastOffSet {3},Counter {4},SearchType {5},TrigType {6},IsFirstSrc {7},In1 {8},In2 {9},In3 {10},In4 {11},InMode {12},InFast1 {13},InFast2 {14},OriType {15},Vel {16},Vel_Slow {17},Vel_Fast {18},WaitTime {19},IsConfig {20}}}",
				grp, name, startOffSet, lastOffSet, counter, searchType, trigType, BtoStr(isFirstSrc), in1, in2, in3, in4, BtoStr(inMode), inFast1, inFast2, "#" + oriType.ToString(), vel, vel_Slow, vel_Fast, waitTime, BtoStr(isConfig)
				);
		}

	#endregion //methods

	}
}
