using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.KUKA.DataTypes
{
    public class CustomStructElement : NotifyPropertyChanged 
    {
        #region properties
        private string m_Type;
        public string Type { get { return m_Type; } set { Set(ref m_Type, value); } }

        private string m_Name;
        public string Name { get { return m_Name; } set { Set(ref m_Name, value); } }

        private ObservableCollection<int?> m_Array;
        public ObservableCollection<int?> Array { get { return m_Array; } set { Set(ref m_Array, value); } }
        #endregion properties

        #region constructors
        public CustomStructElement(string type, string name, List<int?> array = null)
        {
            m_Type = type;
            m_Name = name;
            if (array != null) m_Array = new ObservableCollection<int?>(array);
        }
        #endregion constructors
    }
}
