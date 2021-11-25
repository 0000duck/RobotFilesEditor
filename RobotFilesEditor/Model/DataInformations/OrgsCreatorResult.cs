using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobotFilesEditor.Model.DataOrganization;

namespace RobotFilesEditor.Model.DataInformations
{
    public class OrgsCreatorResult
    {
        public int Type { get; set; }
        public List<IOrgsElement> OrgsElements { get; set; }
        public OrgsCreatorResult()
        {
            OrgsElements = new List<IOrgsElement>();
        }
    }
}
