using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotSafetyGenerator
{
    public class Robot
    {
        public string RobotName { get; set; }
        public double Xpos { get; set; }
        public double Ypos { get; set;  }
        public double Zpos { get; set; }
        public double ElbowOriginX { get; set; }
        public double ElbowOriginY { get; set; }
        public double ElbowOriginZ { get; set; }

        public Robot (string robotName, double xpos, double ypos, double zpos, double elbowOriginX, double elbowOriginY, double elbowOriginZ)
        {
            RobotName = robotName;
            Xpos = xpos;
            Ypos = ypos;
            Zpos = zpos;
            ElbowOriginX = elbowOriginX;
            ElbowOriginY = elbowOriginY;
            ElbowOriginZ = elbowOriginZ;
        }
       
    }
    static class RobotList
    {
        public static List<Robot> RobotsElbow { get; set; }

        static RobotList ()
        {
            List<Robot> robotsElbow = new List<Robot>();
            Robot robot1 = new Robot("irb6700_235_2.65", -262.0, 0.000, 361.000,320,0,1915);
            Robot robot2 = new Robot("irb6700_205_2.80", -262.0, 0.000, 361.000,320,0,2060);
            Robot robot3 = new Robot("irb6700_245_3.00", -268.0, 0.000, 375.000,350,0,1925);
            Robot robot4 = new Robot("irb6700_150_3.20", -262.0, 0.000, 361.000,320,0,2060);
            Robot robot5 = new Robot("irb6700_300_2.70", -268.0, 0.000, 375.000,350,0,1925);
            Robot robot6 = new Robot("irb7600_325_3.10", -254.0, 0.000, 367.000,410,0,1855);

            robotsElbow.Add(robot1);
            robotsElbow.Add(robot2);
            robotsElbow.Add(robot3);
            robotsElbow.Add(robot4);
            robotsElbow.Add(robot5);
            robotsElbow.Add(robot6);

            RobotsElbow = robotsElbow;
        }
    }

}
