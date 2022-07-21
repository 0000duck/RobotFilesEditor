using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Expressions
{
    public class GeometricExpression : SrcItem
    {
        #region constructors
        public GeometricExpression(int Line) : base(Line) { }
        public GeometricExpression(Antlr4.Runtime.ParserRuleContext context) : base(context) { }
        #endregion constructors
    }
}
