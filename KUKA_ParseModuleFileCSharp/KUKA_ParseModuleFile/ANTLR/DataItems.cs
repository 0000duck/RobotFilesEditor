using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR
{
    public class DataItems : Dictionary<string, DataItems>
    {
        #region fields
        private bool isDictionary = false;
        private string value;
        #endregion // fields

        #region properties
        public bool IsDictionary { get { return isDictionary; } }

        public new DataItems this[string key]
        {
            get
            {
                if (isDictionary) return base[key];
                if (key == "value") return new DataItems(value);
                throw new ArgumentException("key");
            }
        }
        #endregion // properties

        #region constructors
        public DataItems(string value)
        {
            isDictionary = false;
            this.value = value;
        }

        public DataItems()
        {
            isDictionary = true;
        }
        #endregion // constructors

        #region methods
        public new bool ContainsKey(string key)
        {
            if (isDictionary) return base.ContainsKey(key);
            if (key == "value") return true;
            return false;
        }

        public override string ToString()
        {
            if (isDictionary)
            {
                List<string> items = new List<string>();
                foreach (var item in this)
                {
                    items.Add(item.Key + " " + item.Value.ToString());
                }
                string outV = string.Join(",", items);
                return "[" + outV + "]";
            }
            else
            {
                return value;
            }
        }

        public void Add(string name, string value)
        {
            if (isDictionary)
                base.Add(name, new DataItems(value));
            else
            {
                throw new ArgumentException("Is not as Dictionary");
            }
        }
        #endregion // methods
    }
}
