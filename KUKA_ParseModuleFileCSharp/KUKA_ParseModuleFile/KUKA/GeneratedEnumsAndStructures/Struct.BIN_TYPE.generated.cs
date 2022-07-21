using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class BIN_TYPE : Variable
	{
	#region fields
		private int f_BIT;
		private int lEN;
		private PARITY pARITY;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "BIN_TYPE";} }
		public int F_BIT { get { return f_BIT; } set { Set(ref f_BIT, value); } }
		public int LEN { get { return lEN; } set { Set(ref lEN, value); } }
		public PARITY PARITY { get { return pARITY; } set { Set(ref pARITY, value); } }
	#endregion //properties

	#region constructors
		public BIN_TYPE(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("F_BIT")) f_BIT = int.Parse(dataItems["F_BIT"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("LEN")) lEN = int.Parse(dataItems["LEN"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("PARITY")) pARITY = (PARITY)System.Enum.Parse(typeof(PARITY), dataItems["PARITY"].ToString().TrimStart('#'), true);
		}

		public BIN_TYPE(int F_BIT, int LEN, PARITY PARITY, string valName="")
		{
			f_BIT = F_BIT;
			lEN = LEN;
			pARITY = PARITY;
			valName = ValName;
		}

		public BIN_TYPE(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["F_BIT"] != null) f_BIT = (int)mem["F_BIT"];
			if (mem["LEN"] != null) lEN = (int)mem["LEN"];
			if (mem["PARITY"] != null) pARITY = (PARITY)mem["PARITY"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL BIN_TYPE " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				f_BIT.ToString(CultureInfo.InvariantCulture),
				lEN.ToString(CultureInfo.InvariantCulture),
				pARITY.ToString(),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{F_BIT {0},LEN {1},PARITY {2}}}",
				f_BIT, lEN, "#" + pARITY.ToString()
				);
		}

	#endregion //methods

	}
}
