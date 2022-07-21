using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.KUKA.DataTypes
{
    public class CustomEnum : NotifyPropertyChanged
    {
        #region properties
        private bool m_IsGlobal;
        public bool IsGlobal { get { return m_IsGlobal; } set { Set(ref m_IsGlobal, value); } }

        private string m_Name;
        public string Name { get { return m_Name; } set { Set(ref m_Name, value); } }

        private ObservableCollection<string> m_Constants;
        public ObservableCollection<string> Constants { get { return m_Constants; } set { Set(ref m_Constants, value); } }
        #endregion properties

        #region constructors
        public CustomEnum(string name, List<string> constants, bool global = false)
        {
            m_Name = name;
            m_Constants = new ObservableCollection<string>(constants);
            m_IsGlobal = global;
        }
        #endregion constructors
    }
}
