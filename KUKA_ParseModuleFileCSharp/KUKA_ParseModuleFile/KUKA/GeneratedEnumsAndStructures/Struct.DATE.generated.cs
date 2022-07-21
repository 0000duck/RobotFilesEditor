using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class DATE : Variable
	{
	#region fields
		private int cSEC;
		private int sEC;
		private int mIN;
		private int hOUR;
		private int dAY;
		private int mONTH;
		private int yEAR;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "DATE";} }
		public int CSEC { get { return cSEC; } set { Set(ref cSEC, value); } }
		public int SEC { get { return sEC; } set { Set(ref sEC, value); } }
		public int MIN { get { return mIN; } set { Set(ref mIN, value); } }
		public int HOUR { get { return hOUR; } set { Set(ref hOUR, value); } }
		public int DAY { get { return dAY; } set { Set(ref dAY, value); } }
		public int MONTH { get { return mONTH; } set { Set(ref mONTH, value); } }
		public int YEAR { get { return yEAR; } set { Set(ref yEAR, value); } }
	#endregion //properties

	#region constructors
		public DATE(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("CSEC")) cSEC = int.Parse(dataItems["CSEC"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("SEC")) sEC = int.Parse(dataItems["SEC"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("MIN")) mIN = int.Parse(dataItems["MIN"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("HOUR")) hOUR = int.Parse(dataItems["HOUR"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("DAY")) dAY = int.Parse(dataItems["DAY"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("MONTH")) mONTH = int.Parse(dataItems["MONTH"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("YEAR")) yEAR = int.Parse(dataItems["YEAR"].ToString(), CultureInfo.InvariantCulture);
		}

		public DATE(int CSEC, int SEC, int MIN, int HOUR, int DAY, int MONTH, int YEAR, string valName="")
		{
			cSEC = CSEC;
			sEC = SEC;
			mIN = MIN;
			hOUR = HOUR;
			dAY = DAY;
			mONTH = MONTH;
			yEAR = YEAR;
			valName = ValName;
		}

		public DATE(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["CSEC"] != null) cSEC = (int)mem["CSEC"];
			if (mem["SEC"] != null) sEC = (int)mem["SEC"];
			if (mem["MIN"] != null) mIN = (int)mem["MIN"];
			if (mem["HOUR"] != null) hOUR = (int)mem["HOUR"];
			if (mem["DAY"] != null) dAY = (int)mem["DAY"];
			if (mem["MONTH"] != null) mONTH = (int)mem["MONTH"];
			if (mem["YEAR"] != null) yEAR = (int)mem["YEAR"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL DATE " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				cSEC.ToString(CultureInfo.InvariantCulture),
				sEC.ToString(CultureInfo.InvariantCulture),
				mIN.ToString(CultureInfo.InvariantCulture),
				hOUR.ToString(CultureInfo.InvariantCulture),
				dAY.ToString(CultureInfo.InvariantCulture),
				mONTH.ToString(CultureInfo.InvariantCulture),
				yEAR.ToString(CultureInfo.InvariantCulture),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{CSEC {0},SEC {1},MIN {2},HOUR {3},DAY {4},MONTH {5},YEAR {6}}}",
				cSEC, sEC, mIN, hOUR, dAY, mONTH, yEAR
				);
		}

	#endregion //methods

	}
}
