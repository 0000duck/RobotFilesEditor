using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class Palletizing_Struc : Variable
	{
	#region fields
		private int grp;
		private string name;
		private int parts;
		private int toolNo;
		private int inColl;
		private bool inTrig;
		private int waitTime;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "Palletizing_Struc";} }
		public int Grp { get { return grp; } set { Set(ref grp, value); } }
		public string Name { get { return name; } set { Set(ref name, value); } }
		public int Parts { get { return parts; } set { Set(ref parts, value); } }
		public int ToolNo { get { return toolNo; } set { Set(ref toolNo, value); } }
		public int InColl { get { return inColl; } set { Set(ref inColl, value); } }
		public bool InTrig { get { return inTrig; } set { Set(ref inTrig, value); } }
		public int WaitTime { get { return waitTime; } set { Set(ref waitTime, value); } }
	#endregion //properties

	#region constructors
		public Palletizing_Struc(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("Grp")) grp = int.Parse(dataItems["Grp"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("Name")) name = dataItems["Name"].ToString().Trim('"');
			if (dataItems.ContainsKey("Parts")) parts = int.Parse(dataItems["Parts"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("ToolNo")) toolNo = int.Parse(dataItems["ToolNo"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("InColl")) inColl = int.Parse(dataItems["InColl"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("InTrig")) inTrig = bool.Parse(dataItems["InTrig"].ToString());
			if (dataItems.ContainsKey("WaitTime")) waitTime = int.Parse(dataItems["WaitTime"].ToString(), CultureInfo.InvariantCulture);
		}

		public Palletizing_Struc(int Grp, string Name, int Parts, int ToolNo, int InColl, bool InTrig, int WaitTime, string valName="")
		{
			grp = Grp;
			name = Name;
			parts = Parts;
			toolNo = ToolNo;
			inColl = InColl;
			inTrig = InTrig;
			waitTime = WaitTime;
			valName = ValName;
		}

		public Palletizing_Struc(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["Grp"] != null) grp = (int)mem["Grp"];
			if (mem["Name"] != null) name = (string)mem["Name"];
			if (mem["Parts"] != null) parts = (int)mem["Parts"];
			if (mem["ToolNo"] != null) toolNo = (int)mem["ToolNo"];
			if (mem["InColl"] != null) inColl = (int)mem["InColl"];
			if (mem["InTrig"] != null) inTrig = (bool)mem["InTrig"];
			if (mem["WaitTime"] != null) waitTime = (int)mem["WaitTime"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL Palletizing_Struc " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				grp.ToString(CultureInfo.InvariantCulture),
				name,
				parts.ToString(CultureInfo.InvariantCulture),
				toolNo.ToString(CultureInfo.InvariantCulture),
				inColl.ToString(CultureInfo.InvariantCulture),
				inTrig.ToString(CultureInfo.InvariantCulture),
				waitTime.ToString(CultureInfo.InvariantCulture),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{Grp {0},Name[] \"{1}\",Parts {2},ToolNo {3},InColl {4},InTrig {5},WaitTime {6}}}",
				grp, name, parts, toolNo, inColl, BtoStr(inTrig), waitTime
				);
		}

	#endregion //methods

	}
}
