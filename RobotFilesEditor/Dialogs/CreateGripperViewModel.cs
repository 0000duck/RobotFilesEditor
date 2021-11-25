using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RobotFilesEditor.Model.DataOrganization;
using RobotFilesEditor.Model.Operations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RobotFilesEditor.Dialogs
{
    public class CreateGripperViewModel : ViewModelBase
    {
        
        #region ctor
        public CreateGripperViewModel()
        {
            if (GlobalData.ControllerType == "KRC2 V8" || GlobalData.ControllerType == "KRC4")
                Controller = 1;
            else if (GlobalData.ControllerType == "KRC2 L6")
                Controller = 0;
            if (GlobalData.ControllerType == "KRC2 V8")
                Groups = FillSignals(1,8);
            else if (GlobalData.ControllerType == "KRC4")
                Groups = FillSignals(1, 16);
            else
                Groups = FillSignals(1, 48);
            NrOfInputs = 0;
            NrOfOutputs = 0;
            Clamps = FillSignals(1,4);
            Types = GetClampTypes();
            GroupsSensors = FillSignals(1, 48);
            HasSoftStart = false;
            SelectedGripperNumber = 1;
            GripperNumbers = FillGripperNumbers(1, 4);
            //SoftStartString = "";
            GetAddresses();
            GripperElements = new ObservableCollection<GripperElementsVM>();
            GripperElementSensors = new ObservableCollection<GripperElementSensorsVM>();
            GripperElementsVM.SendProperty += new GripperElementsVM.SendPropertyHandler(GripperElementsPropertyChanged);
        }
        #endregion

        #region Properties

        //private bool _controller;
        //public bool Controller
        //{
        //    get { return _controller; }
        //    set { _controller = value; }
        //}


        private ObservableCollection<IntItem> _groups;

        public ObservableCollection<IntItem> Groups
        {
            get { return _groups; }
            set {
                _groups = value;
                RaisePropertyChanged(() => Groups);
                }
        }

        private ObservableCollection<IntItem> _groupsSensors;

        public ObservableCollection<IntItem> GroupsSensors
        {
            get { return _groupsSensors; }
            set
            {
                _groupsSensors = value;
                RaisePropertyChanged(() => GroupsSensors);
            }
        }

        private ObservableCollection<IntItem> _sensors;
        public ObservableCollection<IntItem> Sensors
        {
            get { return _sensors; }
            set
            {
                _sensors = value;
                RaisePropertyChanged(() => Sensors);
            }
        }

        private bool _success;

        public bool Success
        {
            get { return _success; }
            set {
                _success = value;
                RaisePropertyChanged(() => Success);
                }
        }


        private ObservableCollection<IntItem> _startAddresses;
        public ObservableCollection<IntItem> StartAddresses
        {
            get { return _startAddresses; }
            set
            {
                _startAddresses = value;
                RaisePropertyChanged(() => StartAddresses);
            }
        }

        private ObservableCollection<int> _gripperNumbers;
        public ObservableCollection<int> GripperNumbers
        {
            get { return _gripperNumbers; }
            set
            {
                _gripperNumbers = value;
                RaisePropertyChanged(() => GripperNumbers);
            }
        }

        private ObservableCollection<IntItem> _outsForClose;
        public ObservableCollection<IntItem> OutsForClose
        {
            get { return _outsForClose; }
            set
            {
                _outsForClose = value;
                RaisePropertyChanged(() => OutsForClose);
            }
        }

        private ObservableCollection<IntItem> _clamps;

        public ObservableCollection<IntItem> Clamps
        {
            get { return _clamps; }
            set
            {
                _clamps = value;
                RaisePropertyChanged(() => Clamps);
            }
        }

        private ObservableCollection<GripperElementsVM> _gripperElements;
        public ObservableCollection<GripperElementsVM> GripperElements
        {
            get { return _gripperElements; }
            set
            {
                if (_gripperElements != value)
                {
                    _gripperElements = value;
                    RaisePropertyChanged(() => GripperElements);
                }
            }
        }

        private ObservableCollection<GripperElementSensorsVM> _gripperElementSensors;
        public ObservableCollection<GripperElementSensorsVM> GripperElementSensors
        {
            get { return _gripperElementSensors; }
            set
            {
                if (_gripperElementSensors != value)
                {
                    _gripperElementSensors = value;
                    RaisePropertyChanged(() => GripperElementSensors);
                }
            }
        }

        private ObservableCollection<TextItem> _types;

        public ObservableCollection<TextItem> Types
        {
            get { return _types; }
            set
            {
                if (_types != value)
                {
                    _types = value;
                    RaisePropertyChanged(() => Types);
                }
            }
        }

        private int _nrOfInputs;
        public int NrOfInputs
        {
            get { return _nrOfInputs; }
            set
            {
                if (_nrOfInputs != value)
                {
                    _nrOfInputs = value;
                    RaisePropertyChanged(() => NrOfInputs);
                }
            }
        }

        private int _nrOfOutputs;
        public int NrOfOutputs
        {
            get { return _nrOfOutputs; }
            set
            {
                if (_nrOfOutputs != value)
                {
                    _nrOfOutputs = value;
                    RaisePropertyChanged(() => NrOfOutputs);
                }
            }
        }

        private int _selectedGripperNumber;
        public int SelectedGripperNumber
        {
            get { return _selectedGripperNumber; }
            set
            {
                if (_selectedGripperNumber != value)
                {
                    _selectedGripperNumber = value;
                    GetAddresses();
                    RaisePropertyChanged(() => SelectedGripperNumber);
                    SoftStartString = "";
                }
            }
        }

        private string _softStartString;
        public string SoftStartString
        {
            get { return _softStartString; }
            set
            {
                if (SoftStartAddress > 0)
                    _softStartString = "I\\O for SoftStart will be automatically added at address " + SoftStartAddress;
                else
                    _softStartString = "";
                RaisePropertyChanged(() => SoftStartString);
                //GetAddresses();
            }
        }

        private int _controller;
        public int Controller
        {
            get { return _controller; }
            set
            {
                if (_controller != value)
                {
                    _controller = value;
                    RaisePropertyChanged(() => Controller);
                }
            }
        }

        private int _softStartAddress;
        public int SoftStartAddress
        {
            get { return _softStartAddress; }
            set
            {
                if (_softStartAddress != value)
                {
                    _softStartAddress = value;
                    RaisePropertyChanged(() => SoftStartAddress);
                    RaisePropertyChanged(() => SoftStartString);
                }
            }
        }

        bool _hasSoftStart;
        public bool HasSoftStart
        {
            get { return _hasSoftStart; }
            set
            {
                if (value != _hasSoftStart)
                {
                    _hasSoftStart = value;
                    RaisePropertyChanged(() => HasSoftStart);
                    GetAddresses();
                    SoftStartString = "";
                    
                }
            }
        }
        #endregion

        #region Methods

        private ObservableCollection<TextItem> GetClampTypes()
        {
            ObservableCollection<TextItem> result = new ObservableCollection<TextItem>();
            result.Add(new TextItem("Clamp"));
            result.Add(new TextItem("Vacuum"));
            return result;
        }

        private ObservableCollection<IntItem> FillSignals(int start, int end)
        {
            ObservableCollection<IntItem> result = new ObservableCollection<IntItem>();
            for (int i = start; i <= end; i++)
            {
                IntItem currentItem = new IntItem();
                currentItem.Value = i;
                result.Add(currentItem);
            }
            return result;
        }

        private void closeOK()
        {
            Success = true;
            var window = Application.Current.Windows
            .Cast<Window>()
            .Single(w => w.DataContext == this);
            window.DialogResult = true;
            window.Close();
        }

        private void closeCancel()
        {
            Success = false;
            var window = Application.Current.Windows
            .Cast<Window>()
            .Single(w => w.DataContext == this);
            window.DialogResult = false;
            window.Close();
        }

        public static void UpdateElements(GripperElement gripperElement)
        {
            
        }

        private void GripperElementsPropertyChanged(object sender, EventArgs arg)
        {
            GripperElementsVM orgsElement = (GripperElementsVM)sender;
        }

        private ObservableCollection<int> FillGripperNumbers(int start, int end)
        {
            ObservableCollection<int> result = new ObservableCollection<int>();
            for (int i = start; i <= end; i++)
                result.Add(i);
            return result;
        }

        private void GetAddresses()
        {
            int startAddressOut = 0, endAddress = 0, startAddressIn = 0, addressForSoftStart = 0;

            if (GlobalData.ControllerType == "KRC4")
            {
                startAddressOut = startAddressIn = 2049;                
                startAddressOut += (SelectedGripperNumber - 1) * 64;
                startAddressIn += (SelectedGripperNumber - 1) * 64;
                endAddress = startAddressOut + 95;
                if (HasSoftStart)
                {
                    addressForSoftStart = startAddressIn;
                    startAddressOut += 8;
                    startAddressIn += 2;                    
                }
            }
            else if (GlobalData.ControllerType == "KRC2 V8")
            {
                startAddressOut = startAddressIn = 1041;
                startAddressIn += (SelectedGripperNumber - 1) * 64;
                startAddressOut += (SelectedGripperNumber - 1) * 64;
                endAddress = startAddressOut + 63;
                if (HasSoftStart)
                {
                    addressForSoftStart = startAddressIn;
                    startAddressOut += 8;
                    startAddressIn += 2;
                }
            }
            else if (GlobalData.ControllerType == "KRC2 L6")
            {
                startAddressOut = 1041;
                startAddressIn += (SelectedGripperNumber - 1) * 64;
                startAddressOut += (SelectedGripperNumber - 1) * 64;
                endAddress = startAddressOut + 63;
            }
            StartAddresses = FillSignals(startAddressIn, endAddress);
            OutsForClose = FillSignals(startAddressOut, endAddress);
            Sensors = FillSignals(startAddressIn, endAddress);
            SoftStartAddress = addressForSoftStart;

        }
        #endregion



        #region Commands
        RelayCommand _closeOK;
        public RelayCommand CloseOK
        {
            get
            {
                if (_closeOK == null)
                {
                    _closeOK = new RelayCommand(closeOK);
                }
                return _closeOK;
            }
        }

        RelayCommand _closeCancel;
        public RelayCommand CloseCancel
        {
            get
            {
                if (_closeCancel == null)
                {
                    _closeCancel = new RelayCommand(closeCancel);
                }
                return _closeCancel;
            }
        }
        #endregion
    }
}
