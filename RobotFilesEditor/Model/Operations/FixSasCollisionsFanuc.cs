using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RobotFilesEditor.Model.Operations
{
    public class FixSasCollisionsFanuc
    {
        Regex collzoneRegex = new Regex(@"^\s*<\s*DynCallProc.*CMN.*CollZone.*(Request|Release)", RegexOptions.IgnoreCase);
        Regex replaceRegex = new Regex(@"P\d+_", RegexOptions.IgnoreCase);

        public FixSasCollisionsFanuc()
        {
            try
            {
                MessageBox.Show("Select .sasz file updated with collisions in SAS", "Select file", MessageBoxButtons.OK, MessageBoxIcon.Information);
                var sasFile = CommonLibrary.CommonMethods.SelectDirOrFile(false, "SAS file", "*.sasz");
                List<string> resultSASmainXml = FixAllLines(sasFile);
                WriteSAS(sasFile, resultSASmainXml);
                MessageBox.Show("SAS updated succesfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<string> FixAllLines(string sasFile)
        {
            List<string> result = new List<string>();
            using (FileStream zipToOpen = new FileStream(sasFile, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
                {
                    var mainEntry = archive.Entries.Single(x => x.FullName.ToLower().Contains("main.xml"));
                    StreamReader reader = new StreamReader(mainEntry.Open());
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (collzoneRegex.IsMatch(line))
                        {
                            line = replaceRegex.Replace(line, "");
                        }
                        result.Add(line);
                    }
                    reader.Close();
                    archive.Dispose();
                }
                zipToOpen.Close();
            }
            return result;
        }

        private void WriteSAS(string sasfile, List<string> resultSASmainXml)
        {
            using (FileStream zipToOpen = new FileStream(sasfile, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    archive.Entries.Where(x => x.FullName.ToLower().Contains("main.xml")).First().Delete();
                    ZipArchiveEntry readmeEntry = archive.CreateEntry("main.xml");
                    using (StreamWriter writer = new StreamWriter(readmeEntry.Open()))
                    {
                        foreach (var line in resultSASmainXml)
                            writer.WriteLine(line);
                        writer.Close();
                    }
                    archive.Dispose();
                }
                zipToOpen.Close();
            }
        }
    }
}
