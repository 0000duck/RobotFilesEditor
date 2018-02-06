using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RobotFilesEditor
{
    public class FileOperation: IOperation, IFileOperations
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
        public GlobalData.Action ActionType
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
        public string SourcePath
        {
            get { return _sourcePath; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException(nameof(SourcePath));
                }

                if (_sourcePath != value)
                {
                    if (Directory.Exists(value))
                    {
                        _sourcePath = value;
                    }
                    else
                    {
                        throw new DirectoryNotFoundException($"Source: \'{value} \'not exist!");
                    }
                }
            }
        }
        public string DestinationPath
        {
            get { return _destinationPath; }
            set
            {
                if (value == null)
                {
                    _destinationPath = string.Empty;
                }

                if (_destinationPath != value)
                {
                    _destinationPath = value;
                }
            }
        }
        public int Priority
        {
            get { return _priority; }
            set
            {
                if (_priority != value)
                {
                    _priority = value;
                }
            }
        }
        public Filter Filter
        {
            get { return _filter; }
            set
            {
                if (_filter != value)
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
                if (_fileExtensions != value)
                {
                    _fileExtensions = value;                    
                }
            }
        }     
        public bool NestedSourcePath
        {
            get { return _nestedSourcePath; }
            set
            {
                if(_nestedSourcePath != value)
                {
                    _nestedSourcePath = value;
                }
            }
        }
        #endregion Public

        #region Private
        protected string _operationName;
        protected GlobalData.Action _action;
        protected string _destinationFolder;
        protected string _sourcePath;
        protected string _destinationPath;
        protected int _priority;
        private List<string> _fileExtensions;
        private Filter _filter;
        private bool _nestedSourcePath;
        private List<string> _fileredFiles;
        #endregion Private
    
        private List<string> FiltrFiles()
        {
            string[] allFilesAtSourcePath = Directory.GetFiles(SourcePath);
            List<string> filteredFiles = new List<string>();

            if (FileExtensions.Count > 0)
            {
                filteredFiles = allFilesAtSourcePath.Where(x => FileExtensions.Contains(Path.GetExtension(x))).ToList();
            }
            else
            {
                filteredFiles = allFilesAtSourcePath.ToList();
            }

            filteredFiles = Filter.CheckAllFilesFilters(filteredFiles);

            return filteredFiles;
        }       
        public bool CopyFile()
        {
            List<string> filteredFiles = FiltrFiles();
            string destination = CreateDestinationFolder();
            filteredFiles.ForEach(x => File.Copy(x, Path.Combine(destination, Path.GetFileName(x))));

            return CheckFilesCorrectness(destination, filteredFiles);
        }
        public bool MoveFile()
        {          
            List<string> filteredFiles = FiltrFiles();

            string destination = CreateDestinationFolder();
            filteredFiles.ForEach(x => File.Move(x, Path.Combine(destination, Path.GetFileName(x))));

            return CheckFilesCorrectness(destination, filteredFiles);
        }
        public bool RemoveFile()
        {
            List<string> filteredFiles = FiltrFiles();
            filteredFiles.ForEach(x => File.Delete(x));

            return CheckFilesCorrectness(SourcePath, filteredFiles) == false;
        }
        string CreateDestinationFolder()
        {
            string destination = Path.Combine(DestinationPath, DestinationFolder);

            if (Directory.Exists(destination) == false)
            {
                Directory.CreateDirectory(destination);
            }

            return destination;
        }
        bool CheckFilesCorrectness(string path, List<string> sourceFiles)
        {
            List<string> resultFiles = Directory.GetFiles(path).ToList();

            if (sourceFiles.Exists(s => resultFiles.Exists(r => Path.GetFileName(r) == Path.GetFileName(s)) == false))
            {
                return false;
            }
            else
            {
                return true;
            }
        }      
        public List<string>GetOperatedFiles()
        {
            _fileredFiles = FiltrFiles();
            return _fileredFiles;
        }

        #region InterfaceImplementation
        public void ExecuteOperation()
        {
            List<string> result = new List<string>();

            switch (ActionType)
            {
                case GlobalData.Action.Copy:
                    {
                        CopyFile();
                    }
                    break;
                case GlobalData.Action.Move:
                    {
                        MoveFile();
                    }
                    break;
                case GlobalData.Action.Remove:
                    {
                        RemoveFile();
                    }
                    break;
                default :
                    {
                        _fileredFiles = FiltrFiles();
                    }
                    break;
            }
        }
        public List<string> GetOperationResult()
        {
            return _fileredFiles;
        }
        #endregion InterfaceImplementation
    }
}
