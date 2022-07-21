using ParseModuleFile.ANTLR.SrcParser.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Statements
{
    public class AnalogInput : Statement
    {
        #region fields
        private bool on;
        private IAssignmentExpression onOn;
        private string onOff;
        #endregion fields

        #region properties
        public bool On { get { return on; } set { Set(ref on, value); } }
        public IAssignmentExpression OnOn { get { return onOn; } set { Set(ref onOn, value); } }
        public string OnOff { get { return onOff; } set { Set(ref onOff, value); } }
        #endregion properties

        #region constructors
        public AnalogInput(int Line, StatementType Type = StatementType.analogInputStatement) : base(Line, Type) { }
        public AnalogInput(Antlr4.Runtime.ParserRuleContext context, StatementType Type = StatementType.analogInputStatement) : base(context, Type) { }
        #endregion constructors

        #region methods
        public override string ToString()
        {
            return "ANIN ...";
        }
        #endregion methods
    }
}
