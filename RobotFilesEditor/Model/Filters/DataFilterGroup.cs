﻿using System.Collections.Generic;
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
        public List<string> LinesToAddToFile
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
        private List<string> _linesToAddToFile;

        public DataFilterGroup()
        {
            Filter = new Filter();
            LinesToAddToFile = new List<string>();
        }

        public List<string>CkeckAllFilters(List<string>listToCheck)
        {
            listToCheck = Filter.FilterContains(listToCheck);
            listToCheck = Filter.FilterNotContains(listToCheck);
            listToCheck = Filter.FilterRegexContain(listToCheck);
            listToCheck = Filter.FilterRegexNotContain(listToCheck);

            return listToCheck;
        }

        public void SetLinesToAddToFile(List<string> filesContent)
        {
           LinesToAddToFile=CkeckAllFilters(filesContent);
           LinesToAddToFile=LinesToAddToFile.Distinct().ToList();   
        }

        public string PrepareGroupToWrite()
        {
            string Buffor = "";
            //LinesToAddToFile = LinesToAddToFile.OrderBy(x => x).ToList();

            if (LinesToAddToFile.Count > 0)
            {
                //using(MemoryStream ms=new MemoryStream())
                
                //{                    
                //    for(int i=0; i<SpaceBefor; i++)
                //    {
                //        sw.WriteLine();
                //    } 

                //    if(string.IsNullOrEmpty(Header)==false){
                //        sw.WriteLine(Header);
                //    }
                   
                //    LinesToAddToFile.ForEach(x => sw.WriteLine(x));

                //    if (string.IsNullOrEmpty(Footer) == false){
                //        sw.WriteLine(Footer);
                //    }
              
                //    for (int i = 0; i < SpaceAfter; i++){
                //        sw.WriteLine();
                //    }                   

                //    Buffor = sw.ToString();   
                //}
            }

            return Buffor;
        }       
    }
}