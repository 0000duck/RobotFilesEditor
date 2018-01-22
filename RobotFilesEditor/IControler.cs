using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor
{
    public interface IControler
    {
        #region Configuration

        void LoadConfigurationSettingsForControler();
                
        void CreateDestinationFolders();
        List<string> GetGroupedFiles();
        void RefreshDestinationPath(string path);
        void RefreshSourcePath(string path);
    
        #endregion Configuration

        #region FilesOperations

        #region Copy
        void MoveProductionFiles();

        void MoveServicesFiles();

        #endregion

        #region CopyData

        void OlpFilesDataCopy();
        void GlobalFilesDataCopy();

        #endregion

        #region Delete
        void DeleteFiles();
        #endregion

        #endregion

        #region IsPossible
        bool IsPossibleCopyProductionFiles();
        bool IsPossibleCopyServicesFiles();
        bool IsPossibleOlpFilesDataCopy();
        bool IsPossibleGlobalFilesDataCopy();
        bool IsPossibleDeleteFiles();

        #endregion IsPossible

    }
}
