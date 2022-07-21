using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Expressions
{
    public class UnaryOpetatorExpression : SrcItem, IExpression
    {
        #region fields
        private IExpression value;
        private Operator ioperator;
        #endregion fields

        #region properties
        public IExpression Value { get { return value; } set { Set(ref value, value); } }
        public Operator Operator { get { return ioperator; } set { Set(ref ioperator, value); } }
        #endregion properties

        #region constructors
        public UnaryOpetatorExpression(int Line) : base(Line) { }
        public UnaryOpetatorExpression(Antlr4.Runtime.ParserRuleContext context) : base(context) { }
        #endregion constructors

        #region methods
        public IExpression Reduce()
        {
            if (ioperator == Expressions.Operator.PLUS) return value;
            if (ioperator == Expressions.Operator.MINUS &&
                value is UnaryOpetatorExpression &&
                ((UnaryOpetatorExpression)value).ioperator == Expressions.Operator.MINUS)
                return ((UnaryOpetatorExpression)value).value;
            if (ioperator == Expressions.Operator.NOT &&
                value is UnaryOpetatorExpression &&
                ((UnaryOpetatorExpression)value).ioperator == Expressions.Operator.NOT)
                return ((UnaryOpetatorExpression)value).value;
            if (ioperator == Expressions.Operator.B_NOT &&
                value is UnaryOpetatorExpression &&
                ((UnaryOpetatorExpression)value).ioperator == Expressions.Operator.B_NOT)
                return ((UnaryOpetatorExpression)value).value;
            return this;
        }
        #endregion methods
    }
}
