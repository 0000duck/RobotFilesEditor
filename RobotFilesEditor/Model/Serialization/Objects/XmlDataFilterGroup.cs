using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RobotFilesEditor
{
    [Serializable]
    [XmlRoot("ControlersConfiguration")]
    public class XmlDataFilterGroup
    {
        [XmlAttribute("GroupHeader")]
        public string GroupHeader { get; set; }

        [XmlAttribute("GroupFooter")]
        public string GroupFooter { get; set; }

        [XmlAttribute("SpaceBeforGroup")]
        public int SpaceBeforGroup { get; set; }

        [XmlAttribute("SpaceAfterGroup")]
        public int SpaceAfterGroup { get; set; }

        [XmlAttribute("TextBefore")]
        public string TextBefore { get; set; }

        [XmlElement("Filter")]
        public XmlFilter Filter { get; set; }

        [XmlAttribute("OnlyRegex")]
        public bool OnlyRegex { get; set; }        
    }
}
