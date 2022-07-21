using ParseModuleFile.ANTLR.SrcParser.Expressions;
using ParseModuleFile.ANTLR.SrcParser.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Movements
{
    public class LIN_REL : LIN
    {
        #region fields
        private EnumElement enumElement;
        #endregion fields

        #region properties
        public EnumElement EnumElement { get { return enumElement; } set { Set(ref enumElement, value); } }
        #endregion properties

        #region constructors
        public LIN_REL(int Line, StatementType Type = StatementType.LIN_REL) : base(Line, Type) { }
        public LIN_REL(Antlr4.Runtime.ParserRuleContext context, StatementType Type = StatementType.LIN_REL) : base(context, Type) { }
        #endregion constructors

        #region methods
        public override string ToString()
        {
            return "LIN_REL " + Target.ToString() + (Approximation != Movements.Approximation.NONE ? " " + Approximation.ToString() : "") + (UsesSecondCDIS ? " C_DIS" : "");
        }
        #endregion methods
    }
}
