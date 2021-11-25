using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Model.DataInformations
{
    public class E6AxisPoint
    {
        public string Name { get; set; }
        public double A1 { get; set; }
        public double A2 { get; set; }
        public double A3 { get; set; }
        public double A4 { get; set; }
        public double A5 { get; set; }
        public double A6 { get; set; }
        public double E1 { get; set; }

        public E6AxisPoint()
        { }

        public E6AxisPoint(string name, double a1, double a2, double a3, double a4, double a5, double a6, double e1)
        {
            Name = name;
            A1 = a1;
            A2 = a2;
            A3 = a3;
            A4 = a4;
            A5 = a5;
            A6 = a6;
            E1 = e1;
        }
    }

    public class SST
    {
        public List<E6AxisPoint> T1 { get; set; }
        public List<E6AxisPoint> T2 { get; set; }

        public SST(List<E6AxisPoint> t1, List<E6AxisPoint> t2)
        {
            T1 = t1;
            T2 = t2;
        }

    }

}
