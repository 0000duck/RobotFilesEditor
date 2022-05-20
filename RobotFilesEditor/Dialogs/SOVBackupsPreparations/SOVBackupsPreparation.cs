using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RobotFilesEditor.Dialogs.SOVBackupsPreparations
{
    public class SOVBackupsPreparation
    {
        public SOVBackupsPreparation(ViewModel.MainViewModel mainVM)
        {
            SOVBackupPreparationVM vm = new SOVBackupPreparationVM(true, GlobalData.RobotController.KUKA);
            SOVBackupsPreparationWindow window = new SOVBackupsPreparationWindow(vm);
            window.Owner = Application.Current.Windows.Cast<Window>().Single(x => x.DataContext == mainVM);
            var dialog = window.ShowDialog();
        }
    }
}
