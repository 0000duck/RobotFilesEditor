using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor
{
    public class FilesDataFilter
    {
        public FilesFilter Filter
        {
            get { return _filter; }
            set
            {
                if(_filter!=value)
                {
                    _filter = value;
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
     
        private FilesFilter _filter;
        private string _fileHeader;
        private string _fileFooter;
        private int _groupSpace;
        private string _writeStart;
        private string _writeStop;
        private List<DataFilterGroup> _dataFilterGroups;        

        FilesDataFilter()
        {
            DataFilterGroups = new List<DataFilterGroup>();
        }   
        
        public string PrepareGroupsToWriteToFile()
        {
            string Buffor = "";
            
                       
            using (StreamWriter sw = new StreamWriter(""))
            {           
                if (string.IsNullOrEmpty(FileHeader) == false){
                    sw.WriteLine(FileHeader);
                }

                foreach(var group in DataFilterGroups)
                {                    
                    var GroupsContent = group.PrepareGroupToWrite();

                    if(string.IsNullOrEmpty(GroupsContent))
                    {
                        sw.WriteLine(GroupsContent);   

                         for (int i = 0; i < GroupSpace; i++){
                             sw.WriteLine();
                         }        
                    }
                }

                if (string.IsNullOrEmpty(FileFooter) == false){
                    sw.WriteLine(FileFooter);
                }                 

                Buffor = sw.ToString();
            }          
            return Buffor;
        }    
    }
}
