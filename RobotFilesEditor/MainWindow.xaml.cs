using RobotFilesEditor.ViewModel;
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
            if (!CommonLibrary.CommonGlobalData.IsAIUTUser)
            {
                MessageBox.Show("You are running this app on not AIUT computer.\r\nApp will close", "Infomation", MessageBoxButton.OK, MessageBoxImage.Information);
                App.Current.MainWindow.Close();
            }            
        }

        private void setIconToApp()
        {
            Uri icon = new Uri("./Resources/Harvester.ico", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(icon);
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            IsEnabled = true;
        }

        public void Close()
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
