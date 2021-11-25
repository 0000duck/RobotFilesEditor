using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RobotFilesEditor.Dialogs
{
    public class SelectLoadVarViewModel
    {
        public SelectLoadVarViewModel(KeyValuePair<int,List<string>> loadVar)
        {
            InputLoadVar = loadVar;
            LoadVarNum = loadVar.Key.ToString();
            InputLoadVarNames = loadVar.Value;
            SetCommands();
        }

        public ICommand CloseCommand { get; set; }

        private void SetCommands()
        {
            CloseCommand = new RelayCommand(OKExecute);
        }

        private void OKExecute()
        {
            var window = Application.Current.Windows
            .Cast<Window>()
            .Single(w => w.DataContext == this);
                        window.DialogResult = true;
                        window.Close();
        }

        private List<string> inputLoadVarNames;
        public List<string> InputLoadVarNames
        {
            get { return inputLoadVarNames; }
            set { inputLoadVarNames = value; }
        }

        private KeyValuePair<int, List<string>> inputLoadVar;
        public KeyValuePair<int, List<string>> InputLoadVar
        {
            get { return inputLoadVar; }
            set { inputLoadVar = value; }
        }

        private int selectedIndex;
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { selectedIndex = value; }
        }

        private string loadVarNum;
        public string LoadVarNum
        {
            get { return loadVarNum; }
            set { loadVarNum = value; }
        }

        
    }
}
