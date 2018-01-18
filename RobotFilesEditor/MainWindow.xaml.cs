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
                }
            }
        }

        public List<string>FilesList
        {
            get;
            set;
        }

        public TreeViewItem FileBrowser
        {
            get { return _fileBrowser; }
            set
            {
                if (_fileBrowser != value)
                {
                    _fileBrowser = value;
                    OnPropertyChanged(nameof(FileBrowser));
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private KukaKrc2 _krc2;
        private KukaKrc4 _krc4;
        private string _selectedSourceFoldersPath;
        private string _selectedDestFoldersPath;
        private GlobalData.ControlerTypes _selectedControlerType;
        private TreeViewItem _fileBrowser;
        private List<FilesTree> _filesExtension;
        public List<string> _filesList;


        public MainWindow()
        {            
             InitializeComponent();

            SelectedControlerType = GlobalData.ControlerTypes.KRC2;
            SelectedSourceFoldersPath = @"C:\Users\ajergas\Downloads\KUKA Organizer\Przykładowe pliki do obrobienia\KRC2";//"No selected path";
            SelectedDestFoldersPath = @"C:\Users\" + Environment.UserName + @"\Documents";
            _filesExtension = new List<FilesTree>();
            _krc2 = new KukaKrc2();
            _krc4 = new KukaKrc4();

            _filesExtension = _krc2.GetFilesExtensions();
            FileBrowser = CreateFoldersTreeView();
            
           // treeView.Items.Add(FileBrowser);        
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
                            _krc2.MoveProductionFiles(_selectedSourceFoldersPath);
                        } break;
                    case GlobalData.ControlerTypes.KRC4: {

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
        public TreeViewItem CreateFoldersTreeView()
        {
            string[] files = Directory.GetFiles(_selectedSourceFoldersPath);
            files=files.OrderBy(x=>x).ToArray();

            TreeViewItem root = new TreeViewItem();
            root.Header = "Files";      

           
            TreeViewItem lastItem=new TreeViewItem();

            foreach (var n in _filesExtension)
            {
                lastItem = new TreeViewItem();
                lastItem.Header = n.NodeName+n.Extension;                

                foreach(var f in files)
                {
                    if(f.Contains(n.Extension))
                    {
                        lastItem.Items.Add(new TreeViewItem().Header = f);
                    }
                }

                root.Items.Add(lastItem);             
            }         

            return root;
        }

        private void GetFilesExtensions()
        {
            switch(_selectedControlerType)
            {
                case GlobalData.ControlerTypes.KRC2:
                    {
                        _filesExtension = _krc2?.GetFilesExtensions();
                    } break;
                case GlobalData.ControlerTypes.KRC4:
                    {
                        _filesExtension = _krc4?.GetFilesExtensions();
                    } break;
            }
        }

        private void FolderSourcePath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            GetFilesExtensions();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                SelectedSourceFoldersPath = dialog.SelectedPath;
            }

            FileBrowser = CreateFoldersTreeView();
        }

        private void FolderDestinationPath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            GetFilesExtensions();

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

        public void InitializeControler()
        {
            switch(_selectedControlerType)
            {
                case GlobalData.ControlerTypes.KRC2:
                    {
                        if (_krc2 == null)
                        {
                            _krc2 = new KukaKrc2();
                            _krc2.LoadConfigurationSettingsForControler();
                            FilesList=_krc2.GetGroupedFiles();
                        }
                    }break;
                case GlobalData.ControlerTypes.KRC4:
                    {

                    }
                    break;
            }
        }
    }
}
