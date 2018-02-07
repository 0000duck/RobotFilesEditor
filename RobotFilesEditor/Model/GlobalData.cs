using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor
{
    public static class GlobalData
    {
        public const string ConfigurationFileName = "Application.config";       
        public enum Action {None, Move, Remove, Copy, CopyData, MoveData, RemoveData}
        public static string ViewProgram { get; private set; } = "notepad.exe";

        private static string[] _viewerPrograms = { "notepad++.exe"};
      
        public static void SetViewProgram()
        {
            foreach(var program in _viewerPrograms)
            {
                if (CheckIfInstaled(program) == true)
                {
                    ViewProgram = program;
                    return;
                }
            }          
        } 

        private static bool CheckIfInstaled(string programName)
        {
            string regKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths";
            using (Microsoft.Win32.RegistryKey key = Registry.LocalMachine.OpenSubKey(regKey))
            {
                foreach(string subkeyName in key.GetSubKeyNames())
                {
                    if(subkeyName.Equals(programName))
                    {
                        return true;
                    }                       
                }
            }
            return false;
        }
    }
}
