using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class ODAT : Variable
	{
	#region fields
		private int oUT_NO;
		private bool sTATE;
		private double pULSE_TIME;
		private OUT_MODETYPE oUT_MODE;
		private double tIME_DELAY;
		private double oFFSET;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "ODAT";} }
		public int OUT_NO { get { return oUT_NO; } set { Set(ref oUT_NO, value); } }
		public bool STATE { get { return sTATE; } set { Set(ref sTATE, value); } }
		public double PULSE_TIME { get { return pULSE_TIME; } set { Set(ref pULSE_TIME, value); } }
		public OUT_MODETYPE OUT_MODE { get { return oUT_MODE; } set { Set(ref oUT_MODE, value); } }
		public double TIME_DELAY { get { return tIME_DELAY; } set { Set(ref tIME_DELAY, value); } }
		public double OFFSET { get { return oFFSET; } set { Set(ref oFFSET, value); } }
	#endregion //properties

	#region constructors
		public ODAT(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("OUT_NO")) oUT_NO = int.Parse(dataItems["OUT_NO"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("STATE")) sTATE = bool.Parse(dataItems["STATE"].ToString());
			if (dataItems.ContainsKey("PULSE_TIME")) pULSE_TIME = double.Parse(dataItems["PULSE_TIME"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("OUT_MODE")) oUT_MODE = (OUT_MODETYPE)System.Enum.Parse(typeof(OUT_MODETYPE), dataItems["OUT_MODE"].ToString().TrimStart('#'), true);
			if (dataItems.ContainsKey("TIME_DELAY")) tIME_DELAY = double.Parse(dataItems["TIME_DELAY"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("OFFSET")) oFFSET = double.Parse(dataItems["OFFSET"].ToString(), CultureInfo.InvariantCulture);
		}

		public ODAT(int OUT_NO, bool STATE, double PULSE_TIME, OUT_MODETYPE OUT_MODE, double TIME_DELAY, double OFFSET, string valName="")
		{
			oUT_NO = OUT_NO;
			sTATE = STATE;
			pULSE_TIME = PULSE_TIME;
			oUT_MODE = OUT_MODE;
			tIME_DELAY = TIME_DELAY;
			oFFSET = OFFSET;
			valName = ValName;
		}

		public ODAT(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["OUT_NO"] != null) oUT_NO = (int)mem["OUT_NO"];
			if (mem["STATE"] != null) sTATE = (bool)mem["STATE"];
			if (mem["PULSE_TIME"] != null) pULSE_TIME = (double)mem["PULSE_TIME"];
			if (mem["OUT_MODE"] != null) oUT_MODE = (OUT_MODETYPE)mem["OUT_MODE"];
			if (mem["TIME_DELAY"] != null) tIME_DELAY = (double)mem["TIME_DELAY"];
			if (mem["OFFSET"] != null) oFFSET = (double)mem["OFFSET"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL ODAT " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				oUT_NO.ToString(CultureInfo.InvariantCulture),
				sTATE.ToString(CultureInfo.InvariantCulture),
				pULSE_TIME.ToString(CultureInfo.InvariantCulture),
				oUT_MODE.ToString(),
				tIME_DELAY.ToString(CultureInfo.InvariantCulture),
				oFFSET.ToString(CultureInfo.InvariantCulture),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{OUT_NO {0},STATE {1},PULSE_TIME {2},OUT_MODE {3},TIME_DELAY {4},OFFSET {5}}}",
				oUT_NO, BtoStr(sTATE), pULSE_TIME, "#" + oUT_MODE.ToString(), tIME_DELAY, oFFSET
				);
		}

	#endregion //methods

	}
}
