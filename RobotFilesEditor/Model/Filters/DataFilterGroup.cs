using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RobotFilesEditor
{
    public class DataFilterGroup
    {
        
        public string Header
        {
            get { return _header; }
            set
            {
                if (_header != value)
                {
                    _header = value;
                    _headerType = GlobalData.ChekIfHeaderIsCreatingByMethod(value);
                    if (_headerType != GlobalData.HeaderType.None)
                    {
                        _header = string.Empty;
                    }
                }
            }
        }
        public string Footer
        {
            get { return _footer; }
            set
            {
                if (_footer != value)
                {
                    _footer = value;
                }
            }
        }
        public string TextBefore
        {
            get { return _textBefore; }
            set
            {
                if (_textBefore != value)
                {
                    _textBefore = value;
                }
            }
        }
        public int SpaceBefor
        {
            get { return _spaceBefor; }
            set
            {
                if (_spaceBefor != value)
                {
                    _spaceBefor = value;
                }
            }
        }
        public int SpaceAfter
        {
            get { return _spaceAfter; }
            set
            {
                if (_spaceAfter != value)
                {
                    _spaceAfter = value;
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
        public bool OnlyRegex
        {
            get { return _onlyRegex; }
            set
            {

                if (_onlyRegex != value)
                {
                    _onlyRegex = value;
                }
            }
        }
        public List<FileLineProperties> LinesToAddToFile
        {
            get { return _linesToAddToFile; }
            set
            {
                if (_linesToAddToFile != value)
                {
                    _linesToAddToFile = value;
                }
            }
        }

        private string _header;
        private string _footer;
        private string _textBefore;
        private int _spaceBefor;
        private int _spaceAfter;
        private Filter _filter;
        private bool _onlyRegex;
        private GlobalData.HeaderType _headerType;
        private List<FileLineProperties> _linesToAddToFile;

        public DataFilterGroup()
        {
            Filter = new Filter();
            LinesToAddToFile = new List<FileLineProperties>();
        }

        public List<FileLineProperties> CkeckAllFilters(List<FileLineProperties> listToCheck, bool onlyRegex)
        {
            if(listToCheck.Any())
            {
                listToCheck = Filter.CheckAllFilters(listToCheck, onlyRegex);
            }
            return listToCheck;
        }

        public void SetLinesToAddToFile(ref List<string> usedFiles, List<FileLineProperties> filesContent, bool deleteDuplicates=true)
        {
            //dodać zmienną i do konfiguracji
            LinesToAddToFile=CkeckAllFilters(filesContent, OnlyRegex);

            if(usedFiles!=null)
            {
                foreach(var line in LinesToAddToFile)
                {
                    usedFiles.Add(line.FileLinePath);
                }
                usedFiles?.Distinct();
            }          

            LinesToAddToFile = LinesToAddToFile.DistinctBy(x => x.LineContent).ToList();
        }

        public void DistinctLinesToAddToFile()
        {
            LinesToAddToFile = LinesToAddToFile.DistinctBy(x => x.LineContent).ToList();
        }

        public void PrepareGroupToWrite(ref List<ResultInfo> resultInfos, GlobalData.SortType sortType)
        {
            if (resultInfos == null)
            {
                resultInfos = new List<ResultInfo>();
            }

            if (LinesToAddToFile.Count > 0)
            {              
                if(_headerType!=GlobalData.HeaderType.None)
                {                    
                   LinesToAddToFile = HeaderCreator.CreateGroupHeader(_headerType, LinesToAddToFile, sortType);                   
                }                  

                for (int i = 0; i < SpaceBefor; i++)
                {
                    resultInfos.Add(ResultInfo.CreateResultInfo(""));
                }

                if (string.IsNullOrEmpty(Header) == false)
                {
                    resultInfos.Add(ResultInfo.CreateResultInfo(Header));
                }

                foreach(var line in LinesToAddToFile)
                {
                    if (string.IsNullOrEmpty(TextBefore) == false)
                    {
                        var tmp = line;
                        tmp.LineContent = $"{TextBefore} {line.LineContent}";
                        resultInfos.Add(ResultInfo.CreateResultInfo(tmp));
                    }
                    else
                    {
                        resultInfos.Add(ResultInfo.CreateResultInfo(line));
                    }                   
                }
               
                if (string.IsNullOrEmpty(Footer) == false)
                {
                    resultInfos.Add(ResultInfo.CreateResultInfo(Footer));
                }

                for (int i = 0; i < SpaceAfter; i++)
                {
                    resultInfos.Add(ResultInfo.CreateResultInfo(String.Format("")));
                }              
            }                 
        }

        public void ClearResult()
        {
            LinesToAddToFile = new List<FileLineProperties>();           
        }
    }
}
