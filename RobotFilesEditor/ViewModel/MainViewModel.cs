using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

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
        public ObservableCollection<ControlItem> CopyTextFromFilesOperations
        {
            get;
            set;
        }
        public ObservableCollection<ControlItem> RemoveFilesOperations
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
                    return "Hidden";
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
                    return "Hidden";
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
                    return "Hidden";
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
                    return "Hidden";
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
                    }
                    
                }              
            }
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

        public MainViewModel(List<Controler> controlers)
        {
            ControlerChooser = new ObservableCollection<ControlItem>();
            MoveFilesOperations = new ObservableCollection<ControlItem>();
            CopyFilesOperations = new ObservableCollection<ControlItem>();
            CopyTextFromFilesOperations = new ObservableCollection<ControlItem>();
            RemoveFilesOperations = new ObservableCollection<ControlItem>();

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
            MoveFilesOperations.Clear();
            CopyFilesOperations.Clear();
            CopyTextFromFilesOperations.Clear();
            RemoveFilesOperations.Clear();
            List<string> operations = new List<string>();
            var opertionGroups = SelectedControler.Operations.FilesOperations.GroupBy(x => x.OperationName);

            foreach (var operationGroup in opertionGroups)
            {
                var operation = operationGroup.FirstOrDefault();
                
                if (operation!= null)
                {
                    var controlItem = new ControlItem(operation.OperationName);
                    controlItem.ControlItemSelected += OperationCommandExecute;

                    switch (operation.ActionType)
                    {
                        case GlobalData.Action.Move:
                            {
                                MoveFilesOperations.Add(controlItem);
                            }
                            break;
                        case GlobalData.Action.Copy:
                            {
                                CopyFilesOperations.Add(controlItem);
                            }
                            break;
                        case GlobalData.Action.CopyData:
                            {
                                CopyTextFromFilesOperations.Add(controlItem);
                            }
                            break;
                        case GlobalData.Action.Remove:
                            {
                                RemoveFilesOperations.Add(controlItem);
                            }
                            break;
                    }
                }                
            }

            MoveFilesOperations.Distinct();
            CopyFilesOperations.Distinct();
            CopyTextFromFilesOperations.Distinct();
            RemoveFilesOperations.Distinct();

            RaisePropertyChanged(nameof(MoveFilesOperationsVisibility));
            RaisePropertyChanged(nameof(CopyFilesOperationsVisibility));
            RaisePropertyChanged(nameof(CopyTextFromFilesOperationsVisibility));
            RaisePropertyChanged(nameof(RemoveFilesOperationsVisibility));
        }

        public void Dispose()
        {
            ControlerChooser?.ToList().ForEach(item => item.ControlItemSelected -= ControlerChooser_Click);
            MoveFilesOperations?.ToList().ForEach(item => item.ControlItemSelected -= OperationCommandExecute);
            CopyFilesOperations?.ToList().ForEach(item => item.ControlItemSelected -= OperationCommandExecute);
            CopyTextFromFilesOperations?.ToList().ForEach(item => item.ControlItemSelected -= OperationCommandExecute);
            RemoveFilesOperations?.ToList().ForEach(item => item.ControlItemSelected -= OperationCommandExecute);
        }

        #endregion ControlersCreator

        #region Command
        public ICommand SetSourcePathCommand { get; set; }
        public ICommand SetDestinationPathCommand { get; set; }

        private void SetCommands()
        {
            SetSourcePathCommand = new RelayCommand(SetSourcePathCommandExecute);
            SetDestinationPathCommand = new RelayCommand(SetDestinationPathCommandExecute);
        }
        #endregion Command

        private void ControlerChooser_Click(object sender, ControlItem e)
        {
            SelectedControler = Controlers.FirstOrDefault(x => x.ContolerType == e.Content);
            CreateOperationsControls();
        }

        private void OperationCommandExecute(object sender, ControlItem e)
        {
            SelectedControler.Operations.FollowOperation(e.Content);
        }

        private void SetSourcePathCommandExecute()
        {
            SourcePath = SetPath(SourcePath);
        }

        private void SetDestinationPathCommandExecute()
        {
            DestinationPath=SetPath(DestinationPath);
        }

        private string SetPath(string path)
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

            return path;
        }
    }
}