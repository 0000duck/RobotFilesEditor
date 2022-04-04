using RobotFilesEditor.Model.Operations;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

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
        public List<string> FilesFilterContains(List<string> source)
        {
            List<string> allFiles = source;
            if (SrcValidator.UnclassifiedPaths == null)
                SrcValidator.UnclassifiedPaths = new List<string>();
            if (SrcValidator.AlreadyContain == null)
                SrcValidator.AlreadyContain = new List<string>();
            List<string> unclassifiedPath = new List<string>();
            if (Contain == null)
                return source;
            if (Contain.Any())
            {
                //TEMP
                if (GlobalData.ControllerType != "KRC4 Not BMW")
                    source = source.Where(x => (Contain.Exists(y => Path.GetFileName(x.ToLower()).Contains(y.ToLower()))) | (Contain.Exists(y => Path.GetFileName(x.ToLower()).Contains(y.ToLower())))).ToList();
            }


            foreach (string file in allFiles)
            {
                if (!source.Contains(file) && !SrcValidator.UnclassifiedPaths.Contains(file))
                    SrcValidator.UnclassifiedPaths.Add(file);
            }

            foreach (string file in source)
            {
                if (SrcValidator.UnclassifiedPaths.Contains(file))
                {
                    SrcValidator.UnclassifiedPaths.Remove(file);
                }
            }

            foreach (string file in SrcValidator.AlreadyContain)
            {
                if (SrcValidator.UnclassifiedPaths.Contains(file))
                {
                    SrcValidator.UnclassifiedPaths.Remove(file);
                }
            }

            SrcValidator.AlreadyContain.AddRange(source);
            return source; 
        }

        public List<string>FilesFilterNotContains(List<string> source)
        {
            if (NotContain?.Count > 0)
            {
                source = source.Where(x => NotContain.Exists(y => Path.GetFileName(x.ToLower()).Contains(y.ToLower())) == false).ToList();         
            }
            return source;
        }

        public List<string>FilesFilterRegexContain(List<string> source)
        {            
            if(string.IsNullOrEmpty(RegexContain) == false)
            {
                source = source.Where(x => Regex.IsMatch(Path.GetFileName(x), RegexContain,RegexOptions.IgnoreCase)).ToList();
            }
            return source;
        }

        public List<string>FilesFilterRegexNotContain(List<string> source)
        {
            if (string.IsNullOrEmpty(RegexNotContain) == false)
            {
                source = source.Where(x => Regex.IsMatch(Path.GetFileName(x), RegexNotContain,RegexOptions.IgnoreCase) == false).ToList();
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

        #region FileLinePropertiesFilter
        public List<FileLineProperties> FilterContains(List<FileLineProperties> source)
        {
            string tempstring = "";
            if (Contain?.Count > 0)
            {
                if (Contain.Contains("xg_tipdressg") && GlobalData.isWeldingRobot)
                {
                    List<bool> foundList = new List<bool>();
                    List<string> tipChangeGlobalPoints = ConfigurationManager.AppSettings["tipChangeGlobals" + GlobalData.ControllerType.Replace(" ", "_")].Split(',').Select(s => s.Trim()).ToList();
                    foreach (string command in tipChangeGlobalPoints)
                    {
                        bool itemFound = false;
                        foreach (var item in source.Where(x => x.LineContent.ToLower().Contains(command)))
                        {
                            itemFound = true;
                            break;
                        }
                        foundList.Add(itemFound);
                    }

                    string message = "";
                    int counter = 0;
                    foreach (bool foundData in foundList)
                    {
                        if (foundData == false)
                            message += tipChangeGlobalPoints[counter] + " ";
                        counter++;
                    }
                    if (!string.IsNullOrEmpty(message))
                        MessageBox.Show("No "+ message +"positions found!\r\nCorrect point names!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                source = source.Where(x => Contain.Exists(y => x.LineContent.ToLower().Contains(y.ToLower()))).ToList();
            }
            List<FileLineProperties> tempList = new List<FileLineProperties>();
            foreach (var item in source)
            {
                if (item.LineContent.ToLower().Contains("decl") && item.LineContent.ToLower().Contains("g_tipdressg") && GlobalData.ControllerType != "KRC4")
                {
                    tempstring = item.LineContent.Replace("DECL ", "");
                    tempstring = tempstring.Replace("GLOBAL ", "");
                    tempstring = tempstring.Replace("E6POS ", "");
                    tempstring = tempstring.Replace("FDAT ", "");
                    item.LineContent = tempstring;
                }
                if (item.LineContent.ToLower().Contains("decl") && item.LineContent.ToLower().Contains("xhome"))
                {
                    tempstring = item.LineContent.Replace("DECL ", "");
                    tempstring = tempstring.Replace("E6AXIS ", "");
                    item.LineContent = tempstring;
                }
                if (item.LineContent.ToLower().Contains("e6axis") && item.LineContent.ToLower().Contains("xhome"))
                {
                    tempstring = item.LineContent.Replace("E6AXIS ", "");
                    item.LineContent = tempstring;
                }
                tempList.Add(item);
            }

            return tempList;
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
                source = source.Where(x =>Regex.IsMatch(x.LineContent, RegexContain,RegexOptions.IgnoreCase)).ToList();

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
                source = source.Where(x => Regex.IsMatch(x.LineContent, RegexNotContain,RegexOptions.IgnoreCase) == false).ToList();
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
        #endregion FileLinePropertiesFilter
    }
}
