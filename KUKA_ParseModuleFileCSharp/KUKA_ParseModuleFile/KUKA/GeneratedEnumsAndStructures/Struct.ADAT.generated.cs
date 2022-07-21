using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class ADAT : Variable
	{
	#region fields
		private TRIGGER_PARA triggerPara;
		private CONSTVEL_PARA constVelPara;
		private CONDSTOP_PARA condStopPara;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "ADAT";} }
		public TRIGGER_PARA TriggerPara { get { return triggerPara; } set { Set(ref triggerPara, value); } }
		public CONSTVEL_PARA ConstVelPara { get { return constVelPara; } set { Set(ref constVelPara, value); } }
		public CONDSTOP_PARA CondStopPara { get { return condStopPara; } set { Set(ref condStopPara, value); } }
	#endregion //properties

	#region constructors
		public ADAT(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("TriggerPara")) triggerPara = new TRIGGER_PARA(dataItems["TriggerPara"]);
			if (dataItems.ContainsKey("ConstVelPara")) constVelPara = new CONSTVEL_PARA(dataItems["ConstVelPara"]);
			if (dataItems.ContainsKey("CondStopPara")) condStopPara = new CONDSTOP_PARA(dataItems["CondStopPara"]);
		}

		public ADAT(TRIGGER_PARA TriggerPara, CONSTVEL_PARA ConstVelPara, CONDSTOP_PARA CondStopPara, string valName="")
		{
			triggerPara = TriggerPara;
			constVelPara = ConstVelPara;
			condStopPara = CondStopPara;
			valName = ValName;
		}

		public ADAT(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			triggerPara = new TRIGGER_PARA((DynamicMemory)mem["TriggerPara"]);
			constVelPara = new CONSTVEL_PARA((DynamicMemory)mem["ConstVelPara"]);
			condStopPara = new CONDSTOP_PARA((DynamicMemory)mem["CondStopPara"]);
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL ADAT " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				triggerPara.ToString(),
				constVelPara.ToString(),
				condStopPara.ToString(),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{TriggerPara {0},ConstVelPara {1},CondStopPara {2}}}",
				triggerPara, constVelPara, condStopPara
				);
		}

	#endregion //methods

	}
}
