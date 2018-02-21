using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RobotFilesEditor
{
    public class DataOperation: IOperation
    {
        #region Public
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
            get { return FileOperation.DestinationFolder; }
            set
            {
                if (value == null)
                {
                    FileOperation.DestinationFolder = string.Empty;
                }

                if (FileOperation.DestinationFolder != value)
                {
                    FileOperation.DestinationFolder = value;
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

                if (FileOperation.SourcePath != value)
                {                         
                    FileOperation.SourcePath = value;
                    _sourcePath = FileOperation.SourcePath;                 
                }
            }
        }
        public string DestinationPath
        {
            get { return Path.Combine(FileOperation.DestinationPath, FileOperation.DestinationFolder); }
            set
            {
                if (value == null)
                {
                    _destinationPath = string.Empty;
                }

                if (FileOperation.DestinationPath != value)
                {                   
                    FileOperation.DestinationPath = value;
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
        public bool DetectDuplicates
        {
            get { return _detectDuplicates; }
            set
            {
                if (_detectDuplicates != value)
                {
                    _detectDuplicates = value;
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
                    _destinationFileSource = Path.Combine(Directory.GetCurrentDirectory(), value);
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
                    _headerType = GlobalData.ChekIfHeaderIsCreatingByMethod(value);
                    if (_headerType != GlobalData.HeaderType.None)
                    {
                        _fileHeader = string.Empty;
                    }
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
        public GlobalData.SortType SortType
        {
            get { return _sortType; }
            set
            {
                if (_sortType != value)
                {
                    _sortType = value;
                }
            }
        }
        #endregion Public

        #region Private
        private FileOperation _fileOperation;
       
        protected string _operationName;
        protected GlobalData.Action _action;
        protected string _destinationFolder;
        protected string _sourcePath;
        protected string _destinationPath;
        protected int _priority;            
        private Filter _filter;
        private string _destinationFileSource;
        private string _fileHeader;
        private string _fileFooter;
        private int _groupSpace;
        private string _writeStart;
        private string _writeStop;
        private bool _detectDuplicates;
        public GlobalData.SortType _sortType;
        private List<string> _textToWrite;     
        private List<DataFilterGroup> _dataFilterGroups;
        private List<string> _filesToPrepare;
        private List<ResultInfo> _resultInfos;
        private List<string> _resultToWrite;
        private List<string> _usedFiles;
        private GlobalData.HeaderType _headerType;
        #endregion Private 

        public DataOperation()
        {
            DataFilterGroups = new List<DataFilterGroup>();
            _textToWrite=new List<string>();
            _filesToPrepare=new List<string>();
            _resultInfos=new List<ResultInfo>();
            _resultToWrite = new List<string>();
            _usedFiles = new List<string>();
        }

        #region DataPreparing
        public void PrepareDataToCopy()
        {
            try
            {
                List<FileLineProperties> filesContent = FilesTool.LoadTextFromFiles(_filesToPrepare);
                FiltrContentOnGroups(filesContent);

                if (DetectDuplicates)
                {
                    DataFilterGroups.ForEach(x => x.LinesToAddToFile = ValidateText.FindVaribleDuplicates(x.LinesToAddToFile));
                }
                DataFilterGroups = DataContentSortTool.SortData(DataFilterGroups, SortType);
            }
            catch (Exception ex)
            {
                throw ex;
            }                      
        }
        #endregion DataPreparing        

        #region DataPreview
        public void PreviewCopyData()
        {
            try
            {
                PrepareDataToCopy();
                string sourcePath = FilesTool.GetSourceFilePath(DestinationFileSource, DestinationPath);
                List<string> sourceText = FilesTool.GetSourceFileText(sourcePath);
                CreateResultToShow(sourceText, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }         
        }
        public void PreviewCutData()
        {
            List<ResultInfo> cutInfo = new List<ResultInfo>();
            List<string> fragmentsToRemove = new List<string>();

            try
            {
                PreviewCopyData();

                DataFilterGroups.ForEach(x => x.LinesToAddToFile.ForEach(y => fragmentsToRemove.Add(y.LineContent)));
                _usedFiles.ForEach(x => cutInfo.Add(ResultInfo.CreateResultInfoHeder(Path.GetFileName(x), x)));

                CreateCutDataResultResultToShow(cutInfo);
            }
            catch (Exception ex)
            {
                throw ex;
            }          
        }        
        #endregion DataPreview

        #region DataExecute
        public void ExecuteCopyData()
        {
            try
            {
                PrepareDataToCopy();
                string sourcePath = FilesTool.GetSourceFilePath(DestinationFileSource, DestinationPath);
                List<string> sourceText = FilesTool.GetSourceFileText(sourcePath);
                CreateResultToShow(sourceText, true);              

                if (_resultToWrite?.Count > 0  && _resultInfos.Where(x => string.IsNullOrEmpty(x.Description) == false).ToList().Count == 0)
                {
                    string destinationPath = FilesTool.CombineFilePath(DestinationFileSource, DestinationPath);
                    FilesTool.CreateDestinationFile(sourcePath, DestinationPath);
                    FilesTool.WriteTextToFile(_resultToWrite, destinationPath);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }           
        }

        public void ExecuteCutData()
        {
            List<ResultInfo> cutInfo = new List<ResultInfo>();
            List<string> fragmentsToRemove = new List<string>();

            try
            {
                PrepareDataToCopy();
                DataFilterGroups.ForEach(x => x.LinesToAddToFile.ForEach(y => fragmentsToRemove.Add(y.LineContent)));

                string sourcePath = FilesTool.GetSourceFilePath(DestinationFileSource, DestinationPath);
                List<string> sourceText = FilesTool.GetSourceFileText(sourcePath);
                CreateResultToShow(sourceText, true);

                string destinationPath = FilesTool.CombineFilePath(DestinationFileSource, DestinationPath);               

                _usedFiles.ForEach(x => cutInfo.Add(ResultInfo.CreateResultInfoHeder(Path.GetFileName(x), x)));
                _usedFiles?.Remove(destinationPath);

                CreateCutDataResultResultToShow(cutInfo);

                var hasErrors = DataFilterGroups.Exists(x => x.LinesToAddToFile.Exists(y => y.HasExeption));

                if (hasErrors==false)
                {
                    if (_resultToWrite?.Count > 0 && _resultInfos.Where(x => string.IsNullOrEmpty(x.Description) == false).ToList().Count == 0)
                    {                        
                        FilesTool.CreateDestinationFile(sourcePath, DestinationPath);
                        FilesTool.WriteTextToFile(_resultToWrite, destinationPath);

                        foreach (var fragment in fragmentsToRemove)
                        {
                            foreach (var path in _usedFiles)
                            {
                                FilesTool.DeleteFromFile(path, fragment);
                            }
                        }
                    }                    
                }               
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }    
        #endregion DataExecute        

        private void CreateResultToShow(List<string> sourceText, bool writeToFile= false)
        {
            List<ResultInfo> resultInfos = new List<ResultInfo>();
            _resultInfos = new List<ResultInfo>();
            _resultToWrite = new List<string>();

            resultInfos=PrepareFilterGroups(resultInfos);

            _resultInfos = WriteNewTextToOldFileContent(sourceText, resultInfos, true);

            if (_headerType != GlobalData.HeaderType.None && resultInfos?.Count>0)
            {
                _resultInfos = HeaderCreator.CreateFileHeader(_headerType, FilesTool.CombineFilePath(DestinationFileSource, DestinationPath), _resultInfos, _filesToPrepare);
            }

            if (writeToFile)
            {
                var toWriteResult = WriteNewTextToOldFileContent(sourceText, resultInfos, false);

                if (_headerType != GlobalData.HeaderType.None && toWriteResult != null)
                {
                    toWriteResult=HeaderCreator.CreateFileHeader(_headerType, FilesTool.CombineFilePath(DestinationFileSource, DestinationPath), toWriteResult, _filesToPrepare);
                }

                if (toWriteResult!=null)
                {
                    toWriteResult.ForEach(x => _resultToWrite.Add(x.Content));
                }                                
            }                     
        }      

        #region PrepareData
        private void FiltrContentOnGroups(List<FileLineProperties> filesContent, bool deleteDuplicates = true)
        {
            try
            {
                List<string> usedFiles = new List<string>();
                DataFilterGroups?.ForEach(x => x.SetLinesToAddToFile( ref usedFiles, filesContent, deleteDuplicates));

                if (usedFiles!=null)
                {
                    _usedFiles = usedFiles;
                }               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private List<ResultInfo> PrepareFilterGroups(List<ResultInfo> resultInfos)
        {
            List<ResultInfo> tmpResult = new List<ResultInfo>();
            if (resultInfos==null)
            {
                resultInfos = new List<ResultInfo>();
            }
            
            try
            {
                if (_headerType != GlobalData.HeaderType.None)
                {
                    DataFilterGroups = HeaderCreator.CreateGroupHeader(_headerType, DataFilterGroups, SortType);
                }               

                foreach (var filter in DataFilterGroups)
                {
                    if (filter.LinesToAddToFile.Count > 0)
                    {
                        filter.PrepareGroupToWrite(ref tmpResult);

                        for (int i = 0; i < GroupSpace; i++)
                        {
                            tmpResult.Add(ResultInfo.CreateResultInfo(String.Format("")));
                        }
                    }
                }

                if(tmpResult?.Count()>0)
                {
                    
                    if (string.IsNullOrEmpty(FileHeader) == false)
                    {
                        resultInfos.Add(ResultInfo.CreateResultInfo(FileHeader));
                    }

                    resultInfos.AddRange(tmpResult);

                    if (string.IsNullOrEmpty(FileFooter) == false)
                    {
                        resultInfos.Add(ResultInfo.CreateResultInfo(FileFooter));
                    }                      
                }               

                return resultInfos;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private List<ResultInfo> WriteNewTextToOldFileContent(List<string> sourceText, List<ResultInfo> newText, bool previewOnly)
        {
            List<ResultInfo> resultInfos = new List<ResultInfo>();
            string resultHeader = Path.Combine(DestinationFolder, Path.GetFileName(DestinationFileSource));
            string fileDestination = Path.Combine(DestinationPath, Path.GetFileName(DestinationFileSource));
            bool IsFileAlreadyExist = false;

            try
            {
                #region WriteToFile
                if ((newText?.Count > 0)==false && previewOnly==false)
                {
                    return null;
                }

                if (sourceText?.Count > 0 && newText?.Count>0)
                {
                    newText = WriteNewTextExistingToFile(sourceText, newText);                    
                }
                #endregion WriteToFile

                #region AddHeaderShowRegion
                if (previewOnly)
                {
                    resultInfos = new List<ResultInfo>();

                    if (File.Exists(fileDestination))
                    {
                        IsFileAlreadyExist = true;
                    }

                    if (newText?.Count > 0)
                    {
                        resultInfos.Add(ResultInfo.CreateResultInfoHeder(resultHeader, fileDestination));
                        resultInfos.Last().Bold = true;
                        resultInfos.AddRange(newText);
                        resultInfos.Add(ResultInfo.CreateResultInfo(string.Empty));
                        return resultInfos;
                    }

                    if((newText?.Count > 0)==false && IsFileAlreadyExist==false)
                    {
                        resultInfos.Add(ResultInfo.CreateResultInfo(resultHeader));
                        resultInfos.Last().Bold = true;
                        resultInfos.Add(ResultInfo.CreateResultInfo(Path.GetFileName("No result to show")));
                        return resultInfos;
                    }

                    if((newText?.Count > 0) == false && sourceText?.Count > 0 && IsFileAlreadyExist)
                    {
                        resultInfos.Add(ResultInfo.CreateResultInfoHeder($"No change in file: {resultHeader}", fileDestination));
                        resultInfos.Last().Bold = true;
                        sourceText.ForEach(x => resultInfos.Add(ResultInfo.CreateResultInfo(x)));
                        resultInfos.Add(ResultInfo.CreateResultInfo(string.Empty));

                        return resultInfos;
                    }
                }
                #endregion AddHeaderShowRegion

                return newText;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private List<ResultInfo> WriteNewTextExistingToFile(List<string> sourceText, List<ResultInfo> newText)
        {
            bool writed = false;
            List<ResultInfo> sourceFile = new List<ResultInfo>();
            List<ResultInfo> newFileText = new List<ResultInfo>();

            sourceText.ForEach(x => sourceFile.Add(ResultInfo.CreateResultInfo(x)));
            ValidateText.ValidateReapitingTextWhitExistContent(sourceFile, ref newText);

            if(newText?.Count > 0 && sourceText?.Count>0)
            {
                if (string.IsNullOrEmpty(WriteStart) == false)
                {
                    foreach (ResultInfo line in sourceFile)
                    {
                        if (line.Content.Contains(WriteStart))
                        {
                            newFileText.Add(line);
                            newFileText.AddRange(newText);
                            writed = true;
                        }
                        else
                        {
                            newFileText.Add(line);
                        }
                    }

                    if (writed != true)
                    {
                        newFileText.AddRange(newText);
                    }

                    return newFileText;
                }
                else
                {
                    if (string.IsNullOrEmpty(WriteStop) == false)
                    {
                        foreach (ResultInfo line in sourceFile)
                        {
                            if (line.Content.Contains(WriteStop))
                            {
                                newFileText.AddRange(newText);
                                newFileText.Add(line);
                                writed = true;
                            }
                            else
                            {
                                newFileText.Add(line);
                            }
                        }                      
                    }

                    if (writed != true)
                    {
                        sourceText.ForEach(x => newFileText.Add(ResultInfo.CreateResultInfo(x)));
                        newFileText.AddRange(newText);
                    }
                    return newFileText;
                }
            }          

            return newText;
        }
        private List<ResultInfo>ShowExistingFile(List<string> sourceText)
        {
            List<ResultInfo> sourceFile = new List<ResultInfo>();
            string resultHeader = Path.Combine(DestinationFolder, Path.GetFileName(DestinationFileSource));
            string fileDestination = Path.Combine(DestinationPath, Path.GetFileName(DestinationFileSource));

            sourceFile.Add(ResultInfo.CreateResultInfoHeder($"No change in File{resultHeader}", fileDestination));
            sourceFile.Last().Bold = true;
            sourceText.ForEach(x => sourceFile.Add(ResultInfo.CreateResultInfo(x)));
            sourceFile.Add(ResultInfo.CreateResultInfo(string.Empty));

            return sourceFile;
        }
        #endregion PrepareData     


        #region CutData
        public List<string> GetSequenceToCut()
        {
            try
            {
                List<FileLineProperties> filesContent = FilesTool.LoadTextFromFiles(_filesToPrepare);
                FiltrContentOnGroups(filesContent);

                if (DetectDuplicates)
                {
                    DataFilterGroups.ForEach(x => x.LinesToAddToFile = ValidateText.FindVaribleDuplicates(x.LinesToAddToFile));
                }

                var isErrors = DataFilterGroups.Where(x => x.LinesToAddToFile.Where(y => y.HasExeption).Count() > 0).ToList();

                if (isErrors.Count > 0)
                {
                    throw new Exception("Result contains exeptions");
                }

                List<string> toWrite = new List<string>();
                DataFilterGroups.ForEach(x => x.LinesToAddToFile.ForEach(y => toWrite.Add(y.LineContent)));

                return toWrite;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void CreateCutDataResultResultToShow(List<ResultInfo>copyFrom)
        {
            List<ResultInfo> cutDataResult = new List<ResultInfo>();
            if(copyFrom?.Count>0)
            {
                cutDataResult.Add(ResultInfo.CreateResultInfo(string.Empty));
                cutDataResult.Add(ResultInfo.CreateResultInfo($"Find fragment to cut in files:"));
                cutDataResult.Last().Bold = true;
                cutDataResult.AddRange(copyFrom);
                cutDataResult.Add(ResultInfo.CreateResultInfo(string.Empty));
            }else
            {
                cutDataResult.Add(ResultInfo.CreateResultInfo($"No files"));
                cutDataResult.Last().Bold = true;
            }
            

            _resultInfos.AddRange(cutDataResult);
        }
      
        #endregion CutData


        #region InterfaceImplementation
        public void PreviewOperation()
        {
            ClearMemory();
            FileOperation.PreviewOperation();
            _filesToPrepare = FileOperation.GetOperatedFiles();            

            try
            {
                switch (ActionType)
                {
                    case GlobalData.Action.CopyData:
                        {
                            PreviewCopyData();
                        }
                        break;
                    case GlobalData.Action.CutData:
                        {                             
                            PreviewCutData();
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
        public void ExecuteOperation()
        {
            ClearMemory();
            FileOperation.ExecuteOperation();
            _filesToPrepare = FileOperation.GetOperatedFiles();            

            try
            {
                switch (ActionType)
                {
                    case GlobalData.Action.CopyData:
                        {
                            ExecuteCopyData();
                        }
                        break;
                    case GlobalData.Action.CutData:
                        {
                            ExecuteCutData();
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
        #endregion InterfaceImplementation
    }
}
