using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using RobotFilesEditor.Model.DataOrganization;
using System.Collections.ObjectModel;

namespace RobotFilesEditor.Dialogs
{
    public class OrgsElementVM : ViewModelBase, IOrgsElement
    {
        #region Fields
        public delegate void SendPropertyHandler(object sender, EventArgs arg);
        public static event SendPropertyHandler SendProperty;
        public OrgsElement OrgsElement { get; set; }
        public enum ChooseType { Anyjob, Usernum, Anyjob_UserNum }
        #endregion

        #region Ctor
        public OrgsElementVM()
        {
            OrgsElement = new OrgsElement();
        }
        #endregion
        protected void OnSendProperty(object sender, EventArgs arg)
        {
            SendProperty?.Invoke(sender, arg);
            if (((OrgsElementVM)sender).Abort == "Home")
                ((OrgsElementVM)sender).AbortNr = 1;
            else
                ((OrgsElementVM)sender).AbortNr = 0;
        }

        public object Clone()
        {
            OrgsElementVM clonedItem = new OrgsElementVM();
            clonedItem.OrgsElement = OrgsElement;
            return clonedItem;
        }

        public string Path
        {
            get { return OrgsElement.Path; }
            set
            {
                if (OrgsElement.Path != value)
                {
                    OrgsElement.Path = value;
                    OrgsElement.AnyJobValue = new ObservableCollection<KeyValuePair<string, int>>();
                    OrgsElement.UserNumValue = new ObservableCollection<KeyValuePair<string, int>>();
                    RaisePropertyChanged("Path");
                    if (value == "ANYJOB")
                    {                        
                        AnyJobValue = Model.Operations.CreateOrgsMethods.CreateUserNumOrAnyJob(Id, ChooseType.Anyjob);
                        RaisePropertyChanged(() => AnyJobValue);
                        OrgsElement.JobAndDescription = "ANYJOB";
                    }
                    else if (value == "USERNUM")
                    {
                        UserNumValue = Model.Operations.CreateOrgsMethods.CreateUserNumOrAnyJob(Id, ChooseType.Usernum);
                        RaisePropertyChanged(() => UserNumValue);
                        OrgsElement.JobAndDescription = "USERNUM";
                    }
                    else if (value == "ANYJOB/USERNUM")
                    {
                        AnyJobUserNumValue = new Dictionary<int, List<KeyValuePair<string, int>>>();
                        AnyJobValue = Model.Operations.CreateOrgsMethods.CreateUserNumOrAnyJob(Id, ChooseType.Anyjob);
                        List<int> jobsFound = new List<int>();
                        foreach (var anyJob in AnyJobValue)
                        {
                            if (!jobsFound.Contains(anyJob.Value))
                            {
                                jobsFound.Add(anyJob.Value);
                                List<KeyValuePair<string, int>> currentAnyJob = Model.Operations.CreateOrgsMethods.CreateUserNumOrAnyJob(Id, ChooseType.Usernum, anyJob.Value).ToList();
                                if (!AnyJobUserNumValue.Keys.Contains(anyJob.Value))
                                    AnyJobUserNumValue.Add(anyJob.Value, currentAnyJob);
                            }
                        }
                        OrgsElement.JobAndDescription = "ANYJOB/USERNUM";
                        
                    }
                    else
                    {
                        OrgsElement.JobAndDescription = "Job " + GlobalData.SrcPathsAndJobs[value].ToString() + ": " + GlobalData.Jobs[GlobalData.SrcPathsAndJobs[value]].ToString();
                        _anyJobValue = null;
                    }
                    RaisePropertyChanged(() => JobAndDescription);
                
                }
            }
        }

        private ObservableCollection<KeyValuePair<string, int>> _anyJobValue;

        public ObservableCollection<KeyValuePair<string, int>> AnyJobValue
        {
            get { return OrgsElement.AnyJobValue; }
            set
            {
                if (OrgsElement.AnyJobValue != value)
                {
                    OrgsElement.AnyJobValue = value;
                    RaisePropertyChanged(() => AnyJobValue);
                    _anyJobValue = value;
                    
                }
            }
        }

        private bool _typNumReq;

        public bool TypNumReq
        {
            get { return OrgsElement.TypNumReq; }
            set
            {
                if (_typNumReq != value)
                {
                    OrgsElement.TypNumReq = value;
                    _typNumReq = value;
                    RaisePropertyChanged(() => TypNumReq);

                }
            }
        }

        private IDictionary<int, List<KeyValuePair<string, int>>> _anyJobUserNumValue;

        public IDictionary<int, List<KeyValuePair<string, int>>> AnyJobUserNumValue
        {
            get { return OrgsElement.AnyJobUserNumValue; }
            set
            {
                if (OrgsElement.AnyJobUserNumValue != value)
                {
                    OrgsElement.AnyJobUserNumValue = value;
                    RaisePropertyChanged(() => AnyJobUserNumValue);
                    _anyJobUserNumValue = value;

                }
            }
        }

        private ObservableCollection<KeyValuePair<string, int>> _userNumValue;
        public ObservableCollection<KeyValuePair<string, int>> UserNumValue
        {
            get { return OrgsElement.UserNumValue; }
            set
            {
                if (OrgsElement.UserNumValue != value)
                {
                    OrgsElement.UserNumValue = value;
                    RaisePropertyChanged(() => UserNumValue);
                    _userNumValue = value;

                }
            }
        }

        public string JobAndDescription
        {
            get { return OrgsElement.JobAndDescription; }
            set
            {
                if (OrgsElement.JobAndDescription != value)
                {
                    OrgsElement.JobAndDescription = value;
                    RaisePropertyChanged("JobAndDescription");
                }
            }
        }

        public string Abort
        {
            get { return OrgsElement.Abort; }
            set
            {
                if (OrgsElement.Abort != value)
                {
                    OrgsElement.Abort = value;
                    RaisePropertyChanged("Abort");
                    OnSendProperty(this, new EventArgs());
                }
            }
        }


        public int? AbortNr
        {
            get { return OrgsElement.AbortNr; }
            set
            {
                if (OrgsElement.AbortNr != value)
                {
                    OrgsElement.AbortNr = value;
                    RaisePropertyChanged(() => AbortNr);
                }
            }
        }

        public string WithPart
        {
            get { return OrgsElement.WithPart; }
            set
            {
                if (OrgsElement.WithPart != value)
                {
                    OrgsElement.WithPart = value;
                    RaisePropertyChanged(() => WithPart);
                }
            }
        }

        public string HomeToCentralPath
        {
            get { return OrgsElement.HomeToCentralPath; }
            set
            {
                if (OrgsElement.HomeToCentralPath != value)
                {
                    OrgsElement.HomeToCentralPath = value;
                    RaisePropertyChanged(() => HomeToCentralPath);
                }
            }
        }

        ObservableCollection<IntItem> _abortNrs;
        public ObservableCollection<IntItem> AbortNrs
        {
            get { return _abortNrs; }
            set
            {
                if (_abortNrs != value)
                {
                    _abortNrs = value;
                    RaisePropertyChanged("AbortNrs");
                }
            }
        }

        ObservableCollection<TextItem> _homeToCentral;
        public ObservableCollection<TextItem> HomeToCentral
        {
            get { return _homeToCentral; }
            set
            {
                if (_homeToCentral != value)
                {
                    _homeToCentral = value;
                    RaisePropertyChanged("HomeToCentral");
                }
            }
        }

        ObservableCollection<TextItem> _withParts;
        public ObservableCollection<TextItem> WithParts
        {
            get { return _withParts; }
            set
            {
                if (_withParts != value)
                {
                    _withParts = value;
                    RaisePropertyChanged("WithParts");
                }
            }
        }

        public int? Id
        {
            get { return OrgsElement.Id; }
            set
            {
                if (OrgsElement.Id != value)
                {
                    OrgsElement.Id = value;
                    RaisePropertyChanged("Id");
                }
            }
        }


    }
}
