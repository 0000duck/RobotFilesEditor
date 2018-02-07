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
    }
}
