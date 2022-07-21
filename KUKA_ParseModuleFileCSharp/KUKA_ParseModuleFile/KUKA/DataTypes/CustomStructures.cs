using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.KUKA.DataTypes
{
    public class CustomStructures : ObservableDictionary<string,CustomStruct>
    {
        public CustomStructures(IEnumerable<CustomStruct> structs = null) : base()
        {
            if (structs != null)
                foreach (CustomStruct @struct in structs)
                    this.Add(@struct.Name, @struct);
        }
    }
}
