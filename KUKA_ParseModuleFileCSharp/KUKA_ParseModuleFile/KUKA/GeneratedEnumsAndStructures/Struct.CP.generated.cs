using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class CP : Variable
	{
	#region fields
		private double cP;
		private double oRI1;
		private double oRI2;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "CP";} }
		public double CP_ { get { return cP; } set { Set(ref cP, value); } }
		public double ORI1 { get { return oRI1; } set { Set(ref oRI1, value); } }
		public double ORI2 { get { return oRI2; } set { Set(ref oRI2, value); } }
	#endregion //properties

	#region constructors
		public CP(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("CP")) cP = double.Parse(dataItems["CP"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("ORI1")) oRI1 = double.Parse(dataItems["ORI1"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("ORI2")) oRI2 = double.Parse(dataItems["ORI2"].ToString(), CultureInfo.InvariantCulture);
		}

		public CP(double CP, double ORI1, double ORI2, string valName="")
		{
			cP = CP;
			oRI1 = ORI1;
			oRI2 = ORI2;
			valName = ValName;
		}

		public CP(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["CP"] != null) cP = (double)mem["CP"];
			if (mem["ORI1"] != null) oRI1 = (double)mem["ORI1"];
			if (mem["ORI2"] != null) oRI2 = (double)mem["ORI2"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL CP " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				cP.ToString(CultureInfo.InvariantCulture),
				oRI1.ToString(CultureInfo.InvariantCulture),
				oRI2.ToString(CultureInfo.InvariantCulture),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{CP {0},ORI1 {1},ORI2 {2}}}",
				cP, oRI1, oRI2
				);
		}

	#endregion //methods

	}
}
