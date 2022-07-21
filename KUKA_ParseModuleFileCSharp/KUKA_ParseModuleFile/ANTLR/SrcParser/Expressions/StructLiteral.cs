using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Expressions
{
    public class StructLiteral : Literal
    {
        #region fields
        private string typeName;
        private StructElementList structElementList;
        #endregion fields

        #region properties
        public string TypeName { get { return typeName; } set { Set(ref typeName, value); } }
        public StructElementList StructElementList { get { return structElementList; } set { Set(ref this.structElementList, value); } }
        #endregion properties

        #region constructors
        public StructLiteral(int Line) : base(Line, LiteralType.STRUCT) { }
        public StructLiteral(Antlr4.Runtime.ParserRuleContext context) : base(context, LiteralType.STRUCT) { }
        #endregion constructors
    }
}
