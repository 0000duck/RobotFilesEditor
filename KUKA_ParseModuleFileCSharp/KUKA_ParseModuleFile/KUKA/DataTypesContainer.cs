using ParseModuleFile.KUKA.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.KUKA
{
    public class DataTypesContainer : NotifyPropertyChanged 
    {
        #region properties

        private CustomStructures m_Structures;
        public CustomStructures Structures { get { return m_Structures; } set { Set(ref m_Structures, value); } }

        private CustomEnums m_Enums;
        public CustomEnums Enums { get { return m_Enums; } set { Set(ref m_Enums, value); } }
        #endregion properties

        #region constructors
        public DataTypesContainer(IEnumerable<CustomEnum> enums = null, IEnumerable<CustomStruct> structs = null)
        {
            m_Enums = new CustomEnums(enums);
            m_Structures = new CustomStructures(structs);
        }
        #endregion constructors
    }
}
