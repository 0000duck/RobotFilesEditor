using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RobotFilesEditor.Dialogs
{
    public class ApplicationSelectorViewModel : ViewModelBase
    {
        public ApplicationSelectorViewModel(List<AppPair> founPairs)
        {
            Data = new ObservableCollection<AppPair>();
            Data.Add(new AppPair("A", 1, true, "PLC Common"));
            Data.Add(new AppPair("A", 2, false, "Toolchanger PTM"));
            Data.Add(new AppPair("B", 2, false, "Toolchange Staubli"));
            Data.Add(new AppPair("A", 3, false, "Gripper"));
            Data.Add(new AppPair("A", 4, false, "Spot pneumatic"));
            Data.Add(new AppPair("A", 5, false, "Spot servo"));
            Data.Add(new AppPair("A", 8, false, "Glue"));
            Data.Add(new AppPair("A", 10, false, "Search"));
            Data.Add(new AppPair("A", 12, false, "Palletizing"));
            Data.Add(new AppPair("A", 13, false, "Rivet HSN/VSN"));
            Data.Add(new AppPair("A", 14, false, "Arc welding"));
            Data.Add(new AppPair("B", 14, false, "Arc welding"));
            Data.Add(new AppPair("A", 15, false, "Laser tacktile"));
            Data.Add(new AppPair("B", 15, false,"Laser tacktile"));
            Data.Add(new AppPair("A", 16, false, "Laser remote"));
            Data.Add(new AppPair("B", 16, false, "Laser remote"));
            Data.Add(new AppPair("A", 19, false, "Clinching"));
            Data.Add(new AppPair("A", 20, false, "Stud weld"));
            Data.Add(new AppPair("A", 21, false, "Stud weld rot."));
            Data.Add(new AppPair("A", 22, false, "FLS"));
            Data.Add(new AppPair("B", 22, false, "FLS"));
            Data.Add(new AppPair("A", 23, false, "Hemming"));
            Data.Add(new AppPair("A", 24, false, "Inline mess."));
            Data.Add(new AppPair("B", 24, false, "Inline mess."));
            Data.Add(new AppPair("A", 27, false, "Blind rivet"));
            Data.Add(new AppPair("A", 28, false, "Fasten"));
            Data.Add(new AppPair("A", 29, false, "Weld check"));
            Data.Add(new AppPair("A", 47, false, "Object detection"));

            foreach (var pair in founPairs)
            {
                Data.Where(x => x.Prefix == pair.Prefix).First(y => y.Suffix == pair.Suffix).IsSelected = true;
            }

            SetCommands();
        }

        
        private ObservableCollection<AppPair> data;
        public ObservableCollection<AppPair> Data
        {
            get { return data; }
            set
            {
                Set(ref data,value);
            }
        }


        public ICommand OK { get; set; }

        private void SetCommands()
        {
            OK = new RelayCommand(OKExecute);
        }

        private void OKExecute()
        {
            var window = Application.Current.Windows
            .Cast<Window>()
            .Single(w => w.DataContext == this);
            window.DialogResult = false;
            window.Close();
        }
    }

    public class AppPair
    {
        public string Prefix { get; set; }
        public int Suffix { get; set; }
        public bool IsSelected { get; set; }
        public string Comment { get; set; }

        public AppPair(string prefix, int suffix, bool isSelected, string comment = "")
        {
            Prefix = prefix;
            Suffix = suffix;
            IsSelected = isSelected;
            Comment = comment;

        }
    }
}
