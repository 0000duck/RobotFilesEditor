using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;

namespace RobotFilesEditor.Dialogs
{
    public class SelectColisionViewModel : ViewModelBase
    {
        #region fields
        int maxLength = GlobalData.ControllerType == "FANUC" ? 32 : 40;
        bool limitLength;
        #endregion

        #region Ctor
        public SelectColisionViewModel(KeyValuePair<int, List<string>> pair, bool fillDescr, bool releaseVisible = true)
        {
            limitLength = fillDescr;
            if (fillDescr)
            {
                IsVisible = Visibility.Visible;
                LimitText = "Description lenght is limited to " + maxLength + " signs!";
            }
            else
                IsVisible = Visibility.Collapsed;

            Pair = pair;
            if (releaseVisible)
            {
                ReleaseVisible = Visibility.Visible;
                RequestHeader = "Request";
            }
            else
            {
                ReleaseVisible = Visibility.Collapsed;
                RequestHeader = "Request/Release";
            }
            if (pair.Value.Count > 0)
            {
                SelectedIndexInReq = 0;
            }
        }
        #endregion


        #region Properties
        KeyValuePair<int, List<string>> _pair;
        public KeyValuePair<int, List<string>> Pair
        {
            get { return _pair; }
            set
            {
                _pair = value;
                RaisePropertyChanged<KeyValuePair<int, List<string>>>(() => Pair);
            }
        }

        private int _selectedIndexInReq;
        public int SelectedIndexInReq
        {
            get { return _selectedIndexInReq; }
            set
            {
                Set(ref _selectedIndexInReq,value);
                if (value > -1)
                    RequestText = Pair.Value[value];
            }
        }

        private int _selectedIndexInClr;
        public int SelectedIndexInClr
        {
            get { return _selectedIndexInClr; }
            set
            {
                Set (ref _selectedIndexInClr, value);
                ReleaseText = Pair.Value[value];
            }
        }

        string _requestText;
        public string RequestText
        {
            get { return _requestText; }
            set
            {
                if (_requestText != value)
                {

                    if (limitLength && value.Length > maxLength)
                        value = value.Substring(0, maxLength);
                    
                    _requestText = value;
                    ReleaseText = value;
                    //SelectedIndexInReq = -1;
                    RaisePropertyChanged(() => RequestText);
                    
                }
            }
        }
        string _requestHeader;
        public string RequestHeader
        {
            get { return _requestHeader; }
            set
            {
                if (_requestHeader != value)
                {
                    _requestHeader = value;
                    RaisePropertyChanged(() => RequestHeader);
                }
            }
        }

        string _releaseText;
        public string ReleaseText
        {
            get { return _releaseText; }
            set
            {
                if (_releaseText != value)
                {
                    if (limitLength && value.Length > maxLength)
                        value = value.Substring(0, maxLength);
                    _releaseText = value;
                    RaisePropertyChanged(() => ReleaseText);

                }
            }
        }

        Visibility _releaseVisible;
        public Visibility ReleaseVisible
        {
            get { return _releaseVisible; }
            set
            {
                if (_releaseVisible != value)
                {
                    _releaseVisible = value;
                    //SelectedIndexInClr = -1;
                    RaisePropertyChanged(() => ReleaseVisible);
                }
            }
        }

        Visibility _isVisible;
        public Visibility IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    //SelectedIndexInClr = -1;
                    RaisePropertyChanged(() => IsVisible);
                }
            }
        }

        string _limitText;
        public string LimitText
        {
            get { return _limitText; }
            set
            {
                if (_limitText != value)
                {
                    _limitText = value;
                    RaisePropertyChanged(() => LimitText);
                }
            }
        }
        #endregion

        #region Methods
        #endregion

        #region Commands
        RelayCommand _closingCommand;
        public RelayCommand ClosingCommand
        {
            get
            {
                if (_closingCommand == null)
                {
                    _closingCommand = new RelayCommand(close);
                }
                return _closingCommand;
            }
        }
        void close()
        {
        }
        #endregion


    }
}
