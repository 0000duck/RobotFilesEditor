using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ParseModuleFile.KUKA.ArchiveInfo
{
    public class Version
    {
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Revision { get; set; }
        public int Build { get; set; }
        public override string ToString()
        {
            return Major.ToString() + "." + Minor.ToString() + "." + Revision.ToString() + " (" + Build.ToString() + ")";
        }
        public Version(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
            {
                switch (child.Name)
                {
                    case "Major":
                        Major = int.Parse(child.InnerText);
                        break;
                    case "Minor":
                        Minor = int.Parse(child.InnerText);
                        break;
                    case "Revision":
                        Revision = int.Parse(child.InnerText);
                        break;
                    case "Build":
                        Build = int.Parse(child.InnerText);
                        break;
                }
            }
        }
    }
}
