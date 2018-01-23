using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RobotFilesEditor.Serializer
{
    [Serializable]
    [XmlRoot("ControlersConfiguration")]
    public class ControlersConfiguration
    {
        [XmlAttribute("DestinationPath")]
        public string DestinationPath { get; set; }
        [XmlAttribute("SourcePath")]
        public string SourcePath { get; set; }

        [XmlArray("ArrayOfControlers")]
        [XmlArrayItem("Controler")]
        public List<Controler> Controlers { get; set; }
    } 

    [Serializable]
    [XmlRoot("ControlersConfiguration")]
    public class Controler
    {
        [XmlAttribute("ControlerType")]
        public string ControlerType { get; set; }

        [XmlArray("ArrayOfOperationFilters")]
        [XmlArrayItem("OperationFilter")]
        public List<OperationFilter> OperationFilters { get; set; }
     }   

    [Serializable]
    [XmlRoot("Controler")]
    public class OperationFilter
    {
        [XmlAttribute("OperationName")]
        public string OperationName { get; set; }

        [XmlAttribute("DestinationFolder")]
        public string DestinationFolder { get; set; }

        [XmlAttribute("Action")]
        public GlobalData.Action Action { get; set; }


        [XmlArray("Arrays")]
        [XmlArrayItem("FileExtension")]
        public List<string> FilesExtension { get; set; }

        [XmlArrayItem("Arrays")]
        [XmlArrayItem("ContainsAtName")]
        public List<string> ContainsAtName { get; set; }
        
        [XmlArray("Arrays")]
        [XmlArrayItem("ContainsAtName")]
        public List<string> NotContainsAtName { get; set; }
        

        [XmlAttribute("RegexContain")]
        public string RegexContain { get; set; }

        [XmlAttribute("RegexNotContain")]
        public string RegexNotContain { get; set; }
    }
}

