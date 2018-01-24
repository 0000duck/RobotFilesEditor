using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RobotFilesEditor
{   
    public partial class App : Application
    {
        //pobranie listy z konfiguracji

        void AppStartup(object sender, StartupEventArgs e)
        {
            MainWindow mainWindow;
            Serializer.FilesSerialization filesSerialization;
            List<Controler>controlers;         

            try
            {
                filesSerialization = new Serializer.FilesSerialization();
                controlers = filesSerialization.GetControlersConfigurations();
                
                if(controlers.Count>0)
                {
                    mainWindow = new RobotFilesEditor.MainWindow(controlers);
                    mainWindow.Show();
                }
                else
                {
                    throw new NullReferenceException("Configuration not contain any controler");
                }
                         
            }
            catch(Exception ex)
            {               
                MessageBoxResult ExeptionMessage = MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);                
            }            

        }
    }
}
