using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class swp_limits : Variable
	{
	#region fields
		private int lower;
		private int upper;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "swp_limits";} }
		public int Lower { get { return lower; } set { Set(ref lower, value); } }
		public int Upper { get { return upper; } set { Set(ref upper, value); } }
	#endregion //properties

	#region constructors
		public swp_limits(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("lower")) lower = int.Parse(dataItems["lower"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("upper")) upper = int.Parse(dataItems["upper"].ToString(), CultureInfo.InvariantCulture);
		}

		public swp_limits(int Lower, int Upper, string valName="")
		{
			lower = Lower;
			upper = Upper;
			valName = ValName;
		}

		public swp_limits(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["lower"] != null) lower = (int)mem["lower"];
			if (mem["upper"] != null) upper = (int)mem["upper"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL swp_limits " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				lower.ToString(CultureInfo.InvariantCulture),
				upper.ToString(CultureInfo.InvariantCulture),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{lower {0},upper {1}}}",
				lower, upper
				);
		}

	#endregion //methods

	}
}
