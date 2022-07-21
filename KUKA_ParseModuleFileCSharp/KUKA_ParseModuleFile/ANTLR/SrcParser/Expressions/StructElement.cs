using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Expressions
{
    public class StructElement : SrcItem
    {
        #region fields
        private VariableName variableName;
        private IExpression expression;
        #endregion fields

        #region properties
        public VariableName VariableName { get { return variableName; } set { Set(ref this.variableName, value); } }
        public IExpression Expression { get { return expression; } set { Set(ref this.expression, value); } }
        #endregion properties

        #region constructors
        public StructElement(int Line) : base(Line) { }
        public StructElement(Antlr4.Runtime.ParserRuleContext context) : base(context) { }
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
