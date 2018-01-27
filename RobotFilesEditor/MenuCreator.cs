using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RobotFilesEditor
{
    class MenuCreator: INotifyPropertyChanged
    {
        public List<Controler> Controlers
        {
            get { return _controlers; }
            set
            {
                if (_controlers != value)
                {
                    _controlers = value;
                    OnPropertyChanged(nameof(Controlers));
                }
            }
        }
        public Menu MainMenu
        {
            get { return _mainMenu; }
            set
            {
                if(value!=_mainMenu && value!=null)
                {
                    _mainMenu = value;
                    OnPropertyChanged(nameof(MainMenu));
                }
            }
        }
        public MenuItem ControlersChooserMenu
        {
            get{ return _controlersChooserMenu; }
            set
            {
                if (value != null && value!=_controlersChooserMenu)
                {
                    _controlersChooserMenu = value;
                    //OnPropertyChanged(nameof(ControlersChooserMenu));
                    OnPropertyChanged(nameof(ControlersChooserMenu));                  
                }
            }
        }
        public MenuItem FilesMenu;
        public MenuItem OperationsMenu
        {
            get { return _operationsMenu; }
            set
            {
                if(_operationsMenu!=value)
                {
                    _operationsMenu = value;
                    //OnPropertyChanged(nameof(OperationsMenu));
                    OnPropertyChanged(nameof(OperationsMenu));                    
                }
            }
        }
        public MenuItem ConfigurationMenu;
        
        public event PropertyChangedEventHandler PropertyChanged;
        
        private Controler _controler;
        private List<Controler> _controlers;
        private Menu _mainMenu;
        private MenuItem _controlersChooserMenu;
        private MenuItem _filesMenu;
        private MenuItem _operationsMenu;
        private MenuItem _configurationMenu;      

        public MenuCreator(Controler controler)
        {
        }

        public MenuCreator(ref List<Controler>controlers)
        {
            if(controlers.Count>0)
            {
                Controlers = controlers;
            }else
            {
                throw new ArgumentNullException(nameof(Controlers));
            }

            _controler = Controlers.FirstOrDefault();
            CreateControlersChooserMenu();
            CreateOperationsMenu();
            //CreateMainMenu();
        }

        [NotifyPropertyChangedInvocatorAttribute]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        void CreateMainMenu()
        {
            MainMenu = new Menu();

            if(ControlersChooserMenu!=null)
            {
                MainMenu.Items.Add(ControlersChooserMenu);
            }

            if (FilesMenu != null)
            {
                MainMenu.Items.Add(FilesMenu);
            }

            if (OperationsMenu != null)
            {
                MainMenu.Items.Add(OperationsMenu);
            }

            if (ConfigurationMenu != null)
            {
                MainMenu.Items.Add(ConfigurationMenu);
            }
        }

        private void CreateControlersChooserMenu()
        {            
            MenuItem menuRoot = new MenuItem();         
           
            foreach(var controler in Controlers)
            {
                MenuItem menuNode = new MenuItem();

                menuNode.Header = controler.ContolerType;
                menuNode.IsCheckable = true;
                menuNode.Click += ControlersChooser_Click;
                menuRoot.Items.Add(menuNode);
            }
            ControlersChooserMenu = menuRoot;
        }       

        private void CreateOperationsMenu()
        {
            MenuItem menuRoot = new MenuItem();
            MenuItem menuLeaf;
            List<MenuItem> actionsNodes=new List<MenuItem>();
            List<FilesFilter> filters = _controler.FilesFilters.OrderBy(x => x.Action).ToList();

            foreach (var filter in _controler.FilesFilters)
            {
                var actualAction = filter.Action.ToString();
                var nodeIndex = -1;

                menuLeaf = new MenuItem();
                menuLeaf.Header = filter.OperationName;
                menuLeaf.Click += OperationsMenu_Click;

                if (actionsNodes.Exists(x=>x.Header.ToString()==actualAction)==false)
                {
                    actionsNodes.Add(new MenuItem() {Header = filter.Action.ToString()});

                    nodeIndex = actionsNodes.IndexOf(actionsNodes.Where(x => x.Header.ToString() == actualAction).FirstOrDefault());

                    if (nodeIndex != -1)
                    {
                        actionsNodes[nodeIndex].Items.Add($"All {actualAction} operations");
                        actionsNodes[nodeIndex].Items.Add(new Separator());
                    }
                }

                if(nodeIndex<0)
                {
                    nodeIndex = actionsNodes.IndexOf(actionsNodes.Where(x => x.Header.ToString() == actualAction).FirstOrDefault());
                }               

                if (actionsNodes[nodeIndex]!=null)
                {
                    actionsNodes[nodeIndex].Items.Add(menuLeaf);
                }                
            }

            actionsNodes.ForEach(x => menuRoot.Items.Add(x));

            OperationsMenu = menuRoot;
        }      

        private void ControlersChooser_Click(object sender, System.Windows.RoutedEventArgs e)
        {            
            Controlers[0].DestinationPath = @"C:\Users\ajergas\Documents\KRC2_20180122";
            OnPropertyChanged(nameof(Controlers));
        }

        private void OperationsMenu_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Controlers[1].DestinationPath = @"C:\Users\ajergas\Documents\KRC2_20180122";
            OnPropertyChanged(nameof(Controlers));
        }
    }
}
