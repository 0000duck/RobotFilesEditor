using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.KUKA
{
    public class DataArray : Variable
    {
        private ObservableCollection<Variable> items = new ObservableCollection<Variable>();
        private string type;
        private int size;
        public int Size { get { return size; } }

        public override string DataTypeName { get { return type; } }

        public DataArray(int size, string type, Robot robot)
        {
            this.type = type;
            this.size = size;
            for (int i = 0; i < size; i++)
            {
                items.Add(DataTypes.MemorySelector.XGet(type, null, robot));
            }
        }

        public Variable this[int key]
        {
            get
            {
                if (key > size || key < 1) throw new ArgumentException("key");
                return items[key - 1];
            }
            set
            {
                if (key > size || key < 1) throw new ArgumentException("key");
                items[key - 1] = value;
            }
        }
        public override string ToString()
        {
            return "ArrayOf(" + type + " [" + size.ToString() + "])";
        }
    }
}
