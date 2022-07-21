using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class EXT_MOD_T : Variable
	{
	#region fields
		private string o_FILE;
		private int oPTION;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "EXT_MOD_T";} }
		public string O_FILE { get { return o_FILE; } set { Set(ref o_FILE, value); } }
		public int OPTION { get { return oPTION; } set { Set(ref oPTION, value); } }
	#endregion //properties

	#region constructors
		public EXT_MOD_T(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("O_FILE")) o_FILE = dataItems["O_FILE"].ToString().Trim('"');
			if (dataItems.ContainsKey("OPTION")) oPTION = int.Parse(dataItems["OPTION"].ToString(), CultureInfo.InvariantCulture);
		}

		public EXT_MOD_T(string O_FILE, int OPTION, string valName="")
		{
			o_FILE = O_FILE;
			oPTION = OPTION;
			valName = ValName;
		}

		public EXT_MOD_T(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["O_FILE"] != null) o_FILE = (string)mem["O_FILE"];
			if (mem["OPTION"] != null) oPTION = (int)mem["OPTION"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL EXT_MOD_T " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				o_FILE,
				oPTION.ToString(CultureInfo.InvariantCulture),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{O_FILE[] \"{0}\",OPTION {1}}}",
				o_FILE, oPTION
				);
		}

	#endregion //methods

	}
}
