using CommonLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RobotFilesEditor.ViewModel.Helper
{
    public static class FilesFromServerManager
    {
        private const string serverPath = @"\\alfa\Dzialy\automatyka\Robotyka\BMW\02_Tools\RobotFilesHarvester\"; 
        public static void CopyProjectFile(string file)
        {
            if (CheckServerConnection())
            {
                var destpath = CommonMethods.GetFilePath(file);
                var sourcePath = Path.Combine(serverPath, file);

                if (File.Exists(sourcePath))
                {
                    if (File.Exists(destpath))
                    {
                        var sourcePathContent = string.Empty;
                        var destPathContent = string.Empty;
                        using (StreamReader reader = new StreamReader(sourcePath))
                        {
                            sourcePathContent = reader.ReadToEnd();
                        }
                        using (StreamReader reader = new StreamReader(destpath))
                        {
                            destPathContent = reader.ReadToEnd();
                        }
                        if (!destPathContent.Equals(sourcePathContent))
                        {
                            var dialog = MessageBox.Show($"File {sourcePath} is different than {destpath}. Would you like to overwrite local file with file from server?", "Overwrite?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                            if (dialog == MessageBoxResult.Yes)
                            {
                                File.Delete(destpath);
                                File.Copy(sourcePath, destpath);
                            }
                        }
                    }
                }
            }
        }

        private static bool CheckServerConnection()
        {
            if (Directory.Exists(serverPath))
                return true;
            return false;
        }
    }
}
