using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CommonLibrary;

namespace RobotFilesEditor.Model.Operations
{
    public static class RetrieveBackupMethods
    {
        public static void Execute()
        {
            List<string> list = new List<string>();
            string selectedDir = CommonMethods.SelectDirOrFile(true);
            if (string.IsNullOrEmpty(selectedDir))
                return;
            List<string> foundBackups = CommonMethods.FindBackupsInDirectory(selectedDir,onlyWelding:false);
            string destinationDir = CommonMethods.SelectDirOrFile(true);
            try
            {
                foreach (var backup in foundBackups)
                {
                    string destpath = destinationDir + "\\" + Path.GetFileName(backup);
                    if (!File.Exists(destpath))
                        File.Copy(backup, destpath);
                    else
                    {
                        Thread.Sleep(1000);
                        File.Copy(backup, destpath.Replace(".zip", "_" + DateTime.Now.ToString("ddMMyyyyHHmmss")+".zip"));
                    }
                }
                MessageBox.Show("Backups copied to " + destinationDir, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                if (!Directory.Exists(destinationDir + "\\SafetyConfigFiles"))
                    Directory.CreateDirectory(destinationDir + "\\SafetyConfigFiles");
                foreach (var backup in foundBackups)
                {
                    using (ZipArchive archive = ZipFile.Open(backup, ZipArchiveMode.Read))
                    {
                        ZipArchiveEntry entry = GetMyEntry(archive.Entries, "KUKASafeRobot.config");
                        if (entry != null)
                        {
                            if (!Directory.Exists((destinationDir + "\\SafetyConfigFiles\\" + Path.GetFileNameWithoutExtension(backup))))
                                Directory.CreateDirectory(destinationDir + "\\SafetyConfigFiles\\" + Path.GetFileNameWithoutExtension(backup));
                            entry.ExtractToFile(destinationDir + "\\SafetyConfigFiles\\" + Path.GetFileNameWithoutExtension(backup) + "\\KUKASafeRobot.config");

                            entry = GetMyEntry(archive.Entries, "BrakeTestDrv.ini");
                            if (entry != null)
                                entry.ExtractToFile(destinationDir + "\\SafetyConfigFiles\\" + Path.GetFileNameWithoutExtension(backup) + "\\BrakeTestDrv.ini");
                            entry = GetMyEntry(archive.Entries, "motiondrv.ini");
                            if (entry != null)
                                entry.ExtractToFile(destinationDir + "\\SafetyConfigFiles\\" + Path.GetFileNameWithoutExtension(backup) + "\\motiondrv.ini");
                        }
                    }
                }
                MessageBox.Show("Safety Config files extracted to " + destinationDir + "\\SafetyConfigFiles\\", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
            }

        }

        private static ZipArchiveEntry GetMyEntry(ReadOnlyCollection<ZipArchiveEntry> entries, string v)
        {
            foreach (var entry in entries)
            {
                if (entry.Name.ToLower().Contains(v.ToLower()))
                    return entry;
            }
            return null;
        }
    }
}
