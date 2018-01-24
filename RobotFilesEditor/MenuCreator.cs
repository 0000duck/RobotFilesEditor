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
        public Menu MainMenu
        {
            get { return _mainMenu; }
            set
            {
                if(value!=_mainMenu && value!=null)
                {
                    _mainMenu = value;
                    OnPropertyChanged(nameof(MainWindow.MainMenu));
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
                    OnPropertyChanged(nameof(ControlersChooserMenu));
                }
            }
        }
        public MenuItem FilesMenu;
        public MenuItem OperationsMenu;
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

        public MenuCreator(List<Controler>controlers)
        {
            if(controlers.Count>0)
            {
                _controlers = controlers;
            }else
            {
                throw new ArgumentNullException(nameof(_controlers));
            }

            _controler = _controlers.FirstOrDefault();
            CreateControlersChooserMenu();
            CreateOperationsMenu();
            CreateMainMenu();
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

            menuRoot.Header = "Controlers";           
           
            foreach(var controler in _controlers)
            {
                MenuItem menuNode = new MenuItem();

                menuNode.Header = controler.ContolerType;
                menuNode.IsCheckable = true;
                menuRoot.Items.Add(menuNode);
            }

            ControlersChooserMenu = menuRoot;
        }

        private void CreateOperationsMenu()
        {
            MenuItem menuRoot = new MenuItem();
            List<MenuItem> actionsNodes=new List<MenuItem>();
            List<FilesOrganizer> filters = _controler.FilesFilters.OrderBy(x => x.Action).ToList();

            menuRoot.Header = "Operations";

            foreach (var filter in _controler.FilesFilters)
            {
                var actualAction = filter.Action.ToString();
                var nodeIndex = -1;

                if (actionsNodes.Exists(x=>x.Header.ToString()==actualAction)==false)
                {
                    actionsNodes.Add(new MenuItem() {Header = filter.Action.ToString()});
                    nodeIndex = actionsNodes.IndexOf(actionsNodes.Where(x => x.Header.ToString() == actualAction).FirstOrDefault());

                    if (nodeIndex != -1)
                    {
                        actionsNodes[nodeIndex].Items.Add($"All {actualAction} operations");
                    }                    
                }

                if(actionsNodes[nodeIndex]!=null)
                {
                    actionsNodes[nodeIndex].Items.Add(filter.OperationName);
                }                
            }

            actionsNodes.ForEach(x => menuRoot.Items.Add(x));

            OperationsMenu = menuRoot;
        }
    }
}
