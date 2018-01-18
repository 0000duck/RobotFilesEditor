using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor
{

    public class KukaKrc4: Controler
    {
        private string _destinationDirectory;

        public KukaKrc4()
        {
            LoadConfigurationSettingsForControler();
        }
        public void LoadConfigurationSettingsForControler()
        {
            Controler controler = base.LoadConfigurationSettingsForControler("KRC4");

            _productionCopiedFiles = controler._productionCopiedFiles;
            _serviceCopiedFiles = controler._serviceCopiedFiles;
            _copiedOlpDataFiles = controler._copiedOlpDataFiles;
            _copiedGlobalDataFiles = controler._copiedGlobalDataFiles;
            _removingDataFiles = controler._removingDataFiles;
        }

        public List<FilesTree> GetFilesExtensions()
        {
            List<FilesTree> extensions = new List<FilesTree>();

            foreach (var pcf in _productionCopiedFiles?.Extension)
            {
                extensions.Add(new FilesTree("Production Copied Files", pcf));
            }

            foreach (var scf in _serviceCopiedFiles?.Extension)
            {
                extensions.Add(new FilesTree("Service Copied Files", scf));
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
                extensions.Add(new FilesTree("Removing Files", rdf));
            }

            return extensions;
        }

        public void CreateDestinationFolders(string path)
        {
            if (_productionCopiedFiles?.Destination != null)
            {
                Directory.CreateDirectory(Path.Combine(path, _destinationDirectory, _productionCopiedFiles.Destination));
            }

            if (_serviceCopiedFiles?.Destination != null)
            {
                Directory.CreateDirectory(Path.Combine(path, _destinationDirectory, _productionCopiedFiles.Destination));
            }

            if (_serviceCopiedFiles?.Destination != null)
            {
                Directory.CreateDirectory(Path.Combine(path, _destinationDirectory, _productionCopiedFiles.Destination));
            }

            if (_copiedOlpDataFiles?.Destination != null)
            {
                Directory.CreateDirectory(Path.Combine(path, _destinationDirectory, _productionCopiedFiles.Destination));
            }

            if (_copiedGlobalDataFiles?.Destination != null)
            {
                Directory.CreateDirectory(Path.Combine(path, _destinationDirectory, _productionCopiedFiles.Destination));
            }
        }

    }
}
