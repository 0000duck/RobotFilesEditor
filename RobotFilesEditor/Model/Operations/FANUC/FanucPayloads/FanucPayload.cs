using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RobotFilesEditor.Model.Operations.FANUC.FanucPayloads
{
    public class FanucPayload
    {
        public string Name { get; set; }
        public double Mass { get; set; }
        public double PosX { get; set; }
        public double PosY { get; set; }
        public double PosZ { get; set; }
        public double Ix_Fanuc { get; set; }
        public double Iy_Fanuc { get; set; }
        public double Iz_Fanuc { get; set; }
        public double Ix_SI { get; private set; }
        public double Iy_SI { get; private set; }
        public double Iz_SI { get; private set; }

        public FanucPayload(string name, double mass, double xpos, double ypos, double zpos, double ix_fanuc, double iy_fanuc, double iz_fanuc)
        {
            Name = name;
            Mass = mass;
            PosX = xpos;
            PosY = ypos;
            PosZ = zpos;
            Ix_Fanuc = ix_fanuc;
            Iy_Fanuc = iy_fanuc;
            Iz_Fanuc = iz_fanuc;
            Ix_SI = Calculate_I_SI(ix_fanuc);
            Iy_SI = Calculate_I_SI(iy_fanuc);
            Iz_SI = Calculate_I_SI(iz_fanuc);
        }

        private double Calculate_I_SI(double i_fanuc)
        {
            return Math.Round(i_fanuc / 980,2);
        }
    }

    public class RobotWithPayloads
    {
        public string Name { get; set; }
        public string BackupPath { get; set; }
        public string RobotType {get; set;}
        public double ArmLoad { get; set; }
        public string Station { get; private set; }
        public List<FanucPayload> Payloads { get; set; }

        public RobotWithPayloads(string name, string backuppath, string robottype, double armLoad, List<FanucPayload> payloads)
        {
            Name = name;
            BackupPath = backuppath;
            RobotType = robottype;
            ArmLoad = armLoad;
            Payloads = payloads;
            Regex stationRegex = new Regex(@"ST\d+(?=_IR\d+)", RegexOptions.IgnoreCase);
            Station = stationRegex.IsMatch(Path.GetFileNameWithoutExtension(backuppath)) ? stationRegex.Match(Path.GetFileNameWithoutExtension(backuppath)).ToString() : "Unknown station";
        }
    }
}
