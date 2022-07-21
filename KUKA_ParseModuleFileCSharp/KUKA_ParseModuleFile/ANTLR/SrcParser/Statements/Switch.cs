using ParseModuleFile.ANTLR.SrcParser.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Statements
{
    public class Switch : StatementBlock
    {
        #region fields
        private IExpression condition;
        private SwitchBlockStatementGroups cases;
        #endregion fields

        #region properties
        public IExpression Condition { get { return condition; } set { Set(ref condition, value); } }
        public SwitchBlockStatementGroups Cases { get { return cases; } set { Set(ref cases, value); } }
        #endregion properties

        #region constructors
        public Switch(int Line, int EndLine) : base(Line, EndLine, StatementType.SWITCH) { }
        public Switch(Antlr4.Runtime.ParserRuleContext context) : base(context, StatementType.SWITCH) { }
        #endregion constructors

        #region methods
        public override string ToString()
        {
            return "SWITCH ... ENDSWITCH";
        }
        #endregion methods
    }
}
