using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RobotFilesEditor
{
    public class KukaKrc2: Controler
    {
        public KukaKrc2()
        {        
        }

        private void LoadConfigurationSettingsForControler()
        {}

        public List<FileTreeNode> GetFilesExtensions()
        {
            List<FileTreeNode> extensions = new List<FileTreeNode>();          

            foreach(var pcf in _productionCopiedFiles?.Extension)
            {
                extensions.Add(new FileTreeNode("Production Copied Files", pcf));
            }

            foreach (var scf in _serviceCopiedFiles?.Extension)
            {
                extensions.Add(new FileTreeNode("Service Copied Files",scf));
            }

            foreach (var codf in _copiedOlpDataFiles?.Extension)
            {
                extensions.Add(new FileTreeNode("Copied Data OLP Files", codf));
            }

            foreach (var cgdf in _copiedGlobalDataFiles?.Extension)
            {
                extensions.Add(new FileTreeNode("Copied Data Global Files", cgdf));
            }
            
            foreach (var rdf in _removingDataFiles?.Extension)
            {
                extensions.Add(new FileTreeNode("Removing Files",rdf));
            }

            return extensions;
        }


        public void MoveProductionFiles(string path)
        {

        }

        public void MoveServicesFiles(string path)
        {

        }


    }
}
