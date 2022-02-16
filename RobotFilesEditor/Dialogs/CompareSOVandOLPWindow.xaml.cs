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

namespace RobotFilesEditor.Dialogs
{
    /// <summary>
    /// Logika interakcji dla klasy CompareSOVandOLPWindow.xaml
    /// </summary>
    public partial class CompareSOVandOLPWindow : Window
    {
        //CompareSOVandOLPViewModel Vm;

        public CompareSOVandOLPWindow(CompareSOVandOLPViewModel vm)
        {
            DataContext = vm;
            //Vm = vm;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //if (Vm.Items.Count > 0)
            //{
            //    var currentItem = Vm.Items[0];
            //    foreach (var prop in currentItem.GetType().GetProperties())
            //    {

            //        MainDataGridView.Columns.First(x=>x.SortMemberPath == prop.Name).Visibility = Visibility.Collapsed;
            //    }

            //}
        }
    }
}
