using RobotFilesEditor.Model.Operations.DataClass;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using CommonLibrary;

namespace RobotFilesEditor.Model.Operations
{
    public static class PrepareSOVBackupMethods
    {
        internal static void Execute()
        {
            try
            {
                MessageBox.Show("Select backup created in SAS", "Select backup", MessageBoxButton.OK, MessageBoxImage.Information);
                string selectedBackup = CommonMethods.SelectDirOrFile(false, filter1Descr: "Zip file", filter1: "*.zip");
                if (string.IsNullOrEmpty(selectedBackup))
                    return;
                MessageBox.Show("Select folder containing offline programs \"after harvester\"", "Select backup", MessageBoxButton.OK, MessageBoxImage.Information);
                string selectedDir = CommonMethods.SelectDirOrFile(true);
                if (string.IsNullOrEmpty(selectedDir))
                    return;
                RemoveEntry(selectedBackup, "init_out.src");
                CopyToDir("BMW_Std", selectedBackup, selectedDir, false);
                CopyToDir("BMW_App", selectedBackup, selectedDir, false);
                CopyToDir("BMW_Init", selectedBackup, selectedDir, false);
                CopyToDir("BMW_Std_User", selectedBackup, selectedDir, false);
                CopyToDir("BMW_Utilities", selectedBackup, selectedDir, false);
                CopyToDir("BMW_Utilities\\Docking", selectedBackup, selectedDir, false, "BMW_Utilities\\Docking");
                CopyToDir("Init_out", selectedBackup, selectedDir, false, "BMW_Std_User");
                CopyToDir("Program", selectedBackup, selectedDir, true);
                CopyToDir("BMW_Program", selectedBackup, selectedDir, true);
                CopyToDir("BMW_Program\\Abort_Programs", selectedBackup, selectedDir, false);
                CopyToDir("TP\\SafeRobot", selectedBackup, selectedDir, false, "TP\\SafeRobot");
                DataForSOV data = GetDataForSOV(selectedDir);
                if (data != null)
                {
                    ReplaceValuesInConfigDat(selectedBackup, data);
                    MessageBox.Show("Backup at " + selectedBackup + " has been updated", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch
            {
                MessageBox.Show("Something went wrong", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static void ReplaceValuesInConfigDat(string selectedBackup, DataForSOV data)
        {
            string directory = Path.GetDirectoryName(CommonGlobalData.ConfigurationFileName);
            Regex isHomeRegex = new Regex(@"XHOME\d+", RegexOptions.IgnoreCase);
            Regex getNumber = new Regex(@"(?<=^.*\[)\d+", RegexOptions.IgnoreCase);
            string outputstring = "";
            using (FileStream zipToOpen = new FileStream(selectedBackup, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    ZipArchiveEntry foundEntry = null;
                    foreach (var entry in archive.Entries.Where(x=>x.FullName.ToLower().Contains("$config.dat")))
                    {
                        foundEntry = entry;                        
                        if (!Directory.Exists(directory + "\\TempConfigFile"))
                            Directory.CreateDirectory(directory + "\\TempConfigFile");
                        if (File.Exists(directory + "\\TempConfigFile\\$config.dat"))
                            File.Delete(directory + "\\TempConfigFile\\$config.dat");
                        entry.ExtractToFile(directory + "\\TempConfigFile\\$config.dat");                        
                    }
                    if (foundEntry != null)
                        foundEntry.Delete();

                    StreamReader reader = new StreamReader(directory + "\\TempConfigFile\\$config.dat");
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (isHomeRegex.IsMatch(line))
                        {
                            Regex homeNum = new Regex(@"(?<=XHOME)\d", RegexOptions.IgnoreCase);
                            int number = int.Parse(homeNum.Match(line).ToString());
                            if (data.HomesSOV.Keys.Contains(number))
                                outputstring += "E6AXIS " + data.HomesSOV[number].Content + "\r\n";
                            else
                                outputstring += line + "\r\n";
                        }
                        else if (line.ToLower().Contains("tool_data"))
                        {
                            int number = int.Parse(getNumber.Match(line).ToString());
                            if (data.ToolDatasSOV.Keys.Contains(number))
                                outputstring += data.ToolDatasSOV[number].Content + "\r\n";
                            else
                                outputstring += line + "\r\n";
                        }
                        else if (line.ToLower().Contains("tool_name"))
                        {
                            int number = int.Parse(getNumber.Match(line).ToString());
                            if (data.ToolDatasSOV.Keys.Contains(number))
                                outputstring += data.ToolDatasSOV[number].Name + "\r\n";
                            else
                                outputstring += line + "\r\n";
                        }
                        else if (line.ToLower().Contains("tool_type"))
                        {
                            int number = int.Parse(getNumber.Match(line).ToString());
                            if (data.ToolDatasSOV.Keys.Contains(number))
                                outputstring += data.ToolDatasSOV[number].Type + "\r\n";
                            else
                                outputstring += line + "\r\n";
                        }
                        else if (line.ToLower().Contains("base_data"))
                        {
                            int number = int.Parse(getNumber.Match(line).ToString());
                            if (data.BaseDatasSOV.Keys.Contains(number))
                                outputstring += data.BaseDatasSOV[number].Content + "\r\n";
                            else
                                outputstring += line + "\r\n";
                        }
                        else if (line.ToLower().Contains("base_name"))
                        {
                            int number = int.Parse(getNumber.Match(line).ToString());
                            if (data.BaseDatasSOV.Keys.Contains(number))
                                outputstring += data.BaseDatasSOV[number].Name + "\r\n";
                            else
                                outputstring += line + "\r\n";
                        }
                        else if (line.ToLower().Contains("base_type"))
                        {
                            int number = int.Parse(getNumber.Match(line).ToString());
                            if (data.BaseDatasSOV.Keys.Contains(number))
                                outputstring += data.BaseDatasSOV[number].Type + "\r\n";
                            else
                                outputstring += line + "\r\n";
                        }
                        else if (line.ToLower().Contains(";fold user globals"))
                        {
                            outputstring += line + "\r\n";
                            foreach (var global in data.GlobalPointsSOV)
                            {
                                outputstring += global.Value.Content + "\r\n";
                            }
                        }
                        else
                            outputstring += line + "\r\n";
                    }
                    reader.Close();
                    ZipArchiveEntry readmeEntry = archive.CreateEntry(foundEntry.FullName);
                    using (StreamWriter writer = new StreamWriter(readmeEntry.Open()))
                    {
                        string[] alllines = outputstring.Split("\r\n".ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                        foreach (var line in alllines)
                            writer.WriteLine(line);
                        writer.Close();
                    }
                    archive.Dispose();
                }
                zipToOpen.Close();
            }
        }

        private static DataForSOV GetDataForSOV(string selectedDir)
        {
            try
            {
                IDictionary<int, ToolDataSOV> tooldatas = new Dictionary<int, ToolDataSOV>();
                IDictionary<int, BaseDataSOV> basedatas = new Dictionary<int, BaseDataSOV>();
                IDictionary<int, HomeSOV> homes = new Dictionary<int, HomeSOV>();
                IDictionary<string, GlobalPointsSOV> globals = new Dictionary<string, GlobalPointsSOV>();
                string inputdatapath = "", copytoconfigpath = "";
                Regex getPointName = new Regex(@"^[a-zA-Z0-9_-]*", RegexOptions.IgnoreCase);
                List<string> files = Directory.GetFiles(selectedDir, "*.*", SearchOption.AllDirectories).ToList();
                foreach (string file in files.Where(x => x.ToLower().ContainsAny(new string[] { "copy_to_$config", "copy_to_uservariables" })))
                {
                    copytoconfigpath = file;
                    break;
                }
                foreach (string file in files.Where(x => x.ToLower().Contains("inputdata")))
                {
                    inputdatapath = file;
                    break;
                }
                if (!String.IsNullOrEmpty(copytoconfigpath))
                {
                    StreamReader reader = new StreamReader(copytoconfigpath);
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        Regex currentRegex = new Regex(@"", RegexOptions.IgnoreCase);
                        if (line.ToLower().Contains("fdat") || line.ToLower().Contains("e6pos") || line.ToLower().Contains("e6axis"))
                            currentRegex = new Regex(@"((?<=(E6AXIS|E6POS|FDAT)\s+)).*", RegexOptions.IgnoreCase);
                        else
                            currentRegex = new Regex(@"^.*", RegexOptions.IgnoreCase);
                        string currentLine = currentRegex.Match(line).ToString();
                        string pointName = getPointName.Match(currentLine).ToString();
                        if (!string.IsNullOrEmpty(pointName))
                            globals.Add(pointName, GetCurrentLine(pointName, currentLine));
                    }
                    reader.Close();
                }
                Regex isHomeRegex = new Regex(@"XHOME\d+", RegexOptions.IgnoreCase);
                if (!String.IsNullOrEmpty(inputdatapath))
                {
                    Regex getNumber = new Regex(@"(?<=^.*\[)\d+", RegexOptions.IgnoreCase);
                    StreamReader reader = new StreamReader(inputdatapath);
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (isHomeRegex.IsMatch(line))
                        {
                            Regex homeNum = new Regex(@"(?<=XHOME)\d", RegexOptions.IgnoreCase);
                            int number = int.Parse(homeNum.Match(line).ToString());
                            homes.Add(number, new HomeSOV(number, line));
                        }
                        if (line.ToLower().Contains("tool_data"))
                        {
                            int number = int.Parse(getNumber.Match(line).ToString());
                            if (!tooldatas.Keys.Contains(number))
                                tooldatas.Add(number, new ToolDataSOV());
                            tooldatas[number].Content = line;
                        }
                        if (line.ToLower().Contains("tool_name"))
                        {
                            int number = int.Parse(getNumber.Match(line).ToString());
                            if (!tooldatas.Keys.Contains(number))
                                tooldatas.Add(number, new ToolDataSOV());
                            tooldatas[number].Name = line;
                        }
                        if (line.ToLower().Contains("tool_type"))
                        {
                            int number = int.Parse(getNumber.Match(line).ToString());
                            if (!tooldatas.Keys.Contains(number))
                                tooldatas.Add(number, new ToolDataSOV());
                            tooldatas[number].Type = line;
                        }
                        if (line.ToLower().Contains("base_data"))
                        {
                            int number = int.Parse(getNumber.Match(line).ToString());
                            if (!basedatas.Keys.Contains(number))
                                basedatas.Add(number, new BaseDataSOV());
                            basedatas[number].Content = line;
                        }
                        if (line.ToLower().Contains("base_name"))
                        {
                            int number = int.Parse(getNumber.Match(line).ToString());
                            if (!basedatas.Keys.Contains(number))
                                basedatas.Add(number, new BaseDataSOV());
                            basedatas[number].Name = line;
                        }
                        if (line.ToLower().Contains("base_type"))
                        {
                            int number = int.Parse(getNumber.Match(line).ToString());
                            if (!basedatas.Keys.Contains(number))
                                basedatas.Add(number, new BaseDataSOV());
                            basedatas[number].Type = line;
                        }
                        if (line.ToLower().Contains("fg_"))
                        {
                            Regex currentRegex = new Regex(@"", RegexOptions.IgnoreCase);
                            if (line.ToLower().Contains("fdat") || line.ToLower().Contains("e6pos") || line.ToLower().Contains("e6axis"))
                                currentRegex = new Regex(@"((?<=(E6POS|FDAT)\s+)).*", RegexOptions.IgnoreCase);
                            else
                                currentRegex = new Regex(@"^.*", RegexOptions.IgnoreCase);
                            string currentLine = currentRegex.Match(line).ToString();
                            string pointName = getPointName.Match(currentLine).ToString();
                            if (!string.IsNullOrEmpty(pointName))
                            {
                                if (!globals.Keys.Contains(pointName))
                                    globals.Add(pointName, GetCurrentLine(pointName, currentLine));
                                else
                                    MessageBox.Show("Double declaration of " + pointName, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                        if (line.ToLower().Contains("xg_"))
                        {
                            Regex currentRegex = new Regex(@"", RegexOptions.IgnoreCase);
                            if (line.ToLower().Contains("fdat") || line.ToLower().Contains("e6pos") || line.ToLower().Contains("e6axis"))
                                currentRegex = new Regex(@"((?<=(E6POS|FDAT)\s+)).*", RegexOptions.IgnoreCase);
                            else
                                currentRegex = new Regex(@"^.*", RegexOptions.IgnoreCase);
                            string currentLine = currentRegex.Match(line).ToString();
                            string pointName = getPointName.Match(currentLine).ToString();
                            if (!string.IsNullOrEmpty(pointName))
                            {
                                if (!globals.Keys.Contains(pointName))
                                    globals.Add(pointName, GetCurrentLine(pointName, currentLine));
                                else
                                    MessageBox.Show("Double declaration of " + pointName, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                    }
                    reader.Close();
                }
                DataForSOV result = new DataForSOV(tooldatas, basedatas, homes, globals);

                return result;
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                return null;
            }
        }

        private static GlobalPointsSOV GetCurrentLine(string pointname,string currentLine)
        {
            string func = "";
            string firstLetter = currentLine.Substring(0, 1);
            if (firstLetter.ToLower() == "x")
                if (currentLine.ToLower().Contains("a1 "))
                    func = "E6AXIS";
                else
                    func = "E6POS";
            else
                func = "FDAT";
            return new GlobalPointsSOV(pointname, "DECL GLOBAL " +func+" "+currentLine);
        }

        private static void RemoveEntry(string selectedBackup, string entrytodelete)
        {
            using (FileStream zipToOpen = new FileStream(selectedBackup, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    List<ZipArchiveEntry> entries = new List<ZipArchiveEntry>();
                    foreach (var item in archive.Entries)
                    {
                        if (item.FullName.ToLower().Contains(entrytodelete.ToLower()))
                            entries.Add(item);
                    }
                    foreach (var entry in entries)
                    {
                        ZipArchiveEntry ent = archive.GetEntry(entry.FullName);
                        if (ent != null)
                            ent.Delete();
                    }                    
                    archive.Dispose();
                }
                zipToOpen.Close();
                
            }
        }

        private static void CopyToDir(string destinationDir, string selectedBackup, string selectedDir, bool removeAllContent, string destDirExtra = "")
        {
            List<string> files = new List<string>();
            if (!Directory.Exists(selectedDir + "\\" + destinationDir))
                return;
            files = Directory.GetFiles(selectedDir + "\\" + destinationDir).ToList();

            using (FileStream zipToOpen = new FileStream(selectedBackup, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    if (removeAllContent)
                    {
                        List<ZipArchiveEntry> entries = new List<ZipArchiveEntry>();
                        foreach (var item in archive.Entries)
                        {
                            if (item.FullName.ToLower().Contains(@"\"+destinationDir.ToLower()+@"\") || item.FullName.ToLower().Contains(@"/" + destinationDir.ToLower() + @"/"))
                            {
                                entries.Add(item);
                            }
                        }

                        foreach (var entry in entries)
                        {
                            ZipArchiveEntry ent = archive.GetEntry(entry.FullName);
                            if (ent != null)
                                ent.Delete();
                        }
                    }

                    foreach (var file in files)
                    {
                        ZipArchiveEntry readmeEntry;
                        if (string.IsNullOrEmpty(destDirExtra))
                        {
                            ZipArchiveEntry entry = archive.GetEntry("KRC\\R1\\" + destinationDir + "\\" + Path.GetFileName(file));
                            if (entry != null)
                                entry.Delete();
                            readmeEntry = archive.CreateEntryFromFile(file, "KRC\\R1\\" + destinationDir + "\\" + Path.GetFileName(file));
                        }
                        else
                        {
                            ZipArchiveEntry entry = archive.GetEntry("KRC\\R1\\" + destDirExtra + "\\" + Path.GetFileName(file));
                            if (entry != null)
                                entry.Delete();
                            readmeEntry = archive.CreateEntryFromFile(file, "KRC\\R1\\" + destDirExtra + "\\" + Path.GetFileName(file));
                        }
                    }
                    archive.Dispose();
                }
                zipToOpen.Close();
            }
        }
    }
}
