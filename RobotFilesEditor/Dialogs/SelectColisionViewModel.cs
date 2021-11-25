using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;

namespace RobotFilesEditor.Dialogs
{
    public class SelectColisionViewModel : ViewModelBase
    {
        #region Ctor
        public SelectColisionViewModel(KeyValuePair<int, List<string>> pair)
        {
            Pair = pair;
            if (pair.Value.Count > 0)
            {
                SelectedIndexInReq = 0;
                //SelectedIndexInClr = 0;
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

        //string _selectedItemReq;
        //public string SelectedItemReq
        //{
        //    get { return _selectedItemReq; }
        //    set
        //    {
        //        if (_selectedItemReq != value)
        //        {
        //            _selectedItemReq = value;
        //            RequestText = value;
        //            ReleaseText = value;
        //            RaisePropertyChanged(() => SelectedItemReq);
        //        }
        //    }
        //}

        //string _selectedItemClr;
        //public string SelectedItemClr
        //{
        //    get { return _selectedItemClr; }
        //    set
        //    {
        //        if (_selectedItemClr != value)
        //        {
        //            _selectedItemClr = value;
        //            ReleaseText = value;
        //            RaisePropertyChanged(() => SelectedItemClr);
        //        }
        //    }
        //}

        string _requestText;
        public string RequestText
        {
            get { return _requestText; }
            set
            {
                if (_requestText != value)
                {
                    _requestText = value;
                    ReleaseText = value;
                    //SelectedIndexInReq = -1;
                    RaisePropertyChanged(() => RequestText);
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
                    _releaseText = value;
                   //SelectedIndexInClr = -1;
                    RaisePropertyChanged(() => ReleaseText);
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
