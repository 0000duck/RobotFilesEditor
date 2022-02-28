using RobotFilesEditor.Model.Operations.ABB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.ComponentModel;
using System.Windows;

namespace RobotFilesEditor.Dialogs.ABBRename
{
    
    public class ABBRenameViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private List<string> modifiedList;


        public ABBRenameViewModel(Dictionary<string, ABBModule> foundModules)
        {
            FoundModules = foundModules;
            PointNameEnabled = false;
            selectedPointsList = new List<string>();
            //FoundProcedures = new Dictionary<string, List<string>>();
            CheckApplyEnabled();
            CheckApply2Enabled();
        }

        private List<string> selectedPointsList;
        public List<string> SelectedPointsList
        {
            get { return selectedPointsList; }
            set
            {
                selectedPointsList = value;
                RaisePropertyChanged(()=>SelectedPointsList);
                SinglePointInput = string.Empty;
                CheckSinglePointInputEnabled();
            }
        }


        private Dictionary<string,ABBModule> _foundModules;
        public Dictionary<string,ABBModule> FoundModules
        {
            get { return _foundModules; }
            set
            {
                if (_foundModules != value)
                {
                    _foundModules = value;
                    RaisePropertyChanged(() => FoundModules);
                }
            }
        }

        private string _selectedModule;
        public string SelectedModule
        {
            get { return _selectedModule; }
            set
            {
                if (value != _selectedModule)
                {
                    _selectedModule = value;
                    RaisePropertyChanged(() => SelectedModule);
                    FoundProcedures = FoundModules[value].ProceduresWithRobtargets;
                    SelectedPointName = null;
                    SelectedProcedure = null;
                    FoundPoints = null;
                    PointNameEnabled = true;
                }
            }
        }

        private Dictionary<string, List<string>> _foundProcedures;
        public Dictionary<string, List<string>> FoundProcedures
        {
            get { return _foundProcedures; }
            set
            {
                if (value != _foundProcedures)
                {
                    _foundProcedures = value;
                    RaisePropertyChanged(() => FoundProcedures);
                }
            }
        }

        private string _selectedProcedure;
        public string SelectedProcedure
        {
            get { return _selectedProcedure; }
            set
            {
                if (value != _selectedProcedure)
                {
                    _selectedProcedure = value;
                    RaisePropertyChanged(() => SelectedProcedure);
                    FoundPoints = FoundModules[SelectedModule].ProceduresWithRobtargets[value];
                    SelectedPointName = null;
                }
            }
        }

        private List<string> _foundPoints;
        public List<string> FoundPoints
        {
            get { return _foundPoints; }
            set
            {
                if (value != _foundPoints)
                {
                    _foundPoints = value;
                    RaisePropertyChanged(() => FoundPoints);
                }
            }
        }

        //private int _selecterPointIndex;
        //public int SelectedPointIndex
        //{
        //    get { return _selecterPointIndex; }
        //    set
        //    {
        //        if (value != _selecterPointIndex && value != null)
        //        {
        //            var window = Application.Current.Windows
        //            .Cast<Window>()
        //            .Single(w => w.DataContext == this);
        //            var selectedPoints = (window as ABBRenameWindow).MyListView.SelectedItems;
        //            _selecterPointIndex = value;
        //            SelectedPointName = FoundPoints[value];
        //            if (!string.IsNullOrEmpty(SelectedPointName))
        //                PointNameEnabled = true;
        //            else
        //                PointNameEnabled = false;
        //            RaisePropertyChanged(() => SelectedPointIndex);
        //        }
        //    }        
        //}



        private string _selectedPointName;
        public string SelectedPointName
        {
            get { return _selectedPointName; }
            set
            {
                if (value != _selectedPointName)
                {
                    RaisePropertyChanged(() => SelectedPointName);
                    SelectedPointName = null;
                }
            }
        }

        private bool _pointNameEnabled;
        public bool PointNameEnabled
        {
            get { return _pointNameEnabled; }
            set {
                if (value != _pointNameEnabled)
                {
                    _pointNameEnabled = value;
                    RaisePropertyChanged(() => PointNameEnabled);
                } }
        }

        private bool _singlePointInputEnabled;
        public bool SinglePointInputEnabled
        {
            get { return _singlePointInputEnabled; }
            set
            {
                if (value != _singlePointInputEnabled)
                {
                    _singlePointInputEnabled = value;
                    RaisePropertyChanged(() => SinglePointInputEnabled);
                }
            }
        }

        private bool _applyEnabled;
        public bool ApplyEnabled
        {
            get { return _applyEnabled; }
            set
            {
                if (value != _applyEnabled)
                {
                    _applyEnabled = value;
                    RaisePropertyChanged(() => ApplyEnabled);
                }
            }
        }

        private bool _applyEnabled2;
        public bool ApplyEnabled2
        {
            get { return _applyEnabled2; }
            set
            {
                if (value != _applyEnabled2)
                {
                    _applyEnabled2 = value;
                    RaisePropertyChanged(() => ApplyEnabled2);
                }
            }
        }

        private string _prefix;
        public string Prefix
        {
            get { return _prefix; }
            set
            {
                if (value != _prefix)
                {
                    _prefix = value;
                    RaisePropertyChanged(() => Prefix);
                    CheckApplyEnabled();
                }
            }
        }

        private string _startNum;
        public string StartNum
        {
            get { return _startNum; }
            set
            {
                if (value != _startNum)
                {
                    _startNum = value;
                    RaisePropertyChanged(() => StartNum);
                    CheckApplyEnabled();
                }
            }
        }

        private string _enumerator;
        public string Enumerator
        {
            get { return _enumerator; }
            set
            {
                if (value != _enumerator)
                {
                    _enumerator = value;
                    RaisePropertyChanged(() => Enumerator);
                    CheckApplyEnabled();
                }
            }
        }
  
        private string _singlePointInput;
        public string SinglePointInput
        {
            get { return _singlePointInput; }
            set
            {
                if (value != _singlePointInput)
                {
                    _singlePointInput = value;
                    RaisePropertyChanged(() => SinglePointInput);
                    CheckApply2Enabled();
                }
            }
        }

        RelayCommand _applyCommand;
        public RelayCommand ApplyCommand
        {
            get
            {
                if (_applyCommand == null)
                {
                    _applyCommand = new RelayCommand(Apply);
                }
                return _applyCommand;
            }
        }

        RelayCommand _apply2Command;
        public RelayCommand Apply2Command
        {
            get
            {
                if (_apply2Command == null)
                {
                    _apply2Command = new RelayCommand(Apply2);
                }
                return _apply2Command;
            }
        }

        RelayCommand _okCommand;
        public RelayCommand OkCommand
        {
            get
            {
                if (_okCommand == null)
                {
                    _okCommand = new RelayCommand(OkExecute);
                }
                return _okCommand;
            }
        }

        RelayCommand _cancelCommand;
        public RelayCommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                {
                    _cancelCommand = new RelayCommand(CancelExecute);
                }
                return _cancelCommand;
            }
        }

        private void ModifyCollection(int mode)
        {
            List<string> tempList = new List<string>();
            switch (mode)
            {
                case 1:
                    {
                        int startNum = int.Parse(StartNum);
                        int enumerator = int.Parse(Enumerator);
                        int counter = 0;

                        foreach (var point in FoundPoints)
                        {
                            if (!SelectedPointsList.Contains(point))
                            {
                                tempList.Add(Prefix + ((startNum + enumerator * counter).ToString()));
                                counter++;

                            }
                            else
                                tempList.Add(point);
                        }
                        break;
                    }
                case 2:
                    {
                        foreach (var point in FoundPoints)
                        {
                            if (SelectedPointsList[0] == point)
                                tempList.Add(SinglePointInput);
                            else
                                tempList.Add(point);
                        }
                        break;
                    }
            }

            modifiedList = tempList;
        }

        //private void ModifyCollection_OLD()
        //{

        //string prefix = string.Empty;
        //bool mainPointFound = false;
        //if (SelectedPointIndex > -1)
        //{
        //    Dictionary<string, string> oldAndNewNameOfPoint = new Dictionary<string, string>();
        //    foreach (var item in FoundPoints)
        //    {
        //        if (!oldAndNewNameOfPoint.Keys.Contains(item))
        //            oldAndNewNameOfPoint.Add(item, string.Empty);
        //    }
        //    int selectedPointIndexMemory = SelectedPointIndex;
        //    List<string> newPointList = new List<string>();
        //    int pointcounter = 0, enumeratorCounter = 10;
        //    var currentPoints = FoundPoints;
        //    foreach (var point in currentPoints)
        //    {
        //        if (pointcounter == SelectedPointIndex || currentPoints[pointcounter].ToLower() == currentPoints[SelectedPointIndex].ToLower())
        //        {
        //            mainPointFound = true;
        //            if (string.IsNullOrEmpty(oldAndNewNameOfPoint[point]))
        //            {
        //                oldAndNewNameOfPoint[point] = SelectedPointName;
        //                newPointList.Add(SelectedPointName);
        //                enumeratorCounter = 10;
        //            }
        //            else
        //                newPointList.Add(oldAndNewNameOfPoint[point]);
        //        }
        //        else
        //        {
        //            if (!mainPointFound)
        //                prefix = "pV";
        //            else
        //                prefix = "pF";
        //            if (string.IsNullOrEmpty(oldAndNewNameOfPoint[point]))
        //            {
        //                oldAndNewNameOfPoint[point] = prefix + SelectedPointName + "_P" + enumeratorCounter;
        //                newPointList.Add(prefix + SelectedPointName + "_P" + enumeratorCounter);
        //                enumeratorCounter = enumeratorCounter + 10;
        //            }
        //            else
        //                newPointList.Add(oldAndNewNameOfPoint[point]);
        //        }
        //        pointcounter++;
        //    }
        //    modifiedList = new List<string>();
        //    FoundPoints = newPointList;
        //    modifiedList = newPointList;
        //    SelectedPointIndex = selectedPointIndexMemory;
        //}
        //}

        private void Apply()
        {
            ModifyCollection(1);
            FoundPoints = modifiedList;
            RaisePropertyChanged(() => FoundPoints);
            FoundModules[SelectedModule].ProceduresWithRobtargets[SelectedProcedure] = modifiedList;
        }

        private void Apply2()
        {
            ModifyCollection(2);
            FoundPoints = modifiedList;
            RaisePropertyChanged(() => FoundPoints);
            FoundModules[SelectedModule].ProceduresWithRobtargets[SelectedProcedure] = modifiedList;
        }

        private void OkExecute()
        {
            ABB_Static_Methods.ExecuteChange(FoundModules);
            CancelExecute();
        }

        private void CancelExecute()
        {
            var window = Application.Current.Windows
            .Cast<Window>()
            .Single(w => w.DataContext == this);
                    window.DialogResult = false;
                    window.Close();
        }

        private void CheckApplyEnabled()
        {
            if (string.IsNullOrEmpty(Prefix) || string.IsNullOrEmpty(Enumerator) || string.IsNullOrEmpty(StartNum) || int.Parse(Enumerator) < 1 || int.Parse(StartNum) < 1)
                ApplyEnabled = false;
            else
                ApplyEnabled = true;
        }

        private void CheckApply2Enabled()
        {
            if (SinglePointInputEnabled && !string.IsNullOrEmpty(SinglePointInput))
                ApplyEnabled2 = true;
            else
                ApplyEnabled2 = false;
        }

            private void CheckSinglePointInputEnabled()
        {
            if (SelectedPointsList.Count == 1)
                SinglePointInputEnabled = true;
            else
                SinglePointInputEnabled = false;
        }

    }
}
