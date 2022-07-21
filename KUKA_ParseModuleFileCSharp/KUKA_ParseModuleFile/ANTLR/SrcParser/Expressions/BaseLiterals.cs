using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser.Expressions
{
    public class Int : Literal 
    {
        #region fields
        private int value;
        #endregion fields

        #region properties
        public int Value { get { return value; } set { Set(ref this.value, value); } }
        #endregion properties

        #region constructors
        public Int(int Line, string ToConvert) 
            : base(Line, LiteralType.INT) 
        {
            value = int.Parse(ToConvert, NumberStyles.Integer);
        }
        public Int(Antlr4.Runtime.ParserRuleContext context, string ToConvert) 
            : base(context, LiteralType.INT)
        {
            value = int.Parse(ToConvert, NumberStyles.Integer);
        }
        #endregion constructors

        #region methods
        public override string ToString()
        {
            return value.ToString(NumberFormatInfo.InvariantInfo);
        }
        #endregion methods
    }

    public class Float : Literal
    {
        #region fields
        private double value;
        #endregion fields

        #region properties
        public double Value { get { return value; } set { Set(ref this.value, value); } }
        #endregion properties

        #region constructors
        public Float(int Line, string ToConvert) 
            : base(Line, LiteralType.FLOAT)
        {
            value = Parse(ToConvert);
        }
        public Float(Antlr4.Runtime.ParserRuleContext context, string ToConvert) 
            : base(context, LiteralType.FLOAT)
        {
            value = Parse(ToConvert);
        }
        #endregion constructors

        #region methods
        private double Parse(string Input)
        {
            double output = 0;
            if (!double.TryParse(Input, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out output))
                throw new ArgumentException();
            return output;
        }
        public override string ToString()
        {
            return value.ToString(NumberFormatInfo.InvariantInfo);
        }
        #endregion methods
    }

    public class Char : Literal
    {
        #region fields
        private char value;
        #endregion fields

        #region properties
        public char Value { get { return value; } set { Set(ref this.value, value); } }
        #endregion properties

        #region constructors
        public Char(int Line, string ToConvert)
            : base(Line, LiteralType.CHAR) 
        {
            string input = ToConvert.Substring(1, ToConvert.Length - 2);
            if (input.Length > 1) System.Diagnostics.Debugger.Break();
            value = input.ToCharArray().First();
        }
        public Char(Antlr4.Runtime.ParserRuleContext context, string ToConvert) 
            : base(context, LiteralType.CHAR)
        {
            string input = ToConvert.Substring(1, ToConvert.Length - 2);
            if (input.Length > 1) System.Diagnostics.Debugger.Break();
            value = input.ToCharArray().First();
        }
        #endregion constructors

        #region methods
        public override string ToString()
        {
            return "'" + value.ToString(CultureInfo.InvariantCulture) + "'";
        }
        #endregion methods
    }

    public class String : Literal
    {
        #region fields
        private string value;
        #endregion fields

        #region properties
        public string Value { get { return value; } set { Set(ref this.value, value); } }
        #endregion properties

        #region constructors
        public String(int Line, string ToConvert)
            : base(Line, LiteralType.STRING)
        {
            value = ToConvert.Trim('"');
        }
        public String(Antlr4.Runtime.ParserRuleContext context, string ToConvert)
            : base(context, LiteralType.STRING)
        {
            value = ToConvert.Trim('"');
        }
        #endregion constructors

        #region methods
        public override string ToString()
        {
            return "\"" + value + "\"";
        }
        #endregion methods
    }

    public class Bool : Literal
    {
        #region fields
        private bool value;
        #endregion fields

        #region properties
        public bool Value { get { return value; } set { Set(ref this.value, value); } }
        #endregion properties

        #region constructors
        public Bool(int Line, bool Value = false) 
            : base(Line, LiteralType.BOOL)
        {
            this.value = Value;
        }
        public Bool(int Line, string ToConvert)
            : base(Line, LiteralType.BOOL)
        {
            this.value = (ToConvert.ToUpperInvariant() == "TRUE");
        }
        public Bool(Antlr4.Runtime.ParserRuleContext context, bool Value = false)
            : base(context, LiteralType.BOOL)
        {
            this.value = Value;
        }
        public Bool(Antlr4.Runtime.ParserRuleContext context, string ToConvert)
            : base(context, LiteralType.BOOL)
        {
            this.value = (ToConvert.ToUpperInvariant() == "TRUE");
        }
        #endregion constructors

        #region methods
        public override string ToString()
        {
            return (value ? "TRUE" : "FALSE");
        }
        #endregion methods
    }
}
