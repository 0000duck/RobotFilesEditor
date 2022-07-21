using ParseModuleFile.ANTLR.SrcParser.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Statements
{
    class Interrupt : Statement 
    {
        #region fields
        private string action;
        private IPrimary priority;
        #endregion fields

        #region properties
        public string Action { get { return action; } set { Set(ref action, value); } }
        public IPrimary Priority { get { return priority; } set { Set(ref priority, value); } }
        #endregion properties

        #region constructors
        public Interrupt(int Line, StatementType Type = StatementType.INTERRUPT) : base(Line, Type) { }
        public Interrupt(Antlr4.Runtime.ParserRuleContext context, StatementType Type = StatementType.INTERRUPT) : base(context, Type) { }
        #endregion constructors

        #region methods
        public override string ToString()
        {
            return "INTERRUPT ...";
        }
        #endregion methods
    }
}
