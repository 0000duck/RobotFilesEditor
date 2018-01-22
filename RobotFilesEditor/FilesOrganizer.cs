using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor
{
    public class FilesOrganizer
    {
        public GlobalData.Action Action { get; set; }
        public string DestinationFolder {get; set; }

        public List<string> FileExtensions { get; set; }
        public List<string> ContainsAtName { get; set; }
        public List<string> NotContainsAtName { get; set; }       

        public string RegexContain { get; set; }
        public string RegexNotContain { get; set; }


        public FilesOrganizer()
        {
            FileExtensions = new List<string>();
            ContainsAtName = new List<string>();
            NotContainsAtName = new List<string>();

            Action = GlobalData.Action.None;
            RegexContain = "";
            DestinationFolder = "";
        }
    }
}
