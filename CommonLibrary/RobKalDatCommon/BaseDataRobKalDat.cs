using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.RobKalDatCommon
{
    public class BaseData
    {
        public string Name { get; set; }
        public int Number { get; set; }
        public bool? ExtBase { get; set; }
        public string Robot { get; set; }
        public string Worktool { get; set; }
        public string TCP { get; set; }
        public Point RobotCoordinates { get; set; }
        public Point WorktoolCoordinates { get; set; }
        public Point TCPCoordinates { get; set; }
        public Point CaluculatedBase { get; set; }

        public BaseData(string name, int number, bool? extBase, string robot, string worktool, Point robotCoordinates, Point worktoolCoordinates, string tcp = "", Point tcpcoordinates = null)
        {
            Name = name;
            Number = number;
            ExtBase = extBase;
            Robot = robot;
            Worktool = worktool;
            RobotCoordinates = robotCoordinates;
            WorktoolCoordinates = worktoolCoordinates;
            TCP = tcp;
            TCPCoordinates = tcpcoordinates;
        }
    }

    public class Point
    {
        public double XPos { get; set; }
        public double YPos { get; set; }
        public double ZPos { get; set; }
        public double RX { get; set; }
        public double RY { get; set; }
        public double RZ { get; set; }

        public Point(double xPos, double yPos, double zPos, double rx, double ry, double rz)
        {
            XPos = xPos;
            YPos = yPos;
            ZPos = zPos;
            RX = rx;
            RY = ry;
            RZ = rz;
        }

        public Point()
        {

        }
    }

    public class RobkaldatAndBackupPair
    {
        public string Robot { get; set; }
        public int Station { get; set; }
        public int RobotNumber { get; set; }
        public IDictionary<int,BaseData> BasesInRobKalDat { get; set; }
        public IDictionary<int,BaseData> BasesInBackups { get; set; }
        public RobkaldatAndBackupPair(string robot, int station, int robotnumber, IDictionary<int,BaseData> basesInRobKalDat, IDictionary<int,BaseData> basesInBackups)
        {
            Robot = robot;
            Station = station;
            RobotNumber = robotnumber;
            BasesInRobKalDat = basesInRobKalDat;
            BasesInBackups = basesInBackups;
        }

        public RobkaldatAndBackupPair()
        {

        }
    }

}
