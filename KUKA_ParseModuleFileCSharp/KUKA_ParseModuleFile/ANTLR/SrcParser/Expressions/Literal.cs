using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Expressions
{
    public abstract class Literal : SrcItem, ILiteral
    {
        #region fields
        private LiteralType type;
        #endregion fields

        #region properties
        public LiteralType Type { get { return type; } set { Set(ref type, value); } }
        #endregion properties

        #region constructors
        public Literal(int Line, LiteralType Type) 
            : base(Line) 
        {
            type = Type;
        }
        public Literal(Antlr4.Runtime.ParserRuleContext context, LiteralType Type)
            : base(context)
        {
            type = Type;
        }
        #endregion constructors
    }
}
