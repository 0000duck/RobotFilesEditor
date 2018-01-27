using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace RobotFilesEditor.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        #region Controls
        #region MenuControls
        public MenuItem ControlersChooserMenu
        {
            get{ return _controlersChooserMenu;}
            set
            {
                if (_controlersChooserMenu != value)
                {
                    _controlersChooserMenu = value;
                    RaisePropertyChanged(nameof(ControlersChooserMenu));
                }
            }
        }
        public MenuItem OperationsMenu
        {
            get { return _operationsMenu; }
            set
            {
                if (_operationsMenu != value)
                {
                    _operationsMenu = value;
                    RaisePropertyChanged(nameof(OperationsMenu));
                }
            }
        }
        public Controler SelectedControler
        {
            get { return _selectedControler; }
            set
            {
                if (_selectedControler != value)
                {
                    _selectedControler = value;
                    RaisePropertyChanged(nameof(SelectedControler));
                }
            }
        }
        public string SelectedOperation
        {
            get { return _selectedOperation; }
            set
            {
                if (_selectedOperation != value)
                {
                    _selectedOperation = value;
                    RaisePropertyChanged(nameof(SelectedOperation));
                }
            }
        }
        #endregion MenuControls
        #endregion Controls

        #region Commands      
        public List<ICommand>ChooseControlerCommand
        {
            get;
            set;
        }
        public ICommand SelectOperationCommand
        {
            get;
            set;
        }
      
        #endregion Commands

        public List<Controler> Controlers
        {
            get { return _controlers; }
            set
            {
                if (_controlers != value)
                {
                    _controlers = value;
                    RaisePropertyChanged(nameof(Controlers));
                }
            }
        }

        private List<Controler> _controlers;
        private MenuItem _controlersChooserMenu;     
        private MenuItem _operationsMenu;
        private Controler _selectedControler;
        private string _selectedOperation;            

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocatorAttribute]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainViewModel(List<Controler> controlers)
        {
            //_menuCreator = new MenuCreator(ref controlers);
            //ControlersChooserMenu = _menuCreator.ControlersChooserMenu;
            //OperationsMenu = _menuCreator.OperationsMenu;
            //InitializeComponent();      
            SelectOperationCommand = new RelayCommand(()=>SelectControler("Działa"));

            ChooseControlerCommand = new List<ICommand>();
            Controlers = controlers;
            CreateControlersChooserMenu();            
        }

        #region ControlersChooserMenuMethodRegion
        public void SelectControler(string selectedControler)
        {
            //CreateOperationsMenu();           
            SelectedOperation = selectedControler;
        }
        #endregion ControlersChooserMenuMethodRegion

        #region OperationsMenuMethodRegion
        private void SelectOperation(string selectedOperation)
        {

        }

        #endregion OperationsMenuMenuMethodRegion

        #region MenuCreator
        private void CreateControlersChooserMenu()
        {
            MenuItem menuRoot = new MenuItem();

            foreach (var controler in Controlers)
            {
                MenuItem menuNode = new MenuItem();
                var controlerType= controler.ContolerType;
                menuNode.Header = controlerType;
                menuNode.IsCheckable = false;
                ChooseControlerCommand.Add(new RelayCommand(() => SelectControler(controlerType)));
                menuNode.Command = ChooseControlerCommand.Last();
                menuRoot.Items.Add(menuNode);
            }
            ControlersChooserMenu = menuRoot;
        }

        private void CreateOperationsMenu()
        {
            MenuItem menuRoot = new MenuItem();
            MenuItem menuLeaf;
            List<MenuItem> actionsNodes = new List<MenuItem>();
            List<FilesFilter> filters = SelectedControler.FilesFilters.OrderBy(x => x.Action).ToList();

            foreach (var filter in SelectedControler.FilesFilters)
            {
                var actualAction = filter.Action.ToString();
                var nodeIndex = -1;
                var operationName = filter.OperationName;

                menuLeaf = new MenuItem();
                menuLeaf.Header = filter.OperationName;
                menuLeaf.CommandParameter = filter.OperationName;
                menuLeaf.Command = new RelayCommand(()=>SelectOperation(operationName));

                if (actionsNodes.Exists(x => x.Header.ToString() == actualAction) == false)
                {
                    actionsNodes.Add(new MenuItem() { Header = filter.Action.ToString() });

                    nodeIndex = actionsNodes.IndexOf(actionsNodes.Where(x => x.Header.ToString() == actualAction).FirstOrDefault());

                    if (nodeIndex != -1)
                    {
                        menuLeaf = new MenuItem();
                        operationName = $"All {actualAction} operations";
                        menuLeaf.Header = operationName;
                        menuLeaf.CommandParameter = filter.OperationName;
                        menuLeaf.Command = new RelayCommand(()=>SelectOperation(operationName));

                        actionsNodes[nodeIndex].Items.Add(menuLeaf);                        
                        actionsNodes[nodeIndex].Items.Add(new Separator());
                    }
                }

                if (nodeIndex < 0)
                {
                    nodeIndex = actionsNodes.IndexOf(actionsNodes.Where(x => x.Header.ToString() == actualAction).FirstOrDefault());
                }

                if (actionsNodes[nodeIndex] != null)
                {
                    actionsNodes[nodeIndex].Items.Add(menuLeaf);
                }
            }

            actionsNodes.ForEach(x => menuRoot.Items.Add(x));

            OperationsMenu = menuRoot;
        }
        #endregion MenuCreator
    }
}