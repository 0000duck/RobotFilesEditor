using CommonLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace CommonLibrary
{
    public static class FilesFromServerManager
    {
        public static void CopyProjectFile(string file)
        {
            if (CheckServerConnection())
            {
                var destpath = CommonMethods.GetFilePath(file);
                var sourcePath = System.IO.Path.Combine(CommonGlobalData.serverPath, file);

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
            if (Directory.Exists(CommonGlobalData.serverPath))
                return true;
            return false;
        }

        public static void TryWriteFileToServer(string localPath, string fileName)
        {
            var fileNameComplete = System.IO.Path.Combine(CommonGlobalData.serverPath, fileName);
            if (!Directory.Exists(CommonGlobalData.serverPath))
            {
                return;
            }
            if (File.Exists(fileNameComplete))
            {
                var dialog = MessageBox.Show($"File {fileNameComplete} exists. Overwrite?", "Overwrite?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (dialog == MessageBoxResult.No)
                    return;
                File.Delete(fileNameComplete);
            }
            File.Copy(localPath, fileNameComplete);
        }
    }
}
