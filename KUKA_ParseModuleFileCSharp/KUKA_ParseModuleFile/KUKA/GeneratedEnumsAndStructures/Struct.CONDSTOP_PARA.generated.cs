using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class CONDSTOP_PARA : Variable
	{
	#region fields
		private int condStopPath;
		private bool condStopOnStart;
		private string stopCondition;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "CONDSTOP_PARA";} }
		public int CondStopPath { get { return condStopPath; } set { Set(ref condStopPath, value); } }
		public bool CondStopOnStart { get { return condStopOnStart; } set { Set(ref condStopOnStart, value); } }
		public string StopCondition { get { return stopCondition; } set { Set(ref stopCondition, value); } }
	#endregion //properties

	#region constructors
		public CONDSTOP_PARA(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("CondStopPath")) condStopPath = int.Parse(dataItems["CondStopPath"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("CondStopOnStart")) condStopOnStart = bool.Parse(dataItems["CondStopOnStart"].ToString());
			if (dataItems.ContainsKey("StopCondition")) stopCondition = dataItems["StopCondition"].ToString().Trim('"');
		}

		public CONDSTOP_PARA(int CondStopPath, bool CondStopOnStart, string StopCondition, string valName="")
		{
			condStopPath = CondStopPath;
			condStopOnStart = CondStopOnStart;
			stopCondition = StopCondition;
			valName = ValName;
		}

		public CONDSTOP_PARA(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["CondStopPath"] != null) condStopPath = (int)mem["CondStopPath"];
			if (mem["CondStopOnStart"] != null) condStopOnStart = (bool)mem["CondStopOnStart"];
			if (mem["StopCondition"] != null) stopCondition = (string)mem["StopCondition"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL CONDSTOP_PARA " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				condStopPath.ToString(CultureInfo.InvariantCulture),
				condStopOnStart.ToString(CultureInfo.InvariantCulture),
				stopCondition,
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{CondStopPath {0},CondStopOnStart {1},StopCondition[] \"{2}\"}}",
				condStopPath, BtoStr(condStopOnStart), stopCondition
				);
		}

	#endregion //methods

	}
}
