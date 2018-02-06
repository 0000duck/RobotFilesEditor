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
        public static void ValidateLienes(List<FileLineProperties>listesToCheck)
        {
            var duplicatesGroups= listesToCheck.GroupBy(x => x.Variable).Where(group=>group.Count()>1);

            if(duplicatesGroups.FirstOrDefault()!=null)
            {
                string errorMessage = "";

                foreach (var group in duplicatesGroups)
                {
                    foreach (var file in group)
                    {
                        errorMessage += CreateErrorInfo(file);
                    }                    
                }

                //throw new Exception(errorMessage);
            }          
        }

        private static string CreateErrorInfo(FileLineProperties fileLineProperties)
        {
            string text =$"Error in file: {Path.GetFileName(fileLineProperties.FileLinePath)}\n" +
                         $"at line: {fileLineProperties.LineNumber}\n" +
                         $"for varible: \"{fileLineProperties.Variable}\"\n\n";

            return text;
        }
    }
}
