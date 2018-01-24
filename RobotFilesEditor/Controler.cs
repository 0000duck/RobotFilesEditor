using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor
{
    public class Controler: INotifyPropertyChanged, IFileOperations, IFileDataOperations
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

                if(_contolerType!=value)
                {
                    _contolerType = value;
                    OnPropertyChanged(nameof(_contolerType));
                }
            }
        }
        public List<FilesOrganizer>FilesFilters
        {
            get { return _filesfilters; }
            set
            {
                if(value==null)
                {
                    _filesfilters=new List<FilesOrganizer>();
                }

                if(_filesfilters!=value && value!=null)
                {
                    _filesfilters = value;
                    OnPropertyChanged(nameof(FilesFilters));
                }
            }
        }
        public string DestinationPath
        {
            get { return _destinationPath; }
            set
            {
                if(string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException(nameof(DestinationPath));
                }

                if (_destinationPath!=value)
                {
                    if (Directory.Exists(value))
                    {
                        _destinationPath = value;
                        OnPropertyChanged(nameof(DestinationPath));
                    }else
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
        #endregion Public

        public event PropertyChangedEventHandler PropertyChanged;
        
        #region Private 
        private List<FilesOrganizer> _filesfilters;
        private string _destinationPath;
        private string _sourcePath;
        private string _contolerType;
        #endregion Private 

        [NotifyPropertyChangedInvocatorAttribute]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        public Controler()
        {
            FilesFilters = new List<FilesOrganizer>();
        }

        public bool DoAction(string operation)
        {
            GlobalData.Action action = FilesFilters.Where(x => x.OperationName == operation).FirstOrDefault().Action;
            bool actionResult = false;

            switch(action)
            {
                case GlobalData.Action.Copy: {
                        actionResult=CopyFile(operation);
                    } break;               
                case GlobalData.Action.Move: {
                        actionResult = MoveFile(operation);
                    } break;
                case GlobalData.Action.Remove: {
                        actionResult = RemoveFile(operation);
                    } break;
                case GlobalData.Action.CopyData: {
                        actionResult = CopyData(operation);
                    } break;
            }

            return actionResult;
        }

        private List<string>FiltrFiles(FilesOrganizer filter)
        {
            string[] allFilesAtSourcePath = Directory.GetFiles(SourcePath);
            List<string> filteredFiles = new List<string>();

            if(filter.FileExtensions.Count>0)
            {
                filteredFiles = allFilesAtSourcePath.Where(x => filter.FileExtensions.Contains(Path.GetExtension(x))).ToList();
            }else
            {
                filteredFiles = allFilesAtSourcePath.ToList();
            }            
           
            if(filter.ContainsAtName.Count>0)
            {
                filteredFiles = filteredFiles.Where(x =>filter.ContainsAtName.Exists(y=>x.Contains(y))).ToList();
            }

            if (filter.NotContainsAtName.Count > 0)
            {
                filteredFiles = filteredFiles.Where(x => filter.NotContainsAtName.Exists(y => x.Contains(y))==false).ToList();
            }

            if(string.IsNullOrEmpty(filter.RegexContain)==false)
            {
                filteredFiles = filteredFiles.Where(x => System.Text.RegularExpressions.Regex.IsMatch(x, filter.RegexContain)).ToList();
            }

            if (string.IsNullOrEmpty(filter.RegexNotContain) == false)
            {
                filteredFiles = filteredFiles.Where(x => System.Text.RegularExpressions.Regex.IsMatch(x, filter.RegexNotContain)==false).ToList();
            }

            return filteredFiles;
        }

        public bool CopyFile(string operation)
        {
            FilesOrganizer filesOrganizer = FilesFilters.Where(x => x.OperationName == operation).FirstOrDefault();
            List<string> filteredFiles = FiltrFiles(filesOrganizer);
            string destination = CreateDestinationFolder(filesOrganizer.DestinationFolder);
            filteredFiles.ForEach(x => File.Copy(x, Path.Combine(destination, Path.GetFileName(x))));    

            return CheckFilesCorrectness(destination, filteredFiles);
        }

        public bool MoveFile(string operation)
        {
            FilesOrganizer filesOrganizer = FilesFilters.Where(x => x.OperationName == operation).FirstOrDefault();
            List<string> filteredFiles = FiltrFiles(filesOrganizer);
            string destination = CreateDestinationFolder(filesOrganizer.DestinationFolder);
            filteredFiles.ForEach(x => File.Move(x, Path.Combine(destination, Path.GetFileName(x))));

            return CheckFilesCorrectness(destination, filteredFiles);
        }

        public bool RemoveFile(string operation)
        {
            FilesOrganizer filesOrganizer = FilesFilters.Where(x => x.OperationName == operation).FirstOrDefault();
            List<string> filteredFiles = FiltrFiles(filesOrganizer);
            filteredFiles.ForEach(x => File.Delete(x));

            return CheckFilesCorrectness(SourcePath, filteredFiles)==false;
        }

        public bool CopyData(string operation)
        {
            throw new NotImplementedException();
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

            if(Directory.Exists(destinationPath)==false)
            {
                Directory.CreateDirectory(destinationPath);
            }

            return destinationPath;
        }

        bool CheckFilesCorrectness(string path, List<string>sourceFiles)
        {
            List<string>resultFiles = Directory.GetFiles(path).ToList();
            
            if (sourceFiles.Exists(s=>resultFiles.Exists(r=>Path.GetFileName(r)== Path.GetFileName(s)) ==false))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
