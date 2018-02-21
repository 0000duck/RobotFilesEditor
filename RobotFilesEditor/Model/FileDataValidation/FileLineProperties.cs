using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RobotFilesEditor
{
    public class FileLineProperties
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
                    Variable = GetVariable(value);                   
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
                }
            }        
        }
        public int VariableIndex
        {
            get { return _variableIndex; }
            set{
                    
                if(_variableIndex!=value)
                {
                    _variableIndex = value;
                }
            }      
        }
        public string Variable
        {
            get { return _variable; }
            set
            {
                if(_variable!=value)
                {
                    _variable = value;
                    VariableName = GetVariableName(Variable);
                    VariableOrderNumber = GetVariableOrderNumber(VariableName);

                    if (string.IsNullOrEmpty(VariableName) == false)
                    {
                        VariableIndex = GetVaribleIndex(Variable);
                    }
                }
            }
        }
        public int VariableOrderNumber
        {
            get { return _variableOrderNumber; }
            set
            {
                if (_variableOrderNumber != value)
                {
                    _variableOrderNumber = value;
                }
            }
        }
        public bool HasExeption
        {
            get { return _hasExeption; }
            set
            {
                if(_hasExeption!=value)
                {
                    _hasExeption = value;
                }
            }
        }

        private string _fileLinePath;
        private int _lineNumber;
        private int _variableOrderNumber;
        private string _lineContent;
        private string _variableName;
        private int _variableIndex;
        private string _variable;
        private bool _hasExeption;

        private string GetVariableName(string value)
        {               
            string varNamePattern= @"[a-zA-Z]+[a-zA-Z0-9_]*";
            Match match;
            string variableName = "";
            
            if(Variable.Length>0)
            {
                match=Regex.Match(Variable, varNamePattern);
                variableName = match.Value;
            }
            return variableName;
        }

        private string GetVariable(string value)
        {
            string varPattern = @"[a-zA-Z]+[a-zA-Z0-9_]*[\[0-9\],]*=";            
            Match match;
            string variableName = "";

            if (value.Length > 0)
            {
                match = Regex.Match(value, varPattern);
                variableName = match.Value.Replace("=", string.Empty);
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

            indexMatch = indexRegex.Match(Variable);           

            if (string.IsNullOrEmpty(indexMatch.Value) == false)
            {
                valueMatch = valueRegex.Match(indexMatch.Value);

                int.TryParse(valueMatch.Value, out index);               
            }

            return index;
        }   
        
        private int GetVariableOrderNumber(string variable)
        {
            string pattern = @"\d+\b";
            int orderNumber = 0;
            Regex regex = new Regex(pattern);
            Match match;

            match = regex.Match(Variable);

            if (string.IsNullOrEmpty(match.Value) == false)
            {
                int.TryParse(match.Value, out orderNumber);
            }

            return orderNumber;
        }  
    }
}
