using System;
using System.IO;

namespace RobotFilesEditor
{
    public class FileDeseralization: Serialization
    {
        public void SaveNewPaths(string sourcePath, string destinationPath)
        {
            bool needChange = false;

            try
            {
                XmlControlersConfiguration xmlConfiguration = ReadAplicationConfiguration();

                if (xmlConfiguration.DestinationPath != destinationPath && Directory.Exists(destinationPath))
                {
                    xmlConfiguration.DestinationPath = destinationPath;
                    needChange = true;
                }

                if (xmlConfiguration.SourcePath != sourcePath && Directory.Exists(sourcePath))
                {
                    xmlConfiguration.SourcePath = sourcePath;
                    needChange = true;
                }

                if (needChange)
                {
                    SaveAplicationConfiguration(xmlConfiguration);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }
    }
}
