using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RobotFilesEditor.Dialogs.NameRobot
{
    public class NameRoboterViewModel : ViewModelBase
    {
        #region fields
        Regex plcNameRegex = new Regex(@"^\s*\d{2}\w{3}\d", RegexOptions.IgnoreCase);
        Regex stOrRobotNameRegex = new Regex(@"^\s*\d+\s*$", RegexOptions.IgnoreCase);
        Regex isValidNameRegex = new Regex(@"^\s*\d{2}\w{3}\d_ST\d{3}_IR\d{3}\s*$", RegexOptions.IgnoreCase);
        string tempName;
        #endregion

        #region ctor
        public NameRoboterViewModel()
        {
            OkEnabled = false;
        }
        #endregion

        #region properties
        private string robotName;
        public string RobotName
        {
            get { return robotName; }
            set {
                if (value != robotName && isValidNameRegex.IsMatch(value))
                {
                    PLCName = value.Substring(0, 6);
                    StationNr = value.Substring(9, 3);
                    RobotNr = value.Substring(15, 3);
                    robotName = value;
                    RaisePropertyChanged(() => RobotName);
                    OkEnabled = true;
                }
            }
        }

        private string pLCName;
        public string PLCName
        {
            get { return pLCName; }
            set
            {
                if (value != pLCName && plcNameRegex.IsMatch(value) && value.Length == 6)
                {
                    pLCName = value;
                    RaisePropertyChanged(() => PLCName);
                    CheckIfTempNameValid();
                }
            }
        }

        private string stationNr;
        public string StationNr
        {
            get { return stationNr; }
            set
                {
                if (value != stationNr && stOrRobotNameRegex.IsMatch(value) && value.Length > 0 && value.Length <= 3)
                {
                    while (value.Length < 3)
                        value = "0" + value;
                    stationNr = value;
                    RaisePropertyChanged(() => StationNr);
                    CheckIfTempNameValid();
                }
            }
        }

        private string robotNr;
        public string RobotNr
        {
            get { return robotNr; }
            set
                {
                if (value != robotNr && stOrRobotNameRegex.IsMatch(value) && value.Length > 0 && value.Length <= 3)
                {
                    while (value.Length < 3)
                        value = "0" + value;
                    robotNr = value;
                    RaisePropertyChanged(() => RobotNr);
                    CheckIfTempNameValid();
                }
            }
        }

        private bool okEnabled;
        public bool OkEnabled
        {
            get { return okEnabled; }
            set
            {
                Set(ref okEnabled, value);
            }
        }

        #endregion

        #region methods
        private void CheckIfTempNameValid()
        {
            tempName = PLCName + "_ST" + StationNr + "_IR" + RobotNr;
            if (isValidNameRegex.IsMatch(tempName))
                RobotName = tempName;
        }
        #endregion
    }
}
