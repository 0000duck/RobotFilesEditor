using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor
{
    public class Operations
    {
        public List<DataOperation> DataOperations;
        public List<FileOperaion> FilesOperations;

        public void FollowOperation(string operation)
        {            
            if (FilesOperations.Exists(x=>x.OperationName==operation))
            {
                FileOperaion action = FilesOperations.FirstOrDefault(x => x.OperationName==operation);
                action.FollowOperation();
            }
            else
            {
                DataOperation action = DataOperations.FirstOrDefault(x => x.OperationName == operation);
                action.FollowOperation();
            }
        }
    }
}
