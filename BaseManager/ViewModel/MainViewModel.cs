using BaseManager.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLibrary.DataClasses;

namespace BaseManager.ViewModel
{
    public partial class MainViewModel : ObservableRecipient
    {

        #region properties
        [ObservableProperty]
        ObservableCollection<RobotBase> baseCollection;

        [ObservableProperty]
        RobotBase selectedBase;
        #endregion properties

        [ObservableProperty]
        ObservableCollection<int> availableBases;

        [ObservableProperty]
        BaseCheckState checkState;

        #region constructor
        public MainViewModel(List<RobotBase> robotBases)
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
            for (int i = 1;i <= 128;i++)
            {
              AvailableBases.Add(i);  
            }
        }
        #endregion private methods
    }
}
