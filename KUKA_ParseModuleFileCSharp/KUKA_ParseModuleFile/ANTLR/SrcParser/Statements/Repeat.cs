using ParseModuleFile.ANTLR.SrcParser.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Statements
{
    class Repeat : Loop
    {
        #region fields
        private IExpression condition;
        #endregion fields

        #region properties
        public IExpression Condition { get { return condition; } set { Set(ref condition, value); } }
        #endregion properties

        #region constructors
        public Repeat(Antlr4.Runtime.ParserRuleContext context) : base(context, StatementType.REPEAT) { }
        public Repeat(int Line, int EndLine) : base(Line, EndLine, StatementType.REPEAT) { }
        #endregion constructors

        #region methods
        public override string ToString()
        {
            return "REPEAT ... UNTIL ...";
        }
        #endregion methods
    }
}
