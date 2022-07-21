using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class monitoring : Variable
	{
	#region fields
		private bool condition;
		private bool active;
		private bool started;
		private bool stoped;
		private bool onbypass;
		private bool timeExpired;
		private int monTime;
		private int startTime;
		private int stopTime;
		private int cycleTime;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "monitoring";} }
		public bool Condition { get { return condition; } set { Set(ref condition, value); } }
		public bool Active { get { return active; } set { Set(ref active, value); } }
		public bool Started { get { return started; } set { Set(ref started, value); } }
		public bool Stoped { get { return stoped; } set { Set(ref stoped, value); } }
		public bool Onbypass { get { return onbypass; } set { Set(ref onbypass, value); } }
		public bool TimeExpired { get { return timeExpired; } set { Set(ref timeExpired, value); } }
		public int MonTime { get { return monTime; } set { Set(ref monTime, value); } }
		public int StartTime { get { return startTime; } set { Set(ref startTime, value); } }
		public int StopTime { get { return stopTime; } set { Set(ref stopTime, value); } }
		public int CycleTime { get { return cycleTime; } set { Set(ref cycleTime, value); } }
	#endregion //properties

	#region constructors
		public monitoring(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("condition")) condition = bool.Parse(dataItems["condition"].ToString());
			if (dataItems.ContainsKey("active")) active = bool.Parse(dataItems["active"].ToString());
			if (dataItems.ContainsKey("started")) started = bool.Parse(dataItems["started"].ToString());
			if (dataItems.ContainsKey("stoped")) stoped = bool.Parse(dataItems["stoped"].ToString());
			if (dataItems.ContainsKey("onbypass")) onbypass = bool.Parse(dataItems["onbypass"].ToString());
			if (dataItems.ContainsKey("timeExpired")) timeExpired = bool.Parse(dataItems["timeExpired"].ToString());
			if (dataItems.ContainsKey("monTime")) monTime = int.Parse(dataItems["monTime"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("startTime")) startTime = int.Parse(dataItems["startTime"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("stopTime")) stopTime = int.Parse(dataItems["stopTime"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("cycleTime")) cycleTime = int.Parse(dataItems["cycleTime"].ToString(), CultureInfo.InvariantCulture);
		}

		public monitoring(bool Condition, bool Active, bool Started, bool Stoped, bool Onbypass, bool TimeExpired, int MonTime, int StartTime, int StopTime, int CycleTime, string valName="")
		{
			condition = Condition;
			active = Active;
			started = Started;
			stoped = Stoped;
			onbypass = Onbypass;
			timeExpired = TimeExpired;
			monTime = MonTime;
			startTime = StartTime;
			stopTime = StopTime;
			cycleTime = CycleTime;
			valName = ValName;
		}

		public monitoring(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["condition"] != null) condition = (bool)mem["condition"];
			if (mem["active"] != null) active = (bool)mem["active"];
			if (mem["started"] != null) started = (bool)mem["started"];
			if (mem["stoped"] != null) stoped = (bool)mem["stoped"];
			if (mem["onbypass"] != null) onbypass = (bool)mem["onbypass"];
			if (mem["timeExpired"] != null) timeExpired = (bool)mem["timeExpired"];
			if (mem["monTime"] != null) monTime = (int)mem["monTime"];
			if (mem["startTime"] != null) startTime = (int)mem["startTime"];
			if (mem["stopTime"] != null) stopTime = (int)mem["stopTime"];
			if (mem["cycleTime"] != null) cycleTime = (int)mem["cycleTime"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL monitoring " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				condition.ToString(CultureInfo.InvariantCulture),
				active.ToString(CultureInfo.InvariantCulture),
				started.ToString(CultureInfo.InvariantCulture),
				stoped.ToString(CultureInfo.InvariantCulture),
				onbypass.ToString(CultureInfo.InvariantCulture),
				timeExpired.ToString(CultureInfo.InvariantCulture),
				monTime.ToString(CultureInfo.InvariantCulture),
				startTime.ToString(CultureInfo.InvariantCulture),
				stopTime.ToString(CultureInfo.InvariantCulture),
				cycleTime.ToString(CultureInfo.InvariantCulture),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{condition {0},active {1},started {2},stoped {3},onbypass {4},timeExpired {5},monTime {6},startTime {7},stopTime {8},cycleTime {9}}}",
				BtoStr(condition), BtoStr(active), BtoStr(started), BtoStr(stoped), BtoStr(onbypass), BtoStr(timeExpired), monTime, startTime, stopTime, cycleTime
				);
		}

	#endregion //methods

	}
}
