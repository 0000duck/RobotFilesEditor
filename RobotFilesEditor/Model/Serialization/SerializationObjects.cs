using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace RobotFilesEditor.Serializer
{
    [Serializable]
    [XmlRoot("ControlersConfiguration", Namespace = "RobotFilesEditor")]
    public class ControlersConfiguration
    {
        [XmlAttribute("DestinationPath")]
        public string DestinationPath { get; set; }

        [XmlAttribute("SourcePath")]
        public string SourcePath { get; set; }

        [XmlArray("ContorolersArray")]
        [XmlArrayItem("Controler")]
        public List<XmlControler> Contorolers { get; set; }
    }

    [Serializable]
    [XmlRoot("ControlersConfiguration")]
    public class XmlControler
    {
        [XmlAttribute("ControlerType")]
        public string ControlerType { get; set; }
                
        [XmlArray("FileOperationsArray")]
        [XmlArrayItem("FileOperation")]
        public List<XmlFileOperation> FileOperations { get; set; }
                
        [XmlArray("DataOperationsArray")]
        [XmlArrayItem("DataOperation")]
        public List<XmlDataOperation> DataOperations { get; set; }
    }

    [Serializable]
    [XmlRoot("ControlersConfiguration")]
    public class XmlFileOperation
    {
        [XmlAttribute("OperationName")]
        public string OperationName { get; set; }

        [XmlAttribute("DestinationFolder")]
        public string DestinationFolder { get; set; }

        [XmlAttribute("ActionType")]
        public string ActionType { get; set; }

        [XmlAttribute("Priority")]
        public int Priority { get; set; }
               
        [XmlElement("Filter")]
        public XmlFilter Filter { get; set; }

        [XmlArray("FilesExtensionArray")]
        [XmlArrayItem("FilesExtension")]
        public List<string> FilesExtensions;

        [XmlAttribute("NestedSourcePath")]
        public bool NestedSourcePath { get; set; }
    }

    [Serializable]
    [XmlRoot("ControlersConfiguration")]
    public class XmlDataOperation
    {
        [XmlAttribute("FileOperationName")]
        public string FileOperationName { get; set; }

        [XmlAttribute("DestinationFile")]
        public string DestinationFile { get; set; }

        [XmlAttribute("ActionType")]
        public string ActionType { get; set; }

        [XmlAttribute("Priority")]
        public int Priority { get; set; }

        [XmlElement("Filter")]
        public XmlFilter Filter { get; set; }

        [XmlAttribute("FileHeader")]
        public string FileHeader { get; set; }

        [XmlAttribute("FileFooter")]
        public string FileFooter { get; set; }

        [XmlAttribute("GroupSpace")]
        public int GroupSpace { get; set; }

        [XmlAttribute("WriteStart")]
        public string WriteStart { get; set; }

        [XmlAttribute("WriteStop")]
        public string WriteStop { get; set; }
    }

    [Serializable]
    [XmlRoot("ControlersConfiguration")]
    public class XmlFilter
    {
        [XmlAttribute("RegexContain")]
        public string RegexContain { get; set; }

        [XmlAttribute("RegexNotContain")]
        public string RegexNotContain { get; set; }

        [XmlArray("ContainArray")]
        [XmlArrayItem("Contain")]
        public List<string> Contains { get; set; }

        [XmlArray("NotContainArray")]
        [XmlArrayItem("NotContain")]
        public List<string> NotContains { get; set; }      
    }  
}

