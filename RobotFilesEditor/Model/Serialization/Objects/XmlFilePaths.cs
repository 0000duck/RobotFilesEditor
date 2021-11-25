using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RobotFilesEditor.Model.Serialization.Objects
{
    public class XmlFilePaths
    {
        public List<Paths> PathList { get; } = new List<Paths>();
    }

    public class Paths
    {
        [XmlAttribute]
        public string DestinationPath { get; set; }
        public string SourcePath { get; set; }
    }
}
