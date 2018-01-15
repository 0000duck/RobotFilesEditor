using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor
{
    public class Controler
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

        protected void   MoveFilesToFolder(string path)
        {
            string[] filesInFolder = Directory.GetFiles(path);

        }
    }
}
