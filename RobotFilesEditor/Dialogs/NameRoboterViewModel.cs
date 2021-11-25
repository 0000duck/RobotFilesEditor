using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Dialogs
{
    public class NameRoboterViewModel : ViewModelBase
    {
        private string robotName;

        public string RobotName
        {
            get { return robotName; }
            set {
                if (value != robotName)
                {
                    robotName = value;
                    RaisePropertyChanged(() => RobotName);
                }
            }
        }

    }
}
