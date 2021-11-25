using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RobotFilesEditor.Model.DataInformations;
using RobotFilesEditor.Model.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RobotFilesEditor.Dialogs
{
    public class ChangeTypIdViewModel : ViewModelBase
    {
        #region fields

        private WeldpointShort currentPoint;
        private TypIdChangeMethods typIdChangeMethods;
        #endregion

        #region properties

        private List<string> _pointsList;
        public List<string> PointsList
        {
            get { return _pointsList; }
            set
            {
                _pointsList = value;
                RaisePropertyChanged(() => PointsList);
            }
        }

        private string _selectedPoint;
        public string SelectedPoint
        {
            get { return _selectedPoint; }
            set
            {
                if (_selectedPoint != value)
                {
                    _selectedPoint = value;
                    currentPoint = typIdChangeMethods.ChangeTypId(value);
                    PointsList = typIdChangeMethods.UpdatePointsList(PointsList,currentPoint);
                    RaisePropertyChanged(() => SelectedPoint);
                }
            }
        }

        #endregion



        #region ctor
        public ChangeTypIdViewModel(List<KeyValuePair<int, WeldpointBMW>> points, TypIdChangeMethods _typIdChangeMethods)
        {
            typIdChangeMethods = _typIdChangeMethods;
            PointsList = typIdChangeMethods.ConvertSpotPointsToList(points);
            SetCommands();
        }
        #endregion

        #region methods

        private void SetCommands()
        {
            OK = new RelayCommand(OKExecute);
            Cancel = new RelayCommand(CancelExecute);
        }

        private void OKExecute()
        {
            var window = Application.Current.Windows
            .Cast<Window>()
            .Single(w => w.DataContext == this);
            window.DialogResult = true;
            window.Close();
        }

        private void CancelExecute()
        {
           var window = Application.Current.Windows
            .Cast<Window>()
            .Single(w => w.DataContext == this);
            window.DialogResult = false;
            window.Close();
        }

        #endregion

        #region Commands

        public ICommand OK { get; set; }
        public ICommand Cancel { get; set; }

        #endregion
    }
}
