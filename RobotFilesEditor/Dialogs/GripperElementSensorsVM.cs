using GalaSoft.MvvmLight;
using RobotFilesEditor.Model.DataOrganization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Dialogs
{
    public class GripperElementSensorsVM : ViewModelBase
    {
        #region ctor
        public GripperElementSensorsVM()
        {
            GripperElementSensors = new GripperElementSensors();
            if (GlobalData.ControllerType == "KRC2 V8" || GlobalData.ControllerType == "KRC4")
                Controller = 1;
            else
                Controller = 0;
        }
        #endregion

        #region Properties
        public GripperElementSensors GripperElementSensors { get; set; }

        private int _controller;

        public int Controller
        {
            get { return _controller; }
            set
            {
                if (_controller != value)
                {
                    _controller = value;
                    RaisePropertyChanged(() => Controller);
                }                
            }
        }


        private int _selectedGroupSensors;
        public int SelectedGroupSensors
        {
            get { return _selectedGroupSensors; }
            set
            {
                if (_selectedGroupSensors != value)
                {
                    _selectedGroupSensors = value;
                    RaisePropertyChanged(() => SelectedGroupSensors);
                    OnSelectedGroupSensorsChanged(value);

                }
            }
        }

        private int? _sensor1;
        public int? SelectedSensor1
        {
            get { return _sensor1; }
            set
            {
                if (_sensor1 != value)
                {
                    _sensor1 = value;
                    RaisePropertyChanged(() => SelectedSensor1);
                }
            }
        }

        private int? _sensor2;
        public int? SelectedSensor2
        {
            get { return _sensor2; }
            set
            {
                if (_sensor2 != value && GlobalData.ControllerType != "KRC2 V8")
                {
                    _sensor2 = value;
                    RaisePropertyChanged(() => SelectedSensor2);
                }
            }
        }

        private int? _sensor3;
        public int? SelectedSensor3
        {
            get { return _sensor3; }
            set
            {
                if (_sensor3 != value && GlobalData.ControllerType != "KRC2 V8")
                {
                    _sensor3 = value;
                    RaisePropertyChanged(() => SelectedSensor3);
                }
            }
        }

        private int? _sensor4;
        public int? SelectedSensor4
        {
            get { return _sensor4; }
            set
            {
                if (_sensor4 != value && GlobalData.ControllerType != "KRC2 V8")
                {
                    _sensor4 = value;
                    RaisePropertyChanged(() => SelectedSensor4);
                }
            }
        }

        private int? _sensor5;
        public int? SelectedSensor5
        {
            get { return _sensor5; }
            set
            {
                if (_sensor5 != value && GlobalData.ControllerType != "KRC2 V8")
                {
                    _sensor5 = value;
                    RaisePropertyChanged(() => SelectedSensor5);
                }
            }
        }

        private int? _sensor6;
        public int? SelectedSensor6
        {
            get { return _sensor6 ; }
            set
            {
                if (_sensor6 != value && GlobalData.ControllerType != "KRC2 V8")
                {
                    _sensor6 = value;
                    RaisePropertyChanged(() => SelectedSensor6);
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
        #endregion

        #region methods

        private void OnSelectedGroupSensorsChanged(int value)
        {

        }

        private void OnSelectedTypeChanged(string value)
        {

        }
        #endregion
    }
}
