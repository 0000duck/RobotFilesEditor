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
        
        public static List<DataFilterGroup>SortData(List<DataFilterGroup> dataToSort, GlobalData.SortType sortType)
        {
            List<DataFilterGroup> result=new List<DataFilterGroup>();

            try
            {
                switch(sortType)
                {
                    case GlobalData.SortType.None: {
                            return dataToSort;
                        } break;
                    case GlobalData.SortType.OrderByVariable: {
                            result=SortOlpDataFiles(dataToSort);
                        } break;
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
