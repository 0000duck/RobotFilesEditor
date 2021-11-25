using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Model.Operations.DataClass
{
    public class FanucProgramClass
    {
        public List<string> Header { get; set; }
        public List<string> Body { get; set; }
        public List<string> Footer { get; set; }

        public FanucProgramClass(List<string> header, List<string> body, List<string> footer)
        {
            Header = header;
            Body = body;
            Footer = footer;
        }
    }
}
