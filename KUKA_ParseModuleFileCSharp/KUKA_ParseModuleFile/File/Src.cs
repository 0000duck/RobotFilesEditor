using WarningHelper;
using ParseModuleFile.KUKA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.IO;

namespace ParseModuleFile.File
{
    public class Src : CFile
    {
        private oldFolds folds;
        private string actDef;

        public oldFolds Folds { get { return folds; } set { folds = value; } }

        private static int fold_level = 0;

        private static oldFolds fold_stack = new oldFolds();
        #region constants - regular expressions patterns
        private const string _StartFOLD = "^;FOLD (?<name>.+)";
        private const string _EndFOLD = "^;ENDFOLD(.*)";
        private const string defLine = @"DEF\s*(?<name>[$\w]*)\s*\(";
        #endregion
        #region regex declarations
        //private static Regex reStartFOLD = new Regex(_StartFOLD, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        //private static Regex reEndFOLD = new Regex(_EndFOLD, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        //private static Regex reDefLine = new Regex(defLine, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        #endregion

        #region " Constructors "
        public Src(string fileName, Stream stream, Warnings warnings)
            : base(fileName, stream, warnings)
        {
        }
        #endregion

        protected override void ParseStream()
        {
            fold_level = 0;
            folds = new oldFolds();
            fold_stack = new oldFolds();
            int i = 0;
            using (var reader = new StreamReader(stream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    oldFold var = ParseSRCLine(i, line, System.IO.Path.GetFileNameWithoutExtension(base.fileName));
                    if (var != null && var.LineStart > -1 && var.LineEnd > -1)
                    {
                        folds.Add(var);
                    }
                    i += 1;
                }
            }
        }

        public oldFold ParseSRCLine(int line_number, string line, string module_name)
        {
            string trimmedLine = line.Trim();
            if (trimmedLine.Length == 0) return null;
            bool isComment = RegexHelper.IsMatch(_comment, trimmedLine);
            if (fold_level == 0 && isComment) return null;

            bool isStartFold = RegexHelper.IsMatch(_StartFOLD, trimmedLine);
            bool isEndFold = (!isStartFold) && RegexHelper.IsMatch(_EndFOLD, trimmedLine);
            if (RegexHelper.IsMatch(defLine, trimmedLine))
            {
                Match m = RegexHelper.Match(defLine, trimmedLine);
                actDef = m.Groups["name"].Value.ToUpperInvariant();
            }

            oldFold functionReturnValue = new oldFold(actDef, -1, _warnings);
            current_line = line_number;

            if (isStartFold)
            {
                Match m = RegexHelper.Match(_StartFOLD, trimmedLine);
                for (int i = 0; i <= fold_level - 1; i++)
                {
                    oldFold f = fold_stack[i];
                    f.Contents.AppendLine(line);
                    fold_stack[i] = f;
                }
                fold_level += 1;
                _warnings.Add(current_line, WarningType.Deep_Intern, Level.Information, "Start of fold at level: " + fold_level.ToString());
                fold_stack.Add(new oldFold(actDef, current_line, line + Environment.NewLine, m.Groups["name"].Value, _warnings));
            }
            else if (isEndFold & fold_stack.Count > 0)
            {
                _warnings.Add(current_line, WarningType.Deep_Intern, Level.Information, "End of fold at level:" + fold_level.ToString());
                for (int i = 0; i <= fold_level - 1; i++)
                {
                    oldFold f = fold_stack[i];
                    f.Contents.AppendLine(line);
                    fold_stack[i] = f;
                }
                fold_level -= 1;
                functionReturnValue = fold_stack[fold_stack.Count - 1];
                functionReturnValue.LineEnd = current_line;
                fold_stack.RemoveAt(fold_stack.Count - 1);
            }
            else
            {
                for (int i = 0; i <= fold_level - 1; i++)
                {
                    oldFold f = fold_stack[i];
                    f.Contents.AppendLine(line);
                    fold_stack[i] = f;
                }
            }
            return functionReturnValue;
        }


    }
}
