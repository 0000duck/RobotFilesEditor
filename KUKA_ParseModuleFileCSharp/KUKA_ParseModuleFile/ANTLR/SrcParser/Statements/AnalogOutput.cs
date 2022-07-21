using ParseModuleFile.ANTLR.SrcParser.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Statements
{
    public class AnalogOutput : AnalogInput
    {
        #region fields
        private bool usesDelay;
        private ILiteral delay;
        private bool usesMinimum;
        private ILiteral minimum;
        private bool usesMaximum;
        private ILiteral maximum;
        #endregion fields

        #region properties
        public bool UsesDelay { get { return usesDelay; } set { Set(ref usesDelay, value); } }
        public ILiteral Delay { get { return delay; } set { Set(ref delay, value); } }
        public bool UsesMinimum { get { return usesMinimum; } set { Set(ref usesMinimum, value); } }
        public ILiteral Minimum { get { return minimum; } set { Set(ref minimum, value); } }
        public bool UsesMaximum { get { return usesMaximum; } set { Set(ref usesMaximum, value); } }
        public ILiteral Maximum { get { return maximum; } set { Set(ref maximum, value); } }
        #endregion properties

        #region constructors
        public AnalogOutput(int Line) : base(Line, StatementType.analogOutputStatement) { }
        public AnalogOutput(Antlr4.Runtime.ParserRuleContext context) : base(context, StatementType.analogOutputStatement) { }
        #endregion constructors

        #region methods
        public override string ToString()
        {
            return "ANOUT ...";
        }
        #endregion methods
    }
}
