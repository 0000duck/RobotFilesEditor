using ParseModuleFile.ANTLR.SrcParser.Expressions;
using ParseModuleFile.ANTLR.SrcParser.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Movements
{
    public class PTP_REL : PTP
    {
        #region constructors
        public PTP_REL(int Line, StatementType Type = StatementType.PTP_REL) : base(Line, Type) { }
        public PTP_REL(Antlr4.Runtime.ParserRuleContext context, StatementType Type = StatementType.PTP_REL) : base(context, Type) { }
        #endregion constructors

        #region methods
        public override string ToString()
        {
            return "PTP_REL " + Target.ToString() + (UsesCPTP ? " C_PTP" : "") + (Approximation != Movements.Approximation.NONE ? " " + Approximation.ToString() : "");
        }
        #endregion methods
    }
}
