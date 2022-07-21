using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class Base : Variable
	{
	#region fields
		private int num;
		private string description;
		private FRAME frame;
		private IPO_M_T type;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "Base";} }
		public int Num { get { return num; } set { Set(ref num, value); } }
		public string Description { get { return description; } set { Set(ref description, value); } }
		public FRAME Frame { get { return frame; } set { Set(ref frame, value); } }
		public IPO_M_T Type { get { return type; } set { Set(ref type, value); } }
	#endregion //properties

	#region constructors
		public Base(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("Num")) num = int.Parse(dataItems["Num"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("Description")) description = dataItems["Description"].ToString().Trim('"');
			if (dataItems.ContainsKey("Frame")) frame = new FRAME(dataItems["Frame"]);
			if (dataItems.ContainsKey("Type")) type = (IPO_M_T)System.Enum.Parse(typeof(IPO_M_T), dataItems["Type"].ToString().TrimStart('#'), true);
		}

		public Base(int Num, string Description, FRAME Frame, IPO_M_T Type, string valName="")
		{
			num = Num;
			description = Description;
			frame = Frame;
			type = Type;
			valName = ValName;
		}

		public Base(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["Num"] != null) num = (int)mem["Num"];
			if (mem["Description"] != null) description = (string)mem["Description"];
			frame = new FRAME((DynamicMemory)mem["Frame"]);
			if (mem["Type"] != null) type = (IPO_M_T)mem["Type"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL Base " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				num.ToString(CultureInfo.InvariantCulture),
				description,
				frame.ToString(),
				type.ToString(),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{Num {0},Description[] \"{1}\",Frame {2},Type {3}}}",
				num, description, frame, "#" + type.ToString()
				);
		}

	#endregion //methods

	}
}
