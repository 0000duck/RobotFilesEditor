using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class ERR_MESS : Variable
	{
	#region fields
		private string p;
		private int e;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "ERR_MESS";} }
		public string P { get { return p; } set { Set(ref p, value); } }
		public int E { get { return e; } set { Set(ref e, value); } }
	#endregion //properties

	#region constructors
		public ERR_MESS(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("P")) p = dataItems["P"].ToString().Trim('"');
			if (dataItems.ContainsKey("E")) e = int.Parse(dataItems["E"].ToString(), CultureInfo.InvariantCulture);
		}

		public ERR_MESS(string P, int E, string valName="")
		{
			p = P;
			e = E;
			valName = ValName;
		}

		public ERR_MESS(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["P"] != null) p = (string)mem["P"];
			if (mem["E"] != null) e = (int)mem["E"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL ERR_MESS " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				p,
				e.ToString(CultureInfo.InvariantCulture),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{P[] \"{0}\",E {1}}}",
				p, e
				);
		}

	#endregion //methods

	}
}
