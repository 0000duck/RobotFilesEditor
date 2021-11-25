using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using RobKalDat.Model.ProjectData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RobKalDat.ViewModel
{
    public class SafetyViewModel : ViewModelBase
    {

        #region ctor
        public SafetyViewModel()
        {
            IstValueVisible = false; RadiusVisible = false;  CellSpaceHeightVisible = false; IsAttachEnabled = false; IsAssignEnabled = false;
            SetCommnads();
            AssignRobotName = "-";
            Messenger.Default.Register<ObservableCollection<Measurement>>(this, "foundMeas", message => Measurements = message);
            Messenger.Default.Register<ObservableCollection<ItemInSafety>>(this, "foundSafety", message => ItemsInSafety = message);

        }

        #endregion

        #region props
        private ObservableCollection<Measurement> robots;
        public ObservableCollection<Measurement> Robots
        {
            get { return robots; }
            set
            {
                Set(ref robots, value);
                SelectedRobot = -1;
                CheckAssignEnabled();
            }
        }

        private int selectedRobot;
        public int SelectedRobot
        {
            get { return selectedRobot; }
            set
            {
                Set(ref selectedRobot, value);
                CheckAssignEnabled();
                if (SelectedRobot >= 0 && Robots[SelectedRobot].Safety != null)
                {
                    SelectedItemInSafety = 0;
                    RobotWithSafety = new KeyValuePair<Measurement, ObservableCollection<ItemInSafety>>(Robots[SelectedRobot], Robots[SelectedRobot].Safety);
                    ItemsInSafety = Robots[SelectedRobot].Safety;
                }
                else
                {
                    if (SelectedRobot >= 0 && Robots[SelectedRobot] != null)
                        RobotWithSafety = new KeyValuePair<Measurement, ObservableCollection<ItemInSafety>>(Robots[SelectedRobot], new ObservableCollection<ItemInSafety>());
                    ItemsInSafety = new ObservableCollection<ItemInSafety>();
                }
                    
            }
        }

        private ObservableCollection<SafetyData> safetyContent;
        public ObservableCollection<SafetyData> SafetyContent
        {
            get { return safetyContent; }
            set
            {
                Set(ref safetyContent, value);
            }
        }

        private ObservableCollection<Measurement> measurements;
        public ObservableCollection<Measurement> Measurements
        {
            get { return measurements; }
            set
            {
                ObservableCollection<Measurement> tempMeas = GetOnlyReals(value);
                Set(ref measurements, tempMeas);
                Robots = GetRobots(value);
                SelectedMeas = -1;
            }
        }

        private int selectedMeas;
        public int SelectedMeas
        {
            get { return selectedMeas; }
            set
            {
                Set(ref selectedMeas, value);
                CheckAssignEnabled();
            }
        }

        private int selectedItemInSafety;
        public int SelectedItemInSafety
        {
            get { return selectedItemInSafety; }
            set
            {
                Set(ref selectedItemInSafety, value);
                SetSafetyData();
                CheckAssignEnabled();
            }
        }

        private ObservableCollection<ItemInSafety> itemsInSafety;
        public ObservableCollection<ItemInSafety> ItemsInSafety
        {
            get { return itemsInSafety; }
            set
            {
                Set(ref itemsInSafety, value);
                CheckAssignEnabled();
                SetSafetyData();
                //SelectedItemInSafety = -1;
            }
        }

        private KeyValuePair<Measurement,ObservableCollection<ItemInSafety>> robotWithSafety;
        public KeyValuePair<Measurement,ObservableCollection<ItemInSafety>> RobotWithSafety
        {
            get
            {
                //Messenger.Default.Send(RobotWithSafety.Key, "safetyAssigned");
                return robotWithSafety;
            }
            set
            {
                Set(ref robotWithSafety ,value);
                SetSafetyData();
                CheckAssignEnabled();
                AssignRobotName = value.Key.Name;
                //Messenger.Default.Send(RobotWithSafety.Key, "safetyAssigned");
            }
        }

        private string assignRobotName;
        public string AssignRobotName
        {
            get { return assignRobotName; }
            set { Set(ref assignRobotName, value); }
        }

        private bool isAssignEnabled;
        public bool IsAssignEnabled
        {
            get { return isAssignEnabled; }
            set { Set (ref isAssignEnabled, value); }
        }

        private bool isAttachEnabled;
        public bool IsAttachEnabled
        {
            get { return isAttachEnabled; }
            set { Set(ref isAttachEnabled, value); }
        }

        private bool radiusVisible;
        public bool RadiusVisible
        {
            get { return radiusVisible; }
            set { Set(ref radiusVisible, value); }
        }

        private bool cellSpaceHeightVisible;
        public bool CellSpaceHeightVisible
        {
            get { return cellSpaceHeightVisible; }
            set { Set(ref cellSpaceHeightVisible, value); }
        }

        private bool istValueVisible;
        public bool IstValueVisible
        {
            get { return istValueVisible; }
            set { Set(ref istValueVisible, value); }
        }

        private bool nrVisible;
        public bool NrVisible
        {
            get { return nrVisible; }
            set { Set(ref nrVisible, value); }
        }

        private bool anglesVisible;
        public bool AnglesVisible
        {
            get { return anglesVisible; }
            set { Set(ref anglesVisible, value); }
        }

        private bool istAnglesVisible;
        public bool IstAnglesVisible
        {
            get { return istAnglesVisible; }
            set { Set(ref istAnglesVisible, value); }
        }


        #endregion

        #region commands
        public ICommand LoadSafety { get; set; }
        public ICommand AssignRobot { get; set; }
        public ICommand Confirm { get; set; }
        public ICommand AttachPressed { get; set; }
        #endregion

        #region methods
        private void SetCommnads()
        {
            LoadSafety = new RelayCommand(LoadSafetyExecute);
            AssignRobot = new RelayCommand(AssignRobotExecute);
            Confirm = new RelayCommand(ConfirmExecute);
            AttachPressed = new RelayCommand(AttachPressedExecute);
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

        private ObservableCollection<Measurement> GetOnlyReals(ObservableCollection<Measurement> measurements)
        {
            if (measurements == null)
                return null;
            ObservableCollection<Measurement> result = new ObservableCollection<Measurement>();
            foreach (var meas in measurements.Where(x => x.HasRealValues.ToLower() == "true"))
                result.Add(meas);
            return result;
        }

        private void ConfirmExecute()
        {
            Model.Methods.ReadSafetyMethods.SaveSafety(RobotWithSafety);
        }

        private void AssignRobotExecute()
        {
            RobotWithSafety = new KeyValuePair<Measurement, ObservableCollection<ItemInSafety>>(Robots[SelectedRobot], ItemsInSafety);
            RobotWithSafety.Key.Safety = ItemsInSafety;
           // Messenger.Default.Send(RobotWithSafety.Key, "safetyAssigned");
        }

        private void LoadSafetyExecute()
        {
            if (RobotWithSafety.Key == null)
            {
                MessageBox.Show("Select robot first!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (RobotWithSafety.Key.Safety != null && RobotWithSafety.Key.Safety.Count > 0)
            {
                System.Windows.Forms.DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Robot already has safety data assigned. Overwrite?", "Sure?", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);
                if (dialogResult == System.Windows.Forms.DialogResult.No)
                    return;
            }
            //string file = CommonLibrary.CommonMethods.SelectDirOrFile(false,"xls file","*.xls","xlsx file","*.xlsx");
            string file = CommonLibrary.CommonMethods.SelectDirOrFile(false, "Excel file", "*.xls; *.xlsm; *.xlsx");
            if (string.IsNullOrEmpty(file))
                return;
            //SelectedRobot = -1;
            Model.Methods.ReadSafetyMethods.ReadSafetyFromXML(file);
            RobotWithSafety = new KeyValuePair<Measurement, ObservableCollection<ItemInSafety>>(RobotWithSafety.Key, ItemsInSafety);
            RobotWithSafety.Key.Safety = ItemsInSafety;
            int selectedRob = SelectedRobot;
            Messenger.Default.Send(RobotWithSafety.Key, "safetyAssigned");
            SelectedRobot = selectedRob;
            CheckAssignEnabled();
        }

        private void AttachPressedExecute()
        {
            var itemInSafety = ItemsInSafety[SelectedItemInSafety];
            int selectedRob = SelectedRobot, selectedItm = SelectedItemInSafety;
            var robot = RobotWithSafety.Key;
            var selectedMeas = Measurements[SelectedMeas];
            ObservableCollection<Coords> result = Model.Methods.Methods.CalculateSafety(itemInSafety, robot, selectedMeas);
            if (itemInSafety.Type == "Safespaces")
            {
                itemInSafety.SafeSpaces.OriginIst = result[0];
                itemInSafety.SafeSpaces.RefObj = Measurements[SelectedMeas].Name;
                SafetyContent[0].IstPoint = result[0];
                RobotWithSafety.Value[SelectedItemInSafety].SafeSpaces.OriginIst=result[0];
                Messenger.Default.Send(RobotWithSafety.Key, "safetyAssigned");
            }
            if (itemInSafety.Type == "Cellspace")
            {
                itemInSafety.CellSpaceIst = new List<Coords>();
               // RobotWithSafety.Key.Safety.Where(x => x.Type == "CellSpace").FirstOrDefault() = new ItemInSafety();
                result.ToList().ForEach(x => itemInSafety.CellSpaceIst.Add(x));
                //result.ToList().ForEach(x => RobotWithSafety.Value[SelectedItemInSafety].CellSpaceIst.Add(x));
                int counter = 1;
                while (SafetyContent.Count > counter)
                {
                    SafetyContent[counter].IstPoint = result[counter - 1];
                    counter++;
                }
                Messenger.Default.Send(RobotWithSafety.Key, "safetyAssigned");
            }
            ObservableCollection<SafetyData> tempContent = new ObservableCollection<SafetyData>();
            tempContent = SafetyContent;
            SafetyContent = null;
            SafetyContent = tempContent;
            SelectedRobot = selectedRob;
            SelectedItemInSafety = selectedItm;
            //Messenger.Default.Send(ItemsInSafety, "foundSafety");


        }

        private void CheckAssignEnabled()
        {
            if (Robots != null && Robots.Count > 0 && ItemsInSafety != null && ItemsInSafety.Count > 0 && (SelectedRobot >= 0 || RobotWithSafety.Key!= null))
            {
                IsAssignEnabled = true;
                if (ItemsInSafety != null && SelectedItemInSafety >= 0 && ItemsInSafety[SelectedItemInSafety].Type != "Tool" && SelectedMeas > 0 && RobotWithSafety.Key!=null)
                    IsAttachEnabled = true;
                else
                    IsAttachEnabled = false;
            }
            else
            {
                IsAssignEnabled = false;
                IsAttachEnabled = false;
            }
        }


        private void SetSafetyData()
        {
            if (ItemsInSafety.Count == 0 || SelectedItemInSafety < 0)
            {
                SafetyContent = new ObservableCollection<SafetyData>();
                return;
            }
            var selectedSafetyItem = ItemsInSafety[SelectedItemInSafety];
            ObservableCollection<SafetyData> safetyData = new ObservableCollection<SafetyData>();
            int counter = 1;
            switch (selectedSafetyItem.Type)
            {
                case "Tool":
                    RadiusVisible = true; CellSpaceHeightVisible = false; IstValueVisible = false; NrVisible = true; AnglesVisible = false; IstAnglesVisible = false;
                    safetyData.Add(new SafetyData { Index = SelectedItemInSafety, Name = "TCP", Number = 0, Type = "Tool", SollPoint = selectedSafetyItem.Tool.TCP, Radius = 0});
                    
                    foreach (var sphere in selectedSafetyItem.Tool.Spheres)
                    {
                        safetyData.Add(new SafetyData { Index = SelectedItemInSafety, Name = "Sphere" + counter, Number = counter, Type = "Tool", SollPoint = sphere.Coordinates, Radius = sphere.Radius });
                        counter++;
                    }
                    break;
                case "Cellspace":
                    RadiusVisible = false; CellSpaceHeightVisible = true; IstValueVisible = true; NrVisible = true; AnglesVisible = false; IstAnglesVisible = false;
                    safetyData.Add(new SafetyData { Index = SelectedItemInSafety, Name = "Height", Number = 0, Type = "Cellspace", CellSpaceHeight = selectedSafetyItem.CellSpaceHeight});
                    foreach (var point in selectedSafetyItem.CellSpaceSoll)
                    {
                        safetyData.Add(new SafetyData { Index = SelectedItemInSafety, Name = "cs" + counter, Number = counter, Type = "Cellspace", SollPoint = new Coords(point.X, point.Y, point.Z, point.RX, point.RY, point.RZ), IstPoint = selectedSafetyItem.CellSpaceIst.Count == selectedSafetyItem.CellSpaceSoll.Count ? selectedSafetyItem.CellSpaceIst[counter-1] : new Coords(0,0,0,0,0,0) });
                        counter++;
                    }
                    break;
                case "Safespaces":
                    RadiusVisible = false; CellSpaceHeightVisible = false; IstValueVisible = true; NrVisible = false; AnglesVisible = true; IstAnglesVisible = true;
                    safetyData.Add(new SafetyData() { Index = SelectedItemInSafety, Name = "Origin", SollPoint = selectedSafetyItem.SafeSpaces.OriginSoll, IstPoint = selectedSafetyItem.SafeSpaces.OriginIst });
                    safetyData.Add(new SafetyData() { Index = SelectedItemInSafety, Name = "Dimensions", SollPoint = selectedSafetyItem.SafeSpaces.Dimensions });
                    break;
                default:
                    break;
            }
            SafetyContent = safetyData;
        }


        #endregion
    }
}
