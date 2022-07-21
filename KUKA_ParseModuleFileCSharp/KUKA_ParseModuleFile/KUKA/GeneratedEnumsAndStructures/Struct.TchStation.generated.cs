using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class TchStation : Variable
	{
	#region fields
		private int num;
		private bool tch_b_Cover_present;
		private int tch_i_Station_BASE;
		private int tch_i_TimeOutCover;
		private int tch_i_rel_Dis_Undocked;
		private int tch_i_rel_Dis_Docked;
		private int tch_i_rel_Dis_Sensor;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "TchStation";} }
		public int Num { get { return num; } set { Set(ref num, value); } }
		public bool Tch_b_Cover_present { get { return tch_b_Cover_present; } set { Set(ref tch_b_Cover_present, value); } }
		public int Tch_i_Station_BASE { get { return tch_i_Station_BASE; } set { Set(ref tch_i_Station_BASE, value); } }
		public int Tch_i_TimeOutCover { get { return tch_i_TimeOutCover; } set { Set(ref tch_i_TimeOutCover, value); } }
		public int Tch_i_rel_Dis_Undocked { get { return tch_i_rel_Dis_Undocked; } set { Set(ref tch_i_rel_Dis_Undocked, value); } }
		public int Tch_i_rel_Dis_Docked { get { return tch_i_rel_Dis_Docked; } set { Set(ref tch_i_rel_Dis_Docked, value); } }
		public int Tch_i_rel_Dis_Sensor { get { return tch_i_rel_Dis_Sensor; } set { Set(ref tch_i_rel_Dis_Sensor, value); } }
	#endregion //properties

	#region constructors
		public TchStation(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("Num")) num = int.Parse(dataItems["Num"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("tch_b_Cover_present")) tch_b_Cover_present = bool.Parse(dataItems["tch_b_Cover_present"].ToString());
			if (dataItems.ContainsKey("tch_i_Station_BASE")) tch_i_Station_BASE = int.Parse(dataItems["tch_i_Station_BASE"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("tch_i_TimeOutCover")) tch_i_TimeOutCover = int.Parse(dataItems["tch_i_TimeOutCover"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("tch_i_rel_Dis_Undocked")) tch_i_rel_Dis_Undocked = int.Parse(dataItems["tch_i_rel_Dis_Undocked"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("tch_i_rel_Dis_Docked")) tch_i_rel_Dis_Docked = int.Parse(dataItems["tch_i_rel_Dis_Docked"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("tch_i_rel_Dis_Sensor")) tch_i_rel_Dis_Sensor = int.Parse(dataItems["tch_i_rel_Dis_Sensor"].ToString(), CultureInfo.InvariantCulture);
		}

		public TchStation(int Num, bool Tch_b_Cover_present, int Tch_i_Station_BASE, int Tch_i_TimeOutCover, int Tch_i_rel_Dis_Undocked, int Tch_i_rel_Dis_Docked, int Tch_i_rel_Dis_Sensor, string valName="")
		{
			num = Num;
			tch_b_Cover_present = Tch_b_Cover_present;
			tch_i_Station_BASE = Tch_i_Station_BASE;
			tch_i_TimeOutCover = Tch_i_TimeOutCover;
			tch_i_rel_Dis_Undocked = Tch_i_rel_Dis_Undocked;
			tch_i_rel_Dis_Docked = Tch_i_rel_Dis_Docked;
			tch_i_rel_Dis_Sensor = Tch_i_rel_Dis_Sensor;
			valName = ValName;
		}

		public TchStation(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["Num"] != null) num = (int)mem["Num"];
			if (mem["tch_b_Cover_present"] != null) tch_b_Cover_present = (bool)mem["tch_b_Cover_present"];
			if (mem["tch_i_Station_BASE"] != null) tch_i_Station_BASE = (int)mem["tch_i_Station_BASE"];
			if (mem["tch_i_TimeOutCover"] != null) tch_i_TimeOutCover = (int)mem["tch_i_TimeOutCover"];
			if (mem["tch_i_rel_Dis_Undocked"] != null) tch_i_rel_Dis_Undocked = (int)mem["tch_i_rel_Dis_Undocked"];
			if (mem["tch_i_rel_Dis_Docked"] != null) tch_i_rel_Dis_Docked = (int)mem["tch_i_rel_Dis_Docked"];
			if (mem["tch_i_rel_Dis_Sensor"] != null) tch_i_rel_Dis_Sensor = (int)mem["tch_i_rel_Dis_Sensor"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL TchStation " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				num.ToString(CultureInfo.InvariantCulture),
				tch_b_Cover_present.ToString(CultureInfo.InvariantCulture),
				tch_i_Station_BASE.ToString(CultureInfo.InvariantCulture),
				tch_i_TimeOutCover.ToString(CultureInfo.InvariantCulture),
				tch_i_rel_Dis_Undocked.ToString(CultureInfo.InvariantCulture),
				tch_i_rel_Dis_Docked.ToString(CultureInfo.InvariantCulture),
				tch_i_rel_Dis_Sensor.ToString(CultureInfo.InvariantCulture),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{Num {0},tch_b_Cover_present {1},tch_i_Station_BASE {2},tch_i_TimeOutCover {3},tch_i_rel_Dis_Undocked {4},tch_i_rel_Dis_Docked {5},tch_i_rel_Dis_Sensor {6}}}",
				num, BtoStr(tch_b_Cover_present), tch_i_Station_BASE, tch_i_TimeOutCover, tch_i_rel_Dis_Undocked, tch_i_rel_Dis_Docked, tch_i_rel_Dis_Sensor
				);
		}

	#endregion //methods

	}
}
