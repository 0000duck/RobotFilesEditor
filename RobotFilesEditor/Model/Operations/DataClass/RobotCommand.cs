using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Model.Operations.DataClass
{
    public class RobotCommand
    {
        public string Content { get; set; }
        public KukaCommandClass CommandType { get; private set;}
        public bool AddSpaceBefore { get; set; }

        public RobotCommand(string content)
        {
            Content = content;
            CommandType = new KukaCommandClass(content);
            AddSpaceBefore = false;
        }
    }
}
