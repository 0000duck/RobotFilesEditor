using Microsoft.Win32;
using System;
namespace RobotFilesEditor
{
    public static class GlobalData
    {
        public const string ConfigurationFileName = "Application.config";       
        public enum Action {None, Move, Remove, Copy, CopyData, CutData, RemoveData}
        public enum SortType {None, OrderByVariable, OrderByOrderNumber}
        public enum HeaderType { None, GlobalFileHeader, GroupsHeadersByVariableOrderNumber }

        public static string ViewProgram { get; private set; } = "notepad.exe";

        private static string[] _viewerPrograms = { "notepad++.exe"};
      
        public static void SetViewProgram()
        {
            try
            {
                foreach (var program in _viewerPrograms)
                {
                    if (CheckIfInstaled(program) == true)
                    {
                        ViewProgram = program;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }          
        } 

        private static bool CheckIfInstaled(string programName)
        {
            string regKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths";

            try
            {
                using (Microsoft.Win32.RegistryKey key = Registry.LocalMachine.OpenSubKey(regKey))
                {
                    foreach (string subkeyName in key.GetSubKeyNames())
                    {
                        if (subkeyName.Equals(programName))
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
                       
            return false;
        }

        public static HeaderType ChekIfHeaderIsCreatingByMethod(string header)
        {
            HeaderType type;
            if (string.IsNullOrEmpty(header))
            {
                return HeaderType.None;
            }

            if (Enum.TryParse(header, out type))
            {
                return type;
            }
            else
            {
                return HeaderType.None;
            }
        }
    }
}
