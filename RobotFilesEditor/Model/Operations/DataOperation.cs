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
        public string DestinationFile
        {
            get { return _destinationFile; }
            set
            {
                if (_destinationFile != value)
                {
                    _destinationFile = value;
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
        private string _destinationFile; //check if regex contais in Resources or make new file with this name
        private string _fileHeader;
        private string _fileFooter;
        private int _groupSpace;
        private string _writeStart;
        private string _writeStop;
        private List<DataFilterGroup> _dataFilterGroups;

        public DataOperation()
        {
            DataFilterGroups = new List<DataFilterGroup>();
        }

        public bool CopyData()
        {
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
        private void CreateDestinationFile()
        {

        }
        private void WriteData(FilesDataFilter fileDataFilter)
        {
            foreach (var filter in fileDataFilter.DataFilterGroups)
            {

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
