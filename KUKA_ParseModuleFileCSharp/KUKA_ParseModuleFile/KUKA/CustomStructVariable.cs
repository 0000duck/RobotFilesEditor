using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.KUKA
{
    public class CustomStructVariable : Variable
    {
        private string m_DataTypeName;
        public override string DataTypeName {  get { return m_DataTypeName; } }

        //TODO: implement a IntVariable,RealVariable,BoolVariable,EnumVariable etc.
        private ObservableDictionary<string, Variable> m_Items;
        public ObservableDictionary<string, Variable> Items { get { return m_Items; } set { Set(ref m_Items, value); } }
    }
}
