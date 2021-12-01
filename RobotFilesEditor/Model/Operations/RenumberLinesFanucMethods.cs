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
            MessageBox.Show("Select .ls file for renumber.", "Select file", MessageBoxButton.OK, MessageBoxImage.Information);
            string filePath = CommonLibrary.CommonMethods.SelectDirOrFile(false, filter1Descr: "LS file", filter1: "*.ls");
            if (string.IsNullOrEmpty(filePath))
                return;
            FanucProgramClass originalFile = GetOriginalHeaderFanuc(filePath);
            FanucProgramClass resultFile = new FanucProgramClass(originalFile.Header,GetRenumberedBody(originalFile.Body),originalFile.Footer);
            WriteFile(resultFile,filePath);

        }

        private static void WriteFile(FanucProgramClass inputFile, string filePath)
        {
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
                if (success)
                    MessageBox.Show("Successfully saved at " + currentDirectory + "\\RenumberedPrograms\\" + Path.GetFileName(filePath), "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static List<string> GetRenumberedBody(List<string> body)
        {
            Regex lineNumberRegex = new Regex(@"(?<=^\s*)\d+\s*(?=\:)", RegexOptions.IgnoreCase);
            List<string> result = new List<string>();
            int counter = 1;
            foreach (string line in body)
            {
                string tempLine = line.TrimStart();
                if (string.IsNullOrEmpty(line.Trim().Replace(" ","")))
                {
                    result.Add(AddSpaces(counter.ToString()) + ":   ;");
                }
                else
                    result.Add(lineNumberRegex.Replace(tempLine, AddSpaces(counter.ToString())));
                counter++;
            }

            return result;
        }

        private static string AddSpaces(string numberString)
        {
            string tempstring = string.Empty;
            for (int i = numberString.Length; i < 4; i++)
            {
                tempstring += " ";
            }
            return tempstring + numberString;

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
