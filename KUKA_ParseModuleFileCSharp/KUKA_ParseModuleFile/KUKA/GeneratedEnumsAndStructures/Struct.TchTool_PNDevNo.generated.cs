using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class TchTool_PNDevNo : Variable
	{
	#region fields
		private int fromIndex;
		private int num;
		private int pNDevNo;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "TchTool_PNDevNo";} }
		public int FromIndex { get { return fromIndex; } set { Set(ref fromIndex, value); } }
		public int Num { get { return num; } set { Set(ref num, value); } }
		public int PNDevNo { get { return pNDevNo; } set { Set(ref pNDevNo, value); } }
	#endregion //properties

	#region constructors
		public TchTool_PNDevNo(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("FromIndex")) fromIndex = int.Parse(dataItems["FromIndex"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("Num")) num = int.Parse(dataItems["Num"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("PNDevNo")) pNDevNo = int.Parse(dataItems["PNDevNo"].ToString(), CultureInfo.InvariantCulture);
		}

		public TchTool_PNDevNo(int FromIndex, int Num, int PNDevNo, string valName="")
		{
			fromIndex = FromIndex;
			num = Num;
			pNDevNo = PNDevNo;
			valName = ValName;
		}

		public TchTool_PNDevNo(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["FromIndex"] != null) fromIndex = (int)mem["FromIndex"];
			if (mem["Num"] != null) num = (int)mem["Num"];
			if (mem["PNDevNo"] != null) pNDevNo = (int)mem["PNDevNo"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL TchTool_PNDevNo " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				fromIndex.ToString(CultureInfo.InvariantCulture),
				num.ToString(CultureInfo.InvariantCulture),
				pNDevNo.ToString(CultureInfo.InvariantCulture),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{FromIndex {0},Num {1},PNDevNo {2}}}",
				fromIndex, num, pNDevNo
				);
		}

	#endregion //methods

	}
}
