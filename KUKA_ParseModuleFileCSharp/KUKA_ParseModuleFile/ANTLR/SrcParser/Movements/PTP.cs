using ParseModuleFile.ANTLR.SrcParser.Expressions;
using ParseModuleFile.ANTLR.SrcParser.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Movements
{
    public class PTP : Statement
    {
        #region fields
        private IGeometricExpression target;
        private bool usesCPTP;
        private Approximation approximation;
        #endregion fields

        #region properties
        public IGeometricExpression Target { get { return target; } set { Set(ref target, value); } }
        public bool UsesCPTP { get { return usesCPTP; } set { Set(ref usesCPTP, value); } }
        public Approximation Approximation { get { return approximation; } set { Set(ref approximation, value); } }
        #endregion properties

        #region constructors
        public PTP(int Line, StatementType Type = StatementType.PTP) : base(Line, Type) { }
        public PTP(Antlr4.Runtime.ParserRuleContext context, StatementType Type = StatementType.PTP) : base(context, Type) { }
        #endregion constructors

        #region methods
        public override string ToString()
        {
            return "PTP " + target.ToString() + (usesCPTP ? " C_PTP" : "") + (approximation != Movements.Approximation.NONE ? " " + approximation.ToString() : "");
        }
        #endregion methods
    }
}
