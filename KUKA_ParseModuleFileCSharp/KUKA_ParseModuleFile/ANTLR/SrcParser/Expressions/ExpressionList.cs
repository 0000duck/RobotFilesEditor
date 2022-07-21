using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Expressions
{
    public class ExpressionList : SrcList<IAssignmentExpression>
    {
        #region constructors
        public ExpressionList(int Line, int EndLine) : base (Line,EndLine) { } 
        public ExpressionList(Antlr4.Runtime.ParserRuleContext context) : base (context) { }
        #endregion constructors

        #region methods
        public override string ToString()
        {
            return string.Join(", ", this);
        }
        #endregion methods
    }
}
