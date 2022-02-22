using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System.Windows;
using RobotFilesEditor.Dialogs;
using RobotFilesEditor.Model.Operations;
using System.Xml;

namespace RobotFilesEditor.ViewModel
{
    public class MainViewModel : ViewModelBase, IDisposable
    {
        #region fields
        private enum OrgController { KUKA, FANUC};
        #endregion

            #region Controls         
        public ObservableCollection<ControlItem> ControlerChooser
        {
            get;
            set;
        }
        public ObservableCollection<ControlItem> MoveFilesOperations
        {
            get;
            set;
        }
        public ObservableCollection<ControlItem> CopyFilesOperations
        {
            get;
            set;
        }
        public ObservableCollection<ControlItem> RemoveFilesOperations
        {
            get;
            set;
        }

        public ObservableCollection<ControlItem> CopyTextFromFilesOperations
        {
            get;
            set;
        }
        public ObservableCollection<ControlItem> CutTextFromFilesOperations
        {
            get;
            set;
        }

        public ObservableCollection<ControlItem> AllOperations
        {
            get;
            set;
        }

        public bool EnableWindow
        {
            get; set;
        }

        public string MoveFilesOperationsVisibility
        {
            get {
                if (MoveFilesOperations.Count > 0)
                {
                    return "Visible";
                }
                else
                {
                    return "Collapsed";
                }
            }
        }
        public string CopyFilesOperationsVisibility
        {          
            get
            {
                if (CopyFilesOperations.Count > 0)
                {
                    return "Visible";
                }
                else
                {
                    return "Collapsed";
                }
            }
        }
        public string CopyTextFromFilesOperationsVisibility
        {           
            get
            {
                if (CopyTextFromFilesOperations.Count > 0)
                {
                    return "Visible";
                }
                else
                {
                    return "Collapsed";
                }
            }
        }
        public string CutTextFromFilesOperationsVisibility
        {
            get
            {
                if (CutTextFromFilesOperations.Count > 0)
                {
                    return "Visible";
                }
                else
                {
                    return "Collapsed";
                }
            }
        }
        public string RemoveFilesOperationsVisibility
        {            
            get
            {
                if (RemoveFilesOperations.Count > 0)
                {
                    return "Visible";
                }
                else
                {
                    return "Collapsed";
                }
            }
        }

        public string SourcePath
        {
            get { return _sourcePath; }
            set
            {
                if (Directory.Exists(value) && _sourcePath != value)
                {
                    _sourcePath = value;
                    RaisePropertyChanged(nameof(SourcePath));
                    WriteToApplicationConfig(false);
                    GlobalData.SourcePath = value;
                    if(SelectedControler!=null)
                    {
                        SelectedControler.SourcePath = SourcePath;
                        foreach(var operation in AllOperations)
                        {
                            operation.Operations.ForEach(x => x.SourcePath = SourcePath);
                        }
                        //ShowAllOperationsResults();
                        foreach (var controller in ControlerChooser)
                            controller.Checked = false;
                    }
                    //SelectedControler = null;
                }
            }
        }

        private bool _englishSelected;
        public bool EnglishSelected
        {
            get { return _englishSelected; }
            set
            {
                Set( ref _englishSelected, value);
                if (value == true)
                    SrcValidator.language = "EN";
                CanScanContent = true;
            }
        }

        private bool _deutschSelected;
        public bool DeutschSelected
        {
            get { return _deutschSelected; }
            set
            {
                Set(ref _deutschSelected, value);
                if (value == true)
                    SrcValidator.language = "DE";
                CanScanContent = true;
            }
        }


        public string DestinationPath
        {
            get { return _destinationPath; }
            set
            {             
                if(Directory.Exists(value) && _destinationPath != value)
                {
                    _destinationPath = value;
                    RaisePropertyChanged(nameof(DestinationPath));
                    GlobalData.DestinationPath = DestinationPath;
                    WriteToApplicationConfig(true);
                    if (SelectedControler != null)
                    {
                        SelectedControler.DestinationPath = DestinationPath;
                        foreach (var operation in AllOperations)
                        {
                            operation.Operations.ForEach(x => x.DestinationPath = DestinationPath);
                        }
                        foreach (var controller in ControlerChooser)
                            controller.Checked = false;
                        //ShowAllOperationsResults();
                    }                    
                }              
            }
        }

        public bool ContinueWithoutConfirm
        {
            get { return _continueWithoutConfirm; }
            set
            {
                if(_continueWithoutConfirm!=value)
                {
                    _continueWithoutConfirm = value;                   
                    RaisePropertyChanged(nameof(ContinueWithoutConfirm));
                    RaisePropertyChanged(nameof(ConfirmButtonEnabled));
                }
            }
        }

        private bool canScanContent;
        public bool CanScanContent
        {
            get { return canScanContent; }
            set
            {
                if (canScanContent != value)
                {
                    canScanContent = value;
                    RaisePropertyChanged(()=>CanScanContent);
                }
            }
        }

        public bool ConfirmButtonEnabled
        {
            get { return _continueWithoutConfirm == false; }            
        }
      
        #endregion Controls

        public Controler SelectedControler
        {
            get { return _selectedControler; }
            set
            {
                if (_selectedControler != value)
                {
                    _selectedControler = value;
                    RaisePropertyChanged(nameof(SelectedControler));

                    if(SelectedControler?.SourcePath!=SourcePath)
                    {
                        SelectedControler.SourcePath = SourcePath;
                    }

                    if(SelectedControler?.DestinationPath!=DestinationPath)
                    {
                        SelectedControler.DestinationPath = DestinationPath;
                    }                    
                }
            }
        }
        public List<Controler> Controlers
        {
            get { return _controlers; }
            set
            {
                if (_controlers != value)
                {
                    _controlers = value;
                    RaisePropertyChanged(nameof(Controlers));
                }
            }
        }

        string _selectedItemFromList;
        public string SelectedItemFromList
        {
            get { return _selectedItemFromList; }
            set
            {
                if (_selectedItemFromList != value)
                {
                    _selectedItemFromList = value;
                    RaisePropertyChanged(() => _selectedItemFromList);
                }
            }
        }

        private bool _checkOrder;
        public bool CheckOrder
        {
            get { return _checkOrder; }
            set
            {
                if (_checkOrder != value)
                {
                    _checkOrder = value;
                    RaisePropertyChanged(() => CheckOrder);
                    GlobalData.CheckOrder = value;
                }
            }
        }

        private Visibility debugVisibility;
        public Visibility DebugVisibility
        {
            get { return debugVisibility; }
            set
            {
                //debugVisibility = value;
                if (Environment.UserName.ToLower().Contains("ttrojniar"))
                    value = Visibility.Visible;
                else
                    value = Visibility.Collapsed;
                debugVisibility = value;
                RaisePropertyChanged(() => DebugVisibility);
            }
        }



        private List<Controler> _controlers;
        private Controler _selectedControler;
        private string _sourcePath;
        private string _destinationPath;
        private bool _continueWithoutConfirm;

        public MainViewModel(List<Controler> controlers)
        {
            if (!ViewModelBase.IsInDesignModeStatic)
            {
                DebugVisibility = Visibility.Visible;
                CheckOrder = false;
                ControlerChooser = new ObservableCollection<ControlItem>();
                MoveFilesOperations = new ObservableCollection<ControlItem>();
                CopyFilesOperations = new ObservableCollection<ControlItem>();
                CopyTextFromFilesOperations = new ObservableCollection<ControlItem>();
                RemoveFilesOperations = new ObservableCollection<ControlItem>();
                CutTextFromFilesOperations = new ObservableCollection<ControlItem>();

                AllOperations = new ObservableCollection<ControlItem>();

                Controlers = controlers;
                SourcePath = Controlers.FirstOrDefault().SourcePath;
                DestinationPath = Controlers.FirstOrDefault().DestinationPath;
                SetCommands();
                CreateControlerChooser();
                //DeutschSelected = true;
                CanScanContent = false;
            }
        }

        #region ControlersCreator
        private void CreateControlerChooser()
        {
            ControlerChooser.Clear();

            Controlers.ForEach(controler =>
            {
                var controlerChooserSelectorItem = new ControlItem(controler.ContolerType);
                controlerChooserSelectorItem.ControlItemSelected += ControlerChooser_Click;
                ControlerChooser.Add(controlerChooserSelectorItem);
            });
        }

        private void CreateOperationsControls()
        {
            try
            {
                MoveFilesOperations.Clear();
                CopyFilesOperations.Clear();
                CopyTextFromFilesOperations.Clear();
                RemoveFilesOperations.Clear();
                CutTextFromFilesOperations.Clear();
                AllOperations.Clear();
                List<string> operations = new List<string>();
                var opertionGroups = SelectedControler.Operations.GroupBy(x => x.OperationName);

                foreach (var operationGroup in opertionGroups)
                {
                    var operation = operationGroup.FirstOrDefault();

                    if (operation != null)
                    {
                        var controlItem = new ControlItem(operation.OperationName);

                        switch (operation.ActionType)
                        {
                            case GlobalData.Action.Move:
                                {
                                    controlItem.OrderNumber = 1;
                                    MoveFilesOperations.Add(controlItem);
                                }
                                break;
                            case GlobalData.Action.Copy:
                                {
                                    controlItem.OrderNumber = 0;
                                    CopyFilesOperations.Add(controlItem);
                                }
                                break;
                            case GlobalData.Action.Remove:
                                {
                                    controlItem.OrderNumber = 3;
                                    RemoveFilesOperations.Add(controlItem);
                                }
                                break;
                            case GlobalData.Action.CopyData:
                                {
                                    controlItem.OrderNumber = 4;
                                    CopyTextFromFilesOperations.Add(controlItem);
                                }
                                break;
                           
                            case GlobalData.Action.CutData:
                                {
                                    controlItem.OrderNumber = 5;
                                    CutTextFromFilesOperations.Add(controlItem);
                                }break;
                        }

                        AllOperations.Add(controlItem);
                    }
                }

                MoveFilesOperations.Distinct();
                CopyFilesOperations.Distinct();
                CopyTextFromFilesOperations.Distinct();
                RemoveFilesOperations.Distinct();
                CutTextFromFilesOperations.Distinct();
                AllOperations.Distinct();
                AllOperations = new ObservableCollection<ControlItem>(AllOperations.OrderBy(x=>x.OrderNumber));

                RaisePropertyChanged(nameof(MoveFilesOperationsVisibility));
                RaisePropertyChanged(nameof(CopyFilesOperationsVisibility));
                RaisePropertyChanged(nameof(CopyTextFromFilesOperationsVisibility));
                RaisePropertyChanged(nameof(RemoveFilesOperationsVisibility));
                RaisePropertyChanged(nameof(CutTextFromFilesOperationsVisibility));

                LoadOperations();
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                throw ex;
            }          
        }
        public void Dispose()
        {
            ControlerChooser?.ToList().ForEach(item => item.ControlItemSelected -= ControlerChooser_Click);
        }

        #endregion ControlersCreator

        #region Command
        public ICommand SetSourcePathCommand { get; set; }
        public ICommand SetDestinationPathCommand { get; set; }
        public ICommand ChangeName { get; set; }
        public ICommand OpenLog { get; set; }
        public ICommand Mirror { get; set; }
        public ICommand Reset { get; set; }
        public ICommand OpenDest { get; set; }
        public ICommand FillExcel { get; set; }
        public ICommand ExecuteAllOperationsCommand { get; set; }
        public ICommand CreateOrgsCommand { get; set; }
        public ICommand ReadConfigDat { get; set; }
        public ICommand CreateGripperCommand { get; set; }
        public ICommand ClosingCommand { get; set; }
        public ICommand OpenInNotepadCommand { get; set; }
        public ICommand SetCorrectCollisionDescription { get; set; }
        public ICommand FixOffline { get; set; }
        public ICommand SpotToPTP { get; set; }
        public ICommand StoppingDistances { get; set; }
        public ICommand ReadSpotPoints { get; set; }
        public ICommand ReadBackupForWB { get; set; }
        public ICommand RenumberPoints { get; set; }
        public ICommand TypIdChange { get; set; }
        public ICommand CheckBases { get; set; }
        public ICommand FillTypID { get; set; }
        public ICommand CompareBosch { get; set; }
        public ICommand RetrieveBackups { get; set; }
        public ICommand PrepareSOVBackup { get; set; }        
        public ICommand ABBandFanucChecksum { get; set; }
        public ICommand SafetyTools { get; set; }
        public ICommand ValidateBackup { get; set; }
        public ICommand ABBHelper { get; set; }
        public ICommand FanucMirror { get; set; }
        public ICommand CreateGripperXML { get; set; }
        public ICommand FixPTPandLIN { get; set; }
        public ICommand RenumberLinesFanuc { get; set; }
        public ICommand ConvertTrello { get; set; }
        public ICommand RenamePointsABB { get; set; }
        public ICommand RobKalDatProp { get; set; }
        public ICommand ReadSpotPointsABB { get; set; }
        public ICommand CollisionsForPS { get; set; }
        public ICommand ModifyE1 { get; set; }
        public ICommand SasFillerFromBackup { get; set; }
        public ICommand GetMenge { get; set; }
        public ICommand ReadBackupForWBABB { get; set; }
        public ICommand CompareSpots { get; set; }
        public ICommand TempExec { get; set; }
        public ICommand DividePathByColls { get; set; }
        public ICommand ScanContent { get; set; }
        public ICommand ShiftBase { get; set; }        
        public ICommand GenerateOrgsFanuc { get; set; }
        public ICommand CompareSOVAndOLP { get; set; }
        public ICommand ValidateComments { get; set; }

        private void SetCommands()
        {
            ChangeName = new RelayCommand(ChangeNameExecute);
            OpenLog = new RelayCommand(OpenLogExecute);
            Mirror = new RelayCommand(MirrorExecute);
            Reset = new RelayCommand(ResetExecute);
            ReadConfigDat = new RelayCommand(ReadConfigDatExecute);
            OpenDest = new RelayCommand(OpenDestExecute);
            FillExcel = new RelayCommand(FillExcelExecute);
            FixOffline = new RelayCommand(FixOfflineExecute);
            SpotToPTP = new RelayCommand(SpotToPTPExecute);
            StoppingDistances = new RelayCommand(StoppingDistancesExecute);
            RenumberPoints = new RelayCommand(RenumberPointsExecute);
            ReadSpotPoints = new RelayCommand(ReadSpotPointsExecute);
            SetCorrectCollisionDescription = new RelayCommand(SetCorrectCollisionDescriptionExecute);
            CreateOrgsCommand = new RelayCommand(CreateOrgsCommandExecute);
            CreateGripperCommand = new RelayCommand(CreateGripperCommandExecute);
            SetSourcePathCommand = new RelayCommand(SetSourcePathCommandExecute);
            SetDestinationPathCommand = new RelayCommand(SetDestinationPathCommandExecute);
            ExecuteAllOperationsCommand = new RelayCommand(ExecuteAllOperationsCommandExecute);
            OpenInNotepadCommand = new RelayCommand(OpenInNotepadCommandExecute);
            ClosingCommand = new RelayCommand(ClosingCommandExecute);
            ReadBackupForWB = new RelayCommand(ReadBackupForWBExecute);
            TypIdChange = new RelayCommand(TypIdChangeExecute);
            CheckBases = new RelayCommand(CheckBasesExecute);
            FillTypID = new RelayCommand(FillTypIDExecute);
            CompareBosch = new RelayCommand(CompareBoschExecute);
            RetrieveBackups = new RelayCommand(RetrieveBackupsExecute);
            PrepareSOVBackup = new RelayCommand(PrepareSOVBackupExecute);
            ABBandFanucChecksum = new RelayCommand(ABBandFanucChecksumExecute);
            SafetyTools = new RelayCommand(SafetyToolsExecute);
            ValidateBackup = new RelayCommand(ValidateBackupExecute);
            ABBHelper = new RelayCommand(ABBHelperExecute);
            FanucMirror = new RelayCommand(FanucMirrorExecute);
            CreateGripperXML = new RelayCommand(CreateGripperXMLExecute);
            FixPTPandLIN = new RelayCommand(FixPTPandLINExecute);
            RenumberLinesFanuc = new RelayCommand(RenumberLinesFanucExecute);
            ConvertTrello = new RelayCommand(ConvertTrelloExecute);
            RenamePointsABB = new RelayCommand(RenamePointsABBExecute);
            RobKalDatProp = new RelayCommand(RobKalDatExecute);
            ReadSpotPointsABB = new RelayCommand(ReadSpotPointsABBExecute);
            CollisionsForPS = new RelayCommand(CollisionsForPSExecute);
            ModifyE1 = new RelayCommand(ModifyE1Execute);
            SasFillerFromBackup = new RelayCommand(SasFillerFromBackupExecute);
            GetMenge = new RelayCommand(GetMengeExecute);
            ReadBackupForWBABB = new RelayCommand(ReadBackupForWBABBExecute);
            CompareSpots = new RelayCommand(CompareSpotsExecute);
            TempExec = new RelayCommand(TempExecExecute);
            DividePathByColls = new RelayCommand(DividePathByCollsExecute);
            ScanContent = new RelayCommand(ScanContentExecute);
            ShiftBase = new RelayCommand(ShiftBaseExecute);
            GenerateOrgsFanuc = new RelayCommand(GenerateOrgsFanucExecute);
            CompareSOVAndOLP = new RelayCommand(CompareSOVAndOLPExecute);
            ValidateComments = new RelayCommand(ValidateCommentsFanucExecute);
        }

        private void ValidateCommentsFanucExecute()
        {
            var validator = new Model.Operations.FANUC.FanucValidateCommentsMethods();
        }

        private void CompareSOVAndOLPExecute()
        {
            var comparer = new Model.Operations.FANUC.FanucCompareSOVToOLPMethods();
        }

        private void ShiftBaseExecute()
        {
            ShiftBaseMethods.Execute();
        }

        private void ScanContentExecute()
        {
            GlobalData.CurrentOpNumFanuc = 0;
            var controller = ControlerChooser.FirstOrDefault(x => x.Checked == true);
            if (controller!=null)
                controller.ClickedCommandExecuteTest(true);
        }


        private void DividePathByCollsExecute()
        {
            Model.Operations.OLPTools.DividePathByCollsMethods divColls = new Model.Operations.OLPTools.DividePathByCollsMethods();
            divColls.Execute();
        }

        private void TempExecExecute()
        {
            Model.TempFunctions.TempClass.Execute();
        }

        private void CompareSpotsExecute()
        {
            Model.Operations.CompareSpotsExecute.Execute();
        }

        private void ReadBackupForWBABBExecute()
        {
            ReadBackupsForWBMethods.Execute("ABB");
        }

        private void GetMengeExecute()
        {
            GetMengeMethods.Execute();
        }

        private void SasFillerFromBackupExecute()
        {
            Model.Operations.SasFillerFromBackupMethods.Execute();
        }

        private void ModifyE1Execute()
        {
            Model.Operations.TempScripts.ModifyE1Value.Execute();
        }

        private void CollisionsForPSExecute()
        {
            CollisionsForPSMethods.Execute();
        }

        private void ReadSpotPointsABBExecute()
        {
            ReadSpotsMethods.Execute("ABB");
        }

        private void RobKalDatExecute()
        {
            RobKalDat.MainWindow window = new RobKalDat.MainWindow();
            window.Owner = Application.Current.Windows
            .Cast<Window>()
            .Single(w => w.DataContext == this);
            window.ShowDialog();
        }

        private void RenamePointsABBExecute()
        {
            RobotPointsRenumber.MainWindow window = new RobotPointsRenumber.MainWindow();
            //window.Owner = Application.Current.Windows
            //.Cast<Window>()
            //.Single(w => w.DataContext == this);
            //window.ShowDialog();
        }

        private void ConvertTrelloExecute()
        {
            TrelloConvertMethods.Execute();
        }

        private void RenumberLinesFanucExecute()
        {
            RenumberLinesFanucMethods.Execute();
        }

        private void FixPTPandLINExecute()
        {
            FixPtpAndLinMethods.Execute();
        }

        private void CreateGripperXMLExecute()
        {
            CreateGripperMethods.CreateGripperXMLExecute();
        }

        private void FanucMirrorExecute()
        {
            Fanuc_mirro.MainWindow window = new Fanuc_mirro.MainWindow();
            window.Show();
        }

        private void ABBHelperExecute()
        {
            ABB_add_spaces.MainWindow window = new ABB_add_spaces.MainWindow();
            window.Show();
        }

        private void ValidateBackupExecute()
        {
            Model.Operations.BackupSyntaxValidation.KUKASynataxValidator.Execute();
        }

        private void SafetyToolsExecute()
        {
            
            RobotSafetyGenerator.MainWindow window = new RobotSafetyGenerator.MainWindow();
            window.Show();
        }

        private void ABBandFanucChecksumExecute()
        {
            //ABBAndFanucChecksumMethods.Execute();
        }

        private void PrepareSOVBackupExecute()
        {
            PrepareSOVBackupMethods.Execute();
        }

        private void RetrieveBackupsExecute()
        {
            RetrieveBackupMethods.Execute();
        }

        private void CompareBoschExecute()
        {
            CompareBoschMethods.Execute();
        }

        private void FillTypIDExecute()
        {
            FillTypIDMethods.Execute();
        }

        private void CheckBasesExecute()
        {
            CheckBasesMethods.Execute();
        }

        private async void TypIdChangeExecute()
        {
            TypIdChangeMethods typIdMethods = new TypIdChangeMethods();
            await typIdMethods.Execute();
        }

        private void RenumberPointsExecute()
        {
            RenumberPointsMethods.Execute(); ;
        }

        private void ReadBackupForWBExecute()
        {
            ReadBackupsForWBMethods.Execute("KUKA");
        }

        private void ReadSpotPointsExecute()
        {
            ReadSpotsMethods.Execute("KUKA");
        }

        private void StoppingDistancesExecute()
        {
            StoppingDistanceMethods.Execute();
        }

        private void SpotToPTPExecute()
        {
            SpotToPTPMethods.ChangeToPTP();
        }

        private void FixOfflineExecute()
        {
            FixOfflineMethods.FixOffline();
        }

        private void ReadConfigDatExecute()
        {
            GetGripperFromConfigDatMethods.ReadConfigDat();
        }

        private void FillExcelExecute()
        {
            FillExcelMethods.FillExcel();
        }

        private void CreateGripperCommandExecute()
        {
            OnCreateGripperCommandExecute();
        }

        private void ResetExecute()
        {
            Model.Operations.SrcValidator.DeleteOldConfigFile();
        }

        private void MirrorExecute()
        {
            Model.Operations.SrcValidator.MirrorPaths();
        }

        private void OpenLogExecute()
        {
            OnOpenLogExecute();
        }

        private void ChangeNameExecute()
        {
            string name = null;
            while (name == null || name.ToLower() == "default" || name == "")
            {
                var vm = new ChangeNameViewModel();
                ChangeName sW = new ChangeName(vm);
                var dialogResult = sW.ShowDialog();
                name = vm.Name;
            }
        }

        private void CreateOrgsCommandExecute()
        {
            OnCreateOrgsCommandExecute(OrgController.KUKA);
        }

        private void GenerateOrgsFanucExecute()
        {
            OnCreateOrgsCommandExecute(OrgController.FANUC);
        }

        private void OnCreateOrgsCommandExecute(OrgController controller)
        {
            if (GlobalData.SrcPathsAndJobs == null || GlobalData.SrcPathsAndJobs.Count == 0)
            {
                MessageBox.Show("No paths found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (GlobalData.ControllerType == "KRC2 L6" || GlobalData.ControllerType == "KRC2 V8" || GlobalData.ControllerType == "KRC4" || GlobalData.ControllerType == "FANUC")
                {
                    var vm = new CreateOrgsViewModel(GlobalData.SrcPathsAndJobs, GlobalData.Jobs);
                    CreateOrgs sW = new CreateOrgs(vm);
                    var dialogResult = sW.ShowDialog();
                    if ((bool)dialogResult)
                    {
                        switch (controller)
                        {
                            case OrgController.KUKA:
                                {
                                    CreateOrgsMethods createOrgsMethods = new CreateOrgsMethods();
                                    createOrgsMethods.CreateOrgs(vm.DictOrgsElements, vm.SelectedToolsNumber, vm.SafeRobot, vm.SelectedLine, vm.SelectedGunsNumber, vm.SelectedPLC, vm.RobotName, vm.SelectedStartOrgNum, vm.WaitForInHome);
                                    break;
                                }
                            case OrgController.FANUC:
                                {
                                    Model.Operations.FANUC.FanucOrgs org = new Model.Operations.FANUC.FanucOrgs(vm);
                                    org.CreateOrgs();
                                    break;
                                }
                        }
                        
                    }
                }
                else
                    MessageBox.Show("Use SAS to generate orgs", "Warning", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SetCorrectCollisionDescriptionExecute()
        {
            
        }

        private void OpenInNotepadCommandExecute()
        {
            OnOpenInNotepadCommandExecute();
        }
        #endregion Command

        private void ControlerChooser_Click(object sender, ControlItem e)
        {
            try
            {
                SelectedControler = Controlers.FirstOrDefault(x => x.ContolerType == e.Title);
                CreateOperationsControls();
                ShowAllOperationsResults();
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                throw ex;
            }           
        }

        private void SetSourcePathCommandExecute()
        {
            try
            {
                SourcePath = SetPath(SourcePath);                
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                MessageBoxResult ExeptionMessage = MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }          
        }

        private void SetDestinationPathCommandExecute()
        {
            try
            {
                DestinationPath = SetPath(DestinationPath);                
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                MessageBoxResult ExeptionMessage = MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }          
        }

        private string SetPath(string path)
        {
            try
            {
                using (var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog())
                {
                    folderBrowserDialog.SelectedPath = path;
                    System.Windows.Forms.DialogResult result = folderBrowserDialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        path = folderBrowserDialog.SelectedPath;
                    }
                }
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                throw ex;
            }          

            return path;
        }

        private void ClosingCommandExecute()
        {
            try
            {
                FileDeseralization fileDeserialization = new FileDeseralization();
                fileDeserialization.SaveNewPaths(SourcePath, DestinationPath);
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                throw ex;
            }          
        }     

        private void LoadOperations()
        {
            foreach(var operation in AllOperations)
            {
                operation.Operations = SelectedControler.Operations.Where(x => x.OperationName == operation.Title).ToList();
            }
        }

        private void ExecuteAllOperationsCommandExecute()
        {
            if ((AllOperations?.Count > 0) == false)
            {
                MessageBoxResult ExeptionMessage = MessageBox.Show("No selected controler", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            foreach (var operation in AllOperations)
            {
                operation.ExecuteOperationCommandExecute();
            }
            if (GlobalData.ControllerType == "FANUC")
            { 
                Model.Operations.FANUC.FanucCreateSOVBackup fanucBackup = new Model.Operations.FANUC.FanucCreateSOVBackup(true);
            }
        }

        private void ShowAllOperationsResults()
        {
            SrcValidator.GetCopyOperationsCount(AllOperations);
            foreach (var operation in AllOperations)
            {
                operation.PreviewOperationCommandExecute();
            }
            CommonLibrary.CommonMethods.CreateLogFile(SrcValidator.logFileContent, "\\log.txt");
        }

        private void OnOpenInNotepadCommandExecute()
        {
            
        }

        private void OnCreateGripperCommandExecute()
        {
            if (GlobalData.ControllerType == "KRC2 L6" || GlobalData.ControllerType == "KRC2 V8" || GlobalData.ControllerType == "KRC4")
            {
                var vm = new CreateGripperViewModel();
                bool? dialogResult = false;
                while (!CreateGripperMethods.ValidateData(vm, dialogResult))
                {
                    CreateGripper sW = new CreateGripper(vm);
                    dialogResult = sW.ShowDialog();
                    sW.Close();
                    if (dialogResult == false)
                        break;
                }

                if (vm.Success)
                {
                    CreateGripperMethods.CreateGripper(vm.GripperElements, vm.GripperElementSensors, vm.NrOfInputs, vm.NrOfOutputs, vm.SelectedGripperNumber, vm.HasSoftStart);
                    if (GlobalData.ControllerType == "KRC4")
                        CreateGripperMethods.CreateGripperXML(vm.GripperElements, vm.GripperElementSensors, vm.NrOfInputs, vm.NrOfOutputs, vm.SelectedGripperNumber, vm.HasSoftStart, vm.StartAddresses.First().Value,vm.OutsForClose.First().Value);
                }
            }
            else
                MessageBox.Show("Use SAS to generate grippers", "Warning", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void WriteToApplicationConfig(bool isDestination)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(GlobalData.PathFile);
            XmlNode root = doc.DocumentElement;
            if (isDestination)
            {
                XmlNode myNode = root.SelectSingleNode("DestinationPath");
                myNode.InnerText = DestinationPath;
            }
            else
            {
                XmlNode myNode = root.SelectSingleNode("SourcePath");
                myNode.InnerText = SourcePath;
            }
            doc.Save(GlobalData.PathFile);


            //string[] readLines = File.ReadAllLines(GlobalData.ConfigurationFileName);

            //var listOfStrings = new List<string>();
            //if (File.Exists(GlobalData.ConfigurationFileName))
            //{
            //    foreach (string line in readLines)
            //    {
            //        if (line.Contains("DestinationPath") & isDestination)
            //        {
            //            listOfStrings.Add("DestinationPath=\"" + DestinationPath + "\"");
            //        }
            //        else if (line.Contains("SourcePath") & !isDestination)
            //        {
            //            listOfStrings.Add("SourcePath=\"" + SourcePath + "\"");
            //        }

            //        else
            //            listOfStrings.Add(line);
            //    }
            //    string[] linesToWrite = listOfStrings.ToArray();
            //    File.WriteAllLines(GlobalData.ConfigurationFileName, linesToWrite);
            //}
        }

        private void OnOpenLogExecute()
        {
            string localPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RobotFilesHarvester\\log.txt");
            if (File.Exists(localPath))
            {
                System.Diagnostics.Process.Start(localPath);
            }
        }

        private void OpenDestExecute()
        {
            if (Directory.Exists(GlobalData.DestinationPath))
                System.Diagnostics.Process.Start(GlobalData.DestinationPath.Trim());
            else
                MessageBox.Show("Destination folder does not exist", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}