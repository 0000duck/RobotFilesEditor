using System;
using System.Collections.Generic;
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
        private List<FileLineProperties> _linesToAddToFile;

        public DataFilterGroup()
        {
            Filter = new Filter();
            LinesToAddToFile = new List<FileLineProperties>();
        }

        public List<FileLineProperties> CkeckAllFilters(List<FileLineProperties> listToCheck)
        {
            listToCheck = Filter.FilterContains(listToCheck);
            listToCheck = Filter.FilterNotContains(listToCheck);
            listToCheck = Filter.FilterRegexContain(listToCheck);
            listToCheck = Filter.FilterRegexNotContain(listToCheck);

            return listToCheck;
        }

        public void SetLinesToAddToFile(List<FileLineProperties> filesContent)
        {
           LinesToAddToFile=CkeckAllFilters(filesContent);
           LinesToAddToFile = LinesToAddToFile.DistinctBy(x => x.LineContent).ToList();
        }

        public string PrepareGroupToWrite()
        {
            string Buffor = "";

            if (LinesToAddToFile.Count > 0)
            {               
                for (int i = 0; i < SpaceBefor; i++)
                {
                    Buffor += String.Format("\n");
                }

                if (string.IsNullOrEmpty(Header) == false)
                {
                    Buffor += String.Format("{0}\n", Header);
                }

                LinesToAddToFile.ForEach(x => Buffor+= String.Format("{0}\n", x.LineContent));

                if (string.IsNullOrEmpty(Footer) == false)
                {
                    Buffor += String.Format("{0}\n", Footer);
                }

                for (int i = 0; i < SpaceAfter; i++)
                {
                    Buffor += String.Format("\n");
                }       
            }

            return Buffor;
        }       
    }
}
