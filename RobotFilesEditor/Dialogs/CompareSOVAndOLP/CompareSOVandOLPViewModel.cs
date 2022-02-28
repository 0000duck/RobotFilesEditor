using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using RobotFilesEditor.Model.Operations.DataClass;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RobotFilesEditor.Dialogs.CompareSOVAndOLP
{
    public class CompareSOVandOLPViewModel : ViewModelBase
    {
        #region ctor
        public CompareSOVandOLPViewModel(IDictionary<string, string> filesSet1, IDictionary<string, string> filesSet2)
        {
            Messenger.Default.Register<string>(this, "updateOkEnable", message => CheckOkEnabled());
            SetCommands();
            PrepareDataSet(filesSet1, filesSet2);
            CheckOkEnabled();
        }

        #endregion

        #region properties
        private ObservableCollection<BackupToOLPComparerDataSet> items;
        public ObservableCollection<BackupToOLPComparerDataSet> Items
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

        private bool oKButtonEnable;

        public bool OKButtonEnable
        {
            get { return oKButtonEnable; }
            set
            {
                Set(ref oKButtonEnable, value);
            }
        }
        #endregion

        #region commands
        public ICommand OKCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand SetAllUseOLP { get; set; }
        public ICommand SetAllUseBackup { get; set; }

        #endregion

        #region methods

        private void PrepareDataSet(IDictionary<string, string> filesSet1, IDictionary<string, string> filesSet2)
        {
            List<string> foundPaths = new List<string>();
            if (Items == null)
                Items = new ObservableCollection<BackupToOLPComparerDataSet>();
            foreach (var file in filesSet1)
            {
                foundPaths.Add(Path.GetFileNameWithoutExtension(file.Key).ToLower());
                if (filesSet2.Any(x => Path.GetFileNameWithoutExtension(x.Key.ToLower()) == Path.GetFileNameWithoutExtension(file.Key).ToLower()))
                {
                    var recordInSet2 = filesSet2.First(x => Path.GetFileNameWithoutExtension(x.Key.ToLower()) == Path.GetFileNameWithoutExtension(file.Key).ToLower());
                    Items.Add(new BackupToOLPComparerDataSet(Path.GetFileNameWithoutExtension(file.Key), recordInSet2.Key, file.Value, recordInSet2.Value));
                }
                else
                {
                    Items.Add(new BackupToOLPComparerDataSet(Path.GetFileNameWithoutExtension(file.Key), string.Empty, file.Value, string.Empty));
                }
            }
            foreach (var file in filesSet2.Where(x => !foundPaths.Contains(Path.GetFileNameWithoutExtension(x.Key.ToLower()))))
            {
                Items.Add(new BackupToOLPComparerDataSet(string.Empty, Path.GetFileNameWithoutExtension(file.Key), string.Empty, file.Value));
            }
            RaisePropertyChanged(() => Items);
        }

        private void SetCommands()
        {
            OKCommand = new RelayCommand(OkCommandExecute);
            CancelCommand = new RelayCommand(CancelCommandExecute);
            SetAllUseOLP = new RelayCommand(SetAllUseOLPExecute);
            SetAllUseBackup = new RelayCommand(SetAllUseBackupExecute);
        }

        private void SetAllUseBackupExecute()
        {
            Items.ToList().Where(y => y.IsSame == false).Where(z=>z.EnableRBBackup).ToList().ForEach(x => x.UseBackup = true);
            RaisePropertyChanged(() => Items);
        }

        private void SetAllUseOLPExecute()
        {
            Items.ToList().Where(y=>y.IsSame == false).Where(z=>z.EnableRBOLP).ToList().ForEach(x => x.UseOLP = true);
            RaisePropertyChanged(() => Items);
        }

        private void CancelCommandExecute()
        {
            var window = Application.Current.Windows
            .Cast<Window>()
            .Single(w => w.DataContext == this);
            window.DialogResult = false;
            window.Close();
        }

        private void OkCommandExecute()
        {
            var window = Application.Current.Windows
            .Cast<Window>()
            .Single(w => w.DataContext == this);
            window.DialogResult = true;
            window.Close();
        }

        private object CheckOkEnabled()
        {
            RaisePropertyChanged(() => Items);
            if (Items.ToList().Where(y=>y.IsSame == false).ToList().Any(x => x.UseBackup == false && x.UseOLP == false))
                OKButtonEnable = false;
            else
                OKButtonEnable = true;
            return null;
        }
        #endregion
    }
}
