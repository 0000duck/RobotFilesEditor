using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Model.Operations.DataClass
{
    public class SovBackupsPreparationResult
    {
        public string ContentToDisplay { get; set; }
        public GlobalData.SovLogContentInfoTypes InfoType { get; set; }

        public SovBackupsPreparationResult(string contentToDisplay, GlobalData.SovLogContentInfoTypes infoType)
        {
            ContentToDisplay = contentToDisplay;
            InfoType = infoType;
        }
    }
}
