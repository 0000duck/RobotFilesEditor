using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Expressions
{
    public class ArrayVariableSuffix : SrcItem
    {
        #region fields
        private int level;
        private IExpression level1;
        private IExpression level2;
        private IExpression level3;
        #endregion fields

        #region properties
        public int Level { get { return level; } set { Set(ref level, value); } }
        public IExpression Level1 { get { return level1; } set { Set(ref this.level1, value); } }
        public IExpression Level2 { get { return level2; } set { Set(ref this.level2, value); } }
        public IExpression Level3 { get { return level3; } set { Set(ref this.level3, value); } }
        #endregion properties

        #region constructors
        public ArrayVariableSuffix(int Line) : base(Line) { }
        public ArrayVariableSuffix(Antlr4.Runtime.ParserRuleContext context) : base(context) { }
        #endregion constructors

        #region methods
        public override string ToString()
        {
            if (level == 0) return "";
            string output = (level1 != null ? level1.ToString() : "");
            if (level > 1)
                output += "," + (level2 != null ? level2.ToString() : "");
            if (level > 2)
                output += "," + (level3 != null ? level3.ToString() : "");
            return "[" + output + "]";
        }
        #endregion methods
    }
}
