using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;

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

                foreach (var xmlControler in controlersConfiguration.Contorolers)
                {
                    controler = new Controler();
                    var controlerType = xmlControler.ControlerType;

                    if (controlers.Exists(x => x.ContolerType == controlerType))
                    {
                        throw new ArgumentException($"Controler type \'{controlerType}\' already exists!" );
                    }

                    controler.ContolerType = controlerType;
                    controler.DestinationPath = destinationPath;
                    controler.SourcePath = sourcePath;

                    foreach (var dataOperation in xmlControler?.DataOperations)
                    {
                        XmlFileOperation fileOperation = xmlControler.FileOperations.FirstOrDefault(x => x.OperationName == dataOperation.FileOperationName && x.Priority == dataOperation.Priority);
                        DataOperation operation = ParseXmlDataOperationToDataOperation(dataOperation, fileOperation, sourcePath, destinationPath);

                        xmlControler.FileOperations.Remove(fileOperation);
                        
                        controler.Operations.Add(operation);
                    }

                    foreach (var filesOperations in xmlControler?.FileOperations)
                    {
                        FileOperation operation = ParseXmlFileOperationToFileOperation(filesOperations, sourcePath, destinationPath);
                        controler.Operations.Add(operation);
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

        private FileOperation ParseXmlFileOperationToFileOperation(XmlFileOperation xmlFileOperation, string source, string desination)
        {
            FileOperation fileOperation = new FileOperation();

            fileOperation.ActionType = StringToAction(xmlFileOperation.ActionType);
            fileOperation.OperationName = xmlFileOperation.OperationName;
            fileOperation.DestinationFolder = xmlFileOperation.DestinationFolder;
            fileOperation.Priority = xmlFileOperation.Priority;
            fileOperation.DestinationPath = desination;
            fileOperation.SourcePath = desination;
            fileOperation.FileExtensions = xmlFileOperation.FilesExtensions;
            fileOperation.NestedSourcePath = xmlFileOperation.NestedSourcePath;
            fileOperation.Filter = ParseXmlFilterToFilter(xmlFileOperation?.Filter);

            return fileOperation;
        }

        private DataOperation ParseXmlDataOperationToDataOperation(XmlDataOperation xmlDataOperation, XmlFileOperation xmlFileOperation, string source, string desination)
        {
            DataOperation operation = new DataOperation();

            operation.FileOperation = ParseXmlFileOperationToFileOperation(xmlFileOperation, source, desination);
            operation.OperationName = xmlDataOperation.FileOperationName;
            operation.DestinationFilePath = xmlDataOperation.DestinationFilePath;
            operation.DestinationFileSource = xmlDataOperation.DestinationFileSource;
            operation.ActionType = StringToAction(xmlDataOperation.ActionType);
            operation.Priority = xmlDataOperation.Priority;
            operation.FileHeader = xmlDataOperation.FileHeader;
            operation.FileFooter = xmlDataOperation.FileFooter;
            operation.GroupSpace = xmlDataOperation.GroupSpace;
            operation.WriteStart = xmlDataOperation.WriteStart;
            operation.WriteStop = xmlDataOperation.WriteStop;
            operation.DestinationPath = desination;
            operation.SourcePath = source;

            xmlDataOperation.DataFilterGroups.ForEach(x => operation.DataFilterGroups.Add(ParseXmlDataFilterGroupToDataFilterGroup(x)));

            return operation;
        }

    }
}
