using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        }
    }

    public class Variable
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsGlobal { get; set; }
        public string Localization { get; set; }
        public string Line { get; set; }
        public Variable(string name, string type, bool isGlobal, string localization, string line)
        {
            Name = name;
            Type = type;
            IsGlobal = isGlobal;
            Localization = localization;
            Line = line;
        }

    }

}
