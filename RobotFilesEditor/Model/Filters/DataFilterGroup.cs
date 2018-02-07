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
        private int _spaceBefor;
        private int _spaceAfter;
        private Filter _filter;
        private bool _onlyRegex;
        private List<FileLineProperties> _linesToAddToFile;

        public DataFilterGroup()
        {
            Filter = new Filter();
            LinesToAddToFile = new List<FileLineProperties>();
        }

        public List<FileLineProperties> CkeckAllFilters(List<FileLineProperties> listToCheck, bool onlyRegex)
        {
            listToCheck = Filter.FilterContains(listToCheck);
            listToCheck = Filter.FilterNotContains(listToCheck);
            listToCheck = Filter.FilterRegexContain(listToCheck, onlyRegex);
            listToCheck = Filter.FilterRegexNotContain(listToCheck);

            return listToCheck;
        }

        public void SetLinesToAddToFile(List<FileLineProperties> filesContent)
        {
           LinesToAddToFile=CkeckAllFilters(filesContent, OnlyRegex);
           LinesToAddToFile = LinesToAddToFile.DistinctBy(x => x.LineContent).ToList();
        }

        public void PrepareGroupToWrite(ref List<ResultInfo> resultInfos)
        {
            if (resultInfos == null)
            {
                resultInfos = new List<ResultInfo>();
            }

            if (LinesToAddToFile.Count > 0)
            {               
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
                    resultInfos.Add(ResultInfo.CreateResultInfo(line));
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
    }
}
