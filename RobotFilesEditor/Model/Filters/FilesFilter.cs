using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor
{
    public class FilesFilter
    {
        public string OperationName
        { 
            get { return _operationName; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException(nameof(OperationName));
                }

                if (_operationName != value)
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
                if (value == null)
                {
                    _destinationFolder = string.Empty;
                }

                if (_destinationFolder != value)
                {
                    _destinationFolder = value;
                }
            }
        }
        public Filter Filter
        {
            get { return _filter; }
            set
            {
                if(_filter!=value)
                {
                    _filter = value;
                }
            }
        }
        public List<string> FileExtensions
        {
            get { return _fileExtensions; }
            set
            {
                if(_fileExtensions!=value)
                {
                    _fileExtensions = value;
                }
            }
        }
        
        private string _operationName;
        public GlobalData.Action _action;
        public string _destinationFolder;
        public List<string> _fileExtensions;
        public Filter _filter;
     
        public FilesFilter()
        {
            Action = GlobalData.Action.None;
            Filter = new Filter();         
            DestinationFolder = "";
        }        
    }
}
