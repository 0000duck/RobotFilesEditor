using RobotFilesEditor.Model.Operations;
using System.Collections.Generic;
using System.Linq;

namespace RobotFilesEditor
{
    public static class ValidateText
    {
        public static List<FileLineProperties>FindVaribleDuplicates(List<FileLineProperties>listesToCheck)
        {
            if(listesToCheck.Any())
            {
                var duplicatesGroups = listesToCheck.GroupBy(x => x.Variable).Where(group => group.Count() > 1);
                //if (duplicatesGroups.ToList().Count > 0)
                //    duplicatesGroups = SrcValidator.FilterDuplicates(duplicatesGroups);
                if (duplicatesGroups.FirstOrDefault() != null)
                {                    
                    foreach (var group in duplicatesGroups)
                    {
                        bool clearList = true;
                        foreach (var item in listesToCheck )
                        {
                            if (!item.LineContent.ToLower().Contains("deltamfg"))
                            {
                                clearList = false;
                                break;
                            }
                        }
                        if (clearList)
                            listesToCheck.Clear();
                        if (group.Key.ToLower() != "deltax")
                            listesToCheck.Where(x => x.Variable == group.Key).ToList().ForEach(y => y.HasExeption = true);
                    }
                }
            }
            return listesToCheck;        
        }
       
        public static void ValidateReapitingTextWhitExistContent(List<ResultInfo> fileExistingContent, ref List<ResultInfo> newText)
        {
            List<ResultInfo> result = new List<ResultInfo>();            

            foreach (ResultInfo line in newText)
            {
                if (string.IsNullOrWhiteSpace(line.Content) == false)
                {
                    fileExistingContent.RemoveAll(x => x.Content.Contains(line.Content));
                }
            }         
        }

        public static void ValidateReapitingTextWhitExistContent(List<string> fileExistingContent, ref List<string> newText)
        {
            List<string> result = new List<string>();

            foreach (string line in fileExistingContent)
            {
                if (string.IsNullOrWhiteSpace(line) == false)
                {
                    fileExistingContent.RemoveAll(x => x.Contains(line));
                }
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
