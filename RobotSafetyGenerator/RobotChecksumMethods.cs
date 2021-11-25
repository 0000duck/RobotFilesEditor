using Microsoft.Office.Interop.Excel;
using Excel = Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace RobotSafetyGenerator
{
    public static class RobotChecksumMethods
    {
        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        private static Microsoft.Office.Interop.Excel.Application oXL;
        private static Workbooks oWBs;
        private static Workbook oWB;
        private static Microsoft.Office.Interop.Excel.Sheets sheets;
        private static Microsoft.Office.Interop.Excel.Worksheet oSheet;
        private static string logContent;

        internal static void Execute()
        {
            string selectedDir = CommonLibrary.CommonMethods.SelectDirOrFile(true);
            if (String.IsNullOrEmpty(selectedDir))
                return;
            List<string> pscUserFiles = GetPSCUsers(selectedDir);
            SortedDictionary<string, string> abbchecksums = GetABBChecksums(pscUserFiles);
            List<string> fanucBackups = GetFanucBackups(selectedDir);
            SortedDictionary<string, FanucChecksums> fanucchecksums = GetFanucChecksums(fanucBackups);
            List<string> kukaBackups = CommonLibrary.CommonMethods.FindBackupsInDirectory(selectedDir, onlyWelding:false);
            SortedDictionary<string, string> kukachecksums = GetKukaChecksums(kukaBackups);
            CreateLogFile();
            WriteToExcel(abbchecksums, fanucchecksums,kukachecksums,selectedDir);
        }

        private static SortedDictionary<string, string> GetKukaChecksums(List<string> kukaBackups)
        {
            SortedDictionary<string, string> result = new SortedDictionary<string, string>();
            foreach (var backup in kukaBackups)
            {
                using (FileStream zipToOpen = new FileStream(backup, FileMode.Open))
                {
                    using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
                    {
                        var entry = archive.Entries.FirstOrDefault(x => x.FullName.ToLower().Contains("sctlcrc.xml"));
                        StreamReader reader = new StreamReader(entry.Open());
                        string content = reader.ReadToEnd();
                        Regex getCrcRegex = new Regex("(?<=(ParameterCrc|ActivationCode)\\s*\\=\\s*\")[\\w\\d]+", RegexOptions.IgnoreCase);
                        string crcsum = getCrcRegex.Match(content).ToString();
                        result.Add(Path.GetFileNameWithoutExtension(backup), crcsum);
                        reader.Close();
                    }
                }
            }
            return result;
        }

        private static void CreateLogFile()
        {
            string directory = Path.GetDirectoryName(CommonLibrary.CommonGlobalData.ConfigurationFileName);
            if (!string.IsNullOrEmpty(logContent))
            {
                File.Delete(directory + "\\log_Checksums.txt");
                using (StreamWriter sw = File.AppendText(directory + "\\log_Checksums.txt"))
                {
                    sw.Write(logContent);
                    sw.Close();
                    MessageBox.Show("Log file has been created at " + directory + "\\log_Checksums.txt");
                    System.Diagnostics.Process.Start(directory + "\\log_Checksums.txt");

                }

            }
        }

        private static void WriteToExcel(IDictionary<string, string> abbchecksums, IDictionary<string, FanucChecksums> fanucchecksums, IDictionary<string, string> kukachecksums, string selectedDir)
        {
            try
            {
                object misValue = System.Reflection.Missing.Value;
                Range aRange;
                int counter = 1;
                oXL = new Microsoft.Office.Interop.Excel.Application();
                oWBs = oXL.Workbooks;
                oWB = oWBs.Add("");
                sheets = oWB.Sheets;
                oSheet = sheets[1] as Microsoft.Office.Interop.Excel.Worksheet;

                oSheet = oWB.ActiveSheet;
                oSheet.Name = "Checksums";

                if (abbchecksums.Count > 0 || kukachecksums.Count > 0)
                {
                    oSheet.Cells[counter, 1] = "ROBOT";
                    oSheet.Cells[counter, 2] = "CheckSum";
                    //oSheet.Range[oSheet.Cells[counter, 2], oSheet.Cells[counter, 4]].Merge();
                    counter++;

                    foreach (var abbchecksum in abbchecksums)
                    {
                        oSheet.Cells[counter, 1] = abbchecksum.Key;
                        oSheet.Cells[counter, 2] = abbchecksum.Value;
                        //oSheet.Range[oSheet.Cells[counter, 2], oSheet.Cells[counter, 4]].Merge();
                        counter++;
                    }
                    foreach (var kukachecksum in kukachecksums)
                    {
                        oSheet.Cells[counter, 1] = kukachecksum.Key;
                        oSheet.Cells[counter, 2] = kukachecksum.Value;
                        //oSheet.Range[oSheet.Cells[counter, 2], oSheet.Cells[counter, 4]].Merge();
                        counter++;
                    }

                }
                if (abbchecksums.Count > 0 || kukachecksums.Count > 0)
                {
                    counter++;
                    //aRange = oSheet.Range[oSheet.Cells[1, 1], oSheet.Cells[counter, 4]];
                    aRange = oSheet.UsedRange;
                    aRange.Columns.AutoFit();
                    FormatAsTable(aRange, "Table1", "TableStyleMedium15");
                }

                int rangeBeginning = counter;

                if (fanucchecksums.Count > 0)
                {
                    oSheet.Cells[counter, 1] = "ROBOT";
                    oSheet.Cells[counter, 2] = "BASE CheckSum";
                    oSheet.Cells[counter, 3] = "Pos/Speed Checksum";
                    oSheet.Cells[counter, 4] = "IO Connect Checksum";
                    counter++;

                    foreach (var fanucchecksun in fanucchecksums)
                    {
                        oSheet.Cells[counter, 1] = fanucchecksun.Key;
                        oSheet.Cells[counter, 2] = fanucchecksun.Value.BaseSum;
                        oSheet.Cells[counter, 3] = fanucchecksun.Value.PosSpeed;
                        oSheet.Cells[counter, 4] = fanucchecksun.Value.IOConnect;
                        counter++;
                    }
                    aRange = oSheet.Range["A" + rangeBeginning, "D" + (counter + 1)];
                    aRange.Columns.AutoFit();
                    FormatAsTable(aRange, "Table2", "TableStyleMedium15");
                }
                string savePath = selectedDir + "\\Robots_Checksums.xlsx";
                try
                {
                    //destinationXlWorkbook.SaveAs(savePath, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                    if (!File.Exists(savePath) || !CommonLibrary.CommonMethods.IsFileLocked(savePath))
                    {
                        oWB.SaveAs(savePath, Excel.XlFileFormat.xlWorkbookDefault, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                        //MessageBox.Show("Succesfully saved at " + savePath, "Success", System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);
                        MessageBox.Show("Succesfully saved at " + savePath, "Success",MessageBoxButtons.OK,MessageBoxIcon.Information);
                        oWB.Close(true, misValue, misValue);
                    }
                    else
                        MessageBox.Show("File " + savePath + " is used by another process. Close the file and try again", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    CleanUpExcel();
                    

                }
                catch
                {
                    CleanUpExcel();
                    
                    MessageBox.Show("Something went wrong", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch
            {
                CleanUpExcel();
                MessageBox.Show("Coś poszło nie tak", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void FormatAsTable(Range SourceRange, string TableName, string TableStyleName)
        {
            SourceRange.Worksheet.ListObjects.Add(XlListObjectSourceType.xlSrcRange,
            SourceRange, System.Type.Missing, XlYesNoGuess.xlYes, System.Type.Missing).Name =
                TableName;
            SourceRange.Select();
            SourceRange.Worksheet.ListObjects[TableName].TableStyle = TableStyleName;
        }

        private static void CleanUpExcel()
        {
            if (oXL != null)
            {
                int hWndDest = oXL.Application.Hwnd;

                uint processID;

                GetWindowThreadProcessId((IntPtr)hWndDest, out processID);
                Process.GetProcessById((int)processID).Kill();
            }
            oWBs = null;
            oWB = null;
            oSheet = null;
        }

        private static SortedDictionary<string, FanucChecksums> GetFanucChecksums(List<string> fanucBackups)
        {
            SortedDictionary<string, FanucChecksums> result = new SortedDictionary<string, FanucChecksums>();
            Regex getCheckSumRegex = new Regex(@"((\d{3}\d+)|((-\s*\d{3}\d+)))", RegexOptions.IgnoreCase);
            string baseSum = "", posspeed = "",ioconnect = ""; 
            string directory = Path.GetDirectoryName(CommonLibrary.CommonGlobalData.ConfigurationFileName);
            foreach (var backup in fanucBackups)
            {
                using (FileStream zipToOpen = new FileStream(backup, FileMode.Open))
                {
                    using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
                    {
                        ZipArchiveEntry foundEntry = null;
                        foreach (var entry in archive.Entries.Where(x => x.FullName.ToLower().Contains("dcsvrfy.dg")))
                        {
                            foundEntry = entry;
                            if (!Directory.Exists(directory + "\\TempDCSFile"))
                                Directory.CreateDirectory(directory + "\\TempDCSFile");
                            if (File.Exists(directory + "\\TempDCSFile\\DCSVRFY.DG"))
                                File.Delete(directory + "\\TempDCSFile\\DCSVRFY.DG");
                            entry.ExtractToFile(directory + "\\TempDCSFile\\DCSVRFY.DG");
                        }
                        StreamReader reader = new StreamReader(directory + "\\TempDCSFile\\DCSVRFY.DG");
                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine();
                            if (line.Replace(" ","").ToLower().Contains("2base:"))
                            {
                                baseSum = getCheckSumRegex.Match(line).ToString();
                            }
                            if (line.Replace(" ", "").ToLower().Contains(@"3pos./speed:"))
                            {
                                posspeed = getCheckSumRegex.Match(line).ToString();
                            }
                            if (line.Replace(" ", "").ToLower().Contains(@"4i/oconnect:"))
                            {
                                ioconnect = getCheckSumRegex.Match(line).ToString();
                            }
                        }
                        if (!result.Keys.Contains(Path.GetFileNameWithoutExtension(backup)))
                            result.Add(Path.GetFileNameWithoutExtension(backup), new FanucChecksums(baseSum, posspeed, ioconnect));
                        else
                        {
                            string message = "Checksums for " + Path.GetFileNameWithoutExtension(backup) + " already found and can't be added!";
                            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            logContent += message + "\r\n";
                        }
                        reader.Close();
                        File.Delete(directory + "\\TempDCSFile\\DCSVRFY.DG");
                        archive.Dispose();
                    }
                    zipToOpen.Close();
                }
            }
            return result;
        }

        private static List<string> GetFanucBackups(string selectedDir)
        {
            List<string> result = new List<string>();
            string[] foundfiles = Directory.GetFiles(selectedDir, "*.zip", SearchOption.AllDirectories);

            foreach (var file in foundfiles)
            {
                using (FileStream zipToOpen = new FileStream(file, FileMode.Open))
                {
                    try
                    {
                        using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                        {
                            foreach (var entry in archive.Entries.Where(x => x.FullName.ToLower().Contains("dcsvrfy.dg")))
                            {
                                result.Add(file);
                                break;
                            }
                            archive.Dispose();
                        }                    
                    }
                    catch
                    {
                        string message = "Archive " + file + " is not readable!";
                        MessageBox.Show(message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        logContent += message + "\r\n";
                    }
                    zipToOpen.Close();
                }
            }
            return result;
        }

        private static SortedDictionary<string, string> GetABBChecksums(List<string> pscUserFiles)
        {
            SortedDictionary<string, string> result = new SortedDictionary<string, string>();
            foreach(var file in pscUserFiles)
            {
                Regex getChecksumRegex = new Regex("(?<=CheckSumReadBack\\s*\\=\\s*\")[\\d\\s]*");
                string resultKey = "", resultValue = "";
                if (File.Exists(Path.GetDirectoryName(file) + "\\backinfo.txt"))
                {
                    Regex getRobotNameRegex = new Regex(@"(?<=BACKUP\\+)[a-zA-Z0-9_]*", RegexOptions.IgnoreCase); 
                    StreamReader reader = new StreamReader(Path.GetDirectoryName(file) + "\\backinfo.txt");
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (line.ToLower().Replace(" ","").Contains("originallyat"))
                        {
                            resultKey = getRobotNameRegex.Match(line).ToString();
                            break;
                        }

                    }
                    reader.Close();

                    reader = new StreamReader(file);
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (line.ToLower().Contains("seal checksum"))
                        {
                            resultValue = getChecksumRegex.Match(line).ToString();
                        }
                    }
                    reader.Close();
                    if (!result.Keys.Contains(resultKey) && !String.IsNullOrEmpty(resultKey))
                        result.Add(resultKey, resultValue);
                    else
                    {
                        if (!String.IsNullOrEmpty(resultKey))
                        {
                            string message = "Checksum for robot " + resultKey + " already added!";
                            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            logContent += message + "\r\n";
                        }
                            
                    }
                }
            }

            return result;
        }

        private static List<string> GetPSCUsers(string selectedDir)
        {
            List<string> result = new List<string>();
            string[] foundfiles = Directory.GetFiles(selectedDir, "*.sxml", SearchOption.AllDirectories);
            foreach (var file in foundfiles.Where(x=>x.ToLower().Contains("psc_user_1.sxml")))
            {
                result.Add(file);
            }
            return result;
        }
    }
}
