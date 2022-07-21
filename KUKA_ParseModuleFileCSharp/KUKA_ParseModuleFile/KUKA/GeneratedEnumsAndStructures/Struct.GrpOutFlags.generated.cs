using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class GrpOutFlags : Variable
	{
	#region fields
		private bool oAdv;
		private bool oRet;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "GrpOutFlags";} }
		public bool OAdv { get { return oAdv; } set { Set(ref oAdv, value); } }
		public bool ORet { get { return oRet; } set { Set(ref oRet, value); } }
	#endregion //properties

	#region constructors
		public GrpOutFlags(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("OAdv")) oAdv = bool.Parse(dataItems["OAdv"].ToString());
			if (dataItems.ContainsKey("ORet")) oRet = bool.Parse(dataItems["ORet"].ToString());
		}

		public GrpOutFlags(bool OAdv, bool ORet, string valName="")
		{
			oAdv = OAdv;
			oRet = ORet;
			valName = ValName;
		}

		public GrpOutFlags(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["OAdv"] != null) oAdv = (bool)mem["OAdv"];
			if (mem["ORet"] != null) oRet = (bool)mem["ORet"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL GrpOutFlags " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				oAdv.ToString(CultureInfo.InvariantCulture),
				oRet.ToString(CultureInfo.InvariantCulture),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{OAdv {0},ORet {1}}}",
				BtoStr(oAdv), BtoStr(oRet)
				);
		}

	#endregion //methods

	}
}
