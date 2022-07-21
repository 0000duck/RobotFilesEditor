using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class ET_AX : Variable
	{
	#region fields
		private ET_AX_E tR_A1;
		private ET_AX_E tR_A2;
		private ET_AX_E tR_A3;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "ET_AX";} }
		public ET_AX_E TR_A1 { get { return tR_A1; } set { Set(ref tR_A1, value); } }
		public ET_AX_E TR_A2 { get { return tR_A2; } set { Set(ref tR_A2, value); } }
		public ET_AX_E TR_A3 { get { return tR_A3; } set { Set(ref tR_A3, value); } }
	#endregion //properties

	#region constructors
		public ET_AX(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("TR_A1")) tR_A1 = (ET_AX_E)System.Enum.Parse(typeof(ET_AX_E), dataItems["TR_A1"].ToString().TrimStart('#'), true);
			if (dataItems.ContainsKey("TR_A2")) tR_A2 = (ET_AX_E)System.Enum.Parse(typeof(ET_AX_E), dataItems["TR_A2"].ToString().TrimStart('#'), true);
			if (dataItems.ContainsKey("TR_A3")) tR_A3 = (ET_AX_E)System.Enum.Parse(typeof(ET_AX_E), dataItems["TR_A3"].ToString().TrimStart('#'), true);
		}

		public ET_AX(ET_AX_E TR_A1, ET_AX_E TR_A2, ET_AX_E TR_A3, string valName="")
		{
			tR_A1 = TR_A1;
			tR_A2 = TR_A2;
			tR_A3 = TR_A3;
			valName = ValName;
		}

		public ET_AX(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["TR_A1"] != null) tR_A1 = (ET_AX_E)mem["TR_A1"];
			if (mem["TR_A2"] != null) tR_A2 = (ET_AX_E)mem["TR_A2"];
			if (mem["TR_A3"] != null) tR_A3 = (ET_AX_E)mem["TR_A3"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL ET_AX " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				tR_A1.ToString(),
				tR_A2.ToString(),
				tR_A3.ToString(),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{TR_A1 {0},TR_A2 {1},TR_A3 {2}}}",
				"#" + tR_A1.ToString(), "#" + tR_A2.ToString(), "#" + tR_A3.ToString()
				);
		}

	#endregion //methods

	}
}
