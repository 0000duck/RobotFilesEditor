using ParseModuleFile.ANTLR.SrcParser.Expressions;
using ParseModuleFile.ANTLR.SrcParser.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Movements
{
    public class LIN : Statement
    {
        #region fields
        private IGeometricExpression target;
        private bool usesSecondCDIS;
        private Approximation approximation;
        #endregion fields

        #region properties
        public IGeometricExpression Target { get { return target; } set { Set(ref target, value); } }
        public bool UsesSecondCDIS { get { return usesSecondCDIS; } set { Set(ref usesSecondCDIS, value); } }
        public Approximation Approximation { get { return approximation; } set { Set(ref approximation, value); } }
        #endregion properties

        #region constructors
        public LIN(int Line, StatementType Type = StatementType.LIN) : base(Line, Type) { }
        public LIN(Antlr4.Runtime.ParserRuleContext context, StatementType Type = StatementType.LIN) : base(context, Type) { }
        #endregion constructors

        #region methods
        public override string ToString()
        {
            return "LIN " + target.ToString() + (approximation != Movements.Approximation.NONE ? " " + approximation.ToString() : "") + (usesSecondCDIS ? " C_DIS" : "");
        }
        #endregion methods
    }
}
