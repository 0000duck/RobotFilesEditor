using RobotFilesEditor.Model.DataInformations;
using RobotFilesEditor.Model.Operations;
using RobotFilesEditor.Serializer;
using RobotFilesEditor.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;

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
                    if (string.IsNullOrEmpty(NestedSourcePath) == false && 
                        value.Contains(NestedSourcePath) == false && 
                        string.IsNullOrEmpty(_destinationPath)==false)
                    {
                        _sourcePath = Path.Combine(_destinationPath, NestedSourcePath);
                    }else
                    {
                        _sourcePath = value;
                    }                  
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
                    throw new ArgumentNullException(nameof(SourcePath));
                }

                if (_destinationPath != value)
                {                    
                    if (Directory.Exists(value))
                    {
                        if (string.IsNullOrEmpty(NestedSourcePath) == false && value.Contains(NestedSourcePath) == false)
                        {                            
                            SourcePath = Path.Combine(value, NestedSourcePath);
                        }
                            _destinationPath = value;                       
                    }
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
                    _fileExtensions.ForEach(x =>x=ValidateText.CheckExtensionCorrectness(x));                                              
                }
            }
        }     
        public string NestedSourcePath
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

        public List<string> AllFiles
        {
            get { return _allFiles; }
            set
            {
                if (_allFiles != value)
                {
                    _allFiles = value;
                }
            }
        }

        public List<string> DatFiles
        {
            get { return _datFiles; }
            set
            {
                if (_datFiles != value)
                {
                    _datFiles = value;
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
        private string _nestedSourcePath;
        private Dictionary<string,string> _filteredFiles;
        private List<string> _allFiles;
        private List<string> _datFiles;
        private int? _currentOpNumber;
        //private IDictionary<string, string> sortedFiles;
        #endregion Private

        public  FileOperation()
        {
            FileExtensions = new List<string>();
            FilteredFiles = new Dictionary<string, string>();
        }

        private void GetFilesToExecuteOperation(bool insideFolders=false)
        {
            List<string>allFilesAtSourcePath=new List<string>();

            if (insideFolders)
            {
                allFilesAtSourcePath = FilesMenager.GetAllFilesFromDirectoryAndIncludedNested(SourcePath);
            } else
            {
                try
                {
                    allFilesAtSourcePath = Directory.GetFiles(SourcePath, "*.*", SearchOption.AllDirectories).ToList();
                }
                catch
                {                }
            }
            List<string> tempList = new List<string>(allFilesAtSourcePath);
            foreach (string path in tempList.Where(x => x.Contains(SourcePath + "\\Mirrored")))
            {
                allFilesAtSourcePath.Remove(path);
            }

            List<string> filteredFiles = new List<string>();
            FilteredFiles = new Dictionary<string, string>();

            try
            {
                if (FileExtensions.Count > 0)
                {
                    filteredFiles = allFilesAtSourcePath.Where(x => FileExtensions.Contains(Path.GetExtension(x).ToLower())).ToList();
                }
                else
                {
                    filteredFiles = allFilesAtSourcePath.ToList();
                }
                if (filteredFiles.Count > 0)
                {
                    _allFiles = filteredFiles;
                    filteredFiles = Filter.CheckAllFilesFilters(filteredFiles);
                    filteredFiles.ForEach(x => FilteredFiles.Add(x, ""));
                }
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                throw ex;
            }
            FilteredFiles = SrcValidator.RemoveNotUsedFilesFromFilteredFiles(FilteredFiles);
        }

        #region Prepare
        public void PrepareToCopyFiles()
        {
            if (ConfigurationManager.AppSettings["Ersteller"] == "Default")
            {
                //FilesSerialization.ApplictionConfigCreator(true);
                SrcValidator.ChangeNameIfDefault();
            }
            if (this.OperationName == "Move program files")
                SrcValidator.CheckDestinationEmpty(GlobalData.DestinationPath);
            GetFilesToExecuteOperation();
            
            // TEMP
            DatFiles = new List<string>();
            IDictionary<string, string> srcFiles = ReadFilesToSpacesToFiles();
            if (SrcValidator.DataToProcess == null | OperationName == "Move program files")
                SrcValidator.DataToProcess = new FileValidationData.ValidationData(new Dictionary<string,string>(),"",new List<string>());

            if (srcFiles.Count > 0)
            {
                SrcValidator.FilterDataFromBackup(true,srcFiles);
                SrcValidator.FilterDataFromBackup(false, null, DatFiles);
            }
            //sortedFiles = SrcValidator.ValidateFile(srcFiles, OperationName, DatFiles);
            // TEMP

            string destination = Path.Combine(DestinationPath, DestinationFolder);
            IDictionary<string, string> filteredFilesIterator = new Dictionary<string, string>(FilteredFiles);
            GlobalData.AllFiles = this.AllFiles;
            if (GlobalData.CurrentOpNum == null)
                GlobalData.CurrentOpNum = 0;
            GlobalData.CurrentOpNum++;
            //if (GlobalData.ControllerType == "KRC4 Not BMW")
            //    SrcValidator.ValidateFile();
            if (GlobalData.CurrentOpNum == GlobalData.AllOperations)
            {
                GlobalData.CurrentOpNum = 0;

                if (!SrcValidator.ValidateFile(filteredFilesIterator))
                    return;
                SrcValidator.UnclassifiedPaths = new List<string>();
                SrcValidator.AlreadyContain = new List<string>();
            }

            foreach (var file in filteredFilesIterator)
            {
                try
                {
                    string filePath = Path.Combine(destination, Path.GetFileName(file.Key));

                    if (File.Exists(filePath))
                    {
                        throw new IOException($"File \"{Path.GetFileName(file.Key)}\" already exist!");
                    }
                }
                catch (Exception ex)
                {
                    SrcValidator.GetExceptionLine(ex);
                    FilteredFiles.Remove(file.Key);
                    FilteredFiles.Add(file.Key, ex.Message);                   
                }
            }
        }
        public void PrepareToMoveFiles()
        {
            string destination = Path.Combine(DestinationPath, DestinationFolder);
            IDictionary<string, string> filteredFilesIterator; 

            try
            {
                GetFilesToExecuteOperation();
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                throw ex;
            }

            filteredFilesIterator = new Dictionary<string, string>(FilteredFiles);

            foreach (var file in filteredFilesIterator)
            {
                try
                {
                    string filePath = Path.Combine(destination, Path.GetFileName(file.Key));

                    if (File.Exists(filePath))
                    {
                        throw new IOException($"File \"{Path.GetFileName(file.Key)}\" already exist!");
                    }
                }
                catch (Exception ex)
                {
                    SrcValidator.GetExceptionLine(ex);
                    FilteredFiles.Remove(file.Key);
                    FilteredFiles.Add(file.Key, ex.Message);                    
                }
            }            
        }
        public void PrepareToRemoveFile()
        {            
            IDictionary<string, string> filteredFilesIterator = new Dictionary<string, string>(FilteredFiles);

            try
            {
                GetFilesToExecuteOperation();
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                throw ex;
            }
        }
        #endregion Prepare

        #region ExecuteOperations
        public void CopyFiles()
        {
            try
            {
                GetFilesToExecuteOperation();
                IDictionary<string, string> filesToCopy = new Dictionary<string, string>();
                if (GlobalData.ControllerType != "FANUC")
                {
                    IDictionary<string, List<string>> datFilesToCopy = SrcValidator.PrepareDatFiles(FilteredFiles);
                    //FilteredFiles = SrcValidator.RemoveNotUsedFilesFromFilteredFiles(FilteredFiles);
                    filesToCopy = SrcValidator.ReplaceDataContent(FilteredFiles, datFilesToCopy);
                }//FilteredFiles = ReplaceDataContent(FilteredFiles, datFilesToCopy);
                else
                {
                    filesToCopy = SrcValidator.Result;
                }
                //foreach (var datFile in datFilesToCopy)
                //{
                //    filesToCopy.Remove(datFile.Key);
                //    string currentString = "";
                //    foreach (string line in datFile.Value)
                //        currentString += line + "\r\n";                     
                //    filesToCopy.Add(datFile.Key,currentString);
                //}
                if (this.OperationName == "Copy init_out")
                {
                    string destination = FilesMenager.CreateDestinationFolderPath(DestinationPath, DestinationFolder);
                    if (GlobalData.isHandlingRobot = true && GlobalData.isWeldingRobot == true)
                        File.Copy("Resources\\GlobalFiles\\KRC2\\" + GlobalData.ControllerType + "\\init_out_gun_gripper.src", destination + "\\init_out.src");
                    else if (GlobalData.isHandlingRobot = true && GlobalData.isWeldingRobot == false)
                        File.Copy("Resources\\GlobalFiles\\KRC2\\" + GlobalData.ControllerType + "\\init_out_gripper.src", destination + "\\init_out.src");
                    else if (GlobalData.isHandlingRobot = false && GlobalData.isWeldingRobot == true)
                        File.Copy("Resources\\GlobalFiles\\KRC2\\" + GlobalData.ControllerType + "\\init_out_gun.src", destination + "\\init_out.src");
                    else
                        MessageBox.Show("");

                }
                if (this.OperationName == "Copy bTypBit")
                {
                    string destination = FilesMenager.CreateDestinationFolderPath(DestinationPath, DestinationFolder);
                    File.Copy("Resources\\GlobalFiles\\KRC2\\bTypBit.src", destination + "\\bTypBit.src");

                }
                if (this.OperationName == "Copy InitProduction")
                {
                    string destination = FilesMenager.CreateDestinationFolderPath(DestinationPath, DestinationFolder);
                    File.Copy("Resources\\GlobalFiles\\KRC4\\InitProduction.src", destination + "\\InitProduction.src");

                }
                if (this.OperationName == "Create SymName.txt")
                {
                    CreateGripperMethods.CreateSymName(FilesMenager.CreateDestinationFolderPath(DestinationPath, DestinationFolder));
                }
                if (filesToCopy?.Count > 0)
                {
                    string destination = FilesMenager.CreateDestinationFolderPath(DestinationPath, DestinationFolder);

                    filesToCopy = FilesMenager.CopyFiles(filesToCopy, destination);

                    foreach (var file in filesToCopy)
                    {
                        try
                        {
                            string filePath = Path.Combine(destination, Path.GetFileName(file.Key));

                            if (File.Exists(filePath))
                            {
                                throw new IOException($"File \"{Path.GetFileName(file.Key)}\" already exist!");
                            }
                        }
                        catch (Exception ex)
                        {
                            FilteredFiles.Remove(file.Key);
                            FilteredFiles.Add(file.Key, ex.Message);
                        }
                    }

                    if (FilesMenager.CheckFilesCorrectness(destination, filesToCopy.Keys.ToList()) == false)
                    {
                        throw new Exception("Copy files veryfication not pass!");
                    }
                }
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                throw ex;
            }             
        }

        public void MoveFile()
        {
            try
            {
                GetFilesToExecuteOperation();
                if(FilteredFiles.Any())
                {
                    string destination = FilesMenager.CreateDestinationFolderPath(DestinationPath, DestinationFolder);
                    IDictionary<string,string> filesToCopy = FilesMenager.MoveFiles(FilteredFiles, destination);
                    //FilteredFiles = FilesMenager.MoveFiles(FilteredFiles, destination);

                    if (FilesMenager.CheckFilesCorrectness(destination, filesToCopy.Keys.ToList()) == false)
                    {
                        throw new Exception("Copy files veryfication not pass!");
                    }
                }             
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                throw ex;
            }           
        }
        public void RemoveFile()
        {            
            IDictionary<string, string> filteredFilesIterator = new Dictionary<string, string>(FilteredFiles);

            try
            {
                GetFilesToExecuteOperation();

                FilteredFiles = FilesMenager.RemoveFile(FilteredFiles);

                if (FilesMenager.CheckFilesCorrectness(SourcePath, FilteredFiles.Keys.ToList()))
                {
                    throw new Exception("Copy files veryfication not pass!");
                }
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                throw ex;
            }           
        }
        #endregion ExecuteOperations

        #region FilesPreparing      
        public List<string>GetOperatedFiles()
        {           
            return FilteredFiles.Keys.ToList();
        }
        #endregion FilesPreparing

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
                            CopyFiles();
                            
                        }
                        break;
                    case GlobalData.Action.Move:
                        {
                            MoveFile();
                        }
                        break;
                    case GlobalData.Action.Remove:
                        {
                            //RemoveFile();
                        }
                        break;
                    case GlobalData.Action.CutData:
                        {
                            //SourcePath = DestinationPath;
                            //GetFilesToExecuteOperation(true);
                        }
                        break;
                    default:
                        {
                            GetFilesToExecuteOperation();
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
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
                SrcValidator.GetExceptionLine(ex);
                throw ex;
            }

            return resultInfos;
        }
        public void ClearMemory()
        {
            FilteredFiles = new Dictionary<string, string>();
        }
        public void PreviewOperation()
        {
            try
            {
                switch (ActionType)
                {
                    case GlobalData.Action.Copy:
                        {
                            PrepareToCopyFiles();
                        }
                        break;
                    case GlobalData.Action.Move:
                        {
                            PrepareToMoveFiles();
                        }
                        break;
                    case GlobalData.Action.Remove:
                        {
                            PrepareToRemoveFile();
                        }
                        break;
                    case GlobalData.Action.CutData:
                        {
                            SourcePath = DestinationPath;
                            GetFilesToExecuteOperation(true);
                        }
                        break;
                    default:
                        {
                            GetFilesToExecuteOperation();
                        }
                        break;
                }
            }
            catch (Exception ex)
            {                
                SrcValidator.GetExceptionLine(ex);
                throw ex;
            }
        }

        private IDictionary<string, string> ReadFilesToSpacesToFiles()
        {
            DatFiles = new List<string>();
            string line = "";
            string controllerTypeFileExtension = ".src";
            if (GlobalData.ControllerType == "FANUC")
                controllerTypeFileExtension = ".ls";
            IDictionary<string, string> filesAndContents = new Dictionary<string, string>();
            foreach (var file in FilteredFiles.Where(x => Path.GetExtension(x.Key).ToLower().Contains(controllerTypeFileExtension.ToLower())))
            {
                string currentString = "";
                var reader = new StreamReader(file.Key);
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    currentString = string.Join(Environment.NewLine, currentString, line);
                }
                filesAndContents.Add(file.Key, currentString);
                if (reader.EndOfStream)
                    reader.Close();
            }
            foreach (var file in FilteredFiles.Where(x => x.Key.Contains(".dat")))
            {
                DatFiles.Add(file.Key);
            }

            return filesAndContents;
        }
        #endregion InterfaceImplementation
    }
}
