using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class JERK_STRUC : Variable
	{
	#region fields
		private double cP;
		private double oRI;
		private E6AXIS aX;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "JERK_STRUC";} }
		public double CP { get { return cP; } set { Set(ref cP, value); } }
		public double ORI { get { return oRI; } set { Set(ref oRI, value); } }
		public E6AXIS AX { get { return aX; } set { Set(ref aX, value); } }
	#endregion //properties

	#region constructors
		public JERK_STRUC(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("CP")) cP = double.Parse(dataItems["CP"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("ORI")) oRI = double.Parse(dataItems["ORI"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("AX")) aX = new E6AXIS(dataItems["AX"]);
		}

		public JERK_STRUC(double CP, double ORI, E6AXIS AX, string valName="")
		{
			cP = CP;
			oRI = ORI;
			aX = AX;
			valName = ValName;
		}

		public JERK_STRUC(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["CP"] != null) cP = (double)mem["CP"];
			if (mem["ORI"] != null) oRI = (double)mem["ORI"];
			aX = new E6AXIS((DynamicMemory)mem["AX"]);
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL JERK_STRUC " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				cP.ToString(CultureInfo.InvariantCulture),
				oRI.ToString(CultureInfo.InvariantCulture),
				aX.ToString(),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{CP {0},ORI {1},AX {2}}}",
				cP, oRI, aX
				);
		}

	#endregion //methods

	}
}
