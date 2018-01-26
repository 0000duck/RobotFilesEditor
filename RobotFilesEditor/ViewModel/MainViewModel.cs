using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace RobotFilesEditor.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase, INotifyPropertyChanged
    {
        #region Controls
        #region MenuControls
        public MenuItem ControlersChooserMenu
        {
            get
            {
                return _menuCreator?.ControlersChooserMenu;
            }
            set
            {
                if (_menuCreator?.ControlersChooserMenu != value)
                {
                    _menuCreator.ControlersChooserMenu = value;
                    OnPropertyChanged(nameof(_menuCreator.ControlersChooserMenu));
                }
            }
        }
        public MenuItem OperationsMenu
        {
            get { return _menuCreator?.OperationsMenu; }
            set
            {
                if (_menuCreator?.OperationsMenu != value)
                {
                    _menuCreator.OperationsMenu = value;
                    OnPropertyChanged(nameof(_menuCreator.OperationsMenu));
                }
            }
        }

        public string SelectedControler
        {
            get { return _selectedControler; }
            set
            {
                if (_selectedControler != value)
                {
                    _selectedControler = value;
                    OnPropertyChanged(nameof(SelectedControler));
                }
            }
        }
        public string SelectedOperation
        {
            get { return _selectedOperation; }
            set
            {
                if (_selectedOperation != value)
                {
                    _selectedOperation = value;
                    OnPropertyChanged(nameof(SelectedOperation));
                }
            }
        }
        #endregion MenuControls
        #endregion Controls

        #region Commands      
        public ICommand ChooseControlerCommand
        {
            get;
            internal set;
        }
        public ICommand SelectOperationCommand
        {
            get;
            internal set;
        }
        private void CreateCommands()
        {
            ChooseControlerCommand = new RelayCommand(SelectControler);
            SelectOperationCommand = new RelayCommand(SelectOperation);
        }       
        #endregion Commands

        public List<Controler> Controlers
        {
            get { return _controlers; }
            set
            {
                if (_controlers != value)
                {
                    _controlers = value;
                    OnPropertyChanged(nameof(Controlers));
                }
            }
        }

        private MenuCreator _menuCreator;
        private List<Controler> _controlers;
        private string _selectedControler;
        private string _selectedOperation;            

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocatorAttribute]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region ControlersChooserMenuMethodRegion
        private void SelectControler()
        {

        }
        #endregion ControlersChooserMenuMethodRegion

        #region OperationsMenuMethodRegion
        private void SelectOperation()
        {

        }

        #endregion OperationsMenuMenuMethodRegion


        public MainViewModel(List<Controler> controlers)
        {         
            _menuCreator = new MenuCreator(ref controlers);
            ControlersChooserMenu = _menuCreator.ControlersChooserMenu;
            OperationsMenu = _menuCreator.OperationsMenu;
            //InitializeComponent();      
        }

    }
}