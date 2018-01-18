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

        void MoveServicesFiles(string path);

        #endregion

        #region CopyData

        void OlpFilesDataCopy();
        void GlobalFilesDataCopy();

        #endregion

        #region Delete
        void DeleteFiles();
        #endregion

        #endregion




    }
}
