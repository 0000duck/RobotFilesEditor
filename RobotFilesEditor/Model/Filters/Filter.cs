using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
                source = source.Where(x => Regex.IsMatch(Path.GetFileName(x), RegexContain)).ToList();
            }
            return source;
        }

        public List<string>FilesFilterRegexNotContain(List<string> source)
        {
            if (string.IsNullOrEmpty(RegexNotContain) == false)
            {
                source = source.Where(x => Regex.IsMatch(Path.GetFileName(x), RegexNotContain) == false).ToList();
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
        public List<FileLineProperties> FilterContains(List<FileLineProperties> source)
        {
            if (Contain?.Count > 0)
            {
                source = source.Where(x => Contain.Exists(y => x.LineContent.Contains(y))).ToList();
            }
            return source;
        }

        public List<FileLineProperties> FilterNotContains(List<FileLineProperties> source)
        {
            if (NotContain?.Count > 0)
            {
                source = source.Where(x => NotContain.Exists(y => x.LineContent.Contains(y)) == false).ToList();
            }
            return source;
        }

        public List<FileLineProperties> FilterRegexContain(List<FileLineProperties> source, bool regexOnly=false)
        {
            if (string.IsNullOrEmpty(RegexContain) == false)
            {
                source = source.Where(x =>Regex.IsMatch(x.LineContent, RegexContain)).ToList();

                if (regexOnly)
                {
                    source.ForEach(x => x.LineContent = GetMachFromRegex(x.LineContent, RegexContain));
                }
            }
            return source;
        }

        private string GetMachFromRegex(string source, string pattern)
        {
            string result;
            Match match;

            match = Regex.Match(source, pattern);
            result = match.Value;

            return result;
        }

        public List<FileLineProperties> FilterRegexNotContain(List<FileLineProperties> source)
        {
            if (string.IsNullOrEmpty(RegexNotContain) == false)
            {
                source = source.Where(x => Regex.IsMatch(x.LineContent, RegexNotContain) == false).ToList();
            }
            return source;
        }

        public List<FileLineProperties> CheckAllFilters(List<FileLineProperties> source, bool regexOnly=false)
        {
            source = FilterContains(source);
            source = FilterNotContains(source);
            source = FilterRegexContain(source, regexOnly);
            source = FilterRegexNotContain(source);
            return source;
        }
        #endregion CommonFilter
    }
}
