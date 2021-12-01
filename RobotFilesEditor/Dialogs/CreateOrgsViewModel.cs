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
using System.Windows.Input;
using System.Windows;
using System.Configuration;
using System.Text.RegularExpressions;

namespace RobotFilesEditor.Dialogs
{
    public class CreateOrgsViewModel : ViewModelBase
    {
        SortedDictionary<int, string> typesAndNames;

        #region Ctor
        public CreateOrgsViewModel(IDictionary<string, int> paths, IDictionary<int,string> jobs)
        {
            EnableOK = false;
            if (GlobalData.ControllerType == "KRC2 V8" || GlobalData.ControllerType == "KRC4")
                RobotName = GetRobotName();
            IsCheckHomeEnabled = GlobalData.ControllerType == "KRC4" ? true : false;
            WaitForInHome = GlobalData.ControllerType == "KRC4" ? false : true;
            PLCCheckHome = GlobalData.ControllerType == "KRC4" ? true : false;
            typesAndNames = new SortedDictionary<int, string>();
            Abort = new ObservableCollection<TextItem>( FillAbort());
            WithParts = new ObservableCollection<TextItem>(FillWithParts());
            AbortNumber = new ObservableCollection<IntItem>();
            SafeRobot = false;
            Lines = FillLines();
            NrOfTools = FillTools();
            SelectedToolsNumber = 1;
            SelectedGunsNumber = 0;
            Orgnumbers = FillOrgNums();
            SelectedStartOrgNum = 1;

            RobotName = GlobalData.RobotNameFanuc;
            Paths = paths;
            GlobalData.Paths = new Dictionary<string, int>(Paths);
            if (!Paths.Keys.Contains("ANYJOB"))
                Paths.Add("ANYJOB", 98);
            if (!Paths.Keys.Contains("USERNUM"))
                Paths.Add("USERNUM", 99);
            if (!Paths.Keys.Contains("ANYJOB/USERNUM") && !GlobalData.ControllerType.Contains("KRC2"))
                Paths.Add("ANYJOB/USERNUM", 97);
            Jobs = jobs;

            OrgsElementVM.SendProperty += new OrgsElementVM.SendPropertyHandler(AbortPropertyChanged);
            List<OrgsElementVM> orgsElements = new List<OrgsElementVM>();

            DictOrgsElementVM = new Dictionary<int,ObservableCollection<OrgsElementVM>>();
            DictOrgsElementVM.Add(0, OrgsElements);
            DictOrgsElements = new Dictionary<int, ICollection<IOrgsElement>>();
            SetCommands();
        }

        private string GetRobotName()
        {
            string result = "";
            Regex findRobotRegex = new Regex(@"\+[a-zA-Z0-9\+_]*", RegexOptions.IgnoreCase);
            result = findRobotRegex.Match(GlobalData.Roboter).ToString();
            return result;
        }

        public ICommand Remove { get; set; }

        private void SetCommands()
        {
            Remove = new RelayCommand(RemoveExecute);
        }
        
        private void RemoveExecute()
        {
            throw new NotImplementedException();
        }

        private List<string> FillLines()
        {
            List<string> lines = ConfigurationManager.AppSettings["Lines"].Split(',').Select(s => s.Trim()).ToList();
            return lines;
        }

        private List<int> FillTools()
        {
            List<int> result = new List<int>();
            for (int i = 1; i <= 7; i++)
                result.Add(i);
            return result;
        }

        //private List<int> FillGuns()
        //{
        //    List<int> result = new List<int>();
        //    for (int i = 0; i <= 5; i++)
        //        result.Add(i);
        //    return result;
        //}

        private void OrgsElements_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                var orgsElement = e.NewItems[0] as OrgsElementVM;
                orgsElement.Id = _orgsElements.Max(c => c.Id) ?? 0;
                orgsElement.Id++;
            }
        }
        #endregion

        #region Properties
        public OrgsCreatorResult OrgsCreatorResult { get; set; }

        bool _safeRobot;
        public bool SafeRobot
        {
            get { return _safeRobot; }
            set
            {
                _safeRobot = value;
                RaisePropertyChanged(()=> SafeRobot);
            }
        }

        bool _enableOK;
        public bool EnableOK
        {
            get { return _enableOK; }
            set
            {
                _enableOK = value;
                RaisePropertyChanged(() => EnableOK);
            }
        }

        bool _pLCCheckHome;
        public bool PLCCheckHome
        {
            get { return _pLCCheckHome; }
            set
            {
                _pLCCheckHome = value;
                if (value == true)
                    WaitForInHome = false;
                RaisePropertyChanged(() => PLCCheckHome);
            }
        }
        bool _isCheckHomeEnabled;
        public bool IsCheckHomeEnabled
        {
            get { return _isCheckHomeEnabled; }
            set
            {
                _isCheckHomeEnabled = value;
                RaisePropertyChanged(() => IsCheckHomeEnabled);
            }
        }
        bool _waitForInHome;
        public bool WaitForInHome
        {
            get { return _waitForInHome; }
            set
            {
                if (value == true)
                    PLCCheckHome = false;
                _waitForInHome = value;
                RaisePropertyChanged(() => WaitForInHome);
            }
        }

        int _selectedtype;
        public int SelectedType
        {
            get { return _selectedtype; }
            set
            {
                _selectedtype = value;
                RaisePropertyChanged("SelectedType");
                OnSelectedTypeChanged(value);
            }
        }

        //bool _multiTools;
        //private bool MultiTools
        //{
        //    get { return _multiTools; }
        //    set
        //    {
        //        _multiTools = value;
        //        RaisePropertyChanged(()=> MultiTools);
        //    }
        //}

        string _selectedLine;
        public string SelectedLine
        {
            get { return _selectedLine; }
            set
            {
                _selectedLine = value;
                RaisePropertyChanged(() => SelectedLine);
                OnSelectedLineChanged(value);
            }
        }

        string _robotName;
        public string RobotName
        {
            get { return _robotName; }
            set
            {
                _robotName = value;
                RaisePropertyChanged(() => RobotName);
                //OnSelectedLineChanged(value);
            }
        }

        private string _selectedPLC;
        public string SelectedPLC
        {
            get { return _selectedPLC; }
            set
            {
                if (_selectedPLC != value)
                {
                    _selectedPLC = value;
                    RaisePropertyChanged(() => SelectedPLC);
                }
            }
        }

        string _selectedTypeName;
        public string SelectedTypeName
        {
            get { return _selectedTypeName; }
            set
            {
                _selectedTypeName = value;
                RaisePropertyChanged(() => SelectedTypeName);
            }
        }

        int _selectedToolsNumber;
        public int SelectedToolsNumber
        {
            get { return _selectedToolsNumber; }
            set
            {
                _selectedToolsNumber = value;
                RaisePropertyChanged(()=>SelectedToolsNumber);
                OnSelectedToolNumberChanged(value);
            }
        }

        int _selectedStartOrgNum;
        public int SelectedStartOrgNum
        {
            get { return _selectedStartOrgNum; }
            set
            {
                _selectedStartOrgNum = value;
                RaisePropertyChanged(() => SelectedStartOrgNum);
            }
        }

        private void OnSelectedToolNumberChanged(int value)
        {
            NrOfGuns = new ObservableCollection<int>();
            if (value != 1)
            {
                for (int i = 0; i <= value; i++)
                {
                    NrOfGuns.Add(i);
                }
                SelectedGunsNumber = 0;
            }
            
           // RaisePropertyChanged(() => NrOfGuns);



        }

        int _selectedGunsNumber;
        public int SelectedGunsNumber
        {
            get { return _selectedGunsNumber; }
            set
            {
                _selectedGunsNumber = value;
                RaisePropertyChanged(() => SelectedGunsNumber);
            }
        }

        List<string> _types;
        public List<string> Types
        {
            get { return _types; }
            set
            {
                _types = value;
                RaisePropertyChanged("Types");
            }
        }

        List<string> _plcs;
        public List<string> PLCs
        {
            get { return _plcs; }
            set
            {
                _plcs = value;
                RaisePropertyChanged("PLCs");
            }
        }


        List<int> _nrOfTools;
        public List<int> NrOfTools
        {
            get { return _nrOfTools; }
            set
            {
                _nrOfTools = value;
                RaisePropertyChanged(()=>NrOfTools);
            }
        }

        //List<int> _nrOfGuns;
        //public List<int> NrOfGuns
        //{
        //    get { return _nrOfGuns; }
        //    set
        //    {
        //        _nrOfGuns = value;
        //        RaisePropertyChanged(() => NrOfGuns);
        //    }
        //}
        ObservableCollection<int> _nrOfGuns;
        public ObservableCollection<int> NrOfGuns
        {
            get { return _nrOfGuns; }
            set
            {
                _nrOfGuns = value;
                RaisePropertyChanged(() => NrOfGuns);
            }
        }

        ObservableCollection<int> _orgnumbers;
        public ObservableCollection<int> Orgnumbers
        {
            get { return _orgnumbers; }
            set
            {
                _orgnumbers = value;
                RaisePropertyChanged(() => Orgnumbers);
            }
        }

        ObservableCollection <TextItem> _abort;
        public ObservableCollection<TextItem> Abort
        {
            get { return _abort; }
            set
            {
                _abort = value;
                RaisePropertyChanged("Abort");
            }
        }

        List<string> _lines;
        public List<string> Lines
        {
            get { return _lines; }
            set
            {
                _lines = value;
                RaisePropertyChanged(()=>Lines);
            }
        }

        ObservableCollection<IntItem> _abortNumber;
        public ObservableCollection<IntItem> AbortNumber
        {
            get { return _abortNumber; }
            set
            {
                _abortNumber = value;
                RaisePropertyChanged("AbortNumber");
            }
        }

        IDictionary<string,int> _paths;
        public IDictionary<string, int> Paths
        {
            get { return _paths; }
            set {
                _paths = value;
                RaisePropertyChanged("Paths");
            }
        }

        IDictionary<int, string> _jobs;
        public IDictionary<int, string> Jobs
        {
            get { return _jobs; }
            set
            {
                _jobs = value;
                RaisePropertyChanged("Jobs");
            }
        }

        public IDictionary<int, ObservableCollection<OrgsElementVM>> DictOrgsElementVM;

        public Dictionary<int, ICollection<IOrgsElement>> DictOrgsElements { get; set; }


        ObservableCollection<OrgsElementVM> _orgsElements;
        public ObservableCollection<OrgsElementVM> OrgsElements
        {
            get { return _orgsElements; }
            set
            {
                if (_orgsElements!= value)
                {
                    _orgsElements = value;
                    RaisePropertyChanged(() => OrgsElements);
                }
            }
        }

        OrgsElementVM _selectedOrgsElement;
        public OrgsElementVM SelectedOrgsElement
        {
            get { return _selectedOrgsElement; }
            set
            {
                    if(_selectedOrgsElement != value)
                {
                    _selectedOrgsElement = value;
                    OnSelectedOrgsElement();                   
                    RaisePropertyChanged(() => SelectedOrgsElement);
                }
            }
        }
 

        private ObservableCollection<TextItem> _withParts;
        public ObservableCollection<TextItem> WithParts
        {
            get { return _withParts; }
            set
            {
                _withParts = value;
                RaisePropertyChanged(() => WithParts);
            }
        }

        #endregion


        #region methods
        private List<int> FillTypes()
        {
            List<int> types = new List<int>();
            for (int i = 0; i <= 32; i++)
                types.Add(i);
            return types;
        }

        private List<TextItem> FillAbort()
        {
            List<TextItem> abort = new List<TextItem>();
            abort.Add( new TextItem { Text = "" });
            abort.Add(new TextItem { Text = "Home" });
            abort.Add(new TextItem { Text = "AbortProg" });
            return abort;
        }

        private List<TextItem> FillWithParts()
        {
            List<TextItem> withPart = new List<TextItem>();
            withPart.Add(new TextItem { Text = "" });
            withPart.Add(new TextItem { Text = "true" });
            withPart.Add(new TextItem { Text = "false" });
            return withPart;
        }

        private void OnSelectedTypeChanged(int value)
        {
            if(!DictOrgsElementVM.Keys.Contains(_selectedtype))
            {
                //DictOrgsElementVM.Add(new KeyValuePair<int, ObservableCollection<OrgsElementVM>>(_selectedtype,new ObservableCollection<OrgsElementVM>()));
                SelectedOrgsElement = new OrgsElementVM();
                SelectedOrgsElement.Id = 1;
                SelectedOrgsElement.Abort = "Home";
                SelectedOrgsElement.WithPart = "false";
                SelectedOrgsElement.AbortNr = 1;
                SelectedOrgsElement.AnyJobValue = new ObservableCollection<KeyValuePair<string, int>>();
                ObservableCollection<OrgsElementVM> temp = new ObservableCollection<OrgsElementVM>();
                if (SelectedLine == "I20_BG")
                    SelectedOrgsElement.TypNumReq = false;
                else
                    SelectedOrgsElement.TypNumReq = true;
                temp.Add(SelectedOrgsElement);
                DictOrgsElementVM.Add(new KeyValuePair<int, ObservableCollection<OrgsElementVM>>(_selectedtype, temp));

            }
            if(OrgsElements!= null)
            {
                OrgsElements.CollectionChanged -= OrgsElements_CollectionChanged;
            }
            OrgsElements = DictOrgsElementVM[_selectedtype];
            OrgsElements.CollectionChanged += OrgsElements_CollectionChanged;

            SelectedTypeName = typesAndNames[value];
            RaisePropertyChanged(() => SelectedTypeName);
        }
 
        void AbortPropertyChanged(object sender, EventArgs arg)
        {
            OrgsElementVM orgsElement = (OrgsElementVM)sender;
            List<IntItem> currentList = new List<IntItem>();
            if (orgsElement.Abort == "Home")
                for (int i = 1; i < 3; i++)
                    currentList.Add(new IntItem { Value = i });
            if (orgsElement.Abort == "AbortProg")
                for (int i = 0; i < 33; i++)
                    currentList.Add(new IntItem { Value = i });
            orgsElement.AbortNrs = new ObservableCollection<IntItem>(currentList); 

        }
        void OnSelectedOrgsElement()
        {
            if (_selectedOrgsElement != null)
            {
                if (_selectedOrgsElement.AbortNrs == null)
                {
                    AbortNumber = null;
                }
                else
                {
                    AbortNumber = new ObservableCollection<IntItem>(_selectedOrgsElement.AbortNrs);
                }
            }
        }

        private void OnSelectedLineChanged(string value)
        {
            if (Lines != null)
            {
                EnableOK = true;
                Types = Model.Operations.CreateOrgsMethods.GetTypesOrPLCs(value, "Line_");
                PLCs = Model.Operations.CreateOrgsMethods.GetTypesOrPLCs(value, "LinePLC_");
                typesAndNames = Model.Operations.CreateOrgsMethods.GetTypesWithDescr(value, Types);
                _selectedtype = 0;
                _selectedTypeName = "";
                RaisePropertyChanged(() => SelectedType);
                RaisePropertyChanged(() => SelectedTypeName);
                OrgsElements = new ObservableCollection<OrgsElementVM>();
                DictOrgsElementVM = new Dictionary<int, ObservableCollection<OrgsElementVM>>();
            }
        }

        void SaveResult()
        {
            foreach (var elem in DictOrgsElementVM)
            {
                var currentElement = elem.Value.Cast<IOrgsElement>().ToList();
                //if (this.SelectedLine != "I20_BG")
                //{
                //    foreach (var item in currentElement)
                //    {
                //        item.OrgsElement.TypNumReq = true;
                //    }
                //}
                //else
                //{
                //    foreach (var item in currentElement)
                //    {
                //        item.OrgsElement.TypNumReq = false;
                //    }
                //}
                //DictOrgsElements.Add(elem.Key, elem.Value.Cast<IOrgsElement>().ToList());
                DictOrgsElements.Add(elem.Key, currentElement);
            }
           
        }

        bool Validate()
        {
            return true;
        }
        #endregion
        #region Commands
        RelayCommand _closingCommand;
        public RelayCommand ClosingCommand
        {
            get
            {
                if (_closingCommand == null)
                {
                    _closingCommand = new RelayCommand(close);
                }
                return _closingCommand;
            }
        }

        RelayCommand _cancelCommand;
        public RelayCommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                {
                    _cancelCommand = new RelayCommand(cancel);
                }
                return _cancelCommand;
            }
        }

        private void cancel()
        {
            var window = Application.Current.Windows
            .Cast<Window>()
            .Single(w => w.DataContext == this);
            window.DialogResult = false;
            window.Close();
        }

        void close()
        {
            if (Validate())
            {
                SaveResult();
                var window = Application.Current.Windows
                .Cast<Window>()
                .Single(w => w.DataContext == this);
                window.DialogResult = true;
                window.Close();
            }
        }


        private ObservableCollection<int> FillOrgNums()
        {
            ObservableCollection<int> result = new ObservableCollection<int>();
            for (int i = 1; i <= 32; i++)
                result.Add(i);
            return result;
        }
        #endregion
    }
}
