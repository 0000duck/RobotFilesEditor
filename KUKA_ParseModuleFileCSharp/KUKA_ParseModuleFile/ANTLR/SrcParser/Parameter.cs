using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser
{
    public class Parameter : SrcItem
    {
        #region constructors
        public Parameter(int line) : base (line) { }
        public Parameter(Antlr4.Runtime.ParserRuleContext context) : base(context) { }
        #endregion constructors

    }
}
