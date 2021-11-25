using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using RobKalDat.Model.ProjectData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RobKalDat.ViewModel
{
    public class MeasurementWindowViewModel : ViewModelBase
    {
        #region ctor
        public MeasurementWindowViewModel()
        {
            SetCommands();     
            Messenger.Default.Register<ObservableCollection<Measurement>>(this, "measToModify",message => Measurements = message);
            //Measurements = GlobalData.Measurements;
            Messenger.Default.Register<Coords>(this, "selectedCSV", message => SelectedCSV = message);
            Messenger.Default.Register<String>(this, "selectedCSVName", message => SelectedCSVName = message);
            SelectedMeasurement = 0;
        }

        #endregion

        #region props
        private ObservableCollection<Measurement> measurements;
        public ObservableCollection<Measurement> Measurements
        {
            get { return measurements; }
            set {
                Set(ref measurements, value);
                //RaisePropertyChanged(() => Measurements);
            }
        }

        private int selectedMeasurement;

        public int SelectedMeasurement
        {
            get { return selectedMeasurement; }
            set
            {
                Set(ref selectedMeasurement, value);
                //RaisePropertyChanged(() => SelectedMeasurement);
                OnSelectedMeasurementChanged();
            }
        }

        private string selectedCSVName;
        public string SelectedCSVName
        {
            get { return selectedCSVName; }
            set {
                Set(ref selectedCSVName, value);
                //RaisePropertyChanged(() => SelectedCSVName);
            }
        }

        private Coords selectedCSV;
        public Coords SelectedCSV
        {
            get { return selectedCSV; }
            set { Set(ref selectedCSV, value);
                //RaisePropertyChanged(() => SelectedCSV);
                }
        }

        private Coords selectedMeasurementValues;
        public Coords SelectedMeasurementValues
        {
            get { return selectedMeasurementValues; }
            set {
                Set(ref selectedMeasurementValues, value);
                //RaisePropertyChanged(() => SelectedMeasurementValues);
            }
        }

        private Coords difference;
        public Coords Difference
        {
            get { return difference; }
            set {
                Set(ref difference, value);
                //RaisePropertyChanged(() => Difference);
            }
        }
        #endregion

        #region commands
        public ICommand OK { get; set; }
        public ICommand Cancel { get; set; }
        #endregion

        #region methods
        private void SetCommands()
        {
            OK = new RelayCommand(OKExecute);
            Cancel = new RelayCommand(CancelExecute);
        }

        private void CancelExecute()
        {
            var window = Application.Current.Windows
            .Cast<Window>()
            .Single(w => w.DataContext == this);
            window.DialogResult = false;
          //  Cleanup();
            CleanupVM();
            window.Close();
        }

        private void OKExecute()
        {
            UpdateMeasurement();
            var window = Application.Current.Windows
            .Cast<Window>()
            .Single(w => w.DataContext == this);
            window.DialogResult = true;
           // Cleanup();
            CleanupVM();
            window.Close();
        }

        private void UpdateMeasurement()
        {
            Messenger.Default.Send(true, "refreshList");
            Measurements[SelectedMeasurement].XIst = SelectedCSV.X;
            Measurements[SelectedMeasurement].YIst = SelectedCSV.Y;
            Measurements[SelectedMeasurement].ZIst = SelectedCSV.Z;
            Measurements[SelectedMeasurement].RXIst = SelectedCSV.RX;
            Measurements[SelectedMeasurement].RYIst = SelectedCSV.RY;
            Measurements[SelectedMeasurement].RZIst = SelectedCSV.RZ;
            Measurements[SelectedMeasurement].HasRealValues = "true";
            Messenger.Default.Send(Measurements, "foundMeas");

        }

        private void OnSelectedMeasurementChanged()
        {
            if (Measurements == null)
                return;
            var selectedMeas = Measurements[SelectedMeasurement];
            SelectedMeasurementValues = new Coords(selectedMeas.XSoll, selectedMeas.YSoll, selectedMeas.ZSoll, selectedMeas.RXSoll, selectedMeas.RYSoll, selectedMeas.RZSoll);
            Difference = new Coords(Math.Abs(SelectedMeasurementValues.X - SelectedCSV.X), Math.Abs(SelectedMeasurementValues.Y - SelectedCSV.Y), Math.Abs(SelectedMeasurementValues.Z - SelectedCSV.Z), Math.Abs(SelectedMeasurementValues.RX - SelectedCSV.RX), Math.Abs(SelectedMeasurementValues.RY - SelectedCSV.RY), Math.Abs(SelectedMeasurementValues.RZ - SelectedCSV.RZ));
        }


        private void CleanupVM()
        {
            Measurements = null;
            SelectedMeasurement = 0;
            SelectedCSVName = string.Empty;
            SelectedCSV = null;
            Difference = null;
        }

        #endregion
    }
}
