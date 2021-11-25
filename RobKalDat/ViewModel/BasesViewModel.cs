using CommonLibrary.RobKalDatCommon;
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
    public class BasesViewModel : ViewModelBase
    {
        #region ctor
        public BasesViewModel()
        {
            Messenger.Default.Register<ObservableCollection<Measurement>>(this, "foundMeas", message => Measurements = message);
            Messenger.Default.Register<ObservableCollection<Measurement>>(this, "updateRobots", message => Measurements = message);
            Messenger.Default.Register<CommonLibrary.RobKalDatCommon.Point>(this, "newBase", message => NewBase = message);
            Messenger.Default.Register<Coords>(this, "adjTCPCalculated", message => AdjTCP = message);
            Messenger.Default.Register<Coords>(this, "extOffsetCalculated", message => ExtOffset = message);
            SetCommands();
            BaseTypes = new ObservableCollection<string>() { "Regular", "External" };
            CalculatedBases = new ObservableCollection<CalculatedBase>();
            SelectedBaseType = 0;
        }

        #endregion

        #region props
        private ObservableCollection<string> baseTypes;
        public ObservableCollection<string> BaseTypes
        {
            get { return baseTypes; }
            set { Set(ref baseTypes, value); }
        }

        private int selectedBaseType;
        public int SelectedBaseType
        {
            get { return selectedBaseType; }
            set
            {
                Set(ref selectedBaseType, value);
                IsTCP = value == 0 ? false : true;
                OkEnabled = OkEnabled;
            }
        }

        private ObservableCollection<Measurement> robots;
        public ObservableCollection<Measurement> Robots
        {
            get { return robots; }
            set { Set(ref robots, value); }
        }

        private int? selectedRobot;
        public int? SelectedRobot
        {
            get { return selectedRobot; }
            set
            {
                Set(ref selectedRobot, value);
                if (value.HasValue)
                {
                    CalculatedBases = Robots[value.Value].Bases;
                    RobotForUC = Robots[SelectedRobot.Value];
                }
                SelectedBase = -1;
                OkEnabled = OkEnabled;
                
            }
        }

        private ObservableCollection<Measurement> measurements;
        public ObservableCollection<Measurement> Measurements
        {
            get { return measurements; }
            set
            {
                Set(ref measurements, value);
                Robots = GetRobots(Measurements);
                SelectedMeas = null;
                SelectedRobot = null;
                SelectedTCP = null;
                MeasurementsWithReal = new ObservableCollection<Measurement>();
                MeasurementsWithoutReal = new ObservableCollection<Measurement>();
                Measurements.Where(x => x.HasRealValues.ToLower() == "true").ToList().ForEach(z=> MeasurementsWithReal.Add(z));
                Measurements.Where(x => x.HasRealValues.ToLower() == "false").ToList().ForEach(z => MeasurementsWithoutReal.Add(z));
                RaisePropertyChanged(() => MeasurementsWithReal);
                RaisePropertyChanged(() => MeasurementsWithoutReal);
                //MeasurementsWithReal = MeasurementsWithReal;
                //MeasurementsWithoutReal = MeasurementsWithoutReal;

            }
        }

        private ObservableCollection<Measurement> measurementsWithReal;
        public ObservableCollection<Measurement> MeasurementsWithReal
        {
            get { return measurementsWithReal; }
            set
            {
                Set(ref measurementsWithReal, value);
            }
        }
        private ObservableCollection<Measurement> measurementsWithoutReal;
        public ObservableCollection<Measurement> MeasurementsWithoutReal
        {
            get { return measurementsWithoutReal; }
            set
            {
                Set(ref measurementsWithoutReal, value);
            }
        }
        private int? selectedMeas;
        public int? SelectedMeas
        {
            get { return selectedMeas; }
            set {
                Set(ref selectedMeas, value);
                OkEnabled = OkEnabled;
                if (value.HasValue)
                    MeasForUC = MeasurementsWithReal[SelectedMeas.Value];
            }
        }

        private int? selectedTCP;
        public int? SelectedTCP
        {
            get { return selectedTCP; }
            set
            {
                Set(ref selectedTCP, value);
                if (value.HasValue)
                    TCPForUC = MeasurementsWithoutReal[value.Value];
                OkEnabled = OkEnabled;
            }
        }

        private string baseNumber;
        public string BaseNumber
        {
            get { return baseNumber; }
            set { Set(ref baseNumber, value); OkEnabled = OkEnabled; }
        }

        private string baseName;
        public string BaseName
        {
            get { return baseName; }
            set { Set(ref baseName, value); OkEnabled = OkEnabled; }
        }

        private ObservableCollection<CalculatedBase> calculatedBases;
        public ObservableCollection<CalculatedBase> CalculatedBases
        {
            get { return calculatedBases; }
            set { Set(ref calculatedBases, value); }
        }

        private int selectedBase;
        public int SelectedBase
        {
            get { return selectedBase; }
            set { Set(ref selectedBase, value); }
        }

        private bool isTCP;
        public bool IsTCP
        {
            get { return isTCP; }
            set {
                Set(ref isTCP, value);
            }
        }

        private CommonLibrary.RobKalDatCommon.Point newBase;
        public CommonLibrary.RobKalDatCommon.Point NewBase
        {
            get { return newBase; }
            set
            {
                Set(ref newBase, value);
                AddBase();
            }
        }

        private bool okEnabled;
        public bool OkEnabled
        {
            get { return okEnabled; }
            set
            {
                if (!string.IsNullOrEmpty(BaseNumber) && !string.IsNullOrEmpty(BaseName) && SelectedRobot.HasValue && SelectedMeas.HasValue && (SelectedBaseType == 0 || SelectedBaseType == 1 && SelectedTCP.HasValue))
                    Set(ref okEnabled, true);
                else
                    Set(ref okEnabled, false);
            }
        }

        private Coords adjTCP;
        public Coords AdjTCP
        {
            get { return adjTCP; }
            set
            {
                Set(ref adjTCP, value);
            }
        }

        private Coords extOffset;
        public Coords ExtOffset
        {
            get { return extOffset; }
            set
            {
                Set(ref extOffset, value);
            }
        }

        private Measurement robotForUC;
        public Measurement RobotForUC
        {
            get { return robotForUC; }
            set { Set(ref robotForUC, value); }
        }

        private Measurement measForUC;
        public Measurement MeasForUC
        {
            get { return measForUC; }
            set { Set(ref measForUC, value); }
        }

        private Measurement tcpForUC;
        public Measurement TCPForUC
        {
            get { return tcpForUC; }
            set { Set(ref tcpForUC, value); }
        }

        #endregion

        #region command
        public ICommand OK { get; set; }
        public ICommand DeleteBase { get; set; }
        public ICommand ExportReal { get; set; }
        #endregion

        #region methods

        private void SetCommands()
        {
            OK = new RelayCommand(OKExecute);
            DeleteBase = new RelayCommand(DeleteExecute);
            ExportReal = new RelayCommand(ExportExecute);
        }

        private void DeleteExecute()
        {
            Messenger.Default.Send(false, "enabledChanged");
            if (SelectedRobot != null && SelectedBase != -1 && Robots[SelectedRobot.Value].Bases.Contains(CalculatedBases[SelectedBase]))
                Robots[SelectedRobot.Value].Bases.Remove(CalculatedBases[SelectedBase]);
            Messenger.Default.Send(true, "enabledChanged");
        }

        private void ExportExecute()
        {
            Messenger.Default.Send(false, "enabledChanged");
            if (SelectedRobot != null)
                Model.Methods.Methods.WriteInputData(Robots[SelectedRobot.Value].Bases);
            Messenger.Default.Send(true, "enabledChanged");
        }

        private void OKExecute()
        {
            Messenger.Default.Send(false, "enabledChanged");
            CommonLibrary.RobKalDatCommon.Point point = Model.Methods.Methods.AddBaseExecute(SelectedBaseType, Robots[SelectedRobot.Value], MeasurementsWithReal[SelectedMeas.Value], SelectedBaseType == 0 ? null : MeasurementsWithoutReal[SelectedTCP.Value]);
            CalculatedBase currentBase = new CalculatedBase(BaseNumber, BaseName, Math.Round(point.XPos,6), Math.Round(point.YPos,6), Math.Round(point.ZPos, 6), Math.Round(point.RX, 6), Math.Round(point.RY, 6), Math.Round(point.RZ, 6), Model.Methods.Methods.IDGenerator(),SelectedBaseType == 0 ? false:true,GetCoords(Robots[SelectedRobot.Value]),GetCoords(MeasurementsWithReal[SelectedMeas.Value]), SelectedBaseType == 0 ? null : GetCoords(MeasurementsWithoutReal[SelectedTCP.Value]), SelectedBaseType == 0 ? null : ExtOffset, SelectedBaseType == 0 ? null : AdjTCP);
            if (Robots[SelectedRobot.Value].Bases == null)
                Robots[SelectedRobot.Value].Bases = new ObservableCollection<CalculatedBase>();
            if (Robots[SelectedRobot.Value].Bases.Any(x => x.Number == BaseNumber))
            {
                MessageBox.Show(String.Format("Base number {0} is already added to this robot", BaseNumber), "Base already added", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                Robots[SelectedRobot.Value].Bases.Add(currentBase);
                if (Robots[SelectedRobot.Value].BaseIDs == null)
                    Robots[SelectedRobot.Value].BaseIDs = new List<string>();
                Robots[SelectedRobot.Value].BaseIDs.Add(currentBase.BaseID);
                SelectedRobot = SelectedRobot;
            }
            Messenger.Default.Send(true, "enabledChanged");
        }

        private Coords GetCoords(Measurement measurement, Measurement measurement2 = null)
        {
            if (measurement2 == null)
                return new Coords(measurement.XIst, measurement.YIst, measurement.ZIst, measurement.RXIst, measurement.RYIst, measurement.RZIst, measurement.Name);
            else                
                return new Coords(measurement2.XSoll-measurement.XSoll, measurement2.YSoll - measurement.YSoll, measurement2.ZSoll - measurement.ZSoll, measurement2.RXSoll - measurement.RXSoll, measurement2.RYSoll - measurement.RYSoll, measurement2.RZSoll - measurement.RZSoll,name:"");
        }

        private ObservableCollection<Measurement> GetRobots(ObservableCollection<Measurement> measurements)
        {
            if (measurements == null)
                return null;
            ObservableCollection<Measurement> result = new ObservableCollection<Measurement>();
            foreach (var meas in measurements.Where(x => !string.IsNullOrEmpty(x.RobotType)))
                result.Add(meas);
            return result;
        }

        private void AddBase()
        {
            CalculatedBases.Add(new CalculatedBase(BaseNumber,BaseName, NewBase.XPos, NewBase.YPos, NewBase.ZPos, NewBase.RX, NewBase.RY, NewBase.RZ));
        }
        #endregion
    }
}
