using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Model.Operations.FANUC
{
    public class FanucRobot
    {
        public List<string> InitialSection { get; set; }
        public List<string> ProgramSection { get; set; }
        public List<string> DeclarationSection { get; set; }

        public FanucRobot (List<string> initialSection, List<string> programSection, List<string> declarationSection)
        {
            InitialSection = initialSection;
            ProgramSection = programSection;
            DeclarationSection = declarationSection;
        }
    }
}
