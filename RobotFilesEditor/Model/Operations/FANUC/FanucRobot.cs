using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Model.Operations.FANUC
{
    public class FanucRobotPath
    {
        public List<string> InitialSection { get; set; }
        public List<string> ProgramSection { get; set; }
        public List<string> DeclarationSection { get; set; }

        public FanucRobotPath (List<string> initialSection, List<string> programSection, List<string> declarationSection)
        {
            InitialSection = initialSection;
            ProgramSection = programSection;
            DeclarationSection = declarationSection;
        }

        public interface RobotApps
        {
            bool isSpotWelding { get; set; }
            bool isGluing { get; set; }
            bool isLaser { get; set; }
            bool isHandling { get; set; }
            bool isRivet { get; set; }
            bool isFLS { get; set; }       
            bool isDocking { get; set; }
        }

        public class FanucRobotApps : RobotApps
        {
            public bool isSpotWelding { get; set; }
            public bool isGluing { get; set; }
            public bool isLaser { get; set; }
            public bool isHandling { get; set; }
            public bool isRivet { get; set; }
            public bool isFLS { get; set; }
            public bool isDocking { get; set; }
        }
    }
}
