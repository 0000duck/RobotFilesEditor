using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace RobotFilesEditor
{
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
}
