using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommonLibrary
{
    public class FoundVariables
    {
        public Dictionary<string, Variable> FDATs { get; set; }
        public Dictionary<string, Variable> PDATs { get; set; }
        public Dictionary<string, Variable> LDATs { get; set; }
        public Dictionary<string, Variable> E6POS { get; set; }
        public Dictionary<string, Variable> E6AXIS { get; set; }
        public Dictionary<string, Variable> INTs { get; set; }
        public Dictionary<string, Variable> BOOLs { get; set; }
        public Dictionary<string, Variable> REALs { get; set; }
        public Dictionary<string, Variable> CHARs { get; set; }
        public Dictionary<string, Variable> STRUCTs { get; set; }
        public Dictionary<string, Variable> SIGNALs { get; set; }
        public Dictionary<string, Variable> Others { get; set; }
        //public Dictionary<string, string> GlobalDat { get; set; }
        public List<Variable> AllVariables { get; private set; }

        public FoundVariables()
        {
            FDATs = new Dictionary<string, Variable>();
            PDATs = new Dictionary<string, Variable>();
            LDATs = new Dictionary<string, Variable>();
            E6POS = new Dictionary<string, Variable>();
            E6AXIS = new Dictionary<string, Variable>();
            INTs = new Dictionary<string, Variable>();
            BOOLs = new Dictionary<string, Variable>();
            REALs = new Dictionary<string, Variable>();
            CHARs = new Dictionary<string, Variable>();
            STRUCTs = new Dictionary<string, Variable>();
            SIGNALs = new Dictionary<string, Variable>();
            Others = new Dictionary<string, Variable>();
            //GlobalDat = new Dictionary<string, string>();
        }

        public void FillAllVariables()
        {
            AllVariables = new List<Variable>();
            foreach (PropertyInfo prop in this.GetType().GetProperties().Where(x => x.PropertyType.Name.ToLower().Contains("dictionary")))
            {
                var property = prop.GetValue(this);
                foreach (var variable in (Dictionary<string,Variable>)property)
                {
                    AllVariables.Add(variable.Value);
                }
            }
        }
    }

    public class Variable
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsGlobal { get; set; }
        public string Localization { get; set; }
        public string LineContent { get; set; }
        public List<string> Names { get; private set; }
        public int LineNum { get; set; }

        public Variable(string name, string type, bool isGlobal, string localization, string line, int linenum)
        {
            Name = name;
            Type = type;
            IsGlobal = isGlobal;
            Localization = localization;
            LineContent = line;
            LineNum = linenum;
        }

        public Variable(string line, bool isGlobal, string localization, int linenum)
        {
            this.Type = (new Regex(@"(?<=^\s*(DECL|)\s*(GLOBAL|)\s*)("+CommonMethods.typeRegex + ")", RegexOptions.IgnoreCase)).Match(line).ToString().ToLower();
            string tempName = string.Empty;
            if (this.Type == "struc")
                tempName = (new Regex(@"(?<=^\s*(DECL|)\s*(GLOBAL|)\s*(STRUC)\s+)[a-zA-Z_\-]+", RegexOptions.IgnoreCase)).Match(line).ToString();
            else
                //tempName = (new Regex(@"(?<=^\s*(DECL|)\s*(GLOBAL|)\s*(INT|CHAR|BOOL|REAL|E6AXIS|E6POS|FDAT|PDAT|LDAT|STRUC|SIGNAL)\s+)[a-zA-Z0-9_\-,\s\[\]]+", RegexOptions.IgnoreCase)).Match(line).ToString();
                tempName = (new Regex(@"(?<=^\s*(DECL|)\s*(GLOBAL|)\s*(" + CommonMethods.typeRegex + @")\s+)[a-zA-Z0-9_\-,\s]+", RegexOptions.IgnoreCase)).Match(line).ToString();
            var splitNames = tempName.Split(',');
            if (splitNames.Count() == 1)
                Name = tempName.Trim();
            else
            {
                Name = splitNames[0].Trim();
                Names = new List<string>();
                splitNames.ToList().ForEach(x => Names.Add(x.Trim()));
            }
            Localization = localization;
            if (isGlobal)
                IsGlobal = true;
            else
            {
                IsGlobal = false;
                if (new Regex(@"(?<=^\s*(DECL|)\s*)GLOBAL", RegexOptions.IgnoreCase).IsMatch(line))
                    IsGlobal = true;
            }
            LineContent = line;
            LineNum = linenum;
        }

    }

    public class KukaValidationData
    {
        public string FilePath { get; set; }
        public Dictionary<string, List<string>> SrcFiles { get; set; }
        public Dictionary<string, List<string>> DatFiles { get; set; }

        public KukaValidationData(string filePath, Dictionary<string, List<string>> srcFiles, Dictionary<string, List<string>> datFiles)
        {
            FilePath = filePath;
            SrcFiles = srcFiles;
            DatFiles = datFiles;
        }
    }

}