using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using RobKalDat.Model;
using RobKalDat.Model.ProjectData;
using System.Linq;

namespace RobKalDat.ViewModel
{
    public class MainViewModel : ViewModelBase
    { 
        #region Props
        private int selectedTab;

        public int SelectedTab
        {
            get { return selectedTab; }
            set { Set(ref selectedTab, value); }
        }

        private ObservableCollection<Measurement> foundMeasurements;

        private ObservableCollection<Measurement> FoundMeasurements
        {
            get { return foundMeasurements; }
            set {
                Set(ref foundMeasurements, value);
                //RaisePropertyChanged(() => FoundMeasurements);
            }
        }

        private bool isWindowEnabled;
        public bool IsWindowEnabled
        {
            get { return isWindowEnabled; }
            set
            {
                Set(ref isWindowEnabled, value);
            }
        }

        private Measurement safetyAssingedRobot;
        public Measurement SafetyAssingedRobot
        {
            get { return safetyAssingedRobot; }
            set
            {
                Set(ref safetyAssingedRobot, value);
                FoundMeasurements.Where(x => x.Name == value.Name).FirstOrDefault().Safety = value.Safety;
                Messenger.Default.Send(FoundMeasurements, "foundMeas");
            }
        }

        #endregion
        #region COMMANDS
        public ICommand OpenProjectCommand { get; set; }
        public ICommand SaveProjectCommand { get; set; }
        public ICommand NewProjectCommand { get; set; }
        #endregion

        #region ctor
        public MainViewModel(IDataService dataService)
        {
            IsWindowEnabled = true;
            SelectedTab = 0;
            SetCommands();
            Messenger.Default.Register<bool>(this, "enabledChanged", message => IsWindowEnabled = message);
            Messenger.Default.Register<ObservableCollection<Measurement>>(this, "importFromExcel", message => FoundMeasurements = message);
            Messenger.Default.Register<Measurement>(this, "safetyAssigned", message => SafetyAssingedRobot = message);
        }
        #endregion

        #region methods
        private void SetCommands()
        {
            OpenProjectCommand = new RelayCommand(OpenProjectExecute);
            SaveProjectCommand = new RelayCommand(SaveProjectExecute);
            NewProjectCommand = new RelayCommand(NewProjectExecute);
        }

        private void NewProjectExecute()
        {
            IsWindowEnabled = false;
            System.Windows.Forms.DialogResult dialog = System.Windows.Forms.MessageBox.Show("Are you sure?", "Sure?", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);
            if (dialog == System.Windows.Forms.DialogResult.Yes)
            {
                FoundMeasurements = new ObservableCollection<Measurement>();
                Messenger.Default.Send(FoundMeasurements, "foundMeas");
            }
            IsWindowEnabled = true;
        }

        private void SaveProjectExecute()
        {
            IsWindowEnabled = false;
            Model.Methods.Methods.SaveProject(FoundMeasurements);
            IsWindowEnabled = true;
        }

        private void OpenProjectExecute()
        {
            ViewModelLocator.Cleanup();
            IsWindowEnabled = false;
            ObservableCollection<Measurement> tempMeas = Model.Methods.Methods.LoadProject();
            if (tempMeas != null)
            {
                FoundMeasurements = tempMeas;
                Messenger.Default.Send(FoundMeasurements, "foundMeas");
            }
            IsWindowEnabled = true;
        }
        #endregion
    }
}