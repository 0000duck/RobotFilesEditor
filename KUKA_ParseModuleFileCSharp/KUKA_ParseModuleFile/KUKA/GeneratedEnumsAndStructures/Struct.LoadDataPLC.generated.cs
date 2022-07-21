using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class LoadDataPLC : Variable
	{
	#region fields
		private string val;
		private LOAD load;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "LoadDataPLC";} }
		public string Val { get { return val; } set { Set(ref val, value); } }
		public LOAD Load { get { return load; } set { Set(ref load, value); } }
	#endregion //properties

	#region constructors
		public LoadDataPLC(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("val")) val = dataItems["val"].ToString().Trim('"');
			if (dataItems.ContainsKey("Load")) load = new LOAD(dataItems["Load"]);
		}

		public LoadDataPLC(string Val, LOAD Load, string valName="")
		{
			val = Val;
			load = Load;
			valName = ValName;
		}

		public LoadDataPLC(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["val"] != null) val = (string)mem["val"];
			load = new LOAD((DynamicMemory)mem["Load"]);
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL LoadDataPLC " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				val,
				load.ToString(),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{val[] \"{0}\",Load {1}}}",
				val, load
				);
		}

	#endregion //methods

	}
}
