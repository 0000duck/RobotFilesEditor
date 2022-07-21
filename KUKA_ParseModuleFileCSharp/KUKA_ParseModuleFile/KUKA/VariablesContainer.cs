using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.KUKA
{
    public class VariablesContainer : ObservableDictionary<string,Variables>
    {
        #region constructors
        public VariablesContainer() : base()
        {
        }
        #endregion // constructors
        #region methods

        public bool Add(string Module, string Name, Variable Item)
        {
            if (!base.ContainsKey(Module))
                base.Add(Module, new Variables());
            Variables list = base[Module];
            return list.Add(Name, Item);
        }

        public Variable Get(string Module, string Name)
        {
            if (base.ContainsKey(Module) && base[Module].ContainsKey(Name))
            {
                return this[Module][Name];
            }
            else
            {
                foreach (var item in this)
                {
                    if (item.Value.ContainsGlobalKey(Name)) return item.Value[Name];
                }
                return null;
            }
        }
        public override string ToString()
        {
            return this.Count.ToString() + " dat files.";
        }
        #endregion // methods
    }
}
