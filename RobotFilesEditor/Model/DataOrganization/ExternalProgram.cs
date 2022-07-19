using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RobotFilesEditor.Model.DataOrganization
{
    public class ExternalProgram
    {
        public string Name { get; set; }
        public string FullPathExtServer { get; set; }
        public string FullPathOnLocalDrive { get; set; }

        public ICommand ExecuteCommand { get; set; }

        public ExternalProgram(string fullPath)
        {
            FullPathExtServer = fullPath;
            Name = Path.GetFileNameWithoutExtension(fullPath);
            FullPathOnLocalDrive = Path.Combine(Path.GetDirectoryName(GlobalData.PathFile), "ExternalFiles",Path.GetFileName(fullPath));
            ExecuteCommand = new RelayCommand(ExecuteClick);
        }

        private void ExecuteClick()
        {
            Process.Start(FullPathOnLocalDrive);
        }
    }

    public static class ExternalProgramMethods
    {
        public static ObservableCollection<ExternalProgram> GetExternalPrograms()
        {
            ObservableCollection<ExternalProgram> result = new ObservableCollection<ExternalProgram>();
            string mainDir = Path.Combine(Path.GetDirectoryName(GlobalData.PathFile), "ExternalFiles");
            if (!Directory.Exists(mainDir))
                Directory.CreateDirectory(mainDir);
            string searchPath = @"\\alfa\RoboBMW\RobotFilesHarvester_ExtPrograms";
            if (!Directory.Exists(searchPath))
            {
                List<string> files = Directory.GetFiles(mainDir, ".exe").ToList();
                files.ForEach(x => result.Add(new ExternalProgram(x)));
            }
            else
            {
                List<string> files = Directory.GetFiles(searchPath, "*.exe").ToList();
                foreach (var file in files)
                {
                    ExternalProgram program = new ExternalProgram(file);
                    if (!File.Exists(program.FullPathOnLocalDrive))
                        File.Copy(program.FullPathExtServer, program.FullPathOnLocalDrive);
                    if (File.GetLastWriteTime(program.FullPathExtServer) > File.GetLastWriteTime(program.FullPathOnLocalDrive))
                        File.Copy(program.FullPathExtServer, program.FullPathOnLocalDrive);
                    result.Add(program);
                }
            }
            return result;
        }
    }
}
