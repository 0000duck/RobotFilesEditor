using GalaSoft.MvvmLight;
using RobotFilesEditor.Dialogs.CreateGripper;
using RobotFilesEditor.Model.DataOrganization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Dialogs.CreateGripper
{
    public class GripperElementsVM : ViewModelBase
    {
        #region ctor
        public GripperElementsVM()
        {
            GripperElement = new GripperElement();
        }
        #endregion

        #region Properties
        public delegate void SendPropertyHandler(object sender, EventArgs arg);
        public static event SendPropertyHandler SendProperty;

        public GripperElement GripperElement { get; set; }

        private int _selectedGroup;

        public int SelectedGroup
        {
            get { return _selectedGroup; }
            set
            {
                if (_selectedGroup != value)
                {
                    _selectedGroup = value;
                    RaisePropertyChanged(() => SelectedGroup);
                    OnSelectedGroupChanged(value);

                }
            }
        }

        private int _selectedClampsNr;
        public int SelectedClampsNr
        {
            get { return _selectedClampsNr; }
            set
            {
                if (_selectedClampsNr != value)
                {
                    _selectedClampsNr = value;
                    RaisePropertyChanged(() => SelectedClampsNr);
                    OnSelectedClampsChanged(value);
                }
            }
        }

        private string _startAddress;
        public string StartAddress
        {
            get { return _startAddress; }
            set
            {
                if (_startAddress != value)
                {
                    _startAddress = value;
                    RaisePropertyChanged(() => StartAddress);
                    OnSelectedStartAddressChanged(value);
                }
            }
        }

        private string _selectedClampType;
        public string SelectedClampType
        {
            get { return _selectedClampType; }
            set
            {
                if (_selectedClampType != value)
                {
                    _selectedClampType = value;
                    RaisePropertyChanged(() => SelectedClampType);
                    OnSelectedTypeChanged(value);
                }
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    RaisePropertyChanged(() => Name);
                    OnSelectedTypeChanged(value);
                }
            }
        }

        private int _outForClose;
        public int OutForClose
        {
            get { return _outForClose; }
            set
            {
                if (_outForClose != value)
                {
                    _outForClose = value;
                    RaisePropertyChanged(() => OutForClose);
                    OnOutForCloseChanged(value);
                }
            }
        }
        #endregion

        #region methods

        private void OnSelectedGroupChanged(int value)
        {
            CreateGripperViewModel.UpdateElements(GripperElement);
        }

        private void OnSelectedClampsChanged(int value)
        {

        }

        private void OnSelectedStartAddressChanged(string value)
        {
            
        }

        private void OnSelectedTypeChanged(string value)
        {
           
        }

        private void OnOutForCloseChanged(int value)
        {

        }

        #endregion
    }
}
