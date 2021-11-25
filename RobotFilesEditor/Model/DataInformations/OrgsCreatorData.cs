using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Model.DataInformations
{
    public class OrgsCreatorData
    {
        public IDictionary<int,string> Paths { get; set; }
        public IDictionary<int, string> Description { get; set; }
        public IDictionary<int, string> Abort { get; set; }
        public IDictionary<int, string> AbortNumber { get; set; }
        public IDictionary<int, string> WithPart { get; set; }
        public IDictionary<int, int> JobNumber { get; set; }


        public OrgsCreatorData(IDictionary<int, string> paths, IDictionary<int, string> description, IDictionary<int, string> abort, IDictionary<int, string> abortNumber, IDictionary<int, string> withPart, IDictionary<int, int> jobNumber)
        {
            Paths = paths;
            Description = description;
            Abort = abort;
            AbortNumber = abortNumber;
            WithPart = withPart;
            JobNumber = jobNumber;
        }

        public OrgsCreatorData()
        {

        }
    }

    public class OrganizationLists
    {
        public int Type { get; set; }
        public List<string> Paths { get; set; }
        public List<string> Description { get; set; }
        public List<string> Abort { get; set; }
        public List<string> AbortNumber { get; set; }
        public List<string> WithPart { get; set; }
        public List<int> JobNumber { get; set; }

        public OrganizationLists(int type, List<string> paths, List<string> description, List<string> abort, List<string> abortNumber, List<string> withPart, List<int> jobNumber)
        {
            Type = type;
            Paths = paths;
            Description = description;
            Abort = abort;
            AbortNumber = abortNumber;
            WithPart = withPart;
            JobNumber = jobNumber;
        }
    }
}
