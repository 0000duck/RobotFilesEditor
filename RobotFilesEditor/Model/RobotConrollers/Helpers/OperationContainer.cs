using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Model.RobotConrollers.Helpers
{
    public static class OperationContainer
    {
        public static void Init()
        {
            SrcFilesAll = new Dictionary<string, string>();
            DatFilesAll = new List<string>();
        }
        public static IDictionary<string, string> SrcFilesAll { get; set; }
        public static List<string> DatFilesAll { get; set; }

    }
}
