using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor
{
    interface IFileOperations
    {
        bool CopyFile(string operation);
        bool MoveFile(string operation);
        bool RemoveFile(string operation);
    }
}
