using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RobotFilesEditor.Dialogs.SelectRobotType
{
    public class SelectRobotTypeVM : ViewModelBase
    {
        #region ctor
        public SelectRobotTypeVM()
        {
            Robotypes = GetRobotTypes();
            IsWelding = false;
        }
        #endregion

        #region properties
        private ObservableCollection<string> _robotypes;
        public ObservableCollection<string> Robotypes
        {
            get { return _robotypes; }
            set
            {
                if (_robotypes != value)
                {
                    _robotypes = value;
                    RaisePropertyChanged(() => Robotypes);
                }
            }
        }

        private string _selectedRobot;
        public string SelectedRobot
        {
            get { return _selectedRobot; }
            set
            {
                if (_selectedRobot != value)
                {
                    _selectedRobot = value;
                    if (_selectedRobot != null)
                        IsOkEnabled = true;
                    else
                        IsOkEnabled = false;
                    RaisePropertyChanged(() => SelectedRobot);
                }
            }
        }

        private bool _isOkEnabled;

        public bool IsOkEnabled
        {
            get { return _isOkEnabled; }
            set
            {
                if (_isOkEnabled != value)
                {
                    _isOkEnabled = value;
                    RaisePropertyChanged(() => IsOkEnabled);
                }
            }
        }

        private bool _isWelding;

        public bool IsWelding
        {
            get { return _isWelding; }
            set
            {
                if (_isWelding != value)
                {
                    _isWelding = value;
                    RaisePropertyChanged(() => IsWelding);
                }
            }
        }

        #endregion

        #region commands
        RelayCommand _okClick;
        public RelayCommand OKClick
        {
            get
            {
                if (_okClick == null)
                {
                    _okClick = new RelayCommand(okClicked);
                }
                return _okClick;
            }
        }

        private void okClicked()
        {
            var window = Application.Current.Windows
            .Cast<Window>()
            .Single(w => w.DataContext == this);
            window.DialogResult = true;
            window.Close();
        }

        RelayCommand _cancelClick;
        public RelayCommand CancelClick
        {
            get
            {
                if (_cancelClick == null)
                {
                    _cancelClick = new RelayCommand(cancelClicked);
                }
                return _cancelClick;
            }
        }

        private void cancelClicked()
        {
            var window = Application.Current.Windows
            .Cast<Window>()
            .Single(w => w.DataContext == this);
            window.DialogResult = false;
            window.Close();
        }
        #endregion

        #region methods
        private ObservableCollection<string> GetRobotTypes()
        {
            ObservableCollection<string> result = new ObservableCollection<string>();
            List<string> robotTypes = ConfigurationManager.AppSettings["RobotTypes" + GlobalData.ControllerType.Replace(" ", "_")].Split(',').Select(s => s.Trim()).ToList();
            foreach (string robottype in robotTypes)
                result.Add(robottype);
            return result;
        }
        #endregion
    }

}
