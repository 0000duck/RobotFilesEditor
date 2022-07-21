using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.KUKA.DataTypes
{
    public class CustomStruct : NotifyPropertyChanged
    {
        #region properties
        private bool m_IsGlobal;
        public bool IsGlobal { get { return m_IsGlobal; } set { Set(ref m_IsGlobal, value); } }

        private string m_Name;
        public string Name { get { return m_Name; } set { Set(ref m_Name, value); } }

        private ObservableCollection<CustomStructElement> m_Elements;
        public ObservableCollection<CustomStructElement> Elements { get { return m_Elements; } set { Set(ref m_Elements, value); } }

        #endregion properties

        #region constructors
        public CustomStruct(string name, List<CustomStructElement> elements = null, bool global = false)
        {
            m_Name = name;
            if (elements != null) m_Elements = new ObservableCollection<CustomStructElement>(elements);
            else m_Elements = new ObservableCollection<CustomStructElement>();
            m_IsGlobal = global;
        }
        #endregion constructors
    }
}
