using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RobotFilesEditor.Model.XML
{
    public static class WriteFilePaths
    {
        public static void WriteFile(string path)
        {
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory + "\\Destination_temp"))
                Directory.CreateDirectory(directory + "\\Destination_temp");
            if (!Directory.Exists(directory + "\\Source_temp"))
                Directory.CreateDirectory(directory + "\\Source_temp");

            XmlWriter xmlWriter = XmlWriter.Create(path);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("Paths");
            xmlWriter.WriteStartElement("DestinationPath");
            xmlWriter.WriteString(directory + "\\Destination_temp");
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("SourcePath");
            xmlWriter.WriteString(directory + "\\Source_temp");
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
        }
        
    } 
}
