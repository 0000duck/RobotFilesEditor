using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RobotFilesEditor.Dialogs
{
    public class CompareOLPToSOVLineByLineViewModel : ViewModelBase
    {
        #region ctor
        public CompareOLPToSOVLineByLineViewModel(ObservableCollection<CompareData> data)
        {
            Data = data;
        }

        #endregion

        #region properties
        private ObservableCollection<CompareData> data;
        public ObservableCollection<CompareData> Data
        {
            get { return data; }
            set { data = value; }
        }

        #endregion

        #region methods
        #endregion
    }

    public class CompareData
    {
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public int IsSame { get; set; }

        public CompareData(string line1, string line2, int isSame)
        {
            Line1 = line1;
            Line2 = line2;
            IsSame = isSame;
            


        }
    }
}
