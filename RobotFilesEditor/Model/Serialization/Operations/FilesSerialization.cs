using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;
using System.Threading;

namespace RobotFilesEditor.Serializer
{
    public class FilesSerialization: Serialization
    {
        public FilesSerialization()
        {}        

        public List<RobotFilesEditor.Controler> GetControlersConfigurations()
        {            
            XmlControlersConfiguration controlersConfiguration;
            List<Controler> controlers = new List<Controler>();          
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

                foreach (var xmlControler in controlersConfiguration.Contorolers)
                {
                    controler = new Controler();
                    var controlerType = xmlControler.ControlerType;

                    if (controlers.Exists(x => x.ContolerType == controlerType))
                    {
                        throw new ArgumentException($"Controler type \'{controlerType}\' already exists!" );
                    }

                    controler.ContolerType = controlerType;           

                    foreach (var dataOperation in xmlControler?.DataOperations)
                    {
                        XmlFileOperation fileOperation = xmlControler.FileOperations.FirstOrDefault(x => x.OperationName == dataOperation.FileOperationName && x.Priority == dataOperation.Priority);
                        DataOperation operation = ParseXmlDataOperationToDataOperation(dataOperation, fileOperation);

                        xmlControler.FileOperations.Remove(fileOperation);
                        
                        controler.Operations.Add(operation);
                    }

                    foreach (var filesOperations in xmlControler?.FileOperations)
                    {
                        FileOperation operation = ParseXmlFileOperationToFileOperation(filesOperations);
                        controler.Operations.Add(operation);
                    }


                    if (Directory.Exists(controlersConfiguration.SourcePath) == false)
                    {
                        controler.SourcePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    }
                    else
                    {
                        controler.SourcePath = controlersConfiguration.SourcePath;
                    }

                    if (Directory.Exists(controlersConfiguration.DestinationPath) == false)
                    {
                        controler.DestinationPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    }
                    else
                    {
                        controler.DestinationPath = controlersConfiguration.DestinationPath;
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

            try
            {
                dataFilterGroup.Header = xmlDataFilterGroup.GroupHeader;
                dataFilterGroup.Footer = xmlDataFilterGroup.GroupFooter;
                dataFilterGroup.SpaceBefor = xmlDataFilterGroup.SpaceBeforGroup;
                dataFilterGroup.SpaceAfter = xmlDataFilterGroup.SpaceAfterGroup;
                dataFilterGroup.OnlyRegex = xmlDataFilterGroup.OnlyRegex;

                dataFilterGroup.Filter = ParseXmlFilterToFilter(xmlDataFilterGroup?.Filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }           

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

        private FileOperation ParseXmlFileOperationToFileOperation(XmlFileOperation xmlFileOperation)
        {
            FileOperation fileOperation = new FileOperation();

            try
            {
                fileOperation.ActionType = StringToAction(xmlFileOperation.ActionType);
                fileOperation.OperationName = xmlFileOperation.OperationName;
                fileOperation.DestinationFolder = xmlFileOperation.DestinationFolder;
                fileOperation.Priority = xmlFileOperation.Priority;
                fileOperation.FileExtensions = xmlFileOperation.FilesExtensions;
                fileOperation.NestedSourcePath = xmlFileOperation.NestedSourcePath;
                fileOperation.Filter = ParseXmlFilterToFilter(xmlFileOperation?.Filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }          

            return fileOperation;
        }

        private DataOperation ParseXmlDataOperationToDataOperation(XmlDataOperation xmlDataOperation, XmlFileOperation xmlFileOperation)
        {
            DataOperation operation = new DataOperation();

            try
            {
                operation.FileOperation = ParseXmlFileOperationToFileOperation(xmlFileOperation);
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

                xmlDataOperation.DataFilterGroups.ForEach(x => operation.DataFilterGroups.Add(ParseXmlDataFilterGroupToDataFilterGroup(x)));
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return operation;
        }

    }
}
