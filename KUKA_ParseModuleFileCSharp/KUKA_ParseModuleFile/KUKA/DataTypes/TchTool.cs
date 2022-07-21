using ParseModuleFile.KUKA.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.KUKA.DataTypes
{
    public partial class TchTool : Variable
    {

        static internal int tch_I_tchToolMax;

        public TchTool(int Num)
        {
            num = Num;
        }

        public override bool HasData()
        {
            if (num <= tch_I_tchToolMax) return true;
            return false;
        }

    }
}
