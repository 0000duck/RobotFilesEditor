using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser
{
    public class FormalParameters : SrcList<Parameter>
    {
        #region constructors
        public FormalParameters(Antlr4.Runtime.ParserRuleContext context) : base(context) { }
        #endregion constructors
    }
}
