using ParseModuleFile.ANTLR.SrcParser.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Statements
{
    class WaitSec : Statement 
    {
        #region fields
        private IExpression condition;
        #endregion fields

        #region properties
        public IExpression Condition { get { return condition; } set { Set(ref condition, value); } }
        #endregion properties

        #region constructors
        public WaitSec(int Line, IExpression Condition = null, StatementType Type = StatementType.WAIT_SEC)
            : base (Line,Type)
        {
            condition = Condition;
        }
        public WaitSec(Antlr4.Runtime.ParserRuleContext context, IExpression Condition = null, StatementType Type = StatementType.WAIT_SEC)
            : base(context, Type)
        {
            condition = Condition;
        }
        #endregion constructors

        #region methods
        public override string ToString()
        {
            return "WHILE SEC ...";
        }
        #endregion methods
    }
}
