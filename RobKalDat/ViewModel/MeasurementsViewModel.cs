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
using System.Windows.Input;

namespace RobKalDat.ViewModel
{
    public class MeasurementsViewModel : ViewModelBase
    {


        #region props
        private int selectedMeasurement;

        public int SelectedMeasurement
        {
            get { return selectedMeasurement; }
            set {
                Set(ref selectedMeasurement, value);
                //RaisePropertyChanged(() => SelectedMeasurement);
            }
        }

        private Coords selectedCSV;
        public Coords SelectedCSV
        {
            get { return selectedCSV; }
            set
            {
                Set(ref selectedCSV, value);
                //RaisePropertyChanged(() => SelectedCSV);
            }
        }

        private ObservableCollection<Measurement> measurements;
        public ObservableCollection<Measurement> Measurements
        {
            get { return measurements; }
            set
            {
                Set(ref measurements, value);
                Messenger.Default.Send(value, "updateRobots");
                //RaisePropertyChanged(() => Measurements);
            }
        }

        private bool refreshList;
        public bool RefreshList
        {
            get { return refreshList; }
            set
            {
                Set(ref refreshList, value);
                //RaisePropertyChanged(() => Measurements);
                if (value)
                    Measurements = new ObservableCollection<Measurement>();
            }
        }

        private Measurement measToUpdate;
        public Measurement MeasToUpdate
        {
            get { return measToUpdate; }
            set
            {
                Set(ref measToUpdate, value);
                OnSelectedMeasChanged();
            }
        }

        private Measurement measToAdd;
        public Measurement MeasToAdd
        {
            get { return measToAdd; }
            set
            {
                Set(ref measToAdd, value);
                OnSelectedMeasAdded();
            }
        }

        private string robotType;
        public string RobotType
        {
            get { return robotType; }
            set
            {
                Set(ref robotType, value);
                Measurements[SelectedMeasurement].RobotType = value;
                //RaisePropertyChanged(() => Measurements);
                ObservableCollection<Measurement> tempMeas = Measurements;
                Measurements = new ObservableCollection<Measurement>();
                Measurements = tempMeas;
            }
        }
        #endregion

        #region ctor
        public MeasurementsViewModel()
        {
            //Test = false;
            SetCommands();
            Messenger.Default.Register<ObservableCollection<Measurement>>(this, "foundMeas", message => Measurements = message);
            Messenger.Default.Register<Coords>(this, "selectedCSV", message => SelectedCSV = message);
            Messenger.Default.Register<bool>(this, "refreshList", message => RefreshList = message);
            Messenger.Default.Register<Measurement>(this, "updateMeasurement", message => MeasToUpdate = message);
            Messenger.Default.Register<Measurement>(this, "addMeasurement", message => MeasToAdd = message);
            //Messenger.Default.Register<ObservableCollection<Measurement>>(this, "modifiedMeasurements", message => Measurements = message);
            //Messenger.Default.Send(Measurements, "measToModify");
        }

        #endregion

        #region commands
        public ICommand LoadReal { get; set; }
        public ICommand EditObject { get; set; }
        public ICommand Delete { get; set; }
        public ICommand ImportExcel { get; set; }
        public ICommand ExportExcel { get; set; }
        public ICommand KukaRobot { get; set; }
        public ICommand ABBRobot { get; set; }
        public ICommand FanucRobot { get; set; }
        public ICommand NoneRobot { get; set; }
        public ICommand NewMeas { get; set; }
        public ICommand ClearReal { get; set; }
        #endregion

        #region methods
        private void SetCommands()
        {
            LoadReal = new RelayCommand(LoadRealExecute);
            EditObject = new RelayCommand(EditObjectExecute);
            Delete = new RelayCommand(DeleteExecute);
            ImportExcel = new RelayCommand(ImportExcelExecute);
            ExportExcel = new RelayCommand(ExportExcelExecute);
            KukaRobot = new RelayCommand(KukaRobotExecute);
            ABBRobot = new RelayCommand(ABBRobotExecute);
            FanucRobot = new RelayCommand(FanucRobotExecute);
            NoneRobot = new RelayCommand(NoneRobotExecute);
            NewMeas = new RelayCommand(NewMeasExecute);
            ClearReal = new RelayCommand(ClearRealExecute);
        }

        private void ClearRealExecute()
        {
            Messenger.Default.Send(false, "enabledChanged");
            if (Measurements != null && Measurements.Count>0 && SelectedMeasurement != -1)
            {
                Measurements[SelectedMeasurement].XIst = 0;
                Measurements[SelectedMeasurement].YIst = 0;
                Measurements[SelectedMeasurement].ZIst = 0;
                Measurements[SelectedMeasurement].RXIst = 0;
                Measurements[SelectedMeasurement].RYIst = 0;
                Measurements[SelectedMeasurement].RZIst = 0;
                Measurements[SelectedMeasurement].HasRealValues = "false";
                ObservableCollection<Measurement> tempMeas = Measurements;
                Measurements = new ObservableCollection<Measurement>();
                Measurements = tempMeas;
            }
            Messenger.Default.Send(true, "enabledChanged");
        }

        private void NoneRobotExecute()
        {
            RobotType = string.Empty;
        }

        private void FanucRobotExecute()
        {
            RobotType = "Fanuc";
        }

        private void ABBRobotExecute()
        {
            RobotType = "ABB";
        }

        private void KukaRobotExecute()
        {
            RobotType = "KUKA";
        }

        private void ImportExcelExecute()
        {
            Messenger.Default.Send(false, "enabledChanged");
            Model.Methods.ExcelMethods.Execute();
            Messenger.Default.Send(true, "enabledChanged");
        }

        private void DeleteExecute()
        {
            Messenger.Default.Send(false, "enabledChanged");
            if (Measurements!=null && SelectedMeasurement>-1 && SelectedMeasurement<Measurements.Count)
                Measurements.Remove(Measurements[SelectedMeasurement]);
            Messenger.Default.Send(true, "enabledChanged");
        }

        private void EditObjectExecute()
        {
            Messenger.Default.Send(false, "enabledChanged");
            if (Measurements!=null && Measurements[SelectedMeasurement] != null && SelectedMeasurement > -1 && SelectedMeasurement < Measurements.Count)
            {              
                Views.EditMeasurement window = new Views.EditMeasurement();
                Messenger.Default.Send(false, "addOrEdit");
                Messenger.Default.Send(Measurements[SelectedMeasurement], "measurementToModify");
                window.ShowDialog();
            }
            Messenger.Default.Send(true, "enabledChanged");
        }

        private void NewMeasExecute()
        {
            Messenger.Default.Send(false, "enabledChanged");
            Measurement newMeas = new Measurement();
            Views.EditMeasurement window = new Views.EditMeasurement();
            Messenger.Default.Send(true, "addOrEdit");
            Messenger.Default.Send(newMeas, "measurementToModify");
            window.ShowDialog();
            Messenger.Default.Send(true, "enabledChanged");
        }

        private void LoadRealExecute()
        {
            Messenger.Default.Send(false, "enabledChanged");
            Model.Methods.Methods.LoadReal(Measurements);
            Messenger.Default.Send(true, "enabledChanged");
        }

        private void OnSelectedMeasChanged()
        {
            Messenger.Default.Send(false, "enabledChanged");
            Measurements[SelectedMeasurement] = MeasToUpdate;
            Messenger.Default.Send(true, "enabledChanged");
        }


        private void OnSelectedMeasAdded()
        {
            Messenger.Default.Send(false, "enabledChanged");
            if (Measurements == null)
                Measurements = new ObservableCollection<Measurement>();
            Measurements.Add(MeasToAdd);
            Messenger.Default.Send(true, "enabledChanged");
        }

        private void ExportExcelExecute()
        {
            Messenger.Default.Send(false, "enabledChanged");
            Model.Methods.ExcelMethods.ExportToExcel(Measurements);
            Messenger.Default.Send(true, "enabledChanged");
        }
        #endregion
    }
}
