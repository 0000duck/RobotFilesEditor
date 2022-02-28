using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Dialogs.SelectJob
{
    public class SelectJobViewModel :ViewModelBase
    {
        #region Ctor
        public SelectJobViewModel(KeyValuePair<int, List<string>> pair, string robot)
        {
            if (pair.Value.Count < 1)
                pair = new KeyValuePair<int, List<string>>(pair.Key, new List<string>() { "Job " + pair.Key + " name" });
            Pair = pair;

            SelectedItem = Pair.Value[0].Trim();
            TextBlockValue = "Select job description.\r\nRobot: " + robot + "\r\nJob number:";
            //ResultText = SelectedItem;
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


        string _selectedItem;
        public string SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value.Trim();
                    RaisePropertyChanged(() => SelectedItem);
                    ResultText = value.Trim();
                }
            }
        }

        string resultText;
        public string ResultText
        {
            get { return resultText; }
            set
            {
                if (resultText != value)
                {
                    resultText = value;
                    RaisePropertyChanged(() => ResultText);
                }
            }
        }

        private string textBlockValue;

        public string TextBlockValue
        {
            get { return textBlockValue; }
            set { Set(ref textBlockValue, value); }
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
