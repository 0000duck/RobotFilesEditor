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
using static RobotFilesEditor.Dialogs.OrgsElementVM;

namespace RobotFilesEditor.Dialogs
{
    public class CreateUserNumsViewModel2 : ViewModelBase   
    {
        #region fields
        private int? Id;
        #endregion

        #region ctor
        public CreateUserNumsViewModel2(int? id, ChooseType chooseType, int jobNr = 0)
        {
            OKEnabled = false;
            Id = id;
            chosenType = chooseType;
            GlobalData.SelectedJobsForAnyJob = new ObservableCollection<KeyValuePair<string, int>>();
            Paths = FillPaths(jobNr);
            cloneOfOriginalPaths = ClonePaths(Paths);
            SelectedPaths = new ObservableCollection<KeyValuePair<string, int>>();
            switch (chooseType)
            {
                case ChooseType.Anyjob:
                {
                    Description = "Select jobs for ANYJOBS";
                    DescrColl1 = "Found jobs";
                    DescrColl2 = "Selected jobs";
                    break;
                }
                case ChooseType.Usernum:
                {
                    if (jobNr == 0)
                        Description = "Select jobs for USERNUM";
                    else
                        Description = "Select paths for job " + jobNr;
                    DescrColl1 = "Found paths";
                    DescrColl2 = "Selected paths";
                    break;
                }
                case ChooseType.Anyjob_UserNum:
                {
                    break;
                }
            }
            SetCommands();
        }

        #endregion

        #region fields
        private ObservableCollection<KeyValuePair<string, int>> cloneOfOriginalPaths;
        private ChooseType chosenType;
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

        private ObservableCollection<KeyValuePair<string, int>> _selectedPaths;
        public ObservableCollection<KeyValuePair<string, int>> SelectedPaths
        {
            get { return _selectedPaths; }
            set
            {
                if (_selectedPaths != value)
                {
                    _selectedPaths = value;
                    RaisePropertyChanged(() => SelectedPaths);
                    CanMoveUpAndDown = SelectedPaths.Count > 1 ? true : false;
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
                //OnSelectedJobChanged(value);
                IsSelectedItem = SelectedJob.Key == null ? false : true;

                //_selectedJob = new KeyValuePair<string, int>();
            }
                
        }

        private KeyValuePair<string, int> _selectedJobInResult;
        public KeyValuePair<string, int> SelectedJobInResult
        {
            get { return _selectedJobInResult; }
            set
            {
                _selectedJobInResult = value;
                RaisePropertyChanged(() => SelectedJobInResult);
                //OnSelectedJobChanged(value);
                IsSelectedItemInResult = SelectedJobInResult.Key == null ? false : true;
                CanMoveUpAndDown = SelectedPaths.Count > 1 && IsSelectedItemInResult ? true : false;
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

        private string _descrColl1;
        public string DescrColl1
        {
            get { return _descrColl1; }
            set
            {
                if (_descrColl1 != value)
                {
                    _descrColl1 = value;
                    RaisePropertyChanged(() => DescrColl1);
                }
            }
        }

        private string _descrColl2;
        public string DescrColl2
        {
            get { return _descrColl2; }
            set
            {
                if (_descrColl2 != value)
                {
                    _descrColl2 = value;
                    RaisePropertyChanged(() => DescrColl2);
                }
            }
        }

        private bool _selectedItem;

        public bool IsSelectedItem
        {
            get { return _selectedItem; }
            set {
                _selectedItem = value;
                RaisePropertyChanged(() => IsSelectedItem);
            }
        }

        private bool _selectedItemInResult;
        public bool IsSelectedItemInResult
        {
            get { return _selectedItemInResult; }
            set
            {
                _selectedItemInResult = value;
                RaisePropertyChanged(() => IsSelectedItemInResult);
            }
        }

        private bool _canMoveUpAndDown;
        public bool CanMoveUpAndDown
        {
            get { return _canMoveUpAndDown; }
            set
            {
                _canMoveUpAndDown = value;
                RaisePropertyChanged(() => CanMoveUpAndDown);
            }
        }

        private bool _oKEnabled;
        public bool OKEnabled
        {
            get { return _oKEnabled; }
            set
            {
                _oKEnabled = value;
                RaisePropertyChanged(() => OKEnabled);
            }
        }


        //private void OnSelectedJobChanged(KeyValuePair<string, int> value)
        //{
        //    if (GlobalData.SelectedUserNums == null)
        //        GlobalData.SelectedUserNums = new ObservableCollection<KeyValuePair<string, int>>();
        //    if (!GlobalData.SelectedUserNums.Contains(value))
        //    {
        //        GlobalData.SelectedUserNums.Add(value);
        //    }
        //    else
        //    {
        //        if (!GlobalData.SelectedUserNums.Contains(value))
        //            GlobalData.SelectedUserNums.Add(value);
        //        else
        //            GlobalData.SelectedUserNums.Remove(value);
        //    }
        //    ////    GlobalData.SelectedJobsForAnyJob.Remove(value);
        //    //else
        //    //    GlobalData.SelectedJobsForAnyJob.Add(value);
        //}


        #endregion


        #region methods
        private void SetCommands()
        {
            OK = new RelayCommand(OKExecute);
            Cancel = new RelayCommand(CancelExecute);
            MoveUp = new RelayCommand(MoveUpExecute);
            MoveDown = new RelayCommand(MoveDownExecute);
            MoveLeft = new RelayCommand(MoveLeftExecute);
            MoveRight = new RelayCommand(MoveRightExecute);
        }

        private void MoveRightExecute()
        {
            SelectedPaths.Add(SelectedJob);
            Paths.Remove(SelectedJob);
            SelectedJob = new KeyValuePair<string, int>();
            CanMoveUpAndDown = SelectedPaths.Count > 1 && IsSelectedItemInResult ? true : false;
            OKEnabled = SelectedPaths.Count > 0 ? true : false;
        }

        private void MoveLeftExecute()
        {
            Paths.Add(SelectedJobInResult);
            SelectedPaths.Remove(SelectedJobInResult);
            SelectedJobInResult = new KeyValuePair<string, int>();
            CanMoveUpAndDown = false;
            OKEnabled = SelectedPaths.Count > 0 ? true : false;
        }

        private void MoveDownExecute()
        {
            if (SelectedPaths[SelectedPaths.Count - 1].Key == SelectedJobInResult.Key)
                return;
            SelectedPaths = MoveItemDown(SelectedJobInResult, SelectedPaths);
        }

        private void MoveUpExecute()
        {
            if (SelectedPaths[0].Key == SelectedJobInResult.Key)
                return;
            var selectedPathsReverse = SelectedPaths.Reverse();
            ObservableCollection<KeyValuePair<string, int>> selectedPathsReverseObsColl = EnumerToObsColl(selectedPathsReverse);
            var movedReversedList = MoveItemDown(SelectedJobInResult, selectedPathsReverseObsColl);
            var movedList = movedReversedList.Reverse();
            SelectedPaths = EnumerToObsColl(movedList);
        }

        private ObservableCollection<KeyValuePair<string, int>> EnumerToObsColl(IEnumerable<KeyValuePair<string, int>> inputPaths)
        {
            ObservableCollection<KeyValuePair<string, int>> result = new ObservableCollection<KeyValuePair<string, int>>();
            inputPaths.ToList().ForEach(x => result.Add(x));
            return result;
        }

        private void OKExecute()
        {
            switch (chosenType)
            {
                case ChooseType.Anyjob:
                    GlobalData.SelectedJobsForAnyJob = SelectedPaths;
                    break;
                case ChooseType.Usernum:
                    GlobalData.SelectedUserNums = SelectedPaths;
                    break;
                case ChooseType.Anyjob_UserNum:
                    break;
            }

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
                {   if (job.Value < 90)
                        result.Add(job);
                }
            }
            return result;
        }

        private ObservableCollection<KeyValuePair<string, int>> ClonePaths(ObservableCollection<KeyValuePair<string, int>> paths)
        {
            ObservableCollection<KeyValuePair<string, int>> result = new ObservableCollection<KeyValuePair<string, int>>();
            paths.ToList().ForEach(x => result.Add(x));
            return result;
        }

        private ObservableCollection<KeyValuePair<string,int>> MoveItemDown(KeyValuePair<string, int> itemToMove, ObservableCollection<KeyValuePair<string, int>> inputElements)
        {
            bool skipNext = false;
            KeyValuePair<string, int> memSelect = itemToMove;
            ObservableCollection<KeyValuePair<string, int>> sortedCollection = new ObservableCollection<KeyValuePair<string, int>>();
            foreach (var item in inputElements)
            {
                if (item.Key == itemToMove.Key)
                {
                    skipNext = true;
                }
                else
                {
                    sortedCollection.Add(item);
                    if (skipNext)
                    {
                        skipNext = false;
                        sortedCollection.Add(memSelect);
                    }
                }
            }
            return sortedCollection;
        }
        #endregion

        #region Commands

        public ICommand OK { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand MoveUp { get; set; }
        public ICommand MoveDown { get; set; }
        public ICommand MoveLeft { get; set; }
        public ICommand MoveRight { get; set; }
        #endregion
    }
}
