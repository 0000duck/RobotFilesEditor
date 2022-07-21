using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.KUKA.DataTypes
{
    public partial class MemorySelector
    {
        public static Variable XGet(string type, ANTLR.DataItems dataItems, Robot robot)
        {
            bool hasInOrig = Get(type, null) != null;
            if (robot.DataTypes.Enums.ContainsKey(type))
            {
                //System.Diagnostics.Debugger.Break();
                return null;
            }
            else if (robot.DataTypes.Structures.ContainsKey(type))
            {
                //PRESET
                //System.Diagnostics.Debugger.Break();
                return null;
            }
            else return Get(type, dataItems);
        }
    }
}
