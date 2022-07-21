using ParseModuleFile.KUKA.DataTypes;
using ParseModuleFile.KUKA.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.KUKA.DataTypes
{
    public partial class Base : Variable
    {
        public Base(int item)
        {
            num = item;
        }
        //public override string ToString()
        //{
        //    return String.Format(CultureInfo.InvariantCulture,
        //        "BASE_DATA[{0}]={1}" + Environment.NewLine +
        //        "BASE_NAME[{0},]=\"{2}\"" + Environment.NewLine +
        //        "BASE_TYPE[{0}]=#{3}",
        //        num,
        //        frame,
        //        description,
        //        type);
        //}

        public override bool HasData()
        {
            if ((!string.IsNullOrEmpty(description) && description != " ") || frame.HasData() || type != IPO_M_T.NONE)
                return true;
            return false;
        }
    }
}
