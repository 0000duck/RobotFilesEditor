using System;
using System.Collections.Generic;
using System.Linq;

namespace RobotFilesEditor
{
    public static class DataContentSortTool
    {
        private static List<DataFilterGroup> SortOlpDataFiles(List<DataFilterGroup> filterGroups)
        {
            try
            {
                foreach (DataFilterGroup group in filterGroups)
                {
                    List<FileLineProperties> linesBuffer = new List<FileLineProperties>();
                    var groups = group.LinesToAddToFile.OrderBy(x => x.VariableName).GroupBy(y => y.VariableName);

                    foreach (var g in groups)
                    {
                        linesBuffer.AddRange(g.OrderBy(x => x.VariableIndex).ToList());
                    }

                    group.LinesToAddToFile.Clear();
                    group.LinesToAddToFile = linesBuffer;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }    

            return filterGroups;
        }   
        
        public static List<DataFilterGroup>SortData(List<DataFilterGroup> dataToSort, string sortType)
        {
            List<DataFilterGroup> result=new List<DataFilterGroup>();

            try
            {
                if (sortType.ToLower().Contains("olp"))
                {
                    result = SortOlpDataFiles(dataToSort);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }   
    }
}
