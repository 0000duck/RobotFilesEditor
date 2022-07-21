using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Statements
{
    public class Label : Statement
    {
        #region fields
        private string identifier;
        #endregion fields

        #region properties
        public string Identifier { get { return identifier; } set { Set(ref identifier, value); } }
        #endregion properties

        #region constructors
        public Label(int Line, string Identifier = "")
            : base(Line, StatementType.LABEL)
        {
            this.identifier = Identifier;
        }
        public Label(Antlr4.Runtime.ParserRuleContext context, string Identifier = "")
            : base(context, StatementType.LABEL)
        {
            this.identifier = Identifier;
        }
        #endregion constructors

        #region methods
        public override string ToString()
        {
            return (identifier == null ? "UNKNOWN LABEL" : identifier) + ":";
        }
        #endregion methods
    }
}
