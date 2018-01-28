using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RobotFilesEditor
{
    public class ControlItem
    {       
        #region events

        public event EventHandler<ControlItem> ControlItemSelected;

        #endregion

        #region Property

        public string Content { get; set; }

        public ICommand ClickedCommand { get; set; }

        #endregion

        #region Constructors

        public ControlItem(string name)
        {
            Content = name;
            ClickedCommand = new RelayCommand(ClickedCommandExecute);
        }

        #endregion

        #region Medthods

        private void ClickedCommandExecute()
        {
            OnControlItemSelected();
        }

        protected void OnControlItemSelected()
        {
            ControlItemSelected?.Invoke(this, this);
        }

        #endregion        
    }
}
