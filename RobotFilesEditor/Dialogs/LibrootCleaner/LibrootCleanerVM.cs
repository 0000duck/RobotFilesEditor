using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RobotFilesEditor.Dialogs.LibrootCleaner
{
    public class LibrootCleanerVM : ViewModelBase
    {
        #region ctor
        public LibrootCleanerVM()
        {
            ExpandersVisible = System.Windows.Visibility.Collapsed;
            SetCommands();
        }
        #endregion

        #region properties
        private string pszFilePath;
        public string PszFilePath
        {
            get { return pszFilePath; }
            set
            {
                Set(ref pszFilePath, value);
                CheckScanEnabled();
            }
        }

        private string librootPath;
        public string LibrootPath
        {
            get { return librootPath; }
            set
            {
                Set(ref librootPath, value);
                CheckScanEnabled();
            }
        }

        private bool scanEnabled;
        public bool ScanEnabled
        {
            get { return scanEnabled; }
            set
            {
                Set(ref scanEnabled, value);
            }
        }

        private System.Windows.Visibility expandersVisible;
        public System.Windows.Visibility ExpandersVisible
        {
            get { return expandersVisible; }
            set
            {
                Set( ref expandersVisible, value);
            }
        }

        private LibrootCleanerExpanderVM missingInPSZ;
        public LibrootCleanerExpanderVM MissingInPSZ
        {
            get { return missingInPSZ; }
            set
            {
                Set(ref missingInPSZ, value);
            }
        }

        private LibrootCleanerExpanderVM missingInLibroot;
        public LibrootCleanerExpanderVM MissingInLibroot
        {
            get { return missingInLibroot; }
            set
            {
                Set(ref missingInLibroot, value);
            }
        }

        private LibrootCleanerExpanderVM okPairs;
        public LibrootCleanerExpanderVM OkPairs
        {
            get { return okPairs; }
            set
            {
                Set(ref okPairs, value);
            }
        }
        #endregion

        #region commands
        public ICommand SelectPSZFile { get; set; }
        public ICommand SelectLibroot { get; set; }
        public ICommand ScanLibrootAndPSZ { get; set; }

        private void SetCommands()
        {
            SelectPSZFile = new RelayCommand(SelectPSZFileExecute);
            SelectLibroot = new RelayCommand(SelectLibrootExecute);
            ScanLibrootAndPSZ = new RelayCommand(ScanLibrootAndPSZExecute);
        }
        #endregion

        #region methods
        private void SelectPSZFileExecute()
        {
            PszFilePath = CommonLibrary.CommonMethods.SelectDirOrFile(false, "PSZ file", "*.psz");
        }

        private void SelectLibrootExecute()
        {
            LibrootPath = CommonLibrary.CommonMethods.SelectDirOrFile(true);
        }

        private void ScanLibrootAndPSZExecute()
        {
            var librootCleanerModel = new LibrootCleanerModel(PszFilePath, LibrootPath);
            if (librootCleanerModel != null)
                ExpandersVisible = System.Windows.Visibility.Visible;
            OkPairs = new LibrootCleanerExpanderVM("Ok objects", librootCleanerModel.OkPairs);
            MissingInLibroot = new LibrootCleanerExpanderVM("Missing in libroot", librootCleanerModel.MissingInLibroot);
            MissingInPSZ = new LibrootCleanerExpanderVM("Missing in PSZ", librootCleanerModel.MissingInPSZ);
        }

        private void CheckScanEnabled()
        {
            if (File.Exists(PszFilePath) && Directory.Exists(LibrootPath))
                ScanEnabled = true;
            else
                ScanEnabled = false;
        }
        #endregion
    }
}
