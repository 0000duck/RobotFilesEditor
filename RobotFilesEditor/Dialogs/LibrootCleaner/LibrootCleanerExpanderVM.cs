using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Dialogs.LibrootCleaner
{
    public class LibrootCleanerExpanderVM : ViewModelBase
    {
        #region ctor
        public LibrootCleanerExpanderVM(string expanderName, ObservableCollection<CojtPair> items)
        {
            ExpanderName = expanderName;
            Items = items;
        }
        #endregion

        #region properties
        public string ExpanderName { get; set; }
        public ObservableCollection<CojtPair> Items { get; set; }
        public int SelectedIndex { get; set; }
        #endregion
    }
}
