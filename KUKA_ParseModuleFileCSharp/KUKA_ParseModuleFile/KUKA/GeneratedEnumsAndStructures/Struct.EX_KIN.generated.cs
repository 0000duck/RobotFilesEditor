using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class EX_KIN : Variable
	{
	#region fields
		private EX_KIN_E eT1;
		private EX_KIN_E eT2;
		private EX_KIN_E eT3;
		private EX_KIN_E eT4;
		private EX_KIN_E eT5;
		private EX_KIN_E eT6;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "EX_KIN";} }
		public EX_KIN_E ET1 { get { return eT1; } set { Set(ref eT1, value); } }
		public EX_KIN_E ET2 { get { return eT2; } set { Set(ref eT2, value); } }
		public EX_KIN_E ET3 { get { return eT3; } set { Set(ref eT3, value); } }
		public EX_KIN_E ET4 { get { return eT4; } set { Set(ref eT4, value); } }
		public EX_KIN_E ET5 { get { return eT5; } set { Set(ref eT5, value); } }
		public EX_KIN_E ET6 { get { return eT6; } set { Set(ref eT6, value); } }
	#endregion //properties

	#region constructors
		public EX_KIN(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("ET1")) eT1 = (EX_KIN_E)System.Enum.Parse(typeof(EX_KIN_E), dataItems["ET1"].ToString().TrimStart('#'), true);
			if (dataItems.ContainsKey("ET2")) eT2 = (EX_KIN_E)System.Enum.Parse(typeof(EX_KIN_E), dataItems["ET2"].ToString().TrimStart('#'), true);
			if (dataItems.ContainsKey("ET3")) eT3 = (EX_KIN_E)System.Enum.Parse(typeof(EX_KIN_E), dataItems["ET3"].ToString().TrimStart('#'), true);
			if (dataItems.ContainsKey("ET4")) eT4 = (EX_KIN_E)System.Enum.Parse(typeof(EX_KIN_E), dataItems["ET4"].ToString().TrimStart('#'), true);
			if (dataItems.ContainsKey("ET5")) eT5 = (EX_KIN_E)System.Enum.Parse(typeof(EX_KIN_E), dataItems["ET5"].ToString().TrimStart('#'), true);
			if (dataItems.ContainsKey("ET6")) eT6 = (EX_KIN_E)System.Enum.Parse(typeof(EX_KIN_E), dataItems["ET6"].ToString().TrimStart('#'), true);
		}

		public EX_KIN(EX_KIN_E ET1, EX_KIN_E ET2, EX_KIN_E ET3, EX_KIN_E ET4, EX_KIN_E ET5, EX_KIN_E ET6, string valName="")
		{
			eT1 = ET1;
			eT2 = ET2;
			eT3 = ET3;
			eT4 = ET4;
			eT5 = ET5;
			eT6 = ET6;
			valName = ValName;
		}

		public EX_KIN(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["ET1"] != null) eT1 = (EX_KIN_E)mem["ET1"];
			if (mem["ET2"] != null) eT2 = (EX_KIN_E)mem["ET2"];
			if (mem["ET3"] != null) eT3 = (EX_KIN_E)mem["ET3"];
			if (mem["ET4"] != null) eT4 = (EX_KIN_E)mem["ET4"];
			if (mem["ET5"] != null) eT5 = (EX_KIN_E)mem["ET5"];
			if (mem["ET6"] != null) eT6 = (EX_KIN_E)mem["ET6"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL EX_KIN " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				eT1.ToString(),
				eT2.ToString(),
				eT3.ToString(),
				eT4.ToString(),
				eT5.ToString(),
				eT6.ToString(),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{ET1 {0},ET2 {1},ET3 {2},ET4 {3},ET5 {4},ET6 {5}}}",
				"#" + eT1.ToString(), "#" + eT2.ToString(), "#" + eT3.ToString(), "#" + eT4.ToString(), "#" + eT5.ToString(), "#" + eT6.ToString()
				);
		}

	#endregion //methods

	}
}
