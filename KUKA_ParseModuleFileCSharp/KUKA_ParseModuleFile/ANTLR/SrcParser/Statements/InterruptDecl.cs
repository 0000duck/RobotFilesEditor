using ParseModuleFile.ANTLR.SrcParser.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Statements
{
    class InterruptDecl : Statement 
    {
        #region fields
        private IExpression condition;
        private IAssignmentExpression action;
        private IPrimary priority;
        private bool global;
        #endregion fields

        #region properties
        public IExpression Condition { get { return condition; } set { Set(ref condition, value); } }
        public IAssignmentExpression Action { get { return action; } set { Set(ref action, value); } }
        public IPrimary Priority { get { return priority; } set { Set(ref priority, value); } }
        public bool Global { get { return global; } set { Set(ref global, value); } }
        #endregion properties

        #region constructors
        public InterruptDecl(int Line, StatementType Type = StatementType.INTERRUPT_DECL) : base(Line, Type) { }
        public InterruptDecl(Antlr4.Runtime.ParserRuleContext context, StatementType Type = StatementType.INTERRUPT_DECL) : base(context, Type) { }
        #endregion constructors

        #region methods
        public override string ToString()
        {
            return "INTERUPT DECL ... WHEN ... DO ...";
        }
        #endregion methods
    }
}
