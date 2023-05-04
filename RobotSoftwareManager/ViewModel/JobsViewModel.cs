using CommonLibrary.DataClasses;
using CommunityToolkit.Mvvm.ComponentModel;
using RobotSoftwareManager.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotSoftwareManager.ViewModel
{
    public partial class JobsViewModel : ViewModelBase
    {
        #region properties

        public ObservableCollection<RobotJob> JobsCollection { get { var collection = Sort(m_JobsCollection.ToList().ConvertAll(x => (RobotSoftwareBase)x)); var resultList = collection.ConvertAll(x => (RobotJob)x); return CommonLibrary.CommonMethods.ToObservableCollection<RobotJob>(resultList); } set { SetProperty(ref m_JobsCollection, value); } }
        private ObservableCollection<RobotJob> m_JobsCollection;

        [ObservableProperty]
        RobotJob selectedJob;

        [ObservableProperty]
        ObservableCollection<int> availableJobs;

        [ObservableProperty]
        RobotSoftCheckState checkState;
        #endregion properties


        #region constructor
        public JobsViewModel(List<RobotJob> robotJobs)
        {
            JobsCollection = CommonLibrary.CommonMethods.ToObservableCollection(robotJobs);
            GetAvailableBases();
            JobsCollection.ToList().ForEach(x => x.UpdateSelectedValue(AvailableJobs));
            SelectedJob = JobsCollection.FirstOrDefault();
        }
        #endregion constructor

        #region private methods
        private void GetAvailableBases()
        {
            if (AvailableJobs is null)
                AvailableJobs = new ObservableCollection<int>();
            else
                AvailableJobs.Clear();
            for (int i = 1; i <= 64; i++)
            {
                AvailableJobs.Add(i);
            }
        }
        #endregion private methods
    }
}
