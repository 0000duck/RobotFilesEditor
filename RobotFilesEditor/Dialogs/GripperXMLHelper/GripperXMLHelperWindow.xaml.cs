using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RobotFilesEditor.Dialogs.GripperXMLHelper
{
    /// <summary>
    /// Logika interakcji dla klasy GripperXMLHelperWindow.xaml
    /// </summary>
    public partial class GripperXMLHelperWindow : Window
    {
        public GripperXMLHelperWindow(GripperXMLHelperVM vM)
        {
            InitializeComponent();
            DataContext = vM;
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }
    }
}
