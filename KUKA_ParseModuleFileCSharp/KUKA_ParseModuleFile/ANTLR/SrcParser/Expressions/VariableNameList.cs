using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Expressions
{
    public class VariableNameList : SrcList<VariableName>, IPrimary
    {
        #region fields
        private bool hasArguments;
        private ExpressionList arguments;
        #endregion fields
        
        #region properties
        public bool HasArguments { get { return hasArguments; } set { Set(ref hasArguments, value); } }
        public ExpressionList Arguments { get { return arguments; } set { Set(ref arguments, value); } }
        #endregion properties

        #region constructors
        public VariableNameList(int Line, int EndLine) : base (Line,EndLine) { }
        public VariableNameList(Antlr4.Runtime.ParserRuleContext context) : base(context) { }
        #endregion constructors

        #region methods
        public override string ToString()
        {
            return string.Join(".",this) + (hasArguments ? "( " + arguments.ToString() + " )" : "");
        }
        #endregion methods
    }
}
