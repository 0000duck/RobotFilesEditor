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

        public string SelectedFoldersPath
        {
            get { return _selectedFoldersPath; }
            set {

                if (_selectedFoldersPath != value)
                {
                    _selectedFoldersPath = value;
                    OnPropertyChanged(nameof(SelectedFoldersPath));
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private KukaKrc2 _krc2;
        private KukaKrc4 _krc4;
        private string _selectedFoldersPath;
        private GlobalData.ControlerTypes _selectedControlerType;

        private List<FileTreeNode> _filesExtension;

        public MainWindow()
        {
            InitializeComponent();
            SelectedControlerType = GlobalData.ControlerTypes.KRC2;
            SelectedFoldersPath = "No selected path";
            _filesExtension = new List<FileTreeNode>();
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
                            _krc2.MoveProductionFiles(_selectedFoldersPath);
                        } break;
                    case GlobalData.ControlerTypes.KRC4: {

                        } break;
                }

            } catch(NullReferenceException ex)
            {

            }
        }

        private void MoveServices_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FolderPath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if(result==System.Windows.Forms.DialogResult.OK)
            {
                SelectedFoldersPath = dialog.SelectedPath;
            }

            CreateFoldersTreeView();
        }



        public void CreateFoldersTreeView()
        {

            string[] files = Directory.GetFiles(_selectedFoldersPath);
            files=files.OrderBy(x=>x).ToArray();


            TreeViewItem root = new TreeViewItem();
            root.Header = "Files";
           

            //MenuItem childItem1 = new MenuItem() { Title = "Child item #1" };
            //childItem1.Items.Add(new MenuItem() { Title = "Child item #1.1" });
            //childItem1.Items.Add(new MenuItem() { Title = "Child item #1.2" });
            //root.Items.Add(childItem1);
            //root.Items.Add(new MenuItem() { Title = "Child item #2" });
            //trvMenu.Items.Add(root);

            foreach (var f in files)
            {
                List<FileTreeNode> nodes=_filesExtension.Where(s => f.Contains(s.Extension)).ToList();

                foreach(var n in nodes )
                {
                    
                }

                
            }

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
    }
}
