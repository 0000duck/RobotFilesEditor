using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser
{
    public class SrcItem : NotifyPropertyChanged, ISrcItem
    {
        #region fields
        private int line;
        #endregion fields

        #region properties
        public int Line { get { return line; } set { Set(ref line, value); } }
        #endregion properties

        #region constructors
        public SrcItem(int line)
        {
            this.line = line;
        }
        public SrcItem(ParserRuleContext context)
        {
            this.line = context.Start.Line;
        }
        #endregion constructors
    }
}
