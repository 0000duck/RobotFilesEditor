using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class FDAT : Variable
	{
	#region fields
		private int tOOL_NO;
		private int bASE_NO;
		private IPO_MODE iPO_FRAME;
		private string pOINT2;
		private bool tQ_STATE;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "FDAT";} }
		public int TOOL_NO { get { return tOOL_NO; } set { Set(ref tOOL_NO, value); } }
		public int BASE_NO { get { return bASE_NO; } set { Set(ref bASE_NO, value); } }
		public IPO_MODE IPO_FRAME { get { return iPO_FRAME; } set { Set(ref iPO_FRAME, value); } }
		public string POINT2 { get { return pOINT2; } set { Set(ref pOINT2, value); } }
		public bool TQ_STATE { get { return tQ_STATE; } set { Set(ref tQ_STATE, value); } }
	#endregion //properties

	#region constructors
		public FDAT(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("TOOL_NO")) tOOL_NO = int.Parse(dataItems["TOOL_NO"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("BASE_NO")) bASE_NO = int.Parse(dataItems["BASE_NO"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("IPO_FRAME")) iPO_FRAME = (IPO_MODE)System.Enum.Parse(typeof(IPO_MODE), dataItems["IPO_FRAME"].ToString().TrimStart('#'), true);
			if (dataItems.ContainsKey("POINT2")) pOINT2 = dataItems["POINT2"].ToString().Trim('"');
			if (dataItems.ContainsKey("TQ_STATE")) tQ_STATE = bool.Parse(dataItems["TQ_STATE"].ToString());
		}

		public FDAT(int TOOL_NO, int BASE_NO, IPO_MODE IPO_FRAME, string POINT2, bool TQ_STATE, string valName="")
		{
			tOOL_NO = TOOL_NO;
			bASE_NO = BASE_NO;
			iPO_FRAME = IPO_FRAME;
			pOINT2 = POINT2;
			tQ_STATE = TQ_STATE;
			valName = ValName;
		}

		public FDAT(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["TOOL_NO"] != null) tOOL_NO = (int)mem["TOOL_NO"];
			if (mem["BASE_NO"] != null) bASE_NO = (int)mem["BASE_NO"];
			if (mem["IPO_FRAME"] != null) iPO_FRAME = (IPO_MODE)mem["IPO_FRAME"];
			if (mem["POINT2"] != null) pOINT2 = (string)mem["POINT2"];
			if (mem["TQ_STATE"] != null) tQ_STATE = (bool)mem["TQ_STATE"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL FDAT " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				tOOL_NO.ToString(CultureInfo.InvariantCulture),
				bASE_NO.ToString(CultureInfo.InvariantCulture),
				iPO_FRAME.ToString(),
				pOINT2,
				tQ_STATE.ToString(CultureInfo.InvariantCulture),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{TOOL_NO {0},BASE_NO {1},IPO_FRAME {2},POINT2[] \"{3}\",TQ_STATE {4}}}",
				tOOL_NO, bASE_NO, "#" + iPO_FRAME.ToString(), pOINT2, BtoStr(tQ_STATE)
				);
		}

	#endregion //methods

	}
}
