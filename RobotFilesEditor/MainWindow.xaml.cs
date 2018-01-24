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
        public Menu MainMenu
        {
            get {
                //MainWindowMenu = _menuCreator.MainMenu;
                return _menuCreator?.MainMenu; }
            set
            {
                _menuCreator.MainMenu = value;
                OnPropertyChanged(nameof(MainMenu));
            }
        }

        private MenuCreator _menuCreator;
       
        public MainWindow(List<Controler>controlers)
        {
            InitializeComponent();

            _menuCreator = new MenuCreator(controlers);
            MainMenu = _menuCreator.MainMenu;
            //MainWindowMenu.Items.Add(_menuCreator.MainMenu);

        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocatorAttribute]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }       
    }
}
