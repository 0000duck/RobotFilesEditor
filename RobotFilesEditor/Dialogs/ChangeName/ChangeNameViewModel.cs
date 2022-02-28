using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RobotFilesEditor.Dialogs.ChangeName
{
    public class ChangeNameViewModel : ViewModelBase
    {
        public ChangeNameViewModel()
        {
            Name = ConfigurationManager.AppSettings["Ersteller"];
        }

        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                ChangeErsteller(_name);
                RaisePropertyChanged("Name");
            }
        }

        private void ChangeErsteller(string name)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);

            config.AppSettings.Settings.Remove("Ersteller");
            config.AppSettings.Settings.Add("Ersteller", name);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
            config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        }
    }
}
