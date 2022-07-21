using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Statements
{
    public class Loop : StatementBlock
    {
        #region fields
        private StatementList statementList;
        #endregion fields

        #region properties
        public StatementList StatementList { get { return statementList; } set { Set(ref statementList, value); } }
        #endregion properties

        #region constructors
        public Loop(Antlr4.Runtime.ParserRuleContext context, StatementType Type = StatementType.LOOP) : base(context, Type) { }
        public Loop(int Line, int EndLine, StatementType Type = StatementType.LOOP) : base(Line, EndLine, Type) { }
        #endregion constructors

        #region methods
        public override string ToString()
        {
            return "LOOP ... ENDLOOP";
        }
        #endregion methods
    }
}
