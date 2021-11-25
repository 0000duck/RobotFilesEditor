using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotPointsRenumber.Model.Operations.ABB
{
    public class ABBModule : ICloneable
    {
        public string Name { get; set; }
        public List<string> Content { get; set; }
        public Dictionary<string, string> Robtargets { get; set; }
        public Dictionary<string, List<string>> Procedures { get; set; }
        public Dictionary<string, List<string>> ProceduresWithRobtargets { get; set; }
        public List<ABBProcedure> ProceduresList { get; set; } 
        public List<KeyValuePair<string, string>> PointsOldAndNew { get; private set; }
        public bool Modified { get; set; }

        public ABBModule(string name)
        {
            Name = name;
            Content = new List<string>();
            Robtargets = new Dictionary<string, string>();
            Procedures = new Dictionary<string, List<string>>();
            ProceduresWithRobtargets = new Dictionary<string, List<string>>();
            ProceduresList = new List<ABBProcedure>();
            PointsOldAndNew = new List<KeyValuePair<string, string>>();
            Modified = false;
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
            foreach (var item in ProceduresList)
                result.ProceduresList.Add(item);
            foreach (var item in PointsOldAndNew)
                result.PointsOldAndNew.Add(item);
            result.Modified = Modified;

            return result;
        }

        public void AssignPointsOldAndNew()
        {
            if (PointsOldAndNew == null)
                PointsOldAndNew = new List<KeyValuePair<string, string>>();
            foreach (var item in Robtargets)
            {
                PointsOldAndNew.Add(new KeyValuePair<string, string>(item.Key, string.Empty));
            }
        }
    }
    public class ABBProcedure
    {
        public string Name { get; set; }
        public  List<string> Content { get; set; }
        public List<KeyValuePair<string, string>> PointsOldAndNew { get; private set; }

        public ABBProcedure(string name)
        {
            Name = name;
            Content = new List<string>();
            PointsOldAndNew = new List<KeyValuePair<string, string>>();
        }

        internal void SetNewName(string point, int counter)
        {
            KeyValuePair<string, string> newPair = new KeyValuePair<string, string>(PointsOldAndNew[counter].Key, point);
            PointsOldAndNew[counter] = newPair;
        }
    }
}
