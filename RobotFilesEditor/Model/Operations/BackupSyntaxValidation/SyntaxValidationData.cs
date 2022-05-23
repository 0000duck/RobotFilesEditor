using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RobotFilesEditor.Model.Operations.BackupSyntaxValidation
{
    public enum ProcType { Procedure, Function }

    #region classes
    public class Procedure : IProcOrFunc    
    {
        public ProcType ProcType { get; set; }
        public string Name { get; set; }
        public bool IsGlobal { get; set; }
        public string Localization { get; set; }
        public string Line { get; set; }
        public List<string> Arguments { get; set; }

        public Procedure(string line, string localization)
        {
            Regex getProcNameRegex = new Regex(@"(?<=^\s*(|GLOBAL)\s*DEF\s+)\w+", RegexOptions.IgnoreCase);
            Regex argumentRegex = new Regex(@"(?<=^\s*(|GLOBAL)\s*(DEF|DEFFCT\s+\w+)\s+[a-zA-Z0-9_\-\s]+\()[a-zA-Z0-9_:,\s\[\]]+", RegexOptions.IgnoreCase);

            Line = line;
            ProcType = ProcType.Procedure;
            Name = getProcNameRegex.Match(line).ToString();
            IsGlobal = line.ToLower().Contains("global") ? true : false;
            Localization = localization;
            Arguments = argumentRegex.Match(line).ToString().Replace(" ", "").Split(',').ToList();
            if (Arguments.Count == 1 && string.IsNullOrEmpty(Arguments[0]))
                Arguments = new List<string>();
        }
    }
    public class Function : IProcOrFunc
    {
        public ProcType ProcType { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsGlobal { get; set; }
        public string Localization { get; set; }
        public string Line { get; set; }
        public List<string> Arguments { get; set; }

        public Function(string line, string localization)
        {
            Regex getFctNameRegex = new Regex(@"(?<=^\s*(|GLOBAL)\s*DEFFCT\s+\w+\s+)\w+", RegexOptions.IgnoreCase);
            Regex getTypeRegex = new Regex(@"(?<=^\s*(|GLOBAL)\s*DEFFCT\s+)\w+", RegexOptions.IgnoreCase);
            Regex argumentRegex = new Regex(@"(?<=^\s*(|GLOBAL)\s*(DEF|DEFFCT\s+\w+)\s+[a-zA-Z0-9_\-\s]+\()[a-zA-Z0-9_:,\s\[\]]+", RegexOptions.IgnoreCase);

            ProcType = ProcType.Function;
            Name = getFctNameRegex.Match(line).ToString();
            Type = getTypeRegex.Match(line).ToString();
            IsGlobal = line.ToLower().Contains("global") ? true : false;
            Localization = localization;
            Line = line;
            Arguments = argumentRegex.Match(line).ToString().Replace(" ","").Split(',').ToList();
            if (Arguments.Count == 1 && string.IsNullOrEmpty(Arguments[0]))
                Arguments = new List<string>();
        }
    }
    #endregion
    public class ProcCall
    {
        public ProcType ProcType { get; set; }
        public string Name { get; set; }
        public string Localization { get; set; }
        public List<string> Arguments { get; set; }
        public string Line { get; set; }

        public ProcCall(string line, string localization)
        {
            ProcType = line.ToLower().Contains("deffct") ? ProcType.Function : ProcType.Procedure;
            Regex nameRegex = new Regex(@"(?<=^\s*[a-zA-Z0-9_\-]*)[a-zA-Z0-9_\-]+\s*(?=\([a-zA-Z0-9_\-\s]*\))", RegexOptions.IgnoreCase);
            Regex argumentRegex = new Regex(@"(?<=^\s*[a-zA-Z0-9_\-]*[a-zA-Z0-9_\-]+\s*\().*(?=\))", RegexOptions.IgnoreCase);
            Name = nameRegex.Match(line).ToString().Trim();
            Localization = localization;
            Line = line;
            Arguments = argumentRegex.Match(line).ToString().Replace(" ", "").Split(',').ToList();
            if (Arguments.Count == 1 && string.IsNullOrEmpty(Arguments[0]))
                Arguments = new List<string>();
        }
    }

    #region interfaces
    interface IProcOrFunc
    {
        ProcType ProcType { get; set; }
        string Name { get; set; }
        bool IsGlobal { get; set; }
        string Localization { get; set; }
        string Line { get; set; }
        List<string> Arguments { get; set; }
    }
    #endregion


}
