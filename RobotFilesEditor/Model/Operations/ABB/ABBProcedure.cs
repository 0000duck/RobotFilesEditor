using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Model.Operations.ABB
{
    public class ABBModule : ICloneable 
    { 
        public string Name { get; set; }
        public List<string> Content { get; set; }
        public Dictionary<string, string> Robtargets { get; set; }
        public Dictionary<string, List<string>> Procedures { get; set; }
        public Dictionary<string, List<string>> ProceduresWithRobtargets { get; set; }

        public ABBModule(string name)
        {
            Name = name;
            Content = new List<string>();
            Robtargets = new Dictionary<string, string>();
            Procedures = new Dictionary<string, List<string>>();
            ProceduresWithRobtargets = new Dictionary<string, List<string>>();
        }

        public object Clone()
        {
            ABBModule result = new ABBModule(Name);
            foreach (var item in Content)
                result.Content.Add(item);
            foreach (var item in Robtargets)
                result.Robtargets.Add(item.Key, item.Value);
            foreach (var item in Procedures)
                result.Procedures.Add(item.Key, item.Value);
            foreach (var item in ProceduresWithRobtargets)
                result.ProceduresWithRobtargets.Add(item.Key, item.Value);

            return result;
        }
    }
}
