using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RobotFilesEditor.Model.TempFunctions
{
    public static class TempClass
    {
        internal static void Execute()
        {
            run_cmd(@"D:\Projekty\inne\KukaOrganizer\GIT\RobotFilesEditor\TestPythonProject\TestPythonProject.py","");
            //Regex isChangeRegex = new Regex(@"(?<=move(j|l).*WObj\s*\:\s*\=\s*[a-zA-Z0-9_-]+,).*", RegexOptions.IgnoreCase);
            //Regex isCommaRegex = new Regex(@"(?<=WObj.*),", RegexOptions.IgnoreCase);
            //Regex isSync = new Regex(@"move(l|j)sync", RegexOptions.IgnoreCase);
            //string file = string.Empty;
            //string result = string.Empty;
            //file = CommonLibrary.CommonMethods.SelectDirOrFile(false);
            //StreamReader reader = new StreamReader(file);
            //while (!reader.EndOfStream)
            //{
            //    string line = reader.ReadLine();
            //    if (isChangeRegex.IsMatch(line))
            //    {
            //        string lineToAdd =  line.Replace(isChangeRegex.Match(line).ToString(), ";");
            //        lineToAdd = isCommaRegex.Replace(lineToAdd, "");                   
            //        if (isSync.IsMatch(lineToAdd))
            //        {
            //            if (isSync.Match(lineToAdd).ToString().ToLower().Contains("movel"))
            //                lineToAdd = isSync.Replace(lineToAdd, "MoveL");
            //            else
            //                lineToAdd = isSync.Replace(lineToAdd, "MoveJ");
            //        }
            //        result += lineToAdd + "\r\n";

            //    }
            //    else
            //        result += line + "\r\n";

            //}
            //reader.Close();
            //string fileToSave = (Path.GetDirectoryName(file) + "\\" + Path.GetFileNameWithoutExtension(file) + "_changed" + Path.GetExtension(file));
            //File.WriteAllText(fileToSave, result);
        }

        private static void run_cmd(string cmd, string args)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "my/full/path/to/python.exe";
            start.Arguments = string.Format("{0} {1}", cmd, args);
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Console.Write(result);
                }
            }
        }
    }
}
