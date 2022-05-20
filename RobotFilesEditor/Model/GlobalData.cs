using Microsoft.Win32;
using RobotFilesEditor.Dialogs.CreateGripper;
using RobotFilesEditor.Model.DataInformations;
using RobotFilesEditor.Model.Operations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace RobotFilesEditor
{
    public static class GlobalData
    {
        public static string PathFile {get; set;}
        public static int? CurrentOpNum { get; set; }
        public static int? CurrentOpNumFanuc { get; set; }
        public static int AllOperations { get; set; }
        public static bool HasToolchager { get; set; }
        public static bool CheckOrder { get; set; }
        public static bool RoboterFound { get; set; }
        public static bool isWeldingRobot { get; set; }
        public static bool isHandlingRobot { get; set; }
        public static bool Has7thAxis { get; set; }
        public static string Roboter = "";
        public static List<string> Operations = new List<string>();        
        //public const string ConfigurationFileName = "Application.config";       
        public enum Action {None, Move, Remove, Copy, CopyData, CutData, RemoveData}
        public enum SortType {None, OrderByVariable, OrderByOrderNumber}
        public enum HeaderType { None, GlobalFileHeader, GroupsHeadersByVariableOrderNumber }
        public static string ControllerType { get; set; }
        public static string ViewProgram { get; private set; } = "notepad.exe";
        public static string DestPath { get; set; }
        public static string DestFolder { get; set; }
        public static IDictionary<string, int> SrcPathsAndJobs { get; set; }
        public static IDictionary<int,string> Jobs { get; set; }
        public static IDictionary<int, string> Tools { get; set; }
        public static IDictionary<int, string> Bases { get; set; }
        public static string IBUSSegments { get; set; }
        public static string InputData { get; set; }
        public static List<string> InputDataList { get; set; }
        public static List<string> GlobalDatsList { get; set; }
        public static string DestinationPath { get; set; }
        public static string SourcePath { get; set; }
        public static CreateGripperViewModel GripperVM { get; internal set; }
        public static IDictionary<string, int> Paths { get; internal set; }
        //public static List<KeyValuePair<string,int>> SelectedJobsForAnyJob { get; internal set; }
        public static ObservableCollection<KeyValuePair<string, int>> SelectedJobsForAnyJob { get; internal set; }
        public static List<string> GlobalFDATs { get; internal set; }
        public static List<string> AllFiles { get; internal set; }
        public static ObservableCollection<KeyValuePair<string, int>> SelectedUserNums { get; internal set; }
        public static List<string> E6axisGlobalsfound { get; set; }
        public static string RobotType { get; set; }
        public static string ToolchangerType { get; set; }
        public static string WeldingType { get; set; }
        public static string LaserType { get; set; }
        //public static string RivetingType { get; set; }
        public static bool LocalHomesFound { get; set; }
        public static IDictionary<int, string> loadVars { get; set; }
        private static string[] _viewerPrograms = { "notepad++.exe"};
        internal static FileValidationData.Userbits SignalNames { get; set; }
        public static IDictionary<int, string> ToolsAndNamesFromStandar { get; internal set; }
        public static string RobotNameFanuc { get; set; }
        public enum RobotController { KUKA, FANUC, ABB };
        public enum RenameWindowType { Point, Path };
        public enum SovLogContentInfoTypes { OK, Warning, Error, Information};




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
                SrcValidator.GetExceptionLine(ex);
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
                SrcValidator.GetExceptionLine(ex);
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
