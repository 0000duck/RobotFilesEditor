using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Model.DataOrganization
{
    public interface IOrgsElement : ICloneable
    {
        OrgsElement OrgsElement { get; set; }
    }
}
