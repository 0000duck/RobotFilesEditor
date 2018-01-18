using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor
{

    public class KukaKrc4 : IControler
    {
        private FilesOrganizer _productionCopiedFiles;
        private FilesOrganizer _serviceCopiedFiles;
        private FilesOrganizer _copiedOlpDataFiles;
        private FilesOrganizer _copiedGlobalDataFiles;
        private FilesOrganizer _removingDataFiles;
        private string _destinationPath;
        private string _sourcePath;
        private string _controlerFolder;

        public KukaKrc4(string sourcePath, string destinationPath)
        {
            _controlerFolder = @"KRC4" + DateTime.Now.ToString("_yyyyMMdd");

            if (destinationPath.Contains(_controlerFolder))
            {
                _destinationPath = destinationPath;
            }
            else
            {
                _destinationPath = Path.Combine(destinationPath, _controlerFolder);
                Directory.CreateDirectory(_destinationPath);
            }

            _sourcePath = sourcePath;
        }

        #region Configuration

        public void LoadConfigurationSettingsForControler()
        {
            var fs = new Serializer.FilesSerialization();
            Controler controler = fs.GetControlerConfigration("KRC4");

            _productionCopiedFiles = controler._productionCopiedFiles;
            _serviceCopiedFiles = controler._serviceCopiedFiles;
            _copiedOlpDataFiles = controler._copiedOlpDataFiles;
            _copiedGlobalDataFiles = controler._copiedGlobalDataFiles;
            _removingDataFiles = controler._removingDataFiles;
        }

        public List<string> GetGroupedFiles()
        {
            List<string> gropuedFiles = new List<string>();
            List<string> extensions = new List<string>();

            extensions.AddRange(_productionCopiedFiles?.Extension);
            extensions.AddRange(_serviceCopiedFiles?.Extension);
            extensions.AddRange(_copiedOlpDataFiles?.Extension);
            extensions.AddRange(_copiedGlobalDataFiles?.Extension);
            extensions.AddRange(_removingDataFiles?.Extension);

            extensions = extensions.Distinct().ToList();

            foreach (string e in extensions)
            {
                List<string> files = Directory.GetFiles(_sourcePath, $"*{e}").ToList();

                files = files.ConvertAll(x => x = Path.GetFileName(x));

                gropuedFiles.Add(e);
                gropuedFiles.AddRange(files.OrderBy(x => x));
                gropuedFiles.Add("");
            }

            gropuedFiles.Distinct();

            return gropuedFiles;
        }

        public void CreateDestinationFolders()
        {
            if (Directory.Exists(_destinationPath) == false)
            {
                var result = Directory.CreateDirectory(_destinationPath);
            }

            if (_productionCopiedFiles?.Destination != null)
            {
                Directory.CreateDirectory(Path.Combine(_destinationPath, _productionCopiedFiles.Destination));
            }

            if (_serviceCopiedFiles?.Destination != null)
            {
                Directory.CreateDirectory(Path.Combine(_destinationPath, _productionCopiedFiles.Destination));
            }

            if (_serviceCopiedFiles?.Destination != null)
            {
                Directory.CreateDirectory(Path.Combine(_destinationPath, _productionCopiedFiles.Destination));
            }

            if (_copiedOlpDataFiles?.Destination != null)
            {
                Directory.CreateDirectory(Path.Combine(_destinationPath, _productionCopiedFiles.Destination));
            }

            if (_copiedGlobalDataFiles?.Destination != null)
            {
                Directory.CreateDirectory(Path.Combine(_destinationPath, _productionCopiedFiles.Destination));
            }
        }

        public void RefreshDestinationPath(string path)
        {
            if (Directory.Exists(path))
            {
                if (path.Contains(_controlerFolder))
                {
                    _destinationPath = path;
                }
                else
                {
                    _destinationPath = Path.Combine(path, _controlerFolder);
                }
            }
        }

        public void RefreshSourcePath(string path)
        {
            if (Directory.Exists(path))
            {
                _sourcePath = path;
            }

        }

        #endregion Configuration


        #region CopyFiles
        public void MoveProductionFiles()
        {
            string[] getedFiles = Directory.GetFiles(_sourcePath);
            List<string> editingFiles = new List<string>();

            if (Directory.Exists(_sourcePath) && Directory.Exists(_destinationPath))
            {
                try
                {
                    if (_productionCopiedFiles.Destination == null)
                        throw new NullReferenceException("Production Copied Files destination directory not found!");

                    foreach (string f in getedFiles)
                    {
                        if (_productionCopiedFiles.Extension.Exists(x => f.Contains(x)) && _productionCopiedFiles.Containing.Exists(x => f.Contains(x)))//uściślić tylko do Glue_070ZG01_G16.src i Glue_070ZG01_G16.dat
                        {
                            editingFiles.Add(f);
                        }
                    }

                    string destDir = Path.Combine(_destinationPath, _productionCopiedFiles.Destination);

                    if (Directory.Exists(destDir) == false)
                    {
                        Directory.CreateDirectory(destDir);
                    }

                    foreach (string ef in editingFiles)
                    {
                        File.Copy(ef, Path.Combine(destDir, Path.GetFileName(ef)));
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }

        }

        public void MoveServicesFiles(string source)
        {
            string[] getedFiles = Directory.GetFiles(source);
            List<string> editingFiles = new List<string>();
            string sPattern = @"^A\d{2}_*\.*";

            try
            {
                if (_serviceCopiedFiles.Destination == null)
                    throw new NullReferenceException("Services Copied Files destination directory not found!");

                foreach (string f in getedFiles)
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(Path.GetFileName(f), sPattern))
                    {
                        if (_serviceCopiedFiles.Extension.Exists(x => Path.GetFileName(f).Contains(x)) && _serviceCopiedFiles.Containing.Exists(x => Path.GetFileName(f).Contains(x)))//uściślić tylko do Glue_070ZG01_G16.src i Glue_070ZG01_G16.dat
                        {
                            editingFiles.Add(f);
                        }
                    }
                }

                string destDir = Path.Combine(_destinationPath, _serviceCopiedFiles.Destination);

                if (Directory.Exists(destDir) == false)
                {
                    Directory.CreateDirectory(destDir);
                }

                foreach (string ef in editingFiles)
                {
                    File.Copy(ef, Path.Combine(destDir, Path.GetFileName(ef)));
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        #endregion CopyFiles


        #region CopyData

        public void OlpFilesDataCopy()
        {
            throw new NotImplementedException();
        }

        public void GlobalFilesDataCopy()
        {
            throw new NotImplementedException();
        }

        #endregion CopyData

        #region DeleteFiles 
        public void DeleteFiles()
        {
            throw new NotImplementedException();
        }
        #endregion DeleteFiles
    }
}
