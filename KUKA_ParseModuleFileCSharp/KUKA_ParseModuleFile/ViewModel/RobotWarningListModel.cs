using ParseModuleFile.ANTLR.SrcParser;
using ParseModuleFile.KUKA;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ParseModuleFile.ViewModel
{
    public class RobotWarningListModel : NotifyPropertyChanged
    {

        public static RobotWarningListModel sharedInstance;
        private bool typeUnknown = true;
        private bool typeVariables = true;
        private bool typeProgram_Paths = true;
        private bool typeProgram_Applications = true;
        private bool typeProgram_Flow = true;
        private bool typeOrganizationPrograms = true;
        private bool typeIntern = true;
        private bool typeDeep_Intern = false;
        private bool levelInformation = true;
        private bool levelWarning = true;
        private bool levelFailure = true;

        private ListCollectionView view;
        private Robot robot = new Robot();

        public bool TypeUnknown { get { return typeUnknown; } set { Set(ref typeUnknown, value); } }
        public bool TypeVariables { get { return typeVariables; } set { Set(ref typeVariables, value); } }
        public bool TypeProgram_Paths { get { return typeProgram_Paths; } set { Set(ref typeProgram_Paths, value); } }
        public bool TypeProgram_Applications { get { return typeProgram_Applications; } set { Set(ref typeProgram_Applications, value); } }
        public bool TypeProgram_Flow { get { return typeProgram_Flow; } set { Set(ref typeProgram_Flow, value); } }
        public bool TypeOrganizationPrograms { get { return typeOrganizationPrograms; } set { Set(ref typeOrganizationPrograms, value); } }
        public bool TypeIntern { get { return typeIntern; } set { Set(ref typeIntern, value); } }
        public bool TypeDeep_Intern { get { return typeDeep_Intern; } set { Set(ref typeDeep_Intern, value); } }
        public bool LevelInformation { get { return levelInformation; } set { Set(ref levelInformation, value); } }
        public bool LevelWarning { get { return levelWarning; } set { Set(ref levelWarning, value); } }
        public bool LevelFailure { get { return levelFailure; } set { Set(ref levelFailure, value); } }

        public ListCollectionView View { get { return view; } set { Set(ref view, value); } }

        public KUKA.Programs Programs
        {
            get { return robot.Programs; }
        }

        public Robot Robot
        {
            get { return robot; }
            set
            {
                Set(ref robot, value);
                view = (ListCollectionView) CollectionViewSource.GetDefaultView(robot.Programs);
                //_View.GroupDescriptions.Add(New PropertyGroupDescription("FileName"))
                //_View.GroupDescriptions.Add(New PropertyGroupDescription("Level"))
                //_View.Filter = New Predicate(Of Object)(AddressOf Me.Filter)
                base.OnPropertyChanged("View");
                base.OnPropertyChanged("Programs");
            }
        }

        protected override void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName()] string propertyName = null)
        {
            foreach (ProcedureDefinition Program in robot.Programs)
            {
                //Program.WarningView.Refresh();
            }
            base.OnPropertyChanged(propertyName);
        }

        public RobotWarningListModel()
        {
            sharedInstance = this;
        }
    }

}
