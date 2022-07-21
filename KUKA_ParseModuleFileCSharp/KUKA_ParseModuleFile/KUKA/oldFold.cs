using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarningHelper;
using ParseModuleFile.KUKA.DataTypes;
using ParseModuleFile.KUKA.Movements.KUKA.Movements;
using ParseModuleFile.KUKA.Movements;

namespace ParseModuleFile.KUKA
{
    public class oldFold : MyNotifyPropertyChanged
    {

        private List<string> localVariableList = new List<string>();
        private Warnings warnings;
        private int lineStart;
        private int lineEnd;
        private StringBuilder contents;
        private string name;
        private Application application;
        private Movement movement;
        private string module;

        public Warnings Warnings
        {
            get { return warnings; }
        }
        public int LineStart { get { return lineStart; } set { Set(ref lineStart, value); } }
        public int LineEnd { get { return lineEnd; } set { Set(ref lineEnd, value); } }
        public StringBuilder Contents { get { return contents; } set { Set(ref contents, value); } }
        public string Name { get { return name; } set { Set(ref name, value); } }
        public Application Application { get { return application; } set { Set(ref application, value); } }
        public Movement Movement { get { return movement; } set { Set(ref movement, value); } }

        public oldFold(Warnings Warnings)
        {
            this.warnings = Warnings;
        }

        public oldFold(string Module, int LineStart, string Contents, string Name, Warnings Warnings)
        {
            module = Module;
            this.warnings = Warnings;
            this.Name = Name;
            lineStart = LineStart;
            this.contents = new StringBuilder(Contents, 1024);
        }

        public oldFold(string Module, int LineStart, Warnings Warnings)
        {
            module = Module;
            this.warnings = Warnings;
            lineStart = LineStart;
        }

        public void FindMovement(List<string> localVariableList, VariablesContainer data)
        {
            Movement m;
            if (name.Contains(" PTP ") || name.StartsWith("PTP "))
                m = new PTP(this);
            else if (Name.Contains(" LIN ") | Name.StartsWith("LIN "))
            {
                m = new LIN(this);
            }
            else return;

            // ########################### E6POS, E6AXIS
            //Variable var = data.Get(module, m.PointName);
            //if (var == null) warnings.Add(lineStart, WarningType.Program_Paths, Level.Failure, "PTP argument definition not found (no E6POS or E6ASIS declared)", m.PointName);
            //else
            //{
            //    if (var.Type == "E6POS") m.Point = new E6POS(var.Value, m.PointName);
            //    else if (var.Type == "E6AXIS") m.Point = new E6AXIS(var.Value, m.PointName);
            //    else throw new NotImplementedException();
            //    if (!var.IsGlobal) localVariableList_add(m.PointName);
            //}

            //// ########################### FDAT
            //var = data.Get(module, m.FDATName);
            //if (var == null) warnings.Add(lineStart, WarningType.Program_Paths, Level.Failure, "PTP argument definition not found (no FDAT declared)", m.FDATName);
            //else
            //{
            //    FDAT fd;
            //    if (var.Type == "FDAT") fd = new FDAT(var.Value, m.FDATName);
            //    else throw new NotImplementedException();
            //    if (m.FDAT.TOOL_NO != -1 & fd.TOOL_NO != m.FDAT.TOOL_NO)
            //        warnings.Add(lineStart, WarningType.Program_Paths, Level.Failure, "ToolNo in fold declaration doesn't match FDAT.TOOL_NO", m.FDATName);
            //    if (m.FDAT.BASE_NO != -1 & fd.BASE_NO != m.FDAT.BASE_NO)
            //        warnings.Add(lineStart, WarningType.Program_Paths, Level.Failure, "BaseNo in fold declaration doesn't match FDAT.BASE_NO", m.FDATName);
            //    m.FDAT = fd;
            //    if (!var.IsGlobal) localVariableList_add(m.FDATName);
            //}

            //if (name.Contains(" PTP ") || name.StartsWith("PTP "))
            //{
            //    PTP n = (PTP)m;
            //    // ########################### PDAT
            //    var = data.Get(module, n.PDATName);
            //    if (var == null) warnings.Add(lineStart, WarningType.Program_Paths, Level.Failure, "PTP argument definition not found (no PDAT declared)", n.PDATName);
            //    else
            //    {
            //        if (var.Type == "PDAT") n.PDAT = new PDAT(var.Value, n.PDATName);
            //        else throw new NotImplementedException();
            //        if (!var.IsGlobal) localVariableList_add(n.PDATName);
            //    }
            //}
            //else if (Name.Contains(" LIN ") || Name.StartsWith("LIN "))
            //{
            //    LIN n = (LIN)m;
            //    // ########################### LDAT
            //    var = data.Get(module, n.LDATName);
            //    if (var == null) warnings.Add(lineStart, WarningType.Program_Paths, Level.Failure, "LIN argument definition not found (no LDAT declared)", n.LDATName);
            //    else
            //    {
            //        if (var.Type == "LDAT") n.LDAT = new LDAT(var.Value, n.LDATName);
            //        else throw new NotImplementedException();
            //        if (!var.IsGlobal) localVariableList_add(n.LDATName);
            //    }
            //}
            movement = m;

        }

        //public void FindMovement(List<string> localVariableList, File.Dat dat, Dictionary<int, object> data)
        //{
        //    this.localVariableList = localVariableList;
        //    if (Name.Contains(" PTP ") | Name.StartsWith("PTP "))
        //    {
        //        PTP m = new PTP(this);

        //        // ########################### E6POS, E6AXIS
        //        if (dat != null && dat.variables.ContainsKey(m.PointName))
        //        {
        //            if (dat.variables[m.PointName].Type == "E6POS")
        //            {
        //                m.Point = new E6POS(dat.variables[m.PointName].Value, m.PointName);
        //                data.Add(data.Count, m.Point);
        //            }
        //            else if (dat.variables[m.PointName].Type == "E6AXIS")
        //            {
        //                m.Point = new E6AXIS(dat.variables[m.PointName].Value, m.PointName);
        //                data.Add(data.Count, m.Point);
        //            }
        //            else
        //            {
        //                throw new NotImplementedException();
        //            }
        //            localVariableList_add(m.PointName);
        //        }
        //        else if (Program.GlobalE6pos != null && Program.GlobalE6pos.ContainsKey(m.PointName))
        //        {
        //            m.Point = Program.GlobalE6pos[m.PointName];
        //        }
        //        else if (Program.GlobalE6Axis != null && Program.GlobalE6Axis.ContainsKey(m.PointName))
        //        {
        //            m.Point = Program.GlobalE6Axis[m.PointName];
        //        }
        //        else
        //        {
        //            warnings.Add(LineStart, WarningType.Program_Paths, Level.Failure, "PTP argument definition not found (no E6POS or E6ASIS declared)", m.PointName);
        //        }

        //        // ########################### PDAT
        //        if (dat != null && dat.variables.ContainsKey(m.PDATName))
        //        {
        //            if (dat.variables[m.PDATName].Type == "PDAT")
        //            {
        //                m.PDAT = new PDAT(dat.variables[m.PDATName].Value, m.PDATName);
        //                data.Add(data.Count, m.PDAT);
        //            }
        //            localVariableList_add(m.PDATName);
        //        }
        //        else if (Program.GlobalPDat != null && Program.GlobalPDat.ContainsKey(m.PDATName))
        //        {
        //            m.PDAT = Program.GlobalPDat[m.PDATName];
        //        }
        //        else
        //        {
        //            warnings.Add(LineStart, WarningType.Program_Paths, Level.Failure, "PTP argument definition not found (no PDAT declared)", m.PDATName);
        //        }

        //        // ########################### FDAT
        //        if (dat != null && m.FDATName != null && dat.variables.ContainsKey(m.FDATName))
        //        {
        //            localVariableList_add(m.FDATName);
        //            FDAT fd = new FDAT(dat.variables[m.FDATName].Value, m.FDATName);
        //            if (m.FDAT.TOOL_NO != -1 & fd.TOOL_NO != m.FDAT.TOOL_NO)
        //                warnings.Add(LineStart, WarningType.Program_Paths, Level.Failure, "ToolNo in fold declaration doesn't match FDAT.TOOL_NO", m.FDATName);
        //            if (m.FDAT.BASE_NO != -1 & fd.BASE_NO != m.FDAT.BASE_NO)
        //                warnings.Add(LineStart, WarningType.Program_Paths, Level.Failure, "BaseNo in fold declaration doesn't match FDAT.BASE_NO", m.FDATName);
        //            m.FDAT = fd;
        //            data.Add(data.Count, fd);
        //        }
        //        else if (Program.GlobalFDat != null && m.FDATName != null && Program.GlobalFDat.ContainsKey(m.FDATName))
        //        {
        //            if (m.FDAT.TOOL_NO != -1 & Program.GlobalFDat[m.FDATName].TOOL_NO != m.FDAT.TOOL_NO)
        //                warnings.Add(LineStart, WarningType.Program_Paths, Level.Failure, "ToolNo in fold declaration doesn't match FDAT.TOOL_NO", m.FDATName);
        //            if (m.FDAT.BASE_NO != -1 & Program.GlobalFDat[m.FDATName].BASE_NO != m.FDAT.BASE_NO)
        //                warnings.Add(LineStart, WarningType.Program_Paths, Level.Failure, "BaseNo in fold declaration doesn't match FDAT.BASE_NO", m.FDATName);
        //            m.FDAT = Program.GlobalFDat[m.FDATName];
        //        }
        //        else if (m.FDATName != null)
        //        {
        //            warnings.Add(LineStart, WarningType.Program_Paths, Level.Failure, "PTP argument definition not found (no FDAT declared)", m.FDATName);
        //        }
        //        else
        //        {
        //            warnings.Add(LineStart, WarningType.Program_Paths, Level.Failure, "PTP argument declaration not found (no FDAT found in any .dat files)");
        //        }
        //        movement = m;
        //    }
        //    else if (Name.Contains(" LIN ") | Name.StartsWith("LIN "))
        //    {
        //        LIN m = new LIN(this);

        //        // ########################### E6POS, E6AXIS
        //        if (dat != null && dat.variables.ContainsKey(m.PointName))
        //        {
        //            if (dat.variables[m.PointName].Type == "E6POS")
        //            {
        //                m.Point = new E6POS(dat.variables[m.PointName].Value, m.PointName);
        //                data.Add(data.Count, m.Point);
        //            }
        //            else if (dat.variables[m.PointName].Type == "E6AXIS")
        //            {
        //                m.Point = new E6AXIS(dat.variables[m.PointName].Value, m.PointName);
        //                data.Add(data.Count, m.Point);
        //            }
        //            else
        //            {
        //                throw new NotImplementedException();
        //            }
        //            localVariableList_add(m.PointName);
        //        }
        //        else if (Program.GlobalE6pos != null && Program.GlobalE6pos.ContainsKey(m.PointName))
        //        {
        //            m.Point = Program.GlobalE6pos[m.PointName];
        //        }
        //        else if (Program.GlobalE6Axis != null && Program.GlobalE6Axis.ContainsKey(m.PointName))
        //        {
        //            m.Point = Program.GlobalE6Axis[m.PointName];
        //        }
        //        else
        //        {
        //            warnings.Add(LineStart, WarningType.Program_Paths, Level.Failure, "LIN argument definition not found (no E6POS or E6ASIS declared)", m.PointName);
        //        }

        //        // ########################### LDAT
        //        if (dat != null && dat.variables.ContainsKey(m.LDATName))
        //        {
        //            if (dat.variables[m.LDATName].Type == "LDAT")
        //            {
        //                m.LDAT = new LDAT(dat.variables[m.LDATName].Value, m.LDATName);
        //                data.Add(data.Count, m.LDAT);
        //            }
        //            localVariableList_add(m.LDATName);
        //        }
        //        else if (Program.GlobalLDat != null && Program.GlobalLDat.ContainsKey(m.LDATName))
        //        {
        //            m.LDAT = Program.GlobalLDat[m.LDATName];
        //        }
        //        else
        //        {
        //            warnings.Add(LineStart, WarningType.Program_Paths, Level.Failure, "LIN argument definition not found (no PDAT declared)", m.LDATName);
        //        }

        //        // ########################### FDAT
        //        if (dat != null && m.FDATName != null && dat.variables.ContainsKey(m.FDATName))
        //        {
        //            localVariableList_add(m.FDATName);
        //            FDAT fd = new FDAT(dat.variables[m.FDATName].Value, m.FDATName);
        //            if (m.FDAT.TOOL_NO != -1 & fd.TOOL_NO != m.FDAT.TOOL_NO)
        //                warnings.Add(LineStart, WarningType.Program_Paths, Level.Failure, "ToolNo in fold declaration doesn't match FDAT.TOOL_NO", m.FDATName);
        //            if (m.FDAT.BASE_NO != -1 & fd.BASE_NO != m.FDAT.BASE_NO)
        //                warnings.Add(LineStart, WarningType.Program_Paths, Level.Failure, "BaseNo in fold declaration doesn't match FDAT.BASE_NO", m.FDATName);
        //            m.FDAT = fd;
        //            data.Add(data.Count, fd);
        //        }
        //        else if (Program.GlobalFDat != null && m.FDATName != null && Program.GlobalFDat.ContainsKey(m.FDATName))
        //        {
        //            if (m.FDAT.TOOL_NO != -1 & Program.GlobalFDat[m.FDATName].TOOL_NO != m.FDAT.TOOL_NO)
        //                warnings.Add(LineStart, WarningType.Program_Paths, Level.Failure, "ToolNo in fold declaration doesn't match FDAT.TOOL_NO", m.FDATName);
        //            if (m.FDAT.BASE_NO != -1 & Program.GlobalFDat[m.FDATName].BASE_NO != m.FDAT.BASE_NO)
        //                warnings.Add(LineStart, WarningType.Program_Paths, Level.Failure, "BaseNo in fold declaration doesn't match FDAT.BASE_NO", m.FDATName);
        //            m.FDAT = Program.GlobalFDat[m.FDATName];
        //        }
        //        else if (m.FDATName != null)
        //        {
        //            warnings.Add(LineStart, WarningType.Program_Paths, Level.Failure, "LIN argument definition not found (no FDAT declared)", m.FDATName);
        //        }
        //        else
        //        {
        //            warnings.Add(LineStart, WarningType.Program_Paths, Level.Failure, "LIN argument declaration not found (no FDAT found in any .dat files)");
        //        }
        //        movement = m;
        //    }
        //}

        internal void localVariableList_add(string item)
        {
            if (!localVariableList.Contains(item.ToUpperInvariant()))
            {
                localVariableList.Add(item.ToUpperInvariant());
            }
        }

        public override string ToString()
        {
            return Movement != null ? Movement.ToString() + " " : "" + Application != null ? Application.ToString() : "";
        }
    }
}
