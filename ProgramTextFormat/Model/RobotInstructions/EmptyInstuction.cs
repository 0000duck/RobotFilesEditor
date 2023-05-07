using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramTextFormat.Model.RobotInstructions
{
    public class EmptyInstuction : RobotInstructionBase
    {
        public override string CommentSign => ";";

        public override bool UsesFold => false;

        public override string FoldStart => "";

        public override string FoldEnd => "";

        public override string RobotType => "";

        public EmptyInstuction()
        {
            Name = "---";
        }
        public override object Clone()
        {
            return new EmptyInstuction();
        }
    }
}
