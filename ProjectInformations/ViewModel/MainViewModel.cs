using CommonLibrary;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectInformations.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace ProjectInformations.ViewModel
{
    public partial class MainViewModel : ObservableRecipient
    {

        #region fields
        const string xmlFileName = "ProjectInfos.xml";
        TypNumber copyOfTyp;
        ProjectInfos xmlDeserialized;
        string tempProjectName;
        Project newproject;
        string path;
        #endregion fields

        #region ctor
        public MainViewModel()
        {
            ExportOnClose = false;
            EditProjectNameVisible = false;
            TypeOptionsVisible = true;
            path = CommonMethods.GetFilePath(xmlFileName);
            Deserialize();
            if (Projects?.Count > 0)
            {
                SelectedProject = Projects[0];
                UpdateGUI();
            }
        }
        #endregion ctor

        #region properties
        [ObservableProperty]
        ObservableCollection<Project> projects;

        [ObservableProperty]
        bool typeEditButtonsVisible;

        [ObservableProperty]
        int mainTypID;

        [ObservableProperty]
        int communalTypID;

        [ObservableProperty]
        ApplicationTypes applicationTypes;

        [ObservableProperty]
        bool exportOnClose;

        public bool EditProjectNameVisible { get { return m_EditProjectNameVisible; } set { SetProperty(ref m_EditProjectNameVisible, value); EditProjectNameDisabled = !value; } }
        private bool m_EditProjectNameVisible;

        [ObservableProperty]
        bool editProjectNameDisabled;
        public bool TypeOptionsVisible { get { return m_TypeOptionsVisible; } set { SetProperty(ref m_TypeOptionsVisible, value); TypeEditButtonsVisible = !value; } }
        private bool m_TypeOptionsVisible;

        [ObservableProperty]
        TypNumber selectedTypNr;
        //public TypNumber SelectedTypNr { get { return m_SelectedTypNr; } set { SetProperty(ref m_SelectedTypNr, value); } }
        //private TypNumber m_SelectedTypNr;

        public Project SelectedProject { get { m_SelectedProject?.TypNumbers?.Sort(); return m_SelectedProject; } set { SetProperty(ref m_SelectedProject, value); UpdateGUI(); } }
        private Project m_SelectedProject;


        public string SeletedRobotType { get { return m_SeletedRobotType; } set { SetProperty(ref m_SeletedRobotType, value); } }
        private string m_SeletedRobotType;

        public List<string> RobotTypes { get => new List<string>() { "KUKA", "FANUC", "ABB" }; }

        public List<int> TypNumbersList { get => CreteTypList(); }


        public bool UseDescrInCollZone { get { return m_UseDescrInCollZone; } set { SetProperty(ref m_UseDescrInCollZone, value); if (SelectedProject?.UseCollDescr != null) SelectedProject.UseCollDescr.Value = value.ToString(); } }
        private bool m_UseDescrInCollZone;

        public bool NotUseDescrInCollZone { get { return m_notUseDescrInCollZone; } set { SetProperty(ref m_notUseDescrInCollZone,value); if (SelectedProject?.UseCollDescr != null) SelectedProject.UseCollDescr.Value = (!value).ToString(); } }
        private bool m_notUseDescrInCollZone;
      
        #endregion properties


        #region commands
        [RelayCommand]
        public void AddType()
        {
            int newNum = 0;
            for (int i = 1; i <= 128; i++)
            {
                if (!SelectedProject.TypNumbers.TypNumber.Any(x => x.Number == i))
                {
                    newNum = i;
                    break;
                }
            }
            var typeToAdd = new TypNumber() { Number = newNum, Editable = true, Name = "New Type" };
            SelectedProject.TypNumbers.TypNumber.Add(typeToAdd);
            SelectedTypNr = typeToAdd;
            EditType();
        }

        [RelayCommand]
        public void DeleteType()
        {
            var num = SelectedTypNr?.Number;
            if (num == null)
                return;
            var type = SelectedProject.TypNumbers.TypNumber.FirstOrDefault(x => x.Number == num);
            SelectedProject.TypNumbers.TypNumber.Remove(type);
            SelectedTypNr = SelectedProject.TypNumbers.TypNumber.FirstOrDefault();
        }

        [RelayCommand]
        public void EditType()
        {
            if (SelectedTypNr == null)
                return;
            copyOfTyp = (TypNumber)SelectedTypNr.Clone();
            SelectedTypNr.SetEditable(true);
            TypeOptionsVisible = false;
            //SelectedProject.TypNumbers.Sort();
        }

        [RelayCommand]
        public void EditOkType()
        {
            SelectedTypNr.SetEditable(false);
            TypeOptionsVisible = true;
            SelectedProject.TypNumbers.Sort();
        }

        [RelayCommand]
        public void EditCancelType()
        {
            SelectedTypNr.GetValuesFromCopy(copyOfTyp);
            SelectedTypNr.SetEditable(false);
            TypeOptionsVisible = true;
        }

        [RelayCommand]
        public void OK(Window window)
        {
            var serializer = new XmlSerializer(typeof(ProjectInfos));
            System.IO.File.WriteAllText(path, string.Empty);
            using (Stream fs = new FileStream(path, FileMode.Open))
            {
                serializer.Serialize(fs, xmlDeserialized);
            }
            if (ExportOnClose)
            {
                CommonLibrary.FilesFromServerManager.TryWriteFileToServer(path,xmlFileName);
            }
            if (window != null)
                window.Close();
        }

        [RelayCommand]
        public void Cancel(Window window) 
        {
            if (window != null)
                window.Close();
        }

        [RelayCommand]
        public void AddProject()
        {
            newproject = new Project(true);
            Projects.Add(newproject);
            SelectedProject = newproject;
            tempProjectName = SelectedProject.Name;
            EditProjectNameVisible = true;
        }

        [RelayCommand]
        public void DeleteProject()
        {
            xmlDeserialized.Project.Remove(SelectedProject);
            //SelectedProject = Projects.Where(x => x != SelectedProject).FirstOrDefault();
            Projects.Remove(SelectedProject);
            SelectedProject = Projects.FirstOrDefault();
        }


        [RelayCommand]
        public void RenameProject()
        {
            tempProjectName = SelectedProject.Name;
            EditProjectNameVisible = true;
        }

        [RelayCommand]
        public void ProjectNameOk()
        {
            if (!xmlDeserialized.Project.Contains(newproject))
                xmlDeserialized.Project.Add(newproject);
            EditProjectNameVisible = false;
            newproject = null;
        }

        [RelayCommand]
        public void ProjectNameCancel()
        {
            if (newproject != null && Projects.Contains(newproject))
            {
                SelectedProject = Projects.Where(x => x != SelectedProject).FirstOrDefault();
                Projects.Remove(newproject);
                SelectedProject = Projects.FirstOrDefault();
            }
            else
                SelectedProject.Name = tempProjectName;
            EditProjectNameVisible = false;
            newproject = null;
        }
        #endregion commands

        #region private methods
        private void Deserialize()
        {
            Projects = new ObservableCollection<Project>();
            
            var serializer = new XmlSerializer(typeof(ProjectInfos));
            using (Stream reader = new FileStream(path, FileMode.Open))
            {
                xmlDeserialized = (ProjectInfos)serializer.Deserialize(reader);
                xmlDeserialized.Project.ForEach(x => Projects.Add(x));
            }
        }

        private void UpdateGUI()
        {
            SeletedRobotType = SelectedProject?.RobotType;
            MainTypID = SelectedProject == null ? 0 : SelectedProject.TypIDMain.Value;
            CommunalTypID = SelectedProject == null ? 0 : SelectedProject.TypIDCommunal.Value;
            ApplicationTypes = SelectedProject?.ApplicationTypes;
            UseDescrInCollZone = BoolParser(SelectedProject?.UseCollDescr?.Value);
            NotUseDescrInCollZone = !BoolParser(SelectedProject?.UseCollDescr?.Value);
        }

        private List<int> CreteTypList()
        {
            var list = new List<int>();
            for (int i = 1; i <= 128; i++)
                list.Add(i);
            return list;
        }

        private bool BoolParser(string str)
        {
            if (!string.IsNullOrEmpty(str) && str.Equals("true", StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }

        #endregion private methods
    }
}
