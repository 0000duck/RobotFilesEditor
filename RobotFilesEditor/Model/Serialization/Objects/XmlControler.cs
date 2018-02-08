using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace RobotFilesEditor
{
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
}
