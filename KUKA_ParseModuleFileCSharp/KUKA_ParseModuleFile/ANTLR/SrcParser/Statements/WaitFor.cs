using ParseModuleFile.ANTLR.SrcParser.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Statements
{
    class WaitFor : WaitSec 
    {
        #region constructors
        public WaitFor(int Line, IExpression Condition = null) : base(Line, Condition, StatementType.WAIT_FOR) { }
        public WaitFor(Antlr4.Runtime.ParserRuleContext context, IExpression Condition = null) : base(context, Condition, StatementType.WAIT_SEC) { }
        #endregion constructors

        #region methods
        public override string ToString()
        {
            return "WAIT FOR ...";
        }
        #endregion methods
    }
}
