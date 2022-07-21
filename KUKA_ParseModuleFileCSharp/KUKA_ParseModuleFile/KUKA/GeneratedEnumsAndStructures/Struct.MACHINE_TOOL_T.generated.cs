using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class MACHINE_TOOL_T : Variable
	{
	#region fields
		private int mACH_DEF_INDEX;
		private string pARENT;
		private string gEOMETRY;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "MACHINE_TOOL_T";} }
		public int MACH_DEF_INDEX { get { return mACH_DEF_INDEX; } set { Set(ref mACH_DEF_INDEX, value); } }
		public string PARENT { get { return pARENT; } set { Set(ref pARENT, value); } }
		public string GEOMETRY { get { return gEOMETRY; } set { Set(ref gEOMETRY, value); } }
	#endregion //properties

	#region constructors
		public MACHINE_TOOL_T(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("MACH_DEF_INDEX")) mACH_DEF_INDEX = int.Parse(dataItems["MACH_DEF_INDEX"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("PARENT")) pARENT = dataItems["PARENT"].ToString().Trim('"');
			if (dataItems.ContainsKey("GEOMETRY")) gEOMETRY = dataItems["GEOMETRY"].ToString().Trim('"');
		}

		public MACHINE_TOOL_T(int MACH_DEF_INDEX, string PARENT, string GEOMETRY, string valName="")
		{
			mACH_DEF_INDEX = MACH_DEF_INDEX;
			pARENT = PARENT;
			gEOMETRY = GEOMETRY;
			valName = ValName;
		}

		public MACHINE_TOOL_T(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["MACH_DEF_INDEX"] != null) mACH_DEF_INDEX = (int)mem["MACH_DEF_INDEX"];
			if (mem["PARENT"] != null) pARENT = (string)mem["PARENT"];
			if (mem["GEOMETRY"] != null) gEOMETRY = (string)mem["GEOMETRY"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL MACHINE_TOOL_T " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				mACH_DEF_INDEX.ToString(CultureInfo.InvariantCulture),
				pARENT,
				gEOMETRY,
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{MACH_DEF_INDEX {0},PARENT[] \"{1}\",GEOMETRY[] \"{2}\"}}",
				mACH_DEF_INDEX, pARENT, gEOMETRY
				);
		}

	#endregion //methods

	}
}
