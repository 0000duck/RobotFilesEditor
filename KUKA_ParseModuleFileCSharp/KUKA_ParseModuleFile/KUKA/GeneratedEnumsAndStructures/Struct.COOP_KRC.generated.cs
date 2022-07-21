using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class COOP_KRC : Variable
	{
	#region fields
		private string iP_ADDR;
		private string nAME;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "COOP_KRC";} }
		public string IP_ADDR { get { return iP_ADDR; } set { Set(ref iP_ADDR, value); } }
		public string NAME { get { return nAME; } set { Set(ref nAME, value); } }
	#endregion //properties

	#region constructors
		public COOP_KRC(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("IP_ADDR")) iP_ADDR = dataItems["IP_ADDR"].ToString().Trim('"');
			if (dataItems.ContainsKey("NAME")) nAME = dataItems["NAME"].ToString().Trim('"');
		}

		public COOP_KRC(string IP_ADDR, string NAME, string valName="")
		{
			iP_ADDR = IP_ADDR;
			nAME = NAME;
			valName = ValName;
		}

		public COOP_KRC(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["IP_ADDR"] != null) iP_ADDR = (string)mem["IP_ADDR"];
			if (mem["NAME"] != null) nAME = (string)mem["NAME"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL COOP_KRC " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				iP_ADDR,
				nAME,
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{IP_ADDR[] \"{0}\",NAME[] \"{1}\"}}",
				iP_ADDR, nAME
				);
		}

	#endregion //methods

	}
}
