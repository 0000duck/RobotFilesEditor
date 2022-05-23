using RobotFilesEditor.Model.Operations.DataClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Model.Operations.BackupSyntaxValidation
{
    interface IBackupSyntaxValidator
    {
        List<SovBackupsPreparationResult> ErrorsFound { get; set; }
    }

    public class FaultSyntaxValidator : IBackupSyntaxValidator
    {
        public List<SovBackupsPreparationResult> ErrorsFound { get; set; }

        public FaultSyntaxValidator()
        {
            throw new Exception("Incorrect robot type");
        }
    }
}
