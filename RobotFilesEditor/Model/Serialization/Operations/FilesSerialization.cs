﻿using RobotFilesEditor.Model.Operations;
using RobotFilesEditor.Model.Serialization.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace RobotFilesEditor.Serializer
{
    public class FilesSerialization: Serialization
    {
        private static string _username;

        public FilesSerialization()
        {}        

        public List<RobotFilesEditor.Controler> GetControlersConfigurations()
        {
            ApplictionConfigCreator();
            LoadFilePaths();
            XmlControlersConfiguration controlersConfiguration;
            List<Controler> controlers = new List<Controler>();          
            Controler controler;

            try
            {                
                controlersConfiguration = ReadAplicationConfiguration();
                Model.XML.DestAndSource destandsource = GetPath();
                controlersConfiguration.DestinationPath = destandsource.DestPath;
                controlersConfiguration.SourcePath = destandsource.SourcePath; 

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
                        if (fileOperation != null)
                        {
                            DataOperation operation = ParseXmlDataOperationToDataOperation(dataOperation, fileOperation);

                            xmlControler.FileOperations.Remove(fileOperation);

                            controler.Operations.Add(operation);
                        }
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
                    return controlers;
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                throw ex;
            }           
        }

        private Model.XML.DestAndSource GetPath()
        {
            string destPath = "", sourcePath = "";
            XmlDocument document = new XmlDocument();
            document.Load(GlobalData.PathFile);
            XmlNodeList nodeList = document.SelectNodes("/Paths");
            foreach (XmlNode node in nodeList)
            {
                destPath = node.SelectSingleNode("DestinationPath").InnerText;
                sourcePath = node.SelectSingleNode("SourcePath").InnerText;
            }
            return new Model.XML.DestAndSource(destPath,sourcePath);
            
        }

        private void LoadFilePaths()
        {            
            string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            Regex regex = new Regex(@"(?<=.*\\).*", RegexOptions.IgnoreCase);
            Match match = regex.Match(userName);
            GlobalData.PathFile = "C:\\Users\\" + match.ToString() + "\\AppData\\Local\\RobotFilesHarvester\\FilePaths.xml";
            if (!File.Exists(GlobalData.PathFile))
            {
                Model.XML.WriteFilePaths.WriteFile(GlobalData.PathFile);
            }
        }

        public static void ApplictionConfigCreator(bool isDefault = false)
        {
            string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            Regex regex = new Regex(@"(?<=.*\\).*", RegexOptions.IgnoreCase);
            Match match = regex.Match(userName);
            _username = match.ToString();

            string[] readLines = File.ReadAllLines("Application.config");
            if (!Directory.Exists("C:\\Users\\" + _username + "\\AppData\\Local\\RobotFilesHarvester"))
                Directory.CreateDirectory("C:\\Users\\" + _username + "\\AppData\\Local\\RobotFilesHarvester");
            File.WriteAllLines("C:\\Users\\" + _username + "\\AppData\\Local\\RobotFilesHarvester\\Application.config", readLines);
        }

        private Filter ParseXmlFilterToFilter(XmlFilter xmlFilter)
        {
            Filter filter = new Filter();

            try
            {
                filter.Contain = xmlFilter?.Contains;
                filter.NotContain = xmlFilter?.NotContains;
                filter.RegexContain = xmlFilter?.RegexContain;
                filter.RegexNotContain = xmlFilter?.RegexNotContain;
                return filter;
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                throw ex;
            }
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
                dataFilterGroup.TextBefore = xmlDataFilterGroup.TextBefore;

                dataFilterGroup.Filter = ParseXmlFilterToFilter(xmlDataFilterGroup?.Filter);
                return dataFilterGroup;
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                throw ex;
            }
        }

        private GlobalData.Action StringToAction(string action)
        {
            GlobalData.Action actionType;

            if (string.IsNullOrEmpty(action))
            {
                return GlobalData.Action.None;
            }

            if (Enum.TryParse(action, out actionType))
            {
                return actionType;
            }
            else
            {
                throw new FormatException(nameof(action));
            }            
        }

        private GlobalData.SortType StringToSortType(string type)
        {
            GlobalData.SortType sortType;
            if(string.IsNullOrEmpty(type))
            {
                return GlobalData.SortType.None;
            }

            if (Enum.TryParse(type, out sortType))
            {
                return sortType;
            }
            else
            {
                throw new FormatException(nameof(type));
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
                return fileOperation;
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                throw ex;
            }         
        }

        private DataOperation ParseXmlDataOperationToDataOperation(XmlDataOperation xmlDataOperation, XmlFileOperation xmlFileOperation)
        {
            DataOperation operation = new DataOperation();

            try
            {
                operation.FileOperation = ParseXmlFileOperationToFileOperation(xmlFileOperation);
                operation.OperationName = xmlDataOperation.FileOperationName;
                operation.DestinationFileSource = xmlDataOperation.DestinationFileSource;
                operation.ActionType = StringToAction(xmlDataOperation.ActionType);
                operation.SortType = StringToSortType(xmlDataOperation.SortType);
                operation.Priority = xmlDataOperation.Priority;
                operation.FileHeader = xmlDataOperation.FileHeader;
                operation.FileFooter = xmlDataOperation.FileFooter;
                operation.GroupSpace = xmlDataOperation.GroupSpace;
                operation.WriteStart = xmlDataOperation.WriteStart;
                operation.WriteStop = xmlDataOperation.WriteStop;
                operation.DetectDuplicates = xmlDataOperation.DetectDuplicates;

                xmlDataOperation.DataFilterGroups.ForEach(x => operation.DataFilterGroups.Add(ParseXmlDataFilterGroupToDataFilterGroup(x)));
                return operation;
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                throw ex;
            }

            
        }

    }
}
