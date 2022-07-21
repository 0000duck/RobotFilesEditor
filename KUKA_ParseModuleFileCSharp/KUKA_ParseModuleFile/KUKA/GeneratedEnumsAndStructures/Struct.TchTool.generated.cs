using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class TchTool : Variable
	{
	#region fields
		private int num;
		private int tch_I_TOOL_DATA;
		private int tch_i_After_Lock_Time;
		private ObservableDictionary<int,TchTool_OptStation> tch_b_option_station;
		private int tch_i_PN_Dev_count;
		private ObservableDictionary<int,TchTool_PNDevNo> tch_i_PN_DevNo;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "TchTool";} }
		public int Num { get { return num; } set { Set(ref num, value); } }
		public int Tch_I_TOOL_DATA { get { return tch_I_TOOL_DATA; } set { Set(ref tch_I_TOOL_DATA, value); } }
		public int Tch_i_After_Lock_Time { get { return tch_i_After_Lock_Time; } set { Set(ref tch_i_After_Lock_Time, value); } }
		public ObservableDictionary<int,TchTool_OptStation> Tch_b_option_station { get { return tch_b_option_station; } set { Set(ref tch_b_option_station, value); } }
		public int Tch_i_PN_Dev_count { get { return tch_i_PN_Dev_count; } set { Set(ref tch_i_PN_Dev_count, value); } }
		public ObservableDictionary<int,TchTool_PNDevNo> Tch_i_PN_DevNo { get { return tch_i_PN_DevNo; } set { Set(ref tch_i_PN_DevNo, value); } }
	#endregion //properties

	#region constructors
		public TchTool(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("Num")) num = int.Parse(dataItems["Num"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("tch_I_TOOL_DATA")) tch_I_TOOL_DATA = int.Parse(dataItems["tch_I_TOOL_DATA"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("tch_i_After_Lock_Time")) tch_i_After_Lock_Time = int.Parse(dataItems["tch_i_After_Lock_Time"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("tch_b_option_station")) tch_b_option_station = ObservableDictionary<int,TchTool_OptStation>.Parse(dataItems["tch_b_option_station"].ToString());
			if (dataItems.ContainsKey("tch_i_PN_Dev_count")) tch_i_PN_Dev_count = int.Parse(dataItems["tch_i_PN_Dev_count"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("tch_i_PN_DevNo")) tch_i_PN_DevNo = ObservableDictionary<int,TchTool_PNDevNo>.Parse(dataItems["tch_i_PN_DevNo"].ToString());
		}

		public TchTool(int Num, int Tch_I_TOOL_DATA, int Tch_i_After_Lock_Time, ObservableDictionary<int,TchTool_OptStation> Tch_b_option_station, int Tch_i_PN_Dev_count, ObservableDictionary<int,TchTool_PNDevNo> Tch_i_PN_DevNo, string valName="")
		{
			num = Num;
			tch_I_TOOL_DATA = Tch_I_TOOL_DATA;
			tch_i_After_Lock_Time = Tch_i_After_Lock_Time;
			tch_b_option_station = Tch_b_option_station;
			tch_i_PN_Dev_count = Tch_i_PN_Dev_count;
			tch_i_PN_DevNo = Tch_i_PN_DevNo;
			valName = ValName;
		}

		public TchTool(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["Num"] != null) num = (int)mem["Num"];
			if (mem["tch_I_TOOL_DATA"] != null) tch_I_TOOL_DATA = (int)mem["tch_I_TOOL_DATA"];
			if (mem["tch_i_After_Lock_Time"] != null) tch_i_After_Lock_Time = (int)mem["tch_i_After_Lock_Time"];
			if (mem["tch_b_option_station"] != null) tch_b_option_station = (ObservableDictionary<int,TchTool_OptStation>)mem["tch_b_option_station"];
			if (mem["tch_i_PN_Dev_count"] != null) tch_i_PN_Dev_count = (int)mem["tch_i_PN_Dev_count"];
			if (mem["tch_i_PN_DevNo"] != null) tch_i_PN_DevNo = (ObservableDictionary<int,TchTool_PNDevNo>)mem["tch_i_PN_DevNo"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL TchTool " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				num.ToString(CultureInfo.InvariantCulture),
				tch_I_TOOL_DATA.ToString(CultureInfo.InvariantCulture),
				tch_i_After_Lock_Time.ToString(CultureInfo.InvariantCulture),
				tch_b_option_station.ToString(),
				tch_i_PN_Dev_count.ToString(CultureInfo.InvariantCulture),
				tch_i_PN_DevNo.ToString(),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{Num {0},tch_I_TOOL_DATA {1},tch_i_After_Lock_Time {2},tch_b_option_station {3},tch_i_PN_Dev_count {4},tch_i_PN_DevNo {5}}}",
				num, tch_I_TOOL_DATA, tch_i_After_Lock_Time, tch_b_option_station, tch_i_PN_Dev_count, tch_i_PN_DevNo
				);
		}

	#endregion //methods

	}
}
