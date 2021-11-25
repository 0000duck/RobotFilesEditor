using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommonLibrary;

namespace RobotFilesEditor.Model.Operations.TempScripts
{
    public static class ModifyE1Value
    {
        const double offset = -1500;

        public static void Execute()
        {
            IDictionary<string, string> resultFiles = new Dictionary<string, string>();
            Regex getE1 = new Regex(@"(?<=E1\s+)((\-\d+\.\d+)|(\d+\.\d+)|(\-\d+)|(\d+))", RegexOptions.IgnoreCase);
            string dir = CommonMethods.SelectDirOrFile(true);
            if (dir == string.Empty)
                return;
            foreach (var file in Directory.GetFiles(dir, "*.dat"))
            {
                string resultfile = string.Empty;
                StreamReader reader = new StreamReader(file);
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (line.ToLower().Contains("e6pos "))
                    {
                        double originalE1 = double.Parse(getE1.Match(line).ToString().Replace(".", ","));
                        double resultE1 = originalE1 + offset;
                        line = getE1.Replace(line, resultE1.ToString().Replace(",", "."));
                    }
                    resultfile += line + "\r\n";
                }
                reader.Close();
                resultFiles.Add(file, resultfile);
            }
            if (resultFiles.Count > 0)
            {
                string[] dirs = Path.GetDirectoryName(resultFiles.Keys.ToList()[0]).Split('\\');
                string dirToSave = dirs[dirs.Length - 1];
                string dirToSaveComplete = Path.Combine(CommonMethods.SelectDirOrFile(true),dirToSave);
                if (!Directory.Exists(dirToSaveComplete))
                    Directory.CreateDirectory(dirToSaveComplete);
                foreach (var file in resultFiles)
                {
                    File.WriteAllText(Path.Combine(dirToSaveComplete, Path.GetFileName(file.Key)), file.Value);
                }
            }
        }
    }
}
