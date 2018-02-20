using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace RobotFilesEditor
{
    public class Controler : INotifyPropertyChanged
    {
        #region Public

        public string ContolerType
        {
            get { return _contolerType; }
            set
            {
                if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(nameof(ContolerType));
                }

                if (_contolerType != value)
                {
                    _contolerType = value;
                    OnPropertyChanged(nameof(_contolerType));
                }
            }
        }
        public string DestinationPath
        {
            get { return _destinationPath; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException(nameof(DestinationPath));
                }

                if (_destinationPath != value)
                {
                    if (Directory.Exists(value))
                    {
                        _destinationPath = value;
                        OnPropertyChanged(nameof(DestinationPath));
                        Operations.ForEach(x => x.DestinationPath = DestinationPath);
                    }
                    else
                    {
                        throw new DirectoryNotFoundException($"Directory \'{value} \'not exist!");
                    }
                }
            }
        }
        public string SourcePath
        {
            get { return _sourcePath; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException(nameof(DestinationPath));
                }

                if (_sourcePath != value)
                {
                    if (Directory.Exists(value))
                    {
                        _sourcePath = value;
                        OnPropertyChanged(nameof(SourcePath));
                        Operations.ForEach(x => x.SourcePath = SourcePath);
                    }
                    else
                    {
                        throw new DirectoryNotFoundException($"Directory: \'{value} \'not exist!");
                    }
                }
            }
        }
        public List<IOperation> Operations
        {
            get { return _operations; }
            set
            {
                if(_operations!=value)
                {
                    _operations = value;
                }
            }
        }      
        #endregion Public

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion Events

        #region Private 
        private string _destinationPath;
        private string _sourcePath;
        private string _contolerType;
        private List<IOperation> _operations;
        private IOperation _activeOperation;
        private List<ResultInfo> _resultInfos;
        #endregion Private 

        [NotifyPropertyChangedInvocatorAttribute]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Controler()
        {
            Operations = new List<IOperation>();
            _resultInfos = new List<ResultInfo>();
        }       

        public void ExecuteOperation(string operationName)
        {
            List<IOperation> activeOperations = new List<IOperation>();
            List<string> exeptions = new List<string>();
            _resultInfos = new List<ResultInfo>();

            activeOperations = Operations.Where(x => x.OperationName.Equals(operationName)).OrderBy(y=>y.Priority).ToList();
            foreach(var operation in activeOperations)
            {
                try
                {                             
                    _activeOperation = operation;
                    _activeOperation.ExecuteOperation();
                    _resultInfos.AddRange(_activeOperation.GetOperationResult());
                }
                catch (Exception ex)
                {
                    throw ex;   
                }               
            }
        }

        public List<ResultInfo>GetTextToPrint()
        {
            return _resultInfos;
        }        
    }
}
