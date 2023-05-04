using RobotSoftwareManager.Model;
using CommonLibrary.DataClasses;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RobotSoftwareManager.ViewModel
{
    public partial class BaseViewModel : ViewModelBase
    {
        #region properties
        public ObservableCollection<RobotBase> BaseCollection { get { var collection = Sort(m_BaseCollection.ToList().ConvertAll(x=>(RobotSoftwareBase)x)); var resultList = collection.ConvertAll(x => (RobotBase)x); return CommonLibrary.CommonMethods.ToObservableCollection<RobotBase>(resultList) ; } set { SetProperty(ref m_BaseCollection, value); } }
        private ObservableCollection<RobotBase> m_BaseCollection;

        [ObservableProperty]
        RobotBase selectedBase;
       
        [ObservableProperty]
        ObservableCollection<int> availableBases;

        [ObservableProperty]
        RobotSoftCheckState checkState;
        #endregion properties

        #region constructor
        public BaseViewModel(List<RobotBase> robotBases)
        {
            BaseCollection = CommonLibrary.CommonMethods.ToObservableCollection(robotBases);
            GetAvailableBases();
            BaseCollection.ToList().ForEach(x => x.UpdateSelectedValue(AvailableBases));
            SelectedBase = BaseCollection.FirstOrDefault();
        }

        #endregion constructor

        #region private methods
        private void GetAvailableBases()
        {
            if (AvailableBases is null)
                AvailableBases = new ObservableCollection<int>();
            else
                AvailableBases.Clear();
            for (int i = 1; i <= 128; i++)
            {
                AvailableBases.Add(i);
            }
        }
        #endregion private methods
    }
}
