using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Statements
{
    public class SwitchBlockStatementGroups : SrcList<SwitchBlockStatementGroup>
    {

        #region constructors
        public SwitchBlockStatementGroups(Antlr4.Runtime.ParserRuleContext context) : base(context) { }
        #endregion constructors


        #region methods
        public override string ToString()
        {
            return this.Count.ToString() + " cases";
        }
        #endregion methods

    }
}
