using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor
{
    public class FileTreeNode
    {
        public string Node { get; set; }
        public string Extension { get; set; }

        public FileTreeNode(string node, string extension)
        {
            if(node!=null && extension!=null)
            {
                this.Node = node;
                this.Extension = extension;
            }           
        }
    }
}
