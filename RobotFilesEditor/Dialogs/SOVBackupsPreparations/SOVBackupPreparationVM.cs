using CommonLibrary;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RobotFilesEditor.Model.Operations.BackupSyntaxValidation;
using RobotFilesEditor.Model.Operations.DataClass;
using RobotFilesEditor.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RobotFilesEditor.Dialogs.SOVBackupsPreparations
{
    public class SOVBackupPreparationVM : ViewModelBase
    {
        #region fields
        bool isSovField;
        ObservableCollection<SovBackupsPreparationResult> allResults;
        GlobalData.RobotController robotType;
        #endregion

        #region ctor
        public SOVBackupPreparationVM(bool isSOV, GlobalData.RobotController robot)
        {
            SetDefaults(isSOV, robot);
        }

        public SOVBackupPreparationVM(bool isSOV, GlobalData.RobotController robot, string filePath)
        {
            SetDefaults(isSOV, robot);
            BackupFilePath = filePath;
            ScanCommandExecute();
        }
        #endregion

        #region properties
        private string backupFilePath;
        public string BackupFilePath
        {
            get { return backupFilePath; }
            set
            {
                if (File.Exists(value))
                    Set(ref backupFilePath, value);
                CheckScanEnable();
            }
        }

        private string pathsDirPath;
        public string PathsDirPath
        {
            get { return pathsDirPath; }
            set
            {
                if (Directory.Exists(value))
                    Set(ref pathsDirPath, value);
                CheckScanEnable();
            }
        }

        private string okContent;
        public string OkContent
        {
            get { return okContent; }
            set
            {              
                Set(ref okContent, value);
            }
        }

        private string warningContent;
        public string WarningContent
        {
            get { return warningContent; }
            set
            {                 
                Set(ref warningContent, value);
            }
        }

        private string errorContent;
        public string ErrorContent
        {
            get { return errorContent; }
            set
            {
                Set(ref errorContent, value);
            }
        }

        private string infoContent;
        public string InfoContent
        {
            get { return infoContent; }
            set
            {
                Set(ref infoContent, value);
            }
        }

        private ObservableCollection<SovBackupsPreparationResult> logContent;
        public ObservableCollection<SovBackupsPreparationResult> LogContent
        {
            get { return logContent; }
            set
            {
                Set(ref logContent, value);
            }
        }

        private bool isScanEnabled;
        public bool IsScanEnabled
        {
            get { return isScanEnabled; }
            set
            {
                Set(ref isScanEnabled, value);
            }
        }

        private bool isExecuteEnabled;
        public bool IsExecuteEnabled
        {
            get { return isExecuteEnabled; }
            set
            {
                Set(ref isExecuteEnabled, value);
            }
        }

        private Visibility logVisibility;
        public Visibility LogVisibility
        {
            get { return logVisibility; }
            set
            {
                Set(ref logVisibility, value);
            }
        }

        private Visibility isDirVisible;
        public Visibility IsDirVisible
        {
            get { return isDirVisible; }
            set
            {
                Set(ref isDirVisible, value);
            }
        }

        private bool oKsFilterChecked;
        public bool OKsFilterChecked
        {
            get { return oKsFilterChecked; }
            set
            {
                Set(ref oKsFilterChecked, value);
                CheckFilters();
            }
        }

        private bool warningsFilterChecked;
        public bool WarningsFilterChecked
        {
            get { return warningsFilterChecked; }
            set
            {
                Set(ref warningsFilterChecked, value);
                CheckFilters();
            }
        }

        private bool errorsFilterChecked;
        public bool ErrorsFilterChecked
        {
            get { return errorsFilterChecked; }
            set
            {
                Set(ref errorsFilterChecked, value);
                CheckFilters();
            }
        }

        private bool infoFilterChecked;
        public bool InfoFilterChecked
        {
            get { return infoFilterChecked; }
            set
            {
                Set(ref infoFilterChecked, value);
                CheckFilters();
            }
        }
        #endregion

        #region commands
        public ICommand SelectBackupFile { get; set; }
        public ICommand SelectPathsDir { get; set; }
        public ICommand ExecuteCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand ScanCommand { get; set; }
        #endregion

        #region methods
        private void SetCommands()
        {
            SelectBackupFile = new RelayCommand(SelectBackupFileExecute);
            SelectPathsDir = new RelayCommand(SelectPathsDirExecute);
            ExecuteCommand = new RelayCommand(ExecuteCommandExecute);
            CancelCommand = new RelayCommand(CancelCommandExecute);
            ScanCommand = new RelayCommand(ScanCommandExecute);
        }

        private void ScanCommandExecute()
        {
            if (isSovField)
            {
                SOVBackupPreparationModel scanResult = new SOVBackupPreparationModel(BackupFilePath, PathsDirPath, robotType);
                allResults = scanResult.LogContent;
                LogContent = scanResult.LogContent;
            }
            else
            {
                IBackupSyntaxValidator validator;
                switch (robotType)
                {
                    case GlobalData.RobotController.KUKA:
                        {
                            validator = new KUKASynataxValidator(BackupFilePath);
                            break;
                        }
                    case GlobalData.RobotController.FANUC:
                        {
                            validator = new FanucSyntaxValidator(BackupFilePath);
                            break;
                        }
                    default:
                        {
                            validator = new FaultSyntaxValidator();
                            break;
                        }
                }
                allResults = CommonMethods.ToObservableCollection(validator.ErrorsFound);
                LogContent = CommonMethods.ToObservableCollection(validator.ErrorsFound);

            }
            if (LogContent != null && LogContent.Count > 0)
            {
                LogVisibility = Visibility.Visible;
                OKsFilterChecked = true;
                WarningsFilterChecked = true;
                ErrorsFilterChecked = true;
                InfoFilterChecked = true;
                OkContent = "OK (" + allResults.Where(x => x.InfoType == GlobalData.SovLogContentInfoTypes.OK).ToList().Count.ToString() + ")";
                WarningContent = "Warnings (" + allResults.Where(x => x.InfoType == GlobalData.SovLogContentInfoTypes.Warning).ToList().Count.ToString() + ")";
                ErrorContent = "Errors (" + allResults.Where(x => x.InfoType == GlobalData.SovLogContentInfoTypes.Error).ToList().Count.ToString() + ")";
                InfoContent = "Infos (" + allResults.Where(x => x.InfoType == GlobalData.SovLogContentInfoTypes.Information).ToList().Count.ToString() + ")";
            }
            else
                LogVisibility = Visibility.Collapsed;
        }

        private void CancelCommandExecute()
        {
            var window = Application.Current.Windows.Cast<Window>().Single(w => w.DataContext == this);
            window.DialogResult = false;
            window.Close();
        }

        private void ExecuteCommandExecute()
        {
            var window = Application.Current.Windows.Cast<Window>().Single(w => w.DataContext == this);
            window.DialogResult = true;
            window.Close();
        }

        private void SelectPathsDirExecute()
        {
            PathsDirPath = CommonMethods.SelectDirOrFile(true);
        }

        private void SelectBackupFileExecute()
        {
            BackupFilePath = CommonMethods.SelectDirOrFile(false, filter1Descr: "Zip file", filter1: "*.zip");
        }

        private void CheckScanEnable()
        {
            if (!string.IsNullOrEmpty(BackupFilePath) && (!string.IsNullOrEmpty(PathsDirPath) || !isSovField))
                IsScanEnabled = true;
            else
                IsScanEnabled = false;
        }

        private void CheckFilters()
        {
            ObservableCollection<SovBackupsPreparationResult> tempResult = new ObservableCollection<SovBackupsPreparationResult>();
            foreach (var item in allResults)
            {
                if (OKsFilterChecked && item.InfoType == GlobalData.SovLogContentInfoTypes.OK || WarningsFilterChecked && item.InfoType == GlobalData.SovLogContentInfoTypes.Warning || ErrorsFilterChecked && item.InfoType == GlobalData.SovLogContentInfoTypes.Error || InfoFilterChecked && item.InfoType == GlobalData.SovLogContentInfoTypes.Information)
                    tempResult.Add(item);
            }
            LogContent = tempResult;
        }

        private void SetDefaults(bool isSOV, GlobalData.RobotController robot)
        {
            allResults = new ObservableCollection<SovBackupsPreparationResult>();
            robotType = robot;
            isSovField = isSOV;
            LogVisibility = Visibility.Collapsed;
            if (isSOV)
                IsDirVisible = Visibility.Visible;
            else
                IsDirVisible = Visibility.Collapsed;
            OKsFilterChecked = true;
            WarningsFilterChecked = true;
            ErrorsFilterChecked = true;
            InfoFilterChecked = true;
            SetCommands();
        }
        #endregion
    }
}
