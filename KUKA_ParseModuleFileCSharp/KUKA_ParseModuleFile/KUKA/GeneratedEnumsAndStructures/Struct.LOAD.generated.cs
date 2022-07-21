using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class LOAD : Variable
	{
	#region fields
		private double m;
		private FRAME cM;
		private POS j;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "LOAD";} }
		public double M { get { return m; } set { Set(ref m, value); } }
		public FRAME CM { get { return cM; } set { Set(ref cM, value); } }
		public POS J { get { return j; } set { Set(ref j, value); } }
	#endregion //properties

	#region constructors
		public LOAD(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("M")) m = double.Parse(dataItems["M"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("CM")) cM = new FRAME(dataItems["CM"]);
			if (dataItems.ContainsKey("J")) j = new POS(dataItems["J"]);
		}

		public LOAD(double M, FRAME CM, POS J, string valName="")
		{
			m = M;
			cM = CM;
			j = J;
			valName = ValName;
		}

		public LOAD(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["M"] != null) m = (double)mem["M"];
			cM = new FRAME((DynamicMemory)mem["CM"]);
			j = new POS((DynamicMemory)mem["J"]);
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL LOAD " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				m.ToString(CultureInfo.InvariantCulture),
				cM.ToString(),
				j.ToString(),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{M {0},CM {1},J {2}}}",
				m, cM, j
				);
		}

	#endregion //methods

	}
}
