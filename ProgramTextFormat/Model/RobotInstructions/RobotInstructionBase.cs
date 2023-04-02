using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProgramTextFormat.Model.RobotInstructions
{
    [XmlRoot(ElementName = "RobotInstructionBase")]
    public abstract class RobotInstructionBase
    {
        [XmlAttribute(AttributeName = "Name")]
        public string? Name { get; set; }

        [XmlAttribute(AttributeName = "IsComment")]
        public bool IsComment { get; set; }

        [XmlAttribute(AttributeName = "IsFold")]
        public bool IsFold { get; set; }

        [XmlIgnore]
        public abstract string CommentSign { get; }
        
        [XmlIgnore]
        public abstract bool UsesFold { get; }

        [XmlIgnore]
        public abstract string FoldStart { get; }

        [XmlIgnore]
        public abstract string FoldEnd { get; }

        [XmlAttribute(AttributeName = "KeyWordsString")]
        public string? KeyWordsString { get; set; }

        [XmlIgnore]
        public List<string> KeyWordList { get => KeyWordsString is null ? new List<string>() : KeyWordsString.Replace(" ", "").ToLower().Split(",").ToList(); }

    }
}
