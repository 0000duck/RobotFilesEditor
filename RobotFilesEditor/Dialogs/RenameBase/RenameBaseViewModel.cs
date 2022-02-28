using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Dialogs.RenameBase
{
    public class RenameBaseViewModel : ViewModelBase
    {
        public KeyValuePair<int, List<string>> Pair { get; set; }

        public RenameBaseViewModel(KeyValuePair<int, List<string>> pair)
        {
            Pair = pair;
            if (Pair.Key > 0 && Pair.Key < 11)
                StandardName = "xxxXXxx";
            else if (Pair.Key > 10 && Pair.Key < 21)
                StandardName = "xxxXXxx_ext";
            else
                StandardName = ConfigurationManager.AppSettings["BaseNames" + GlobalData.ControllerType.Replace(" ", "_")].Split(',').Select(s => s.Trim()).ToList()[pair.Key];
            ShouldNameString = "Name for base number " + pair.Key + " should be " + StandardName;
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
