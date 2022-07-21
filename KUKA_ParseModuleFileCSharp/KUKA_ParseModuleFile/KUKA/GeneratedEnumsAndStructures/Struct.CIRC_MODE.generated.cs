using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class CIRC_MODE : Variable
	{
	#region fields
		private CIRC_MODE_items aUX_PT;
		private CIRC_MODE_items tARGET_PT;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "CIRC_MODE";} }
		public CIRC_MODE_items AUX_PT { get { return aUX_PT; } set { Set(ref aUX_PT, value); } }
		public CIRC_MODE_items TARGET_PT { get { return tARGET_PT; } set { Set(ref tARGET_PT, value); } }
	#endregion //properties

	#region constructors
		public CIRC_MODE(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("AUX_PT")) aUX_PT = new CIRC_MODE_items(dataItems["AUX_PT"]);
			if (dataItems.ContainsKey("TARGET_PT")) tARGET_PT = new CIRC_MODE_items(dataItems["TARGET_PT"]);
		}

		public CIRC_MODE(CIRC_MODE_items AUX_PT, CIRC_MODE_items TARGET_PT, string valName="")
		{
			aUX_PT = AUX_PT;
			tARGET_PT = TARGET_PT;
			valName = ValName;
		}

		public CIRC_MODE(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			aUX_PT = new CIRC_MODE_items((DynamicMemory)mem["AUX_PT"]);
			tARGET_PT = new CIRC_MODE_items((DynamicMemory)mem["TARGET_PT"]);
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL CIRC_MODE " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				aUX_PT.ToString(),
				tARGET_PT.ToString(),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{AUX_PT {0},TARGET_PT {1}}}",
				aUX_PT, tARGET_PT
				);
		}

	#endregion //methods

	}
}
