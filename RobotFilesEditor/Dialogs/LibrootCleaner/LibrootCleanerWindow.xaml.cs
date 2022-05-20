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

namespace RobotFilesEditor.Dialogs.LibrootCleaner
{
    /// <summary>
    /// Logika interakcji dla klasy LibrootCleanerWindow.xaml
    /// </summary>
    public partial class LibrootCleanerWindow : Window
    {
        public LibrootCleanerWindow(LibrootCleanerVM vm)
        {
            DataContext = vm;
            InitializeComponent();
        }
    }
}
