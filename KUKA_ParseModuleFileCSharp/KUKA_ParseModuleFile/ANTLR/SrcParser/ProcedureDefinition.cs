using ParseModuleFile.ANTLR.SrcParser.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser
{
    public class ProcedureDefinition : SrcItem
    {
        #region fields
        private bool isGlobal;
        private string name;
        private FormalParameters formalParameters;
        private string dataSection;
        private StatementList statementList;
        //private ISrcParser body;
        #endregion // fields

        #region properties
        public bool IsGlobal { get { return isGlobal; } set { Set(ref isGlobal, value); } }
        public string Name { get { return name; } set { Set(ref name, value); } }
        public FormalParameters FormalParameters { get { return formalParameters; } set { Set(ref formalParameters, value); } }
        public string DataSection { get { return dataSection; } set { Set(ref dataSection, value); } }
        public StatementList StatementList { get { return statementList; } set { Set(ref statementList, value); } }
        #endregion // properties

        #region constructors
        public ProcedureDefinition(int Line) : base(Line) { }
        public ProcedureDefinition(Antlr4.Runtime.ParserRuleContext context) : base(context) { }
        #endregion // constructors

        #region methods
        public override string ToString()
        {
            return name;
            //TODO: formal parameters
        }
        #endregion methods
    }
}
