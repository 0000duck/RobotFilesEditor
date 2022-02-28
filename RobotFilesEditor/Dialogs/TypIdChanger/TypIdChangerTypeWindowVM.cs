using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RobotFilesEditor.Dialogs.TypIdChanger
{
    public class TypIdChangerTypeWindowVM : ViewModelBase
    {
        public ICommand PointByPoint { get; set; }
        public ICommand BySameTypId { get; set; }

        #region ctor
        public TypIdChangerTypeWindowVM()
        {
            PointByPoint = new RelayCommand(PointByPointExecute);
            BySameTypId = new RelayCommand(BySameTypIdExecute);
        }
        #endregion

        #region properties
        private bool ispointbypoint;
        public bool Ispointbypoint
        {
            get { return ispointbypoint; }
            set { ispointbypoint = value; }
        }
        #endregion

        #region commands
        private void BySameTypIdExecute()
        {
            Ispointbypoint = false;
            Close();
        }

        private void PointByPointExecute()
        {
            Ispointbypoint = true;
            Close();
        }
        private void Close()
        {
            var window = Application.Current.Windows
            .Cast<Window>()
            .Single(w => w.DataContext == this);            
            window.Close();
        }
        #endregion
    }


}
