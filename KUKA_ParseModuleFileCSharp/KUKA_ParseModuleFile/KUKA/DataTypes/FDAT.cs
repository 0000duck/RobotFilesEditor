using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.KUKA.DataTypes
{
    public partial class FDAT
    {
        public FDAT(int toolNo, int baseNo)
        {
            tOOL_NO = toolNo;
            bASE_NO = baseNo;
        }
    }
}
