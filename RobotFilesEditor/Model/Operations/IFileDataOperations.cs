using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor
{
    interface IFileDataOperations
    {
        bool CopyData();
        bool CutData();
        bool CreateNewFileFromData();
    }
}
