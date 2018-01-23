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
               
            }
            catch(Exception ex)
            {
                throw ex;
            }            

        }
    }
}
