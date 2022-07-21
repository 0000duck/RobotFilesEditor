using ParseModuleFile.ANTLR.SrcParser.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Statements
{
    public class If : StatementBlock
    {
        #region fields
        private IExpression condition;
        private StatementList ifTrue;
        private StatementList ifFalse;
        #endregion fields

        #region properties
        public IExpression Condition { get { return condition; } set { Set(ref condition, value); } }
        public StatementList IfTrue { get { return ifTrue; } set { Set(ref ifTrue, value); } }
        public StatementList IfFalse { get { return ifFalse; } set { Set(ref ifFalse, value); } }
        #endregion properties

        #region constructors
        public If(int Line, int EndLine, StatementType Type = StatementType.IF) : base(Line, EndLine, Type) { }
        public If(Antlr4.Runtime.ParserRuleContext context, StatementType Type = StatementType.IF) : base(context, Type) { }
        #endregion constructors

        #region methods
        public override string ToString()
        {
            return "IF ... THEN ... " + (ifFalse != null ? "ELSE ..." : "") + "ENDIF";
        }
        #endregion methods
    }
}
