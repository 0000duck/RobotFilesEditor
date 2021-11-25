using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
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

namespace RobotPointsRenumber.View
{
    /// <summary>
    /// Logika interakcji dla klasy RenameWindow.xaml
    /// </summary>
    public partial class RenameWindow : Window
    {
        RenameWindowViewModel vM;
        List<string> tempList;
        string selectedItem;
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);


        public RenameWindow(RenameWindowViewModel vm)
        {
            InitializeComponent();
            var aiutLogo = RobotPointsRenumber.Properties.Resources.aiut;
            aiut_img.Source = ImageSourceFromBitmap(aiutLogo);
            vM = vm;
            DataContext = vM;
        }

        private int CountUsages(string selectedItem)
        {
            if (this.MyListView.ItemsSource == null)
                return 0;
            int counter = 0;
            foreach (var item in this.MyListView.ItemsSource)
            {
                if (selectedItem.ToLower() == item.ToString().ToLower())
                    counter++;
            }
            return counter;

        }

        private string GetSelectedItem(IList points, out bool isIncrement)
        {
            isIncrement = false;
            string returnDouble = string.Empty ;
            if (this.MyListView.SelectedItems.Count < vM.SelectedPointsList.Count)
            {
                foreach (var item in vM.SelectedPointsList)
                {
                    if (CheckDouble((string)item, out returnDouble))
                        return returnDouble;
                    if (!this.MyListView.SelectedItems.Contains(item))
                        return (string)item;
                }
            }
            else
            {
                isIncrement = true;
                foreach (var item in this.MyListView.SelectedItems)
                {
                    if (!vM.SelectedPointsList.Contains(item))
                        return (string)item;
                }
            }
            return "";
        }

        private bool CheckDouble(string item,out string returnDouble)
        {
            int countusages = CountUsages(item);
            returnDouble = item;
            foreach (var point in this.MyListView.SelectedItems)
            {
                if ((point as string).ToLower() == item.ToLower())
                    countusages--;
            }
            if (countusages > 0)
                return true;
            return false; 
                
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        private void MyListView_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            List<string> points = GetStringsFromList(this.MyListView.SelectedItems);
            bool isIncrement;
            selectedItem = GetSelectedItem(points, out isIncrement);
            int timesUsed = CountUsages(selectedItem);
            tempList = new List<string>();
            if (timesUsed > 1)
            {
                int counter = 0;
                foreach (var item in points)
                {
                    if ((item as string).ToLower() == selectedItem.ToLower())
                    {
                        for (int i = 1; i <= timesUsed; i++)
                        {
                            if (isIncrement)
                            {
                                tempList.Add((string)item);
                                if (counter > 0)
                                    this.MyListView.SelectedItems.Add(item);
                            }
                            else
                            {
                                foreach (var pt in points)
                                {

                                    if (counter > 0 && (string)pt.ToLower() == selectedItem.ToLower())
                                        this.MyListView.SelectedItems.Remove(item);
                                }
                            }
                            counter++;
                        }
                    }
                    else
                        tempList.Add((string)item);
                }
            }
            else
            {
                foreach (var item in points)
                    tempList.Add((string)item);
            }
            vM.SelectedPointsList = tempList;
        }

        private List<string> GetStringsFromList(IList selectedItems)
        {
            List<string> result = new List<string>();
            foreach (var item in selectedItems)
            {
                result.Add((string)item);
            }
            return result;
        }

        public ImageSource ImageSourceFromBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }
    }

}

