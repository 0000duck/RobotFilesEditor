using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Model.Operations.DataClass
{
    public class SafetyElementsClass
    {
        public List<SafetyTool> Tools { get; set; }
        public List<SafetyZone> SafeZones { get; set; }
        public Cellspace Cellspace { get; set; }

        public SafetyElementsClass(List<SafetyTool> tools, List<SafetyZone> safeZones, Cellspace cellspace)
        {
            Tools = tools;
            SafeZones = safeZones;
            Cellspace = cellspace;
        }
    }

    public class SafetyZone
    {
        public int Number { get; set; }
        public bool IsPermanent { get; set; }
        public string Name { get; set; }
        public PointKUKA Origin { get; set; }
        public PointKUKA P1 { get; set; }
        public PointKUKA P2 { get; set; }

        public SafetyZone(int number, bool ispermanent, string name, PointKUKA origin, PointKUKA p1, PointKUKA p2)
        {
            Number = number;
            IsPermanent = ispermanent;
            Name = name;
            Origin = origin;
            P1 = p1;
            P2 = p2;
        }
    }

    public class SafetyTool
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public PointKUKA TCP { get; set; }
        public List<Sphere> Spheres { get; set; }

        public SafetyTool(int number, string name, PointKUKA tcp, List<Sphere> spheres)
        {
            Number = number;
            Name = name;
            TCP = tcp;
            Spheres = spheres;
        }
    }

    public class Sphere
    {
        public int Number { get; set; }
        public double Radius { get; set; }
        public PointKUKA Center { get; set; }

        public Sphere(int number, double radius, PointKUKA center)
        {
            Number = number;
            Radius = radius;
            Center = center;
        }
    }

    public class Cellspace
    {
        public double Min { get; set; }
        public double Max { get; set; }
        public List<PointKUKA> Points { get; set; }

        public Cellspace(double min, double max, List<PointKUKA> points)
        {
            Min = min;
            Max = max;
            Points = points;
        }
    }
}
