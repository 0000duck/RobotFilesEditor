using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RobotFilesEditor.Dialogs
{
    public class CreateAnyJobViewModel : ViewModelBase
    {
        #region fields
        private int? Id;
        #endregion

        #region ctor
        public CreateAnyJobViewModel(int? id)
        {
            Id = id;
            GlobalData.SelectedJobsForAnyJob = new ObservableCollection<KeyValuePair<string, int>>();
            Paths = FillPaths();
            SetCommands();
        }



        #endregion

        #region Properties

        private ObservableCollection<KeyValuePair<string, int>> _paths;

        public ObservableCollection<KeyValuePair<string, int>> Paths
        {
            get { return _paths; }
            set {
                if (_paths != value)
                {
                    _paths = value;
                    RaisePropertyChanged(() => Paths);
                }                
            }
        }

        private KeyValuePair<string,int> _selectedJob;

        public KeyValuePair<string,int> SelectedJob
        {
            get { return _selectedJob; }
            set {
                _selectedJob = value;
                RaisePropertyChanged(() => SelectedJob);
                OnSelectedJobChanged(value);
                _selectedItem = true;

                //_selectedJob = new KeyValuePair<string, int>();
            }
                
        }

        private bool _selectedItem;

        public bool SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value; }
        }



        private void OnSelectedJobChanged(KeyValuePair<string, int> value)
        {
            if (GlobalData.SelectedJobsForAnyJob == null)
                GlobalData.SelectedJobsForAnyJob = new ObservableCollection<KeyValuePair<string, int>>();
            if (!GlobalData.SelectedJobsForAnyJob.Contains(value))
            {
                bool addItem = true;
                foreach (var item in GlobalData.SelectedJobsForAnyJob)
                {
                    if (item.Value == value.Value)
                    {
                        addItem = false;
                        break;
                    }
                }
                if (addItem)
                    GlobalData.SelectedJobsForAnyJob.Add(value);
            }
            else
            {
                if (!GlobalData.SelectedJobsForAnyJob.Contains(value))
                    GlobalData.SelectedJobsForAnyJob.Add(value);
                else
                    GlobalData.SelectedJobsForAnyJob.Remove(value);
            }
            ////    GlobalData.SelectedJobsForAnyJob.Remove(value);
            //else
            //    GlobalData.SelectedJobsForAnyJob.Add(value);
        }


        #endregion


        #region methods
        private void SetCommands()
        {
            OK = new RelayCommand(OKExecute);
            Cancel = new RelayCommand(CancelExecute);
        }

        private void OKExecute()
        {
            var window = Application.Current.Windows
            .Cast<Window>()
            .Single(w => w.DataContext == this);
            window.DialogResult = false;
            window.Close();
        }

        private void CancelExecute()
        {
            var window = Application.Current.Windows
            .Cast<Window>()
            .Single(w => w.DataContext == this);
            window.DialogResult = false;
            window.Close();
        }

        private ObservableCollection<KeyValuePair<string, int>> FillPaths()
        {
            ObservableCollection<KeyValuePair<string, int>> result = new ObservableCollection<KeyValuePair<string, int>>();
            foreach (var job in GlobalData.Paths)
            {
                result.Add(job);
            }
            return result;
        }
        #endregion

        #region Commands

        public ICommand OK { get; set; }
        public ICommand Cancel { get; set; }

        #endregion
    }
}
