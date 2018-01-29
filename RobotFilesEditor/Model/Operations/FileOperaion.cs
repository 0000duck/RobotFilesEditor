using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RobotFilesEditor
{
    public class FileOperation: Operation, IFileOperations
    {
        #region Public
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
        private List<string> _fileExtensions;
        private Filter _filter;       
        #endregion Private

        public new void FollowOperation()
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
            }
        }
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
        private bool _nestedSourcePath;

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

    }
}
