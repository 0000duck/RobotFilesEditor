using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Dialogs
{
    public class CompareSOVandOLPViewModel: ViewModelBase
    {
        #region ctor
        public CompareSOVandOLPViewModel(IDictionary<string,string> filesSet1, IDictionary<string, string> filesSet2)
        {
            PrepareDataSet(filesSet1, filesSet2);
        }
        #endregion

        #region properties
        private ObservableCollection<ComparerDataSet> items;
        public ObservableCollection<ComparerDataSet> Items
        {
            get { return items; }
            set
            {
                if (items != value)
                {
                    items = value;
                }
            }
        }

        #endregion

        #region methods
        private void PrepareDataSet(IDictionary<string, string> filesSet1, IDictionary<string, string> filesSet2)
        {
            List<string> foundPaths = new List<string>();
            foreach (var file in filesSet1)
            {
                foundPaths.Add(Path.GetFileNameWithoutExtension(file.Key).ToLower());
                if (filesSet2.Any(x => Path.GetFileNameWithoutExtension(x.Key.ToLower()) == Path.GetFileNameWithoutExtension(file.Key).ToLower()))
                {
                    var recordInSet2 = filesSet2.First(x => Path.GetFileNameWithoutExtension(x.Key.ToLower()) == Path.GetFileNameWithoutExtension(file.Key).ToLower());
                    if (Items == null)
                        Items = new ObservableCollection<ComparerDataSet>();
                    Items.Add(new ComparerDataSet(Path.GetFileNameWithoutExtension(file.Key), recordInSet2.Key, file.Value, recordInSet2.Value));
                }
                else
                    Items.Add(new ComparerDataSet(Path.GetFileNameWithoutExtension(file.Key), string.Empty, file.Value, string.Empty));
            }
            foreach (var file in filesSet2.Where(x => !foundPaths.Contains(Path.GetFileNameWithoutExtension(x.Key.ToLower()))))
            {
                Items.Add(new ComparerDataSet(string.Empty, Path.GetFileNameWithoutExtension(file.Key), string.Empty, file.Value));
            }
            RaisePropertyChanged(() => Items);
        }
        #endregion
    }

    public class ComparerDataSet
    {
        public string FileInSet1 { get; set; }
        public string FileInSet2 { get; set; }
        public string ContentSet1 { get; set; }
        public string ContentSet2 { get; set; }
        public bool IsSame { get; set; }

        public ComparerDataSet(string fileInSet1, string fileInSet2, string contentSet1, string contentSet2)
        {
            FileInSet1 = fileInSet1;
            FileInSet2 = fileInSet1;
            ContentSet1 = contentSet1;
            ContentSet2 = contentSet2;
            IsSame = contentSet1 == contentSet2 ? true : false;
        }
    }
}
