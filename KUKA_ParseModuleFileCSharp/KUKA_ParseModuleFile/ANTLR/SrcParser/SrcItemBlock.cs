using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser
{
    public class SrcItemBlock : SrcItem, ISrcItemBlock
    {
        #region fields
        private int endLine;
        #endregion // fields

        #region properties
        public int EndLine { get { return endLine; } set { Set(ref endLine, value); } }
        #endregion // properties

        #region constructors
        public SrcItemBlock(int Line, int EndLine)
            : base(Line)
        {
            this.endLine = EndLine;
        }
        public SrcItemBlock(ParserRuleContext context)
            : base(context)
        {
            this.endLine = context.Stop.Line;
        }
        #endregion constructors
    }
}
