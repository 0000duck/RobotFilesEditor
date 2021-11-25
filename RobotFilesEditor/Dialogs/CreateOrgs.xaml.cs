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
using System.Windows.Controls.Primitives;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace RobotFilesEditor.Dialogs
{
    /// <summary>
    /// Logika interakcji dla klasy Window1.xaml
    /// </summary>
    /// 

    public partial class CreateOrgs : Window
    {
        CreateOrgsViewModel Vm;
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        public CreateOrgs(CreateOrgsViewModel vm)
        {
            Vm = vm;
            InitializeComponent();
            DataContext = vm;
            this.SizeToContent = SizeToContent.WidthAndHeight;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }

        private void DataGridTemplateColumn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
