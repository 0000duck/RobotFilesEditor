using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor
{
    public class FilesTree
    {
        public string NodeName { get; set; }
        public string Extension { get; set; }
        public List<string> FilesNames { get; set; }
        public List<string>FilePaths { get; set; }

        public FilesTree(string node, string extension)
        {
            if(node!=null && extension!=null)
            {
                this.NodeName = node;
                this.Extension = extension;
            }           
        }

        public FilesTree()
        {    
                  
        }
    }
}
