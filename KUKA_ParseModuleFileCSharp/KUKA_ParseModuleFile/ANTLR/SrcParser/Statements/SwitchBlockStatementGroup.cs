using ParseModuleFile.ANTLR.SrcParser.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Statements
{
    public class SwitchBlockStatementGroup : StatementBlock
    {
        #region fields
        private bool isDefault;
        private SrcList<IExpression> label;
        private StatementList statementList;
        #endregion fields

        #region properties
        public bool IsDefault { get { return isDefault; } set { Set(ref isDefault, value); } }
        public SrcList<IExpression> Label { get { return label; } set { Set(ref label, value); } }
        public StatementList StatementList { get { return statementList; } set { Set(ref statementList, value); } }
        #endregion properties

        #region constructors
        public SwitchBlockStatementGroup(int Line, int EndLine, bool IsDefault)
            : base(Line, EndLine, StatementType.SWITCH_CASE)
        {
            isDefault = IsDefault;
            label = new SrcList<IExpression>(Line, EndLine);
        }
        public SwitchBlockStatementGroup(Antlr4.Runtime.ParserRuleContext context, bool IsDefault)
            : base(context, StatementType.SWITCH_CASE)
        {
            isDefault = IsDefault;
            label = new SrcList<IExpression>(context);
        }
        #endregion constructors

        #region methods
        public override string ToString()
        {
            return (isDefault ? "DEFAULT" : "CASE ...");
        }
        #endregion methods
    }
}
