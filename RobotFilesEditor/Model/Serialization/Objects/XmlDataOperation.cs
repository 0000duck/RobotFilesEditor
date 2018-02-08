using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace RobotFilesEditor
{
    [Serializable]
    [XmlRoot("ControlersConfiguration")]
    public class XmlDataOperation
    {
        [XmlAttribute("FileOperationName")]
        public string FileOperationName { get; set; }

        [XmlAttribute("ActionType")]
        public string ActionType { get; set; }

        [XmlAttribute("Priority")]
        public int Priority { get; set; }

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

        [XmlArray("DataFilterGroupsArray")]
        [XmlArrayItem("DataFilterGroup")]
        public List<XmlDataFilterGroup> DataFilterGroups { get; set; }

        [XmlAttribute("DestinationFilePath")]
        public string DestinationFilePath { get; set; }

        [XmlAttribute("DestinationFileSource")]
        public string DestinationFileSource { get; set; }
    }
}
