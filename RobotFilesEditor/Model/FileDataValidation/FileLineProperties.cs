using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RobotFilesEditor
{
    public class FileLineProperties: IEquatable<FileLineProperties>
    {
        public string FileLinePath
        {
            get { return _fileLinePath; }
            set {
                    if(_fileLinePath!=value)
                    {
                        _fileLinePath = value;
                    }
            }
        }
        public int LineNumber
        {
            get { return _lineNumber; }
            set
            {
                if (_lineNumber != value && value>=0)
                {
                    _lineNumber = value;
                }
            }
        }
        public string LineContent
        {
            get { return _lineContent; }
            set
            {
                if (_lineContent != value)
                {
                    _lineContent = value;
                    VariableName = GetVariableName(value);
                }
            }
        }
        public string VariableName
        {
            get { return _variableName; }
            set
            {
                if(_variableName!=value)
                {
                    _variableName = value;
                    if(string.IsNullOrEmpty(_variableName))
                    {
                        VaribleIndex = GetVaribleIndex(_variableName);
                    }
                }
            }        
        }
        public int VaribleIndex
        {
            get { return _variableIndex; }
            set{
                    
                if(_variableIndex!=value)
                {
                    _variableIndex = value;
                }
            }      
        }

        private string _fileLinePath;
        private int _lineNumber;
        private string _lineContent;
        private string _variableName;
        private int _variableIndex;

        private string GetVariableName(string value)
        {
            string varPattern= @"[a-zA-Z]+[a-zA-Z0-9_]*[\[0-9\],]*=";
            //string varNamePattern= @"[a-zA-Z]+[a-zA-Z0-9_]*[\[0-9\],]*";
            Match match;
            string variableName = "";
            
            if(value.Length>0)
            {
                match=Regex.Match(value, varPattern);
                variableName=match.Value.Replace("=", string.Empty);
            }

            return variableName;
        }

        private int GetVaribleIndex(string value)
        {
            string indexPattern = @"\[[0-9]+,*\]";
            Regex indexRegex = new Regex(indexPattern);
            Match indexMatch;

            string valuePattern = "[0-9]+";
            Regex valueRegex = new Regex(valuePattern);
            Match valueMatch;

            int index = -1;

            indexMatch = indexRegex.Match(value);           

            if (string.IsNullOrEmpty(indexMatch.Value) == false)
            {
                valueMatch = valueRegex.Match(indexMatch.Value);

                int.TryParse(valueMatch.Value, out index);               
            }

            return index;
        }

        public bool Equals(FileLineProperties other)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
