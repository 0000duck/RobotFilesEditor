﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RobKalDat.Views
{
    /// <summary>
    /// Logika interakcji dla klasy BasesUserControl.xaml
    /// </summary>
    public partial class BasesUserControl : UserControl
    {
        public BasesUserControl()
        {
            InitializeComponent();
           // myDataGrid.Columns[0].SortDirection = System.ComponentModel.ListSortDirection.Descending;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[0-9]");
            e.Handled = !regex.IsMatch(e.Text);
        }
    }
}
