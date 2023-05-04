using RobotSoftwareManager.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLibrary.DataClasses;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Input;
using CommonLibrary.Messaging;
using RobotSoftwareManager.Messaging;
using System.Windows;

namespace RobotSoftwareManager.ViewModel
{
    public partial class MainViewModel : ObservableRecipient, IRecipient<UpdateMainViewModelMessage>
    {
        public bool Result { get; set; }

        [ObservableProperty]
        bool okEnabled;

        [ObservableProperty]
        BaseViewModel baseViewModel;

        [ObservableProperty]
        CollisionsViewModel collisionsViewModel;

        [ObservableProperty]
        JobsViewModel jobsViewModel;

        [ObservableProperty]
        LogCollection logCollection;

        [RelayCommand]
        void Ok(Window window)
        {
            Result = true;
            if (window != null)
                window.Close();
        }

        [RelayCommand]
        void Cancel(Window window)
        {
            Result = false;
            if (window != null)
                window.Close();
        }
        public MainViewModel(List<RobotBase> robotBases, List<RobotJob> robotJobs, List<RobotCollision> robotCollisions)
        {
            WeakReferenceMessenger.Default.Register<UpdateMainViewModelMessage>(this);
            LogCollection = new LogCollection(true);
            BaseViewModel = new BaseViewModel(robotBases);
            CollisionsViewModel = new CollisionsViewModel(robotCollisions);
            JobsViewModel = new JobsViewModel(robotJobs);
            UpdateLogCollection();
        }

        public void UpdateLogCollection()
        {
            OkEnabled = true;
            WeakReferenceMessenger.Default.Send<ClearLogMessage>(new ClearLogMessage(true));
            if (BaseViewModel != null)
            {
                if (BaseViewModel.BaseCollection.Any(x => x.CheckState == RobotSoftCheckState.Warning))
                    WeakReferenceMessenger.Default.Send<CommonLibrary.Messaging.AddLogMessage>(new CommonLibrary.Messaging.AddLogMessage(new LogResult("Warnings present in bases tab", LogResultTypes.Warning)));
                if (BaseViewModel.BaseCollection.Any(x => x.CheckState == RobotSoftCheckState.Error))
                {
                    OkEnabled = false;
                    WeakReferenceMessenger.Default.Send<CommonLibrary.Messaging.AddLogMessage>(new CommonLibrary.Messaging.AddLogMessage(new LogResult("Errors present in bases tab", LogResultTypes.Error)));
                }
            }
            if (CollisionsViewModel != null)
            {
                if (CollisionsViewModel.CollisionsCollection.Any(x => x.CheckState == RobotSoftCheckState.Warning))
                    WeakReferenceMessenger.Default.Send<CommonLibrary.Messaging.AddLogMessage>(new CommonLibrary.Messaging.AddLogMessage(new LogResult("Warnings present in collisions tab", LogResultTypes.Warning)));
                if (BaseViewModel.BaseCollection.Any(x => x.CheckState == RobotSoftCheckState.Error))
                {
                    OkEnabled = false;
                    WeakReferenceMessenger.Default.Send<CommonLibrary.Messaging.AddLogMessage>(new CommonLibrary.Messaging.AddLogMessage(new LogResult("Errors present in collisions tab", LogResultTypes.Error)));
                }
            }
            if (JobsViewModel != null)
            {
                if (JobsViewModel.JobsCollection.Any(x => x.CheckState == RobotSoftCheckState.Warning))
                    WeakReferenceMessenger.Default.Send<CommonLibrary.Messaging.AddLogMessage>(new CommonLibrary.Messaging.AddLogMessage(new LogResult("Warnings present in jobs tab", LogResultTypes.Warning)));
                if (JobsViewModel.JobsCollection.Any(x => x.CheckState == RobotSoftCheckState.Error))
                {
                    OkEnabled = false;
                    WeakReferenceMessenger.Default.Send<CommonLibrary.Messaging.AddLogMessage>(new CommonLibrary.Messaging.AddLogMessage(new LogResult("Errors present in jobs tab", LogResultTypes.Error)));
                }
            }

        }

        public void Receive(UpdateMainViewModelMessage message)
        {
            UpdateLogCollection();
        }
    }
}
