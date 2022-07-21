using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class CIRC_MODE_items : Variable
	{
	#region fields
		private CIRC_MODE_args oRI;
		private CIRC_MODE_args e1;
		private CIRC_MODE_args e2;
		private CIRC_MODE_args e3;
		private CIRC_MODE_args e4;
		private CIRC_MODE_args e5;
		private CIRC_MODE_args e6;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "CIRC_MODE_items";} }
		public CIRC_MODE_args ORI { get { return oRI; } set { Set(ref oRI, value); } }
		public CIRC_MODE_args E1 { get { return e1; } set { Set(ref e1, value); } }
		public CIRC_MODE_args E2 { get { return e2; } set { Set(ref e2, value); } }
		public CIRC_MODE_args E3 { get { return e3; } set { Set(ref e3, value); } }
		public CIRC_MODE_args E4 { get { return e4; } set { Set(ref e4, value); } }
		public CIRC_MODE_args E5 { get { return e5; } set { Set(ref e5, value); } }
		public CIRC_MODE_args E6 { get { return e6; } set { Set(ref e6, value); } }
	#endregion //properties

	#region constructors
		public CIRC_MODE_items(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("ORI")) oRI = (CIRC_MODE_args)System.Enum.Parse(typeof(CIRC_MODE_args), dataItems["ORI"].ToString().TrimStart('#'), true);
			if (dataItems.ContainsKey("E1")) e1 = (CIRC_MODE_args)System.Enum.Parse(typeof(CIRC_MODE_args), dataItems["E1"].ToString().TrimStart('#'), true);
			if (dataItems.ContainsKey("E2")) e2 = (CIRC_MODE_args)System.Enum.Parse(typeof(CIRC_MODE_args), dataItems["E2"].ToString().TrimStart('#'), true);
			if (dataItems.ContainsKey("E3")) e3 = (CIRC_MODE_args)System.Enum.Parse(typeof(CIRC_MODE_args), dataItems["E3"].ToString().TrimStart('#'), true);
			if (dataItems.ContainsKey("E4")) e4 = (CIRC_MODE_args)System.Enum.Parse(typeof(CIRC_MODE_args), dataItems["E4"].ToString().TrimStart('#'), true);
			if (dataItems.ContainsKey("E5")) e5 = (CIRC_MODE_args)System.Enum.Parse(typeof(CIRC_MODE_args), dataItems["E5"].ToString().TrimStart('#'), true);
			if (dataItems.ContainsKey("E6")) e6 = (CIRC_MODE_args)System.Enum.Parse(typeof(CIRC_MODE_args), dataItems["E6"].ToString().TrimStart('#'), true);
		}

		public CIRC_MODE_items(CIRC_MODE_args ORI, CIRC_MODE_args E1, CIRC_MODE_args E2, CIRC_MODE_args E3, CIRC_MODE_args E4, CIRC_MODE_args E5, CIRC_MODE_args E6, string valName="")
		{
			oRI = ORI;
			e1 = E1;
			e2 = E2;
			e3 = E3;
			e4 = E4;
			e5 = E5;
			e6 = E6;
			valName = ValName;
		}

		public CIRC_MODE_items(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["ORI"] != null) oRI = (CIRC_MODE_args)mem["ORI"];
			if (mem["E1"] != null) e1 = (CIRC_MODE_args)mem["E1"];
			if (mem["E2"] != null) e2 = (CIRC_MODE_args)mem["E2"];
			if (mem["E3"] != null) e3 = (CIRC_MODE_args)mem["E3"];
			if (mem["E4"] != null) e4 = (CIRC_MODE_args)mem["E4"];
			if (mem["E5"] != null) e5 = (CIRC_MODE_args)mem["E5"];
			if (mem["E6"] != null) e6 = (CIRC_MODE_args)mem["E6"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL CIRC_MODE_items " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				oRI.ToString(),
				e1.ToString(),
				e2.ToString(),
				e3.ToString(),
				e4.ToString(),
				e5.ToString(),
				e6.ToString(),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{ORI {0},E1 {1},E2 {2},E3 {3},E4 {4},E5 {5},E6 {6}}}",
				"#" + oRI.ToString(), "#" + e1.ToString(), "#" + e2.ToString(), "#" + e3.ToString(), "#" + e4.ToString(), "#" + e5.ToString(), "#" + e6.ToString()
				);
		}

	#endregion //methods

	}
}
