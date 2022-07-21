using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Expressions
{
    public class ParExpression : SrcItem, IPrimary
    {
        #region fields
        private IAssignmentExpression value;
        #endregion fields

        #region properites
        public IAssignmentExpression Value { get { return value; } set { Set(ref this.value, value); } }
        #endregion properites

        #region constructors
        public ParExpression(int Line) : base(Line) { }
        public ParExpression(Antlr4.Runtime.ParserRuleContext context) : base(context) { }
        #endregion constructors

        #region methods
        public override string ToString()
        {
            return "( " + value.ToString() + " )";
        }
        #endregion methods
    }
}
