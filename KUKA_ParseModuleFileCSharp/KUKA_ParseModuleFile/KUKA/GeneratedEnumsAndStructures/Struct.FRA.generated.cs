using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class FRA : Variable
	{
	#region fields
		private int n;
		private int d;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "FRA";} }
		public int N { get { return n; } set { Set(ref n, value); } }
		public int D { get { return d; } set { Set(ref d, value); } }
	#endregion //properties

	#region constructors
		public FRA(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("N")) n = int.Parse(dataItems["N"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("D")) d = int.Parse(dataItems["D"].ToString(), CultureInfo.InvariantCulture);
		}

		public FRA(int N, int D, string valName="")
		{
			n = N;
			d = D;
			valName = ValName;
		}

		public FRA(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["N"] != null) n = (int)mem["N"];
			if (mem["D"] != null) d = (int)mem["D"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL FRA " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				n.ToString(CultureInfo.InvariantCulture),
				d.ToString(CultureInfo.InvariantCulture),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{N {0},D {1}}}",
				n, d
				);
		}

	#endregion //methods

	}
}
