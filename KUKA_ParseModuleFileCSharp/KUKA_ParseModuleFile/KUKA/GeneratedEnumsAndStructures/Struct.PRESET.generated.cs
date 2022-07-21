using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class PRESET : Variable
	{
	#region fields
		private int oUT;
		private string pKG;
		private int eRR;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "PRESET";} }
		public int OUT { get { return oUT; } set { Set(ref oUT, value); } }
		public string PKG { get { return pKG; } set { Set(ref pKG, value); } }
		public int ERR { get { return eRR; } set { Set(ref eRR, value); } }
	#endregion //properties

	#region constructors
		public PRESET(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("OUT")) oUT = int.Parse(dataItems["OUT"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("PKG")) pKG = dataItems["PKG"].ToString().Trim('"');
			if (dataItems.ContainsKey("ERR")) eRR = int.Parse(dataItems["ERR"].ToString(), CultureInfo.InvariantCulture);
		}

		public PRESET(int OUT, string PKG, int ERR, string valName="")
		{
			oUT = OUT;
			pKG = PKG;
			eRR = ERR;
			valName = ValName;
		}

		public PRESET(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["OUT"] != null) oUT = (int)mem["OUT"];
			if (mem["PKG"] != null) pKG = (string)mem["PKG"];
			if (mem["ERR"] != null) eRR = (int)mem["ERR"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL PRESET " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				oUT.ToString(CultureInfo.InvariantCulture),
				pKG,
				eRR.ToString(CultureInfo.InvariantCulture),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{OUT {0},PKG[] \"{1}\",ERR {2}}}",
				oUT, pKG, eRR
				);
		}

	#endregion //methods

	}
}
