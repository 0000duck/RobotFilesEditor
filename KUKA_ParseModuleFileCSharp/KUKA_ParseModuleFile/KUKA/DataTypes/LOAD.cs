using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.KUKA.DataTypes
{
    public partial class LOAD
    {
        public override bool HasData()
        {
            if (m != -1) return true;
            if (cM.X != 0 || cM.Y != 0 || cM.Z != 0 || cM.A != 0 || cM.B != 0 || cM.C != 0) return true;
            if (j.X != 0 || j.Y != 0 || j.Z != 0) return true;
            return false;
        }
    }
}
