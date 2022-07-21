using ParseModuleFile.ANTLR.SrcParser.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Statements
{
    class Trigger : Statement 
    {
        #region fields
        private IExpression atEnd;
        private IExpression timeDelay;
        private IAssignmentExpression action;
        private bool withPriotiry;
        private IExpression priority;
        #endregion fields

        #region properties
        public IExpression AtEnd { get { return atEnd; } set { Set(ref atEnd, value); } }
        public IExpression TimeDelay { get { return timeDelay; } set { Set(ref timeDelay, value); } }
        public IAssignmentExpression Action { get { return action; } set { Set(ref action, value); } }
        public bool WithPriority { get { return withPriotiry; } set { Set(ref withPriotiry, value); } }
        public IExpression Priority { get { return priority; } set { Set(ref priority, value); } }
        #endregion properties

        #region constructors
        public Trigger(int Line) : base(Line, StatementType.TRIGGER) { }
        public Trigger(Antlr4.Runtime.ParserRuleContext context) : base(context, StatementType.TRIGGER) { }
        #endregion constructors

        #region methods
        public override string ToString()
        {
            return "TRIGGER WHEN DISTANCE ...";
        }
        #endregion methods
    }
}
