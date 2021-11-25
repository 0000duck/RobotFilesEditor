using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Model.Operations.DataClass
{
    public class FillTypIDData
    {

    }

    public class PointAndTypIDFromMPL
    {
        public int PointNum { get; set; }
        public int TypID { get; set; }

        public PointAndTypIDFromMPL(int pointNum, int typID)
        {
            PointNum = pointNum;
            TypID = typID;
        }
    }

}
