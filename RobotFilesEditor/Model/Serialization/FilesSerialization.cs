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
                        GlobalData.Action actionType;
                        Filter filter;
                        

                        if (Enum.TryParse(filesOperations.ActionType, out actionType))
                        {
                            operation.ActionType = actionType;
                        }
                        else
                        {
                            throw new FormatException(nameof(operation.ActionType));
                        }

                        operation.ActionType = actionType;
                        operation.OperationName = filesOperations.OperationName;
                        operation.DestinationFolder = filesOperations.DestinationFolder;                        
                        operation.Priority = filesOperations.Priority;                        
                        operation.DestinationPath = destinationPath;
                        operation.SourcePath = sourcePath;
                        operation.FileExtensions = filesOperations.FilesExtensions;
                        operation.NestedSourcePath = filesOperations.NestedSourcePath;

                        filter = new Filter();
                        filter.ContainsAtName = filesOperations.Filter?.Contains;
                        filter.NotContainsAtName = filesOperations.Filter?.NotContains;
                        filter.RegexContain = filesOperations.Filter?.RegexContain;
                        filter.RegexNotContain = filesOperations.Filter?.RegexNotContain;

                        operation.Filter = filter;

                        controler.Operations.FilesOperations.Add(operation);
                    }

                    foreach (var filesOperations in controlerXml?.DataOperations)
                    {
                        DataOperation operation = new DataOperation();
                        GlobalData.Action actionType;
                        Filter filter;
                        DataFilterGroup dataFilterGroup;


                        if (Enum.TryParse(filesOperations.ActionType, out actionType))
                        {
                            operation.ActionType = actionType;
                        }
                        else
                        {
                            throw new FormatException(nameof(operation.ActionType));
                        }

                        operation.OperationName = filesOperations.FileOperationName;
                        operation.DestinationFilePath = filesOperations.DestinationFile;
                        operation.ActionType = actionType;
                        operation.Priority = filesOperations.Priority;
                        operation.FileHeader = filesOperations.FileHeader;
                        operation.FileFooter = filesOperations.FileFooter;
                        operation.GroupSpace = filesOperations.GroupSpace;
                        operation.WriteStart = filesOperations.WriteStart;
                        operation.WriteStop = filesOperations.WriteStop;
                        operation.DestinationPath = destinationPath;
                        operation.SourcePath = sourcePath;

                        foreach(XmlDataFilterGroup filterGroup in filesOperations.DataFilterGroups)
                        {
                            dataFilterGroup = new DataFilterGroup();

                            dataFilterGroup.Header = filterGroup.GroupHeader;
                            dataFilterGroup.Footer = filterGroup.GroupFooter;
                            dataFilterGroup.SpaceBefor = filterGroup.SpaceBeforGroup;
                            dataFilterGroup.SpaceAfter = filterGroup.SpaceAfterGroup;

                            filter = new Filter();
                            filter.ContainsAtName = filterGroup.Filter.Contains;
                            filter.NotContainsAtName = filterGroup.Filter.NotContains;
                            filter.RegexContain = filterGroup.Filter.RegexContain;
                            filter.RegexNotContain = filterGroup.Filter.RegexNotContain;
                            dataFilterGroup.Filter = filter;
                            operation.DataFilterGroups.Add(dataFilterGroup);
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
    }
}
