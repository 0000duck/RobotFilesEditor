using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor
{
    public class DataOperation: Operation, IFileDataOperations
    {
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

        public void FollowOperation(List<string>filesToPrepare)
        {
            if(filesToPrepare?.Count>0)
            {
                _filesToPrepare = filesToPrepare;
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

                    }
                    break;
            }
        }

        public DataOperation()
        {
            DataFilterGroups = new List<DataFilterGroup>();
        }

        public bool CopyData()
        {
            List<string>filesContent=LoadFilesContent();
            FiltrContentOnGroups(filesContent);
            DataFilterGroups.ForEach(x => x.PrepareGroupToWrite());
            string destinationFile=GetDestinationFile();
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

        private string GetDestinationFile()
        {
            string destinationFilePath="";

            string[] files = Directory.GetFileSystemEntries(@"...\Resource\");

            string destinationFile =files.FirstOrDefault(x => System.Text.RegularExpressions.Regex.IsMatch(x, DestinationFileSource));

            if (string.IsNullOrEmpty(destinationFile)==false)
            {
                File.Copy(destinationFile, Path.Combine(DestinationFilePath, Path.GetFileName(destinationFile)));
                return Path.Combine(DestinationFilePath, Path.GetFileName(destinationFile));
            }else
            {
                if (DestinationFileSource.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) != -1)
                {
                    throw new FileFormatException(nameof(DestinationFileSource));
                }
                else
                {
                    destinationFilePath = Path.Combine(DestinationFilePath, Path.GetFileName(DestinationFileSource));
                    File.Create(destinationFile);

                    return destinationFilePath;
                }
            }            
        }

        private string PreparedDataToWrite()
        {
            string Buffor = "";

            foreach (var filter in DataFilterGroups)
            {
                if (filter.LinesToAddToFile.Count > 0)
                {
                    using (StreamWriter sw = File.CreateText(""))
                    {
                        if (string.IsNullOrEmpty(FileHeader) == false)
                        {
                            sw.WriteLine(FileHeader);
                        }

                        filter.PrepareGroupToWrite();

                        if (string.IsNullOrEmpty(FileFooter) == false)
                        {
                            sw.WriteLine(FileFooter);
                        }

                        for (int i = 0; i < GroupSpace; i++)
                        {
                            sw.WriteLine();
                        }

                        Buffor = sw.ToString();
                    }
                }
           }
                return Buffor;           
        }
        private List<string> LoadFilesContent()
        {
            List<string> filesContent = new List<string>();

            foreach (string path in _filesToPrepare)
            {
                filesContent.AddRange(File.ReadAllLines(path).ToList());
            }
            return filesContent;
        }
        private void FiltrContentOnGroups(List<string>filesContent)
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
                    if (string.IsNullOrEmpty(WriteStop))
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
    }
}
