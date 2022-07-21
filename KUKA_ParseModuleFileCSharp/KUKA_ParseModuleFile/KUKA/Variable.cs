using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.KUKA
{
    public abstract class Variable : NotifyPropertyChanged
    {
        #region constructors
        //protected DataType()
        //{
        //}
        #endregion // constructors

        internal string valName = "";
        public string ValName { get { return valName; } set { Set(ref valName, value); } }
        internal bool isGlobal = false;
        public bool IsGlobal { get { return isGlobal; } set { Set(ref isGlobal, value); } }
        internal bool isConst = false;
        public bool IsConst { get { return isConst; } set { Set(ref isConst, value); } }

        public abstract string DataTypeName { get; }

        public static string BtoStr(bool @in)
        {
            return @in ? "TRUE" : "FALSE";
        }

        public virtual bool HasData()
        {
            return true;
        }
    }

}
