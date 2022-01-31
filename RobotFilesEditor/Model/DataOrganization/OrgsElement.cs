using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Model.DataOrganization
{
    public class OrgsElement
    {
        public string JobDescription { get; set; }
        public string Path { get; set; }
        public string SelectedPLC { get; set; }
        public string WithPart { get; set; }
        public string HomeToCentralPath { get; set; }
        public string JobAndDescription { get; set; }
        //public string AbortDescription { get; set; }
        public ObservableCollection<KeyValuePair<string, int>> AnyJobValue { get; set; }
        public ObservableCollection<KeyValuePair<string, int>> UserNumValue { get; set; }
        public IDictionary<int, List<KeyValuePair<string, int>>> AnyJobUserNumValue { get; set; }
        public bool TypNumReq { get; set; }
        public string Abort { get; set; }
        public int? AbortNr { get; set; }
        public int? Id { get; set; }
    }
}
