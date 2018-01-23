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
            Serializer.ControlersConfiguration controlerConfiguration;
            List<Controler>controlers;
            string destinationPath;
            string sourcePath;

            try
            {
                filesSerialization = new Serializer.FilesSerialization();
                controlerConfiguration = filesSerialization.ReadAplicationConfiguration();
                controlers = new List<Controler>();
                Controler controler;
                FilesOrganizer fileOrganizer;

                if (string.IsNullOrEmpty(controlerConfiguration.SourcePath))
                {
                    throw new NullReferenceException();
                }

                if (string.IsNullOrEmpty(controlerConfiguration.DestinationPath))
                {
                    throw new NullReferenceException();
                }

                sourcePath = controlerConfiguration.SourcePath;
                destinationPath = controlerConfiguration.DestinationPath;

                foreach (var config in controlerConfiguration.Controlers)
                {
                    controler = new Controler();


                    if(string.IsNullOrEmpty(config.ControlerType) ||  string.IsNullOrWhiteSpace(config.ControlerType) || controlers.Exists(x=>x.ContolerType==config.ControlerType))
                    {
                        throw new Exception();
                    }

                    foreach(var files in config.OperationFilters)
                    {
                        fileOrganizer = new FilesOrganizer();

                        fileOrganizer.Action = (GlobalData.Action)files.Action;
                        fileOrganizer.ContainsAtName = files.ContainsAtName;
                        fileOrganizer.DestinationFolder = files.DestinationFolder;
                        fileOrganizer.FileExtensions = files.FilesExtension;
                        fileOrganizer.NotContainsAtName = files.NotContainsAtName;
                        fileOrganizer.RegexContain = files.RegexContain;
                        fileOrganizer.RegexNotContain = files.RegexNotContain;                        

                        controler.Files.Add(fileOrganizer);
                    }                 

                    controlers.Add(controler);
                }
            }
            catch(Exception ex)
            {

            }            

        }
    }
}
