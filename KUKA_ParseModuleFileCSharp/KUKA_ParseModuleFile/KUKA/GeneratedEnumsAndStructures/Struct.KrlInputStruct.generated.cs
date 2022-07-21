using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class KrlInputStruct : Variable
	{
	#region fields
		private bool stopOnInput;
		private string module;
		private double min;
		private double max;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "KrlInputStruct";} }
		public bool StopOnInput { get { return stopOnInput; } set { Set(ref stopOnInput, value); } }
		public string Module { get { return module; } set { Set(ref module, value); } }
		public double Min { get { return min; } set { Set(ref min, value); } }
		public double Max { get { return max; } set { Set(ref max, value); } }
	#endregion //properties

	#region constructors
		public KrlInputStruct(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("StopOnInput")) stopOnInput = bool.Parse(dataItems["StopOnInput"].ToString());
			if (dataItems.ContainsKey("Module")) module = dataItems["Module"].ToString().Trim('"');
			if (dataItems.ContainsKey("Min")) min = double.Parse(dataItems["Min"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("Max")) max = double.Parse(dataItems["Max"].ToString(), CultureInfo.InvariantCulture);
		}

		public KrlInputStruct(bool StopOnInput, string Module, double Min, double Max, string valName="")
		{
			stopOnInput = StopOnInput;
			module = Module;
			min = Min;
			max = Max;
			valName = ValName;
		}

		public KrlInputStruct(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["StopOnInput"] != null) stopOnInput = (bool)mem["StopOnInput"];
			if (mem["Module"] != null) module = (string)mem["Module"];
			if (mem["Min"] != null) min = (double)mem["Min"];
			if (mem["Max"] != null) max = (double)mem["Max"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL KrlInputStruct " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				stopOnInput.ToString(CultureInfo.InvariantCulture),
				module,
				min.ToString(CultureInfo.InvariantCulture),
				max.ToString(CultureInfo.InvariantCulture),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{StopOnInput {0},Module[] \"{1}\",Min {2},Max {3}}}",
				BtoStr(stopOnInput), module, min, max
				);
		}

	#endregion //methods

	}
}
