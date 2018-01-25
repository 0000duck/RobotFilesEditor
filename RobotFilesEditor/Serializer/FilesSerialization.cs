using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace RobotFilesEditor.Serializer
{
    public class FilesSerialization
    {
        public FilesSerialization()
        {}

        public ControlersConfiguration ReadAplicationConfiguration()
        {            
            ControlersConfiguration controlerConfiguration;

            if (String.IsNullOrEmpty(GlobalData.ConfigurationFileName))
            {
                throw new ArgumentNullException();
            }

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ControlersConfiguration));
                FileStream fileStream = new FileStream(GlobalData.ConfigurationFileName, FileMode.Open);
                XmlReader reader = XmlReader.Create(fileStream); 
                controlerConfiguration = (ControlersConfiguration)serializer.Deserialize(reader);
                fileStream.Close();                
            }catch(Exception ex)
            {
                throw ex;
            }
            return controlerConfiguration;
        }

        public List<RobotFilesEditor.Controler> GetControlersConfigurations()
        {            
            Serializer.ControlersConfiguration controlersConfiguration;
            List<RobotFilesEditor.Controler> controlers = new List<RobotFilesEditor.Controler>();
            string destinationPath;
            string sourcePath;
            RobotFilesEditor.Controler controler;
            FilesFilter fileOrganizer;

            try
            {                
                controlersConfiguration = ReadAplicationConfiguration();               

                if (string.IsNullOrEmpty(controlersConfiguration.SourcePath))
                {
                    throw new NullReferenceException();
                }

                if (string.IsNullOrEmpty(controlersConfiguration.DestinationPath))
                {
                    throw new NullReferenceException();
                }

                sourcePath = controlersConfiguration.SourcePath;
                destinationPath = controlersConfiguration.DestinationPath;

                foreach (var controlersArray in controlersConfiguration.Contorolers)
                {
                    controler = new RobotFilesEditor.Controler();
                    var controlerType = controlersArray.ControlerType;

                    if (controlers.Exists(x => x.ContolerType == controlerType))
                    {
                        throw new ArgumentException($"Controler type \'{controlerType}\' already exists!" );
                    }

                    controler.ContolerType = controlerType;
                    controler.DestinationPath = destinationPath;
                    controler.SourcePath = sourcePath;

                    foreach (var files in controlersArray?.OperationFilters)
                    {
                        fileOrganizer = new FilesFilter();
                        var operationName = files.OperationName;
                        GlobalData.Action action;

                        if (controler.FilesFilters.Exists(x => x.OperationName == operationName))
                        {
                            throw new ArgumentException($"Controler operation \'{operationName}\' already exists!");
                        }
                                              
                        fileOrganizer.OperationName = files.OperationName;
                        if(Enum.TryParse(files.Action, out action))
                        {
                            fileOrganizer.Action = action;
                        }else
                        {
                            throw new FormatException(nameof(files.Action));
                        }

                        fileOrganizer.Filter.ContainsAtName = files.ContainsAtName;
                        fileOrganizer.DestinationFolder = files.DestinationFolder;
                        fileOrganizer.FileExtensions = files.FilesExtension;
                        fileOrganizer.Filter.NotContainsAtName = files.NotContainsAtName;
                        fileOrganizer.Filter.RegexContain = files.RegexContain;
                        fileOrganizer.Filter.RegexNotContain = files.RegexNotContain;

                        controler.FilesFilters.Add(fileOrganizer);
                    }
                    controlers.Add(controler);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return controlers;
        }
    }
}
