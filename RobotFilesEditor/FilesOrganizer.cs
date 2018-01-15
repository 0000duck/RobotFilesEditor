using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor
{
    public class FilesOrganizer
    {
        public List<string> Extension { get; set; }
        public List<string> Containing { get; set; }
        public string Destination { get; set; }


        public FilesOrganizer()
        {
            Extension = new List<string>();
            Containing = new List<string>();
        }
    }
}
