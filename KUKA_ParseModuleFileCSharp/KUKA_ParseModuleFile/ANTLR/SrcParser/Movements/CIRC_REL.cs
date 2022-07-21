using ParseModuleFile.ANTLR.SrcParser.Expressions;
using ParseModuleFile.ANTLR.SrcParser.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Movements
{
    public class CIRC_REL : CIRC
    {
        #region constructors
        public CIRC_REL(int Line, StatementType Type = StatementType.CIRC_REL) : base(Line, Type) { }
        public CIRC_REL(Antlr4.Runtime.ParserRuleContext context, StatementType Type = StatementType.CIRC_REL) : base(context, Type) { }
        #endregion constructors

        #region methods
        public override string ToString()
        {
            return "CIRC_REL " + AuxiliaryPoint.ToString() + " " + Target.ToString() + (UsesCircularAngle ? " CA" + CAValue.ToString() : "") + (Approximation != Movements.Approximation.NONE ? " " + Approximation.ToString() : "");
        }
        #endregion methods
    }
}
