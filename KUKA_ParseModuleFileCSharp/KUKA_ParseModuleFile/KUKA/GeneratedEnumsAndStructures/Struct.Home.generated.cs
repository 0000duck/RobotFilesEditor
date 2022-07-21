using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class Home : Variable
	{
	#region fields
		private int num;
		private string description;
		private E6AXIS position;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "Home";} }
		public int Num { get { return num; } set { Set(ref num, value); } }
		public string Description { get { return description; } set { Set(ref description, value); } }
		public E6AXIS Position { get { return position; } set { Set(ref position, value); } }
	#endregion //properties

	#region constructors
		public Home(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("Num")) num = int.Parse(dataItems["Num"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("Description")) description = dataItems["Description"].ToString().Trim('"');
			if (dataItems.ContainsKey("Position")) position = new E6AXIS(dataItems["Position"]);
		}

		public Home(int Num, string Description, E6AXIS Position, string valName="")
		{
			num = Num;
			description = Description;
			position = Position;
			valName = ValName;
		}

		public Home(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["Num"] != null) num = (int)mem["Num"];
			if (mem["Description"] != null) description = (string)mem["Description"];
			position = new E6AXIS((DynamicMemory)mem["Position"]);
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL Home " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				num.ToString(CultureInfo.InvariantCulture),
				description,
				position.ToString(),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{Num {0},Description[] \"{1}\",Position {2}}}",
				num, description, position
				);
		}

	#endregion //methods

	}
}
