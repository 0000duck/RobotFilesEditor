using ProgramTextFormat.Model.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProgramTextFormat.Model.Serialization
{
    class SerializationHelper
    {
       
        [XmlRoot(ElementName = "KukaInstruction")]
        public class KukaInstruction
        {
            [XmlAttribute(AttributeName = "Name")]
            public string Name { get; set; }
            [XmlAttribute(AttributeName = "IsComment")]
            public string IsComment { get; set; }
            [XmlAttribute(AttributeName = "IsFold")]
            public string IsFold { get; set; }
            [XmlAttribute(AttributeName = "KeyWordsString")]
            public string KeyWordsString { get; set; }
        }

        



    }
}
