using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Statements
{
    public class Brake : Statement
    {
        #region fields
        private bool fast;
        #endregion fields

        #region properties
        public bool Fast { get { return fast; } set { Set(ref fast, value); } }
        #endregion properties

        #region constructors
        public Brake(int Line, bool Fast)
            : base(Line, StatementType.BRAKE)
        {
            fast = Fast;
        }
        public Brake(Antlr4.Runtime.ParserRuleContext context, bool Fast)
            : base(context, StatementType.BRAKE)
        {
            fast = Fast;
        }
        #endregion constructors

        #region methods
        public override string ToString()
        {
            return "BRAKE" + (fast ? " F" : "");
        }
        #endregion methods
    }
}
