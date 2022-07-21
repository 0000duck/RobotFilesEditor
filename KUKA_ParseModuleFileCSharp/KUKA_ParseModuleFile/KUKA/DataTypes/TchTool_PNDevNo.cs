using ParseModuleFile.KUKA.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.KUKA.DataTypes
{
    public partial class TchTool_PNDevNo : Variable
    {

        static internal int tch_i_PN_Dev_count;

        public TchTool_PNDevNo(int Num, int FromIndex)
        {
            num = Num;
            fromIndex = FromIndex;
        }

        public override bool HasData()
        {
            if (num <= tch_i_PN_Dev_count) return true;
            return false;
        }

    }
}
