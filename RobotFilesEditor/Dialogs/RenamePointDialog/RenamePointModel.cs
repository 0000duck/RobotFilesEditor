using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Dialogs.RenamePointDialog
{
    public class RenamePointModel
    {
        public string Content  { get; set; }
        public bool HasError { get; set; }

        public RenamePointModel(string content, bool hasError)
        {
            Content = content;
            HasError = hasError;
        }
    }
}
