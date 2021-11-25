using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Dialogs
{
    public class SignalNameViewModel : ViewModelBase
    {
        public KeyValuePair<int, List<string>> Pair { get; set; }

        public SignalNameViewModel(KeyValuePair<int, List<string>> pair)
        {
            Pair = pair;
            CorrectedName = Pair.Value[0];
        }

        private string _correctedName;
        public string CorrectedName
        {
            get { return _correctedName; }
            set
            {
                _correctedName = value;
                RaisePropertyChanged(() => CorrectedName);
            }
        }

        private string _standardName;
        public string StandardName
        {
            get { return _standardName; }
            set
            {
                _standardName = value;
                RaisePropertyChanged(() => StandardName);
            }
        }

        public string ShouldNameString { get; set; }
    }
}
