using ParseModuleFile.ANTLR.SrcParser.Expressions;
using ParseModuleFile.ANTLR.SrcParser.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Movements
{
    public class CIRC : Statement
    {
        #region fields
        private IGeometricExpression auxiliaryPoint;
        private IGeometricExpression target;
        private bool usesCircularAngle;
        private IPrimary caValue;
        private Approximation approximation;
        #endregion fields

        #region properties
        public IGeometricExpression AuxiliaryPoint { get { return auxiliaryPoint; } set { Set(ref auxiliaryPoint, value); } }
        public IGeometricExpression Target { get { return target; } set { Set(ref target, value); } }
        public bool UsesCircularAngle { get { return usesCircularAngle; } set { Set(ref usesCircularAngle, value); } }
        public IPrimary CAValue { get { return caValue; } set { Set(ref caValue, value); } }
        public Approximation Approximation { get { return approximation; } set { Set(ref approximation, value); } }
        #endregion properties

        #region constructors
        public CIRC(int Line, StatementType Type = StatementType.CIRC_REL) : base(Line, Type) { }
        public CIRC(Antlr4.Runtime.ParserRuleContext context, StatementType Type = StatementType.CIRC_REL) : base(context, Type) { }
        #endregion constructors


        #region methods
        public override string ToString()
        {
            return "CIRC " + auxiliaryPoint.ToString() + " " + target.ToString() + (usesCircularAngle ? " CA" + caValue.ToString() : "") + (approximation != Movements.Approximation.NONE ? " " + approximation.ToString(): "");
        }
        #endregion methods
    }
}
