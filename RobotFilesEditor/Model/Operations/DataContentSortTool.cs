using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RobotFilesEditor
{
    public class DataContentSortTool
    {
        public List<DataFilterGroup> SortOlpDataFiles(List<DataFilterGroup>filterGroups)
        {
            Dictionary<string, int> sortBuffer = new Dictionary<string, int>();

            string indexPattern = @"\[[0-9]+,*\]";
            Regex indexRegex = new Regex(indexPattern);
            Match indexMatch;

            string valuePattern = "[0-9]+";
            Regex valueRegex = new Regex(valuePattern);
            Match valueMatch;

            foreach (DataFilterGroup group in filterGroups)
            {
                sortBuffer = new Dictionary<string, int>();

                foreach (string line in group.LinesToAddToFile)
                {                   
                    indexMatch = indexRegex.Match(line);
                    int value = -1;

                    if (string.IsNullOrEmpty(indexMatch.Value)==false)
                    {
                        valueMatch = valueRegex.Match(indexMatch.Value);

                        int.TryParse(valueMatch.Value, out value);

                        if(indexMatch.Value.Contains(','))
                        {
                            sortBuffer = UpValues(sortBuffer, value);
                        }
                    }    
                    sortBuffer.Add(line, value);
                }

                group.LinesToAddToFile = GroupByContain(sortBuffer, group);
            }

            return filterGroups;
        }

        private Dictionary<string, int> UpValues(Dictionary<string, int> source, int value)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();

            foreach (var item in source)
            {
                if (item.Value>value)
                {
                    result.Add(item.Key, item.Value + 1);
                }else
                {
                    result.Add(item.Key, item.Value);
                }
            }
            return result;
        }

        private List<string>DictionaryToStrinList(Dictionary<string, int> source)
        {
            source.OrderBy(x => x.Value);
            List<string> result = new List<string>();

            foreach (var item in source)
            {
                result.Add(item.Key);
            }

            return result;
        }

        private List<string>GroupByContain(Dictionary<string, int> source, DataFilterGroup filterGroup)
        {
            source = source.OrderBy(x => x.Value).ToDictionary(x=>x.Key, x=>x.Value);
            Dictionary<string, int> sourceTemp = new Dictionary<string, int>(source);
            List<string> result = new List<string>();

            foreach (string contain in filterGroup.Filter.Contain)
            {
                foreach(var item in source)
                {
                    if(item.Key.Contains(contain) && item.Value>=0)
                    {
                        result.Add(item.Key);
                        sourceTemp.Remove(item.Key);
                    }
                }
                source = new Dictionary<string, int>(sourceTemp);
            }

            if(source.Count>0)
            {
                source = source.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
                foreach (var item in source)
                {
                    result.Add(item.Key);
                }
            }

            return result;
        }
    }
}
