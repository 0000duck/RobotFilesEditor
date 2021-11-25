using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABB_add_spaces
{
    public class RobotProgram
    {
        public string Name { get; set; }
        public int StartLine { get; set; }
        public int EndLine { get; set; }
        public string Content { get; set; }

        public RobotProgram(string name, int startLine, int endLine, string content)
        {
            Name = name;
            StartLine = startLine;
            EndLine = endLine;
            Content = content;
        }
        public RobotProgram()
        {

        }
    }
}
