using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RobotFilesEditor.Model.Operations.DataClass
{
    class SasFillerClass
    {
    }

    public class OrgFromBackup
    {
        public int Number { get; set; }
        public string OrgName { get; set; }
        public List<IJobOrAnyjob> Jobs { get; set; }
        public int StartHome { get; set; }

        public OrgFromBackup()
        {
            Jobs = new List<IJobOrAnyjob>();
        }
    }

    public class JobsInOrg : IJobOrAnyjob
    {
        public int JobNum { get; set; }
        public bool TypNumReq { get; set; }
        public List<int> TypesUsed { get; set; }
        public string Name { get; set; }
        public List<int> AssignedToAreas { get; set; }

        public JobsInOrg(int jobNum, string name, bool typnumreq, List<int> typesUsed)
        {
            JobNum = jobNum;
            Name = name;
            TypNumReq = typnumreq;
            TypesUsed = typesUsed;
        }
        public JobsInOrg()
        {

        }
    }

    public class AnyJobInOrg : IJobOrAnyjob
    {
        public int JobNum { get; set; }
        public bool TypNumReq { get; set; }
        public List<int> TypesUsed { get; set; }
        public string Name { get; set; }
        public List<int> AssignedToAreas { get; set; }
        public List<int> JobsInAnyjob { get; set; }
        public IDictionary<int, string> JobsAndDescriptions { get; set; }
        public AnyJobInOrg(int jobNum, string name, bool typnumreq, List<int> typesUsed, List<int> jobsInAnyJob, IDictionary<int, string> jobsAndDescriptions)
        {
            JobNum = jobNum;
            Name = name;
            TypNumReq = typnumreq;
            TypesUsed = typesUsed;
            JobsInAnyjob = jobsInAnyJob;
            JobsAndDescriptions = jobsAndDescriptions;
        }
        public AnyJobInOrg()
        {

        }
    }

    public interface IJobOrAnyjob
    {
        int JobNum { get; set; }
        bool TypNumReq { get; set; }
        List<int> TypesUsed { get; set; }
        string Name { get; set; }
        List<int> AssignedToAreas { get; set; }
    }

    public class RobotFromSas
    {
        public string RobotName { get; set; }
        public string SG { get; set; }
        public int Station { get; set; }
        public int Number { get; set; }
        public XElement RobotXML { get; set; }
        public string BackupName { get; set; }
        public SortedDictionary<int, OrgFromBackup> OrgsFromBackup { get; set; }
        public bool IsSafeRobot { get; set; }
        public int NrOfHomes { get; set; }
        public bool? IsKRC4 { get; set; }

        public RobotFromSas(string robotName, string sg, XElement robotxml, int station, int number, bool? iskrc4)
        {
            RobotName = robotName;
            SG = sg;
            RobotXML = robotxml;
            Station = station;
            Number = number;
            IsKRC4 = iskrc4;
        }
    }

    public class RobotAssingmentData
    {
        public RobotFromSas RobotsFromSas { get; set; }
        public string RobotsFromBackups { get; set; }
    }

    public class RobotSafetyAndHomes
    {
        public bool IsSafe { get; set; }
        public int NrOfHomes { get; set; }
    }

    public class AreaClass
    {
        public int Number { get; set; }
        public List<string> UsedInPaths { get; set; }
        public List<string> Descriptions { get; set; }

        public AreaClass(int number)
        {
            Number = number;
            UsedInPaths = new List<string>();
            Descriptions = new List<string>();
        }
    }

    public class SwitchInstruction
    {
        public int Id { get; set;}
        public int Level { get; set; }
        public string SwitchVariable { get; set; }
        public List<int> Cases { get; set; }
        public List<SwitchInstruction> InnerSwitches { get; set; }
        public int CaseOfParent { get; set; }
        public int MyLastCase { get; set; }

        public SwitchInstruction(int id, int level)
        {
            Id = id;
            Level = level;
            Cases = new List<int>();
        }
    }

    public class SwitchLevels
    {
        public int LastId { get; set; }
        public List<int> Ids { get; set; }
    }



}
