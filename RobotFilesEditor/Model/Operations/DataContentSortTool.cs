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
        public List<DataFilterGroup> SortOlpDataFiles(List<DataFilterGroup> filterGroups)
        {           
            foreach (DataFilterGroup group in filterGroups)
            {
                List<FileLineProperties> linesBuffer = new List<FileLineProperties>();
                var groups = group.LinesToAddToFile.OrderBy(x => x.VariableName).GroupBy(y=>y.VariableName);

                foreach(var g in groups)
                {
                    linesBuffer.AddRange(g.OrderBy(x => x.VariableIndex).ToList());
                }

                group.LinesToAddToFile.Clear();
                group.LinesToAddToFile = linesBuffer;               
            }

            return filterGroups;
        }      
    }
}
