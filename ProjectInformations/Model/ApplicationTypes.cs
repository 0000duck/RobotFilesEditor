using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProjectInformations.Model
{
    [XmlRoot(ElementName = "ApplicationTypes")]
    public class ApplicationTypes : ObservableObject
    {

        #region ctor
        public ApplicationTypes()
        {
        }
        #endregion ctor


        #region methods

        #endregion methods

        #region fields

        #endregion fields

        #region properties
        [XmlElement(ElementName = "SpotType")]
        public SpotType SpotType { get; set; }
        [XmlElement(ElementName = "LaserType")]
        public LaserType LaserType { get; set; }
        [XmlElement(ElementName = "GlueType")]
        public GlueType GlueType { get; set; }
        [XmlElement(ElementName = "TchType")]
        public TchType TchType { get; set; }
        #endregion properties


        #region private methods

        #endregion private methods
    }
    [XmlRoot(ElementName = "SpotType")]
    public class SpotType : ApplicationBase
    {
        [XmlAttribute(AttributeName = "A04")]
        public string A04
        {
            get { return m_A04; }
            set { if (value != m_A04) { SetProperty(ref m_A04, value); A05 = ReverseStringValue(m_A04); } }
        }
        private string m_A04;
        [XmlAttribute(AttributeName = "A05")]
        public string A05
        {
            get { return m_A05; }
            set { if (value != m_A05) { SetProperty(ref m_A05, value); A04 = ReverseStringValue(m_A05); } }
        }
        private string m_A05;
    }

    [XmlRoot(ElementName = "LaserType")]
    public class LaserType : ApplicationBase
    {
        [XmlAttribute(AttributeName = "A15")]
        public string A15 {
            get { return m_A15; } 
            set { if (value != m_A15) { SetProperty(ref m_A15, value); B15 = ReverseStringValue(m_A15); } } }
        private string m_A15;
        [XmlAttribute(AttributeName = "B15")]
        public string B15 { 
            get { return m_B15; } 
            set { if (value != m_B15) { SetProperty(ref m_B15, value); A15 = ReverseStringValue(m_B15); } } }
        private string m_B15;
    }

    [XmlRoot(ElementName = "GlueType")]
    public class GlueType : ApplicationBase
    {
        [XmlAttribute(AttributeName = "A08")]
        public string A08
        {
            get { return m_A08; }
            set { if (value != m_A08) { SetProperty(ref m_A08, value); B08 = ReverseStringValue(m_A08); } }
        }
        private string m_A08;
        [XmlAttribute(AttributeName = "B08")]
        public string B08
        {
            get { return m_B08; }
            set { if (value != m_B08) { SetProperty(ref m_B08, value); A08 = ReverseStringValue(m_B08); } }
        }
        private string m_B08;
    }

    [XmlRoot(ElementName = "TchType")]
    public class TchType : ApplicationBase
    {
        [XmlAttribute(AttributeName = "A02")]
        public string A02
        {
            get { return m_A02; }
            set { if (value != m_A02) { SetProperty(ref m_A02, value); B02 = ReverseStringValue(m_A02); } }
        }
        private string m_A02;
        [XmlAttribute(AttributeName = "B02")]
        public string B02
        {
            get { return m_B02; }
            set { if (value != m_B02) { SetProperty(ref m_B02, value); A02 = ReverseStringValue(m_B02); } }
        }
        private string m_B02;
    }
}
