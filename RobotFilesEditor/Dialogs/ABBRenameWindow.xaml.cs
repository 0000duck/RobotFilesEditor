using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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

namespace RobotFilesEditor.Dialogs
{
    /// <summary>
    /// Logika interakcji dla klasy ABBRenameWindow.xaml
    /// </summary>
    public partial class ABBRenameWindow : Window
    {
        ABBRenameViewModel vM;

        public ABBRenameWindow(ABBRenameViewModel vm)
        {
            InitializeComponent();
            vM = vm;
            DataContext = vM;
        }

        private void MyListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var points = this.MyListView.SelectedItems;
            string selectedItem = GetSelectedItem(points);
            //int timesUsed = CountUsages(selectedItem);
            List<string> tempList = new List<string>();
            foreach (var point in points)
                tempList.Add((string)point);

            vM.SelectedPointsList = tempList;
        }

        //private int CountUsages(string selectedItem)
        //{
        //    foreach (var item in this.)
        //}

        private string GetSelectedItem(IList points)
        {
            if (this.MyListView.SelectedItems.Count < vM.SelectedPointsList.Count)
            {
                foreach (var item in vM.SelectedPointsList)
                {
                    if (!this.MyListView.SelectedItems.Contains(item))
                        return (string)item;
                }
            }
            else
            {
                foreach (var item in this.MyListView.SelectedItems)
                {
                    if (!vM.SelectedPointsList.Contains(item))
                        return (string)item;
                }
            }
            return "";
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }
    }
}
