using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace RobotFilesEditor.Model.Operations.FANUC
{
    public class GetFanucCheckSums
    {
        public GetFanucCheckSums()
        {
            Execute();
        }

        private void Execute()
        {
            string outputString = string.Empty;
            string directory = CommonLibrary.CommonMethods.SelectDirOrFile(true);
            if (!Directory.Exists(directory))
                return;
            List<string> backupFiles = Directory.GetFiles(directory, "*.zip",SearchOption.AllDirectories).ToList();
            foreach (var file in backupFiles)
            {
                if (!CommonLibrary.CommonMethods.IsFileLocked(file))
                {
                    using (FileStream zipToOpen = new FileStream(file, FileMode.Open))
                    {
                        using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
                        {
                            var currentEntry = archive.Entries.ToList().SingleOrDefault(x => Path.GetFileName(x.FullName).ToLower() == "dcsvrfy.pdf");
                            if (currentEntry != null)
                            {
                                string dcsDir = Path.Combine(Path.GetDirectoryName(GlobalData.PathFile), "DCSReport");
                                if (!Directory.Exists(dcsDir))
                                    Directory.CreateDirectory(dcsDir);
                                if (File.Exists(Path.Combine(dcsDir, "DCSVRFY.PDF")))
                                    File.Delete(Path.Combine(dcsDir, "DCSVRFY.PDF"));
                                currentEntry.ExtractToFile(Path.Combine(dcsDir, "DCSVRFY.PDF"));
                                string pdfContent = ExtractTextFromPDF(Path.Combine(dcsDir, "DCSVRFY.PDF"));
                                FanucChecksum currentChecksums = GetChecksums(pdfContent);
                                outputString += currentChecksums.BackupName + "\r\n";
                                outputString += currentChecksums.Robot + "\r\n";
                                outputString += currentChecksums.Other + "\r\n";
                                outputString += currentChecksums.Pos_Speed + "\r\n";
                                outputString += currentChecksums.Mastering + "\r\n";
                                outputString += currentChecksums.IOconnect + "\r\n";
                                outputString += "\r\n\r\n";
                            }
                            archive.Dispose();
                        }
                        zipToOpen.Close();
                    }
                }
            }
            File.WriteAllText(Path.Combine(Path.GetDirectoryName(GlobalData.PathFile), "DCSReport", "DCSCheckSums.txt"), outputString);
            Process.Start(Path.Combine(Path.GetDirectoryName(GlobalData.PathFile), "DCSReport", "DCSCheckSums.txt"));
        }

        private FanucChecksum GetChecksums(string pdfContent)
        {
            Regex robotNameRegex = new Regex(@"(?<=DCS report for\s+).*$", RegexOptions.IgnoreCase);
            Regex masteringRegex = new Regex(@"(?<=^\s*Mastering\s*\:\s+)[\d-]+", RegexOptions.IgnoreCase);
            Regex robotRegex = new Regex(@"(?<=^\s*Robot\s*\:\s+)[\d-]+", RegexOptions.IgnoreCase);
            Regex otherRegex = new Regex(@"(?<=^\s*Other\s*\:\s+)[\d-]+", RegexOptions.IgnoreCase);
            Regex posspeedRegex = new Regex(@"(?<=^\s*\d+\s+Pos\./Speed\s*\:\s+)[\d+-]+", RegexOptions.IgnoreCase);
            Regex ioConnectRegex = new Regex(@"(?<=^\s*\d+\s+I/O\s+connect\s*\:\s+)[\d+-]+", RegexOptions.IgnoreCase);
            bool signatureSection = false;
            FanucChecksum result = new FanucChecksum();
            StringReader reader = new StringReader(pdfContent);
            while (true)
            {
                string line = reader.ReadLine();
                if (line == null)
                    break;
                if (robotNameRegex.IsMatch(line))
                    result.BackupName = robotNameRegex.Match(line).ToString();
                if (line.ToLower().Contains("signature number (dec)"))
                    signatureSection = true;
                if (signatureSection)
                {
                    if (masteringRegex.IsMatch(line))
                        result.Mastering = masteringRegex.Match(line).ToString();
                    if (robotRegex.IsMatch(line))
                        result.Robot = robotRegex.Match(line).ToString();
                    if (otherRegex.IsMatch(line))
                        result.Other = otherRegex.Match(line).ToString();
                    if (posspeedRegex.IsMatch(line))
                        result.Pos_Speed = posspeedRegex.Match(line).ToString();
                    if (ioConnectRegex.IsMatch(line))
                        result.IOconnect = ioConnectRegex.Match(line).ToString();
                }
                
            }
            return result;
        }

        private string ExtractTextFromPDF(string filePath)
        {
            string result = string.Empty;
            PdfReader pdfReader = new PdfReader(filePath);
            PdfDocument pdfDoc = new PdfDocument(pdfReader);
            for (int page = 1; page <= pdfDoc.GetNumberOfPages(); page++)
            {
                ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                string pageContent = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(page), strategy);
                result+= pageContent + "\r\n";
            }
            pdfDoc.Close();
            pdfReader.Close();
            return result;
        }
    }

    public class FanucChecksum
    {
        public string BackupName { get; set; }
        public string Mastering { get; set; }
        public string Robot { get; set; }
        public string Other { get; set; }
        public string Pos_Speed { get; set; }
        public string IOconnect { get; set; }

        public FanucChecksum()
        {

        }
    }
}
