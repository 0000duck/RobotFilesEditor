using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile
{
    [System.Serializable()]
    public class Variable : NotifyPropertyChanged, ISerializable
    {
        #region fields
        private bool isArray;
        private bool isTable;
        private bool isGlobal;

        private int count;
        private int count2;

        private string type;
        private string name;
        private string module;

        private DynamicMemory value;

        private ObservableCollection<DynamicMemory> listOfValue;
        private ObservableCollection<ObservableCollection<DynamicMemory>> listOflistOfValue;
        #endregion // fields

        #region properties
        public bool IsArray { get { return isArray; } set { Set(ref isArray, value); } }
        public bool IsTable { get { return isTable; } set { Set(ref isTable, value); } }
        public bool IsGlobal { get { return isGlobal; } set { Set(ref isGlobal, value); } }

        public int Count { get { return count; } set { Set(ref count, value); } }
        public int Count2 { get { return count2; } set { Set(ref count2, value); } }

        public string Type { get { return type; } set { Set(ref type, value); } }
        public string Name { get { return name; } set { Set(ref name, value); } }
        public string Module { get { return module; } set { Set(ref module, value); } }
        public DynamicMemory Value { get { return value; } set { Set(ref this.value, value); } }

        public ObservableCollection<DynamicMemory> ListOfValue { get { return listOfValue; } set { Set(ref listOfValue, value); } }
        public ObservableCollection<ObservableCollection<DynamicMemory>> ListOflistOfValue { get { return listOflistOfValue; } set { Set(ref listOflistOfValue, value); } }

        public DynamicMemory this[int index]
        {
            get
            {
                if (isArray == false)
                    return this.value;
                if (index - 1 > count)
                    throw new IndexOutOfRangeException();
                if (index - 1 > listOfValue.Count)
                    throw new IndexOutOfRangeException();
                return listOfValue[index - 1];
            }
            set
            {
                if (isArray == false)
                {
                    this.value = value;
                    return;
                }
                if (index - 1 > count)
                    throw new IndexOutOfRangeException();
                if (index - 1 > listOfValue.Count)
                    throw new IndexOutOfRangeException();
                listOfValue[index - 1] = value;
            }
        }

        public DynamicMemory this[int index1, int index2]
        {
            get
            {
                if (isTable == false)
                    return value;
                if (index1 - 1 > count)
                    throw new IndexOutOfRangeException();
                if (index1 - 1 > listOflistOfValue.Count)
                    throw new IndexOutOfRangeException();
                if (index2 - 1 > count2)
                    throw new IndexOutOfRangeException();
                if (index2 - 1 > listOflistOfValue[index1 - 1].Count)
                    throw new IndexOutOfRangeException();
                return listOflistOfValue[index1 - 1][index2 - 1];
            }
            set
            {
                if (isTable == false)
                {
                    this.value = value;
                    return;
                }
                if (index1 - 1 > count)
                    throw new IndexOutOfRangeException();
                if (index1 - 1 > listOflistOfValue.Count)
                    throw new IndexOutOfRangeException();
                if (index2 - 1 > count2)
                    throw new IndexOutOfRangeException();
                if (index2 - 1 > listOflistOfValue[index1 - 1].Count)
                    throw new IndexOutOfRangeException();
                listOflistOfValue[index1 - 1][index2 - 1] = value;
            }
        }
        #endregion

        #region constructors
        protected Variable(SerializationInfo info, StreamingContext context)
        {
            isGlobal = info.GetBoolean("global");
            type = info.GetString("type");
            name = info.GetString("name");
            //_value = info.GetValue("value", GetType(KUKA_Memory))
        }

        public Variable(string Module, bool isGlobal, string type, string name)
        {
            this.module = Module;
            this.isGlobal = isGlobal;
            this.type = type;
            this.name = name.ToUpperInvariant();
        }

        public Variable(string Module, bool isGlobal, string type, string name, int count)
        {
            this.module = Module;
            isArray = true;
            this.count = count;
            this.isGlobal = isGlobal;
            this.type = type;
            this.name = name.ToUpperInvariant();
            listOfValue = new ObservableCollection<DynamicMemory>();
            for (int i = 1; i <= count; i++)
            {
                listOfValue.Add(null);
            }
        }

        public Variable(string Module, bool isGlobal, string type, string name, int count, int count2)
        {
            this.module = Module;
            isTable = true;
            this.count = count;
            this.count2 = count2;
            this.isGlobal = isGlobal;
            this.type = type;
            this.name = name.ToUpperInvariant();
            listOflistOfValue = new ObservableCollection<ObservableCollection<DynamicMemory>>();
            for (int i = 1; i <= count; i++)
            {
                ObservableCollection<DynamicMemory> _list = new ObservableCollection<DynamicMemory>();
                for (int j = 1; j <= count2; j++)
                {
                    _list.Add(null);
                }
                listOflistOfValue.Add(_list);
            }
        }

        public Variable(string Module, bool type_global, string type, string name, DynamicMemory value)
        {
            this.module = Module;
            this.isGlobal = type_global;
            this.type = type;
            this.name = name.ToUpperInvariant();
            this.value = value;
        }

        #endregion // constructors

        #region methods
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("global", isGlobal);
            info.AddValue("type", type);
            info.AddValue("name", name);
            //info.AddValue("value", GetType(KUKA_Memory))
        }

        public override string ToString()
        {
            if (isTable)
            {
                return type + "[" + count.ToString() + "," + count2.ToString() + "]";
            }
            else if (isArray)
            {
                return type + "[" + count.ToString() + "]";
            }
            else
            {
                return type + "=" + value.ToString();
            }
        }

        #endregion // methods
    }
}
