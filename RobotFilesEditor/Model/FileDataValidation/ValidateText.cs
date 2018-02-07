using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor
{
    public static class ValidateText
    {
        public static List<FileLineProperties>ValidateLienes(List<FileLineProperties>listesToCheck)
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

        public static void ValidateTextWhitExistContent(List<string> fileExistContent, ref List<string> newText)
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
    }

  
}
