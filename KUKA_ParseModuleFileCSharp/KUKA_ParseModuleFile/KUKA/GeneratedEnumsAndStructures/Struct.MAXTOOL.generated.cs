using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class MAXTOOL : Variable
	{
	#region fields
		private double lOAD_CM_R;
		private double lOAD_CM_Z;
		private double lOAD_M;
		private double lOAD_J;
		private double tOOL_R;
		private double tOOL_Z;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "MAXTOOL";} }
		public double LOAD_CM_R { get { return lOAD_CM_R; } set { Set(ref lOAD_CM_R, value); } }
		public double LOAD_CM_Z { get { return lOAD_CM_Z; } set { Set(ref lOAD_CM_Z, value); } }
		public double LOAD_M { get { return lOAD_M; } set { Set(ref lOAD_M, value); } }
		public double LOAD_J { get { return lOAD_J; } set { Set(ref lOAD_J, value); } }
		public double TOOL_R { get { return tOOL_R; } set { Set(ref tOOL_R, value); } }
		public double TOOL_Z { get { return tOOL_Z; } set { Set(ref tOOL_Z, value); } }
	#endregion //properties

	#region constructors
		public MAXTOOL(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("LOAD_CM_R")) lOAD_CM_R = double.Parse(dataItems["LOAD_CM_R"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("LOAD_CM_Z")) lOAD_CM_Z = double.Parse(dataItems["LOAD_CM_Z"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("LOAD_M")) lOAD_M = double.Parse(dataItems["LOAD_M"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("LOAD_J")) lOAD_J = double.Parse(dataItems["LOAD_J"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("TOOL_R")) tOOL_R = double.Parse(dataItems["TOOL_R"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("TOOL_Z")) tOOL_Z = double.Parse(dataItems["TOOL_Z"].ToString(), CultureInfo.InvariantCulture);
		}

		public MAXTOOL(double LOAD_CM_R, double LOAD_CM_Z, double LOAD_M, double LOAD_J, double TOOL_R, double TOOL_Z, string valName="")
		{
			lOAD_CM_R = LOAD_CM_R;
			lOAD_CM_Z = LOAD_CM_Z;
			lOAD_M = LOAD_M;
			lOAD_J = LOAD_J;
			tOOL_R = TOOL_R;
			tOOL_Z = TOOL_Z;
			valName = ValName;
		}

		public MAXTOOL(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["LOAD_CM_R"] != null) lOAD_CM_R = (double)mem["LOAD_CM_R"];
			if (mem["LOAD_CM_Z"] != null) lOAD_CM_Z = (double)mem["LOAD_CM_Z"];
			if (mem["LOAD_M"] != null) lOAD_M = (double)mem["LOAD_M"];
			if (mem["LOAD_J"] != null) lOAD_J = (double)mem["LOAD_J"];
			if (mem["TOOL_R"] != null) tOOL_R = (double)mem["TOOL_R"];
			if (mem["TOOL_Z"] != null) tOOL_Z = (double)mem["TOOL_Z"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL MAXTOOL " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				lOAD_CM_R.ToString(CultureInfo.InvariantCulture),
				lOAD_CM_Z.ToString(CultureInfo.InvariantCulture),
				lOAD_M.ToString(CultureInfo.InvariantCulture),
				lOAD_J.ToString(CultureInfo.InvariantCulture),
				tOOL_R.ToString(CultureInfo.InvariantCulture),
				tOOL_Z.ToString(CultureInfo.InvariantCulture),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{LOAD_CM_R {0},LOAD_CM_Z {1},LOAD_M {2},LOAD_J {3},TOOL_R {4},TOOL_Z {5}}}",
				lOAD_CM_R, lOAD_CM_Z, lOAD_M, lOAD_J, tOOL_R, tOOL_Z
				);
		}

	#endregion //methods

	}
}
