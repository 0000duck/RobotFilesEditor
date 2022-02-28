using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RobotFilesEditor.Dialogs.SelectCollision
{
    /// <summary>
    /// Logika interakcji dla klasy SelectCollisionFromDuplicate.xaml
    /// </summary>
    public partial class SelectCollisionFromDuplicate : Window
    {
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public SelectCollisionFromDuplicate(SelectColisionViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            //if (this.CollDescriptionsReq.Items.Count > 0)
            //    this.CollDescriptionsReq.SelectedIndex = 0;
            this.CollDescriptionsReq.LostFocus += (s, e) => this.CollDescriptionsReq.UnselectAll();
            this.CollDescriptionsClr.LostFocus += (s, e) => this.CollDescriptionsClr.UnselectAll();

        }

        private void CollDescrOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.CollDescriptionsReq.UnselectAll();
            this.CollDescriptionsClr.UnselectAll();
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }
    }
}
