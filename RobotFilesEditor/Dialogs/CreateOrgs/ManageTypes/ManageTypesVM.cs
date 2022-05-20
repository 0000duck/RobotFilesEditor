using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RobotFilesEditor.Dialogs.CreateOrgs.ManageTypes
{
    public class ManageTypesVM : ViewModelBase
    {
        #region fields
        List<ManageTypesData> linesAndTypes;
        public enum ListSelection { Lines, Types, PLCs, LinesRename, TypesRename, PLCRename, None}
        enum GuiState { OnlyTop, SelectionWithoutTypes, SelectionWithTypes }
        public ListSelection lastChange;
        #endregion

        #region ctor
        public ManageTypesVM(List<ManageTypesData> linesAndTypes)
        {
            Messenger.Default.Register<List<ManageTypesData>>(this, "importXMLOrgsFeedback", message => ImportXMLOrgsFeedback(message));
            ErrorText = string.Empty;
            TypeNumsAvailable = new ObservableCollection<int>();
            this.linesAndTypes = linesAndTypes;
            AvailableLines = FillListView<string>(ListSelection.Lines);
            AvailableTypes = FillDataGridView(ListSelection.Types);
            PLCsNumbers = FillListView<int>(ListSelection.PLCs);
            SetGui(GuiState.OnlyTop);
            SetCommands();
            CheckRemoveAndRenameEnabled();
        }
        #endregion


        #region properties
        private ObservableCollection<int> typeNumsAvailable;
        public ObservableCollection<int> TypeNumsAvailable
        {
            get { return typeNumsAvailable; }
            set
            {
                for (int i = 1; i <= 64; i++)
                    value.Add(i);
                Set(ref typeNumsAvailable, value);
            }
        }

        private ObservableCollection<string> availableLines;
        public ObservableCollection<string> AvailableLines
        {
            get { return availableLines; }
            set
            {
                Set(ref availableLines, value);
            }
        }

        private ObservableCollection<TypeAndNum> availableTypes;
        public ObservableCollection<TypeAndNum> AvailableTypes
        {
            get { return availableTypes; }
            set
            {
                Set(ref availableTypes, value);
            }
        }

        private ObservableCollection<int> pLCsNumbers;
        public ObservableCollection<int> PLCsNumbers
        {
            get { return pLCsNumbers; }
            set
            {
                Set(ref pLCsNumbers, value);
            }
        }

        private int selectedLineIndex;
        public int SelectedLineIndex
        {
            get { return selectedLineIndex; }
            set
            {
                if (value > -1)
                    Set(ref selectedLineIndex, value);
                AvailableTypes = FillDataGridView(ListSelection.Types);
                PLCsNumbers = FillListView<int>(ListSelection.PLCs);
                CheckRemoveAndRenameEnabled();
            }
        }

        private int selectedTypeIndex;
        public int SelectedTypeIndex
        {
            get { return selectedTypeIndex; }
            set
            {
                Set(ref selectedTypeIndex, value);
                CheckRemoveAndRenameEnabled();
            }
        }

        private int selectedPLCIndex;
        public int SelectedPLCIndex
        {
            get { return selectedPLCIndex; }
            set
            {
                Set(ref selectedPLCIndex, value);
                CheckRemoveAndRenameEnabled();
            }
        }

        private int selectedTypeNum;
        public int SelectedTypeNum
        {
            get { return selectedTypeNum; }
            set
            {
                Set(ref selectedTypeNum, value);
            }
        }

        private string newValueText;
        public string NewValueText
        {
            get { return newValueText; }
            set
            {
                Set(ref newValueText, value);
            }
        }

        private string errorText;
        public string ErrorText
        {
            get { return errorText; }
            set
            {
                Set(ref errorText, value);
                if (string.IsNullOrEmpty(value))
                    ErrorVisibility = Visibility.Collapsed;
                else
                    ErrorVisibility = Visibility.Visible;
            }
        }

        private Visibility isChangeActive;
        public Visibility IsChangeActive
        {
            get { return isChangeActive; }
            set
            {
                Set(ref isChangeActive, value);
            }
        }

        private Visibility typeSelectorVisibility;
        public Visibility TypeSelectorVisibility
        {
            get { return typeSelectorVisibility; }
            set
            {
                Set(ref typeSelectorVisibility, value);
            }
        }

        private Visibility errorVisibility;
        public Visibility ErrorVisibility
        {
            get { return errorVisibility; }
            set
            {
                Set(ref errorVisibility, value);
            }
        }

        private bool gUIEnabled;
        public bool GUIEnabled
        {
            get { return gUIEnabled; }
            set
            {
                Set(ref gUIEnabled, value);
            }
        }

        private bool typeSelectionEnabled;
        public bool TypeSelectionEnabled
        {
            get { return typeSelectionEnabled; }
            set
            {
                Set(ref typeSelectionEnabled, value);
            }
        }

        private bool removeLineEnabled;
        public bool RemoveLineEnabled
        {
            get { return removeLineEnabled; }
            set
            {
                Set(ref removeLineEnabled, value);
            }
        }

        private bool removeTypeEnabled;
        public bool RemoveTypeEnabled
        {
            get { return removeTypeEnabled; }
            set
            {
                Set(ref removeTypeEnabled, value);
            }
        }

        private bool removePLCEnabled;
        public bool RemovePLCEnabled
        {
            get { return removePLCEnabled; }
            set
            {
                Set(ref removePLCEnabled, value);
            }
        }

        private bool renameLineEnabled;
        public bool RenameLineEnabled
        {
            get { return renameLineEnabled; }
            set
            {
                Set(ref renameLineEnabled, value);
            }
        }

        private bool renameTypeEnabled;
        public bool RenameTypeEnabled
        {
            get { return renameTypeEnabled; }
            set
            {
                Set(ref renameTypeEnabled, value);
            }
        }

        private bool renamePLCEnabled;
        public bool RenamePLCEnabled
        {
            get { return renamePLCEnabled; }
            set
            {
                Set(ref renamePLCEnabled, value);
            }
        }
        #endregion

        #region commands
        public ICommand AddLine { get; set; }
        public ICommand RemoveLine { get; set; }
        public ICommand RenameLine { get; set; }
        public ICommand AddType { get; set; }
        public ICommand RemoveType { get; set; }
        public ICommand RenameType { get; set; }
        public ICommand AddPLC { get; set; }
        public ICommand RemovePLC { get; set; }
        public ICommand RenamePLC { get; set; }
        public ICommand ApplyExecute { get; set; }
        public ICommand CancelExecute { get; set; }
        public ICommand ImportXMLExecute { get; set; }
        public ICommand ExportXMLExecute { get; set; }
        public ICommand ChangeOk { get; set; }
        public ICommand ChangeCancel { get; set; }
        #endregion

        #region methods

        private void SetCommands()
        {
            AddLine = new RelayCommand(AddLineExecute);
            RemoveLine = new RelayCommand(RemoveLineExecute);
            AddType = new RelayCommand(AddTypeExecute);
            RemoveType = new RelayCommand(RemoveTypeExecute);
            AddPLC = new RelayCommand(AddPLCExecute);
            RemovePLC = new RelayCommand(RemovePLCExecute);
            ApplyExecute = new RelayCommand(ApplyExecuteExecute);
            CancelExecute = new RelayCommand(CancelExecuteExecute);
            ImportXMLExecute = new RelayCommand(ImportXMLExecuteExecute);
            ExportXMLExecute = new RelayCommand(ExportXMLExecuteExecute);
            ChangeOk = new RelayCommand(ChangeOkExecute);
            ChangeCancel = new RelayCommand(ChangeCancelExecute);
            RenameLine = new RelayCommand(RenameLineExecute);
            RenameType = new RelayCommand(RenameTypeExecute);
            RenamePLC = new RelayCommand(RenamePLCExecute);
        }

        private void ChangeCancelExecute()
        {
            SetGui(GuiState.OnlyTop);
        }

        private void RenamePLCExecute()
        {
            SetGui(GuiState.SelectionWithoutTypes);
            lastChange = ListSelection.PLCRename;
            NewValueText = linesAndTypes[SelectedLineIndex].PLCs[SelectedPLCIndex].ToString();
        }

        private void RenameTypeExecute()
        {
            TypeSelectionEnabled = false;
            SetGui(GuiState.SelectionWithTypes);
            lastChange = ListSelection.TypesRename;
            SelectedTypeNum = linesAndTypes[SelectedLineIndex].Types[SelectedTypeIndex].Number;
            NewValueText = linesAndTypes[SelectedLineIndex].Types[SelectedTypeIndex].Description;
        }

        private void RenameLineExecute()
        {
            SetGui(GuiState.SelectionWithoutTypes);
            lastChange = ListSelection.LinesRename;
            NewValueText = linesAndTypes[SelectedLineIndex].LineName;
        }

        private void ChangeOkExecute()
        {
            List<ManageTypesData> newLines = new List<ManageTypesData>();
            switch (lastChange)
            {
                case ListSelection.Lines:
                    {
                        if (ValidationOK())
                        {
                            newLines = new List<ManageTypesData>() { new ManageTypesData(NewValueText, new List<TypeAndNum>() { new TypeAndNum(1, "TempDescription") }, new List<int>() { 1 }) };
                            linesAndTypes.ForEach(x => newLines.Add(x));
                        }
                        else
                            return;
                        break;
                    }
                case ListSelection.Types: case ListSelection.PLCs:
                    {
                        linesAndTypes.ForEach(x => newLines.Add(x));
                        var currentLine = newLines[SelectedLineIndex];
                        if (lastChange == ListSelection.Types)
                        {
                            if (ValidationOK())
                            {
                                currentLine.Types.Add(new TypeAndNum(SelectedTypeNum, NewValueText));
                                currentLine.Types = currentLine.Types.OrderBy(x => x.Number).ToList();
                            }
                            else
                                return;
                        }
                        else
                        {
                            if (ValidationOK())
                            {
                                currentLine.PLCs.Add(int.Parse(NewValueText));
                                currentLine.PLCs = currentLine.PLCs.OrderBy(x => x).ToList();
                            }
                            else
                                return;

                        }
                        break;
                    }
                case ListSelection.LinesRename:
                    {
                        linesAndTypes.ForEach(x => newLines.Add(x));
                        var currentLine = newLines[SelectedLineIndex];
                        if (ValidationOK())
                            currentLine.LineName = NewValueText;
                        else
                            return;
                        break;
                    }
                case ListSelection.TypesRename: case ListSelection.PLCRename:
                    {
                        linesAndTypes.ForEach(x => newLines.Add(x));
                        if (lastChange == ListSelection.TypesRename)
                        {
                            if (ValidationOK())
                                newLines[SelectedLineIndex].Types[SelectedTypeIndex] = new TypeAndNum(SelectedTypeNum, NewValueText);
                            else
                                return;
                        }
                        else
                        {
                            if (ValidationOK())
                            {
                                newLines[SelectedLineIndex].PLCs[SelectedPLCIndex] = int.Parse(NewValueText);
                                newLines[SelectedLineIndex].PLCs = newLines[SelectedLineIndex].PLCs.OrderBy(x => x).ToList();
                            }
                            else
                                return;
                        }
                        break;
                    }

            }
            SetGui(GuiState.OnlyTop);
            linesAndTypes = newLines;
            FillAll();
            lastChange = ListSelection.None;
           
        }

        private bool ValidationOK()
        {
            ErrorText = string.Empty;
            switch (lastChange)
            {
                case ListSelection.Lines:
                    {
                        if (linesAndTypes.Any(x => x.LineName.ToLower() == NewValueText.ToLower()))
                        {
                            ErrorText = "Line already exists!";
                            return false;
                        }
                        return true;
                    }
                case ListSelection.Types:
                    {
                        if (SelectedTypeNum < 1)
                        {
                            ErrorText = "Type number invalid!";
                            return false;
                        }
                        if (string.IsNullOrEmpty(NewValueText))
                        {
                            ErrorText = "Type description invalid!";
                            return false;
                        }
                        if (linesAndTypes[SelectedLineIndex].Types.Any(x => x.Number == SelectedTypeNum))
                        {
                            ErrorText = "Type number already exists!";
                            return false;
                        }
                        if (AvailableTypes.Any(x => x.Description.ToLower() == NewValueText.ToLower()))
                        {
                            ErrorText = "Type with this description exists!";
                            return false;
                        }
                        return true;
                    }
                case ListSelection.PLCs:
                    {
                        if (linesAndTypes[SelectedLineIndex].PLCs.Any(x => x == int.Parse(NewValueText)))
                        {
                            ErrorText = "PLC number already exists!";
                            return false;
                        }
                        return true;
                    }
                case ListSelection.LinesRename:
                    {
                        if (AvailableLines[SelectedLineIndex].ToLower() != NewValueText.ToLower() && linesAndTypes.Any(x => x.LineName.ToLower() == NewValueText.ToLower()))
                        {
                            ErrorText = "Line already exists!";
                            return false;
                        }
                        return true;
                    }
                case ListSelection.TypesRename:
                    {
                        if (AvailableTypes[SelectedTypeIndex].Description.ToLower() != NewValueText.ToLower() && AvailableTypes.Any(x => x.Description.ToLower() == NewValueText.ToLower()))
                        {
                            ErrorText = "Type with this description exists!";
                            return false;
                        }
                        return true;
                    }
                case ListSelection.PLCRename:
                    {
                        if (PLCsNumbers[SelectedPLCIndex] != int.Parse(NewValueText) && PLCsNumbers.Any(x => x == int.Parse(NewValueText)))
                        {
                            ErrorText = "PLC number already exists!";
                            return false;
                        }
                        return true;
                    }
                default:
                    return true;
            }
                    
        }

        private void FillAll()
        {
            AvailableLines = FillListView<string>(ListSelection.Lines);
            AvailableTypes = FillDataGridView(ListSelection.Types);
            PLCsNumbers = FillListView<int>(ListSelection.PLCs);
            CheckRemoveAndRenameEnabled();
        }

        private void ExportXMLExecuteExecute()
        {
            Messenger.Default.Send(linesAndTypes, "exportXMLOrgs");
        }

        private void ImportXMLExecuteExecute()
        {
            Messenger.Default.Send("", "importXMLOrgs");
        }

        private void CancelExecuteExecute()
        {
            var window = Application.Current.Windows.Cast<Window>().Single(x => x.DataContext == this);
            window.Close();
        }

        private void ApplyExecuteExecute()
        {
            Messenger.Default.Send(linesAndTypes, "applyXMLOrgs");
            var createOrgsVM = Application.Current.Windows.Cast<Window>().Single(x=> x.GetType().Name == "CreateOrgs").DataContext;
            (createOrgsVM as CreateOrgsViewModel).UpdateLines();
            var window = Application.Current.Windows.Cast<Window>().Single(x => x.DataContext == this);
            window.Close();
        }

        private void RemovePLCExecute()
        {
            List<ManageTypesData> newList = new List<ManageTypesData>();
            linesAndTypes.ForEach(x => newList.Add(x));
            newList[SelectedLineIndex].PLCs.RemoveAt(SelectedPLCIndex);
            linesAndTypes = newList;
            FillAll();
        }

        private void AddPLCExecute()
        {
            lastChange = ListSelection.PLCs;
            SetGui(GuiState.SelectionWithoutTypes);
        }

        private void RemoveTypeExecute()
        {
            List<ManageTypesData> newList = new List<ManageTypesData>();
            linesAndTypes.ForEach(x => newList.Add(x));
            newList[SelectedLineIndex].Types.RemoveAt(SelectedTypeIndex);
            linesAndTypes = newList;
            FillAll();
        }

        private void AddTypeExecute()
        {
            TypeSelectionEnabled = true;
            lastChange = ListSelection.Types;
            SetGui(GuiState.SelectionWithTypes);
        }

        private void RemoveLineExecute()
        {
            List<ManageTypesData> newLines = new List<ManageTypesData>();
            linesAndTypes.ForEach(x => newLines.Add(x));
            newLines.RemoveAt(SelectedLineIndex);
            linesAndTypes = newLines;
            SelectedLineIndex = 0;
            FillAll();
        }

        private void AddLineExecute()
        {
            lastChange = ListSelection.Lines;
            SetGui(GuiState.SelectionWithoutTypes);
        }

        private void SetGui(GuiState state)
        {
            ErrorText = string.Empty;
            switch (state)
            {
                case GuiState.OnlyTop:
                    {
                        IsChangeActive = Visibility.Collapsed;
                        GUIEnabled = true;
                        break;
                    }
                case GuiState.SelectionWithoutTypes:
                    {
                        NewValueText = string.Empty;
                        IsChangeActive = Visibility.Visible;
                        GUIEnabled = false;
                        TypeSelectorVisibility = Visibility.Collapsed;
                        break;
                    }
                case GuiState.SelectionWithTypes:
                    {
                        NewValueText = string.Empty;
                        IsChangeActive = Visibility.Visible;
                        GUIEnabled = false;
                        TypeSelectorVisibility = Visibility.Visible;
                        break;
                    }
            }
        }

        private ObservableCollection<T> FillListView<T>(ListSelection lineSelect)
        {
            List<T> currentList = new List<T>();
            switch (lineSelect)
            {
                case ListSelection.Lines:
                    {
                        linesAndTypes.ForEach(x => (currentList as List<string>).Add(x.LineName));
                        break;
                    }
                case ListSelection.PLCs:
                    {
                        if (SelectedLineIndex >= 0)
                            linesAndTypes[SelectedLineIndex].PLCs.ForEach(x => (currentList as List<int>).Add(x));
                        break;
                    }
                default:
                    {
                        throw new NotSupportedException();
                    }
            }
            ObservableCollection<T> result = CommonLibrary.CommonMethods.ToObservableCollection(currentList);
            return result;
        }

        private ObservableCollection<TypeAndNum> FillDataGridView(ListSelection lineSelect)
        {
            List<TypeAndNum> currentList = new List<TypeAndNum>();
            switch (lineSelect)
            {
                case ListSelection.Types:
                    {
                        if (SelectedLineIndex >= 0)
                            currentList = linesAndTypes[SelectedLineIndex].Types;
                        break;
                    }
            }
            ObservableCollection<TypeAndNum> result = CommonLibrary.CommonMethods.ToObservableCollection(currentList);
            return result;
        }

        private void CheckRemoveAndRenameEnabled()
        {
            RemoveLineEnabled = AvailableLines.Count > 1 && SelectedLineIndex > -1 ? true : false;
            RenameLineEnabled = SelectedLineIndex > -1 ? true : false;
            RemoveTypeEnabled = AvailableTypes.Count > 1 && SelectedTypeIndex > -1 ? true : false;
            RenameTypeEnabled = SelectedTypeIndex > -1 ? true : false;
            RemovePLCEnabled = PLCsNumbers.Count > 1 && SelectedPLCIndex > -1 ? true : false;
            RenamePLCEnabled = SelectedPLCIndex > -1 ? true : false;
        }


        private void ImportXMLOrgsFeedback(List<ManageTypesData> message)
        {
            linesAndTypes = message;
            FillAll();
        }
        #endregion
    }
}
