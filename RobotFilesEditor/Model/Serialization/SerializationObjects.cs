using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RobotFilesEditor.Serializer
{
    [Serializable]
    [XmlRoot("ControlersConfiguration", Namespace = "RobotFilesEditor")]
    public class ControlersConfiguration
    {
        [XmlAttribute("DestinationPath")]
        public string DestinationPath { get; set; }

        [XmlAttribute("Source")]
        public string SourcePath { get; set; }

        [XmlArray("ContorolersArray")]
        [XmlArrayItem("Controler")]
        public List<SingleControler> Contorolers { get; set; }
    }

    [Serializable]
    [XmlRoot("ControlersConfiguration")]
    public class SingleControler
    {
        [XmlAttribute("ControlerType")]
        public string ControlerType { get; set; }

        [XmlArray("OperationFiltersArray")]
        [XmlArrayItem("OperationFilter")]
        public List<OperationFilter> OperationFilters { get; set; }
    }

    [Serializable]
    [XmlRoot("ControlersConfiguration")]
    public class OperationFilter
    {
        [XmlAttribute("OperationName")]
        public string OperationName { get; set; }

        [XmlAttribute("DestinationFolder")]
        public string DestinationFolder { get; set; }

        [XmlAttribute("Action")]
        public string Action { get; set; }


        [XmlArray("FilesExtensionsArray")]
        [XmlArrayItem("FilesExtension")]
        public List<string> FilesExtension { get; set; }

        [XmlArray("ContainsAtNameArray")]
        [XmlArrayItem("ContainsAtName")]
        public List<string> ContainsAtName { get; set; }

        [XmlArray("NotContainsAtNameArray")]
        [XmlArrayItem("NotContainsAtName")]
        public List<string> NotContainsAtName { get; set; }


        [XmlAttribute("RegexContain")]
        public string RegexContain { get; set; }

        [XmlAttribute("RegexNotContain")]
        public string RegexNotContain { get; set; }
    }
}

