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
                    }
                    else
                    {
                        throw new DirectoryNotFoundException($"Directory: \'{value} \'not exist!");
                    }
                }
            }
        }
        public Operations Operations
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

        public List<FilesFilter> FilesFilters
        {
            get { return _filesfilters; }
            set
            {
                if (value == null)
                {
                    _filesfilters = new List<FilesFilter>();
                }

                if (_filesfilters != value && value != null)
                {
                    _filesfilters = value;
                    OnPropertyChanged(nameof(FilesFilters));
                }
            }
        }
        private List<FilesDataFilter> FilesDataFilter
        {
            get { return _filesDataFilter; }
            set
            {
                if (_filesDataFilter != value)
                {
                    _filesDataFilter = value;
                }
            }
        }
       

        #endregion Public

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Private 
        private List<FilesFilter> _filesfilters;
        private List<FilesDataFilter> _filesDataFilter;
        private string _destinationPath;
        private string _sourcePath;
        private string _contolerType;
        private Operations _operations;
        #endregion Private 

        [NotifyPropertyChangedInvocatorAttribute]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Controler()
        {
            FilesFilters = new List<FilesFilter>();
            FilesDataFilter = new List<FilesDataFilter>();
        }

        public bool DoAction(string operation)
        {
            GlobalData.Action action = FilesFilters.Where(x => x.OperationName == operation).FirstOrDefault().Action;
            bool actionResult = false;

            switch (action)
            {
                case GlobalData.Action.Copy:
                    {
                        actionResult = CopyFile(operation);
                    }
                    break;
                case GlobalData.Action.Move:
                    {
                        actionResult = MoveFile(operation);
                    }
                    break;
                case GlobalData.Action.Remove:
                    {
                        actionResult = RemoveFile(operation);
                    }
                    break;
                case GlobalData.Action.CopyData:
                    {
                        actionResult = CopyData(operation);
                    }
                    break;
            }

            return actionResult;
        }

        private List<string> FiltrFiles(FilesFilter filter)
        {
            string[] allFilesAtSourcePath = Directory.GetFiles(SourcePath);
            List<string> filteredFiles = new List<string>();

            if (filter.FileExtensions.Count > 0)
            {
                filteredFiles = allFilesAtSourcePath.Where(x => filter.FileExtensions.Contains(Path.GetExtension(x))).ToList();
            }
            else
            {
                filteredFiles = allFilesAtSourcePath.ToList();
            }

            filteredFiles = filter.Filter.FilterContainsAtName(filteredFiles);
            filteredFiles = filter.Filter.FilterNotContainsAtName(filteredFiles);
            filteredFiles = filter.Filter.FilterRegexContain(filteredFiles);
            filteredFiles = filter.Filter.FilterRegexNotContain(filteredFiles);
            return filteredFiles;
        }

        public bool CopyFile(string operation)
        {
            FilesFilter filesOrganizer = FilesFilters.Where(x => x.OperationName == operation).FirstOrDefault();
            List<string> filteredFiles = FiltrFiles(filesOrganizer);
            string destination = CreateDestinationFolder(filesOrganizer.DestinationFolder);
            filteredFiles.ForEach(x => File.Copy(x, Path.Combine(destination, Path.GetFileName(x))));

            return CheckFilesCorrectness(destination, filteredFiles);
        }

        public bool MoveFile(string operation)
        {
            FilesFilter filesOrganizer = FilesFilters.Where(x => x.OperationName == operation).FirstOrDefault();
            List<string> filteredFiles = FiltrFiles(filesOrganizer);
            string destination = CreateDestinationFolder(filesOrganizer.DestinationFolder);
            filteredFiles.ForEach(x => File.Move(x, Path.Combine(destination, Path.GetFileName(x))));

            return CheckFilesCorrectness(destination, filteredFiles);
        }

        public bool RemoveFile(string operation)
        {
            FilesFilter filesOrganizer = FilesFilters.Where(x => x.OperationName == operation).FirstOrDefault();
            List<string> filteredFiles = FiltrFiles(filesOrganizer);
            filteredFiles.ForEach(x => File.Delete(x));

            return CheckFilesCorrectness(SourcePath, filteredFiles) == false;
        }

        public bool CopyData(string operation)
        {
            FilesFilter filesOrganizer = FilesFilters.Where(x => x.OperationName == operation).FirstOrDefault();
            List<string> filteredFiles = FiltrFiles(filesOrganizer);

            filteredFiles.ForEach(x => File.Delete(x));

            return CheckFilesCorrectness(SourcePath, filteredFiles) == false;
        }

        public bool CutData(string operation)
        {
            throw new NotImplementedException();
        }

        public bool CreateNewFileFromData(string operation)
        {
            throw new NotImplementedException();
        }

        string CreateDestinationFolder(string newFolder)
        {
            string destinationPath = Path.Combine(_destinationPath, newFolder);

            if (Directory.Exists(destinationPath) == false)
            {
                Directory.CreateDirectory(destinationPath);
            }

            return destinationPath;
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

        private List<string> LoadFiles(List<string> filesPaths)
        {
            List<string> filesContent = new List<string>();

            foreach (string path in filesPaths)
            {
                filesContent.AddRange(File.ReadAllLines(path).ToList());
            }
            return filesContent;
        }

        private FilesDataFilter GetGroups(List<string> filesContent, FilesDataFilter fileDataFilter)
        {
            fileDataFilter.DataFilterGroups.ForEach(x => x.SetLinesToAddToFile(filesContent));

            return fileDataFilter;
        }

        private void CreateDestinationFile()
        {

        }

        private void WriteData(FilesDataFilter fileDataFilter)
        {
            foreach (var filter in fileDataFilter.DataFilterGroups)
            {

            }
        }
    }
}
