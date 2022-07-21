using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Expressions
{
    public class EnumElement : Literal
    {
        #region fields
        private string value;
        #endregion fields

        #region properties
        public string Value { get { return value; } set { Set(ref this.value, value); } }
        #endregion properties

        #region constructors
        public EnumElement(int Line) : base(Line, LiteralType.ENUM) { }
        public EnumElement(Antlr4.Runtime.ParserRuleContext context) : base(context, LiteralType.ENUM) { }
        #endregion constructors

        #region methods
        public override string ToString()
        {
            return "#" + (value == null ? "UNKNOWN ENUM" : value);
        }
        #endregion methods
    }
}
