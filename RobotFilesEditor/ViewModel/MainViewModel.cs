using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace RobotFilesEditor
{
    class MainViewModel: INotifyPropertyChanged
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
        #endregion MenuControls
        #endregion Controls

        #region Commands

        #region ControlersChooserMenuCommandRegion       
        public ICommand ChooseControlerCommand
        {
            get;
            internal set;
        }

        private void CreateSaveCommand()
        {
            ChooseControlerCommand = new 
        }

        public void SaveExecute()
        {
            SelectControler();
        }

        #endregion ControlersChooserMenuCommandRegion
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

        public MainViewModel(List<Controler> controlers)
        {
            _menuCreator = new MenuCreator(ref controlers);
            ControlersChooserMenu = _menuCreator.ControlersChooserMenu;
            OperationsMenu = _menuCreator.OperationsMenu;
            //InitializeComponent();
        }

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
    }
}
