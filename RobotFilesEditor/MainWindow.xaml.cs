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
                
        private KukaKrc2 _krc2;
        private KukaKrc4 _krc4;

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
                switch(_selectedControlerType)
                {
                    case GlobalData.ControlerTypes.KRC2: {
                            _krc2.MoveProductionFiles();
                        } break;
                    case GlobalData.ControlerTypes.KRC4: {
                            _krc4.MoveProductionFiles();
                        } break;
                }

            } catch(NullReferenceException ex)
            {
                MessageBoxResult result = MessageBox.Show("Problem with read a configration file. Error "+ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MoveServices_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (_selectedControlerType)
                {
                    case GlobalData.ControlerTypes.KRC2:
                        {
                            _krc2.MoveServicesFiles(_selectedSourceFoldersPath);
                        }
                        break;
                    case GlobalData.ControlerTypes.KRC4:
                        {

                        }
                        break;
                }

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

            switch (_selectedControlerType)
            {
                case GlobalData.ControlerTypes.KRC2:
                    {
                        _krc2?.CreateDestinationFolders(_selectedDestFoldersPath);
                    }
                    break;
                case GlobalData.ControlerTypes.KRC4:
                    {
                        _krc4?.CreateDestinationFolders(_selectedDestFoldersPath);
                    }
                    break;
            }
        }

        public void RefreshControlerProperty()
        {
            switch (_selectedControlerType)
            {
                case GlobalData.ControlerTypes.KRC2:
                    {
                        if (_krc2 == null)
                        {
                            _krc2 = new KukaKrc2(_selectedSourceFoldersPath, _selectedDestFoldersPath);
                            _krc2.LoadConfigurationSettingsForControler();
                            FilesList = _krc2.GetGroupedFiles();
                        }else
                        {
                            _krc2.RefreshSourcePath(_selectedSourceFoldersPath);
                            _krc4.RefreshDestinationPath(_selectedDestFoldersPath);
                            FilesList = _krc2.GetGroupedFiles();
                        }
                    }
                    break;
                case GlobalData.ControlerTypes.KRC4:
                    {
                        if (_krc4 == null)
                        {
                            _krc4 = new KukaKrc4(_selectedSourceFoldersPath, _selectedDestFoldersPath);
                            _krc4.LoadConfigurationSettingsForControler();
                            FilesList = _krc2.GetGroupedFiles();
                        }else
                        {
                            _krc2.RefreshSourcePath(_selectedSourceFoldersPath);
                            _krc4.RefreshDestinationPath(_selectedDestFoldersPath);
                            FilesList = _krc2.GetGroupedFiles();
                        }
                    }
                    break;
            }
        }

        public void RefreshSourcePath()
        {
            switch (_selectedControlerType)
            {
                case GlobalData.ControlerTypes.KRC2:
                    {
                        if(_krc2!=null)
                        {
                            _krc2.RefreshSourcePath(_selectedSourceFoldersPath);
                            FilesList = _krc2.GetGroupedFiles();
                        }
                    }
                    break;
                case GlobalData.ControlerTypes.KRC4:
                    {
                        if (_krc4 != null)
                        {
                            _krc4.RefreshSourcePath(_selectedSourceFoldersPath);
                            FilesList = _krc4.GetGroupedFiles();
                        }
                    }
                    break;
            }
        }

        public void RefreshDestinationPath()
        {
            switch (_selectedControlerType)
            {
                case GlobalData.ControlerTypes.KRC2:
                    {
                        if (_krc2 != null)
                        {
                            _krc2.RefreshDestinationPath(_selectedDestFoldersPath);
                        }
                    }
                    break;
                case GlobalData.ControlerTypes.KRC4:
                    {
                        if (_krc4 != null)
                        {
                            _krc4.RefreshDestinationPath(_selectedDestFoldersPath);
                        }
                    }
                    break;
            }
        }
    }
}
