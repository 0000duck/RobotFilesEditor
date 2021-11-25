using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Model.Operations.DataClass
{
    public class TrelloReportDatas
    {
        public string Task { get; set; }
        public bool Complete { get; set; }
        public string WhoCompleted { get; set; }

        public TrelloReportDatas()
        {

        }
    }
}
