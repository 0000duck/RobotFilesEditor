using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class CONSTVEL_PARA : Variable
	{
	#region fields
		private int constVelTyp;
		private int constVelPath;
		private bool constVelOnStart;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "CONSTVEL_PARA";} }
		public int ConstVelTyp { get { return constVelTyp; } set { Set(ref constVelTyp, value); } }
		public int ConstVelPath { get { return constVelPath; } set { Set(ref constVelPath, value); } }
		public bool ConstVelOnStart { get { return constVelOnStart; } set { Set(ref constVelOnStart, value); } }
	#endregion //properties

	#region constructors
		public CONSTVEL_PARA(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("ConstVelTyp")) constVelTyp = int.Parse(dataItems["ConstVelTyp"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("ConstVelPath")) constVelPath = int.Parse(dataItems["ConstVelPath"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("ConstVelOnStart")) constVelOnStart = bool.Parse(dataItems["ConstVelOnStart"].ToString());
		}

		public CONSTVEL_PARA(int ConstVelTyp, int ConstVelPath, bool ConstVelOnStart, string valName="")
		{
			constVelTyp = ConstVelTyp;
			constVelPath = ConstVelPath;
			constVelOnStart = ConstVelOnStart;
			valName = ValName;
		}

		public CONSTVEL_PARA(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["ConstVelTyp"] != null) constVelTyp = (int)mem["ConstVelTyp"];
			if (mem["ConstVelPath"] != null) constVelPath = (int)mem["ConstVelPath"];
			if (mem["ConstVelOnStart"] != null) constVelOnStart = (bool)mem["ConstVelOnStart"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL CONSTVEL_PARA " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				constVelTyp.ToString(CultureInfo.InvariantCulture),
				constVelPath.ToString(CultureInfo.InvariantCulture),
				constVelOnStart.ToString(CultureInfo.InvariantCulture),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{ConstVelTyp {0},ConstVelPath {1},ConstVelOnStart {2}}}",
				constVelTyp, constVelPath, BtoStr(constVelOnStart)
				);
		}

	#endregion //methods

	}
}
