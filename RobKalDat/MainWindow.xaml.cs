﻿using System.Windows;
using RobKalDat.ViewModel;

namespace RobKalDat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            ViewModelLocator.Cleanup();    
        }
    }
}