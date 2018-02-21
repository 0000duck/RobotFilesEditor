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
                        } 
                    case GlobalData.SortType.OrderByVariable: {
                            return SortOlpDataFiles(dataToSort);
                        } 
                    case GlobalData.SortType.OrderByOrderNumber:  {
                            return SortGlobalFilesData(dataToSort);
                        }
                }

                return result;              
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        private static List<DataFilterGroup>SortGlobalFilesData(List<DataFilterGroup> filterGroups)
        {
            foreach (DataFilterGroup group in filterGroups)
            {
                List<FileLineProperties> linesBuffer = new List<FileLineProperties>();
                var groups = group.LinesToAddToFile.GroupBy(y => y.VariableOrderNumber);

                groups.OrderBy(x => x.Key);

                foreach (var g in groups)
                {
                    linesBuffer.AddRange(g);
                }

                group.LinesToAddToFile.Clear();
                group.LinesToAddToFile = linesBuffer;
            }

            return filterGroups;
        }
    }
}
