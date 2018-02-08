using System.Collections.Generic;

namespace RobotFilesEditor
{
    public interface IOperation
    {
        #region Public
        string OperationName { get; set; }
        GlobalData.Action ActionType { get; set; }
        string DestinationFolder { get; set; }
        string SourcePath { get; set; }
        string DestinationPath { get; set; }
        int Priority { get; set; }

        #endregion Public
        void ExecuteOperation();
        List<ResultInfo> GetOperationResult();
        void ClearMemory();
    }
}
