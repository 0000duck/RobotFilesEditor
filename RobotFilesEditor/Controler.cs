using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor
{
    public class Controler: IControler
    {
        public FilesOrganizer _productionCopiedFiles;
        public FilesOrganizer _serviceCopiedFiles;
        public FilesOrganizer _copiedOlpDataFiles;
        public FilesOrganizer _copiedGlobalDataFiles;
        public FilesOrganizer _removingDataFiles;    
        
        public Controler()
        {
            _productionCopiedFiles = new FilesOrganizer();
            _serviceCopiedFiles = new FilesOrganizer();

            _copiedOlpDataFiles = new FilesOrganizer();
            _copiedGlobalDataFiles = new FilesOrganizer();

            _removingDataFiles = new FilesOrganizer();
        }

        public Controler LoadConfigurationSettingsForControler(string controlerType)
        {
            var fs = new Serializer.FilesSerialization();
            return fs.GetControlerConfigration(controlerType);
        }

        public void LoadConfigurationSettingsForControler()
        {
            throw new NotImplementedException();
        }

        public List<FilesTree> GetFilesExtensions()
        {
            throw new NotImplementedException();
        }

        public void MoveProductionFiles(string source)
        {
            throw new NotImplementedException();
        }

        public void MoveServicesFiles(string source)
        {
            throw new NotImplementedException();
        }

        public void CreateDestinationFolders(string path)
        {
            throw new NotImplementedException();
        }

        public void RefreshDestinationPath()
        {
            throw new NotImplementedException();
        }

        public void RefreshSourcePath()
        {
            throw new NotImplementedException();
        }

        public bool CheckDestinationPath()
        {
            throw new NotImplementedException();
        }

        public void OlpFilesDataCopy()
        {
            throw new NotImplementedException();
        }

        public void GlobalFilesDataCopy()
        {
            throw new NotImplementedException();
        }

        public void DeleteFiles()
        {
            throw new NotImplementedException();
        }

        public void CreateDestinationFolders()
        {
            throw new NotImplementedException();
        }

        public List<string> GetGroupedFiles()
        {
            throw new NotImplementedException();
        }

        public void RefreshDestinationPath(string path)
        {
            throw new NotImplementedException();
        }

        public void RefreshSourcePath(string path)
        {
            throw new NotImplementedException();
        }

        public void MoveProductionFiles()
        {
            throw new NotImplementedException();
        }
    }
}
