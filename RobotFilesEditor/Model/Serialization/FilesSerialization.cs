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

        public XmlControlersConfiguration ReadAplicationConfiguration()
        {            
            XmlControlersConfiguration controlerConfiguration;

            if (String.IsNullOrEmpty(GlobalData.ConfigurationFileName))
            {
                throw new ArgumentNullException();
            }

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(XmlControlersConfiguration));
                FileStream fileStream = new FileStream(GlobalData.ConfigurationFileName, FileMode.Open);
                XmlReader reader = XmlReader.Create(fileStream); 
                controlerConfiguration = (XmlControlersConfiguration)serializer.Deserialize(reader);
                fileStream.Close();                
            }catch(Exception ex)
            {
                throw ex;
            }
            return controlerConfiguration;
        }

        public List<RobotFilesEditor.Controler> GetControlersConfigurations()
        {            
            XmlControlersConfiguration controlersConfiguration;
            List<Controler> controlers = new List<Controler>();
            string destinationPath;
            string sourcePath;
            Controler controler;

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

                foreach (var controlerXml in controlersConfiguration.Contorolers)
                {
                    controler = new Controler();
                    var controlerType = controlerXml.ControlerType;

                    if (controlers.Exists(x => x.ContolerType == controlerType))
                    {
                        throw new ArgumentException($"Controler type \'{controlerType}\' already exists!" );
                    }

                    controler.ContolerType = controlerType;
                    controler.DestinationPath = destinationPath;
                    controler.SourcePath = sourcePath;

                    foreach(var filesOperations in controlerXml?.FileOperations)
                    {
                        FileOperation operation = new FileOperation();

                        operation.ActionType = StringToAction(filesOperations.ActionType);
                        operation.OperationName = filesOperations.OperationName;
                        operation.DestinationFolder = filesOperations.DestinationFolder;                        
                        operation.Priority = filesOperations.Priority;                        
                        operation.DestinationPath = destinationPath;
                        operation.SourcePath = sourcePath;
                        operation.FileExtensions = filesOperations.FilesExtensions;
                        operation.NestedSourcePath = filesOperations.NestedSourcePath;
                        operation.Filter = ParseXmlFilterToFilter(filesOperations?.Filter);

                        controler.Operations.FilesOperations.Add(operation);
                    }

                    foreach (var dataOperations in controlerXml?.DataOperations)
                    {
                        DataOperation operation = new DataOperation();                   

                        operation.OperationName = dataOperations.FileOperationName;
                        operation.DestinationFilePath = dataOperations.DestinationFilePath;
                        operation.DestinationFileSource = dataOperations.DestinationFileSource;
                        operation.ActionType = StringToAction(dataOperations.ActionType);
                        operation.Priority = dataOperations.Priority;
                        operation.FileHeader = dataOperations.FileHeader;
                        operation.FileFooter = dataOperations.FileFooter;
                        operation.GroupSpace = dataOperations.GroupSpace;
                        operation.WriteStart = dataOperations.WriteStart;
                        operation.WriteStop = dataOperations.WriteStop;                        
                        operation.DestinationPath = destinationPath;
                        operation.SourcePath = sourcePath;
                        

                        foreach (XmlDataFilterGroup filterGroup in dataOperations.DataFilterGroups)
                        {
                            operation.DataFilterGroups.Add(ParseXmlDataFilterGroupToDataFilterGroup(filterGroup));
                        }

                        controler.Operations.DataOperations.Add(operation);
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

        private Filter ParseXmlFilterToFilter(XmlFilter xmlFilter)
        {
            Filter filter = new Filter();

            filter.Contain = xmlFilter?.Contains;
            filter.NotContain = xmlFilter?.NotContains;
            filter.RegexContain = xmlFilter?.RegexContain;
            filter.RegexNotContain = xmlFilter?.RegexNotContain;

            return filter;
        }

        private DataFilterGroup ParseXmlDataFilterGroupToDataFilterGroup(XmlDataFilterGroup xmlDataFilterGroup)
        {
            DataFilterGroup dataFilterGroup = new DataFilterGroup();           

            dataFilterGroup.Header = xmlDataFilterGroup.GroupHeader;
            dataFilterGroup.Footer = xmlDataFilterGroup.GroupFooter;
            dataFilterGroup.SpaceBefor = xmlDataFilterGroup.SpaceBeforGroup;
            dataFilterGroup.SpaceAfter = xmlDataFilterGroup.SpaceAfterGroup;
            dataFilterGroup.OnlyRegex = xmlDataFilterGroup.OnlyRegex;

            dataFilterGroup.Filter = ParseXmlFilterToFilter(xmlDataFilterGroup?.Filter);

            return dataFilterGroup;
        }

        private GlobalData.Action StringToAction(string action)
        {
            GlobalData.Action actionType;

            if (Enum.TryParse(action, out actionType))
            {
                return actionType;
            }
            else
            {
                throw new FormatException(nameof(action));
            }            
        }

    }
}
