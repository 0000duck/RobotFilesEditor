using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Dialogs.CreateOrgs.ManageTypes
{
    public class ManageTypesData : ICloneable   
    {
        public string LineName { get; set; }
        public List<TypeAndNum> Types { get; set; }
        public List<int> PLCs { get; set; }

        public ManageTypesData(string lineName, List<TypeAndNum> types, List<int> plcs)
        {
            LineName = lineName;
            Types = types;
            PLCs = plcs;
        }

        public object Clone()
        {
            return new ManageTypesData(this.LineName, this.Types, this.PLCs);
        }
    }

    public class TypeAndNum
    {
        public int Number { get; set; }
        public string Description { get; set; }
        public TypeAndNum(int number, string description)
        {
            Number = number;
            Description = description;
        }
    }
}
