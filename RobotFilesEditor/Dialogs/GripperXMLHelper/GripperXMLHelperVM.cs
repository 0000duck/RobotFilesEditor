using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RobotFilesEditor.Dialogs.GripperXMLHelper
{
    public class GripperXMLHelperVM : ViewModelBase
    {
        public GripperXMLHelperVM()
        {
            FGSelected = true;
            StartAddress = "2049";
            NrOfInputs = "40";
            NrOfOutputs = "40";
            SetCommands();
        }

        private bool fGSelected;
        public bool FGSelected
        {
            get { return fGSelected; }
            set
            {
                Set(ref fGSelected, value);
            }
        }

        private bool fCSelected;
        public bool FCSelected
        {
            get { return fCSelected; }
            set
            {
                Set(ref fCSelected, value);
            }
        }

        private string startAddress;
        public string StartAddress
        {
            get { return startAddress; }
            set
            {
               Set(ref startAddress, value); 
            }
        }

        private string nrOfInputs;
        public string NrOfInputs
        {
            get { return nrOfInputs; }
            set
            {
                Set(ref nrOfInputs, value);
            }
        }

        private string nrOfOutputs;
        public string NrOfOutputs
        {
            get { return nrOfOutputs; }
            set
            {
                Set(ref nrOfOutputs, value);
            }
        }

        private void SetCommands()
        {
            OKCommand = new RelayCommand(OKCommandExecute);
        }

        private void OKCommandExecute()
        {
            var window = Application.Current.Windows
            .Cast<Window>()
            .Single(w => w.DataContext == this);
            window.DialogResult = true;
            window.Close();
        }

        public ICommand OKCommand { get; set; }
    }
}
