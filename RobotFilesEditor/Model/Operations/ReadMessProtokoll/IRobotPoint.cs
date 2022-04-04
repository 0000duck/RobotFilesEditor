using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Model.Operations.ReadMessProtokoll
{
    public interface IRobotPoint
    {
        string Name { get; set; }
        double Xpos { get; set; }
        double Ypos { get; set; }
        double Zpos { get; set; }
    }
}
