using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace RobotFilesEditor.Model.Operations
{
    public static class SpotToPTPMethods
    {
        static string currentDir = "";

        internal static void ChangeToPTP()
        {
            IDictionary<string, string> files = LoadFiles();
            if (files != null)
            {
                IDictionary<string, List<string>> resultSrcFiles = SrcValidator.DivideToFolds(files);
                IDictionary<string, List<string>> modifiedFiles = new Dictionary<string, List<string>>();
                foreach (var file in resultSrcFiles)
                    modifiedFiles.Add(Path.GetFileNameWithoutExtension(file.Key), ConvertToPTP(file.Value));
                SaveFiles(modifiedFiles);
            }
        }

        private static IDictionary<string, string> LoadFiles()
        {
            string directory = "";
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.EnsurePathExists = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                directory = dialog.FileName;
                currentDir = directory;
            }
            else
            {
                return null;
            }
            List<string> files = Directory.GetFiles(directory).Where(x => x.Substring(x.Length - 4, 4).ToLower() == ".src").ToList();
            IDictionary<string, string> result = new Dictionary<string, string>();
            foreach (var file in files)
            {
                StreamReader reader = new StreamReader(file);
                string fileContent = reader.ReadToEnd();
                result.Add(Path.GetFileName(file), fileContent);
                reader.Close();
            }
            return result;

        }


        private static List<string> ConvertToPTP(List<string> file)
        {
            List<string> result = new List<string>();
            foreach (string command in file)
            {
                string currentCommand = "";
                if (command.ToLower().Contains("advspot") && command.ToLower().Contains("spot_exe"))
                {
                    Regex regexPDAT = new Regex(@"(?<=PDAT_ACT\s*=\s*)[a-zA-Z0-9_]*", RegexOptions.IgnoreCase);
                    Regex regexFDAT = new Regex(@"(?<=FDAT_ACT\s*=\s*)[a-zA-Z0-9_]*", RegexOptions.IgnoreCase);
                    Regex regexPTP = new Regex(@"(?<=PTP\s+)X[a-zA-Z0-9_]*", RegexOptions.IgnoreCase);
                    Regex regexTool = new Regex(@"(?<=Tool\s*=\s*)\d+", RegexOptions.IgnoreCase);
                    Regex regexBase = new Regex(@"(?<=Base\s*=\s*)\d+", RegexOptions.IgnoreCase);
                    Regex regexPos = new Regex(@"(?<=Pos\s*=\s*)\d+", RegexOptions.IgnoreCase);
                    string pdat = regexPDAT.Match(command).ToString();
                    string fdat = regexFDAT.Match(command).ToString();
                    string ptp = regexPTP.Match(command).ToString();
                    string tool = regexTool.Match(command).ToString();
                    string baseNum = regexBase.Match(command).ToString();
                    string pos = regexPos.Match(command).ToString();

                    currentCommand = String.Join(Environment.NewLine,
                        ";FOLD PTP " + ptp.Substring(1, ptp.Length-1) + " Vel=100 % PP10 Tool["+tool+"]:ToolName Base["+baseNum+"]:BaseName;%{PE}%R5.6.32,%MKUKATPBASIS,%CMOVE,%VPTP,%P 1:PTP, 2:P10, 3:, 5:100, 7:"+ ptp.Substring(1, ptp.Length - 1),
                        "$BWDSTART = FALSE",
                        "PDAT_ACT= "+pdat,
                        "FDAT_ACT= "+fdat,
                        "BAS(#PTP_PARAMS,100)",
                        "PTP " + ptp,
                        ";ENDFOLD","");
                    currentCommand += ";FOLD CloseAndOpenGun " + pos + "mm\r\nSET_POS (1, 0, #CTRL)\r\nWait Sec 0.7\r\nSET_POS (1, " + pos+", #CTRL)\r\n;ENDFOLD\r\n";
                }
                else
                    currentCommand = command;
                result.Add(currentCommand);
            }
            return result;
            
        }


        private static void SaveFiles(IDictionary<string, List<string>> resultFiles)
        {
            try
            {
                if (resultFiles != null && resultFiles.Count > 0)
                {
                    if (!Directory.Exists(currentDir + "\\ConvertedToPTP"))
                        Directory.CreateDirectory(currentDir + "\\ConvertedToPTP");
                    foreach (var file in resultFiles)
                    {
                        string fileContent = "";
                        foreach (string command in file.Value)
                        {
                            string tempCommand = "";
                            if (command.ToLower().Contains("def") && command.ToLower().Contains(file.Key.ToLower()))
                                tempCommand = "DEF " + file.Key + "ptp ( ) \r\n";
                            else
                                tempCommand = command;
                            fileContent += tempCommand;
                        }
                        File.WriteAllText(currentDir + "\\ConvertedToPTP\\" + file.Key + "ptp.src", fileContent);
                        string datFileConverted = "";
                        if (File.Exists(currentDir + "\\" + file.Key + ".dat"))
                        {
                            datFileConverted = ConvertDatFile(currentDir + "\\" + file.Key + ".dat");
                            File.WriteAllText(currentDir + "\\ConvertedToPTP\\" + file.Key + "ptp.dat", datFileConverted);
                        }
                    }
                    MessageBox.Show("Success", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static string ConvertDatFile(string datFile)
        {
            string result = "";
            StreamReader reader = new StreamReader(datFile);
            while (!reader.EndOfStream)
            {
                string lineToAdd = "";
                string line = reader.ReadLine();
                if (line.ToLower().Contains("defdat") && line.ToLower().Contains(Path.GetFileNameWithoutExtension(datFile).ToLower()))
                    lineToAdd = "DEFDAT " + Path.GetFileNameWithoutExtension(datFile) + "ptp";
                else
                    lineToAdd = line;
                result += lineToAdd + "\r\n";
            }
            return result;
        }
    }
}
