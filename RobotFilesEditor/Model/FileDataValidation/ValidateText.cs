using System.Collections.Generic;
using System.Linq;

namespace RobotFilesEditor
{
    public static class ValidateText
    {
        public static List<FileLineProperties>FindVaribleDuplicates(List<FileLineProperties>listesToCheck)
        {
            if(listesToCheck?.Count>0)
            {
                var duplicatesGroups = listesToCheck.GroupBy(x => x.Variable).Where(group => group.Count() > 1);

                if (duplicatesGroups.FirstOrDefault() != null)
                {
                    foreach (var group in duplicatesGroups)
                    {
                        listesToCheck.Where(x => x.Variable == group.Key).ToList().ForEach(y => y.HasExeption = true);
                    }
                }
            }
            return listesToCheck;        
        }
        public static void ValidateReapitingTextWhitExistContent(List<string> fileExistContent, ref List<string> newText)
        {
            List<string> result = new List<string>();

            foreach(string line in fileExistContent)
            {
                if(string.IsNullOrEmpty(line)==false && string.IsNullOrWhiteSpace(line)==false)
                {
                    result.AddRange(newText.Where(x => line.Contains(x)).ToList());                                                        
                }
            }

            foreach(var contain in result)
            {
                newText.Remove(contain);
            }
        }

        public static string CheckExtensionCorrectness(string extension)
        {
            if(extension.Contains(".")==false)
            {
                return $".{extension}";
            }
            else
            {
                return extension;
            }
        }
    }

  
}
