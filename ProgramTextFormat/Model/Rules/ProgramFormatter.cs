using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProgramTextFormat.Model.Serialization.SerializationHelper;
using System.Xml.Serialization;

namespace ProgramTextFormat.Model.Rules
{
    [XmlRoot(ElementName = "ProgramFormatter")]
    public class ProgramFormatter
    {
        [XmlElement(ElementName = "Rules")]
        public Rules Rules { get; set; }
        [XmlElement(ElementName = "Instructions")]
        public Instructions Instructions { get; set; }
    }
}
