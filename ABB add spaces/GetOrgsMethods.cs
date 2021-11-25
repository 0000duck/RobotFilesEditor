using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ABB_add_spaces
{
    public static class GetOrgsMethods
    {
        internal static void Execute()
        {
            string directory = SelectDirOrFile(true);
            List<string> progmods = GetProgModDirectories(directory);
            if (progmods == null)
                return;
            List<string> mainMods = GetMainMods(progmods);
            IDictionary<string, List<string>> progstrings = GetProgStrings(mainMods);
            IDictionary<string, IDictionary<string, string>> robotPrograms = GetRobotPrograms(progstrings);
            SaveAllFiles(directory,robotPrograms);


        }

        private static void SaveAllFiles(string directory, IDictionary<string, IDictionary<string, string>> robotPrograms)
        {
            string directoryFinal = "\\OrgsFromBackup";
            if (!Directory.Exists(directory + directoryFinal))
                Directory.CreateDirectory(directory + directoryFinal);
            DirectoryInfo d = new DirectoryInfo(directory + directoryFinal);
            FileInfo[] files = d.GetFiles("*");
            if (files.Length > 0)
            {
                foreach (var file in files)
                    file.Delete();
            }
            foreach (var robot in robotPrograms)
            {
                string fileName = Path.GetFileNameWithoutExtension(robot.Key);
                string savePath = directory + directoryFinal + "\\" + fileName + ".txt";
                string saveString = "";
                foreach (var content in robot.Value)
                {
                    saveString += content.Key + ":\r\n"; 
                    StringReader reader = new StringReader(content.Value);
                    while (true)
                    {
                        string line = reader.ReadLine();
                        if (line == null)
                            break;
                        saveString += line + "\r\n";
                    }
                    saveString += "\r\n\r\n";
                    reader.Close();
                        
                }
                File.WriteAllText(savePath, saveString);
            }

        }

        private static IDictionary<string, List<string>> GetProgStrings(List<string> progmods)
        {
            Regex isProgram = new Regex(@"PROC\s+Program_\d+\s*\(\s*\)", RegexOptions.IgnoreCase);
            IDictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            foreach (var mainMod in progmods)
            {
                List<string> programStrings = new List<string>();
                string currentString = "";
                bool addLine = false;
                StreamReader reader = new StreamReader(mainMod);
                while(!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (isProgram.IsMatch(line))
                        addLine = true;
                    if (addLine)
                        currentString += line + "\r\n";
                    if (line.ToLower().Contains("endproc"))
                    {
                        if (!string.IsNullOrEmpty(currentString))
                            programStrings.Add(currentString);
                        currentString = "";
                        addLine = false;
                    }
                        
                }
                reader.Close();
                result.Add(mainMod, programStrings);
            }
            return result;
        }

        private static IDictionary<string, IDictionary<string,string>> GetRobotPrograms(IDictionary<string, List<string>> progstrings)
        {
            Regex isProgram = new Regex(@"PROC\s+Program_\d+\s*\(\s*\)", RegexOptions.IgnoreCase);
            Regex programNameRegex = new Regex(@"Program_\d+", RegexOptions.IgnoreCase);
            IDictionary<string, IDictionary<string, string>> result = new Dictionary<string, IDictionary<string, string>>();
            foreach (var program in progstrings)
            {
                IDictionary<string, string> currentProg = new Dictionary<string, string>();
                foreach (var progString in program.Value)
                {
                    string currentProgramName = "";
                    string currentProgContent = "";
                    

                    StringReader reader = new StringReader(progString);
                    while (true)
                    {
                        string line = reader.ReadLine();
                        if (line == null)
                            break;
                        if (isProgram.IsMatch(line))
                        {
                            currentProgramName = programNameRegex.Match(line).ToString();
                        }
                        string lineToLower = line.ToLower();
                        if (lineToLower.Contains("test ") || lineToLower.Contains("case ") || lineToLower.Contains("procedure call") || lineToLower.Contains("programmaufruf") || lineToLower.Contains("endtest"))
                            if (lineToLower.Contains("procedure call") || lineToLower.Contains("programmaufruf"))
                                currentProgContent += "\t" + line + "\r\n";
                            else
                                currentProgContent += line + "\r\n";
                    }

                    reader.Close();
                    currentProg.Add(currentProgramName, currentProgContent);
                }
                result.Add(program.Key, currentProg);
            }
            return result;
        }

        private static List<string> GetMainMods(List<string> progmods)
        {
            Regex getMainModsRegex = new Regex(@"ST\d+_IR\d+\.mod", RegexOptions.IgnoreCase);
            List<string> result = new List<string>();
            foreach (var directory in progmods)
            {
                DirectoryInfo d = new DirectoryInfo(directory);
                FileInfo[] files = d.GetFiles("*.mod");
                foreach (var file in files)
                {
                    if (getMainModsRegex.IsMatch(file.Name))
                    {
                        result.Add(file.FullName);
                        break;
                    }
                }
            }
            return result;
        }

        private static List<string> GetProgModDirectories(string directory)
        {
            if (string.IsNullOrEmpty(directory))
                return null;
            List<string> result = new List<string>();
            string[] folders = Directory.GetDirectories(directory, "PROGMOD", SearchOption.AllDirectories);
            foreach (string folder in folders.Where(x => x.ToLower().Contains("task1") || x.ToLower().Contains("task2")))
                result.Add(folder);
            return result;
        }

        public static string SelectDirOrFile(bool isDir, string filter1Descr = "", string filter1 = "", string filter2Descr = "", string filter2 = "")
        {
            var dialog = new CommonOpenFileDialog();
            if (isDir)
                dialog.IsFolderPicker = true;
            else
            {
                dialog.IsFolderPicker = false;
                if (!string.IsNullOrEmpty(filter1))
                    dialog.Filters.Add(new CommonFileDialogFilter(filter1Descr, filter1));
                if (!string.IsNullOrEmpty(filter2))
                    dialog.Filters.Add(new CommonFileDialogFilter(filter2Descr, filter2));
            }
            dialog.EnsurePathExists = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                return dialog.FileName;
            }
            else
                return "";
        }
    }
}
