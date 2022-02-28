using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Dialogs.IBUSSegmentName
{
    public class IBUSSegmentNameViewModel : ViewModelBase
    {
        public IBUSSegmentNameViewModel(int tool)
        {
            CurrentTool = tool;
        }

        private int _currentTool;

        public int CurrentTool
        {
            get { return _currentTool; }
            set
            {
                _currentTool = value;
                RaisePropertyChanged(() => CurrentTool);
            }
        }

        private string _nameTool;

        public string NameTool
        {
            get { return _nameTool; }
            set
            {
                _nameTool = value;
                RaisePropertyChanged(() => NameTool);
            }
        }

    }
}
