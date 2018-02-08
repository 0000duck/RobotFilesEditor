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
        public Dictionary<string, string> FilteredFiles
        {
            get { return _filteredFiles; }
            set
            {
                if(_filteredFiles!=value)
                {
                    _filteredFiles = value;
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
        private Dictionary<string,string> _filteredFiles;
        #endregion Private
    
        public  FileOperation()
        {
            FileExtensions = new List<string>();
            FilteredFiles = new Dictionary<string, string>();
        }

        private void FiltrFiles()
        {
            string[] allFilesAtSourcePath = Directory.GetFiles(SourcePath);
            List<string> filteredFiles = new List<string>();
            FilteredFiles = new Dictionary<string, string>();

            try
            {
                if (FileExtensions.Count > 0)
                {
                    filteredFiles = allFilesAtSourcePath.Where(x => FileExtensions.Contains(Path.GetExtension(x))).ToList();
                }
                else
                {
                    filteredFiles = allFilesAtSourcePath.ToList();
                }

                filteredFiles = Filter.CheckAllFilesFilters(filteredFiles);
                filteredFiles.ForEach(x => FilteredFiles.Add(x, ""));
            }
            catch (Exception ex)
            {
                throw ex;
            }                
        }       
        public bool CopyFile()
        {
            bool result = true;
            FiltrFiles();
            string destination = CreateDestinationFolderPath();
            IDictionary<string, string> filteredFilesIterator = new Dictionary<string, string>(FilteredFiles);
            
            foreach(var file in filteredFilesIterator)
            {
                try
                {
                    string filePath = Path.Combine(destination, Path.GetFileName(file.Key));

                    if(File.Exists(filePath))
                    {
                        throw new IOException($"File \"{Path.GetFileName(file.Key)}\" already exist!");
                    }

                    File.Copy(file.Key, filePath);
                }                
                catch (Exception ex)
                {
                    FilteredFiles.Remove(file.Key);
                    FilteredFiles.Add(file.Key, ex.Message);
                    result = false;                                
                }                
            }

            if(result)
            {
                result = CheckFilesCorrectness(destination);
            }

            return result;
        }
        public bool MoveFile()
        {
            bool result = true;
            string destination = CreateDestinationFolderPath();
            IDictionary<string, string> filteredFilesIterator = new Dictionary<string, string>(FilteredFiles);

            try
            {
                FiltrFiles();
            }
            catch (Exception ex)
            {
                throw ex;
            }            

            FilteredFiles.Keys.ToList().ForEach(x => File.Move(x, Path.Combine(destination, Path.GetFileName(x))));

            foreach (var file in filteredFilesIterator)
            {
                try
                {
                    string filePath = Path.Combine(destination, Path.GetFileName(file.Key));

                    if (File.Exists(filePath))
                    {
                        throw new IOException($"File \"{Path.GetFileName(file.Key)}\" already exist!");
                    }

                    File.Move(file.Key, filePath);
                }
                catch (Exception ex)
                {
                    FilteredFiles.Remove(file.Key);
                    FilteredFiles.Add(file.Key, ex.Message);
                    result = false;
                }
            }

            if (result)
            {
                result = CheckFilesCorrectness(destination);
            }

            return result;
        }
        public bool RemoveFile()
        {
            bool result = true;
            IDictionary<string, string> filteredFilesIterator = new Dictionary<string, string>(FilteredFiles);

            try
            {
                FiltrFiles();
            }
            catch (Exception ex)
            {
                throw ex;
            }        

            foreach (var file in filteredFilesIterator)
            {
                try
                {
                    File.Delete(file.Key);
                }
                catch (Exception ex)
                {
                    FilteredFiles.Remove(file.Key);
                    FilteredFiles.Add(file.Key, ex.Message);
                    result = false;
                }
            }

            if (result)
            {
                result = CheckFilesCorrectness(SourcePath)==false;
            }

            return result;
        }
        public string CreateDestinationFolderPath()
        {
            string destination = Path.Combine(DestinationPath, DestinationFolder);

            try
            {
                if (Directory.Exists(destination) == false)
                {
                    Directory.CreateDirectory(destination);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }            

            return destination;
        }
        bool CheckFilesCorrectness(string path)
        {
            List<string> resultFiles = new List<string>();

            try
            {
                resultFiles = Directory.GetFiles(path).ToList();

                if (FilteredFiles.Keys.ToList().Exists(s => resultFiles.Exists(r => Path.GetFileName(r) == Path.GetFileName(s)) == false))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }      
        public List<string>GetOperatedFiles()
        {           
            return FilteredFiles.Keys.ToList();
        }

        #region InterfaceImplementation
        public void ExecuteOperation()
        {
            List<string> result = new List<string>();

            try
            {
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
                    default:
                        {
                            FiltrFiles();
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }
        public List<ResultInfo> GetOperationResult()
        {
            List<ResultInfo> resultInfos = new List<ResultInfo>();
            ResultInfo resultInfo = new ResultInfo();

            try
            {
                foreach (var file in FilteredFiles)
                {
                    resultInfo = new ResultInfo();
                    resultInfo.Content = Path.GetFileName(file.Key);
                    resultInfo.Path = file.Key;
                    resultInfo.Description = file.Value;
                    resultInfos.Add(resultInfo);
                }                
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resultInfos;
        }

        public void ClearMemory()
        {
            FilteredFiles = new Dictionary<string, string>();
        }
        #endregion InterfaceImplementation
    }
}
