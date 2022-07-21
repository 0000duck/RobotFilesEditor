using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class TRIGGER_PARA : Variable
	{
	#region fields
		private int triggerPath;
		private bool triggerOnStart;
		private int triggerDelay;
		private string triggerTask;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "TRIGGER_PARA";} }
		public int TriggerPath { get { return triggerPath; } set { Set(ref triggerPath, value); } }
		public bool TriggerOnStart { get { return triggerOnStart; } set { Set(ref triggerOnStart, value); } }
		public int TriggerDelay { get { return triggerDelay; } set { Set(ref triggerDelay, value); } }
		public string TriggerTask { get { return triggerTask; } set { Set(ref triggerTask, value); } }
	#endregion //properties

	#region constructors
		public TRIGGER_PARA(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("TriggerPath")) triggerPath = int.Parse(dataItems["TriggerPath"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("TriggerOnStart")) triggerOnStart = bool.Parse(dataItems["TriggerOnStart"].ToString());
			if (dataItems.ContainsKey("TriggerDelay")) triggerDelay = int.Parse(dataItems["TriggerDelay"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("TriggerTask")) triggerTask = dataItems["TriggerTask"].ToString().Trim('"');
		}

		public TRIGGER_PARA(int TriggerPath, bool TriggerOnStart, int TriggerDelay, string TriggerTask, string valName="")
		{
			triggerPath = TriggerPath;
			triggerOnStart = TriggerOnStart;
			triggerDelay = TriggerDelay;
			triggerTask = TriggerTask;
			valName = ValName;
		}

		public TRIGGER_PARA(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["TriggerPath"] != null) triggerPath = (int)mem["TriggerPath"];
			if (mem["TriggerOnStart"] != null) triggerOnStart = (bool)mem["TriggerOnStart"];
			if (mem["TriggerDelay"] != null) triggerDelay = (int)mem["TriggerDelay"];
			if (mem["TriggerTask"] != null) triggerTask = (string)mem["TriggerTask"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL TRIGGER_PARA " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				triggerPath.ToString(CultureInfo.InvariantCulture),
				triggerOnStart.ToString(CultureInfo.InvariantCulture),
				triggerDelay.ToString(CultureInfo.InvariantCulture),
				triggerTask,
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{TriggerPath {0},TriggerOnStart {1},TriggerDelay {2},TriggerTask[] \"{3}\"}}",
				triggerPath, BtoStr(triggerOnStart), triggerDelay, triggerTask
				);
		}

	#endregion //methods

	}
}
