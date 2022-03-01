using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using RobotFilesEditor.Dialogs.CompareSOVAndOLP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RobotFilesEditor.Model.Operations.DataClass
{
    public class BackupToOLPComparerDataSet : ViewModelBase
    {
        #region fields
        ObservableCollection<CompareData> data;
        #endregion

        #region commands
        public ICommand CompareCommand { get; set; }

        private void CompareCommandExecute()
        {
            CompareOLPToSOVLineByLineViewModel vm = new CompareOLPToSOVLineByLineViewModel(data);
            var window = new CompareOLPToSOVLineByLine(vm);
            var windowResult = window.ShowDialog();
        }
        #endregion

        #region properties
        public string RadioButtonGroupName { get; set; }
        public string FileInSet1 { get; set; }
        public string FileInSet2 { get; set; }
        public string ContentSet1 { get; set; }
        public string ContentSet2 { get; set; }
        public bool IsSame { get; set; }
        public bool EnableButtons { get; set; }
        public bool EnableRBOLP { get; set; }
        public bool EnableRBBackup { get; set; }

        private bool useOLP;
        public bool UseOLP
        {
            get { return useOLP; }
            set
            {
                if (useOLP != value)
                {
                    Set(ref useOLP, value);
                    Messenger.Default.Send("", "updateOkEnable");
                }
            }
        }

        private bool useBackup;
        public bool UseBackup
        {
            get { return useBackup; }
            set
            {
                if (useBackup != value)
                    Set(ref useBackup, value);
                Messenger.Default.Send("", "updateOkEnable");
            }
        }

        private bool notSelected;
        public bool NotSelected
        {
            get { return notSelected; }
            set
            {
                if (notSelected != value)
                    Set(ref notSelected, value);
            }
        }

        private int compareStatus;
        public int CompareStatus
        {
            get { return compareStatus; }
            set
            {
                Set(ref compareStatus, value);
                switch(value)
                {
                    case 1:
                        CompStatusMsg = "No difference"; break;
                    case 2:
                        CompStatusMsg = "Difference in program"; break;
                    case 3:
                        CompStatusMsg = "Difference in comments"; break;
                }
            }
        }

        private string cmpStatuMsg;
        public string CompStatusMsg
        {
            get { return cmpStatuMsg; }
            set
            {
                Set(ref cmpStatuMsg, value);
            }
        }

        #endregion

        #region ctor
        public BackupToOLPComparerDataSet(string fileInSet1, string fileInSet2, string contentSet1, string contentSet2)
        {
            CompareStatus = PrepareDataToShow(contentSet1, contentSet2);
            RadioButtonGroupName = fileInSet1.Length > 0 ? fileInSet1 : fileInSet2;
            SetCommands();
            FileInSet1 = fileInSet1;
            FileInSet2 = fileInSet2;
            ContentSet1 = contentSet1;
            ContentSet2 = contentSet2;
            IsSame = contentSet1 == contentSet2 ? true : false;
            EnableButtons = !IsSame;
            NotSelected = true;
            SetRadioButtons(fileInSet1, fileInSet2);
        }
        #endregion

        #region methods
        private void SetCommands()
        {
            CompareCommand = new RelayCommand(CompareCommandExecute);
        }

        private void SetRadioButtons(string fileInSet1, string fileInSet2)
        {
            if (IsSame)
            {
                EnableRBOLP = false;
                EnableRBBackup = false;
                UseBackup = false;
                UseOLP = false;
            }
            else if (!string.IsNullOrEmpty(fileInSet1.Trim()) && !string.IsNullOrEmpty(fileInSet2.Trim()))
            {
                EnableRBBackup = true;
                EnableRBOLP = true;
            }
            else if (!string.IsNullOrEmpty(fileInSet1.Trim()) && string.IsNullOrEmpty(fileInSet2.Trim()))
            {
                EnableRBBackup = true;
                UseBackup = true;
                EnableRBOLP = false;
            }
            else if (string.IsNullOrEmpty(fileInSet1.Trim()) && !string.IsNullOrEmpty(fileInSet2.Trim()))
            {
                EnableRBOLP = true;
                UseOLP = true;
                EnableRBBackup = false;
            }
            else
            {
                EnableRBOLP = false;
                EnableRBBackup = false;
                UseBackup = false;
                UseOLP = false;
            }
        }

        private int PrepareDataToShow(string fileInSet1, string fileInSet2)
        {
            int result = 1;
            Regex removeLineRegex = new Regex(@"(?<=^\s*\d+\s*:).*$", RegexOptions.IgnoreCase);
            Regex headerEndRegex = new Regex(@"^\s*/MN\s*$", RegexOptions.IgnoreCase); 
            data = new ObservableCollection<CompareData>();
            List<string> file1Lines = DivideToLines(fileInSet1);
            List<string> file2Lines = DivideToLines(fileInSet2);
            bool addStepsPossible = true, headerFinished1 = false, headerFinished2 = false;
            int iterations = file1Lines.Count > file2Lines.Count ? file1Lines.Count : file2Lines.Count;

            for (int i = 0; i < iterations; i++)
            {
                if (file1Lines.Count - 1 >= i && file2Lines.Count - 1 >= i)
                {
                    if (headerEndRegex.IsMatch(file1Lines[i]))
                        headerFinished1 = true;
                    if (headerEndRegex.IsMatch(file2Lines[i]))
                        headerFinished2 = true;
                    if (file1Lines[i] != file2Lines[i])
                    {
                        //if (isCommentOrEmptyLine.IsMatch(file1Lines[i]) && isCommentOrEmptyLine.IsMatch(file2Lines[i]))
                        if (CheckEmptyOrComment(file1Lines[i]) && CheckEmptyOrComment(file2Lines[i]))
                        {
                            if (removeLineRegex.IsMatch(file1Lines[i]) && removeLineRegex.IsMatch(file2Lines[i]) && removeLineRegex.Match(file1Lines[i]).ToString() == removeLineRegex.Match(file2Lines[i]).ToString())
                            {
                                data.Add(new CompareData(file1Lines[i], file2Lines[i], 1));
                            }
                            else
                            {
                                data.Add(new CompareData(file1Lines[i], file2Lines[i], 3));
                                if (result == 1)
                                    result = 3;
                            }
                        }
                        else if (CheckEmptyOrComment(file1Lines[i]) && !CheckEmptyOrComment(file2Lines[i]))
                        {
                            data.Add(new CompareData(file1Lines[i], string.Empty, 3));
                            if (result == 1)
                                result = 3;
                            if (addStepsPossible)
                                file2Lines.Insert(i, string.Empty);
                        }
                        else if (CheckEmptyOrComment(file2Lines[i]) && !CheckEmptyOrComment(file1Lines[i]))
                        {
                            data.Add(new CompareData(string.Empty, file2Lines[i], 3));
                            if (result == 1)
                                result = 3;
                            if (addStepsPossible)
                                file1Lines.Insert(i, string.Empty);
                        }
                        else
                        {
                            if (file1Lines.Count - 1 >= i && file2Lines.Count - 1 >= i)
                            {
                                if (removeLineRegex.IsMatch(file1Lines[i]) && removeLineRegex.IsMatch(file2Lines[i]) && removeLineRegex.Match(file1Lines[i]).ToString().Trim() == removeLineRegex.Match(file2Lines[i]).ToString().Trim())
                                {
                                    data.Add(new CompareData(file1Lines[i], file2Lines[i], 1));
                                }
                                else
                                {
                                    if (!headerFinished1 && !headerFinished2)
                                    {
                                        data.Add(new CompareData(file1Lines[i], file2Lines[i], 3));
                                        result = 3;
                                    }
                                    else
                                    {
                                        data.Add(new CompareData(file1Lines[i], file2Lines[i], 2));
                                        result = 2;
                                    }
                                }
                            }
                            else if (file1Lines.Count - 1 < i && file2Lines.Count - 1 >= i)
                            {
                                data.Add(new CompareData(string.Empty, file2Lines[i], 2));
                                result = 2;
                            }
                            else if (file1Lines.Count - 1 >= i && file2Lines.Count - 1 < i)
                            {
                                data.Add(new CompareData(file1Lines[i], string.Empty, 2));
                                result = 2;
                            }
                        }
                    }
                    else
                        data.Add(new CompareData(file1Lines[i], file2Lines[i], 1));
                }

                //if (file1Lines.Count == file2Lines.Count)
                //    addStepsPossible = false;
            }
            return result;
        }

        private bool CheckEmptyOrComment(string v)
        {
            Regex isCommentOrEmptyLine = new Regex(@"^\s*\d+\s*\:\s*(!|;)", RegexOptions.IgnoreCase);
            if (isCommentOrEmptyLine.IsMatch(v) || string.IsNullOrEmpty(v.Trim()))
                return true;
            return false;
        }

        //private bool Check(string v)
        //{
        //    Regex isCommentOrEmptyLine = new Regex(@"^\s*\d+\s*\:\s*(!|;)", RegexOptions.IgnoreCase);
        //    if (isCommentOrEmptyLine.IsMatch(v) || string.IsNullOrEmpty(v.Trim()))
        //        return true;
        //    return false;
        //}

        private List<string> DivideToLines(string file)
        {
            List<string> result = new List<string>();
            StringReader reader = new StringReader(file);
            while (true)
            {
                string line = reader.ReadLine();
                if (line == null)
                    break;
                result.Add(line);
            }
            reader.Close();
            return result;
        }

        #endregion
    }

}
