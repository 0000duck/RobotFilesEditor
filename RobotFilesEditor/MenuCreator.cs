using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RobotFilesEditor
{
    class MenuCreator
    {
        public MenuItem ControlersChooserMenu;
        public MenuItem FilesMenu;
        public MenuItem OperationsMenu;
        public MenuItem ConfigurationMenu;

        private Controler _controler;
        private List<Controler> _controlers;
        private MenuItem _controlersChooserMenu;
        private MenuItem _filesMenu;
        private MenuItem _operationsMenu;
        private MenuItem _configurationMenu;

        public MenuCreator(Controler controler)
        {

        }

        public MenuCreator(List<Controler>controler)
        {

        }
    }
}
