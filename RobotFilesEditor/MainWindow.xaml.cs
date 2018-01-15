using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RobotFilesEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        KukaKrc2 _krc2;
        KukaKrc4 _krc4;

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

        private GlobalData.ControlerTypes _selectedControlerType;
        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();
            _selectedControlerType = GlobalData.ControlerTypes.KRC2;          
        }

        [NotifyPropertyChangedInvocator] 
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }    
    }
}
