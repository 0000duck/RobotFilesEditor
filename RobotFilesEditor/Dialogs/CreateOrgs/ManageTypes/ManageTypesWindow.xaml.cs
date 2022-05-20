using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RobotFilesEditor.Dialogs.CreateOrgs.ManageTypes
{
    /// <summary>
    /// Logika interakcji dla klasy ManageTypesWindow.xaml
    /// </summary>
    public partial class ManageTypesWindow : Window
    {
        ManageTypesVM VM;

        public ManageTypesWindow(ManageTypesVM vm)
        {
            DataContext = vm;
            InitializeComponent();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (((DataContext as ManageTypesVM).lastChange == ManageTypesVM.ListSelection.PLCRename || (DataContext as ManageTypesVM).lastChange == ManageTypesVM.ListSelection.PLCs))
            {
                e.Handled = new System.Text.RegularExpressions.Regex("[^0-9]+").IsMatch(e.Text);
            }
        }
    }
}
