using ParseModuleFile.KUKA.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.KUKA.DataTypes
{
    public partial class E6POS : Variable
    {
        public override bool HasData()
        {
            if (x != 0 || y != 0 || z != 0 || a != 0 || b != 0 || c != 0)
                return true;
            return false;
        }

        public static double Distance(E6POS left, E6POS right)
        {
            return Math.Sqrt(Math.Pow((left.X - right.X), 2) + Math.Pow((left.Y - right.Y), 2) + Math.Pow((left.Z - right.Z), 2));
        }

        public static double AngleDifference(E6POS left, E6POS right)
        {
            return (Math.Abs(left.A - right.A) % 360) + (Math.Abs(left.B - right.B) % 360) + (Math.Abs(left.C - right.C) % 360);
        }

    }

}
