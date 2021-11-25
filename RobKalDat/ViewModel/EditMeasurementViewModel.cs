using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using RobKalDat.Model.ProjectData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RobKalDat.ViewModel
{
    public class EditMeasurementViewModel : ViewModelBase
    {
        #region ctor
        public EditMeasurementViewModel()
        {
            SetCommands();
            Messenger.Default.Register<Measurement>(this, "measurementToModify", message => InputMeasurement = message);
            Messenger.Default.Register<bool>(this, "addOrEdit", message => AddMeas = message);
        }

        #endregion

        #region props
        private Measurement inputMeasurement;
        public Measurement InputMeasurement
        {
            get { return inputMeasurement; }
            set { Set(ref inputMeasurement, value); }
        }

        private Measurement resultMeasurement;
        public Measurement ResultMeasurement
        {
            get { return resultMeasurement; }
            set { Set(ref resultMeasurement, value); }
        }

        private bool addMeas;
        public bool AddMeas
        {
            get { return addMeas; }
            set { Set(ref addMeas, value); }
        }


        #endregion

        #region commands
        public ICommand OK { get; set; }
        public ICommand Cancel { get; set; }
        #endregion

        #region methods
        private void SetCommands()
        {
            OK = new RelayCommand(OkExecute);
            Cancel = new RelayCommand(CancelExecute);
        }

        private void OkExecute()
        {
            UpdateMeasurement();
            var window = Application.Current.Windows
            .Cast<Window>()
            .Single(w => w.DataContext == this);
            window.DialogResult = true;
            // Cleanup();
            //CleanupVM();
            window.Close();
        }

        private void UpdateMeasurement()
        {
            //Messenger.Default.Send(true, "refreshList");
            if (addMeas)
                Messenger.Default.Send(InputMeasurement, "addMeasurement");
            else
                Messenger.Default.Send(InputMeasurement, "updateMeasurement");
        }

        private void CancelExecute()
        {
            var window = Application.Current.Windows
            .Cast<Window>()
            .Single(w => w.DataContext == this);
            window.DialogResult = false;
            // Cleanup();
            //CleanupVM();
            window.Close();
        }
        #endregion

    }
}
