using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private List<DataFilterGroup> _dataFilterGroups;
        private List<string> _filesToPrepare;
        #endregion Private 

        public void FollowOperation(List<string>filesToPrepare, string destinationFilePath)
        {
            
        }
        public DataOperation()
        {
            DataFilterGroups = new List<DataFilterGroup>();
        }
        public bool CopyData()
        {
            List<FileLineProperties> filesContent=LoadFilesContent();
            FiltrContentOnGroups(filesContent);
            DataFilterGroups.ForEach(x => ValidateText.ValidateLienes(x.LinesToAddToFile));
            SortGroupsContent();
            DataFilterGroups.ForEach(x => x.PrepareGroupToWrite());            
            string destinationFile=GetCreatedDestinationFile();
            string fileContent = PreparedDataToWrite();
            WriteTextToFile(destinationFile, fileContent);
            return false;
        }
        public bool CutData()
        {
            throw new NotImplementedException();
        }
        public bool CreateNewFileFromData()
        {
            throw new NotImplementedException();
        }
        private string GetCreatedDestinationFile()
        {
            string destinationFilePath="";
            string destinationFile = "";                 

            if(string.IsNullOrEmpty(DestinationFileSource)==false)
            {
                string[] files = GetAllFilesFromDirectory(@"...\Resources").ToArray();
                destinationFile = files.FirstOrDefault(x => System.Text.RegularExpressions.Regex.IsMatch(x, DestinationFileSource));
            }            

            if (string.IsNullOrEmpty(destinationFile)==false)
            {
                string destination= Path.Combine(DestinationFilePath, Path.GetFileName(destinationFile));
                if(File.Exists(destination)==false)
                {
                    File.Copy(destinationFile, Path.Combine(DestinationFilePath, Path.GetFileName(destinationFile)));
                }               
                return destination;
            }else
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
        private List<string>GetAllFilesFromDirectory(string path)
        {
            List<string> files = new List<string>();
            string[] paths = Directory.GetFileSystemEntries(path);

            foreach (var p in paths)
            {               
                if (string.IsNullOrEmpty(Path.GetExtension(p)))
                {
                    var temp = GetAllFilesFromDirectory(p);

                    if (temp.Count>0)
                    {
                        files.AddRange(temp);
                    }
                }
                else
                    {
                        files.Add(p);
                    }                
            }

            return files;
        }
        private string PreparedDataToWrite()
        {
            string Buffor = "";

            if (string.IsNullOrEmpty(FileHeader) == false)
            {
                Buffor += String.Format("{0}\n", FileHeader);
            }

            foreach (var filter in DataFilterGroups)
            {
                if (filter.LinesToAddToFile.Count > 0)
                {  
                    Buffor += String.Format("{0}", filter.PrepareGroupToWrite());                  

                    for (int i = 0; i < GroupSpace; i++)
                    {
                        Buffor += String.Format("\n");
                    }                                       
                }
            }

            if (string.IsNullOrEmpty(FileFooter) == false)
            {
                Buffor += String.Format("{0}\n", FileFooter);
            }
            return Buffor;           
        }
        private List<FileLineProperties> LoadFilesContent()
        {
            List<FileLineProperties> filesContent = new List<FileLineProperties>();
            FileLineProperties fileLineProperties;
            string []fileContent;
            int lineNumber;

            foreach (string path in _filesToPrepare)
            {                
                lineNumber = 1;
                fileContent = File.ReadAllLines(path);
                 
                foreach(string line in fileContent)
                {
                    fileLineProperties = new FileLineProperties();
                    fileLineProperties.FileLinePath = path;
                    fileLineProperties.LineContent = line;
                    fileLineProperties.LineNumber = lineNumber;                    
                    lineNumber++;
                    filesContent.Add(fileLineProperties);
                }                
            }
            return filesContent;
        }
        private void FiltrContentOnGroups(List<FileLineProperties> filesContent)
        {
           DataFilterGroups.ForEach(x => x.SetLinesToAddToFile(filesContent));            
        }
        private bool WriteTextToFile(string filePath, string fileContet)
        {
            List<string> destinationFile = File.ReadAllLines(filePath).ToList();
            bool writed = false;

            if(destinationFile?.Count>0)
            {               
                if (string.IsNullOrEmpty(WriteStart) == false)
                {                    
                        using (StreamWriter fileStream = new StreamWriter(filePath))
                        {
                            foreach (string line in destinationFile)
                            {
                                if (line.Contains(WriteStart))
                                {
                                    fileStream.WriteLine(line);
                                    fileStream.Write(fileContet);
                                    writed = true;
                                }
                                else
                                {
                                    fileStream.WriteLine(line);
                                }
                            }

                            if(writed!=true)
                            {
                                fileStream.Write(fileContet);
                            }
                        }                   
                }
                else
                {
                    if (string.IsNullOrEmpty(WriteStop)==false)
                    {
                        using (StreamWriter fileStream = new StreamWriter(filePath))
                        {
                            foreach (string line in destinationFile)
                            {
                                if (line.Contains(WriteStop))
                                {
                                    fileStream.Write(fileContet);
                                    fileStream.WriteLine(line);
                                    writed = true;
                                }
                                else
                                {
                                    fileStream.WriteLine(line);
                                }
                            }

                            if (writed != true)
                            {
                                fileStream.Write(fileContet);
                            }
                        }
                    }
                }
            }else
            {
                using (StreamWriter fileStream = new StreamWriter(filePath))
                {
                    fileStream.Write(fileContet);
                }
            }          

            return true;
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
            if(OperationName.ToLower().Contains("olp"))
            {
                DataContentSortTool sortTool = new DataContentSortTool();

                DataFilterGroups=sortTool.SortOlpDataFiles(DataFilterGroups);
            }
        }

        public void ExecuteOperation()
        {
            FileOperation.ExecuteOperation();
            _filesToPrepare = FileOperation.GetOperationResult();
            
            if(_filesToPrepare?.Count()==0 || _filesToPrepare==null)
            {
                throw new Exception("No files to prepare");
            }
            
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

        public List<string> GetOperationResult()
        {
            List<string> result = new List<string>();

            if (_filesToPrepare?.Count>0)
            {
                DataFilterGroups.ForEach(x => x.LinesToAddToFile.ForEach(y => result.Add(y.LineContent)));
            }           

            return result;
        }
    }
}
