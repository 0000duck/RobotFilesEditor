using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Model.Operations.DataClass
{
    public class V8Clamp
    {
        public int Number { get; set; }
        public SortedDictionary<int, int> RetractInputs { get; set; }
        public SortedDictionary<int, int> AdvanceInputs { get; set; }
        public SortedDictionary<int, bool> UsedActuators { get; set; } 
        public int OutForClose { get; set; }
        public int OutForOpen { get; set; }

        public V8Clamp(int number, SortedDictionary<int,int> retractInputs, SortedDictionary<int, int> advanceInputs, SortedDictionary<int, bool> usedActuators, int outForClose, int outForOpen)
        {
            Number = number;
            RetractInputs = retractInputs;
            AdvanceInputs = advanceInputs;
            UsedActuators = usedActuators;
            OutForClose = outForClose;
            OutForOpen = outForOpen;
        }
    }

    public class SrcFilesForFixing
    {
        public List<string> ProgramFiles { get; set; }
        public List<string> ServiceFiles { get; set; }
        public List<string> Orgs { get; set; }
        public List<string> TP { get; set; }

        public SrcFilesForFixing(List<string> programFiles, List<string> serviceFiles, List<string> orgs, List<string> tp)
        {
            ProgramFiles = programFiles;
            ServiceFiles = serviceFiles;
            Orgs = orgs;
            TP = tp;

        }
    }
    public class DatFiles
    {
        public List<string> ProgramFiles { get; set; }
        public List<string> ServiceFiles { get; set; }
        public List<string> Orgs { get; set; }
        public List<string> TP { get; set; }

        public DatFiles(List<string> programFiles, List<string> serviceFiles, List<string> orgs, List<string> tp)
        {
            ProgramFiles = programFiles;
            ServiceFiles = serviceFiles;
            Orgs = orgs;
            TP = tp;
        }
    }
}
