using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Model.Operations.DataClass
{
    class RenumberPointClasses
    {
    }

    public class SrcAndDat
    {
        public string Src { get; set; }
        public List<string> SrcContent { get; set; }
        public string Dat { get; set; }
        public List<string> DatContent { get; set; }

        public SrcAndDat(string src,  string dat, List<string> srcContent = null, List<string> datContent = null)
        {
            Src = src;
            SrcContent = srcContent;
            Dat = dat;
            DatContent = datContent;
        }
    }

    public class PointInSrc
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Target { get; set; }
        public string Fdat { get; set; }
        public List<string> Pdat { get; set; }
        public List<string> Ldat { get; set; }

        public PointInSrc(string name, string type, string target,string fdat, List<string> pdat, List<string> ldat)
        {
            Name = name;
            Type = type;
            Target = target;
            Fdat = fdat;
            Pdat = pdat;
            Ldat = ldat;
        }

        public PointInSrc()
        { }
    }

    public class PointOldAndNew
    {
        public PointInSrc OldPoint { get; set; }
        public PointInSrc NewPoint { get; set; }

        public PointOldAndNew()
        {
                
        }

        public PointOldAndNew(PointInSrc oldPoint, PointInSrc newPoint)
        {
            OldPoint = oldPoint;
            NewPoint = newPoint;
        }
    }

    public class Point
    {
        public double XPos { get; set; }
        public double YPos { get; set; }
        public double ZPos { get; set; }
        public double ARot { get; set; }
        public double BRot { get; set; }
        public double CRot { get; set; }

        public Point(double xpos, double ypos, double zpos, double arot, double brot, double crot)
        {
            XPos = xpos;
            YPos = ypos;
            ZPos = zpos;
            ARot = arot;
            BRot = brot;
            CRot = crot;
        }
    }

    public class Point2d
    {
        public double XPos { get; set; }
        public double YPos { get; set; }

        public Point2d()
        { }

        public Point2d(double xpos, double ypos)
        {
            XPos = xpos;
            YPos = ypos;
        }
    }

    public class LineAndParams
    {
        public string Line { get; set; }
        public PointInSrc Point { get; set; }

        public LineAndParams(string line, PointInSrc point)
        {
            Line = line;
            Point = point;
        }
    }
}
