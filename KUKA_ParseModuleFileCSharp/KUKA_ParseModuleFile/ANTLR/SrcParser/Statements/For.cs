using ParseModuleFile.ANTLR.SrcParser.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Statements
{
    class For : Loop
    {
        #region fields
        private string identifier;
        private IExpression from;
        private IExpression to;
        private IExpression step;
        #endregion fields

        #region properties
        public string Identifier { get { return identifier; } set { Set(ref identifier, value); } }
        public IExpression From { get { return from; } set { Set(ref from, value); } }
        public IExpression To { get { return to; } set { Set(ref to, value); } }
        public IExpression Step { get { return step; } set { Set(ref step, value); } }
        #endregion properties

        #region constructors
        public For(int Line, int EndLine, StatementType Type = StatementType.FOR) : base(Line, EndLine, Type) { }
        public For(Antlr4.Runtime.ParserRuleContext context, StatementType Type = StatementType.FOR) : base(context, Type) { }
        #endregion constructors

        #region methods
        public override string ToString()
        {
            return "FOR ... = ... TO ... ... ENDFOR";
        }
        #endregion methods
    }
}
