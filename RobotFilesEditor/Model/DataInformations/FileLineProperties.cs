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
        public string Variable
        {
            get { return _variable; }
            set
            {
                if (_variable != value)
                {
                    _variable = value;
                    VariableName = GetVariableName(Variable);


                    if (string.IsNullOrEmpty(VariableName) == false)
                    {
                        VariableIndex = GetVaribleIndex(Variable);
                        VariableOrderNumber = GetVariableOrderNumber(VariableName);
                        RobotNumber = GetRobotNumber(VariableName);
                    }
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
        public string RobotNumber
        {
            get; set;
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
            try
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
            catch (Exception ex)
            {
                throw ex;
            }           
        }

        private int GetVaribleIndex(string value)
        {
            try
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
            catch (Exception ex)
            {
                throw ex;
            }           
        }   
        
        private int GetVariableOrderNumber(string variable)
        {
            string pattern = @"\d+\b";
            int orderNumber = 0;
            Regex regex = new Regex(pattern);
            Match match;

            try
            {
                match = regex.Match(Variable);

                if (string.IsNullOrEmpty(match.Value) == false)
                {
                    int.TryParse(match.Value, out orderNumber);
                }

                return orderNumber;
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }  

        private string GetRobotNumber(string variable)
        {
            string robotNumber = string.Empty;
            string pattern = @"\d{3}IR\d{3}";          
            Match match;

            try
            {
                match = Regex.Match(Variable, pattern);

                if (string.IsNullOrEmpty(match.Value) == false)
                {
                    robotNumber = match.Value;
                    return robotNumber;
                }

                return robotNumber;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
