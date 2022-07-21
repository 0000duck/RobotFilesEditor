using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ParseModuleFile.KUKA.ArchiveInfo
{
    public class ArchiveInfo
    {
        private XmlDocument doc = new XmlDocument();
        public string Name { get; set; }
        public string SerialNo { get; set; }
        public string RobotType { get; set; }
        public string SystemVersion { get; set; }
        public ObservableCollection<OptionInformation> Options { get; set; }

        public ArchiveInfo()
        {
            Name = "Unknown Robot";
        }

        public ArchiveInfo(string data, bool asFile=true)
        {
            if (asFile) doc.Load(data);
            else doc.LoadXml(data);
            XmlNamespaceManager xmlnsManager = new System.Xml.XmlNamespaceManager(doc.NameTable);
            xmlnsManager.AddNamespace("k", "KUKARoboter.Contracts.Archive");
            XmlNode xmlnodeName = doc.SelectSingleNode("/k:ArchiveInformation/k:Roboter/k:Name", xmlnsManager);
            XmlNode xmlnodeSerial = doc.SelectSingleNode("/k:ArchiveInformation/k:Roboter/k:Serialnumber", xmlnsManager);
            XmlNode xmlnodeType = doc.SelectSingleNode("/k:ArchiveInformation/k:Roboter/k:Type", xmlnsManager);
            XmlNodeList xmlOptions = doc.SelectNodes("/k:ArchiveInformation/k:Options/k:OptionData", xmlnsManager);
            XmlNode xmlnodeSystemVersion = doc.SelectSingleNode("/k:ArchiveInformation/k:Roboter/k:Type", xmlnsManager);
            Name = xmlnodeName.InnerText;
            SerialNo = xmlnodeSerial.InnerText;
            RobotType = xmlnodeType.InnerText;
            Options = new ObservableCollection<OptionInformation>();
            foreach (XmlNode childnode in xmlOptions)
            {
                Options.Add(new OptionInformation(childnode));
            }

        }
    }
}
