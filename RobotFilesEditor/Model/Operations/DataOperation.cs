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

                if (_sourcePath != value)
                {                         
                    FileOperation.SourcePath = value;
                    _sourcePath = FileOperation.SourcePath;                 
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

                if (FileOperation.DestinationPath != value)
                {                   
                    FileOperation.DestinationPath = value;
                    _destinationPath = Path.Combine(FileOperation.DestinationPath, FileOperation.DestinationFolder);
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
        #endregion Private 

        public DataOperation()
        {
            DataFilterGroups = new List<DataFilterGroup>();
            _textToWrite=new List<string>();
            _filesToPrepare=new List<string>();
            _resultInfos=new List<ResultInfo>();
        }

        #region DataPreparing
        public void PrepareDataToCopy()
        {
            List<FileLineProperties> filesContent = FilesTool.LoadTextFromFiles(_filesToPrepare);
            FiltrContentOnGroups(filesContent);

            if (DetectDuplicates)
            {
                DataFilterGroups.ForEach(x => x.LinesToAddToFile = ValidateText.FindVaribleDuplicates(x.LinesToAddToFile));
            }

            DataFilterGroups = DataContentSortTool.SortData(DataFilterGroups, SortType);              
        }
        #endregion DataPreparing
        

        #region DataPreview
        public void PreviewCopyData()
        {
            PrepareDataToCopy();
            string sourcePath = FilesTool.GetSourceFilePath(DestinationFileSource, DestinationPath);
            List<string> sourceText = FilesTool.GetSourceFileText(sourcePath);
            CreateResultToShow(sourceText, true);
        }
        #endregion DataPreview

        #region DataExecute
        public void ExecuteCopyData()
        {
            List<string> newFileText = new List<string>();
            try
            {
                PrepareDataToCopy();
                string sourcePath = FilesTool.GetSourceFilePath(DestinationFileSource, DestinationPath);
                List<string> sourceText = FilesTool.GetSourceFileText(sourcePath);
                newFileText = CreateResultToWrite(sourceText);

                string destinationPath = FilesTool.CombineFilePath(DestinationFileSource, DestinationPath);
                FilesTool.CreateDestinationFile(DestinationFileSource, destinationPath);
                FilesTool.WriteTextToFile(newFileText, destinationPath);
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
        #endregion DataExecute
        

        private void CreateResultToShow(List<string> sourceText, bool addHeader=false)
        {
            List<ResultInfo> resultInfos = new List<ResultInfo>();            

            resultInfos=PrepareFilterGroups(resultInfos);

            resultInfos= WriteNewTextToOldFileContent(sourceText, resultInfos, addHeader);         

            _resultInfos = resultInfos;
        }        
        private List<string> CreateResultToWrite(List<string> sourceText)
        {
            List<string> newFileText = new List<string>();
            CreateResultToShow(sourceText, false);
            _resultInfos.ForEach(x => newFileText.Add(x.Content));
            return newFileText;
        }

        #region PrepareData
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
        private List<ResultInfo> PrepareFilterGroups(List<ResultInfo> resultInfos)
        {
            if(resultInfos==null)
            {
                resultInfos = new List<ResultInfo>();
            }
            
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

                return resultInfos;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private List<ResultInfo> WriteNewTextToOldFileContent(List<string> sourceText, List<ResultInfo> newText, bool addHeader)
        {
            List<ResultInfo> resultInfos = new List<ResultInfo>();
            string resultHeader = Path.Combine(DestinationFolder, Path.GetFileName(DestinationFileSource));

            try
            {
                if ((sourceText?.Count > 0 && addHeader==false) || (newText?.Count>0 && addHeader))
                {
                    newText = WriteNewTextExistingToFile(sourceText, newText);
                }

                if(addHeader)
                {
                    resultInfos = new List<ResultInfo>();

                    if(newText?.Count > 0)
                    {
                        resultInfos.Add(ResultInfo.CreateResultInfo(resultHeader));
                        resultInfos.Last().Bold = true;
                        resultInfos.AddRange(newText);
                        resultInfos.Add(ResultInfo.CreateResultInfo(Path.GetFileName(string.Empty)));
                    }
                    else
                    {
                        resultInfos.Add(ResultInfo.CreateResultInfo(resultHeader));
                        resultInfos.Last().Bold = true;
                        resultInfos.Add(ResultInfo.CreateResultInfo(Path.GetFileName("No result to show")));
                    }                   
                    return resultInfos;
                }

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

                    if (writed != true)
                    {
                        newFileText.AddRange(newText);
                    }
                    return newFileText;
                }
            }

            return newText;
        }
        #endregion PrepareData     

        #region InterfaceImplementation
        public void PreviewOperation()
        {
            FileOperation.PreviewOperation();
            _filesToPrepare = FileOperation.GetOperatedFiles();

            //if (_filesToPrepare?.Count() == 0 || _filesToPrepare == null)
            //{
            //    return;
            //}

            try
            {
                switch (ActionType)
                {
                    case GlobalData.Action.CopyData:
                        {
                            PreviewCopyData();
                        }
                        break;
                    case GlobalData.Action.MoveData:
                        {
                            //CutData();
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
            FileOperation.ExecuteOperation();
            _filesToPrepare = FileOperation.GetOperatedFiles();

            //if (_filesToPrepare?.Count() == 0 || _filesToPrepare == null)
            //{
            //    return;
            //}

            try
            {
                switch (ActionType)
                {
                    case GlobalData.Action.CopyData:
                        {
                            ExecuteCopyData();
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
        #endregion InterfaceImplementation
    }
}
