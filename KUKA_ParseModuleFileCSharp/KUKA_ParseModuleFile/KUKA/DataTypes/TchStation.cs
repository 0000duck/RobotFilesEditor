using ParseModuleFile.KUKA.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.KUKA.DataTypes
{
    public partial class TchStation : Variable
    {

        static internal int tch_I_MaxStation;
        static internal int tch_I_MaxExtStation;

        public TchStation(int Num)
        {
            num = Num;
        }

        public override bool HasData()
        {
            if (num<=tch_I_MaxStation) return true;
            if (num <= tch_I_MaxStation && num > 15) return true;
            return false;
        }

    }
}
