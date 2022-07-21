using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ParseModuleFile.KUKA.ArchiveInfo
{
    public class OptionInformation
    {
        public string Name { get; set; }
        public Version Version { get; set; }

        public OptionInformation(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
            {
                switch (child.Name)
                {
                    case "Name":
                        Name = child.InnerText;
                        break;
                    case "Version":
                        Version = new Version(child);
                        break;
                }
            }
        }
    }
}
