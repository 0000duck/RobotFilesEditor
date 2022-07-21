using ParseModuleFile.ANTLR.SrcParser.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Statements
{
    class While : Loop
    {
        #region fields
        private IExpression condition;
        #endregion fields

        #region properties
        public IExpression Condition { get { return condition; } set { Set(ref condition, value); } }
        #endregion properties

        #region constructors
        public While(int Line, int EndLine) : base(Line, EndLine, StatementType.WHILE) { }
        public While(Antlr4.Runtime.ParserRuleContext context) : base(context, StatementType.WHILE) { }
        #endregion constructors

        #region methods
        public override string ToString()
        {
            return "WHILE ... ENDWHILE";
        }
        #endregion methods
    }
}
