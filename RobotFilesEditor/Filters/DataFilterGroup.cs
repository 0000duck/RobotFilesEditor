using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public List<string>CkeckAllFilters(List<string>listToCheck)
        {
            listToCheck = Filter.FilterContainsAtName(listToCheck);
            listToCheck = Filter.FilterNotContainsAtName(listToCheck);
            listToCheck = Filter.FilterRegexContain(listToCheck);
            listToCheck = Filter.FilterRegexNotContain(listToCheck);

            return listToCheck;
        }

        public void SetLinesToAddToFile(List<string> filesContent)
        {
           LinesToAddToFile=CkeckAllFilters(filesContent);
           LinesToAddToFile.Distinct();   
        }

        public string PrepareGroupToWrite()
        {
            string Buffor = "";

            if (LinesToAddToFile.Count > 0)
            {
                using (StreamWriter sw = File.CreateText(""))
                {
                    if(SpaceBefor>0)
                    {
                        for(int i=0; i<SpaceBefor; i++){
                            sw.WriteLine();
                        }                            
                    }

                    if(string.IsNullOrEmpty(Header)==false){
                        sw.WriteLine(Header);
                    }

                    LinesToAddToFile.ForEach(x => sw.WriteLine(x));

                    if (string.IsNullOrEmpty(Footer) == false){
                        sw.WriteLine(Footer);
                    }
              
                    for (int i = 0; i < SpaceAfter; i++){
                        sw.WriteLine();
                    }                   

                    Buffor = sw.ToString();   
                }
            }

            return Buffor;
        }
    }
}
