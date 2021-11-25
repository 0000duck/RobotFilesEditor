using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Dialogs
{
    public class TypeCollisionCommentViewModel : ViewModelBase
    {
        int _number;
        public int Number
        {
            get { return _number; }
            set
            {
                _number = value;
                _wholeText = "Collision number " + Number.ToString() + " does not have any description!";
                RaisePropertyChanged<int>(() => Number);
            }
        }

        string _wholeText;
        public string WholeText
        {
            get { return _wholeText; }
            set
            {                
                RaisePropertyChanged<string>(() => WholeText);
            }
        }

        string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                RaisePropertyChanged<string>(() => Description);
            }
        }

        public TypeCollisionCommentViewModel(int number)
        {
            Number = number;
        }

        RelayCommand _setDescription;
        public RelayCommand SetDescription
        {
            get
            {
                if (_setDescription == null)
                {
                    _setDescription = new RelayCommand(descrChange);
                }
                return _setDescription;
            }
        }

        void descrChange()
        {
            
        }
    }
}
