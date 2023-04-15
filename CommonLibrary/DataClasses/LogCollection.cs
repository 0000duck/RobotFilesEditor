using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CommonLibrary.DataClasses
{
    public class LogCollection : INotifyPropertyChanged
    {

        #region constructor
        public LogCollection()
        {
            Entries = new ObservableCollection<LogResult>();
            OKsFilterChecked = true;
            WarningsFilterChecked = true;
            ErrorsFilterChecked = true;
            InfoFilterChecked = true;
          
        }
        #endregion constructor


        #region properties
        public ObservableCollection<LogResult> Entries { get { return m_Entries; } set { m_Entries = value; OnPropertyChanged(nameof(Entries)); } }
        private ObservableCollection<LogResult> m_Entries;

        public string OkContent { get { return m_OkContent; } set { m_OkContent =  value; OnPropertyChanged(nameof(OkContent)); } }
        private string m_OkContent;

        public string WarningContent { get { return m_WarningContent; } set { m_WarningContent = value; OnPropertyChanged(nameof(WarningContent)); } }
        private string m_WarningContent;

        public string ErrorContent { get { return m_ErrorContent; } set { m_ErrorContent = value; OnPropertyChanged(nameof(ErrorContent)); } }
        private string m_ErrorContent;

        public string InfoContent { get { return m_InfoContent; } set { m_InfoContent = value; OnPropertyChanged(nameof(InfoContent)); } }
        private string m_InfoContent;

        public bool OKsFilterChecked { get { return m_OKsFilterChecked; } set { m_OKsFilterChecked = value; CheckFilters(); OnPropertyChanged(nameof(OKsFilterChecked)); } }

        private bool m_OKsFilterChecked;

        public bool WarningsFilterChecked { get { return m_WarningsFilterChecked; } set { m_WarningsFilterChecked = value; CheckFilters(); OnPropertyChanged(nameof(WarningsFilterChecked)); } }
        private bool m_WarningsFilterChecked;

        public bool ErrorsFilterChecked { get { return m_ErrorsFilterChecked; } set { m_ErrorsFilterChecked = value; CheckFilters(); OnPropertyChanged(nameof(ErrorsFilterChecked)); } }
        private bool m_ErrorsFilterChecked;

        public bool InfoFilterChecked { get { return m_InfoFilterChecked; } set { m_InfoFilterChecked = value; CheckFilters(); OnPropertyChanged(nameof(InfoFilterChecked)); } }
        private bool m_InfoFilterChecked;
        #endregion properties


        #region commands


        public RelayCommand ClearLogCommand => m_ClearLogCommand ?? (m_ClearLogCommand = new RelayCommand(OnClearLogCommand));
        private RelayCommand m_ClearLogCommand;

        private void OnClearLogCommand()
        {
            Entries = new ObservableCollection<LogResult>();
            UpdateGUI();
        }

        public RelayCommand ExportLogCommand => m_ExportLogCommand ?? (m_ExportLogCommand = new RelayCommand(OnExportLogCommand));
        private RelayCommand m_ExportLogCommand;

        private void OnExportLogCommand()
        {
            ExportLog();
        }

        #endregion commands

        #region notifypropertychanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion notifypropertychanged


        #region public methods
        public void AddEntry(LogResult entry)
        {
            Entries.Insert(0, entry);
            UpdateGUI();
        }
        #endregion public methods


        #region private methods

        private void UpdateGUI()
        {
            OkContent = $"OK ({Entries.Where(x=>x.InfoType == LogResultTypes.OK).Count()})";
            WarningContent = $"Warnings ({Entries.Where(x => x.InfoType == LogResultTypes.Warning).Count()})";
            InfoContent = $"Informations ({Entries.Where(x => x.InfoType == LogResultTypes.Information).Count()})";
            ErrorContent = $"Errors ({Entries.Where(x => x.InfoType == LogResultTypes.Error).Count()})";
        }
        private void CheckFilters()
        {
            if (m_OKsFilterChecked)
                Entries.Where(x => x.InfoType == LogResultTypes.OK).ToList().ForEach(x => x.SwitchVisibility(true));
            else
                Entries.Where(x => x.InfoType == LogResultTypes.OK).ToList().ForEach(x => x.SwitchVisibility(false));

            if (m_WarningsFilterChecked)
                Entries.Where(x => x.InfoType == LogResultTypes.Warning).ToList().ForEach(x => x.SwitchVisibility(true));
            else
                Entries.Where(x => x.InfoType == LogResultTypes.Warning).ToList().ForEach(x => x.SwitchVisibility(false));

            if (m_ErrorsFilterChecked)
                Entries.Where(x => x.InfoType == LogResultTypes.Error).ToList().ForEach(x => x.SwitchVisibility(true));
            else
                Entries.Where(x => x.InfoType == LogResultTypes.Error).ToList().ForEach(x => x.SwitchVisibility(false));

            if (m_InfoFilterChecked)
                Entries.Where(x => x.InfoType == LogResultTypes.Information).ToList().ForEach(x => x.SwitchVisibility(true));
            else
                Entries.Where(x => x.InfoType == LogResultTypes.Information).ToList().ForEach(x => x.SwitchVisibility(false));
        }

        private void ExportLog()
        {
            var file = CommonMethods.SelectDirOrFile(false, "Text file .txt", "*.txt");
            if (string.IsNullOrEmpty(file))
                return;
            if (!file.ToLower().EndsWith(".txt"))
                file += ".txt";
            if (File.Exists(file))
                File.Delete(file);
            string output = string.Empty;
            foreach (var entry in Entries)
            {
                output += entry.ToString() + "\r\n";
            }
            File.WriteAllText(file, output);
            Process.Start(file);
        }
        #endregion private methods



    }
}
