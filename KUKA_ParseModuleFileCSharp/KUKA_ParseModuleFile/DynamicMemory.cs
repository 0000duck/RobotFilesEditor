using Antlr4.Runtime;
using ParseModuleFile.KUKA.Enums;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile
{
    public class DynamicMemory : DynamicObject
    {
        protected Dictionary<string, object> _data;
        protected DynamicMemory()
        {
        }
        public DynamicMemory(string[] names, Type[] types)
        {
            if (names.Length != types.Length)
                throw new ArgumentOutOfRangeException();
            _data = new Dictionary<string, object>();
            for (int i = 0; i <= names.Length - 1; i++)
            {
                if (types[i] == typeof(string))
                {
                    _data.Add(names[i], "");
                }
                else
                {
                    _data.Add(names[i], Activator.CreateInstance(types[i]));
                }
            }
        }
        public DynamicMemory(string[] names, DynamicMemory[] memory)
        {
            if (names.Length != memory.Length)
                throw new ArgumentOutOfRangeException();
            _data = new Dictionary<string, object>();
            for (int i = 0; i <= names.Length - 1; i++)
            {
                _data.Add(names[i], memory[i]);
            }
        }

        public DynamicMemory(string[] names, object[] memory)
        {
            if (names.Length != memory.Length)
                throw new ArgumentOutOfRangeException();
            _data = new Dictionary<string, object>();
            for (int i = 0; i <= names.Length - 1; i++)
            {
                _data.Add(names[i], memory[i]);
            }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (_data.ContainsKey(binder.Name))
            {
                result = _data[binder.Name];
                return true;
            }
            result = null;
            return false;
        }
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (_data.ContainsKey(binder.Name))
            {
                _data[binder.Name] = value;
                return true;
            }
            return false;
        }
        public bool ContainsKey(string key)
        {
            return _data.ContainsKey(key);
        }
        public Type GetType(string key)
        {
            if (!_data.ContainsKey(key)) throw new ArgumentException("key");
            return _data[key].GetType();
        }
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _data.Keys;
        }

        public object this[string key]
        {
            get {
                if (!_data.ContainsKey(key)) return null;
                return _data[key]; 
            }
            set { _data[key] = value; }
        }

        public override string ToString()
        {
            string retString = "{";
            bool start = true;
            foreach (KeyValuePair<string, object> Item in _data)
            {
                string x = "";
                if ((Item.Value) is double)
                {
                    x = " " + Item.Value.ToString().Replace(",", ".");
                    if (double.IsNaN((double)Item.Value))
                        x = " 0";
                }
                else if ((Item.Value) is string)
                {
                    x = "[] \"" + Item.Value + "\"";
                }
                else if ((Item.Value) is bool)
                {
                    if ((bool)Item.Value == true)
                    {
                        x = " TRUE";
                    }
                    else
                    {
                        x = " FALSE";
                    }
                }
                else if ((Item.Value) is GrpACT_Type || (Item.Value) is Grp_STATE || (Item.Value) is MECH_TYPE || (Item.Value) is CIRC_MODE_args || (Item.Value) is CIRC_TYPE || (Item.Value) is ORI_TYPE || (Item.Value) is IPO_M_T || (Item.Value) is OPTION_CTL || (Item.Value) is swp_GUN_MOUNTING)
                {
                    x = " #" + Item.Value.ToString();
                }
                else if (Item.Value == null)
                {
                    x = " #NOTHING#";
                }
                else
                {
                    x = " " + Item.Value.ToString();
                }
                if (start)
                {
                    start = false;
                }
                else
                {
                    retString += ",";
                }
                retString += Item.Key + x;
            }
            retString += "}";
            return retString;
        }
        /*
 * 		COMMENT=1, FILEATTRIBUTES=2, NEWLINE=3, DEFDAT=4, ENDDAT=5, DECL=6, GLOBAL=7, 
PUBLIC=8, EXT=9, IN=10, OUT=11, SIGNAL=12, TO=13, ENUMD=14, STRUCD=15, 
ASSIGMENT=16, RBracketOpen=17, RBracketClose=18, BracketOpen=19, BracketClose=20, 
BraceOpen=21, BraceClose=22, COMMA=23, COLON=24, WS=25, BOOL=26, ID=27, 
ENUM=28, INT=29, FLOAT=30, BYTE=31, STRING=32;
*/
        /* rules
         * 		RULE_prog = 0, RULE_data = 1, RULE_defdat = 2, RULE_noDecl = 3, RULE_assign = 4, 
		RULE_decl = 5, RULE_ext = 6, RULE_signal = 7, RULE_myenum = 8, RULE_mystruc = 9, 
		RULE_structure = 10, RULE_parameterList = 11, RULE_sparamList = 12, RULE_parameters = 13, 
		RULE_params = 14, RULE_signalRange = 15, RULE_array = 16, RULE_idList = 17, 
		RULE_value = 18;
*/

        private static void ParseStructure(ANTLR.kukaParser.StructureContext context, DynamicMemory obj)
        {
            string curID=null;
            //bool isArray = false;
            //ANTLR.kukaParser.ValueContext curVal;
            foreach (var child in context.children)
            {
                if (child.Payload is CommonToken)
                {
                    CommonToken token = (CommonToken)child.Payload;
                    if (token.Type == ANTLR.kukaLexer.ID)
                    {
                        curID = token.Text;
                        //isArray = false;
                    }
                }
                else if (child.Payload is ANTLR.kukaParser.ValueContext)
                {
                    if (curID != null && obj.ContainsKey(curID))
                    {
                        var val = ParseValue((ANTLR.kukaParser.ValueContext)child, obj.GetType(curID));
                        if (val.GetType() != obj.GetType(curID))
                        {
                            System.Diagnostics.Debugger.Break();
                        }
                        obj[curID] = val;
                    }
                        
                }
                else if (child.Payload is ANTLR.kukaParser.ArrayContext)
                {
                    //isArray = true;
                }
                else
                {
                    System.Diagnostics.Debugger.Break();
                }

            }
        }

        private static object ParseValue(ANTLR.kukaParser.ValueContext context, Type DestinationType)
        {
            if (context.ChildCount != 1) System.Diagnostics.Debugger.Break();
            var child = context.children[0];
            //if (child is ANTLR.kukaParser.StructureContext)
                //ParseStructure((ANTLR.kukaParser.StructureContext)child);
            //else 
            if (child.Payload is CommonToken)
            {
                CommonToken token = (CommonToken) child.Payload;
                switch (token.Type)
                {
                    case ANTLR.kukaLexer.BOOL:
                        if (DestinationType != typeof(bool)) throw new ArgumentException("DestinationType");
                        return token.Text;
                    case ANTLR.kukaLexer.ENUM:
                        if (DestinationType.BaseType != typeof(Enum))
                        {
                            System.Diagnostics.Debugger.Break();
                        }
                        return Enum.Parse(DestinationType, token.Text.TrimStart('#'));
                    case ANTLR.kukaLexer.INT:
                        if (DestinationType != typeof(int)) throw new ArgumentException("DestinationType");
                        return int.Parse(token.Text);
                    case ANTLR.kukaLexer.FLOAT:
                        if (DestinationType != typeof(double)) throw new ArgumentException("DestinationType");
                        return double.Parse(token.Text);
                    case ANTLR.kukaLexer.BITArr:
                        if (DestinationType != typeof(int)) throw new ArgumentException("DestinationType");
                        break;
                    case ANTLR.kukaLexer.STRING:
                        if (DestinationType != typeof(string)) throw new ArgumentException("DestinationType");
                        return token.Text.Split('"');
                    default:
                        System.Diagnostics.Debugger.Break();
                        break;
                }
            }
            else
            {
                System.Diagnostics.Debugger.Break();
            }
            return null;
        }


        public void SetFromContext(ANTLR.kukaParser.ValueContext context)
        {
            if (context == null) return;
            if (context.ChildCount != 1) System.Diagnostics.Debugger.Break();
            var item = context.children[0];
            if (item is ANTLR.kukaParser.StructureContext)
            {
                ParseStructure((ANTLR.kukaParser.StructureContext)item, this);
            }
            else
            {
                System.Diagnostics.Debugger.Break();
            }
        }
    }
}
