using ParseModuleFile.KUKA.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.KUKA.DataTypes
{
    public partial class Gun : Variable
    {

        static internal int maxNoGuns;

        public Gun(int Num)
        {
            num = Num;
        }

        public override bool HasData()
        {
            if (num <= maxNoGuns)
                return true;
            return false;
        }

    }
}
