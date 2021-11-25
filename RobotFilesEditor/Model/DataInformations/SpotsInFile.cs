using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Model.DataInformations
{
    public class SpotsInFile
    {
        public string PathName { get; set; } 
        public int SpotNr { get; set; }
        public int TypID { get; set; }
        public string ProcessType { get; set; }
        public int PointNum { get; set; }
        public string PointFullName { get; set; }

        public SpotsInFile(string pathName, int spotNr, int typID, string processType, string pointFullName)
        {

            PathName = pathName;
            SpotNr = spotNr;
            TypID = typID;
            ProcessType = processType;
            PointFullName = pointFullName;
        }
    }
}
