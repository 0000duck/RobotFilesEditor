using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Expressions
{
    public class AssignmentExpression : SrcItem, IAssignmentExpression
    {
        #region fields
        private SrcList<IExpression> declarations;
        private IExpression value;
        #endregion fields

        #region properties
        public SrcList<IExpression> Declarations { get { return declarations; } set { Set(ref declarations, value); } }
        public IExpression Value { get { return value; } set { Set(ref this.value, value); } }
        #endregion properties

        #region constructors
        public AssignmentExpression(int Line) : base(Line) { }
        public AssignmentExpression(Antlr4.Runtime.ParserRuleContext context) : base(context) { }
        #endregion constructors

        #region methods
        public override string ToString()
        {
            string output = (declarations != null ? string.Join(" = ", declarations) : "");
            output += ((declarations != null && declarations.Count > 0) ? " = " : "");
            output += (value != null ? value.ToString() : "");
            return output;
        }
        #endregion methods
    }
}
