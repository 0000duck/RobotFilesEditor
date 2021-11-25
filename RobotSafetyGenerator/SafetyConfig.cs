using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotSafetyGenerator
{
    public class SafetyConfigABB
    {
        public List<SafeToolABB> SafeTools { get; set; }
        public List<SafeZoneABB> SafeZones { get; set; }
        public List<SafeToolOldABB> SafeToolsOldABB { get; set; }
        public ElbowABB Elbow { get; set; }
        public string RobotName { get; set; }

        public SafetyConfigABB(List<SafeToolABB> safeTools, List<SafeZoneABB> safeZones, ElbowABB elbow, string robotName)
        {
            SafeTools = safeTools;
            SafeZones = safeZones;
            Elbow = elbow;
            RobotName = robotName;
        }

        public SafetyConfigABB()
        { }
    }

    public class SafetyConfigFanuc
    {
        public List<SafeToolFanuc> SafeTools { get; set; }
        public List<UserFrame> UserFrames { get; set; }
        public SafeZonesFanuc SafeZones { get; set; }
        public ElbowFanuc Elbow { get; set; }
        public string RobotName { get; set; }

        public SafetyConfigFanuc(List<SafeToolFanuc> safeTools, List<UserFrame> userFrames,SafeZonesFanuc safeZones, ElbowFanuc elbow,string robotName)
        {
            SafeTools = safeTools;
            UserFrames = userFrames;
            SafeZones = safeZones;
            Elbow = elbow;
            RobotName = robotName;
        }

        public SafetyConfigFanuc()
        { }
    }

    public class SafeZonesFanuc
    {
        public List<SafeZoneFanucTwoPoints> SafeZonesTwoPoints { get; set; }
        public List<SafeZoneFanucMultiPoints> SafeZonesMultiPoints { get; set; }

        public void Sort()
        {
            
            List<SafeZoneFanucTwoPoints> sortedList = new List<SafeZoneFanucTwoPoints>();
            int[] intArray = new int[this.SafeZonesTwoPoints.Count];
            int loopCounter = 0;
            foreach (SafeZoneFanucTwoPoints zone in this.SafeZonesTwoPoints)
            {
                intArray[loopCounter] = zone.Number;
                loopCounter++;
            }
            int[] sortedCopy = (from element in intArray orderby element ascending select element).ToArray();
            for (int i = 1; i <= 32; i++)
            {
                foreach (SafeZoneFanucTwoPoints zone in this.SafeZonesTwoPoints.Where(item => item.Number == i))
                    sortedList.Add(zone);
                if (sortedList.Count == this.SafeZonesTwoPoints.Count)
                    break;
            }
            this.SafeZonesTwoPoints =sortedList;
        }

        public SafeZonesFanuc(List<SafeZoneFanucTwoPoints> safeZonesTwoPoints, List<SafeZoneFanucMultiPoints> safeZonesMultiPoints)
        {
            SafeZonesTwoPoints = safeZonesTwoPoints;
            SafeZonesMultiPoints = safeZonesMultiPoints;
        }

        public SafeZonesFanuc()
        { }
    }

    public class ElbowFanuc
    {
        public CapsuleFanuc Elbow { get; set; }

        public ElbowFanuc(CapsuleFanuc elbow)
        {
            Elbow = elbow;
        }
        public ElbowFanuc()
        { }
    }

    public class FanucZoneStrings
    {
        public string SafeZonesTwoPoints { get; set; }
        public string SafeZonesMultiPoints { get; set; }

        public FanucZoneStrings(string safeZonesTwoPoints, string safeZonesMultiPoints)
        {
            SafeZonesTwoPoints = safeZonesTwoPoints;
            SafeZonesMultiPoints = safeZonesMultiPoints;
        }

        public FanucZoneStrings()
        { }
    }

    public class ElbowABB
    {
        public Point Origin { get; set; }
        public List<CapsuleABB> Capsules { get; set; }
        public List<SphereABB> Spheres { get; set; }
        public List<LozengeABB> Lozenges { get; set; }

        public ElbowABB (Point origin = null ,List<CapsuleABB> capsules = null, List<SphereABB> spheres = null, List<LozengeABB> lozenges = null)
        {
            Origin = origin;
            Capsules = capsules;
            Spheres = spheres;
            Lozenges = lozenges;
        }
        public ElbowABB()
        {
        }
    }

    public class ToolAndZoneNumbers
    {
        public List<int> SafeToolNumbers { get; set; }
        public List<int> SafeZones { get; set; }

        public ToolAndZoneNumbers (List<int> safeToolNumbers, List<int> safeZones)
        {
            SafeToolNumbers = safeToolNumbers;
            SafeZones = safeZones;
        }
    }

    public class Point
    {
        public float Xpos { get;set; }
        public float Ypos { get; set; }
        public float Zpos { get; set; }
        public float Quat1 { get; set; }
        public float Quat2 { get; set; }
        public float Quat3 { get; set; }
        public float Quat4 { get; set; }

        public Point (float xpos, float ypos, float zpos, float quat1, float quat2, float quat3, float quat4)
        {
            Xpos = xpos;
            Ypos = ypos;
            Zpos = zpos;
            Quat1 = quat1;
            Quat2 = quat2;
            Quat3 = quat3;
            Quat4 = quat4;
        }
        public Point()
        { }
    }

    public class PointEuler
    {
        public float Xpos { get; set; }
        public float Ypos { get; set; }
        public float Zpos { get; set; }
        public float A { get; set; }
        public float B { get; set; }
        public float C { get; set; }
        
        public PointEuler(float xpos, float ypos, float zpos, float a, float b, float c)
        {
            Xpos = xpos;
            Ypos = ypos;
            Zpos = zpos;
            A = a;
            B = b;
            C = c;
        }
        public PointEuler()
        { }
    }

    public class LozengeABB
    {
        public int Number { get; set; }
        public float Radius { get; set; }
        public float Height { get; set; }
        public float Width { get; set; }
        public Point CenterPoint { get; set; }

        public LozengeABB (int number, float radius, float height, float width, Point centerPoint)
        {
            Number = number;
            Radius = radius;
            Height = height;
            Width = width;
            CenterPoint = centerPoint;
        }

        public LozengeABB()
        { }
    }

    public class CapsuleABB
    {
        public int Number { get; set; }
        public Point Point1 { get; set;}
        public Point Point2 { get; set; }
        public float Radius { get; set; }

        public CapsuleABB (int number, Point point1, Point point2, float radius)
        {
            Number = number;
            Point1 = point1;
            Point2 = point2;
            Radius = radius;
        }

        public CapsuleABB ()
        { }
    }

    public class CapsuleFanuc
    {
        public int Number { get; set; }
        public Point Point1 { get; set; }
        public Point Point2 { get; set; }
        public float Radius { get; set; }

        public CapsuleFanuc(int number, Point point1, Point point2, float radius)
        {
            Number = number;
            Point1 = point1;
            Point2 = point2;
            Radius = radius;
        }

        public CapsuleFanuc()
        { }
    }

    public class SphereFanuc
    {
        public int Number { get; set; }
        public Point CenterPoint { get; set; }
        public float Radius { get; set; }

        public SphereFanuc(int number, Point centerPoint, float radius)
        {
            Number = number;
            CenterPoint = centerPoint;
            Radius = radius;
        }

        public SphereFanuc()
        { }
    }

    public class BoxFanuc
    {
        public int Number { get; set; }
        public Point CenterPoint { get; set; }
        public float Height { get; set; }
        public float Width { get; set; }
        public float Thickness { get; set; }

        public BoxFanuc(int number, Point centerPoint, float height, float width, float thickness)
        {
            Number = number;
            CenterPoint = centerPoint;
            Height = height;
            Width = width;
            Thickness = thickness;
        }

        public BoxFanuc()
        { }
    }

    public class SphereABB
    {
        public int Number { get; set; }
        public Point CenterPoint { get; set; }
        public float Radius { get; set; }

        public SphereABB (int number, Point centerPoint, float radius)
        {
            Number = number;
            CenterPoint = centerPoint;
            Radius = radius;
        }

        public SphereABB ()
        { }
    }

    public class SafeToolABB
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public Point TCP { get; set; }
        public List<SphereABB> Spheres { get; set; }
        public List<LozengeABB> Lozenges { get; set; }
        public List<CapsuleABB> Capsules { get; set; }

        public SafeToolABB (int number, string name, Point tcp, List<SphereABB> spheres, List<LozengeABB> lozenges, List<CapsuleABB> capsules)
        {
            Number = number;
            Name = name;
            TCP = tcp;
            Spheres = spheres;
            Lozenges = lozenges;
            Capsules = capsules;
        }

        public SafeToolABB()
        { }
    }

    public class SafeToolOldABB
    {
        public List<Point> Points { get; set; }
        public int Number { get; set; }

        public SafeToolOldABB(int number,List<Point> points)
        {
            Number = number;
            Points = points;
        }

        public SafeToolOldABB()
        {

        }
    }

    public class SafeToolFanuc
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public Point TCP { get; set; }
        public List<CapsuleFanuc> Capsules { get; set; }
        public List<SphereFanuc> Spheres { get; set; }
        public List<BoxFanuc> Boxes { get; set; }

        public SafeToolFanuc(int number, string name, Point tcp, List<CapsuleFanuc> capsules, List<SphereFanuc> spheres, List<BoxFanuc> boxes)
        {
            Number = number;
            Name = name;
            TCP = tcp;
            Capsules = capsules;
            Spheres = spheres;
            Boxes = boxes;
        }

        public SafeToolFanuc()
        { }
    }

    public class SafeZoneABB
    {
        public List<Point> SafeZonePoints { get; set; }
        public float Height { get; set; }
        public int SafeZoneNumber { get; set; }
        public string Name;
        public bool IsWorkCell;

        public SafeZoneABB (int safeZoneNumber, string name,List<Point> safeZonePoints, float height, bool isWorkCell = false)
        {
            SafeZoneNumber = safeZoneNumber;
            Name = name;
            SafeZonePoints = safeZonePoints;
            Height = height;
            IsWorkCell = isWorkCell;
        }

        public SafeZoneABB()
        { }
    }

    public class SafeZoneFanucTwoPoints
    {
        public bool IsDI;
        public int Number;
        public string Name { get; set; }
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
        public UserFrame UFrame { get; set; }

        public SafeZoneFanucTwoPoints(bool isDI, int number, string name, Point startPoint, Point endPoint, UserFrame uframe)
        {
            IsDI = isDI;
            Number = number;
            Name = name;
            StartPoint = startPoint;
            EndPoint = endPoint;
            UFrame = uframe;
        }

        public SafeZoneFanucTwoPoints()
        { }
    }

    public class UserFrame
    {
        public PointEuler Point;
        public int Number;
        public int UserFrameUFrameNumber;
        public bool IsValidUserFrame;

        public UserFrame(PointEuler point, int number, int userFrameUFrameNumber, bool isValidUserFrame = true)
        {
            Point = point;
            Number = number;
            UserFrameUFrameNumber = userFrameUFrameNumber;
            IsValidUserFrame = isValidUserFrame;
        }

        public UserFrame()
        { }
    }

    public class SafeZoneFanucMultiPoints
    {
        public int Number;
        public string Name { get; set; }
        public List<Point> Points { get; set; }
        public float Bottom { get; set; }
        public float Top { get; set; }
        public bool IsLI { get; set; }

        public SafeZoneFanucMultiPoints(int number, string name, List<Point> points, float bottom, float top, bool isLI)
        {
            Number = number;
            Name = name;
            Points = points;
            Bottom = bottom;
            Top = top;
            IsLI = isLI;
        }

        public SafeZoneFanucMultiPoints()
        { }
    }

    public class SafetyConfigKukaKRC4
    {
        public List<SafeToolKUKA> SafeTools { get; set; }
        public CellSpaceKuka CellSpace { get; set; }
        public List<SafeZoneKUKA> SafeZones { get; set; }
        public string RobotName { get; set; }

        public SafetyConfigKukaKRC4(List<SafeToolKUKA> safeTools, CellSpaceKuka cellSpace, List<SafeZoneKUKA> safeZones, string robotName)
        {
            SafeTools = safeTools;
            CellSpace = cellSpace;
            SafeZones = safeZones;
            RobotName = robotName;
        }

        public SafetyConfigKukaKRC4()
        { }
    }

    public class SafeToolKUKA
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public PointEuler TCP { get; set; }
        public List<SphereKuka> Spheres { get; set; }

        public SafeToolKUKA(int number, string name, PointEuler tcp, List<SphereKuka> spheres)
        {
            Number = number;
            Name = name;
            TCP = tcp;
            Spheres = spheres;
        }

        public SafeToolKUKA()
        { }
    }

    public class SphereKuka
    {
        public int Number { get; set; }
        public PointEuler CenterPoint { get; set; }
        public float Radius { get; set; }

        public SphereKuka(int number, PointEuler centerPoint, float radius)
        {
            Number = number;
            CenterPoint = centerPoint;
            Radius = radius;
        }

        public SphereKuka()
        { }
    }

    public class CellSpaceKuka
    {
        public List<PointEuler> Points { get; set; }
        public float Top { get; set; }

        public CellSpaceKuka(List<PointEuler> points, float top)
        {
            Points = points;
            Top = top;
        }

        public CellSpaceKuka()
        { }
    }

    public class SafeZoneKUKA
    {
        public int Number;
        public string Name { get; set; }
        public PointEuler Origin { get; set; }
        public PointEuler Max { get; set; }
        List<PointEuler> PoinsAtBase { get; set; }

        public SafeZoneKUKA(int number, string name, PointEuler origin, PointEuler max, List<PointEuler> pointsAtBase)
        {
            Number = number;
            Name = name;
            Origin = origin;
            Max = max;
            PoinsAtBase = pointsAtBase;
        }

        public SafeZoneKUKA()
        { }
    }


}
