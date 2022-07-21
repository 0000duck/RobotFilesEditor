using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Statements
{
    public class StatementBlock : SrcItemBlock
    {
        #region fields
        private StatementType type;
        #endregion fields

        #region properties
        public StatementType Type { get { return type; } set { Set(ref type, value); } } 
        #endregion properties

        #region constructors
        public StatementBlock(int Line, int EndLine, StatementType Type)
            : base(Line, EndLine)
        {
            this.type = Type;
        }
        public StatementBlock(Antlr4.Runtime.ParserRuleContext context, StatementType Type) : base(context)
        {
            this.type = Type;
        }
        #endregion constructors

        #region methods
        public override string ToString()
        {
            return "statement block";
        }
        #endregion methods
    }
}
