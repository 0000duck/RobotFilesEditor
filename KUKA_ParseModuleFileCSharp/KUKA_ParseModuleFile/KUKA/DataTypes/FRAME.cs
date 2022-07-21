using ParseModuleFile.KUKA.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.KUKA.DataTypes
{
    public partial class FRAME : Variable
    {
        public override bool HasData()
        {
            if (x != 0 | y != 0 | z != 0 | a != 0 | b != 0 | c != 0)
                return true;
            return false;
        }
    }
}
