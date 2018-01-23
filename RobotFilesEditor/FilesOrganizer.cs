using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor
{
    public class FilesOrganizer
    {
        public string OperationName
        {
            get { return _operationName; }
            set
            {
                if(string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException(nameof(OperationName));
                }

                if (_operationName!=value)
                {
                    _operationName = value;
                }
            }           
        }
        public GlobalData.Action Action
        {           
            get { return _action; }
            set
            {
                if (value==null)
                {
                    throw new ArgumentNullException(nameof(Action));
                }

                if (_action != value)
                {
                    _action = value;
                }
             }            
        }
        public string DestinationFolder
        {
            get { return _destinationFolder; }
            set
            {
                if (value==null)
                {
                    _destinationFolder = string.Empty;
                }

                if (_destinationFolder != value)
                {
                    _destinationFolder = value;
                }
            }            
        }
        public List<string> FileExtensions
        {
            get { return _fileExtensions; }
            set
            {
                if (value==null)
                {
                    _fileExtensions = new List<string>();
                }

                if(_fileExtensions!=value)
                {
                    _fileExtensions = value;
                }
            }           
        }
        public List<string> ContainsAtName
        {
            get { return _containsAtName; }
            set
            {
                if (value==null)
                {
                    _containsAtName = new List<string>();
                }

                if (_containsAtName != value)
                {
                    _containsAtName = value;
                }
            }            
        }
        public List<string> NotContainsAtName
        {
            get { return _notContainsAtName; }
            set
            {
                if (value==null)
                {
                    _notContainsAtName = new List<string>();
                }

                if (_notContainsAtName != value)
                {
                _notContainsAtName = value;
                }
            }          
        }

        public string RegexContain
        {
            get { return _regexContain; }
            set
            {
                if (value==null)
                {
                    _regexContain = "";
                }

                if (_regexContain != value)
                {
                    _regexContain = value;
                }
            }         
        }
        public string RegexNotContain
        {
            get { return _regexNotContain; }
            set
            {
                if (value == null)
                {
                    _regexContain = "";
                }            

                if (_regexNotContain != value)
                {
                    _regexNotContain = value;
                }
            }            
        }
        
        private string _operationName;
        public GlobalData.Action _action;
        public string _destinationFolder;
        public List<string> _fileExtensions;
        public List<string> _containsAtName;
        public List<string> _notContainsAtName;
        public string _regexContain;
        public string _regexNotContain;
     
        public FilesOrganizer()
        {
            FileExtensions = new List<string>();
            ContainsAtName = new List<string>();
            NotContainsAtName = new List<string>();

            Action = GlobalData.Action.None;
            RegexContain = "";
            DestinationFolder = "";
        }
        
    }
}
