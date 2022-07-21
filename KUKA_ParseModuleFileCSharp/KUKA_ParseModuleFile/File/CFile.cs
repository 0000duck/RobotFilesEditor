using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WarningHelper;

namespace ParseModuleFile.File
{
    public abstract class CFile
    {
	    //private List<string> c;
        internal Stream stream;
        internal Warnings _warnings;
        internal string fileName;
	    static internal int current_line;

        public List<string> InfoList;

	    #region constants - regular expressions patterns
	    internal const string _s = "\\s*";
        internal const string  _comment = "^" + _s + ";(.+)";
        #endregion // constants - regular expressions patterns

        #region regex declarations
        //static internal Regex reComment = new Regex(_comment, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        #endregion // regex declarations

        #region properties
        //public List<string> contents {
        //    get { return c; }
        //}
	    public string FileName {
		    get { return fileName; }
	    }
        #endregion // properties

	    #region constructors
        public CFile(Warnings warnings)
	    {
		    _warnings = warnings;
	    }

        public CFile(string fileName, Stream stream, Warnings warnings)
        {
            _warnings = warnings;
            this.stream = stream;
            this.fileName = fileName;
            ParseStream();
        }

        #endregion // constructors

        #region methods
        protected abstract void ParseStream();

        //public override string ToString()
        //{
        //    return string.Join(Environment.NewLine, c);
        //}

        #endregion	    
    }
}
