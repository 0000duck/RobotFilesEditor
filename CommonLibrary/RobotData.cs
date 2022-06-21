using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary
{
    class RobotData
    {
    }

    public interface IRobotPoint
    {
        double X { get; set; }
        double Y { get; set; }
        double Z { get; set; }
    }

    public class PointXYZABC : IRobotPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }

        public PointXYZABC(double x, double y, double z, double a, double b, double c)
        {
            X = x;
            Y = y;
            Z = z;
            A = a;
            B = b;
            C = c;
        }

        public PointXYZABC()
        { }
        
    }
}
