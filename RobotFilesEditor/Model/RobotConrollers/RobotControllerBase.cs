using CommonLibrary;
using CommonLibrary.DataClasses;
using GalaSoft.MvvmLight.Messaging;
using ProjectInformations.Model;
using RobotFilesEditor.Model.Operations.DataClass;
using RobotFilesEditor.Model.Operations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static RobotFilesEditor.Model.DataInformations.FileValidationData;
using System.Configuration;
using System.Windows.Forms;
using RobotFilesEditor.Model.DataInformations;

namespace RobotFilesEditor.Model.RobotConrollers
{
    public class RobotControllerBase
    {

        #region fields
        private ProjectInfos projectsDeserialized;
        private Project selectedProject;
        public ValidationData DataToProcess;
        public bool fillDescrs;
        public string language;
        public string matchRoboter;

        internal IDictionary<string, string> srcFiles;
        internal List<string> datFiles;
        #endregion fields


        #region properties
        public static IDictionary<string, string> Result { get; set; }
        #endregion properties

        #region public methods
        public virtual bool ValidateFiles(IDictionary<string, string> srcFilesIn, List<string> datFilesIn)
        {
            Messenger.Default.Send<LogResult>(new LogResult("Validation started", LogResultTypes.Information), "AddLog");
            if (!Initialize(srcFilesIn, datFilesIn))
                return false;
            DeserializeProjects();
            GlobalData.ToolchangerType = DetectAppNew(srcFilesIn.Keys, selectedProject.ApplicationTypes.TchType, new string[3] { "dock", "dockdresser", "_tch_" }, "toolchanger");
            GlobalData.WeldingType = DetectAppNew(srcFilesIn.Keys, selectedProject.ApplicationTypes.SpotType, new string[3] { "spot", "notUsed", "_swx_" }, "welding");
            return true;
        }
        #endregion public methods



        #region private methods
        private bool Initialize(IDictionary<string, string> srcFilesIn, List<string> datFilesIn)
        {
            language = GlobalData.Language;
            FilterDataFromBackup(true, srcFilesIn);
            FilterDataFromBackup(false, null, datFilesIn);
            GlobalData.loadVars = new Dictionary<int, string>();
            GlobalData.LocalHomesFound = false;
            matchRoboter = string.Empty;
            GlobalData.HasToolchager = false;
            if (DataToProcess.SrcFiles.Count == 0)
                return false;
            GlobalData.GlobalDatsList = new List<string>();
            GlobalData.InputDataList = new List<string>();
            IDictionary<string, string> srcFiles = DataToProcess.SrcFiles;
            srcFiles = FilterSRC(srcFiles);
            GlobalData.isWeldingRobot = FindWelding(srcFiles);
            GlobalData.isHandlingRobot = FindHandling(srcFiles);
            string opereationName = DataToProcess.OpereationName;
            List<string> datFiles = DataToProcess.DatFiles;
            //GlobalData.Roboter = roboter;
            if (srcFiles == null | datFiles == null | !DetectDuplicates(srcFiles))
                return false;
            return true;
        }

        private void DeserializeProjects()
        {
            var path = CommonMethods.GetFilePath("ProjectInfos.xml");
            var projName = string.Empty;
            var serializer = new XmlSerializer(typeof(ProjectInfos));
            var projects = new List<Project>();
            using (Stream reader = new FileStream(path, FileMode.Open))
            {
                projectsDeserialized = (ProjectInfos)serializer.Deserialize(reader);
                projectsDeserialized.Project.ForEach(x => projects.Add(x));
                projName = projectsDeserialized.SelectedProject.Name;
            }
            selectedProject = projects.FirstOrDefault(x => x.Name == projName);
        }

        private void FilterDataFromBackup(bool isSrc, IDictionary<string, string> srcFiles = null, List<string> datFiles = null)
        {
            if (DataToProcess is null)
                DataToProcess = new FileValidationData.ValidationData(new Dictionary<string, string>(), "", new List<string>());
            IDictionary<string, string> result = new Dictionary<string, string>();
            if (isSrc)
                result = srcFiles;
            else
                datFiles.ForEach(x => result.Add(x, ""));
            foreach (var item in result)
            {
                string[] omitedDataInCheck = ConfigurationManager.AppSettings["OmitedDataInCheck" + GlobalData.ControllerType.Replace(" ", "_")].Split(',').Select(s => s.Trim()).ToArray();
                bool flag = false;
                foreach (string omitedData in omitedDataInCheck)
                {
                    if (ConfigurationManager.AppSettings[omitedData] != null && item.Key.Contains(ConfigurationManager.AppSettings[omitedData]))
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    if (isSrc)
                    {
                        if (!DataToProcess.SrcFiles.Contains(item))
                        {
                            DataToProcess.SrcFiles.Add(item);
                            //Result = DataToProcess.SrcFiles;
                        }
                    }
                    else
                    {
                        if (!DataToProcess.DatFiles.Contains(item.Key))
                            DataToProcess.DatFiles.Add(item.Key);
                    }

                }
            }
        }

        private IDictionary<string, string> FilterSRC(IDictionary<string, string> srcFiles)
        {
            IDictionary<string, string> result = srcFiles;
            IDictionary<string, string> copyOfSrcFiles = new Dictionary<string, string>();
            foreach (var item in result)
                copyOfSrcFiles.Add(item);
            foreach (var item in ConfigurationManager.AppSettings["SystemFile" + GlobalData.ControllerType.Replace(" ", "_")].Split(',').ToArray())
            {
                foreach (var item2 in copyOfSrcFiles.Where(x => Path.GetFileName(x.Key.ToLower()).Contains(item)))
                    result.Remove(item2);
            }
            return result;

        }

        private bool FindWelding(IDictionary<string, string> srcFiles)
        {
            foreach (var item in srcFiles.Where(x => x.Key.ToLower().Contains("spot")))
                return true;
            return false;
        }
        private bool FindHandling(IDictionary<string, string> srcFiles)
        {
            foreach (var item in srcFiles.Where(x => x.Key.ToLower().Contains("pick") || x.Key.ToLower().Contains("drop")))
                return true;
            return false;
        }

        private bool DetectDuplicates(IDictionary<string, string> srcFiles)
        {
            List<string> files = new List<string>();
            foreach (var file in srcFiles)
                files.Add(Path.GetFileNameWithoutExtension(file.Key));

            List<string> testlist = new List<string>();
            foreach (var file in files)
            {
                if (testlist.Contains(file))
                {
                    MessageBox.Show("Multiple paths with same name found. Program will abort!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                else
                    testlist.Add(file);
            }
            return true;
        }

        private string DetectAppNew(ICollection<string> keys, ApplicationBase variants, string[] type, string label)
        {
            if (keys.Any(x => x.ToLower().Contains(type[0]) && !x.ToLower().Contains(type[1])) || keys.Any(x => x.ToLower().Contains(type[2].Replace("x", "p"))) || keys.Any(x => x.ToLower().Contains(type[2].Replace("x", "i"))))
            {
                var props = variants.GetType().GetProperties();
                foreach (var prop in props)
                {
                    if ((prop.GetValue(variants) as string) == "true")
                        return prop.Name;
                }
                return string.Empty;
            }
            else
                return string.Empty;

        }
        #endregion private methods


        #region internal methods
        internal void SetFillDescr()
        {
            fillDescrs = selectedProject.UseCollDescr.Value.Equals("true", StringComparison.InvariantCultureIgnoreCase) ? true : false;
        }
        #endregion internal methods
    }
}
