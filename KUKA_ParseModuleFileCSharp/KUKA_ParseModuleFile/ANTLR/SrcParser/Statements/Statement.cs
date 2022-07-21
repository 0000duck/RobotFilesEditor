using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Statements
{
    public class Statement : SrcItem 
    {
        #region fields
        private StatementType type;
        #endregion fields

        #region properties
        public StatementType Type { get { return type; } set { Set(ref type, value); } } 
        #endregion properties

        #region constructors
        public Statement(int Line, StatementType Type)
            : base(Line)
        {
            this.type = Type;
        }
        public Statement(Antlr4.Runtime.ParserRuleContext context, StatementType Type)
            : base(context)
        {
            this.type = Type;
        }
        #endregion constructors

        #region methods
        public override string ToString()
        {
            return type.ToString();
        }
        #endregion methods
    }
}
