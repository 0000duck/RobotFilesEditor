using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Dialogs.TypIdChanger
{
    public class TypIdChangerViewModel : ViewModelBase
    {
        #region properties
        private int _number;
        public int Number
        {
            get { return _number; }
            set
            {
                _number = value;
                RaisePropertyChanged(()=> Number);
            }
        }

        private int _typId;

        public int TypId
        {
            get { return _typId; }
            set
            {
                if (_typId != value)
                {
                    _typId = value;
                    RaisePropertyChanged(() => TypId);
                }
            }
        }

        #endregion

        #region ctor
        public TypIdChangerViewModel(int number, int typId)
        {
            Number = number;
            TypId = typId;
        }
        #endregion
    }
}
