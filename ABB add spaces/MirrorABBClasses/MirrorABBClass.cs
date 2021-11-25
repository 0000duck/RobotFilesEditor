using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABB_add_spaces.MirrorABBClasses
{
    public class MirrorABBClass
    {

    }

    public class Jointtarget
    {
        public string A1 { get; set; }
        public string A2 { get; set; }
        public string A3 { get; set; }
        public string A4 { get; set; }
        public string A5 { get; set; }
        public string A6 { get; set; }
        public string E1 { get; set; }

        public Jointtarget(string a1, string a2, string a3, string a4, string a5, string a6, string e1)
        {
            A1 = a1;
            A2 = a2;
            A3 = a3;
            A4 = a4;
            A5 = a5;
            A6 = a6;
            E1 = e1;
        }
    }

    public class Robtarget
    {
        public string X { get; set; }
        public string Y { get; set; }
        public string Z { get; set; }
        public string Quat1 { get; set; }
        public string Quat2 { get; set; }
        public string Quat3 { get; set; }
        public string Quat4 { get; set; }
        public string Conf1 { get; set; }
        public string Conf2 { get; set; }
        public string Conf3 { get; set; }
        public string Conf4 { get; set; }
        public string E1 { get; set; }

        public Robtarget(string x, string y, string z, string quat1, string quat2, string quat3, string quat4, string conf1, string conf2, string conf3, string conf4, string e1)
        {
            X = x;
            Y = y;
            Z = z;
            Quat1 = quat1;
            Quat2 = quat2;
            Quat3 = quat3;
            Quat4 = quat4;
            Conf1 = conf1;
            Conf2 = conf2;
            Conf3 = conf3;
            Conf4 = conf4;
            E1 = e1;
        }
    }

    public class ABBAxisConf
    {
        public string Conf1 { get; set; }
        public string Conf2 { get; set; }
        public string Conf3 { get; set; }
        public string Conf4 { get; set; }
    }

    public class Tooldata
    {
        public string Robhold { get; set; }
        public string X { get; set; }
        public string Y { get; set; }
        public string Z { get; set; }
        public string Quat1 { get; set; }
        public string Quat2 { get; set; }
        public string Quat3 { get; set; }
        public string Quat4 { get; set; }

        public Tooldata(string robhold, string x, string y, string z, string quat1, string quat2, string quat3, string quat4)
        {
            Robhold = robhold;
            X = x;
            Y = y;
            Z = z;
            Quat1 = quat1;
            Quat2 = quat2;
            Quat3 = quat3;
            Quat4 = quat4;
        }

    }
}
