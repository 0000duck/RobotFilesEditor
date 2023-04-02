using ProgramTextFormat.Model.RobotInstructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProgramTextFormat.Model.Rules
{
    [XmlRoot(ElementName = "Instructions")]
    public class Instructions
    {
        [XmlElement(ElementName = "KukaInstruction")]
        public List<KukaInstruction> KukaInstruction { get; set; }
    }
}
