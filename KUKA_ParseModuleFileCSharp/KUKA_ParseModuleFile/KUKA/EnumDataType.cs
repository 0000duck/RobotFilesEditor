using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.KUKA
{
    public class EnumDataType<T> : Variable where T : struct, IConvertible
    {
        #region fields
        private string type;
        private T value;
        #endregion //fields

        #region properties
        public override string DataTypeName { get { return type; } }
        public T Value { get { return value; } set { Set(ref this.value, value); } }
        #endregion //properties

        public EnumDataType(string type, T value)
        {
            this.type = type;
            this.value = value;
        }

        public EnumDataType(string type, ANTLR.DataItems dataItems)
        {
            if (dataItems == null) return;
            if (dataItems.IsDictionary) throw new NotImplementedException("data items is a dictionary");
            value = (T) Enum.Parse(typeof(T), dataItems["value"].ToString().TrimStart('#'), true);
        }

        public override string ToString()
        {
            return "#" + value.ToString();
        }
    }
}
