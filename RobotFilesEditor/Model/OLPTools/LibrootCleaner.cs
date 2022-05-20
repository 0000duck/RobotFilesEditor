using RobotFilesEditor.Dialogs.LibrootCleaner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RobotFilesEditor.Model.OLPTools
{
    public class LibrootCleaner
    {
        ViewModel.MainViewModel mainVM;

        public LibrootCleaner(ViewModel.MainViewModel vmIn)
        {
            mainVM = vmIn;
            Execute();
        }

        private void Execute()
        {
            LibrootCleanerVM vmCleaner = new LibrootCleanerVM();
            var window = new LibrootCleanerWindow(vmCleaner);
            window.Owner = Application.Current.Windows.Cast<Window>().Single(x => x.DataContext == mainVM);
            window.ShowDialog();
        }
    }
}
