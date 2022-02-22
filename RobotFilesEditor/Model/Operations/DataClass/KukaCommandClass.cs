using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RobotFilesEditor.Model.Operations.DataClass
{
    public class KukaCommandClass
    {
        #region properties
        public string Content { get; set; }
        public bool IsSpecialComment { get; private set; }
        public bool IsSingleInstruction { get; private set; }
        public bool IsComment { get; private set; }
        public bool IsMeaningfulFold { get; private set; }
        public bool IsMotionFoldFold { get; private set; }
        public bool IsTriggeredAction { get; private set; }
        public bool IsEnd { get; private set; }
        public bool IsCollisionReqRel { get; private set; }

        #endregion

        #region ctor
        public KukaCommandClass(string content)
        {
            Content = content;
            AnalyzeContent();
        }
        #endregion

        #region private methods
        private void AnalyzeContent()
        {
            IsMeaningfulFold = new Regex(@"^\s*[\&\$\w]+", RegexOptions.IgnoreCase|RegexOptions.Multiline).IsMatch(Content) && new Regex(@"^\s*;\s*fold", RegexOptions.IgnoreCase | RegexOptions.Multiline).IsMatch(Content);
            IsMotionFoldFold = IsMeaningfulFold && new Regex(@"^\s*(PTP|LIN|CIRC)\s+.*", RegexOptions.IgnoreCase | RegexOptions.Multiline).IsMatch(Content);
            IsTriggeredAction = IsMeaningfulFold && new Regex(@"^\s*(TRIGGER)\s+.*\(.*\)", RegexOptions.IgnoreCase | RegexOptions.Multiline).IsMatch(Content);
            IsSpecialComment = new Regex(@"^\s*;\s*\*", RegexOptions.IgnoreCase | RegexOptions.Multiline).IsMatch(Content);
            IsComment = !IsSpecialComment && !IsMeaningfulFold && new Regex(@"^\s*;", RegexOptions.IgnoreCase|RegexOptions.Multiline).IsMatch(Content);
            IsSingleInstruction = !IsComment && new Regex(@"^\s*[\&\$\w]+", RegexOptions.IgnoreCase).IsMatch(Content);
            IsEnd = IsSingleInstruction && new Regex(@"^\s*END\s+", RegexOptions.IgnoreCase).IsMatch(Content);
            IsCollisionReqRel = IsMeaningfulFold && new Regex(@"^\s*Plc_Coll(Safety|Prepare)", RegexOptions.IgnoreCase | RegexOptions.Multiline).IsMatch(Content);
            var validator = this.GetType().GetProperties().ToList().Where(x => x.PropertyType.Name == "Boolean").ToList().Any(y => (y.GetValue(this) as bool?) == true);
            if (!validator)
                MessageBox.Show("Nieznana instrukcja");

        }
        #endregion
    }

}
