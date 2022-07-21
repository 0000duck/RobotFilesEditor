using System.Collections.Generic;
using System.Globalization;
using ParseModuleFile.KUKA.Enums;
using System.Collections.ObjectModel;

namespace ParseModuleFile.KUKA.DataTypes
{
	public partial class TRPSPIN : Variable
	{
	#region fields
		private int tRPSP_AXIS;
		private int tRPSP_COP_AX;
		private double tRPSP_A;
		private double tRPSP_B;
		private double tRPSP_C;
		private double tRPSP_D;
	#endregion //fields

	#region properties
		public override string DataTypeName { get { return "TRPSPIN";} }
		public int TRPSP_AXIS { get { return tRPSP_AXIS; } set { Set(ref tRPSP_AXIS, value); } }
		public int TRPSP_COP_AX { get { return tRPSP_COP_AX; } set { Set(ref tRPSP_COP_AX, value); } }
		public double TRPSP_A { get { return tRPSP_A; } set { Set(ref tRPSP_A, value); } }
		public double TRPSP_B { get { return tRPSP_B; } set { Set(ref tRPSP_B, value); } }
		public double TRPSP_C { get { return tRPSP_C; } set { Set(ref tRPSP_C, value); } }
		public double TRPSP_D { get { return tRPSP_D; } set { Set(ref tRPSP_D, value); } }
	#endregion //properties

	#region constructors
		public TRPSPIN(ANTLR.DataItems dataItems)
		{
			if (dataItems == null) return;
			if (dataItems.ContainsKey("TRPSP_AXIS")) tRPSP_AXIS = int.Parse(dataItems["TRPSP_AXIS"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("TRPSP_COP_AX")) tRPSP_COP_AX = int.Parse(dataItems["TRPSP_COP_AX"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("TRPSP_A")) tRPSP_A = double.Parse(dataItems["TRPSP_A"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("TRPSP_B")) tRPSP_B = double.Parse(dataItems["TRPSP_B"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("TRPSP_C")) tRPSP_C = double.Parse(dataItems["TRPSP_C"].ToString(), CultureInfo.InvariantCulture);
			if (dataItems.ContainsKey("TRPSP_D")) tRPSP_D = double.Parse(dataItems["TRPSP_D"].ToString(), CultureInfo.InvariantCulture);
		}

		public TRPSPIN(int TRPSP_AXIS, int TRPSP_COP_AX, double TRPSP_A, double TRPSP_B, double TRPSP_C, double TRPSP_D, string valName="")
		{
			tRPSP_AXIS = TRPSP_AXIS;
			tRPSP_COP_AX = TRPSP_COP_AX;
			tRPSP_A = TRPSP_A;
			tRPSP_B = TRPSP_B;
			tRPSP_C = TRPSP_C;
			tRPSP_D = TRPSP_D;
			valName = ValName;
		}

		public TRPSPIN(DynamicMemory mem, string ValName="")
		{
			valName = ValName;
			if(mem==null) return;
			if (mem["TRPSP_AXIS"] != null) tRPSP_AXIS = (int)mem["TRPSP_AXIS"];
			if (mem["TRPSP_COP_AX"] != null) tRPSP_COP_AX = (int)mem["TRPSP_COP_AX"];
			if (mem["TRPSP_A"] != null) tRPSP_A = (double)mem["TRPSP_A"];
			if (mem["TRPSP_B"] != null) tRPSP_B = (double)mem["TRPSP_B"];
			if (mem["TRPSP_C"] != null) tRPSP_C = (double)mem["TRPSP_C"];
			if (mem["TRPSP_D"] != null) tRPSP_D = (double)mem["TRPSP_D"];
		}
	#endregion //constructors

	#region methods
		public string ToDefString()
		{
			return "DECL TRPSPIN " + ToString();
		}

		public List<string> ToList()
		{
			return new List<string>() {
				valName,
				tRPSP_AXIS.ToString(CultureInfo.InvariantCulture),
				tRPSP_COP_AX.ToString(CultureInfo.InvariantCulture),
				tRPSP_A.ToString(CultureInfo.InvariantCulture),
				tRPSP_B.ToString(CultureInfo.InvariantCulture),
				tRPSP_C.ToString(CultureInfo.InvariantCulture),
				tRPSP_D.ToString(CultureInfo.InvariantCulture),
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"{{TRPSP_AXIS {0},TRPSP_COP_AX {1},TRPSP_A {2},TRPSP_B {3},TRPSP_C {4},TRPSP_D {5}}}",
				tRPSP_AXIS, tRPSP_COP_AX, tRPSP_A, tRPSP_B, tRPSP_C, tRPSP_D
				);
		}

	#endregion //methods

	}
}
