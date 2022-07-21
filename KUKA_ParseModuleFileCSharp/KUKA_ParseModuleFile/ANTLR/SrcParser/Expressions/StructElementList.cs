using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Expressions
{
    public class StructElementList : SrcList<StructElement>
    {
        #region constructors
        public StructElementList(int Line, int EndLine) : base (Line,EndLine) { }
        public StructElementList(Antlr4.Runtime.ParserRuleContext context) : base(context) { }
        #endregion constructors
    }
}
