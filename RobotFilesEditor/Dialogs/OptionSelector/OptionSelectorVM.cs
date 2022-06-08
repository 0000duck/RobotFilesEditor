using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RobotFilesEditor.Dialogs.OptionSelector
{
    public class OptionSelectorVM : ViewModelBase
    {
        #region ctor
        public OptionSelectorVM(List<string> options, string optionDescription)
        {
            SetCommands();
            OptionType = optionDescription;
            FillValues(options);
        }
        #endregion

        #region properties
        private string optionType;
        public string OptionType
        {
            get { return optionType; }
            set
            {
               Set(ref optionType, value);
            }
        }

        private string result;
        public string Result
        {
            get { return result; }
            set
            {
                Set(ref result, value);
            }
        }

        private List<string> optionsToSelect;
        public List<string> OptionsToSelect
        {
            get { return optionsToSelect; }
            set
            {
               Set (ref optionsToSelect, value);
            }
        }

        private List<Visibility> visibilityEnabled;
        public List<Visibility> VisibilityEnabled
        {
            get { return visibilityEnabled; }
            set
            {
                Set(ref visibilityEnabled, value);
            }
        }

        private List<bool> selected;
        public List<bool> Selected
        {
            get { return selected; }
            set
            {
                Set(ref selected, value);
            }
        }

        #endregion

        #region commands

        public ICommand OKExecute { get; set; }

        #endregion

        #region methods

        private void SetCommands()
        {
            OKExecute = new RelayCommand(OkExecuteExecute);
        }

        private void OkExecuteExecute()
        {
            GetResult();
            var window = Application.Current.Windows.Cast<Window>().Single(w => w.DataContext == this);
            window.DialogResult = true;
            window.Close();
        }

        private void GetResult()
        {
            int counter = 0;
            foreach (var item in Selected)
            {
                if (item)
                {
                    Result = OptionsToSelect[counter];
                    break;
                }
                counter++;
            }
        }

        private void FillValues(List<string> options)
        {
            var optionsToSelect = new List<string>();
            var visibilityEnabled = new List<Visibility>();
            var selected = new List<bool>();
            for (int i = 0; i <=7; i++)
            {
                selected.Add(false);
                if (options.Count >= (i+1))
                {
                    optionsToSelect.Add(options[i]);
                    visibilityEnabled.Add(Visibility.Visible);
                }
                else
                {
                    optionsToSelect.Add(string.Empty);
                    visibilityEnabled.Add(Visibility.Collapsed);
                }
            }
            OptionsToSelect = optionsToSelect;
            VisibilityEnabled = visibilityEnabled;
            Selected = selected;
        }
        #endregion
    }
}
