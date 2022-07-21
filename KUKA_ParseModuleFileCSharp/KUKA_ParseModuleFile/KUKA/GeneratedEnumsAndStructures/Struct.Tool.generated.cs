using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class Tool : Variable
	{
	#region fields
		private int num;
		private string description;
		private FRAME frame;
		private LOAD load;
		private IPO_M_T type;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "Tool";} }
		public int Num { get { return num; } set { Set(ref num, value); } }
		public string Description { get { return description; } set { Set(ref description, value); } }
		public FRAME Frame { get { return frame; } set { Set(ref frame, value); } }
		public LOAD Load { get { return load; } set { Set(ref load, value); } }
		public IPO_M_T Type { get { return type; } set { Set(ref type, value); } }
	#endregion //properties

	#region constructors
		public Tool(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("Num")) num = int.Parse(dataItems["Num"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("Description")) description = dataItems["Description"].ToString().Trim('"');
			if (dataItems.ContainsKey("Frame")) frame = new FRAME(dataItems["Frame"]);
			if (dataItems.ContainsKey("Load")) load = new LOAD(dataItems["Load"]);
			if (dataItems.ContainsKey("Type")) type = (IPO_M_T)System.Enum.Parse(typeof(IPO_M_T), dataItems["Type"].ToString().TrimStart('#'), true);
		}

		public Tool(int Num, string Description, FRAME Frame, LOAD Load, IPO_M_T Type, string valName="")
		{
			num = Num;
			description = Description;
			frame = Frame;
			load = Load;
			type = Type;
			valName = ValName;
		}

		public Tool(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["Num"] != null) num = (int)mem["Num"];
			if (mem["Description"] != null) description = (string)mem["Description"];
			frame = new FRAME((DynamicMemory)mem["Frame"]);
			load = new LOAD((DynamicMemory)mem["Load"]);
			if (mem["Type"] != null) type = (IPO_M_T)mem["Type"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL Tool " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				num.ToString(CultureInfo.InvariantCulture),
				description,
				frame.ToString(),
				load.ToString(),
				type.ToString(),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{Num {0},Description[] \"{1}\",Frame {2},Load {3},Type {4}}}",
				num, description, frame, load, "#" + type.ToString()
				);
		}

	#endregion //methods

	}
}
