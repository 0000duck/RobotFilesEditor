using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
//using System.Windows.Forms;

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
            try
            { 
            MessageBox.Show("Select directory containing OLP files", "Select dir", MessageBoxButton.OK, MessageBoxImage.Information);
            string dirWithOLP = CommonLibrary.CommonMethods.SelectDirOrFile(true);
            if (string.IsNullOrEmpty(dirWithOLP))
                return;
            MessageBox.Show("Select SOV backup", "Select backup", MessageBoxButton.OK, MessageBoxImage.Information);
            string backupFile = CommonLibrary.CommonMethods.SelectDirOrFile(false, ".Zip", "*.zip");
            if (string.IsNullOrEmpty(backupFile))
                return;
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
            var dialogresult =  dialog.ShowDialog();
                if ((bool)dialogresult)
                {
                    MessageBox.Show("Select directory to save results.", "Select directory", MessageBoxButton.OK, MessageBoxImage.Information);
                    string dirTosSave = CommonLibrary.CommonMethods.SelectDirOrFile(true);
                    if (Path.Combine(dirTosSave, "CompareOLPBackupResult","OLP").ToLower() == dirWithOLP.ToLower() || Path.Combine(dirTosSave, "CompareOLPBackupResult", "Backup").ToLower() == Path.GetDirectoryName(backupFile).ToLower())
                    {
                        MessageBox.Show("Source and destination paths must not be the same!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    string destinationDirectory = Path.Combine(dirTosSave, "CompareOLPBackupResult");
                    string destinationDirectoryOLP = Path.Combine(dirTosSave, "CompareOLPBackupResult", "OLP");
                    string destinationDirectoryBackup = Path.Combine(dirTosSave, "CompareOLPBackupResult", "Backup");
                    if (!Directory.Exists(destinationDirectory))
                        Directory.CreateDirectory(destinationDirectory);
                    if (!Directory.Exists(destinationDirectoryOLP))
                        Directory.CreateDirectory(destinationDirectoryOLP);
                    if (!Directory.Exists(destinationDirectoryBackup))
                        Directory.CreateDirectory(destinationDirectoryBackup);

                    // write OLP
                    foreach (var file in vm.Items)
                    {
                        string currentPath = Path.Combine(destinationDirectoryOLP, file.FileInSet1.Length > file.FileInSet2.Length ? Path.GetFileNameWithoutExtension(file.FileInSet1) : Path.GetFileNameWithoutExtension(file.FileInSet2)) + ".ls";
                        var fileOnDisk = File.Create(currentPath);
                        fileOnDisk.Close();
                        if (file.UseBackup && !file.UseOLP)
                            File.WriteAllText(currentPath,file.ContentSet1);
                        else if (!file.UseBackup && file.UseOLP)
                            File.WriteAllText(currentPath, file.ContentSet2);
                        else
                            File.WriteAllText(currentPath, file.ContentSet1.Length > file.ContentSet2.Length ? file.ContentSet1 : file.ContentSet2);
                    }

                    //write backup
                    if (File.Exists(Path.Combine(destinationDirectoryBackup, Path.GetFileName(backupFile))))
                        File.Delete(Path.Combine(destinationDirectoryBackup, Path.GetFileName(backupFile)));
                    if (File.Exists(Path.Combine(destinationDirectoryBackup, "tempAscii.zip")))
                        File.Delete(Path.Combine(destinationDirectoryBackup, "tempAscii.zip"));
                    File.Copy(backupFile, Path.Combine(destinationDirectoryBackup, Path.GetFileName(backupFile)));
                    using (FileStream zipToOpen = new FileStream(Path.Combine(destinationDirectoryBackup, Path.GetFileName(backupFile)), FileMode.Open))
                    {
                        using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                        {
                            archive.Entries.FirstOrDefault(x => x.FullName.ToLower().Contains("ascii.zip")).ExtractToFile(Path.Combine(destinationDirectoryBackup, "tempAscii.zip"));
                            using (FileStream zipToOpenAscii = new FileStream(Path.Combine(destinationDirectoryBackup, "tempAscii.zip"), FileMode.Open))
                            {
                                using (ZipArchive archiveAscii = new ZipArchive(zipToOpenAscii, ZipArchiveMode.Update))
                                {
                                    foreach (var file in vm.Items)
                                    {
                                        string currentPath = Path.Combine(destinationDirectoryOLP, file.FileInSet1.Length > file.FileInSet2.Length ? Path.GetFileNameWithoutExtension(file.FileInSet1) : Path.GetFileNameWithoutExtension(file.FileInSet2)) + ".ls";
                                        string currentContent;
                                        if (file.UseBackup && !file.UseOLP)
                                            currentContent = file.ContentSet1;
                                        else if (!file.UseBackup && file.UseOLP)
                                            currentContent = file.ContentSet2;
                                        else
                                            currentContent = file.ContentSet1.Length > file.ContentSet2.Length ? file.ContentSet1 : file.ContentSet2;
                                        var entryToChange = archiveAscii.Entries.FirstOrDefault(x => x.FullName.ToLower().Contains(Path.GetFileNameWithoutExtension(currentPath).ToLower()));
                                        if (entryToChange != null)
                                        {
                                            entryToChange.Delete();
                                        }
                                        var currentFile = File.Create(Path.Combine(destinationDirectoryBackup, Path.GetFileName(currentPath)));
                                        currentFile.Close();
                                        File.WriteAllText(Path.Combine(destinationDirectoryBackup, Path.GetFileName(currentPath)), currentContent);
                                        archiveAscii.CreateEntryFromFile(Path.Combine(destinationDirectoryBackup, Path.GetFileName(currentPath)), Path.GetFileName(currentPath));
                                        File.Delete(Path.Combine(destinationDirectoryBackup, Path.GetFileName(currentPath)));
                                    }
                                }
                                zipToOpenAscii.Close();
                                archive.Entries.FirstOrDefault(x => x.FullName.ToLower().Contains("ascii.zip")).Delete();
                                archive.CreateEntryFromFile(Path.Combine(destinationDirectoryBackup, "tempAscii.zip"), "ASCII.zip");
                                File.Delete(Path.Combine(destinationDirectoryBackup, "tempAscii.zip"));
                            }
                        }
                        zipToOpen.Close();
                        var dialogSuccess = System.Windows.Forms.MessageBox.Show("Select directory to save results at " + destinationDirectory +".\r\nWould you like to open directory?", "Success", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);
                        if (dialogSuccess == System.Windows.Forms.DialogResult.Yes)
                            Process.Start(destinationDirectory);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

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
