using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.KUKA
{
    public class Variables : NotifyPropertyChanged 
    {
        #region fields
        private bool isPublic = false;
        private ObservableDictionary<string, Variable> list = new ObservableDictionary<string, Variable>();
        #endregion // fields

        #region properties
        public ObservableDictionary<string, Variable> Items { get { return list; } }
        public Variable this[string Name]
        {
            get
            {
                if (ContainsKey(Name)) return list[Name];
                throw new ArgumentException("Name");
            }
            set
            {
                if (ContainsKey(Name)) list[Name] = value;
                else throw new ArgumentException("Name");
            }
        }
        public bool IsPublic { get { return isPublic; } set { Set(ref isPublic, value); } }
        #endregion // properties

        #region methods
        public bool Add(string Name, Variable Item)
        {
            if (list.ContainsKey(Name)) return false;
            list.Add(Name, Item);
            return true;
        }
        public bool ContainsKey(string Name)
        {
            if (list.ContainsKey(Name)) return true;
            return false;
        }
        public bool ContainsGlobalKey(string Name)
        {
            return (list.ContainsKey(Name) && list[Name].IsGlobal);
        }
        public override string ToString()
        {
            return (isPublic ? "Public " : "") + "datalist with " + list.Count.ToString() + " elements.";
        }
        #endregion // methods
    }
}
