using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor
{
    public class Operation
    {
        #region Public
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
        public string Source
        {
            get { return _source; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException(nameof(Source));
                }

                if (_source != value)
                {
                    if (Directory.Exists(value))
                    {
                        _source = value;
                    }
                    else
                    {
                        throw new DirectoryNotFoundException($"Source: \'{value} \'not exist!");
                    }
                }
            }
        }
        #endregion Public

        #region Private
        protected string _operationName;
        protected GlobalData.Action _action;
        protected string _destinationFolder;
        protected string _source;
        #endregion Private      

        public void FollowOperation()
        {
        }
    }
}
