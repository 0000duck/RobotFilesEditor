using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Dialogs.RenameTool
{
    public class RenameToolViewModel : ViewModelBase
    {
        public KeyValuePair<int, List<string>> Pair { get; set; }

        public RenameToolViewModel (KeyValuePair<int, List<string>> pair)
        {
            Pair = pair;
            StandardName = ConfigurationManager.AppSettings["ToolNames" + GlobalData.ControllerType.Replace(" ", "_")].Split(',').Select(s => s.Trim()).ToList()[pair.Key];
            //Pair.Value.Add(StandardName);
            ShouldNameString = "Name for tool number " + pair.Key + " should be " + StandardName;
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
