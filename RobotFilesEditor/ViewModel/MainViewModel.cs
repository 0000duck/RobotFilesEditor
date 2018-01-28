using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.ObjectModel;

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
        private string _selectedOperation;

        public MainViewModel(List<Controler> controlers)
        {
            ControlerChooser = new ObservableCollection<ControlItem>();
            MoveFilesOperations = new ObservableCollection<ControlItem>();
            CopyFilesOperations = new ObservableCollection<ControlItem>();
            CopyTextFromFilesOperations = new ObservableCollection<ControlItem>();
            RemoveFilesOperations = new ObservableCollection<ControlItem>();

            Controlers = controlers;
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

            foreach (var filter in SelectedControler.FilesFilters)
            {
                var controlItem = new ControlItem(filter.OperationName);
                controlItem.ControlItemSelected += Operation_Click;

                switch (filter.Action)
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

        public void Dispose()
        {
            ControlerChooser?.ToList().ForEach(item => item.ControlItemSelected -= ControlerChooser_Click);
            MoveFilesOperations?.ToList().ForEach(item => item.ControlItemSelected -= Operation_Click);
            CopyFilesOperations?.ToList().ForEach(item => item.ControlItemSelected -= Operation_Click);
            CopyTextFromFilesOperations?.ToList().ForEach(item => item.ControlItemSelected -= Operation_Click);
            RemoveFilesOperations?.ToList().ForEach(item => item.ControlItemSelected -= Operation_Click);
        }

        #endregion ControlersCreator

        private void ControlerChooser_Click(object sender, ControlItem e)
        {
            SelectedControler = Controlers.FirstOrDefault(x => x.ContolerType == e.Content);
            CreateOperationsControls();
        }

        private void Operation_Click(object sender, ControlItem e)
        {

        }
    }
}