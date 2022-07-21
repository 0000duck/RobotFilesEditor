using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Expressions
{
    public class OperatorExpression : SrcItem, IExpression
    {
        #region fields
        private SrcList<IExpression> values;
        private SrcList<Operator> operators;
        #endregion fields

        #region properties
        public SrcList<IExpression> Values { get { return values; } set { Set(ref values, value); } }
        public SrcList<Operator> Operators { get { return operators; } set { Set(ref operators, value); } }
        #endregion properties

        #region constructors
        public OperatorExpression(int Line) : base(Line) { }
        public OperatorExpression(Antlr4.Runtime.ParserRuleContext context) : base(context) { }
        #endregion constructors

        #region methods
        public IExpression Reduce()
        {
            if (values.Count == 0) return null;
            if (values.Count == 1) return values[0];
            return this;
        }
        public override string ToString()
        {
            string output = "";
            int i = 0;
            foreach (var item in values)
            {
                if (values.Last() != item)
                {
                    output += item.ToString() + " " + operators.ToString() + " ";
                    i++;
                }
            }
            return output + values.Last().ToString();
        }
        #endregion methods
    }
}
