using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor
{
    interface IFileDataOperations
    {
        bool CopyData(string operation);
        bool CutData(string operation);
        bool CreateNewFileFromData(string operation);
    }
}
