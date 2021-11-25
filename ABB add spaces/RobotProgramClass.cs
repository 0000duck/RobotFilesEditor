using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABB_add_spaces
{
    public class RobotProgramClass
    {
        public string Progname { get; set; }
        IDictionary<int, List<string>> Types { get; set; }
        public string ProgString { get; set; }

        public RobotProgramClass()
        {

        }
    }
}