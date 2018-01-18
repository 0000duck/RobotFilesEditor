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

        List<FilesTree> GetFilesExtensions();
        
        void CreateDestinationFolders(string path);

        void RefreshDestinationPath();
        void RefreshSourcePath();
        bool CheckDestinationPath();

        #endregion Configuration

        #region FilesOperations

        #region Copy
        void MoveProductionFiles(string source);

        void MoveServicesFiles(string source);

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
