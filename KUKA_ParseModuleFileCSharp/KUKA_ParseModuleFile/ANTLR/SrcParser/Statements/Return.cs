using ParseModuleFile.ANTLR.SrcParser.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Statements
{
    public class Return : Statement
    {
        #region fields
        private IAssignmentExpression expression;
        #endregion fields

        #region properties
        public IAssignmentExpression Expression { get { return expression; } set { Set(ref expression, value); } }
        #endregion properties

        #region constructors
        public Return(int Line, IAssignmentExpression Expression = null)
            : base(Line, StatementType.RETURN)
        {
            this.expression = Expression;
        }
        public Return(Antlr4.Runtime.ParserRuleContext context, IAssignmentExpression Expression = null)
            : base(context, StatementType.RETURN)
        {
            this.expression = Expression;
        }
        #endregion constructors

        #region methods
        public override string ToString()
        {
            return "RETURN ...";
        }
        #endregion methods
    }
}
