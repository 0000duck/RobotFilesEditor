using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobKalDat.Model.ProjectData
{
    public class Measurement
    {
        public string Name { get; set; }
        public string RobotType { get; set; }
        public string HasRealValues { get; set; }
        public double XSoll { get; set; }
        public double YSoll { get; set; }
        public double ZSoll { get; set; }
        public double RXSoll { get; set; }
        public double RYSoll { get; set; }
        public double RZSoll { get; set; }
        public double XIst { get; set; }
        public double YIst { get; set; }
        public double ZIst { get; set; }
        public double RXIst { get; set; }
        public double RYIst { get; set; }
        public double RZIst { get; set; }
        public ObservableCollection<CalculatedBase> Bases { get; set; }
        public ObservableCollection<ItemInSafety> Safety { get; set; }
        public List<string> BaseIDs { get; set; }
    }
}
