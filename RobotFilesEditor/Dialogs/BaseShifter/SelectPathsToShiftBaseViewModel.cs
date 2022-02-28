using RobotFilesEditor.Model.Operations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using System.Windows;

namespace RobotFilesEditor.Dialogs.BaseShifter
{
    public class SelectPathsToShiftBaseViewModel : ViewModelBase
    {
        #region ctor
        public SelectPathsToShiftBaseViewModel(IDictionary<string, SrcDatPair> filepairs)
        {
            AvailablePaths = filepairs;
            SetCommands();
        }
        #endregion

        #region properties
        private IDictionary<string, SrcDatPair> availablePaths;
        public IDictionary<string, SrcDatPair> AvailablePaths
        {
            get { return availablePaths; }
            set
            {
                availablePaths = value;
                RaisePropertyChanged(() => AvailablePaths);
            }
        }

        private KeyValuePair<string,SrcDatPair> selectedPaths;
        public KeyValuePair<string,SrcDatPair> SelectedPaths
        {
            get { return selectedPaths; }
            set
            {
                selectedPaths = value;
                RaisePropertyChanged(() => SelectedPaths);
            }
        }

        #endregion

        #region Methods

        private void SetCommands()
        {
            OKCommand = new RelayCommand(OKExecute);
            CancelCommand = new RelayCommand(CancelExecute);
        }
    
        #endregion

        #region commands
        public ICommand OKCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        private void CancelExecute()
        {
            var window = Application.Current.Windows
            .Cast<Window>()
            .Single(w => w.DataContext == this);
            window.DialogResult = false;
            window.Close();
        }

        private void OKExecute()
        {
            var window = Application.Current.Windows
            .Cast<Window>()
            .Single(w => w.DataContext == this);
            window.DialogResult = true;
            window.Close();
        }
        #endregion


    }
}
