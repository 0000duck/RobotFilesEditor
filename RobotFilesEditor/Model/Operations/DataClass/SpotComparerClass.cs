using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Model.Operations.DataClass
{
    public class SpotComparerClass
    {
        public string Backup1 { get; set; }
        public string Backup2 { get; set; }

        public SpotComparerClass(string backup1, string backup2)
        {
            Backup1 = backup1;
            Backup2 = backup2;
        }
    }

    public class PointsPair
    {
        public string Name { get; set; }
        public CommonLibrary.PointXYZABC Point1 { get; set; }
        public CommonLibrary.PointXYZABC Point2 { get; set; }

        public PointsPair(string name, CommonLibrary.PointXYZABC point1, CommonLibrary.PointXYZABC point2)
        {
            Name = name;
            Point1 = point1;
            Point2 = point2;
        }
    }

}
