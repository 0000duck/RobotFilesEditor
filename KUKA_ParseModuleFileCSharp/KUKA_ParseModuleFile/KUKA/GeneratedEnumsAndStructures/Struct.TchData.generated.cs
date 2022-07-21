using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class TchData : Variable
	{
	#region fields
		private int tch_I_MaxStation;
		private int tch_I_tchToolMax;
		private int tch_I_MaxExtStation;
		private int tch_i_tchToolNo;
		private bool tch_b_Safe_Stations;
		private ObservableDictionary<int,object> stations;
		private ObservableDictionary<int,object> tools;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "TchData";} }
		public int Tch_I_MaxStation { get { return tch_I_MaxStation; } set { Set(ref tch_I_MaxStation, value); } }
		public int Tch_I_tchToolMax { get { return tch_I_tchToolMax; } set { Set(ref tch_I_tchToolMax, value); } }
		public int Tch_I_MaxExtStation { get { return tch_I_MaxExtStation; } set { Set(ref tch_I_MaxExtStation, value); } }
		public int Tch_i_tchToolNo { get { return tch_i_tchToolNo; } set { Set(ref tch_i_tchToolNo, value); } }
		public bool Tch_b_Safe_Stations { get { return tch_b_Safe_Stations; } set { Set(ref tch_b_Safe_Stations, value); } }
		public ObservableDictionary<int,object> Stations { get { return stations; } set { Set(ref stations, value); } }
		public ObservableDictionary<int,object> Tools { get { return tools; } set { Set(ref tools, value); } }
	#endregion //properties

	#region constructors
		public TchData(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("Tch_I_MaxStation")) tch_I_MaxStation = int.Parse(dataItems["Tch_I_MaxStation"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("Tch_I_tchToolMax")) tch_I_tchToolMax = int.Parse(dataItems["Tch_I_tchToolMax"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("Tch_I_MaxExtStation")) tch_I_MaxExtStation = int.Parse(dataItems["Tch_I_MaxExtStation"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("Tch_i_tchToolNo")) tch_i_tchToolNo = int.Parse(dataItems["Tch_i_tchToolNo"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("Tch_b_Safe_Stations")) tch_b_Safe_Stations = bool.Parse(dataItems["Tch_b_Safe_Stations"].ToString());
			if (dataItems.ContainsKey("Stations")) stations = ObservableDictionary<int,object>.Parse(dataItems["Stations"].ToString());
			if (dataItems.ContainsKey("Tools")) tools = ObservableDictionary<int,object>.Parse(dataItems["Tools"].ToString());
		}

		public TchData(int Tch_I_MaxStation, int Tch_I_tchToolMax, int Tch_I_MaxExtStation, int Tch_i_tchToolNo, bool Tch_b_Safe_Stations, ObservableDictionary<int,object> Stations, ObservableDictionary<int,object> Tools, string valName="")
		{
			tch_I_MaxStation = Tch_I_MaxStation;
			tch_I_tchToolMax = Tch_I_tchToolMax;
			tch_I_MaxExtStation = Tch_I_MaxExtStation;
			tch_i_tchToolNo = Tch_i_tchToolNo;
			tch_b_Safe_Stations = Tch_b_Safe_Stations;
			stations = Stations;
			tools = Tools;
			valName = ValName;
		}

		public TchData(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["Tch_I_MaxStation"] != null) tch_I_MaxStation = (int)mem["Tch_I_MaxStation"];
			if (mem["Tch_I_tchToolMax"] != null) tch_I_tchToolMax = (int)mem["Tch_I_tchToolMax"];
			if (mem["Tch_I_MaxExtStation"] != null) tch_I_MaxExtStation = (int)mem["Tch_I_MaxExtStation"];
			if (mem["Tch_i_tchToolNo"] != null) tch_i_tchToolNo = (int)mem["Tch_i_tchToolNo"];
			if (mem["Tch_b_Safe_Stations"] != null) tch_b_Safe_Stations = (bool)mem["Tch_b_Safe_Stations"];
			if (mem["Stations"] != null) stations = (ObservableDictionary<int,object>)mem["Stations"];
			if (mem["Tools"] != null) tools = (ObservableDictionary<int,object>)mem["Tools"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL TchData " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				tch_I_MaxStation.ToString(CultureInfo.InvariantCulture),
				tch_I_tchToolMax.ToString(CultureInfo.InvariantCulture),
				tch_I_MaxExtStation.ToString(CultureInfo.InvariantCulture),
				tch_i_tchToolNo.ToString(CultureInfo.InvariantCulture),
				tch_b_Safe_Stations.ToString(CultureInfo.InvariantCulture),
				stations.ToString(),
				tools.ToString(),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{Tch_I_MaxStation {0},Tch_I_tchToolMax {1},Tch_I_MaxExtStation {2},Tch_i_tchToolNo {3},Tch_b_Safe_Stations {4},Stations {5},Tools {6}}}",
				tch_I_MaxStation, tch_I_tchToolMax, tch_I_MaxExtStation, tch_i_tchToolNo, BtoStr(tch_b_Safe_Stations), stations, tools
				);
		}

	#endregion //methods

	}
}
