using ParseModuleFile.KUKA.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.KUKA.DataTypes
{
    public class NumOfHomes : Variable
    {
        private int numberOfHomes;


        [CAttrField("Number of homes", EAttrType.Int, Max = 5, Min = 1, BoundCheck = true)]
        public int NumberOfHomes { get { return numberOfHomes; } set { Set(ref numberOfHomes, value); } }

        public override string DataTypeName { get { return "NumOfHomes"; } }

        public NumOfHomes(int number)
        {
            numberOfHomes = number;
        }
        public override string ToString()
        {
            string ret = "PLC_ACTIVE_HOMES=" + numberOfHomes.ToString(CultureInfo.InvariantCulture) + Environment.NewLine;
            return ret;
        }
    }
}
