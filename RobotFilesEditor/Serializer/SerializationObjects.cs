using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RobotFilesEditor.Serializer
{
    [Serializable]
    [XmlRoot("ControlersConfiguration")]
    public class ControlersConfiguration
    {        
        [XmlArray("Controlers")]
        [XmlArrayItem("Controler")]
        public List<SingleControler> Controlers { get; set; }        
    }

    [Serializable]
    [XmlRoot("ControlersConfiguration")]
    public class SingleControler
    {
        [XmlAttribute("ControlerType")]
        public string ControlerType { get; set; }       

        [XmlArray("FilesToCopy")]
        [XmlArrayItem("FilesFilter")]
        public List<FileFilter> FilesToCopy { get; set; }

         [XmlArray("DataToCopy")]
        [XmlArrayItem("FilesFilter")]
        public List<FileFilter> DataToCopy { get; set; }

        [XmlArray("FilesToRemove")]
        [XmlArrayItem("FilesFilter")]
        public List<FileFilter> FilesToRemove { get; set; }       
    }   

    [Serializable]  
    public class FileFilter
    {
        [XmlAttribute("Destination")]
        public string DestinationFolder { get; set; }

        [XmlAttribute("Type")]
        public string Type { get; set; }

        [XmlArray("FilesExtension")]
        [XmlArrayItem("Extension")]
        public List<string> FilesExtension { get; set; }

        [XmlArray("ContainNames")]
        [XmlArrayItem("ContainName")]
        public List<string> ContainNames { get; set; }        
    }

}

