using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;
using RobotFilesEditor.Model.Operations.DataClass;

namespace RobotFilesEditor.Model.Operations
{
    public static class FixOfflineMethods
    {
        static string selectedPath;

        internal static void FixOffline()
        {
            try
            {
                SrcFilesForFixing files = LoadFiles();
                if (files != null)
                {
                    SaveFiles(FixOrgs(files.Orgs), "Orgs");
                    SaveFiles(FixPaths(files.ProgramFiles), "Program");
                    SaveFiles(FixPaths(files.ServiceFiles), "BMW_Std_User");
                    SaveFiles(FixPaths(files.TP), "TP\\SafeRobot");
                    CorrectCopyToConfig();
                    files = null;
                    selectedPath = "";
                    MessageBox.Show("Offline corrected succesfully", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private static void SaveFiles(IDictionary<string, string> dictionary, string path)
        {
            string savePath = selectedPath + "\\FixedFiles\\" + path;
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);
            foreach (var file in dictionary)
            {
                string correctedName = "";
                if (file.Key.ToLower() == "tip_change_auto_pr1.src")
                {
                    correctedName = "tip_change_pr1_auto.src";
                }
                else if (file.Key.ToLower() == "tip_change_auto_pr2.src")
                {
                    correctedName = "tip_change_pr2_auto.src";
                }
                else
                    correctedName = file.Key;
                File.WriteAllText(savePath + "\\" + correctedName, file.Value);
                if (File.Exists(selectedPath + "\\" + path + "\\" + file.Key.Replace(".src",".dat")))
                {
                    File.Copy((selectedPath + "\\" + path + "\\" + file.Key.Replace(".src", ".dat")), selectedPath + "\\FixedFiles\\" + path + "\\" + correctedName.Replace(".src", ".dat"));
                }
            }
        }

        private static SrcFilesForFixing LoadFiles()
        {
            string directory = "";
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.EnsurePathExists = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                directory = dialog.FileName;
                selectedPath = directory;
            }
            else
            {
                return null;
            }
            List<string> programs = Directory.GetFiles(directory + "\\Program").Where(x => x.Substring(x.Length - 4, 4).ToLower() == ".src").ToList();
            List<string> services = Directory.GetFiles(directory + "\\BMW_Std_User").Where(x => x.Substring(x.Length - 4, 4).ToLower() == ".src").ToList();
            List<string> orgs = Directory.GetFiles(directory).Where(x => x.Substring(x.Length - 4, 4).ToLower() == ".src").ToList();
            if (orgs.Count == 0)
            {
                if (Directory.Exists(directory + "\\Orgs"))
                {
                    orgs = Directory.GetFiles(directory + "\\Orgs").Where(x => x.Substring(x.Length - 4, 4).ToLower() == ".src").ToList();
                }
            }
            List<string> tp = Directory.GetFiles(directory + "\\TP\\SafeRobot").Where(x => x.Substring(x.Length - 4, 4).ToLower() == ".src").ToList();

            programs = FilterExclusions(programs, "Programs");
            services = FilterExclusions(services, "Services");
            orgs = FilterExclusions(orgs, "Orgs");
            tp = FilterExclusions(tp, "TP");

            return new SrcFilesForFixing(programs, services, orgs, tp);

        }

        private static List<string> FilterExclusions(List<string> files, string filter)
        {
            string[] itemToRemove = ConfigurationManager.AppSettings["ExcludeFromCheck" + filter].Split(',').Select(s => s.Trim()).ToArray();
            List<string> filesToExclude = new List<string>();
            foreach (string item in itemToRemove)
                foreach (string fileToExclude in files.Where(x => x.ToLower().Contains(item)))
                    filesToExclude.Add(fileToExclude);
            foreach (string fileToExclude in filesToExclude)
                files.Remove(fileToExclude);
            return files;
        }

        private static IDictionary<string, string> FixOrgs(List<string> orgs)
        {
            IDictionary<string, string> result = new Dictionary<string, string>();
            foreach (string file in orgs)
            {
                string fileContent = "";
                StreamReader reader = new StreamReader(file);
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (line.Contains("ü"))
                        line = line.Replace("ü", "u");
                    if (line.ToLower().Contains("tip_change_auto_pr1"))
                    {
                        line = line.Replace("tip_change_auto_pr1", "tip_change_pr1_auto");
                        line = line.Replace("Tip_change_auto_pr1", "tip_change_pr1_auto");
                    }
                    if (line.ToLower().Contains("tip_change_auto_pr2"))
                    {
                        line = line.Replace("tip_change_auto_pr2", "tip_change_pr2_auto");
                        line = line.Replace("Tip_change_auto_pr2", "tip_change_pr2_auto");
                    }

                    fileContent += line + "\r\n";
                }
                reader.Close();
                result.Add(Path.GetFileName(file), fileContent);
            }
            return result;
        }

        private static IDictionary<string, string> FixPaths(List<string> files)
        {
            IDictionary<string, string> result = new Dictionary<string, string>();
            foreach (string file in files)
            {
                string fileContent = RemoveHardSpaceAndHeaderTrash(file);
                fileContent = AddStartPath(fileContent, file);
                fileContent = FixCollComment(fileContent);
                fileContent = FixTipChange(fileContent, file);
                result.Add(Path.GetFileName(file), fileContent);
            }

            return result;
        }

        private static string AddStartPath(string fileContent, string file)
        {
            StringReader reader = new StringReader(fileContent);
            bool hasStartPath = false;
            while (true)
            {
                string line = reader.ReadLine();
                if (line == null)
                    break;
                if (line.ToLower().Contains("start path") && line.Contains("---"))
                {
                    hasStartPath = true;
                    break;
                }
            }
            reader.Close();
            string result = "";            
            if (!hasStartPath)
            {
                reader = new StringReader(fileContent);
                bool endOfHeaderFound = false;
                while (true)
                {
                    string line = reader.ReadLine();
                    if (line == null)
                        break;
                    if (line.ToLower().Contains("tip_change_auto_pr1") || line.ToLower().Contains("tip_change_auto_pr2"))
                    {
                        line = line.Replace("tip_change_auto_pr1", "tip_change_pr1_auto");
                        line = line.Replace("Tip_change_auto_pr1", "tip_change_pr1_auto");
                        line = line.Replace("tip_change_auto_pr2", "tip_change_pr2_auto");
                        line = line.Replace("Tip_change_auto_pr2", "tip_change_pr2_auto");
                    }
                    if (line.ToLower().Contains("aenderungsverlauf"))
                        endOfHeaderFound = true;
                    if (line.Contains(";*****") && endOfHeaderFound)
                        result += line + "\r\n\r\n\r\n;# --------- START PATH : "+Path.GetFileNameWithoutExtension(file)+" ---------\r\n\r\n";
                    else
                        result += line + "\r\n";                    
                }
                reader.Close();
            }
            else
                result = fileContent;            
            return result;
        }

        private static string RemoveHardSpaceAndHeaderTrash(string file)
        {
            string result = "";
            StreamReader reader = new StreamReader(file);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (!(line.ToLower().Contains(";#end_header") || line.ToLower().Contains(";#start_trailer")))
                    result += line.Replace(Char.ConvertFromUtf32(160), "") + "\r\n";
            }
            reader.Close();
            return result;
        }

        private static string FixCollComment(string fileContent)
        {
            string result = "";
            StringReader reader = new StringReader(fileContent);
            while (true)
            {
                string line = reader.ReadLine();
                if (line == null)
                    break;
                if (line.ToLower().Contains("kollisionsbereich"))
                {

                }
                result += line + "\r\n";
                if (line.ToString().Trim().ToLower() == "end" || line.ToString().Trim().ToLower() == "endfct")
                    break;
            }
            return result;
        }


        private static void CorrectCopyToConfig()
        {
            if (File.Exists(selectedPath+ "\\BMW_App\\copy_to_UserVariables.dat"))
            {
                StreamReader reader = new StreamReader(selectedPath + "\\BMW_App\\Copy_to_UserVariables.dat");
                string fileContent = reader.ReadToEnd();
                reader.Close();
                
                if (!Directory.Exists(selectedPath + "\\FixedFiles\\BMW_App\\"))
                    Directory.CreateDirectory(selectedPath + "\\FixedFiles\\BMW_App\\");
                File.WriteAllText(selectedPath + "\\FixedFiles\\BMW_App\\Copy_to_UserVariables.txt", fileContent);
            }
        }


        private static string FixTipChange(string fileContent,string file)
        {
            string result ="";
            StringReader reader = new StringReader(fileContent);
            while (true)
            {
                string line = reader.ReadLine();
                if (line == null)
                    break;
                if (line.ToLower().Contains("tip_change_auto_pr1") || line.ToLower().Contains("tip_change_auto_pr2"))
                {
                    line = line.Replace("tip_change_auto_pr1", "tip_change_pr1_auto");
                    line = line.Replace("Tip_change_auto_pr1", "tip_change_pr1_auto");
                    line = line.Replace("tip_change_auto_pr2", "tip_change_pr2_auto");
                    line = line.Replace("Tip_change_auto_pr2", "tip_change_pr2_auto");
                }
                if (file.ToLower().Contains("tip_change") && line.ToLower().Contains("beschreibung"))
                {
                    line = ";* Beschreibung        : Kappenwechseln";
                    FixDat(file, "Kappenwechseln");
                }
                if (file.ToLower().Contains("service_pr") && line.ToLower().Contains("beschreibung"))
                {
                    line = ";* Beschreibung        : Wartung";
                    FixDat(file, "Wartung");
                }
                result += line + "\r\n";
            }
            return result;
        }

        private static void FixDat(string file, string procedureType)
        {
            string fileContent = "";
            StreamReader reader = new StreamReader(file.Replace(".src", ".dat"));
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (line.ToLower().Contains("beschreibung"))
                    line = ";* Beschreibung        : " + procedureType;
                fileContent += line + "\r\n";
            }
            reader.Close();
            File.WriteAllText(file.Replace(".src", ".dat"), fileContent);

        }
    }
}
