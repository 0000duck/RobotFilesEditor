using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace RobotFilesEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MenuItem ControlersChooserMenu
        {
            get {                
                return _menuCreator?.ControlersChooserMenu; }
            set
            {
                if(_menuCreator?.ControlersChooserMenu!=value)
                {
                    _menuCreator.ControlersChooserMenu = value;
                    OnPropertyChanged(nameof(ControlersChooserMenu));
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
                    OnPropertyChanged(nameof(ControlersChooserMenu));
                }
            }
        }

        public List<Controler>Controlers
        {
            get { return _controlers; }
            set
            {
                if(_controlers!=value)
                {
                    _controlers = value;
                    OnPropertyChanged(nameof(Controlers));
                }
            }
        }

        private MenuCreator _menuCreator;
        private List<Controler> _controlers;
       
        public MainWindow(List<Controler>controlers)
        {          
            _menuCreator = new MenuCreator(ref controlers);
            ControlersChooserMenu = _menuCreator.ControlersChooserMenu;
            OperationsMenu = _menuCreator.OperationsMenu;
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocatorAttribute]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }        
    }
}
