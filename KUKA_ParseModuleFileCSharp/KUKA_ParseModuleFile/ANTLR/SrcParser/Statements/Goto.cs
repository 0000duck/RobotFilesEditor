using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Statements
{
    public class Goto : Statement
    {
        #region fields
        private string identifier;
        #endregion fields

        #region properties
        public string Identifier { get { return identifier; } set { Set(ref identifier, value); } }
        #endregion properties

        #region constructors
        public Goto(int Line, string Identifier = null, StatementType Type = StatementType.GOTO)
            : base(Line, Type)
        {
            this.identifier = Identifier;
        }
        public Goto(Antlr4.Runtime.ParserRuleContext context, string Identifier = null, StatementType Type = StatementType.GOTO) : base(context, Type) 
        {
            this.identifier = Identifier;
        }
        #endregion constructors

        #region methods
        public override string ToString()
        {
            return "GOTO " + (identifier == null ? "UNKNOWN LABEL" : identifier);
        }
        #endregion methods
    }
}
