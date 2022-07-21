using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.KUKA.DataTypes
{
    public class CustomEnums : ObservableDictionary<string,CustomEnum>
    {
        public CustomEnums(IEnumerable<CustomEnum> enums = null) : base ()
        {
            if (enums != null)
                foreach (CustomEnum @enum in enums)
                    this.Add(@enum.Name, @enum);
        }
    }
}
