using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RobotFilesEditor.Model.Operations.FANUC
{
    public class FanucCompareSOVToOLPMethods
    {
        string appDataFolder;

        public FanucCompareSOVToOLPMethods()
        {
            Execute();
        }

        private void Execute()
        {
            MessageBox.Show("Select directory containing OLP files", "Select dir", MessageBoxButton.OK, MessageBoxImage.Information);
            string dirWithOLP = CommonLibrary.CommonMethods.SelectDirOrFile(true);
            MessageBox.Show("Select SOV backup", "Select backup", MessageBoxButton.OK, MessageBoxImage.Information);
            string backupFile = CommonLibrary.CommonMethods.SelectDirOrFile(false, ".Zip", "*.zip");

            var appdataCurrent = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            appDataFolder = Path.Combine(appdataCurrent, "RobotFilesHarvester", "SOVBackupTemp");
            if (!Directory.Exists(Path.Combine(appdataCurrent, "RobotFilesHarvester")))
                Directory.CreateDirectory(Path.Combine(appdataCurrent, "RobotFilesHarvester"));
            if (!Directory.Exists(Path.Combine(appDataFolder)))
                Directory.CreateDirectory(Path.Combine(appDataFolder));
            else
            {
                var files = Directory.GetFiles(Path.Combine(appDataFolder)).ToList();
                files.ForEach(x => File.Delete(x));
            }

            List<string> filesInOlpList = Directory.GetFiles(dirWithOLP, "*.ls", SearchOption.AllDirectories).ToList();
            IDictionary<string, string> filesInBackup = GetFilesFromBackup(backupFile);
            IDictionary<string, string> filesInOlp = GetFilesFromOLP(filesInOlpList);
            Dialogs.CompareSOVandOLPViewModel vm = new Dialogs.CompareSOVandOLPViewModel(filesInBackup, filesInOlp);
            var dialog = new Dialogs.CompareSOVandOLPWindow(vm);
            dialog.Show();

        }

        private IDictionary<string, string> GetFilesFromOLP(List<string> filesInOlpList)
        {
            IDictionary<string, string> result = new Dictionary<string, string>();
            foreach (var file in filesInOlpList)
            {
                StreamReader reader = new StreamReader(file);
                result.Add(Path.GetFileName(file), reader.ReadToEnd());
                reader.Close();
            }
            return result;
        }

        private IDictionary<string, string> GetFilesFromBackup(string backupFile)
        {
            IDictionary<string, string> result = new Dictionary<string, string>();
            using (FileStream zipToOpen = new FileStream(Path.Combine(backupFile), FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
                {
                    if (!archive.Entries.Any(x => x.FullName.ToLower().Contains("ascii.zip")))
                    {
                        MessageBox.Show("ZIP file not valid. No ASCII.ZIP file found", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return null;
                    }
                    var asciiEntry = archive.Entries.First(x => x.FullName.ToLower().Contains("ascii.zip"));
                    asciiEntry.ExtractToFile(Path.Combine(appDataFolder, "ASCII.zip"),true);

                }
                zipToOpen.Close();
            }
            using (FileStream zipToOpen = new FileStream(Path.Combine(appDataFolder, "ASCII.zip"), FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
                {
                    foreach (var entry in archive.Entries)
                    {
                        StreamReader reader = new StreamReader(entry.Open());
                        result.Add(entry.FullName, reader.ReadToEnd());
                        reader.Close();
                    }
                }
                zipToOpen.Close();
            }
            return result;
        }
    }
}
