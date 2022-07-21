using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class WS_CONFIG : Variable
	{
	#region fields
		private int wS_CONFIG;
		private string wS_NAME;
		private int wS_PRIO;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "WS_CONFIG";} }
		public int WS_CONFIG_ { get { return wS_CONFIG; } set { Set(ref wS_CONFIG, value); } }
		public string WS_NAME { get { return wS_NAME; } set { Set(ref wS_NAME, value); } }
		public int WS_PRIO { get { return wS_PRIO; } set { Set(ref wS_PRIO, value); } }
	#endregion //properties

	#region constructors
		public WS_CONFIG(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("WS_CONFIG")) wS_CONFIG = int.Parse(dataItems["WS_CONFIG"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("WS_NAME")) wS_NAME = dataItems["WS_NAME"].ToString().Trim('"');
			if (dataItems.ContainsKey("WS_PRIO")) wS_PRIO = int.Parse(dataItems["WS_PRIO"].ToString(), CultureInfo.InvariantCulture);
		}

		public WS_CONFIG(int WS_CONFIG, string WS_NAME, int WS_PRIO, string valName="")
		{
			wS_CONFIG = WS_CONFIG;
			wS_NAME = WS_NAME;
			wS_PRIO = WS_PRIO;
			valName = ValName;
		}

		public WS_CONFIG(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["WS_CONFIG"] != null) wS_CONFIG = (int)mem["WS_CONFIG"];
			if (mem["WS_NAME"] != null) wS_NAME = (string)mem["WS_NAME"];
			if (mem["WS_PRIO"] != null) wS_PRIO = (int)mem["WS_PRIO"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL WS_CONFIG " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				wS_CONFIG.ToString(CultureInfo.InvariantCulture),
				wS_NAME,
				wS_PRIO.ToString(CultureInfo.InvariantCulture),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{WS_CONFIG {0},WS_NAME[] \"{1}\",WS_PRIO {2}}}",
				wS_CONFIG, wS_NAME, wS_PRIO
				);
		}

	#endregion //methods

	}
}
