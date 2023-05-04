using RobotSoftwareManager.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLibrary.DataClasses;
using System.Collections.ObjectModel;

namespace RobotSoftwareManager.ViewModel
{
    public partial class CollisionsViewModel : ViewModelBase
    {

        #region properties
        public ObservableCollection<RobotCollision> CollisionsCollection { get { var collection = Sort(m_CollisionsCollection.ToList().ConvertAll(x => (RobotSoftwareBase)x)); var resultList = collection.ConvertAll(x => (RobotCollision)x); return CommonLibrary.CommonMethods.ToObservableCollection<RobotCollision>(resultList); } set { SetProperty(ref m_CollisionsCollection, value); } }
        private ObservableCollection<RobotCollision> m_CollisionsCollection;

        [ObservableProperty]
        RobotCollision selectedCollision;

        [ObservableProperty]
        ObservableCollection<int> availableCollisions;

        [ObservableProperty]
        RobotSoftCheckState checkState;
        #endregion properties

        #region constructor
        public CollisionsViewModel(List<RobotCollision> robotCollisions)
        {
            CollisionsCollection = CommonLibrary.CommonMethods.ToObservableCollection(robotCollisions);
            GetAvailableCollisions();
            CollisionsCollection.ToList().ForEach(x => x.UpdateSelectedValue(AvailableCollisions));
            SelectedCollision = CollisionsCollection.FirstOrDefault();
            robotCollisions.ForEach(x => x.UpdateGui());
        }
        #endregion constructor

        #region private methods
        private void GetAvailableCollisions()
        {
            if (AvailableCollisions is null)
                AvailableCollisions = new ObservableCollection<int>();
            else
                AvailableCollisions.Clear();
            for (int i = 1; i <= 254; i++)
            {
                AvailableCollisions.Add(i);
            }
        }
        #endregion private methods
    }
}
