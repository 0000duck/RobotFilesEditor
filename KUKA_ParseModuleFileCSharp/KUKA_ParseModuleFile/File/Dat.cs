using WarningHelper;
using ParseModuleFile.KUKA.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using ParseModuleFile.KUKA;

namespace ParseModuleFile.File
{
    public class Dat : CFile
    {

        public Variables variables;
        #region Constants - regular expressions patterns
	    private const string _declaration = "^DECL (?:(?<global>GLOBAL )|)(?<type>.\\w+) (?<name>\\$?\\w+)=\\{(?<value>.+)\\}";
	    private const string _declaration_default = "^(?:DECL |)(?:(?<global>GLOBAL )|)(?<type>\\w+) (?<name>\\$?\\w+)(?:\\s?\\[(?<dim>\\d+)(?:\\s?,\\s?(?<dim2>\\d+)|)\\s?\\]|)(?:=(?<value>.+)|)";
        private const string _defdat = @"DEFDAT\s*(?<name>[$\w]*)\s*(?:(?<global>PUBLIC)|)";

        private const string _assigment = "^(?:(?<type>\\w+)\\s|)\\s?(?<name>\\$?\\w+)(?:\\s?\\[(?:(?<dim>\\d+)|)\\s?,?(?:(?<dim2>\\d+)|)\\]|)=(?<value>.+)";
	    private const string _bool = _s + "(true|false)";
	    private const string _int = _s + "(?:[-+]?[0-9]+)";
	    private const string _float = _s + "(?:[-+]?[0-9]*\\.?[0-9]+(?:[eE][-+]?[0-9]+)?)";
        private const string _any = _s + "\\\"(.*?)\\\"";
        #endregion // Constants - regular expressions patterns

        #region Regex declarations
        //private static Regex reDeclaration = new Regex(_declaration, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        //private static Regex reDeclarationDefault = new Regex(_declaration_default, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        //private static Regex reAssigment = new Regex(_assigment, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        //private static Regex reDefDat = new Regex(_defdat, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        #endregion // Regex declarations

        #region fields
        string actDefDat = null;
        bool isGlobal = false;
        #endregion

        #region " Constructors "
        public Dat(string fileName, Stream stream, Warnings warnings)
            : base(fileName, stream, warnings)
        {
        }

	    #endregion

        protected override void ParseStream()
        {
        }

	    private void ParseDATLine(int line_number, string line, string module_name, Dictionary<string, Variable> vars)
	    {
            string trimmed_line = line.Trim();
		    // counting from 0
		    if (line_number == 546 - 1) {
			    int z = 0;
                z++;
		    }
		    Variable var = null;
		    current_line = line_number;
		    if (trimmed_line.Length == 0)
			    return;
            if (RegexHelper.IsMatch(_comment, trimmed_line))
            {
			    //_warnings.Add(current_line, 1, "comment")
			    return;
		    }

            if (RegexHelper.IsMatch(_declaration, trimmed_line)) 
            {
                Match match = RegexHelper.Match(_declaration, trimmed_line);
			    bool _global = match.Groups["global"].Success && isGlobal;
			    string _type = match.Groups["type"].Value;
			    string _name = match.Groups["name"].Value.ToUpperInvariant();
			    string _valueList = match.Groups["value"].Value;
			    if (_type == "STRUC")
				    return;
			    if (_type == "ENUM")
				    return;
                var = new Variable(actDefDat, _global, _type, _name);
                var.Value = GetCorrectMemory(_type, _warnings);
			    AssignMemoryFromData(GetCorrectPattern(_type, _warnings), var.Value, _valueList, _warnings);
			    _warnings.Add(current_line, WarningType.Deep_Intern, Level.Information, "Created declaration of value " + _name);
			    if (!(_global && vars.ContainsKey(_name)))
                {
				    if ((vars.ContainsKey(_name)))
					    System.Diagnostics.Debugger.Break();
				    vars.Add(_name, var);
			    }
			    return;
		    }
            else if (RegexHelper.IsMatch(_declaration_default, trimmed_line)) 
            {
                Match match = RegexHelper.Match(_declaration_default, trimmed_line);
			    bool _global = match.Groups["global"].Success && isGlobal;
			    string _type = match.Groups["type"].Value;
			    string _name = match.Groups["name"].Value.ToUpperInvariant();
			    string _valueList = match.Groups["value"].Value;
			    if (_type == "STRUC")
				    return;
			    if (_type == "ENUM")
				    return;
                if (!(_type == "CHAR" && (!match.Groups["dim2"].Success)) && match.Groups["dim"].Success)
                {
				    int _dim = 1;
				    if (!int.TryParse(match.Groups["dim"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out _dim))
					    _warnings.Add(current_line, WarningType.Deep_Intern, Level.Failure, "Dimension - Could not convert to integer: " + match.Groups["dim"].Value);
				    if (_type != "CHAR" && match.Groups["dim2"].Success) {
					    int _dim2 = 1;
					    if (!int.TryParse(match.Groups["dim2"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out _dim2))
						    _warnings.Add(current_line, WarningType.Deep_Intern, Level.Failure, "Second dimension - Could not convert to integer: " + match.Groups["dim"].Value);
                        var = new Variable(actDefDat,_global, _type, _name, _dim, _dim2);
					    for (int i = 1; i <= _dim; i++) {
						    for (int j = 1; j <= _dim2; j++) {
							    var[i, j] = GetCorrectMemory(_type, _warnings);
							    //AssignMemoryFromData(GetCorrectPattern(_type, _warnings), var[i, j], null, _warnings);
						    }
					    }
				    } else {
                        var = new Variable(actDefDat, _global, _type, _name, _dim);
					    for (int i = 1; i <= _dim; i++) {
						    var[i] = GetCorrectMemory(_type, _warnings);
						    //AssignMemoryFromData(GetCorrectPattern(_type, _warnings), var[i], null, _warnings);
					    }
				    }
				    _warnings.Add(current_line, WarningType.Deep_Intern, Level.Information, "Created declaration of dimensioned value with defualt values " + _name);
			    } else {
                    var = new Variable(actDefDat,_global, _type, _name);
				    var.Value = GetCorrectMemory(_type, _warnings);
				    if (match.Groups["value"].Success) {
					    AssignMemoryFromData(GetCorrectPattern(_type, _warnings), var.Value, _valueList, _warnings);
				    } else {
					    //AssignMemoryFromData(GetCorrectPattern(_type, _warnings), var.Value, null, _warnings);
				    }
				    if (var.Value != null) {
					    _warnings.Add(current_line, WarningType.Deep_Intern, Level.Information, "Created " + _name + " with values:" + var.Value.ToString());
				    } else {
					    _warnings.Add(current_line, WarningType.Deep_Intern, Level.Information, "Created " + _name + " with unknown values");
				    }
			    }
			    vars.Add(_name, var);
			    return;
            }
            else if (RegexHelper.IsMatch(_assigment, trimmed_line))
            {
                Match match = RegexHelper.Match(_assigment, trimmed_line);
			    string _name = match.Groups["name"].Value.ToUpperInvariant();
			    string _type = match.Groups["type"].Value;
			    string _valueList = match.Groups["value"].Value;
			    var = vars[_name];
			    _type = var.Type;
			    if (match.Groups["dim"].Success) {
				    int _dim = 1;
				    if (!int.TryParse(match.Groups["dim"].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out _dim)) {
					    _warnings.Add(current_line, WarningType.Deep_Intern, Level.Failure, "Dimension - Could not convert to integer: " + match.Groups["dim"].Value);
					    _dim = 1;
				    }
				    if (_type != "CHAR" && match.Groups["dim2"].Success) {
					    int _dim2 = 1;
					    if (!int.TryParse(match.Groups["dim2"].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out _dim2)) {
						    _warnings.Add(current_line, WarningType.Deep_Intern, Level.Failure, "Second dimension - Could not convert to integer: " + match.Groups["dim"].Value);
						    _dim2 = 1;
					    }
					    var[_dim, _dim2] = GetCorrectMemory(_type, _warnings);
					    AssignMemoryFromData(GetCorrectPattern(_type, _warnings), var[_dim, _dim2], _valueList, _warnings);
					    if (var[_dim, _dim2] == null) {
						    _warnings.Add(current_line, WarningType.Deep_Intern, Level.Information, "Assigned " + _name + "[" + _dim + "," + _dim2 + "] with unknown data");
					    } else {
						    _warnings.Add(current_line, WarningType.Deep_Intern, Level.Information, "Assigned " + _name + "[" + _dim + "," + _dim2 + "] with values:" + var[_dim, _dim2].ToString());
					    }
				    } else {
					    var[_dim] = GetCorrectMemory(_type, _warnings);
					    AssignMemoryFromData(GetCorrectPattern(_type, _warnings), var[_dim], _valueList, _warnings);
					    if (var[_dim] == null) {
						    _warnings.Add(current_line, WarningType.Deep_Intern, Level.Information, "Assigned " + _name + "[" + _dim + "] with unknown data");
					    } else {
						    _warnings.Add(current_line, WarningType.Deep_Intern, Level.Information, "Assigned " + _name + "[" + _dim + "] with values:" + var[_dim].ToString());
					    }
				    }
			    } else {
				    var.Value = GetCorrectMemory(_type, _warnings);
				    AssignMemoryFromData(GetCorrectPattern(_type, _warnings), var.Value, trimmed_line, _warnings);
				    if (var.Value == null) {
					    _warnings.Add(current_line, WarningType.Deep_Intern, Level.Information, "Assigned " + _name + " with unknown data");
				    } else {
					    _warnings.Add(current_line, WarningType.Deep_Intern, Level.Information, "Assigned " + _name + " with values:" + var.Value.ToString());
				    }
			    }
			    return;
		    }
            else if (RegexHelper.IsMatch(_defdat, trimmed_line))
            {
                Match match = RegexHelper.Match(_defdat, trimmed_line);
                isGlobal = match.Groups["global"].Success;
                actDefDat = match.Groups["name"].Value;
            }
		    _warnings.Add(current_line, WarningType.Deep_Intern, Level.Failure, "Unknown data: " + line.Trim());
	    }

	    #region " Creating Patterns (shared functions) "
	    #region " Pattern generators "
	    private static string enumpattern(string[] list)
	    {
		    //(?:(?<X>#X)|(?<Y>#Y))
		    string ret = "(?:";
		    bool first = true;
		    foreach (string item in list) {
			    if (first) {
				    first = false;
			    } else {
				    ret += "|";
			    }
                ret += string.Concat( "(?<", item, ">#", item, ")");
                //ret += "(?<" + item + ">#" + item + ")";
		    }
		    return ret + ")";
	    }

	    private static string pattern(string name, string type)
	    {
            return string.Concat(_s, name, _s, "(", type, ")");
		    //return _s + name + _s + "(" + type + ")";
	    }

	    private static string simplepattern(string text)
	    {
            return string.Concat("(", text, ")");
            //return "(" + text + ")";
	    }

	    private static string subpattern(string name, string type, string prefix)
	    {
            return string.Concat(_s, name, _s, "(?<", prefix, name, ">", type, ")");
		    //return _s + name + _s + "(?<" + prefix + name + ">" + type + ")";
	    }
	    #endregion
	    #region " CMemory Type Creators "
	    private static DynamicMemory CreateSimpleType(System.Type type)
	    {
            DynamicMemory ret = new DynamicMemory(new string[] { "val" }, new Type[] { type });
		    return ret;
	    }

	    private static DynamicMemory CreateTypeMachineDefT()
	    {
		    string str = "";
		    int @int = 0;
		    MECH_TYPE mt = default(MECH_TYPE);
            return new DynamicMemory(new string[] {
			    "NAME",
			    "COOP_KRC_INDEX",
			    "PARENT",
			    "ROOT",
			    "MECH_TYPE",
			    "GEOMETRY"
		    }, new Object[] {
			    str,
			    @int,
			    str,
			    CreateTypeFrame(),
			    mt,
			    str
		    });
	    }

	    private static DynamicMemory CreateTypeGripStruc()
	    {
            return new DynamicMemory(new string[] {
			    "Num",
			    "Name",
			    "PP1",
			    "PP2",
			    "PP3",
			    "PP4",
			    "PP5",
			    "PP6",
			    "PP7",
			    "PP8",
			    "PP9",
			    "PP10",
			    "PP11",
			    "PP12",
			    "PP13",
			    "PP14",
			    "PP15",
			    "PP16",
			    "I_PrSwUsed",
			    "I_PrSw",
			    "O_SafeValve",
			    "IsTimeOut",
			    "IsConfig"
		    }, new Type[] {
			    typeof(int),
			    typeof(string),
			    typeof(int),
			    typeof(int),
			    typeof(int),
			    typeof(int),
			    typeof(int),
			    typeof(int),
			    typeof(int),
			    typeof(int),
			    typeof(int),
			    typeof(int),
			    typeof(int),
			    typeof(int),
			    typeof(int),
			    typeof(int),
			    typeof(int),
			    typeof(int),
			    typeof(bool),
			    typeof(int),
			    typeof(int),
			    typeof(bool),
			    typeof(bool)
		    });
	    }

	    private static DynamicMemory CreateTypeGrpActuator()
	    {
            return new DynamicMemory(new string[]{
			    "Num",
			    "Name",
			    "Type",
			    "IsUsed",
			    "Check",
			    "C1Used",
			    "C2Used",
			    "C3Used",
			    "C4Used",
			    "I_C1Retracted",
			    "I_C1Advanced",
			    "I_C2Retracted",
			    "I_C2Advanced",
			    "I_C3Retracted",
			    "I_C3Advanced",
			    "I_C4Retracted",
			    "I_C4Advanced",
			    "O_Retracted",
			    "O_Advanced",
			    "T_ErrWait",
			    "T_Retracted",
			    "T_Advanced",
			    "I_VAChnA",
			    "T_Ret_Pulse",
			    "TOutHandle",
			    "SetState"
		    }, new Type[] {
			    typeof(int),
			    typeof(string),
			    typeof(GrpACT_Type),
			    typeof(bool),
			    typeof(bool),
			    typeof(bool),
			    typeof(bool),
			    typeof(bool),
			    typeof(bool),
			    typeof(int),
			    typeof(int),
			    typeof(int),
			    typeof(int),
			    typeof(int),
			    typeof(int),
			    typeof(int),
			    typeof(int),
			    typeof(int),
			    typeof(int),
			    typeof(int),
			    typeof(double),
			    typeof(double),
			    typeof(int),
			    typeof(double),
			    typeof(int),
			    typeof(Grp_STATE)
		    });
	    }

	    private static DynamicMemory CreateType_calEqPressure()
	    {
		    return new DynamicMemory(new string[]{
			    "min",
			    "max",
			    "stat"
		    }, new Type[] {
			    typeof(double),
			    typeof(double),
			    typeof(double)
		    });
	    }

	    private static DynamicMemory CreateTypeDate()
	    {
		    return new DynamicMemory(new string[]{
			    "CSEC",
			    "SEC",
			    "MIN",
			    "HOUR",
			    "DAY",
			    "MONTH",
			    "YEAR"
		    }, new Type[]{
			    typeof(int),
			    typeof(int),
			    typeof(int),
			    typeof(int),
			    typeof(int),
			    typeof(int),
			    typeof(int)
		    });
	    }

	    private static DynamicMemory CreateTypeTipDress()
	    {
		    return new DynamicMemory(new string[]{
			    "DressTime",
			    "DressForce",
			    "Interval"
		    }, new Type[]{
			    typeof(double),
			    typeof(int),
			    typeof(int)
		    });
	    }

	    private static DynamicMemory CreateTypeSpsProgType()
	    {
		    return new DynamicMemory(new string[]{
			    "PROG_NR",
			    "PROG_NAME"
		    }, new Type[]{
			    typeof(int),
			    typeof(string)
		    });
	    }

	    private static DynamicMemory CreateTypeMachineToolT()
	    {
		    return new DynamicMemory(new string[]{
			    "MACH_DEF_INDEX",
			    "PARENT",
			    "GEOMETRY"
		    }, new Type[]{
			    typeof(int),
			    typeof(string),
			    typeof(string)
		    });
	    }

	    private static DynamicMemory CreateTypeLoad()
	    {
		    return new DynamicMemory(new string[]{
			    "M",
			    "CM",
			    "J"
		    }, new Object[]{
			    double.NaN,
			    CreateTypeFrame(),
			    CreateTypeInertia()
		    });
	    }

	    private static DynamicMemory CreateTypeFDat()
	    {
		    return new DynamicMemory(new string[]{
			    "TOOL_NO",
			    "BASE_NO",
			    "IPO_FRAME",
			    "POINT2",
			    "TQ_STATE"
		    }, new Type[]{
			    typeof(int),
			    typeof(int),
			    typeof(IPO_M_T),
			    typeof(string),
			    typeof(bool)
		    });
	    }

	    private static DynamicMemory CreateTypePDat()
	    {
		    return new DynamicMemory(new string[]{
			    "VEL",
			    "ACC",
			    "APO_DIST",
			    "APO_MODE",
			    "GEAR_JERK"
		    }, new Type[]{
			    typeof(double),
			    typeof(double),
			    typeof(double),
			    typeof(APO_MODE_T),
			    typeof(int)
		    });
	    }

	    private static DynamicMemory CreateTypCBin()
	    {
		    return new DynamicMemory(new string[]{
			    "ORI",
			    "E1",
			    "E2",
			    "E3",
			    "E4",
			    "E5",
			    "E6"
		    }, new Type[]{
			    typeof(CIRC_MODE_args),
			    typeof(CIRC_MODE_args),
			    typeof(CIRC_MODE_args),
			    typeof(CIRC_MODE_args),
			    typeof(CIRC_MODE_args),
			    typeof(CIRC_MODE_args),
			    typeof(CIRC_MODE_args)
		    });
	    }

	    private static DynamicMemory CreateTypCircMode()
	    {
		    return new DynamicMemory(new string[]{
			    "AUX_PT",
			    "TARGET_PT"
		    }, new Object[]{
			    CreateTypCBin(),
			    CreateTypCBin()
		    });
	    }

	    private static DynamicMemory CreateTypeLDat()
	    {
		    ORI_TYPE ori_typ = default(ORI_TYPE);
		    CIRC_TYPE circ_typ = default(CIRC_TYPE);
		    int @int = 0;
		    return new DynamicMemory(new string[]{
			    "VEL",
			    "ACC",
			    "APO_DIST",
			    "APO_FAC",
			    "AXIS_VEL",
			    "AXIS_ACC",
			    "ORI_TYP",
			    "CIRC_TYP",
			    "JERK_FAC",
			    "GEAR_JERK",
			    "EXAX_IGN",
			    "CB"
		    }, new Object[]{
			    double.NaN,
			    double.NaN,
			    double.NaN,
			    double.NaN,
			    double.NaN,
			    double.NaN,
			    ori_typ,
			    circ_typ,
			    double.NaN,
			    double.NaN,
			    @int,
			    CreateTypCircMode()
		    });
	    }

	    private static DynamicMemory CreateTypeAxis()
	    {
		    return new DynamicMemory(new string[]{
			    "A1",
			    "A2",
			    "A3",
			    "A4",
			    "A5",
			    "A6"
		    }, new Object[]{
			    double.NaN,
			    double.NaN,
			    double.NaN,
			    double.NaN,
			    double.NaN,
			    double.NaN
		    });
	    }

	    private static DynamicMemory CreateTypeE6Axis()
	    {
		    return new DynamicMemory(new string[]{
			    "A1",
			    "A2",
			    "A3",
			    "A4",
			    "A5",
			    "A6",
			    "E1",
			    "E2",
			    "E3",
			    "E4",
			    "E5",
			    "E6"
		    }, new Object[]{
			    double.NaN,
			    double.NaN,
			    double.NaN,
			    double.NaN,
			    double.NaN,
			    double.NaN,
			    double.NaN,
			    double.NaN,
			    double.NaN,
			    double.NaN,
			    double.NaN,
			    double.NaN
		    });
	    }

	    private static DynamicMemory CreateTypeInertia()
	    {
		    return new DynamicMemory(new string[]{
			    "X",
			    "Y",
			    "Z"
		    }, new Object[]{
			    double.NaN,
			    double.NaN,
			    double.NaN
		    });
	    }

	    private static DynamicMemory CreateTypeFrame()
	    {
		    return new DynamicMemory(new string[]{
			    "X",
			    "Y",
			    "Z",
			    "A",
			    "B",
			    "C"
		    }, new Object[]{
			    double.NaN,
			    double.NaN,
			    double.NaN,
			    double.NaN,
			    double.NaN,
			    double.NaN
		    });
	    }

	    private static DynamicMemory CreateTypePos()
	    {
		    int @int = -1;
		    return new DynamicMemory(new string[]{
			    "X",
			    "Y",
			    "Z",
			    "A",
			    "B",
			    "C",
			    "S",
			    "T"
		    }, new Object[]{
			    double.NaN,
			    double.NaN,
			    double.NaN,
			    double.NaN,
			    double.NaN,
			    double.NaN,
			    @int,
			    @int
		    });
	    }

	    private static DynamicMemory CreateTypeE6Pos()
	    {
		    int @int = -1;
		    return new DynamicMemory(new string[]{
			    "X",
			    "Y",
			    "Z",
			    "A",
			    "B",
			    "C",
			    "S",
			    "T",
			    "E1",
			    "E2",
			    "E3",
			    "E4",
			    "E5",
			    "E6"
		    }, new Object[]{
			    double.NaN,
			    double.NaN,
			    double.NaN,
			    double.NaN,
			    double.NaN,
			    double.NaN,
			    @int,
			    @int,
			    double.NaN,
			    double.NaN,
			    double.NaN,
			    double.NaN,
			    double.NaN,
			    double.NaN
		    });
	    }

	    private static DynamicMemory CreateTypeFRA()
	    {
		    return new DynamicMemory(new string[]{
			    "N",
			    "D"
		    }, new Type[]{
			    typeof(int),
			    typeof(int)
		    });
	    }
	    #endregion

	    #region " Converters "
        private static double ToDouble(string text, string key, Warnings _warnings)
	    {
		    double dbl = 0;
		    if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out dbl))
			    _warnings.Add(current_line, WarningType.Deep_Intern, Level.Failure, "Could not convert " + key + " to double: " + text);
		    return dbl;
	    }

        private static int ToInteger(string text, string key, Warnings _warnings)
	    {
		    int @int = 0;
		    if (!int.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out @int))
			    _warnings.Add(current_line, WarningType.Deep_Intern, Level.Failure, "Could not convert " + key + " to integer: " + text);
		    return @int;
	    }

        private static bool ToBoolean(string text, string key, Warnings _warnings)
	    {
		    bool bo = false;
		    if (text.ToLower() == "false") {
			    bo = false;
		    } else if (text.ToLower() == "true") {
			    bo = true;
		    } else {
			    _warnings.Add(current_line, WarningType.Deep_Intern, Level.Failure, "Could not convert " + key + " to boolean: " + text);
		    }
		    return bo;
	    }

        private static APO_MODE_T ToAPO_MODE_T(Match match, string key, Warnings _warnings)
	    {
		    APO_MODE_T ip = default(APO_MODE_T);
		    if (match.Groups["CPTP"].Success) {
			    ip = APO_MODE_T.CPTP;
		    } else if (match.Groups["CDIS"].Success) {
			    ip = APO_MODE_T.CDIS;
		    } else {
			    _warnings.Add(current_line, WarningType.Deep_Intern, Level.Failure, "Could not convert " + key + " to enum APO_MODE_T: " + match.Groups[1].Value);
		    }
		    return ip;
	    }

        private static IPO_M_T ToIPO_M_T(Match match, string key, Warnings _warnings)
	    {
		    IPO_M_T ip = default(IPO_M_T);
		    if (match.Groups["NONE"].Success) {
			    ip = IPO_M_T.NONE;
		    } else if (match.Groups["BASE"].Success) {
			    ip = IPO_M_T.BASE;
		    } else if (match.Groups["TCP"].Success) {
			    ip = IPO_M_T.TCP;
		    } else {
			    _warnings.Add(current_line, WarningType.Deep_Intern, Level.Failure, "Could not convert " + key + " to enum IPO_M_T: " + match.Groups[1].Value);
		    }
		    return ip;
	    }

        private static ORI_TYPE ToORI_TYP(Match match, string key, Warnings _warnings)
	    {
            ORI_TYPE ip = default(ORI_TYPE);
		    if (match.Groups["VAR"].Success) {
                ip = ORI_TYPE.VAR;
		    } else if (match.Groups["CONSTANT"].Success) {
                ip = ORI_TYPE.CONSTANT;
		    } else if (match.Groups["JOINT"].Success) {
                ip = ORI_TYPE.JOINT;
		    } else {
			    _warnings.Add(current_line, WarningType.Deep_Intern, Level.Failure, "Could not convert " + key + " to enum ORI_TYP: " + match.Groups[1].Value);
		    }
		    return ip;
	    }

        private static CIRC_TYPE ToCIRC_TYP(Match match, string key, Warnings _warnings)
	    {
            CIRC_TYPE ip = default(CIRC_TYPE);
		    if (match.Groups["BASE"].Success) {
                ip = CIRC_TYPE.BASE;
		    } else if (match.Groups["PATH"].Success) {
                ip = CIRC_TYPE.PATH;
		    } else {
			    _warnings.Add(current_line, WarningType.Deep_Intern, Level.Failure, "Could not convert " + key + " to enum CIRC_TYP: " + match.Groups[1].Value);
		    }
		    return ip;
	    }

        private static CIRC_MODE_args ToCB_In(string text, string key, Warnings _warnings)
	    {
            string str = text.ToUpperInvariant();
            CIRC_MODE_args bo = default(CIRC_MODE_args);
		    if (str == "#CONSIDER") {
			    bo = CIRC_MODE_args.CONSIDER;
		    } else if (str == "#INTERPOLATE") {
			    bo = CIRC_MODE_args.INTERPOLATE;
		    } else {
			    _warnings.Add(current_line, WarningType.Deep_Intern, Level.Failure, "Could not convert " + key + " to CB_In: " + text);
		    }
		    return bo;
	    }

        private static MECH_TYPE ToMECH_TYPE(string text, string key, Warnings _warnings)
	    {
            string str = text.ToUpperInvariant();
            MECH_TYPE bo = default(MECH_TYPE);
		    if (str == "#NONE") {
			    bo = MECH_TYPE.NONE;
		    } else if (str == "#ROBOT") {
			    bo = MECH_TYPE.ROBOT;
		    } else {
			    _warnings.Add(current_line, WarningType.Deep_Intern, Level.Failure, "Could not convert " + key + " to MECH_TYPE: " + text);
		    }
		    return bo;
	    }

        private static GrpACT_Type ToGrpACT_Type(string text, string key, Warnings _warnings)
	    {
            string str = text.ToUpperInvariant();
		    GrpACT_Type bo = default(GrpACT_Type);
		    if (str == "#STATICALLY") {
			    bo = GrpACT_Type.STATICALLY;
		    } else if (str == "#IMPULSE") {
			    bo = GrpACT_Type.IMPULSE;
		    } else if (str == "#VACUUM") {
			    bo = GrpACT_Type.VACUUM;
		    } else if (str == "#SPECIAL") {
			    bo = GrpACT_Type.SPECIAL;
		    } else {
			    _warnings.Add(current_line, WarningType.Deep_Intern, Level.Failure, "Could not convert " + key + " to GrpACT_Type: " + text);
		    }
		    return bo;
	    }

        private static Grp_STATE ToGrp_STATE(string text, string key, Warnings _warnings)
	    {
            string str = text.ToUpperInvariant();
            Grp_STATE bo = default(Grp_STATE);
		    if (str == "#UNUSED") {
			    bo = Grp_STATE.UNUSED;
		    } else if (str == "#SETRETRACTED") {
			    bo = Grp_STATE.SETRetracted;
		    } else if (str == "#SETADVANCED") {
			    bo = Grp_STATE.SETAdvanced;
		    } else {
			    _warnings.Add(current_line, WarningType.Deep_Intern, Level.Failure, "Could not convert " + key + " to Grp_STATE: " + text);
		    }
		    return bo;
	    }

        private static swp_GUN_MOUNTING Toswp_GUN_MOUNTING(Match match, string key, Warnings _warnings)
	    {
		    swp_GUN_MOUNTING bo = default(swp_GUN_MOUNTING);
		    if (match.Groups["ROBOT"].Success) {
			    bo = swp_GUN_MOUNTING.ROBOT;
		    } else if (match.Groups["PEDESTAL"].Success) {
			    bo = swp_GUN_MOUNTING.PEDESTAL;
		    } else {
			    _warnings.Add(current_line, WarningType.Deep_Intern, Level.Failure, "Could not convert " + key + " to swp_GUN_MOUNTING: " + match.Groups[1].Value);
		    }
		    return bo;
	    }

        private static OPTION_CTL ToOPTION_CTL(Match match, string key, Warnings _warnings)
	    {
            OPTION_CTL bo = default(OPTION_CTL);
		    if (match.Groups["Enabled"].Success) {
                bo = OPTION_CTL.ENABLED;
		    } else if (match.Groups["Disabled"].Success) {
                bo = OPTION_CTL.DISABLED;
		    } else {
			    _warnings.Add(current_line, WarningType.Deep_Intern, Level.Failure, "Could not convert " + key + " to swp_OPTION_CTL: " + match.Groups[1].Value);
		    }
		    return bo;
	    }
	    #endregion

        #region static "GetCorrectPattern" data

        private static Dictionary<string, string> dateGetCorrectPattern = new Dictionary<string, string>()
        {
            {"CSEC",pattern("CSEC", _int)},
		    {"SEC", pattern("SEC", _int)},
		    {"MIN", pattern("MIN", _int)},
		    {"HOUR", pattern("HOUR", _int)},
		    {"DAY", pattern("DAY", _int)},
		    {"MONTH", pattern("MONTH", _int)},
		    {"YEAR", pattern("YEAR", _int)},
        };

        private static Dictionary<string, string> _caleqpressureGetCorrectPattern = new Dictionary<string, string>()
        {
		    {"min", pattern("min", _float)},
            {"max", pattern("max", _float)},
            {"stat", pattern("stat", _float)},
        };

        private static Dictionary<string, string> swp_option_ctlGetCorrectPattern = new Dictionary<string, string>()
        {
            {"val", enumpattern(new string[] {"Enabled","Disabled"})},
        };

        private static Dictionary<string, string> tipdressGetCorrectPattern = new Dictionary<string, string>()
        {
		    {"DressTime", pattern("DressTime", _float)},
		    {"DressForce", pattern("DressForce", _int)},
		    {"Interval", pattern("Interval", _int)},
        };

        private static Dictionary<string, string> swp_gun_mountingGetCorrectPattern = new Dictionary<string, string>()
        {
            {"val", enumpattern(new string[] {"ROBOT", "PEDESTAL"})},
        };

        private static Dictionary<string, string> gripstrucGetCorrectPattern = new Dictionary<string, string>()
        {
		    {"Num", pattern("Num", _int)},
		    {"Name", "Name\\s*\\[\\s*\\]" + _any},
		    {"PP1", pattern("PP1", _int)},
		    {"PP2", pattern("PP2", _int)},
		    {"PP3", pattern("PP3", _int)},
		    {"PP4", pattern("PP4", _int)},
		    {"PP5", pattern("PP5", _int)},
		    {"PP6", pattern("PP6", _int)},
		    {"PP7", pattern("PP7", _int)},
		    {"PP8", pattern("PP8", _int)},
		    {"PP9", pattern("PP9", _int)},
		    {"PP10", pattern("PP10", _int)},
		    {"PP11", pattern("PP11", _int)},
		    {"PP12", pattern("PP12", _int)},
		    {"PP13", pattern("PP13", _int)},
		    {"PP14", pattern("PP14", _int)},
		    {"PP15", pattern("PP15", _int)},
		    {"PP16", pattern("PP16", _int)},
		    {"I_PrSwUsed", pattern("I_PrSwUsed", _bool)},
		    {"I_PrSw", pattern("I_PrSw", _int)},
		    {"O_SafeValve", pattern("O_SafeValve", _int)},
		    {"IsTimeOut", pattern("IsTimeOut", _bool)},
		    {"IsConfig", pattern("IsConfig", _bool)},
        };

        private static Dictionary<string, string> grpactuatorGetCorrectPattern = new Dictionary<string, string>()
        {
		    {"Num", pattern("Num", _int)},
		    {"Name", "Name\\s*\\[\\s*\\]" + _any},
		    {"Type", pattern("Type", enumpattern(new string[] {"STATICALLY", "IMPULSE", "VACUUM", "SPECIAL"}))},
		    {"IsUsed", pattern("IsUsed", _bool)},
		    {"Check", pattern("Check", _bool)},
		    {"C1Used", pattern("C1Used", _bool)},
		    {"C2Used", pattern("C2Used", _bool)},
		    {"C3Used", pattern("C3Used", _bool)},
		    {"C4Used", pattern("C4Used", _bool)},
		    {"I_C1Retracted", pattern("I_C1Retracted", _int)},
		    {"I_C1Advanced", pattern("I_C1Advanced", _int)},
		    {"I_C2Retracted", pattern("I_C2Retracted", _int)},
		    {"I_C2Advanced", pattern("I_C2Advanced", _int)},
		    {"I_C3Retracted", pattern("I_C3Retracted", _int)},
		    {"I_C3Advanced", pattern("I_C3Advanced", _int)},
		    {"I_C4Retracted", pattern("I_C4Retracted", _int)},
		    {"I_C4Advanced", pattern("I_C4Advanced", _int)},
		    {"O_Retracted", pattern("O_Retracted", _int)},
		    {"O_Advanced", pattern("O_Advanced", _int)},
		    {"T_ErrWait", pattern("T_ErrWait", _int)},
		    {"T_Retracted", pattern("T_Retracted", _float)},
		    {"T_Advanced", pattern("T_Advanced", _float)},
		    {"I_VAChnA", pattern("I_VAChnA", _int)},
		    {"T_Ret_Pulse", pattern("T_Ret_Pulse", _float)},
		    {"TOutHandle", pattern("TOutHandle", _int)},
		    {"SetState", pattern("SetState", enumpattern(new string[] {"UNUSED", "SETRetracted", "SETAdvanced"}))},
        };

        private static Dictionary<string, string> sps_prog_typeGetCorrectPattern = new Dictionary<string, string>()
        {
		    {"PROG_NR", pattern("PROG_NR", _int)},
            {"PROG_NAME", pattern("PROG_NAME", _any)},
        };

        private static Dictionary<string, string> machine_def_tGetCorrectPattern = new Dictionary<string, string>()
        {
		    {"NAME", pattern("NAME", _any)},
		    {"COOP_KRC_INDEX", pattern("COOP_KRC_INDEX", _int)},
		    {"PARENT", pattern("PARENT", _any)},
		    {"ROOT", "ROOT" + _s + "{" + subpattern("X", _float, "ROOT_") + _s + "," + _s + subpattern("Y", _float, "ROOT_") + _s + "," + _s + subpattern("Z", _float, "ROOT_") + _s + "," + _s + subpattern("A", _float, "ROOT_") + _s + "," + _s + subpattern("B", _float, "ROOT_") + _s + "," + _s + subpattern("C", _float, "ROOT_") + _s + "}"},
		    {"MECH_TYPE", pattern("MECH_TYPE", enumpattern(new string[] {"NONE", "ROBOT"}))},
		    {"GEOMETRY", pattern("GEOMETRY", _any)},
        };

        private static Dictionary<string, string> fraGetCorrectPattern = new Dictionary<string, string>()
        {
		    {"N", pattern("N", _int)},
		    {"D", pattern("D", _int)},
        };

        private static Dictionary<string, string> machine_frame_tGetCorrectPattern = new Dictionary<string, string>()
        {
		    {"MACH_DEF_INDEX", pattern("MACH_DEF_INDEX", _int)},
		    {"PARENT", pattern("PARENT", _any)},
		    {"GEOMETRY", pattern("GEOMETRY", _any)},
        };

        private static string enumpatternConsiderInterpolateString = enumpattern(new string[] { "CONSIDER", "INTERPOLATE" });

        private static Dictionary<string, string> ldatGetCorrectPattern = new Dictionary<string, string>()
        {
		    {"VEL", pattern("VEL", _float)},
		    {"ACC", pattern("ACC", _float)},
		    {"APO_DIST", pattern("APO_DIST", _float)},
		    {"APO_FAC", pattern("APO_FAC", _float)},
		    {"AXIS_VEL", pattern("AXIS_VEL", _float)},
		    {"AXIS_ACC", pattern("AXIS_ACC", _float)},
		    {"ORI_TYP", pattern("ORI_TYP", enumpattern(new string[] {"VAR", "CONSTANT", "JOINT"}))},
		    {"CIRC_TYP", pattern("CIRC_TYP", enumpattern(new string[] {"BASE", "PATH"}))},
		    {"JERK_FAC", pattern("JERK_FAC", _float)},
		    {"GEAR_JERK", pattern("GEAR_JERK", _float)},
		    {"EXAX_IGN", pattern("EXAX_IGN", _int)},
		    {"CB", 
                "CB" + _s + "{" + _s + "(?<CB_AUX_PT>AUX_PT)" + _s + 
                "{" + subpattern("ORI", enumpatternConsiderInterpolateString, "CB_AUX_PT_") + _s + "," + 
                _s + subpattern("E1", enumpatternConsiderInterpolateString, "CB_AUX_PT_") + _s + "," + 
                _s + subpattern("E2", enumpatternConsiderInterpolateString, "CB_AUX_PT_") + _s + "," + 
                _s + subpattern("E3", enumpatternConsiderInterpolateString, "CB_AUX_PT_") + _s + "," + 
                _s + subpattern("E4", enumpatternConsiderInterpolateString, "CB_AUX_PT_") + _s + "," + 
                _s + subpattern("E5", enumpatternConsiderInterpolateString, "CB_AUX_PT_") + _s + "," + 
                _s + subpattern("E6", enumpatternConsiderInterpolateString, "CB_AUX_PT_") + _s + "}" + _s + "," + 
                "(?<CB_TARGET_PT>TARGET_PT)" + _s + 
                "{" + subpattern("ORI", enumpatternConsiderInterpolateString, "CB_TARGET_PT_") + _s + "," + 
                _s + subpattern("E1", enumpatternConsiderInterpolateString, "CB_TARGET_PT_") + _s + "," + 
                _s + subpattern("E2", enumpatternConsiderInterpolateString, "CB_TARGET_PT_") + _s + "," + 
                _s + subpattern("E3", enumpatternConsiderInterpolateString, "CB_TARGET_PT_") + _s + "," + 
                _s + subpattern("E4", enumpatternConsiderInterpolateString, "CB_TARGET_PT_") + _s + "," + 
                _s + subpattern("E5", enumpatternConsiderInterpolateString, "CB_TARGET_PT_") + _s + "," + 
                _s + subpattern("E6", enumpatternConsiderInterpolateString, "CB_TARGET_PT_") + _s + "}" + _s + "}"},
        };

        private static Dictionary<string, string> pdatGetCorrectPattern = new Dictionary<string, string>()
        {
		    {"VEL", pattern("VEL", _int)},
		    {"ACC", pattern("ACC", _int)},
		    {"APO_DIST", pattern("APO_DIST", _int)},
		    {"APO_MODE", pattern("APO_MODE", enumpattern(new string[] {"CPTP", "CDIS"}))},
		    {"GEAR_JERK", pattern("GEAR_JERK", _int)},
        };

        private static Dictionary<string, string> fdatGetCorrectPattern = new Dictionary<string, string>()
        {
		    {"TOOL_NO", pattern("TOOL_NO", _int)},
		    {"BASE_NO", pattern("BASE_NO", _int)},
		    {"IPO_FRAME", pattern("IPO_FRAME", enumpattern(new string[] {"NONE", "TCP", "BASE"}))},
		    {"POINT2", pattern("POINT2\\s*\\[\\s*\\]", _any)},
		    {"TQ_STATE", pattern("TQ_STATE", _bool)},
        };

        private static Dictionary<string, string> realGetCorrectPattern = new Dictionary<string, string>() { { "val", simplepattern(_float) } };
        private static Dictionary<string, string> boolGetCorrectPattern = new Dictionary<string, string>() { { "val", simplepattern(_bool) } };
        private static Dictionary<string, string> intGetCorrectPattern = new Dictionary<string, string>() { { "val", simplepattern(_int) } };
        private static Dictionary<string, string> ipo_m_tGetCorrectPattern = new Dictionary<string, string>() { { "val", enumpattern(new string[] { "NONE", "TCP", "BASE" }) } };

        private static Dictionary<string, string> loadGetCorrectPattern = new Dictionary<string, string>()
        {
		    {"M", pattern("M", _float)},
		    {"CM", "CM" + _s + "{" + subpattern("X", _float, "CM_") + _s + "," + _s + subpattern("Y", _float, "CM_") + _s + "," + _s + subpattern("Z", _float, "CM_") + _s + "," + _s + subpattern("A", _float, "CM_") + _s + "," + _s + subpattern("B", _float, "CM_") + _s + "," + _s + subpattern("C", _float, "CM_") + _s + "}"},
		    {"J", "J" + _s + "{" + subpattern("X", _float, "J_") + _s + "," + _s + subpattern("Y", _float, "J_") + _s + "," + _s + subpattern("Z", _float, "J_") + _s + "}"},
        };

        private static Dictionary<string, string> charGetCorrectPattern = new Dictionary<string, string>() { { "val", "(" + _any + ")" } };

        private static Dictionary<string, string> axisGetCorrectPattern = new Dictionary<string, string>()
        {
		    {"A1", pattern("A1", _float)},
		    {"A2", pattern("A2", _float)},
		    {"A3", pattern("A3", _float)},
		    {"A4", pattern("A4", _float)},
		    {"A5", pattern("A5", _float)},
		    {"A6", pattern("A6", _float)},
        };

        private static Dictionary<string, string> e6axisGetCorrectPattern = new Dictionary<string, string>()
        {
		    {"A1", pattern("A1", _float)},
		    {"A2", pattern("A2", _float)},
		    {"A3", pattern("A3", _float)},
		    {"A4", pattern("A4", _float)},
		    {"A5", pattern("A5", _float)},
		    {"A6", pattern("A6", _float)},
		    {"E1", pattern("E1", _float)},
		    {"E2", pattern("E2", _float)},
		    {"E3", pattern("E3", _float)},
		    {"E4", pattern("E4", _float)},
		    {"E5", pattern("E5", _float)},
		    {"E6", pattern("E6", _float)},
        };

        private static Dictionary<string, string> frameGetCorrectPattern = new Dictionary<string, string>()
        {
		    {"X", pattern("X", _float)},
		    {"Y", pattern("Y", _float)},
		    {"Z", pattern("Z", _float)},
		    {"A", pattern("A", _float)},
		    {"B", pattern("B", _float)},
		    {"C", pattern("C", _float)},
        };

        private static Dictionary<string, string> posGetCorrectPattern = new Dictionary<string, string>()
        {
		    {"X", pattern("X", _float)},
		    {"Y", pattern("Y", _float)},
		    {"Z", pattern("Z", _float)},
		    {"A", pattern("A", _float)},
		    {"B", pattern("B", _float)},
		    {"C", pattern("C", _float)},
		    {"S", pattern("S", _int)},
		    {"T", pattern("T", _int)},
        };

        private static Dictionary<string, string> e6posGetCorrectPattern = new Dictionary<string, string>()
        {
		    {"X", pattern("X", _float)},
		    {"Y", pattern("Y", _float)},
		    {"Z", pattern("Z", _float)},
		    {"A", pattern("A", _float)},
		    {"B", pattern("B", _float)},
		    {"C", pattern("C", _float)},
		    {"S", pattern("S", _int)},
		    {"T", pattern("T", _int)},
		    {"E1", pattern("E1", _float)},
		    {"E2", pattern("E2", _float)},
		    {"E3", pattern("E3", _float)},
		    {"E4", pattern("E4", _float)},
		    {"E5", pattern("E5", _float)},
		    {"E6", pattern("E6", _float)},
        };

        #endregion // static "GetcCorrectPattern" data

        public static Dictionary<string, string> GetCorrectPattern(string type, Warnings _warnings)
	    {
            //Dictionary<string, string> functionReturnValue = new Dictionary<string, string>();
		    switch (type.ToLower()) {
			    case "date": return dateGetCorrectPattern;
			    case "_caleqpressure": return _caleqpressureGetCorrectPattern;
			    case "swp_option_ctl": return swp_option_ctlGetCorrectPattern;
			    case "tipdress": return tipdressGetCorrectPattern;
			    case "swp_gun_mounting": return swp_gun_mountingGetCorrectPattern;
			    case "gripstruc": return gripstrucGetCorrectPattern;
			    case "grpactuator": return grpactuatorGetCorrectPattern;
			    case "sps_prog_type": return sps_prog_typeGetCorrectPattern;
			    case "machine_def_t": return machine_def_tGetCorrectPattern;
			    case "fra": return fraGetCorrectPattern;
			    case "machine_frame_t": return machine_frame_tGetCorrectPattern;
			    case "machine_tool_t": return machine_frame_tGetCorrectPattern;
			    case "ldat": return ldatGetCorrectPattern;
                case "pdat": return pdatGetCorrectPattern;
			    case "fdat": return fdatGetCorrectPattern;
			    case "real": return realGetCorrectPattern;
			    case "bool": return boolGetCorrectPattern;
                case "int": return intGetCorrectPattern;
                case "ipo_m_t": return ipo_m_tGetCorrectPattern;
			    case "load": return loadGetCorrectPattern;
                case "char": return charGetCorrectPattern;
                case "axis": return axisGetCorrectPattern;
                case "e6axis": return e6axisGetCorrectPattern;
                case "frame": return frameGetCorrectPattern;
                case "pos": return posGetCorrectPattern;
                case "e6pos": return e6posGetCorrectPattern;
			    default:
				    _warnings.Add(current_line, WarningType.Deep_Intern, Level.Information, "GetCorrectPattern(): could not find a RegExp pattern to type " + type);
				    //Throw New NotImplementedException
				    return null;
		    }
		    //return functionReturnValue;
	    }

        public static DynamicMemory GetCorrectMemory(string type, Warnings _warnings)
	    {
		    switch (type.ToLower()) {
			    case "date":
				    return CreateTypeDate();
			    case "_caleqpressure":
				    return CreateType_calEqPressure();
			    case "swp_option_ctl":
				    return CreateSimpleType(typeof(OPTION_CTL));
			    case "tipdress":
				    return CreateTypeTipDress();
			    case "swp_gun_mounting":
				    return CreateSimpleType(typeof(swp_GUN_MOUNTING));
			    case "gripstruc":
				    return CreateTypeGripStruc();
			    case "grpactuator":
				    return CreateTypeGrpActuator();
			    case "sps_prog_type":
				    return CreateTypeSpsProgType();
			    case "machine_def_t":
				    return CreateTypeMachineDefT();
			    case "machine_frame_t":
				    return CreateTypeMachineToolT();
			    case "machine_tool_t":
				    return CreateTypeMachineToolT();
			    case "ldat":
				    return CreateTypeLDat();
			    case "pdat":
				    return CreateTypePDat();
			    case "fdat":
				    return CreateTypeFDat();
			    case "real":
				    return CreateSimpleType(typeof(double));
			    case "bool":
				    return CreateSimpleType(typeof(bool));
			    case "int":
				    return CreateSimpleType(typeof(int));
			    case "ipo_m_t":
				    return CreateSimpleType(typeof(IPO_M_T));
			    case "load":
				    return CreateTypeLoad();
			    case "char":
				    return CreateSimpleType(typeof(string));
			    case "axis":
				    return CreateTypeAxis();
			    case "e6axis":
				    return CreateTypeE6Axis();
			    case "frame":
				    return CreateTypeFrame();
			    case "pos":
				    return CreateTypePos();
			    case "e6pos":
				    return CreateTypeE6Pos();
			    case "fra":
				    return CreateTypeFRA();
			    default:
				    _warnings.Add(current_line, WarningType.Deep_Intern, Level.Failure, "GetCorrectMemory(): could not find correct memory to type " + type);
				    return null;
		    }
	    }

        public static void AssignMemoryFromData(Dictionary<string, string> patterns, DynamicMemory memory, string value, Warnings _warnings)
	    {
		    if (patterns == null)
			    return;
		    if (value == null)
			    return;
		    if (memory == null)
			    return;
		    foreach (KeyValuePair<string, string> item in patterns) {
                Match match = RegexHelper.Match(item.Value,value);
                //if (value.Contains("{")) System.Diagnostics.Debugger.Break();
			    if (match.Success) {
                    var xitem = memory[item.Key];
                    if (xitem is double) memory[item.Key] = ToDouble(match.Groups[1].Value, item.Key, _warnings);
                    else if (xitem is int) memory[item.Key] = ToInteger(match.Groups[1].Value, item.Key, _warnings);
                    else if (xitem is bool) memory[item.Key] = ToBoolean(match.Groups[1].Value, item.Key, _warnings);
                    else if (xitem is IPO_M_T) memory[item.Key] = ToIPO_M_T(match, item.Key, _warnings);
                    else if (xitem is ORI_TYPE) memory[item.Key] = ToORI_TYP(match, item.Key, _warnings);
                    else if (xitem is CIRC_TYPE) memory[item.Key] = ToCIRC_TYP(match, item.Key, _warnings);
                    else if (xitem is MECH_TYPE) memory[item.Key] = ToMECH_TYPE(match.Groups[1].Value, item.Key, _warnings);
                    else if (xitem is GrpACT_Type) memory[item.Key] = ToGrpACT_Type(match.Groups[1].Value, item.Key, _warnings);
                    else if (xitem is Grp_STATE) memory[item.Key] = ToGrp_STATE(match.Groups[1].Value, item.Key, _warnings);
                    else if (xitem is swp_GUN_MOUNTING) memory[item.Key] = Toswp_GUN_MOUNTING(match, item.Key, _warnings);
                    else if (xitem is OPTION_CTL) memory[item.Key] = ToOPTION_CTL(match, item.Key, _warnings);
                    else if (xitem is APO_MODE_T) memory[item.Key] = ToAPO_MODE_T(match, item.Key, _warnings);
                    else if (xitem is string) memory[item.Key] = match.Groups[2].Value;
                    else if (xitem is DynamicMemory)
                    {
                        DynamicMemory x = (DynamicMemory) memory[item.Key];
                        string[] members = x.GetDynamicMemberNames().ToArray();
                        foreach (string mitem in members)
                        {
                            if (!match.Groups[item.Key + "_" + mitem].Success)
                            {
                                _warnings.Add(current_line, WarningType.Deep_Intern, Level.Failure, "Could not find match group with name " + mitem);
                                continue;
                            }
                            if (x[mitem] is DynamicMemory)
                            {
                                DynamicMemory z = (DynamicMemory) x[mitem];
                                string[] zmembers = z.GetDynamicMemberNames().ToArray();
                                foreach (string zitem in zmembers)
                                {
                                    if (!match.Groups[item.Key + "_" + mitem + "_" + zitem].Success)
                                    {
                                        _warnings.Add(current_line, WarningType.Deep_Intern, Level.Failure, "Could not find match group with name " + mitem);
                                        continue;
                                    }
                                    if (z[zitem] is CIRC_MODE_args) z[zitem] = ToCB_In(match.Groups[item.Key + "_" + mitem + "_" + zitem].Value, item.Key, _warnings);
                                    else
                                    {
                                        string s = memory[item.Key].GetType().ToString();
                                        throw new NotImplementedException();
                                    }
                                }
                                x[mitem] = z;
                            }
                            else if (x[mitem] is double) x[mitem] = ToDouble(match.Groups[item.Key + "_" + mitem].Value, mitem, _warnings);
                            else
                            {
                                string s = memory[item.Key].GetType().ToString();
                                throw new NotImplementedException();
                            }
                        }
                        memory[item.Key] = x;
                    }
                    else
                    {
                        string s = memory[item.Key].GetType().ToString();
                        throw new NotImplementedException();
                    }
			    }
		    }
	    }
	    #endregion

    }
}
