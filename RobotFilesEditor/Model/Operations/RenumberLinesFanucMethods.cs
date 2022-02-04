using RobotFilesEditor.Model.Operations.DataClass;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace RobotFilesEditor.Model.Operations
{
    public static class RenumberLinesFanucMethods
    {
        public static void Execute()
        {
            bool allSuccess = true;
            MessageBox.Show("Select directory containing files to renumber.", "Select directory", MessageBoxButton.OK, MessageBoxImage.Information);
            string filePath = CommonLibrary.CommonMethods.SelectDirOrFile(true);
            if (string.IsNullOrEmpty(filePath))
                return;
            List<string> filesToRenumber = Directory.GetFiles(filePath, "*.ls", SearchOption.TopDirectoryOnly).ToList();
            foreach (var file in filesToRenumber)
            {
                FanucProgramClass originalFile = GetOriginalHeaderFanuc(file);
                FanucProgramClass resultFile = new FanucProgramClass(originalFile.Header, CommonLibrary.CommonMethods.GetRenumberedBody(originalFile.Body), originalFile.Footer);
                if (!WriteFile(resultFile, file))
                    allSuccess = false;
            }
            if (allSuccess)
                MessageBox.Show("Successfully saved at " + filePath + "\\RenumberedPrograms\\", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show("Task completed with errors!", "Success", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private static bool WriteFile(FanucProgramClass inputFile, string filePath)
        {
            bool result = false;
            try
            {
                bool success = false;
                List<string> resultString = new List<string>();
                resultString.AddRange(inputFile.Header);
                resultString.AddRange(inputFile.Body);
                resultString.AddRange(inputFile.Footer);

                string currentDirectory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(currentDirectory + "\\RenumberedPrograms"))
                    Directory.CreateDirectory(currentDirectory + "\\RenumberedPrograms");
                if (File.Exists(currentDirectory + "\\RenumberedPrograms\\" + Path.GetFileName(filePath)))
                {
                    System.Windows.Forms.DialogResult dialog = System.Windows.Forms.MessageBox.Show(String.Format("File {0} exists.\r\nOverwrite?", currentDirectory + "\\RenumberedPrograms\\" + Path.GetFileName(filePath)), "Overwrite?", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);
                    if (dialog == System.Windows.Forms.DialogResult.Yes)
                    {
                        File.Delete(currentDirectory + "\\RenumberedPrograms\\" + Path.GetFileName(filePath));
                        Thread.Sleep(100);
                        File.WriteAllLines(currentDirectory + "\\RenumberedPrograms\\" + Path.GetFileName(filePath), resultString);
                        success = true;
                    }
                }
                else
                {
                    File.WriteAllLines(currentDirectory + "\\RenumberedPrograms\\" + Path.GetFileName(filePath), resultString);
                    success = true;
                }
                //if (success)
                    //MessageBox.Show("Successfully saved at " + currentDirectory + "\\RenumberedPrograms\\" + Path.GetFileName(filePath), "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                result = success;
                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return result;
            }
        }

        private static FanucProgramClass GetOriginalHeaderFanuc(string filePath)
        {
            bool headerFinished = false, bodyFinished = false;

            Regex isMNRegex = new Regex(@"^/\s*MN\s*$", RegexOptions.IgnoreCase);
            Regex isPosRegex = new Regex(@"^/\s*POS\s*$", RegexOptions.IgnoreCase);

            List<string> headerOriginal = new List<string>();
            List<string> body = new List<string>();
            List<string> footer = new List<string>();

            StreamReader reader = new StreamReader(filePath);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (isPosRegex.IsMatch(line))
                    bodyFinished = true;

                if (!headerFinished && !bodyFinished)
                    headerOriginal.Add(line);
                else if (headerFinished && !bodyFinished)
                    body.Add(line);
                else if (headerFinished && bodyFinished)
                    footer.Add(line);

                if (isMNRegex.IsMatch(line))
                    headerFinished = true;
            }
            reader.Close();

            List<string> headerReworked = new List<string>();
            foreach (var line in headerOriginal)
            {
                if (line.ToLower().Contains("line_count"))
                    headerReworked.Add(String.Format("LINE_COUNT\t= {0};", body.Count.ToString()));
                else
                    headerReworked.Add(line);
            }

            return new FanucProgramClass(headerReworked,body,footer);
        }
    }
}
