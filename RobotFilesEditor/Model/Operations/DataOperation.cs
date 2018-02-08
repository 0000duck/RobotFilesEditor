using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RobotFilesEditor
{
    public class DataOperation: IOperation, IFileDataOperations
    {
        public FileOperation FileOperation
        {
            get { return _fileOperation; }
            set
            {
                if(_fileOperation!=value)
                {
                    _fileOperation = value;
                }
            }
        }

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
                        FileOperation.SourcePath = SourcePath;
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
                    FileOperation.DestinationPath = DestinationPath;
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
        public List<DataFilterGroup> DataFilterGroups
        {
            get { return _dataFilterGroups; }
            set
            {
                if (_dataFilterGroups != value)
                {
                    _dataFilterGroups = value;
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
        public string DestinationFilePath
        {
            get { return _destinationFilePath; }
            set
            {
                if (_destinationFilePath != value)
                {
                    _destinationFilePath = value;
                }
            }
        }
        public string DestinationFileSource
        {
            get { return _destinationFileSource; }
            set
            {
                if (_destinationFileSource != value)
                {
                    _destinationFileSource = value;
                }
            }
        }
        public string FileHeader
        {
            get { return _fileHeader; }
            set
            {
                if (_fileHeader != value)
                {
                    _fileHeader = value;
                }
            }
        }
        public string FileFooter
        {
            get { return _fileFooter; }
            set
            {
                if (_fileFooter != value)
                {
                    _fileFooter = value;
                }
            }
        }
        public int GroupSpace
        {
            get { return _groupSpace; }
            set
            {
                if (_groupSpace != value)
                {
                    _groupSpace = value;
                }
            }
        }
        public string WriteStart
        {
            get { return _writeStart; }
            set
            {
                if (_writeStart != value)
                {
                    _writeStart = value;
                }
            }
        }
        public string WriteStop
        {
            get { return _writeStop; }
            set
            {
                if (_writeStop != value)
                {
                    _writeStop = value;
                }
            }
        }

       
        #region Private
        private FileOperation _fileOperation;
       
        protected string _operationName;
        protected GlobalData.Action _action;
        protected string _destinationFolder;
        protected string _sourcePath;
        protected string _destinationPath;
        protected int _priority;            
        private Filter _filter;
        private string _destinationFilePath; //check if regex contais in Resources or make new file with this name
        private string _destinationFileSource;
        private string _fileHeader;
        private string _fileFooter;
        private int _groupSpace;
        private string _writeStart;
        private string _writeStop;
        private List<string> _textToWrite;     
        private List<DataFilterGroup> _dataFilterGroups;
        private List<string> _filesToPrepare;
        private List<ResultInfo> _resultInfos;
        #endregion Private 

        public DataOperation()
        {
            DataFilterGroups = new List<DataFilterGroup>();
            _textToWrite=new List<string>();
            _filesToPrepare=new List<string>();
            _resultInfos=new List<ResultInfo>();
    }
        public void CopyData()
        {
            try
            {
                #region LoadData
                List<FileLineProperties> filesContent = LoadFilesContent();
                #endregion LoadData

                #region FilterData
                FiltrContentOnGroups(filesContent);
                #endregion FilterData

                #region ValidateData
                DataFilterGroups.ForEach(x => x.LinesToAddToFile = ValidateText.FindVaribleDuplicates(x.LinesToAddToFile));
                #endregion

                #region OrganizeData
                SortGroupsContent();
                #endregion

                #region PrepareData
                List<string> fileContent = PreparedDataToWrite();
                #endregion

                #region WriteData
                string destinationFile = GetCreatedDestinationFile();               
                PrepareToWrite(destinationFile, fileContent);
                WriteToFile(destinationFile);
                #endregion 
            }
            catch (Exception ex)
            {
                throw ex;
            }           
        }
        public void CutData()
        {
            throw new NotImplementedException();
        }
        public void CreateNewFileFromData()
        {
            throw new NotImplementedException();
        }
        private string GetCreatedDestinationFile()
        {
            string destinationFilePath="";
            string destinationFile = "";
            
            try
            {
                DestinationFilePath = FileOperation.CreateDestinationFolderPath();

                if (string.IsNullOrEmpty(DestinationFileSource) == false)
                {
                    string[] files = GetAllFilesFromDirectory(@"...\Resources").ToArray();
                    destinationFile = files.FirstOrDefault(x => System.Text.RegularExpressions.Regex.IsMatch(x, DestinationFileSource));
                }

                if (string.IsNullOrEmpty(destinationFile) == false)
                {
                    string destination = Path.Combine(DestinationFilePath, Path.GetFileName(destinationFile));
                    if (File.Exists(destination) == false)
                    {
                        File.Copy(destinationFile, Path.Combine(DestinationFilePath, Path.GetFileName(destinationFile)));
                    }
                    return destination;
                }
                else
                {
                    if (Path.GetFileName(DestinationFileSource).IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) != -1)
                    {
                        throw new FileFormatException(nameof(DestinationFilePath));
                    }
                    else
                    {
                        destinationFilePath = Path.Combine(DestinationFilePath, Path.GetFileName(DestinationFileSource));

                        if (File.Exists(destinationFilePath) == false)
                        {
                            File.CreateText(destinationFilePath).Close();
                        }

                        return destinationFilePath;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }                        
        }
        private List<string>GetAllFilesFromDirectory(string path)
        {
            List<string> files = new List<string>();

            try
            {
                string[] paths = Directory.GetFileSystemEntries(path);

                foreach (var p in paths)
                {
                    if (string.IsNullOrEmpty(Path.GetExtension(p)))
                    {
                        var temp = GetAllFilesFromDirectory(p);

                        if (temp.Count > 0)
                        {
                            files.AddRange(temp);
                        }
                    }
                    else
                    {
                        files.Add(p);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }            

            return files;
        }
        private List<string> PreparedDataToWrite()
        {
            List<string> buffor = new List<string>();
            List<ResultInfo> resultInfos = new List<ResultInfo>();

            try
            {
                if (string.IsNullOrEmpty(FileHeader) == false)
                {
                    resultInfos.Add(ResultInfo.CreateResultInfo(FileHeader));
                }

                foreach (var filter in DataFilterGroups)
                {
                    if (filter.LinesToAddToFile.Count > 0)
                    {
                        filter.PrepareGroupToWrite(ref resultInfos);

                        for (int i = 0; i < GroupSpace; i++)
                        {
                            resultInfos.Add(ResultInfo.CreateResultInfo(String.Format("")));
                        }
                    }
                }

                if (string.IsNullOrEmpty(FileFooter) == false)
                {
                    resultInfos.Add(ResultInfo.CreateResultInfo(FileFooter));
                }

                _resultInfos = resultInfos;
                resultInfos.ForEach(x => buffor.Add(x.Content));
            }
            catch (Exception ex)
            {
                throw ex;
            }          

            return buffor;           
        }
        private List<FileLineProperties> LoadFilesContent()
        {
            List<FileLineProperties> filesContent = new List<FileLineProperties>();
            FileLineProperties fileLineProperties;
            string []fileContent;
            int lineNumber;

            try
            {
                foreach (string path in _filesToPrepare)
                {
                    lineNumber = 1;
                    fileContent = File.ReadAllLines(path);

                    foreach (string line in fileContent)
                    {
                        fileLineProperties = new FileLineProperties();
                        fileLineProperties.FileLinePath = path;
                        fileLineProperties.LineContent = line;
                        fileLineProperties.LineNumber = lineNumber;
                        lineNumber++;
                        filesContent.Add(fileLineProperties);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }           

            return filesContent;
        }
        private void FiltrContentOnGroups(List<FileLineProperties> filesContent, bool deleteDuplicates = true)
        {
            try
            {
                DataFilterGroups?.ForEach(x => x.SetLinesToAddToFile(filesContent, deleteDuplicates));
            }
            catch (Exception ex)
            {
                throw ex;
            }               
        }
        private void PrepareToWrite(string filePath, List<string>fileNewContet)
        {
            List<string> destinationFile = File.ReadAllLines(filePath).ToList();
            bool writed = false;
            List<string> buffer = new List<string>();

            ValidateText.ValidateReapitingTextWhitExistContent(destinationFile, ref fileNewContet);

            try
            {
                if (destinationFile?.Count > 0)
                {
                    if (string.IsNullOrEmpty(WriteStart) == false)
                    {
                        foreach (string line in destinationFile)
                        {
                            if (line.Contains(WriteStart))
                            {
                                buffer.Add(line);
                                buffer.AddRange(fileNewContet);
                                writed = true;
                            }
                            else
                            {
                                buffer.Add(line);
                            }
                        }

                        if (writed != true)
                        {
                            buffer.AddRange(fileNewContet);
                        }

                        _textToWrite = buffer;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(WriteStop) == false)
                        {
                            foreach (string line in destinationFile)
                            {
                                if (line.Contains(WriteStop))
                                {
                                    buffer.AddRange(fileNewContet);
                                    buffer.Add(line);
                                    writed = true;
                                }
                                else
                                {
                                    buffer.Add(line);
                                }
                            }

                            if (writed != true)
                            {
                                buffer.AddRange(fileNewContet);
                            }
                            _textToWrite = buffer;
                        }
                    }
                }
                else
                {
                    buffer.AddRange(fileNewContet);
                    _textToWrite = buffer;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }           
        }
        private void WriteToFile(string filePath)
        {
            try
            {
                if (_textToWrite?.Count > 0)
                {
                    using (StreamWriter fileStream = new StreamWriter(filePath))
                    {
                        _textToWrite.ForEach(x => fileStream.WriteLine(x));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex; 
            }            
        }
        public bool CutData(string operation)
        {
            throw new NotImplementedException();
        }
        public bool CreateNewFileFromData(string operation)
        {
            throw new NotImplementedException();
        }
        void SortGroupsContent()
        {
            DataContentSortTool sortTool = new DataContentSortTool();
            try
            {
                if (OperationName.ToLower().Contains("olp"))
                {                    
                    DataFilterGroups = sortTool.SortOlpDataFiles(DataFilterGroups);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }           
        }
        public void ExecuteOperation()
        {
            FileOperation.ExecuteOperation();
            _filesToPrepare = FileOperation.GetOperatedFiles();

            if (_filesToPrepare?.Count() == 0 || _filesToPrepare == null)
            {
                return;
            }

            try
            {
                switch (ActionType)
                {
                    case GlobalData.Action.CopyData:
                        {
                            CopyData();
                        }
                        break;
                    case GlobalData.Action.MoveData:
                        {
                            CutData();
                        }
                        break;
                    case GlobalData.Action.RemoveData:
                        {
                            //???
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
            return _resultInfos;
        }
        public void ClearMemory()
        {
            DataFilterGroups.ForEach(x => x.ClearResult());
            List<string> _textToWrite=new List<string>();
            List<DataFilterGroup> _dataFilterGroups=new List<DataFilterGroup>();
            List<string> _filesToPrepare=new List<string>();
            List<ResultInfo> _resultInfos=new List<ResultInfo>();
        }
    }
}
