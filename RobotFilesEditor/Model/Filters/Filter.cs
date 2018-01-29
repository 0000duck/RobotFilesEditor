using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor
{
    public class Filter
    {
        public List<string> ContainsAtName
        {
            get { return _containsAtName; }
            set
            {
                if (value == null)
                {
                    _containsAtName = new List<string>();
                }

                if (_containsAtName != value)
                {
                    _containsAtName = value;
                }
            }
        }
        public List<string> NotContainsAtName
        {
            get { return _notContainsAtName; }
            set
            {
                if (value == null)
                {
                    _notContainsAtName = new List<string>();
                }

                if (_notContainsAtName != value)
                {
                    _notContainsAtName = value;
                }
            }
        }
        public string RegexContain
        {
            get { return _regexContain; }
            set
            {
                if (value == null)
                {
                    _regexContain = "";
                }

                if (_regexContain != value)
                {
                    _regexContain = value;
                }
            }
        }
        public string RegexNotContain
        {
            get { return _regexNotContain; }
            set
            {
                if (value == null)
                {
                    _regexContain = "";
                }

                if (_regexNotContain != value)
                {
                    _regexNotContain = value;
                }
            }
        }
      
        public List<string> _containsAtName;
        public List<string> _notContainsAtName;
        public string _regexContain;
        public string _regexNotContain;

        public Filter()
        {          
            ContainsAtName = new List<string>();
            NotContainsAtName = new List<string>();            
        }

        #region FileFilter
        public List<string>FilesFilterContains(List<string>source)
        {
            if(ContainsAtName?.Count>0)
            {
                source = source.Where(x => ContainsAtName.Exists(y => Path.GetFileName(x).Contains(y))).ToList();
            }
                return source; 
        }

        public List<string>FilesFilterNotContains(List<string> source)
        {
            if (NotContainsAtName?.Count > 0)
            {
                source = source.Where(x => NotContainsAtName.Exists(y => Path.GetFileName(x).Contains(y)) == false).ToList();         
            }
            return source;
        }

        public List<string>FilesFilterRegexContain(List<string> source)
        {            
            if(string.IsNullOrEmpty(RegexContain) == false)
            {
                source = source.Where(x => System.Text.RegularExpressions.Regex.IsMatch(Path.GetFileName(x), RegexContain)).ToList();
            }
            return source;
        }

        public List<string>FilesFilterRegexNotContain(List<string> source)
        {
            if (string.IsNullOrEmpty(RegexNotContain) == false)
            {
                source = source.Where(x => System.Text.RegularExpressions.Regex.IsMatch(Path.GetFileName(x), RegexNotContain) == false).ToList();
            }
            return source;
        }

        public List<string>CheckAllFilesFilters(List<string>source)
        {
            source = FilesFilterContains(source);
            source = FilesFilterNotContains(source);
            source = FilesFilterRegexContain(source);
            source = FilesFilterRegexNotContain(source);
            return source;
        }
        #endregion FileFilter

        #region CommonFilter
        public List<string> FilterContains(List<string> source)
        {
            if (ContainsAtName?.Count > 0)
            {
                source = source.Where(x => ContainsAtName.Exists(y => x.Contains(y))).ToList();
            }
            return source;
        }

        public List<string> FilterNotContains(List<string> source)
        {
            if (NotContainsAtName?.Count > 0)
            {
                source = source.Where(x => NotContainsAtName.Exists(y => x.Contains(y)) == false).ToList();
            }
            return source;
        }

        public List<string> FilterRegexContain(List<string> source)
        {
            if (string.IsNullOrEmpty(RegexContain) == false)
            {
                source = source.Where(x => System.Text.RegularExpressions.Regex.IsMatch(Path.GetFileName(x), RegexContain)).ToList();
            }
            return source;
        }

        public List<string> FilterRegexNotContain(List<string> source)
        {
            if (string.IsNullOrEmpty(RegexNotContain) == false)
            {
                source = source.Where(x => System.Text.RegularExpressions.Regex.IsMatch(x, RegexNotContain) == false).ToList();
            }
            return source;
        }

        public List<string> CheckAllFilters(List<string> source)
        {
            source = FilterContains(source);
            source = FilterNotContains(source);
            source = FilterRegexContain(source);
            source = FilterRegexNotContain(source);
            return source;
        }
        #endregion CommonFilter
    }
}
