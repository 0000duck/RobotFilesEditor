using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RobotFilesEditor
{
    public class KukaKrc2: Controler
    {
        public string DestinationPath
        {
            get;
            set;
        }

        public string SourcePath
        {
            get;
            set;
        }

        private string _destinationDirectory;
        private string _destinationFolder;

        private string _destinationPath;
        private string _sourcePath;


        public KukaKrc2(string sourcePath, string destinationPath)
        {
            string folder= @"KRC2" + DateTime.Now.ToString("_yyyyMMdd");

            if (destinationPath.Contains(folder))
            {
                _destinationPath = destinationPath;
            }
            else
            {
                _destinationPath = Path.Combine(destinationPath, folder);
            }                
        }

        #region Configuration

        public void LoadConfigurationSettingsForControler()
        {
            Controler controler = base.LoadConfigurationSettingsForControler("KRC2");

            _productionCopiedFiles=controler._productionCopiedFiles;
            _serviceCopiedFiles = controler._serviceCopiedFiles;
            _copiedOlpDataFiles = controler._copiedOlpDataFiles;
            _copiedGlobalDataFiles = controler._copiedGlobalDataFiles;
            _removingDataFiles = controler._removingDataFiles;
        }

        public List<string>GetGroupedFiles()
        {
            List<string> gropuedFiles = new List<string>();
            List<string> extensions = new List<string>();

            extensions.AddRange(_productionCopiedFiles?.Extension);   
            extensions.AddRange(_serviceCopiedFiles?.Extension);
            extensions.AddRange(_copiedOlpDataFiles?.Extension);
            extensions.AddRange(_copiedGlobalDataFiles?.Extension);
            extensions.AddRange(_removingDataFiles?.Extension);
           
            extensions = extensions.Distinct().ToList();

            foreach(string e in extensions)
            {
                List<string>files = Directory.GetFiles(_sourcePath, $"*{e}").ToList();

                files.ForEach(x => Path.GetFileName(x));
                gropuedFiles.AddRange(files);
            }

            gropuedFiles.Distinct();

            return gropuedFiles;
        }

        public List<FilesTree> GetFilesExtensions()
        {
            List<FilesTree> extensions = new List<FilesTree>();          

            foreach(var pcf in _productionCopiedFiles?.Extension)
            {
                extensions.Add(new FilesTree("Production Copied Files", pcf));
            }

            foreach (var scf in _serviceCopiedFiles?.Extension)
            {
                extensions.Add(new FilesTree("Service Copied Files",scf));
            }

            foreach (var codf in _copiedOlpDataFiles?.Extension)
            {
                extensions.Add(new FilesTree("Copied Data OLP Files", codf));
            }

            foreach (var cgdf in _copiedGlobalDataFiles?.Extension)
            {
                extensions.Add(new FilesTree("Copied Data Global Files", cgdf));
            }
            
            foreach (var rdf in _removingDataFiles?.Extension)
            {
                extensions.Add(new FilesTree("Removing Files",rdf));
            }

            return extensions;
        }

        public void CreateDestinationFolders(string path)
        {
            _destinationDirectory = Path.Combine(path, _destinationFolder);

            if (Directory.Exists(_destinationDirectory) == false)
            {
                var result = Directory.CreateDirectory(_destinationDirectory);
            }

            if (_productionCopiedFiles?.Destination != null)
            {
                Directory.CreateDirectory(Path.Combine(_destinationDirectory, _productionCopiedFiles.Destination));
            }

            if (_serviceCopiedFiles?.Destination != null)
            {
                Directory.CreateDirectory(Path.Combine(_destinationDirectory, _productionCopiedFiles.Destination));
            }

            if (_serviceCopiedFiles?.Destination != null)
            {
                Directory.CreateDirectory(Path.Combine(_destinationDirectory, _productionCopiedFiles.Destination));
            }

            if (_copiedOlpDataFiles?.Destination != null)
            {
                Directory.CreateDirectory(Path.Combine(_destinationDirectory, _productionCopiedFiles.Destination));
            }

            if (_copiedGlobalDataFiles?.Destination != null)
            {
                Directory.CreateDirectory(Path.Combine(_destinationDirectory, _productionCopiedFiles.Destination));
            }
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



        #endregion Configuration


        #region CopyFiles
        public void MoveProductionFiles(string source)
        {
            string []getedFiles= Directory.GetFiles(source);
            List<string> editingFiles = new List<string>();

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

                string destDir = Path.Combine(_destinationDirectory, _productionCopiedFiles.Destination);

                if (Directory.Exists(destDir) == false)
                {
                    Directory.CreateDirectory(destDir);
                }

                foreach (string ef in editingFiles)
                {
                    File.Copy(ef, Path.Combine(destDir, Path.GetFileName(ef)));
                }
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);               
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
                    if(System.Text.RegularExpressions.Regex.IsMatch(Path.GetFileName(f), sPattern))
                    {
                        if (_serviceCopiedFiles.Extension.Exists(x => f.Contains(x)) && _serviceCopiedFiles.Containing.Exists(x => f.Contains(x)))//uściślić tylko do Glue_070ZG01_G16.src i Glue_070ZG01_G16.dat
                        {
                            editingFiles.Add(f);
                        }
                    }                   
                }

                string destDir = Path.Combine(_destinationDirectory, _serviceCopiedFiles.Destination);

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
