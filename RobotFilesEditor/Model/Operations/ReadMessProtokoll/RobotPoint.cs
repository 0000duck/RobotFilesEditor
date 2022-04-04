using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Model.Operations.ReadMessProtokoll
{
    public class RobotPointMessprotokoll
    {
        public string Name { get; set; }
        public double XSoll { get; set; }
        public double YSoll { get; set; }
        public double ZSoll { get; set; }
        public double ASoll { get; set; }
        public double BSoll { get; set; }
        public double CSoll { get; set; }

        public double XIst { get; set; }
        public double YIst { get; set; }
        public double ZIst { get; set; }
        public double AIst { get; set; }
        public double BIst { get; set; }
        public double CIst { get; set; }
    }

        public class RobotPointKUKA : IRobotPoint
    {
        public string Name { get; set; }
        public double Xpos { get; set; }
        public double Ypos { get; set; }
        public double Zpos { get; set; }
        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }
    }

    public class RobotPointFANUC : IRobotPoint
    {
        public string Name { get; set; }
        public double Xpos { get; set; }
        public double Ypos { get; set; }
        public double Zpos { get; set; }
        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }
    }

    public class RobotPointABB : IRobotPoint
    {
        public string Name { get; set; }
        public double Xpos { get; set; }
        public double Ypos { get; set; }
        public double Zpos { get; set; }
        public double Q1 { get; set; }
        public double Q2 { get; set; }
        public double Q3 { get; set; }
        public double Q4 { get; set; }
    }

}
