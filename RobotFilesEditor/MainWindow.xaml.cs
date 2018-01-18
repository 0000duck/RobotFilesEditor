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
       
        public bool SelectedKRC2
        {
            get { return _selectedControlerType==GlobalData.ControlerTypes.KRC2; }
            set
            {
                if (_selectedControlerType == GlobalData.ControlerTypes.KRC2 != value)
                {
                    if(value==true)
                    {
                        SelectedControlerType = GlobalData.ControlerTypes.KRC2;
                        OnPropertyChanged(nameof(SelectedControlerType));                                     
                    }                    
                }
            }
        }

        public bool SelectedKRC4
        {
            get { return _selectedControlerType == GlobalData.ControlerTypes.KRC4; }
            set
            {
                if (_selectedControlerType == GlobalData.ControlerTypes.KRC4 != value)
                {
                    if (value)
                    {
                        SelectedControlerType = GlobalData.ControlerTypes.KRC4;
                        OnPropertyChanged(nameof(SelectedControlerType));
                       
                    }                                
                }
            }
        }

        public GlobalData.ControlerTypes SelectedControlerType
        {
            get { return _selectedControlerType;}
            set
            {
                if(_selectedControlerType!=value)
                {
                    _selectedControlerType = value;
                    OnPropertyChanged(nameof(SelectedKRC2));
                    OnPropertyChanged(nameof(SelectedKRC4));
                    RefreshControlerProperty();
                }
            }
        }

        public string SelectedSourceFoldersPath
        {
            get { return $"Source path: {_selectedSourceFoldersPath}"; }
            set {
                if (_selectedSourceFoldersPath != value)
                {
                    _selectedSourceFoldersPath = value;
                    OnPropertyChanged(nameof(SelectedSourceFoldersPath));
                    RefreshSourcePath();
                }
            }
        }

        public string SelectedDestFoldersPath
        {
            get { return $"Dest. path: {_selectedDestFoldersPath}"; }
            set
            {
                if (_selectedDestFoldersPath != value)
                {
                    _selectedDestFoldersPath = value;
                    OnPropertyChanged(nameof(SelectedDestFoldersPath));
                    RefreshDestinationPath();
                }
            }
        }

        public List<string>FilesList
        {
            get { return _filesList; }
            set
            {
                if (_filesList != value)
                {
                    _filesList = value;
                    OnPropertyChanged(nameof(FilesList));
                    RefreshDestinationPath();                
                }
            }
        }
      
        public event PropertyChangedEventHandler PropertyChanged;     

        private string _selectedSourceFoldersPath;
        private string _selectedDestFoldersPath;
        private GlobalData.ControlerTypes _selectedControlerType;        
        public List<string> _filesList;
        private IControler _controler;

        public MainWindow()
        {            
            InitializeComponent();
            SelectedSourceFoldersPath = @"C:\Users\ajergas\Downloads\KUKA Organizer\Przykładowe pliki do obrobienia\KRC2";//"No selected path";
            SelectedDestFoldersPath = @"C:\Users\" + Environment.UserName + @"\Documents";
            SelectedControlerType = GlobalData.ControlerTypes.KRC2;
            RefreshControlerProperty();                       
        }

        [NotifyPropertyChangedInvocatorAttribute] 
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void MoveProduction_Click(object sender, RoutedEventArgs e)
        {
            try {             
                  _controler.MoveProductionFiles();

            } catch(NullReferenceException ex)
            {
                MessageBoxResult result = MessageBox.Show("Problem with read a configration file. Error "+ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MoveServices_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _controler.MoveServicesFiles(_selectedSourceFoldersPath);
            }
            catch (NullReferenceException ex)
            {
                MessageBoxResult result = MessageBox.Show("Problem with read a configration file. Error " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void FolderSourcePath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();          

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                SelectedSourceFoldersPath = dialog.SelectedPath;
            }
        }

        private void FolderDestinationPath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();           

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                SelectedDestFoldersPath = dialog.SelectedPath;
            }         
                _controler?.CreateDestinationFolders();
        }

        public void RefreshControlerProperty()
        {
            if (_controler == null)
            {
                switch (_selectedControlerType)
                {
                    case GlobalData.ControlerTypes.KRC2:
                        {
                            _controler = new KukaKrc2(_selectedSourceFoldersPath, _selectedDestFoldersPath);                         
                        }
                        break;
                    case GlobalData.ControlerTypes.KRC4:
                        {
                             _controler= new KukaKrc4(_selectedSourceFoldersPath, _selectedDestFoldersPath);                                
                        }
                        break;
                }
            }

            _controler.LoadConfigurationSettingsForControler();
            FilesList = _controler.GetGroupedFiles();
        }

        public void RefreshSourcePath()
        {
            if (_controler != null)
            {
                _controler.RefreshSourcePath(_selectedSourceFoldersPath);
                FilesList = _controler.GetGroupedFiles();
            }else
            {
                RefreshControlerProperty();
            }               
        }

        public void RefreshDestinationPath()
        {
            if (_controler != null)
            {
                _controler.RefreshDestinationPath(_selectedDestFoldersPath);
                FilesList = _controler.GetGroupedFiles();
            }
            else
            {
                RefreshControlerProperty();
            }
        }
    }
}
