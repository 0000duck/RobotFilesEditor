using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Dialogs.SetRobotStartAddress
{
    public class SetRobotStartAddressVM : ViewModelBase
    {
        public SetRobotStartAddressVM()
        {
        }

        private string _address;

        public string Address
        {
            get { return _address; }
            set
            {
                if (value != _address)
                {
                    _address = value;
                    RaisePropertyChanged(() => Address);
                }
            }
        }

    }
}
