using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace RobotFilesEditor
{
    [Serializable]
    [XmlRoot("ControlersConfiguration", Namespace = "RobotFilesEditor")]
    public class XmlControlersConfiguration
    {
        [XmlAttribute("DestinationPath")]
        public string DestinationPath { get; set; }

        [XmlAttribute("SourcePath")]
        public string SourcePath { get; set; }

        [XmlArray("ContorolersArray")]
        [XmlArrayItem("Controler")]
        public List<XmlControler> Contorolers { get; set; }
    }
}
