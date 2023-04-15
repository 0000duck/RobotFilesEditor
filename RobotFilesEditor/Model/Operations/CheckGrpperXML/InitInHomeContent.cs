using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Model.Operations.CheckGrpperXML
{
    internal class InitInHomeContent
    {
        public GrpSettingInHome Home1 { get; set; }
        public GrpSettingInHome Home2 { get; set; }

        public InitInHomeContent()
        {
            Home1 = new GrpSettingInHome();
            Home2 = new GrpSettingInHome();
        }
    }


    internal class GrpSettingInHome
    {
        public List<int> Clamps { get; set; }
        public List<int> Sensors { get; set; }

        public GrpSettingInHome()
        {
            Clamps = new List<int>();
            Sensors = new List<int>();
        }
    }
}
