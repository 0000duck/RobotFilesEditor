using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Expressions
{
    public class UnaryPlusMinusExpression : SrcItem, IExpression
    {
        #region fields
        private bool minusSign;
        private IPrimary value;
        #endregion fields

        #region properties
        public bool MinusSign { get { return minusSign; } set { Set(ref this.minusSign, value); } }
        public IPrimary Value { get { return value; } set { Set(ref this.value, value); } }
        #endregion properties

        #region constructors
        public UnaryPlusMinusExpression(int Line) : base(Line) { }
        public UnaryPlusMinusExpression(Antlr4.Runtime.ParserRuleContext context) : base(context) { }
        #endregion constructors

        #region methods
        public override string ToString()
        {
            return base.ToString();
            //return "#" + (value == null ? "UNKNOWN ENUM" : value);
        }
        #endregion methods
    }
}
