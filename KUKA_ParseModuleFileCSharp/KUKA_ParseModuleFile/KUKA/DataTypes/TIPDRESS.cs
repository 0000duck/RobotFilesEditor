using ParseModuleFile.KUKA.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.KUKA.DataTypes
{
    public partial class TIPDRESS : Variable
    {

        static internal int maxNoDresser;

        public TIPDRESS(int Num)
        {
            num = Num;
        }

        public override bool HasData()
        {
            if (num <= maxNoDresser)
                return true;
            return false;
        }

    }
}
