using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class DHART : Variable
	{
	#region fields
		private double dHART_A;
		private double dHART_D;
		private double dHART_ALPHA;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "DHART";} }
		public double DHART_A { get { return dHART_A; } set { Set(ref dHART_A, value); } }
		public double DHART_D { get { return dHART_D; } set { Set(ref dHART_D, value); } }
		public double DHART_ALPHA { get { return dHART_ALPHA; } set { Set(ref dHART_ALPHA, value); } }
	#endregion //properties

	#region constructors
		public DHART(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("DHART_A")) dHART_A = double.Parse(dataItems["DHART_A"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("DHART_D")) dHART_D = double.Parse(dataItems["DHART_D"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("DHART_ALPHA")) dHART_ALPHA = double.Parse(dataItems["DHART_ALPHA"].ToString(), CultureInfo.InvariantCulture);
		}

		public DHART(double DHART_A, double DHART_D, double DHART_ALPHA, string valName="")
		{
			dHART_A = DHART_A;
			dHART_D = DHART_D;
			dHART_ALPHA = DHART_ALPHA;
			valName = ValName;
		}

		public DHART(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["DHART_A"] != null) dHART_A = (double)mem["DHART_A"];
			if (mem["DHART_D"] != null) dHART_D = (double)mem["DHART_D"];
			if (mem["DHART_ALPHA"] != null) dHART_ALPHA = (double)mem["DHART_ALPHA"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL DHART " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				dHART_A.ToString(CultureInfo.InvariantCulture),
				dHART_D.ToString(CultureInfo.InvariantCulture),
				dHART_ALPHA.ToString(CultureInfo.InvariantCulture),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{DHART_A {0},DHART_D {1},DHART_ALPHA {2}}}",
				dHART_A, dHART_D, dHART_ALPHA
				);
		}

	#endregion //methods

	}
}
