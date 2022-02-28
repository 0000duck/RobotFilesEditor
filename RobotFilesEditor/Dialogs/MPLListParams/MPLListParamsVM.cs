using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Dialogs.MPLListParams
{
    public class MPLListParamsVM : ViewModelBase
    {
        #region proerties
        private string _spotNumberColumnInMPL;
        public string SpotNumberColumnInMPL
        {
            get { return _spotNumberColumnInMPL; }
            set
            {
                if (_spotNumberColumnInMPL != value)
                {
                    _spotNumberColumnInMPL = value;
                    RaisePropertyChanged(() => SpotNumberColumnInMPL);
                }
            }
        }

        private string _xColumnInMPL;
        public string XColumnInMPL
        {
            get { return _xColumnInMPL; }
            set
            {
                if (_xColumnInMPL != value)
                {
                    _xColumnInMPL = value;
                    RaisePropertyChanged(() => XColumnInMPL);
                }
            }
        }

        private string _yColumnInMPL;
        public string YColumnInMPL
        {
            get { return _yColumnInMPL; }
            set
            {
                if (_yColumnInMPL != value)
                {
                    _yColumnInMPL = value;
                    RaisePropertyChanged(() => YColumnInMPL);
                }
            }
        }

        private string _zColumnInMPL;
        public string ZColumnInMPL
        {
            get { return _zColumnInMPL; }
            set
            {
                if (_zColumnInMPL != value)
                {
                    _zColumnInMPL = value;
                    RaisePropertyChanged(() => ZColumnInMPL);
                }
            }
        }

        private string _firstSpotRow;
        public string FirstSpotRow
        {
            get { return _firstSpotRow; }
            set
            {
                if (_firstSpotRow != value)
                {
                    _firstSpotRow = value;
                    RaisePropertyChanged(() => FirstSpotRow);
                }
            }
        }

        private string _sheetName;
        public string SheetName
        {
            get { return _sheetName; }
            set
            {
                if (_sheetName != value)
                {
                    _sheetName = value;
                    RaisePropertyChanged(() => SheetName);
                }
            }
        }

        private string _puntkType;
        public string PunktType
        {
            get { return _puntkType; }
            set
            {
                if (_puntkType != value)
                {
                    _puntkType = value;
                    RaisePropertyChanged(() => PunktType);
                }
            }
        }

        private ObservableCollection<string> _availableTypes;
        public ObservableCollection<string> AvailableTypes
        {
            get { return _availableTypes; }
            set {Set(ref _availableTypes, value); }
        }

        private string _selectedType;
        public string SelectedType
        {
            get { return _selectedType; }
            set {
                Set (ref _selectedType, value);
                if (value == "SWH/SWR")
                    SelectedType = "SWx";
                if (value == "Based on type column")
                    SelectedType = "";
            }
        }

        private int _selectedIndex;

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set { Set(ref _selectedIndex, value); }
        }

        private string _spotIndexVW;
        public string SpotIndexVW
        {
            get { return _spotIndexVW; }
            set
            {
                Set(ref _spotIndexVW, value);
            }
        }


        #endregion


        #region ctor

        public MPLListParamsVM(Model.DataInformations.DatasInExcel data)
        {
            //SpotNumberColumnInMPL = ConfigurationManager.AppSettings["MPLSpotNumberColumn"];
            //XColumnInMPL = ConfigurationManager.AppSettings["MPLXCoord"];
            //YColumnInMPL = ConfigurationManager.AppSettings["MPLYCoord"];
            //ZColumnInMPL = ConfigurationManager.AppSettings["MPLZCoord"];
            //FirstSpotRow = ConfigurationManager.AppSettings["MPLFirstPointRow"];
            //SheetName = ConfigurationManager.AppSettings["MPLSheetName"];
            AvailableTypes = new ObservableCollection<string>() { "Based on type column", "SWP", "SWH/SWR", "RVT", "FLS", "CLI", "BRT" };
            SelectedIndex = 1;
            SpotNumberColumnInMPL = data.SpotNumberColumnInMPL.ToString();
            XColumnInMPL = data.XColumnInMPL.ToString() ;
            YColumnInMPL = data.YColumnInMPL.ToString() ;
            ZColumnInMPL = data.ZColumnInMPL.ToString() ;
            FirstSpotRow = data.FirstSpotRow.ToString() ;
            SheetName = data.SheetName;
            PunktType = data.Punkttype;
            SpotIndexVW = data.SpotIndex;
        }
        #endregion
    }
}
