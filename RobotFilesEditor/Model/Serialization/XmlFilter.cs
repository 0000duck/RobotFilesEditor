using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace RobotFilesEditor
{
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
