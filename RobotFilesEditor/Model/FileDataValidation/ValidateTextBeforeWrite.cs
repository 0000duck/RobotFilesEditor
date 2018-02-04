using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor
{
    public static class ValidateText
    {
        public static void ValidateTextBeforeWrite(List<FileLineProperties>lisesToCheck)
        {
            var duplicates=lisesToCheck.GroupBy(x => x.VariableName).Where(group=>group.Count()>1);


        }
    }
}
