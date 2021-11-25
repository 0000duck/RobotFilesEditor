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
    public class CreateUserNumsViewModel : ViewModelBase
    {
        #region fields
        private int? Id;
        #endregion

        #region ctor
        public CreateUserNumsViewModel(int? id, int jobNr = 0)
        {
            Id = id;
            GlobalData.SelectedJobsForAnyJob = new ObservableCollection<KeyValuePair<string, int>>();
            Paths = FillPaths(jobNr);
            if (jobNr == 0)
                Description = "Select jobs for USERNUM";
            else
                Description = "Select paths for job " + jobNr;
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

        private string _description;

        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    RaisePropertyChanged(() => Description);
                }
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
            if (GlobalData.SelectedUserNums == null)
                GlobalData.SelectedUserNums = new ObservableCollection<KeyValuePair<string, int>>();
            if (!GlobalData.SelectedUserNums.Contains(value))
            {
                GlobalData.SelectedUserNums.Add(value);
            }
            else
            {
                if (!GlobalData.SelectedUserNums.Contains(value))
                    GlobalData.SelectedUserNums.Add(value);
                else
                    GlobalData.SelectedUserNums.Remove(value);
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

        private ObservableCollection<KeyValuePair<string, int>> FillPaths(int jobNr = 0)
        {
            ObservableCollection<KeyValuePair<string, int>> result = new ObservableCollection<KeyValuePair<string, int>>();
            foreach (var job in GlobalData.Paths)
            {
                if (jobNr != 0)
                {
                    if (job.Value == jobNr)
                        result.Add(job);
                }
                else
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
