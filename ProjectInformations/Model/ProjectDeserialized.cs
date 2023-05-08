using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProjectInformations.Model
{
    [XmlRoot(ElementName = "TypNumber")]
    public class TypNumber : ObservableObject, ICloneable
    {
        [XmlAttribute(AttributeName = "Number")]
        public int Number { get; set; }
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get { return m_Name; } set { SetProperty(ref m_Name, value); } }
        private string m_Name;

        [XmlIgnore]
        public bool Editable { get { return m_Editable; } set { SetProperty(ref m_Editable, value); } }
        private bool m_Editable;

        public void SetEditable(bool editable)
        {
            Editable = editable;
        }

        public object Clone()
        {
            var result = new TypNumber();
            result.Number = Number;
            result.Name = Name;
            result.Editable = Editable;
            return result;
        }

        internal void GetValuesFromCopy(TypNumber copyOfTyp)
        {
            Number = copyOfTyp.Number;
            Name = copyOfTyp.Name;
            Editable = copyOfTyp.Editable;
        }
    }

    [XmlRoot(ElementName = "TypNumbers")]
    public class TypNumbers
    {
        public TypNumbers()
        { }
        public TypNumbers(bool v)
        {
            TypNumber = new ObservableCollection<TypNumber>() { new Model.TypNumber() { Name = "NewType", Editable = false, Number = 1 } };
        }

        [XmlElement(ElementName = "TypNumber")]
        public ObservableCollection<TypNumber> TypNumber { get; set; }

        public void Sort()
        {
            var sortableList = TypNumber.OrderBy(x => x.Number).ToList();
            for (int i = 0; i < sortableList.Count; i++)
            {
                TypNumber.Move(TypNumber.IndexOf(sortableList[i]), i);
            }


        }
    }

    [XmlRoot(ElementName = "TypIDMain")]
    public class TypIDMain
    {
        [XmlAttribute(AttributeName = "Value")]
        public int Value { get; set; }
    }

    [XmlRoot(ElementName = "TypIDCommunal")]
    public class TypIDCommunal
    {
        [XmlAttribute(AttributeName = "Value")]
        public int Value { get; set; }
    }

    [XmlRoot(ElementName = "Project")]
    public class Project : ObservableObject
    {
        [XmlElement(ElementName = "TypNumbers")]
        public TypNumbers TypNumbers { get; set; }
        [XmlElement(ElementName = "TypIDMain")]
        public TypIDMain TypIDMain { get; set; }
        [XmlElement(ElementName = "TypIDCommunal")]
        public TypIDCommunal TypIDCommunal { get; set; }

        [XmlElement(ElementName = "ApplicationTypes")]
        public ApplicationTypes ApplicationTypes { get; set; }

        [XmlElement(ElementName = "UseCollDescr")]
        public UseCollDescr UseCollDescr { get; set; }

        [XmlAttribute(AttributeName = "Name")]
        public string Name { get { return m_name; } set { SetProperty(ref m_name, value); } }
        private string m_name;

        public Project()
        {}
        public Project(bool v)
        {
            Name = "NewProject";
            TypNumbers = new TypNumbers(true);
            TypIDMain = new TypIDMain() { Value = 0 };
            TypIDCommunal = new TypIDCommunal() { Value = 0 };
            ApplicationTypes = new ApplicationTypes() { GlueType = new GlueType() { A08 = "true" }, LaserType = new LaserType() { A15 = "true"}, SpotType = new SpotType() { A04 = "true"}, TchType = new TchType() {A02 = "true" } };
            RobotType = "KUKA";
        }

        [XmlAttribute(AttributeName = "RobotType")]
        public string RobotType { get; set; }
    }

    [XmlRoot(ElementName = "ProjectInfos")]
    public class ProjectInfos
    {
        [XmlElement(ElementName = "Project")]
        public List<Project> Project { get; set; }

        [XmlElement(ElementName = "SelectedProject")]
        public SelectedProject SelectedProject { get; set; }
    }

    [XmlRoot(ElementName = "SelectedProject")]
    public class SelectedProject 
    {
        [XmlAttribute(AttributeName = "Name")]

        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "UseCollDescr")]
    public class UseCollDescr
    {
        [XmlAttribute(AttributeName = "Value")]

        public string Value { get; set; }
    }
}
