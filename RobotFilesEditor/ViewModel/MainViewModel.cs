using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System.Windows;

namespace RobotFilesEditor.ViewModel
{
    public class MainViewModel : ViewModelBase, IDisposable
    {
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
                    if(SelectedControler!=null)
                    {
                        SelectedControler.SourcePath = SourcePath;
                        foreach(var operation in AllOperations)
                        {
                            operation.Operations.ForEach(x => x.SourcePath = SourcePath);
                        }
                        ShowAllOperationsResults();
                    }                    
                }
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
                    if (SelectedControler != null)
                    {
                        SelectedControler.DestinationPath = DestinationPath;
                        foreach (var operation in AllOperations)
                        {
                            operation.Operations.ForEach(x => x.DestinationPath = DestinationPath);
                        }
                        ShowAllOperationsResults();
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
                       
        private List<Controler> _controlers;
        private Controler _selectedControler;
        private string _sourcePath;
        private string _destinationPath;
        private bool _continueWithoutConfirm;

        public MainViewModel(List<Controler> controlers)
        {
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
        public ICommand ExecuteAllOperationsCommand { get; set; }
        public ICommand ClosingCommand { get; set; }
        private void SetCommands()
        {
            SetSourcePathCommand = new RelayCommand(SetSourcePathCommandExecute);
            SetDestinationPathCommand = new RelayCommand(SetDestinationPathCommandExecute);
            ExecuteAllOperationsCommand = new RelayCommand(ExecuteAllOperationsCommandExecute);
            ClosingCommand = new RelayCommand(ClosingCommandExecute);
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
        }

        private void ShowAllOperationsResults()
        {
            foreach (var operation in AllOperations)
            {
                operation.PreviewOperationCommandExecute();
            }
        }
    }
}