using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class SpotProcess : Variable
	{
	#region fields
		private int maxNoGuns;
		private int maxNoWeldTimer;
		private int maxNoDresser;
		private ObservableDictionary<int,Gun> guns;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "SpotProcess";} }
		public int MaxNoGuns { get { return maxNoGuns; } set { Set(ref maxNoGuns, value); } }
		public int MaxNoWeldTimer { get { return maxNoWeldTimer; } set { Set(ref maxNoWeldTimer, value); } }
		public int MaxNoDresser { get { return maxNoDresser; } set { Set(ref maxNoDresser, value); } }
		public ObservableDictionary<int,Gun> Guns { get { return guns; } set { Set(ref guns, value); } }
	#endregion //properties

	#region constructors
		public SpotProcess(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("MaxNoGuns")) maxNoGuns = int.Parse(dataItems["MaxNoGuns"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("MaxNoWeldTimer")) maxNoWeldTimer = int.Parse(dataItems["MaxNoWeldTimer"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("MaxNoDresser")) maxNoDresser = int.Parse(dataItems["MaxNoDresser"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("Guns")) guns = ObservableDictionary<int,Gun>.Parse(dataItems["Guns"].ToString());
		}

		public SpotProcess(int MaxNoGuns, int MaxNoWeldTimer, int MaxNoDresser, ObservableDictionary<int,Gun> Guns, string valName="")
		{
			maxNoGuns = MaxNoGuns;
			maxNoWeldTimer = MaxNoWeldTimer;
			maxNoDresser = MaxNoDresser;
			guns = Guns;
			valName = ValName;
		}

		public SpotProcess(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["MaxNoGuns"] != null) maxNoGuns = (int)mem["MaxNoGuns"];
			if (mem["MaxNoWeldTimer"] != null) maxNoWeldTimer = (int)mem["MaxNoWeldTimer"];
			if (mem["MaxNoDresser"] != null) maxNoDresser = (int)mem["MaxNoDresser"];
			if (mem["Guns"] != null) guns = (ObservableDictionary<int,Gun>)mem["Guns"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL SpotProcess " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				maxNoGuns.ToString(CultureInfo.InvariantCulture),
				maxNoWeldTimer.ToString(CultureInfo.InvariantCulture),
				maxNoDresser.ToString(CultureInfo.InvariantCulture),
				guns.ToString(),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{MaxNoGuns {0},MaxNoWeldTimer {1},MaxNoDresser {2},Guns {3}}}",
				maxNoGuns, maxNoWeldTimer, maxNoDresser, guns
				);
		}

	#endregion //methods

	}
}
