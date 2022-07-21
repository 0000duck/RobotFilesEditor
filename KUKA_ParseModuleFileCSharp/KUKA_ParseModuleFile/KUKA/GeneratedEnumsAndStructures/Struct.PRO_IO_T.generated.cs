using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class PRO_IO_T : Variable
	{
	#region fields
		private string mODULE;
		private MODE cOLD_BOOT_RUN;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "PRO_IO_T";} }
		public string MODULE { get { return mODULE; } set { Set(ref mODULE, value); } }
		public MODE COLD_BOOT_RUN { get { return cOLD_BOOT_RUN; } set { Set(ref cOLD_BOOT_RUN, value); } }
	#endregion //properties

	#region constructors
		public PRO_IO_T(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("MODULE")) mODULE = dataItems["MODULE"].ToString().Trim('"');
			if (dataItems.ContainsKey("COLD_BOOT_RUN")) cOLD_BOOT_RUN = (MODE)System.Enum.Parse(typeof(MODE), dataItems["COLD_BOOT_RUN"].ToString().TrimStart('#'), true);
		}

		public PRO_IO_T(string MODULE, MODE COLD_BOOT_RUN, string valName="")
		{
			mODULE = MODULE;
			cOLD_BOOT_RUN = COLD_BOOT_RUN;
			valName = ValName;
		}

		public PRO_IO_T(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["MODULE"] != null) mODULE = (string)mem["MODULE"];
			if (mem["COLD_BOOT_RUN"] != null) cOLD_BOOT_RUN = (MODE)mem["COLD_BOOT_RUN"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL PRO_IO_T " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				mODULE,
				cOLD_BOOT_RUN.ToString(),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{MODULE[] \"{0}\",COLD_BOOT_RUN {1}}}",
				mODULE, "#" + cOLD_BOOT_RUN.ToString()
				);
		}

	#endregion //methods

	}
}
