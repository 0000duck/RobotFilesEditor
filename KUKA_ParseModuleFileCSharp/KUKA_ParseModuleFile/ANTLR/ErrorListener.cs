using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR
{
    public class ErrorListener : IAntlrErrorListener<IToken>
    {
        #region fields
        private string filename;
        private string archive;
        #endregion fields

        #region constructors
        public ErrorListener(string archive, string filename)
        {
            this.archive = archive;
            this.filename = filename;
        }
        #endregion constructors

        public void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            Console.WriteLine(String.Format("Error in archive {0}, file {1}, line {2}, column {3}: {4}",
                System.IO.Path.GetFileNameWithoutExtension(archive), System.IO.Path.GetFileName(filename), line, charPositionInLine, msg));
            //System.Diagnostics.Debugger.Break();
        }
    }
}
