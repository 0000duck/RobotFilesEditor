using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Expressions
{
    public class Expression : SrcItem, IExpression
    {
        #region constructors
        public Expression(int Line) : base(Line) { }
        public Expression(Antlr4.Runtime.ParserRuleContext context) : base(context) { }
        #endregion constructors
    }
}
