using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor
{
    interface IFileOperations
    {
        void CopyFiles();
        bool MoveFile();
        bool RemoveFile();
    }
}
