using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RobotFilesEditor.Dialogs.BaseShifter
{
    public class BaseShifterViewModel : ViewModelBase
    {
        #region ctor
        public BaseShifterViewModel(int baseNum, string pathName)
        {
            InputText = "Paste Base_Data for base " + baseNum + " to be shifted to.\r\nPath name: "+ pathName;
            SetCommands();
        }

        #endregion

        #region properties
        private string inputText;
        public string InputText
        {
            get { return inputText; }
            set { inputText = value; }
        }

        private string baseData;
        public string BaseData
        {
            get { return baseData; }
            set { baseData = value; }
        }
        #endregion

        #region commands
        public ICommand OK { get; set; }
        public ICommand Cancel { get; set; }

        private void SetCommands()
        {
            OK = new RelayCommand(OKExecute);
            Cancel = new RelayCommand(CancelExecute);
        }

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
