using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace RobotFilesEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            setIconToApp();
        }

        private void setIconToApp()
        {
            Uri icon = new Uri(".../Resources/Harvester.ico", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(icon);
        }
    }
}
