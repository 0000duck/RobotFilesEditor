using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RobotFilesEditor.Dialogs.RenamePointDialog
{
    public class RenamePointDialogVM : ViewModelBase
    {
        #region ctor
        public RenamePointDialogVM(string name, int maxLength, GlobalData.RenameWindowType type, List<string> alreadyAdded, bool canContainDash)
        {
            _canContainDash = canContainDash;
            maxLengthField = maxLength;
            alreadyAddedField = alreadyAdded;
            InputName = name;
            OutputName = name;         
            LabelText1 = "Please rename " + (type == GlobalData.RenameWindowType.Path ? "path" : "point") + ":";
            SetCommands();
        }
        #endregion

        #region fields
        int maxLengthField;
        List<string> alreadyAddedField;
        bool _canContainDash;
        Regex startWithLetterRegex = new Regex(@"^\s*[a-zA-Z]", RegexOptions.IgnoreCase);
        Regex containsSpecialsigns = new Regex("[\\?\\,\\.\\$\\#\\!\\@\\%\\~\\^\\&\\*\\=\\+\\(\\)\\[\\]\\{\\}\"\\'\\:\\;\\|\\<\\>\\`\\]");
        Regex containsDash = new Regex(@"-");
        #endregion

        #region properties
        private string inputName;
        public string InputName
        {
            get { return inputName; }
            set
            {
                Set(ref inputName,value);
            }
        }

        private string outputName;
        public string OutputName
        {
            get { return outputName; }
            set
            {
                ObservableCollection<RenamePointModel> templist = new ObservableCollection<RenamePointModel>();
                if (value.Length > maxLengthField)
                    templist.Add(new RenamePointModel("Name is too long. Type new name shorter than " + maxLengthField.ToString() + " signs.",true));
                if (!startWithLetterRegex.IsMatch(value))
                    templist.Add(new RenamePointModel("Position name must start with letter!",true));
                if (containsSpecialsigns.IsMatch(value))
                    templist.Add(new RenamePointModel("Name must not contain special signs!", true));
                if (containsDash.IsMatch(value) && !_canContainDash)
                    templist.Add(new RenamePointModel("Name must not contain dash sing!", true));
                if (alreadyAddedField.Any(x => x.ToLower() == value.ToLower()))
                    templist.Add(new RenamePointModel("Position name was already added.",true));
                if (templist.Count == 0)
                {
                    templist.Add(new RenamePointModel("OK", false));
                    OKEnabled = true;
                }
                else
                    OKEnabled = false;
                ProblemSheet = templist;
                Set(ref outputName, value);
            }
        }

        private bool okEnabled;
        public bool OKEnabled
        {
            get { return okEnabled; }
            set
            {
                Set(ref okEnabled,value);
            }
        }

        private string labelText1;
        public string LabelText1
        {
            get { return labelText1; }
            set
            {
                Set(ref labelText1, value);
            }
        }

        private ObservableCollection<RenamePointModel> problemSheet;
        public ObservableCollection<RenamePointModel> ProblemSheet
        {
            get { return problemSheet; }
            set
            {
                Set(ref problemSheet, value);
            }
        }

        #endregion

        #region commands
        public ICommand OkCommand { get; set; }
        #endregion

        #region methods

        private void SetCommands()
        {
            OkCommand = new RelayCommand(OkCommandExecute);
        }

        private void OkCommandExecute()
        {
            var window = Application.Current.Windows.Cast<Window>().Single(w => w.DataContext == this);
            window.DialogResult = true;
            window.Close();
        }
        #endregion
    }
}
