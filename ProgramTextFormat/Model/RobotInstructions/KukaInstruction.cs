using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProgramTextFormat.Model.RobotInstructions
{
    [XmlRoot(ElementName = "KukaInstruction")]
    public class KukaInstruction : RobotInstructionBase
    {
        [XmlIgnore]
        public override string CommentSign => ";";

        [XmlIgnore]
        public override bool UsesFold => true;

        [XmlIgnore]
        public override string FoldStart => ";fold";

        [XmlIgnore]
        public override string FoldEnd => ";endfold";

        [XmlIgnore]
        public override string RobotType => "KUKA";

        public override object Clone()
        {
            KukaInstruction result = new KukaInstruction();
            this.FillBasicInfos(result);
            return result;
        }

        
    }
}
