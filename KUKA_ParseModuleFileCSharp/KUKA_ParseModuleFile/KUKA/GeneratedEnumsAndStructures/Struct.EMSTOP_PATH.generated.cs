using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class EMSTOP_PATH : Variable
	{
	#region fields
		private MODE t1;
		private MODE t2;
		private MODE aUT;
		private MODE eX;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "EMSTOP_PATH";} }
		public MODE T1 { get { return t1; } set { Set(ref t1, value); } }
		public MODE T2 { get { return t2; } set { Set(ref t2, value); } }
		public MODE AUT { get { return aUT; } set { Set(ref aUT, value); } }
		public MODE EX { get { return eX; } set { Set(ref eX, value); } }
	#endregion //properties

	#region constructors
		public EMSTOP_PATH(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("T1")) t1 = (MODE)System.Enum.Parse(typeof(MODE), dataItems["T1"].ToString().TrimStart('#'), true);
			if (dataItems.ContainsKey("T2")) t2 = (MODE)System.Enum.Parse(typeof(MODE), dataItems["T2"].ToString().TrimStart('#'), true);
			if (dataItems.ContainsKey("AUT")) aUT = (MODE)System.Enum.Parse(typeof(MODE), dataItems["AUT"].ToString().TrimStart('#'), true);
			if (dataItems.ContainsKey("EX")) eX = (MODE)System.Enum.Parse(typeof(MODE), dataItems["EX"].ToString().TrimStart('#'), true);
		}

		public EMSTOP_PATH(MODE T1, MODE T2, MODE AUT, MODE EX, string valName="")
		{
			t1 = T1;
			t2 = T2;
			aUT = AUT;
			eX = EX;
			valName = ValName;
		}

		public EMSTOP_PATH(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["T1"] != null) t1 = (MODE)mem["T1"];
			if (mem["T2"] != null) t2 = (MODE)mem["T2"];
			if (mem["AUT"] != null) aUT = (MODE)mem["AUT"];
			if (mem["EX"] != null) eX = (MODE)mem["EX"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL EMSTOP_PATH " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				t1.ToString(),
				t2.ToString(),
				aUT.ToString(),
				eX.ToString(),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{T1 {0},T2 {1},AUT {2},EX {3}}}",
				"#" + t1.ToString(), "#" + t2.ToString(), "#" + aUT.ToString(), "#" + eX.ToString()
				);
		}

	#endregion //methods

	}
}
