using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RobotFilesEditor.Model.DataInformations;
using RobotFilesEditor.Model.DataOrganization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RobotFilesEditor.Dialogs.TypIdChanger
{
    public class TypIDBySameIDWindowVM : ViewModelBase
    {
        #region ctor
        public TypIDBySameIDWindowVM(List<KeyValuePair<int, WeldpointBMW>> inputPoints)
        {
            InputPoints = inputPoints;
            OKCommand = new RelayCommand(OKCommandExecute);
            CancelCommand = new RelayCommand(CancelCommandExecute);
            FoundTypIDs = FillInputPoints(inputPoints);
        }
        #endregion

        #region fields
        private List<KeyValuePair<int, WeldpointBMW>> InputPoints;
        #endregion

        #region commands
        public ICommand OKCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        #endregion

        #region properties
        private List<string> pointsList;

        public List<string> PointsList
        {
            get { return pointsList; }
            set { pointsList = value; }
        }

        private ObservableCollection<TypId> foundTypIDs;
        public ObservableCollection<TypId> FoundTypIDs
        {
            get { return foundTypIDs; }
            set { foundTypIDs = value; }
        }

        private int selectedTypId;
        public int SelectedTypId
        {
            get { return selectedTypId; }
            set {
                selectedTypId = value;
            }
        }
        #endregion

        #region methods
        private void OKCommandExecute()
        {
            FoundTypIDs = FillEmptyTypIds();
            PointsList = fillPointsList();
            Close(true);
        }
        private void CancelCommandExecute()
        {
            Close(false);
        }

        private ObservableCollection<TypId> FillEmptyTypIds()
        {
            ObservableCollection<TypId> result = new ObservableCollection<TypId>();
            foreach (var item in FoundTypIDs)
            {
                TypId currentTypId = new TypId(item.OldTypIds);
                if (string.IsNullOrEmpty(item.NewTypIds))
                    currentTypId.NewTypIds = item.OldTypIds;
                else
                    currentTypId = item;
                result.Add(currentTypId);
            }
            return result;
        }

        private List<string> fillPointsList()
        {
            List<string> result = new List<string>();
            foreach (var point in InputPoints)
            {
                string typIdToAdd = FoundTypIDs.First(x => x.OldTypIds == point.Value.TypId.ToString()).NewTypIds;
                result.Add("Point: " + point.Key + " TypID: " + typIdToAdd);
            }
            return result;
        }

        private ObservableCollection<TypId> FillInputPoints(List<KeyValuePair<int, WeldpointBMW>> inputPoints)
        {
            ObservableCollection<TypId> result = new ObservableCollection<TypId>();
            foreach (var point in inputPoints)
            {
                if (!result.Any(x => x.OldTypIds == point.Value.TypId.ToString()))
                    result.Add(new TypId(point.Value.TypId.ToString()));
            }
            return result;
        }

        private void Close(bool success)
        {
            var window = Application.Current.Windows
            .Cast<Window>()
            .Single(w => w.DataContext == this);
            window.DialogResult = success;
            window.Close();
        }
        #endregion
    }
}
