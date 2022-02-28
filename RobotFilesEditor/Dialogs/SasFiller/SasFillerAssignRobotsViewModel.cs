using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
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

namespace RobotFilesEditor.Dialogs.SasFiller
{
    public class SasFillerAssignRobotsViewModel : ViewModelBase
    {
        public SasFillerAssignRobotsViewModel(ObservableCollection<RobotAssingmentData> inputRobots, ObservableCollection<string> robotsFromBackups)
        {
            //originalBackup = new ObservableCollection<string>();
            //robotsFromBackups.ToList().ForEach(x => originalBackup.Add(x));
            RobotsBackups = robotsFromBackups;
            Robots = inputRobots;
            SelectedRobot = 0;
        }

        //private ObservableCollection<string> originalBackup;

        private ObservableCollection<RobotAssingmentData> robots;
        public ObservableCollection<RobotAssingmentData> Robots
        {
            get { return robots; }
            set
            {
                Set(ref robots, value);
            }
        }

        private ObservableCollection<string> robotsBackups;
        public ObservableCollection<string> RobotsBackups
        {
            get { return robotsBackups; }
            set
            {
                Set(ref robotsBackups, value);
            }
        }

        private int selectedRobot;
        public int SelectedRobot
        {
            get { return selectedRobot; }
            set
            {
                Set(ref selectedRobot, value);
                //RobotsBackups = UpdateRobotList();
                //RaisePropertyChanged(() => Robots);
            }
        }

        RelayCommand _oKCommand;
        public RelayCommand OKCommand
        {
            get
            {
                if (_oKCommand == null)
                {
                    _oKCommand = new RelayCommand(OKCommandExecute);
                }
                return _oKCommand;
            }
        }

        RelayCommand _cancelCommand;
        public RelayCommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                {
                    _cancelCommand = new RelayCommand(CancelCommandExecute);
                }
                return _cancelCommand;
            }
        }

        private void CancelCommandExecute()
        {
            var window = Application.Current.Windows
            .Cast<Window>()
            .Single(w => w.DataContext == this);
            window.DialogResult = false;
            window.Close();
        }

        private void OKCommandExecute()
        {
            if (CheckDoubleAssignment())
                return;
            var window = Application.Current.Windows
            .Cast<Window>()
            .Single(w => w.DataContext == this);
            window.DialogResult = true;
            window.Close();
        }

        private bool CheckDoubleAssignment()
        {
            List<string> assignedRobots = new List<string>();
            foreach (var robot in Robots.Where(x=>!string.IsNullOrEmpty(x.RobotsFromBackups)))
            {
                if (!assignedRobots.Contains(robot.RobotsFromBackups))
                    assignedRobots.Add(robot.RobotsFromBackups);
                else
                {
                    MessageBox.Show("Mulitple robots assignedo to backup " + robot.RobotsFromBackups + "\r\nCorrect data and try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return true;
                }
            }
            return false;
        }

        //private ObservableCollection<string> UpdateRobotList()
        //{
        //    List<string> alreadyAssignedRobots = new List<string>();

        //    foreach (var robot in Robots.ToList().Where(x=>!string.IsNullOrEmpty(x.RobotsFromBackups)))
        //    {
        //        alreadyAssignedRobots.Add(robot.RobotsFromBackups.ToLower());
        //    }
        //    ObservableCollection<string> result = new ObservableCollection<string>();
        //    foreach (var robot in originalBackup)
        //    {
        //        if (alreadyAssignedRobots.Contains(Path.GetFileNameWithoutExtension(robot).ToLower()))
        //        { }
        //        else
        //            result.Add(robot);
        //    }
        //    return result;
        //}

    }
}
 