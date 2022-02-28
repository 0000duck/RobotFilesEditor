using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;

namespace RobotFilesEditor.Dialogs.SelectCollision
{
    public class SelectColisionViewModel : ViewModelBase
    {
        #region fields
        //int maxLength = GlobalData.ControllerType == "FANUC" ? 32 : 40;
        int maxLength;
        bool limitLength;
        #endregion

        #region Ctor
        public SelectColisionViewModel(KeyValuePair<int, List<string>> pair, bool fillDescr, int lenght, bool line2Visible, bool releaseVisible = true)
        {
            Line1Selected = true;
            Line2Visibility = line2Visible ? Visibility.Visible : Visibility.Collapsed;
            List<string> pairValue = new List<string>();
            if (line2Visible)
            {
                foreach (var coll in pair.Value)
                {
                    pairValue.Add(" Coll " + pair.Key + ": " + coll);
                }
            }
            else
                pairValue = pair.Value;
            maxLength = lenght;
            limitLength = fillDescr;
            if (fillDescr || lenght > 0)
            {
                IsVisible = Visibility.Visible;
                LimitText = "Description length is limited to " + maxLength + " signs for each line!";
            }
            else
                IsVisible = Visibility.Collapsed;
            if (fillDescr && line2Visible)
                CollDescrWarning = Visibility.Visible;
            else
                CollDescrWarning = Visibility.Collapsed;

            Pair = new KeyValuePair<int, List<string>>(pair.Key,pairValue);
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
                Set(ref _selectedIndexInReq, value);
                if (value > -1)
                {
                    RequestTextLine2 = string.Empty;
                    RequestText = Pair.Value[value];
                    
                }
            }
        }

        private int _selectedIndexInClr;
        public int SelectedIndexInClr
        {
            get { return _selectedIndexInClr; }
            set
            {
                if (value > -1)
                {
                    Set(ref _selectedIndexInClr, value);
                    ReleaseText = Pair.Value[value];
                }
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
                    Regex collRegex = new Regex(@"^\sColl\s" + Pair.Key+ @":\s", RegexOptions.IgnoreCase);
                    if (collRegex.IsMatch(value) || GlobalData.ControllerType != null && GlobalData.ControllerType != "FANUC")
                    {
                        if ((limitLength || Line2Visibility == Visibility.Visible) && value.Length > maxLength)
                            if (Line2Visibility == Visibility.Visible)
                            {
                                if (RequestTextLine2 == null)
                                    RequestTextLine2 = string.Empty;
                                RequestTextLine2 = value.Substring(maxLength, value.Length - maxLength) + RequestTextLine2;
                                //RequestTextLine2 = value.Substring(maxLength, value.Length - maxLength);
                                value = value.Substring(0, maxLength).TrimEnd();
                                _requestText = value;
                            }
                            else
                                value = value.Substring(0, maxLength);
                        else if (value.Contains("\r\n"))
                        {
                            _requestText = new Regex(@".*(?=\r\n)").Match(value).ToString();
                            RequestTextLine2 = new Regex(@"(?<=\r\n).*").Match(value).ToString();
                        }
                        else
                        {
                            _requestText = value;
                        }
                        ReleaseText = value;
                    }
                    RaisePropertyChanged(() => RequestText);
                    
                }
            }
        }

        string _requestTextLine2;
        public string RequestTextLine2
        {
            get { return _requestTextLine2; }
            set
            {
                if (_requestTextLine2 != value)
                {

                    if ((limitLength || Line2Visibility == Visibility.Visible) && value.Length <= maxLength)
                    //value = value.Substring(0, maxLength);
                    {
                        _requestTextLine2 = " " + value.TrimStart();
                        //_requestTextLine2 = value;
                        RaisePropertyChanged(() => RequestTextLine2);
                        if (string.IsNullOrEmpty(value.Trim()))
                        {
                            Line1Selected = true;
                            Line2DescriptionEnabled = false;
                        }
                        else
                            Line2DescriptionEnabled = true;


                    }
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

        string _releaseTextLine2;
        public string ReleaseTextLine2
        {
            get { return _releaseTextLine2; }
            set
            {
                if (_releaseTextLine2 != value)
                {
                    if (limitLength && value.Length > maxLength)
                        value = value.Substring(0, maxLength);
                    _releaseTextLine2 = value;
                    RaisePropertyChanged(() => ReleaseTextLine2);

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

        Visibility _line2Visibility;
        public Visibility Line2Visibility
        {
            get { return _line2Visibility; }
            set
            {
                if (_line2Visibility != value)
                {
                    _line2Visibility = value;
                    //SelectedIndexInClr = -1;
                    RaisePropertyChanged(() => Line2Visibility);
                }
            }
        }

        Visibility _collDescrWarning;
        public Visibility CollDescrWarning
        {
            get { return _collDescrWarning; }
            set
            {
                if (_collDescrWarning != value)
                {
                    _collDescrWarning = value;
                    //SelectedIndexInClr = -1;
                    RaisePropertyChanged(() => CollDescrWarning);
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

        private bool line1Selected;
        public bool Line1Selected
        {
            get { return line1Selected; }
            set
            {
                Set(ref line1Selected, value);
            }
        }

        private bool line2Selected;
        public bool Line2Selected
        {
            get { return line2Selected; }
            set
            {
                Set(ref line2Selected, value);
            }
        }

        private bool line2DescriptionEnabled;
        public bool Line2DescriptionEnabled
        {
            get { return line2DescriptionEnabled; }
            set
            {
                Set(ref line2DescriptionEnabled, value);
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
