using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Model.DataInformations
{
    public interface IWeldpoint
    {
        string Robot { get; set; }
        double XPos { get; set; }
        double YPos { get; set; }
        double ZPos { get; set; }
        double A { get; set; }
        double B { get; set; }
        double C { get; set; }
        int Status { get; set; }
        int Turn { get; set; }
        string Path { get; set; }
        string ProcessType { get; set; }
        string PathToBackup { get; set; }
        string PointFullName { get; set; }
    }

    public class WeldpointVW : IWeldpoint
    {
        public string Robot { get; set; }
        public string Name { get; set; }
        public double XPos { get; set; }
        public double YPos { get; set; }
        public double ZPos { get; set; }
        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }
        public int Status { get; set; }
        public int Turn { get; set; }
        public string Path { get; set; }
        public string ProcessType { get; set; }
        public string PathToBackup { get; set; }
        public string PointFullName { get; set; }
        public string SpotIndex { get; set; }

        public WeldpointVW(string robot, string name, double xpos, double ypos, double zpos, double a, double b, double c, string path, string processType, string pathToBackup, string spotIndex, int status = 0, int turn = 0)
        {
            Robot = robot;
            Name = name;
            XPos = xpos;
            YPos = ypos;
            ZPos = zpos;
            A = a;
            B = b;
            C = c;
            Status = status;
            Turn = turn;
            Path = path;
            ProcessType = processType;
            PathToBackup = pathToBackup;
            SpotIndex = spotIndex;
        }
    }

    public class WeldpointBMW : IWeldpoint
    {
        public string Robot { get; set; }
        public string PLC { get; set; }
        public int Number { get; set; }
        public int TypId { get; set; }
        public int PointNum { get; set; }
        public double XPos { get; set; }
        public double YPos { get; set; }
        public double ZPos { get; set; }
        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }
        public int Status { get; set; }
        public int Turn { get; set; }
        public string Path { get; set; }
        public string ProcessType { get; set; }
        public string PathToBackup { get; set; }
        public List<int> TypIDsInBosch { get; set; }
        public string PointFullName { get; set; }

        public WeldpointBMW(string robot, string path, string plc, int number, int typId, double xpos, double ypos, double zpos, double a, double b, double c, string processType, string pathToBackup, int status = 0, int turn = 0, List<int> typIDsInBosch = null)
        {
            Robot = robot;
            Path = path;
            PLC = plc;
            Number = number;
            TypId = typId;
            XPos = xpos;
            YPos = ypos;
            ZPos = zpos;
            A = a;
            B = b;
            C = c;
            Status = status;
            Turn = turn;
            ProcessType = processType;
            PathToBackup = pathToBackup;
            TypIDsInBosch = typIDsInBosch;
        }
    }

    public class WeldpointShort
    {
        public int Number { get; set; }
        public int TypId { get; set; }

        public WeldpointShort()
        {

        }

        public WeldpointShort(int number, int typid)
        {
            Number = number;
            TypId = typid;
        }

    }

    public class DatasInExcel
    {
        public string SpotNumberColumnInMPL { get; set; }
        public string XColumnInMPL { get; set; }
        public string YColumnInMPL { get; set; }
        public string ZColumnInMPL { get; set; }
        public string FirstSpotRow { get; set; }
        public string SheetName { get; set; }
        public string Punkttype {get; set;}
        public string SpotIndex { get; set; }

        public DatasInExcel(string spotNumberColumnInMPL, string xColumnInMPL, string yColumnInMPL, string zColumnInMPL, string firstSpotRow, string sheetName, string punktType, string spotIndex)
        {
            SpotNumberColumnInMPL = spotNumberColumnInMPL;
            XColumnInMPL = xColumnInMPL;
            YColumnInMPL = yColumnInMPL;
            ZColumnInMPL = zColumnInMPL;
            FirstSpotRow = firstSpotRow;
            SheetName = sheetName;
            Punkttype = punktType;
            SpotIndex = spotIndex;
            
        }
    }
}
