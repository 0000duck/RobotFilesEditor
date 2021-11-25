using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobKalDat.Model.ProjectData
{
    public class Coords
    {
        public string Name { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double RX { get; set; }
        public double RY { get; set; }
        public double RZ { get; set; }

        public Coords(double x, double y, double z, double rx, double ry, double rz, string name = "")
        {
            Name = name;
            X = Math.Round(x, 2) ;
            Y = Math.Round(y, 2);
            Z = Math.Round(z, 2);
            RX = Math.Round(rx, 4);
            RY = Math.Round(ry,4);
            RZ = Math.Round(rz,4);
        }
        public Coords()
        { }
    }

    public class CalculatedBase
    {
        public string Number { get; set; }
        public string Name { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double RX { get; set; }
        public double RY { get; set; }
        public double RZ { get; set; }
        public string BaseID { get; set; }
        public bool ExtTCP { get; set; }
        public Coords Robot { get; set; }
        public Coords Obj2 { get; set; }
        public Coords Obj3 { get; set; }
        public Coords Tool { get; set; }
        public Coords AdjustedTCP { get; set; }

        public CalculatedBase(string number, string name, double x, double y, double z, double rx, double ry, double rz, string baseID = "", bool extTCP = false, Coords robot = null, Coords obj2 = null, Coords obj3 = null, Coords tool = null, Coords adjustedTCP = null)
        {
            Number = number;
            Name = name;
            X = x;
            Y = y;
            Z = z;
            RX = rx;
            RY = ry;
            RZ = rz;
            BaseID = baseID;
            ExtTCP = extTCP;
            Robot = robot;
            Obj2 = obj2;
            Obj3 = obj3;
            Tool = tool;
            AdjustedTCP = adjustedTCP;
        }
    }
}
