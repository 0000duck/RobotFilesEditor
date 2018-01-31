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
        public List<string> Contain
        {
            get { return _contain; }
            set
            {
                if (value == null)
                {
                    _contain = new List<string>();
                }

                if (_contain != value)
                {
                    _contain = value;
                }
            }
        }
        public List<string> NotContain
        {
            get { return _notContain; }
            set
            {
                if (value == null)
                {
                    _notContain = new List<string>();
                }

                if (_notContain != value)
                {
                    _notContain = value;
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
      
        public List<string> _contain;
        public List<string> _notContain;
        public string _regexContain;
        public string _regexNotContain;

        public Filter()
        {          
            Contain = new List<string>();
            NotContain = new List<string>();            
        }

        #region FileFilter
        public List<string>FilesFilterContains(List<string>source)
        {
            if(Contain?.Count>0)
            {
                source = source.Where(x => Contain.Exists(y => Path.GetFileName(x).Contains(y))).ToList();
            }
                return source; 
        }

        public List<string>FilesFilterNotContains(List<string> source)
        {
            if (NotContain?.Count > 0)
            {
                source = source.Where(x => NotContain.Exists(y => Path.GetFileName(x).Contains(y)) == false).ToList();         
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
            if (Contain?.Count > 0)
            {
                source = source.Where(x => Contain.Exists(y => x.Contains(y))).ToList();
            }
            return source;
        }

        public List<string> FilterNotContains(List<string> source)
        {
            if (NotContain?.Count > 0)
            {
                source = source.Where(x => NotContain.Exists(y => x.Contains(y)) == false).ToList();
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
