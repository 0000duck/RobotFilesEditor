using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class GrpActuator : Variable
	{
	#region fields
		private int num;
		private string name;
		private GrpACT_Type type;
		private bool isUsed;
		private bool check;
		private bool c1Used;
		private bool c2Used;
		private bool c3Used;
		private bool c4Used;
		private int i_C1Retracted;
		private int i_C1Advanced;
		private int i_C2Retracted;
		private int i_C2Advanced;
		private int i_C3Retracted;
		private int i_C3Advanced;
		private int i_C4Retracted;
		private int i_C4Advanced;
		private int o_Retracted;
		private int o_Advanced;
		private int t_ErrWait;
		private double t_Retracted;
		private double t_Advanced;
		private int i_VAChnA;
		private double t_Ret_Pulse;
		private int tOutHandle;
		private Grp_STATE setState;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "GrpActuator";} }
		public int Num { get { return num; } set { Set(ref num, value); } }
		public string Name { get { return name; } set { Set(ref name, value); } }
		public GrpACT_Type Type { get { return type; } set { Set(ref type, value); } }
		public bool IsUsed { get { return isUsed; } set { Set(ref isUsed, value); } }
		public bool Check { get { return check; } set { Set(ref check, value); } }
		public bool C1Used { get { return c1Used; } set { Set(ref c1Used, value); } }
		public bool C2Used { get { return c2Used; } set { Set(ref c2Used, value); } }
		public bool C3Used { get { return c3Used; } set { Set(ref c3Used, value); } }
		public bool C4Used { get { return c4Used; } set { Set(ref c4Used, value); } }
		public int I_C1Retracted { get { return i_C1Retracted; } set { Set(ref i_C1Retracted, value); } }
		public int I_C1Advanced { get { return i_C1Advanced; } set { Set(ref i_C1Advanced, value); } }
		public int I_C2Retracted { get { return i_C2Retracted; } set { Set(ref i_C2Retracted, value); } }
		public int I_C2Advanced { get { return i_C2Advanced; } set { Set(ref i_C2Advanced, value); } }
		public int I_C3Retracted { get { return i_C3Retracted; } set { Set(ref i_C3Retracted, value); } }
		public int I_C3Advanced { get { return i_C3Advanced; } set { Set(ref i_C3Advanced, value); } }
		public int I_C4Retracted { get { return i_C4Retracted; } set { Set(ref i_C4Retracted, value); } }
		public int I_C4Advanced { get { return i_C4Advanced; } set { Set(ref i_C4Advanced, value); } }
		public int O_Retracted { get { return o_Retracted; } set { Set(ref o_Retracted, value); } }
		public int O_Advanced { get { return o_Advanced; } set { Set(ref o_Advanced, value); } }
		public int T_ErrWait { get { return t_ErrWait; } set { Set(ref t_ErrWait, value); } }
		public double T_Retracted { get { return t_Retracted; } set { Set(ref t_Retracted, value); } }
		public double T_Advanced { get { return t_Advanced; } set { Set(ref t_Advanced, value); } }
		public int I_VAChnA { get { return i_VAChnA; } set { Set(ref i_VAChnA, value); } }
		public double T_Ret_Pulse { get { return t_Ret_Pulse; } set { Set(ref t_Ret_Pulse, value); } }
		public int TOutHandle { get { return tOutHandle; } set { Set(ref tOutHandle, value); } }
		public Grp_STATE SetState { get { return setState; } set { Set(ref setState, value); } }
	#endregion //properties

	#region constructors
		public GrpActuator(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("Num")) num = int.Parse(dataItems["Num"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("Name")) name = dataItems["Name"].ToString().Trim('"');
			if (dataItems.ContainsKey("Type")) type = (GrpACT_Type)System.Enum.Parse(typeof(GrpACT_Type), dataItems["Type"].ToString().TrimStart('#'), true);
			if (dataItems.ContainsKey("IsUsed")) isUsed = bool.Parse(dataItems["IsUsed"].ToString());
			if (dataItems.ContainsKey("Check")) check = bool.Parse(dataItems["Check"].ToString());
			if (dataItems.ContainsKey("C1Used")) c1Used = bool.Parse(dataItems["C1Used"].ToString());
			if (dataItems.ContainsKey("C2Used")) c2Used = bool.Parse(dataItems["C2Used"].ToString());
			if (dataItems.ContainsKey("C3Used")) c3Used = bool.Parse(dataItems["C3Used"].ToString());
			if (dataItems.ContainsKey("C4Used")) c4Used = bool.Parse(dataItems["C4Used"].ToString());
			if (dataItems.ContainsKey("I_C1Retracted")) i_C1Retracted = int.Parse(dataItems["I_C1Retracted"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("I_C1Advanced")) i_C1Advanced = int.Parse(dataItems["I_C1Advanced"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("I_C2Retracted")) i_C2Retracted = int.Parse(dataItems["I_C2Retracted"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("I_C2Advanced")) i_C2Advanced = int.Parse(dataItems["I_C2Advanced"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("I_C3Retracted")) i_C3Retracted = int.Parse(dataItems["I_C3Retracted"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("I_C3Advanced")) i_C3Advanced = int.Parse(dataItems["I_C3Advanced"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("I_C4Retracted")) i_C4Retracted = int.Parse(dataItems["I_C4Retracted"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("I_C4Advanced")) i_C4Advanced = int.Parse(dataItems["I_C4Advanced"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("O_Retracted")) o_Retracted = int.Parse(dataItems["O_Retracted"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("O_Advanced")) o_Advanced = int.Parse(dataItems["O_Advanced"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("T_ErrWait")) t_ErrWait = int.Parse(dataItems["T_ErrWait"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("T_Retracted")) t_Retracted = double.Parse(dataItems["T_Retracted"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("T_Advanced")) t_Advanced = double.Parse(dataItems["T_Advanced"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("I_VAChnA")) i_VAChnA = int.Parse(dataItems["I_VAChnA"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("T_Ret_Pulse")) t_Ret_Pulse = double.Parse(dataItems["T_Ret_Pulse"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("TOutHandle")) tOutHandle = int.Parse(dataItems["TOutHandle"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("SetState")) setState = (Grp_STATE)System.Enum.Parse(typeof(Grp_STATE), dataItems["SetState"].ToString().TrimStart('#'), true);
		}

		public GrpActuator(int Num, string Name, GrpACT_Type Type, bool IsUsed, bool Check, bool C1Used, bool C2Used, bool C3Used, bool C4Used, int I_C1Retracted, int I_C1Advanced, int I_C2Retracted, int I_C2Advanced, int I_C3Retracted, int I_C3Advanced, int I_C4Retracted, int I_C4Advanced, int O_Retracted, int O_Advanced, int T_ErrWait, double T_Retracted, double T_Advanced, int I_VAChnA, double T_Ret_Pulse, int TOutHandle, Grp_STATE SetState, string valName="")
		{
			num = Num;
			name = Name;
			type = Type;
			isUsed = IsUsed;
			check = Check;
			c1Used = C1Used;
			c2Used = C2Used;
			c3Used = C3Used;
			c4Used = C4Used;
			i_C1Retracted = I_C1Retracted;
			i_C1Advanced = I_C1Advanced;
			i_C2Retracted = I_C2Retracted;
			i_C2Advanced = I_C2Advanced;
			i_C3Retracted = I_C3Retracted;
			i_C3Advanced = I_C3Advanced;
			i_C4Retracted = I_C4Retracted;
			i_C4Advanced = I_C4Advanced;
			o_Retracted = O_Retracted;
			o_Advanced = O_Advanced;
			t_ErrWait = T_ErrWait;
			t_Retracted = T_Retracted;
			t_Advanced = T_Advanced;
			i_VAChnA = I_VAChnA;
			t_Ret_Pulse = T_Ret_Pulse;
			tOutHandle = TOutHandle;
			setState = SetState;
			valName = ValName;
		}

		public GrpActuator(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["Num"] != null) num = (int)mem["Num"];
			if (mem["Name"] != null) name = (string)mem["Name"];
			if (mem["Type"] != null) type = (GrpACT_Type)mem["Type"];
			if (mem["IsUsed"] != null) isUsed = (bool)mem["IsUsed"];
			if (mem["Check"] != null) check = (bool)mem["Check"];
			if (mem["C1Used"] != null) c1Used = (bool)mem["C1Used"];
			if (mem["C2Used"] != null) c2Used = (bool)mem["C2Used"];
			if (mem["C3Used"] != null) c3Used = (bool)mem["C3Used"];
			if (mem["C4Used"] != null) c4Used = (bool)mem["C4Used"];
			if (mem["I_C1Retracted"] != null) i_C1Retracted = (int)mem["I_C1Retracted"];
			if (mem["I_C1Advanced"] != null) i_C1Advanced = (int)mem["I_C1Advanced"];
			if (mem["I_C2Retracted"] != null) i_C2Retracted = (int)mem["I_C2Retracted"];
			if (mem["I_C2Advanced"] != null) i_C2Advanced = (int)mem["I_C2Advanced"];
			if (mem["I_C3Retracted"] != null) i_C3Retracted = (int)mem["I_C3Retracted"];
			if (mem["I_C3Advanced"] != null) i_C3Advanced = (int)mem["I_C3Advanced"];
			if (mem["I_C4Retracted"] != null) i_C4Retracted = (int)mem["I_C4Retracted"];
			if (mem["I_C4Advanced"] != null) i_C4Advanced = (int)mem["I_C4Advanced"];
			if (mem["O_Retracted"] != null) o_Retracted = (int)mem["O_Retracted"];
			if (mem["O_Advanced"] != null) o_Advanced = (int)mem["O_Advanced"];
			if (mem["T_ErrWait"] != null) t_ErrWait = (int)mem["T_ErrWait"];
			if (mem["T_Retracted"] != null) t_Retracted = (double)mem["T_Retracted"];
			if (mem["T_Advanced"] != null) t_Advanced = (double)mem["T_Advanced"];
			if (mem["I_VAChnA"] != null) i_VAChnA = (int)mem["I_VAChnA"];
			if (mem["T_Ret_Pulse"] != null) t_Ret_Pulse = (double)mem["T_Ret_Pulse"];
			if (mem["TOutHandle"] != null) tOutHandle = (int)mem["TOutHandle"];
			if (mem["SetState"] != null) setState = (Grp_STATE)mem["SetState"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL GrpActuator " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				num.ToString(CultureInfo.InvariantCulture),
				name,
				type.ToString(),
				isUsed.ToString(CultureInfo.InvariantCulture),
				check.ToString(CultureInfo.InvariantCulture),
				c1Used.ToString(CultureInfo.InvariantCulture),
				c2Used.ToString(CultureInfo.InvariantCulture),
				c3Used.ToString(CultureInfo.InvariantCulture),
				c4Used.ToString(CultureInfo.InvariantCulture),
				i_C1Retracted.ToString(CultureInfo.InvariantCulture),
				i_C1Advanced.ToString(CultureInfo.InvariantCulture),
				i_C2Retracted.ToString(CultureInfo.InvariantCulture),
				i_C2Advanced.ToString(CultureInfo.InvariantCulture),
				i_C3Retracted.ToString(CultureInfo.InvariantCulture),
				i_C3Advanced.ToString(CultureInfo.InvariantCulture),
				i_C4Retracted.ToString(CultureInfo.InvariantCulture),
				i_C4Advanced.ToString(CultureInfo.InvariantCulture),
				o_Retracted.ToString(CultureInfo.InvariantCulture),
				o_Advanced.ToString(CultureInfo.InvariantCulture),
				t_ErrWait.ToString(CultureInfo.InvariantCulture),
				t_Retracted.ToString(CultureInfo.InvariantCulture),
				t_Advanced.ToString(CultureInfo.InvariantCulture),
				i_VAChnA.ToString(CultureInfo.InvariantCulture),
				t_Ret_Pulse.ToString(CultureInfo.InvariantCulture),
				tOutHandle.ToString(CultureInfo.InvariantCulture),
				setState.ToString(),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{Num {0},Name[] \"{1}\",Type {2},IsUsed {3},Check {4},C1Used {5},C2Used {6},C3Used {7},C4Used {8},I_C1Retracted {9},I_C1Advanced {10},I_C2Retracted {11},I_C2Advanced {12},I_C3Retracted {13},I_C3Advanced {14},I_C4Retracted {15},I_C4Advanced {16},O_Retracted {17},O_Advanced {18},T_ErrWait {19},T_Retracted {20},T_Advanced {21},I_VAChnA {22},T_Ret_Pulse {23},TOutHandle {24},SetState {25}}}",
				num, name, "#" + type.ToString(), BtoStr(isUsed), BtoStr(check), BtoStr(c1Used), BtoStr(c2Used), BtoStr(c3Used), BtoStr(c4Used), i_C1Retracted, i_C1Advanced, i_C2Retracted, i_C2Advanced, i_C3Retracted, i_C3Advanced, i_C4Retracted, i_C4Advanced, o_Retracted, o_Advanced, t_ErrWait, t_Retracted, t_Advanced, i_VAChnA, t_Ret_Pulse, tOutHandle, "#" + setState.ToString()
				);
		}

	#endregion //methods

	}
}
