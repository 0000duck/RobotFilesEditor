using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProgramTextFormat.Model.Rules
{
    [XmlRoot(ElementName = "Rules")]
    public class Rules
    {
        [XmlElement(ElementName = "ProgramFormatRule")]
        public List<ProgramFormatRule> ProgramFormatRule { get; set; }

        [XmlIgnore]
                public string SelectedAction { get; set; }

    }
}
