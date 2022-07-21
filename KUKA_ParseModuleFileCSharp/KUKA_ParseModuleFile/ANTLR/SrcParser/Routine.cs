using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser
{
    public class RoutineBody : SrcItem
    {
        #region constructors
        public RoutineBody(int Line) : base(Line) { }
        public RoutineBody(ParserRuleContext context) : base(context) { }
        #endregion constructors
    }
}
