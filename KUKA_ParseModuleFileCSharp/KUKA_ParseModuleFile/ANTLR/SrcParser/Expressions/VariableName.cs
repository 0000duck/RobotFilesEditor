using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Expressions
{
    public class VariableName : SrcItem, IGeometricExpression, IExpression
    {
        #region fields
        private string name;
        private ArrayVariableSuffix arrayVariableSuffix;
        #endregion fields

        #region properties
        public string Name { get { return name; } set { Set(ref name, value); } }
        public ArrayVariableSuffix ArrayVariableSuffix { get { return arrayVariableSuffix; } set { Set(ref this.arrayVariableSuffix, value); } }
        #endregion properties

        #region constructors
        public VariableName(int Line) : base(Line) { }
        public VariableName(Antlr4.Runtime.ParserRuleContext context) : base(context) { }
        #endregion constructors

        #region methods
        public override string ToString()
        {
            return name + (arrayVariableSuffix != null ? " " + arrayVariableSuffix.ToString() : "");
        }
        #endregion methods
    }
}
