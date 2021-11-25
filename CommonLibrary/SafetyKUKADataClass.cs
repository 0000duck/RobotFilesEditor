using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary
{
    //public class SafetyKUKADataClass
    //{
    //    public SafetyConfig SafetyConfig { get; set; }
    //}
    #region READ FROM BACKUP CLASSES
    public class SafetyConfig
    {
        public Cellspace Cellspace { get; set; }
        public List<ISafeSpace> SafeSpaces { get; set; }
        public List<SafeTool> SafeTools { get; set; }

        public SafetyConfig(Cellspace cellspace, List<ISafeSpace> safespaces, List<SafeTool> safeTools = null)
        {
            Cellspace = cellspace;
            SafeSpaces = safespaces;
            SafeTools = safeTools;
        }
        public SafetyConfig()
        { }

    }
    public class PointInSafety
    {
        public double Xpos { get; set; }
        public double Ypos { get; set; }
        public double Zpos { get; set; }

        public PointInSafety(double xpos, double ypos, double zpos)
        {
            Xpos = xpos;
            Ypos = ypos;
            Zpos = zpos;
        }
    }

    public class Cellspace
    {
        public IDictionary<int, PointInSafety> Points { get; set; }
        public Cellspace(IDictionary<int, PointInSafety> points)
        {
            Points = points;
        }

        public Cellspace()
        { }
    }

    public class SafeSpace2points : ISafeSpace
    {
        public int Number { get; set; }
        public PointInSafety Origin { get; set; }
        public PointInSafety Max { get; set; }

        public SafeSpace2points(int number, PointInSafety origin, PointInSafety max)
        {
            Number = number;
            Origin = origin;
            Max = max;
        }
    }

    public class SafeSpaceMultiPoints : ISafeSpace
    {
        public int Number { get; set; }
        public List<PointInSafety> Points { get; set; }
       // public double MinHeight { get; set; }
        public double MaxHeight { get; set; }

        public SafeSpaceMultiPoints(int number, List<PointInSafety> points, double maxHeight)
        {
            Number = number;
            Points = points;
            //MinHeight = minHeight;
            MaxHeight = maxHeight;
        }
    }

    public class Sphere
    {
        public int Number { get; set; }
        public PointInSafety Coordinates { get; set; }
        public int Radius { get; set; }
    }

    public class SafeTool
    {
        public int Number { get; set; }
        public List<Sphere> Spheres { get; set; }
        public PointInSafety TCP { get; set; }
    }

    public interface ISafeSpace
    {
        int Number { get; set; }
    }
    #endregion
}
