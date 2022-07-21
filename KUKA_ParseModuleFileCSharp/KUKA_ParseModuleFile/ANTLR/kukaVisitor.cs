using Antlr4.Runtime;
using ParseModuleFile.KUKA;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;
using ParseModuleFile.KUKA.DataTypes;
using Antlr4.Runtime.Tree;

namespace ParseModuleFile.ANTLR
{
    public class kukaVisitor : kukaBaseVisitor<int>
    {
        #region fields
        private Robot m_Robot;
        private Variables m_dataList = new Variables();

        private List<int?> arrayItems;
        private CustomStruct currentStruct;
        private string module;
        private List<string> idList;
        #endregion fields

        #region properties
        public Variables DataList { get { return m_dataList; } }
        public string Module { get { return module; } }
        #endregion properties

        public int Visit(IParseTree tree, Robot robot)
        {
            m_Robot = robot;
            return Visit(tree);
        }

        public override int VisitDefdat(kukaParser.DefdatContext context)
        {
            foreach (var item in context.children)
            {
                if (item.Payload is CommonToken)
                {
                    CommonToken token = (CommonToken)item.Payload;
                    if (token.Type == ANTLR.kukaLexer.ID)
                    {
                        module = token.Text.ToUpperInvariant();
                        if (module == "$CONFIG") m_dataList.IsPublic = true;
                    }
                    else if (token.Type == ANTLR.kukaLexer.PUBLIC)
                    {
                        m_dataList.IsPublic = true;
                    }
                }
            }
            return base.VisitDefdat(context);
        }

        public override int VisitMyenum([NotNull] kukaParser.MyenumContext context)
        {
            bool global = m_dataList.IsPublic;
            if (context.GLOBAL() != null) { global = true; }
            string name = context.id().GetText();
            idList = new List<string>();
            VisitIdList(context.idList());
            m_Robot.DataTypes.Enums.Add(name, new CustomEnum(name,idList,global));
            return 1;
        }

        public override int VisitMystruc([NotNull] kukaParser.MystrucContext context)
        {
            bool global = m_dataList.IsPublic;
            if (context.GLOBAL() != null) { global = true; }
            string name = context.id().GetText();
            currentStruct = new CustomStruct(name, null, global);
            VisitSparamList(context.sparamList());
            m_Robot.DataTypes.Structures.Add(currentStruct.Name,currentStruct);
            return 1;
        }

        public override int VisitSparamList([NotNull] kukaParser.SparamListContext context)
        {
            foreach (kukaParser.ParamsContext param in context.@params())
                VisitParams(param);
            return 1;
        }

        public override int VisitParams([NotNull] kukaParser.ParamsContext context)
        {
            string type = null;
            string name = null;
            arrayItems = null;
            foreach (var child in context.children)
            {
                if (child is kukaParser.IdContext && type == null)
                    type = child.GetText();
                else if (child is kukaParser.IdContext)
                    name = child.GetText();
                else if (child is kukaParser.ArrayContext)
                {
                    VisitArray((kukaParser.ArrayContext)child).ToString();
                }
                else if (child.Payload is CommonToken && ((CommonToken)child.Payload).Type == ANTLR.kukaLexer.COMMA)
                {
                    currentStruct.Elements.Add(new CustomStructElement(type, name, arrayItems));
                    arrayItems = null; 
                }
            }
            currentStruct.Elements.Add(new CustomStructElement(type, name, arrayItems));
            return 1;
        }

        public override int VisitArray([NotNull] kukaParser.ArrayContext context)
        {
            arrayItems = new List<int?>();
            int? curr = null;
            foreach (var child in context.children)
            {
                if (child.Payload is CommonToken && ((CommonToken)child.Payload).Type == ANTLR.kukaLexer.INT)
                    curr = int.Parse(child.GetText());
                else if (child.Payload is CommonToken && ((CommonToken)child.Payload).Type == ANTLR.kukaLexer.COMMA)
                {
                    arrayItems.Add(curr);
                    curr = null;
                }
            }
            arrayItems.Add(curr);
            return 1;
        }

        public override int VisitData(kukaParser.DataContext context)
        {
            if (context.ChildCount != 1)
                return -1;
            var child = context.children[0];
            if (context.ChildCount == 1)
            {
                if (child is kukaParser.DeclContext)
                {
                    var ret = ParseDecl((kukaParser.DeclContext)child);
                    m_dataList.Add(ret.Key,ret.Value);
                }
                else if (child is kukaParser.XconstContext)
                {
                    var ret = ParseConst((kukaParser.XconstContext)child);
                    m_dataList.Add(ret.Key, ret.Value);
                }
                else if (child is kukaParser.NoDeclContext)
                {
                    var ret = ParseNoDecl((kukaParser.NoDeclContext)child);
                    foreach (var item in ret)
                        m_dataList.Add(item.Key, item.Value);
                }
                else if (child is kukaParser.AssignContext)
                {
                    string name;
                    List<int> isArray;
                    var ret = ParseAssign((kukaParser.AssignContext)child, out name, out isArray, false);
                    if (!m_dataList.ContainsKey(name)) System.Diagnostics.Debugger.Break();
                    //...array
                    if (isArray.Count == 0) m_dataList[name] = MemorySelector.XGet(m_dataList[name].DataTypeName, ParseValue(ret), m_Robot);
                    else if (isArray.Count == 1) ((DataArray)m_dataList[name])[isArray[0]] = MemorySelector.XGet(m_dataList[name].DataTypeName, ParseValue(ret), m_Robot);
                }
                else
                {
                    System.Diagnostics.Debugger.Break();
                }
            }
            else
                System.Diagnostics.Debugger.Break();
            return base.VisitData(context);
        }
        //private void ParseSrtucture(kukaParser.StructureContext context)
        //{
        //    string curID=null;
        //    bool isArray = false;
        //    kukaParser.ValueContext curVal;
        //    foreach (var child in context.children)
        //    {
        //        if (child.Payload is CommonToken)
        //        {
        //            CommonToken token = (CommonToken)child.Payload;
        //            if (token.Type == kukaLexer.ID)
        //            {
        //                curID = token.Text;
        //                isArray = false;
        //            }
        //        }
        //        else if (child.Payload is kukaParser.ValueContext)
        //        {
        //            ParseValue((kukaParser.ValueContext)child);
        //        }
        //        else if (child.Payload is kukaParser.ArrayContext)
        //        {
        //            isArray = true;
        //        }
        //        else
        //        {
        //            System.Diagnostics.Debugger.Break();
        //        }

        //    }
        //}

        //private object ParseValue(kukaParser.ValueContext context)
        //{
        //    if (context.ChildCount != 1) System.Diagnostics.Debugger.Break();
        //    var child = context.children[0];
        //    if (child is kukaParser.StructureContext)
        //        ParseSrtucture((kukaParser.StructureContext)child);
        //    else if (child.Payload is CommonToken)
        //    {
        //        CommonToken token = (CommonToken) child.Payload;
        //        switch (token.Type)
        //        {
        //            case kukaLexer.BOOL:
        //                return token.Text;
        //            case kukaLexer.ENUM:
        //                return token.Text;
        //            case kukaLexer.INT:
        //                break;
        //            case kukaLexer.FLOAT:
        //                break;
        //            case kukaLexer.BYTE:
        //                break;
        //            case kukaLexer.STRING:
        //                break;
        //            default:
        //                System.Diagnostics.Debugger.Break();
        //                break;
        //        }
        //    }
        //    else
        //    {
        //        System.Diagnostics.Debugger.Break();
        //    }
        //    return null;
        //}*/

        private List<int> ParseArray(kukaParser.ArrayContext context, bool isCharDef)
        {
            List<int> items = new List<int>();
            if (context == null) return items;
            foreach (var child in context.children)
            {
                if (child.Payload is CommonToken)
                {
                    CommonToken token = (CommonToken)child.Payload;
                    if (token.Type == ANTLR.kukaLexer.INT)
                        items.Add(int.Parse(token.Text, CultureInfo.InvariantCulture));
                }
                //System.Diagnostics.Debugger.Break();
            }
            if (isCharDef) items.RemoveAt(items.Count - 1);
            return items;
        }

        private kukaParser.ValueContext ParseAssign(kukaParser.AssignContext context, out string Name, out List<int> Array, bool isCharDef)
        {
            Name = context.id().GetText();
            //Name = context.ID().Symbol.Text;
            Array = ParseArray(context.array(),isCharDef);
            return context.value();
        }

        public override int VisitIdList([NotNull] kukaParser.IdListContext context)
        {
            foreach (kukaParser.IdContext item in context.id())
            {
                idList.Add(item.GetText());
            }
            return 1;
        }

        private static DataItems ParseStructure(kukaParser.StructureContext context)
        {
            DataItems items = new DataItems();
            string curID = null;
            //bool isArray = false;
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
                else if (child.Payload is kukaParser.ValueContext)
                {
                    if (curID != null)
                    {
                        DataItems val = ParseValue((kukaParser.ValueContext)child);
                        items.Add(curID, val);
                    }
                }
                else if (child.Payload is kukaParser.ArrayContext)
                {
                    //isArray = true;
                }
                else if (child.Payload is kukaParser.IdContext)
                {
                    curID = child.GetText();
                }
                else
                {
                    System.Diagnostics.Debugger.Break();
                    System.Diagnostics.Debugger.Break();
                }
            }
            return items;
        }
        private static DataItems ParseValue(kukaParser.ValueContext context)
        {
            if (context == null) return null;
            if (context.ChildCount != 1) System.Diagnostics.Debugger.Break();
            var child = context.children[0];
            if (child is ANTLR.kukaParser.StructureContext)
                return ParseStructure((ANTLR.kukaParser.StructureContext)child);
            else if (child.Payload is CommonToken)
            {
                CommonToken token = (CommonToken)child.Payload;
                if (token.Type == ANTLR.kukaLexer.BITArr)
                {
                    string barr = token.Text.Trim('\'');
                    int j = 1;
                    int output = 0;
                    for (int i = barr.Length - 1; i > 0; i--)
                    {
                        if (barr[i] == '1') output += j;
                        j*=2;
                    }
                    return new DataItems(output.ToString());
                    //System.Diagnostics.Debugger.Break();
                }
                return new DataItems(token.Text);
            }
            else
            {
                System.Diagnostics.Debugger.Break();
            }
            return null;
        } 

        private KeyValuePair<string, KUKA.Variable> ParseDecl(kukaParser.DeclContext context)
        {
            if (context.ChildCount != 3 && context.ChildCount != 4)
                throw new ArgumentException("context");

            bool global = context.GLOBAL().Length > 0;
            string Type = context.id().GetText();
            //string Type = context.ID().Symbol.Text;
            string Name;
            List<int> Array;
            kukaParser.ValueContext Value = ParseAssign(context.assign(), out Name, out Array, (Type=="CHAR"));
            //TODO: MemorySelector with basic types (enum,string,int,real,bool)
            KUKA.Variable item;
            if  (Array.Count==0) item = MemorySelector.XGet(Type, ParseValue(Value), m_Robot);
            else if (Array.Count == 1) item = new DataArray(Array[0], Type, m_Robot);
            else if (Array.Count == 2)
            {
                item = new KUKA.DataArray(Array[0], Type, m_Robot);
                for (int i = 1; i <= Array[0]; i++)
                {
                    ((DataArray)item)[i] = new DataArray(Array[1], Type, m_Robot);
                }
            }
            else throw new NotImplementedException("Array dim > 2");

            if (item != null)item.IsGlobal = global;
            return new KeyValuePair<string, KUKA.Variable>(Name, item);
        }
        private KeyValuePair<string, KUKA.Variable> ParseConst(kukaParser.XconstContext context)
        {
            if (context.ChildCount != 3 && context.ChildCount != 4)
                throw new ArgumentException("context");

            bool global = context.GLOBAL() != null;
            string Type = context.id().GetText();
            //string Type = context.id().Symbol.Text;
            string Name;
            List<int> Array;
            kukaParser.ValueContext Value = ParseAssign(context.assign(), out Name, out Array, (Type == "CHAR"));
            //TODO: MemorySelector with basic types (enum,string,int,real,bool)
            KUKA.Variable item;
            if (Array.Count == 0) item = KUKA.DataTypes.MemorySelector.XGet(Type, ParseValue(Value), m_Robot);
            else if (Array.Count == 1) item = new KUKA.DataArray(Array[0], Type, m_Robot);
            else if (Array.Count == 2)
            {
                item = new DataArray(Array[0], Type, m_Robot);
                for (int i = 1; i <= Array[0]; i++)
                {
                    ((DataArray)item)[i] = new DataArray(Array[1], Type, m_Robot);
                }
            }
            else throw new NotImplementedException("Array dim > 2");

            if (item != null)
            {
                item.IsGlobal = global;
                item.IsConst = true;
            }
            return new KeyValuePair<string, KUKA.Variable>(Name, item);
        }
        private Dictionary<string, KUKA.Variable> ParseNoDecl(kukaParser.NoDeclContext context)
        {
            Dictionary<string, KUKA.Variable> items = new Dictionary<string, KUKA.Variable>();
            bool global = context.GLOBAL() != null;
            string Type = context.id().GetText();
            //string Type = context.ID().Symbol.Text;
            string Name="";
            List<int> Array;
            foreach (var ctx in context.assign())
            {
                kukaParser.ValueContext Value = ParseAssign(ctx, out Name, out Array, false);
                KUKA.Variable item;
                if (Array.Count == 0) item = MemorySelector.XGet(Type, ParseValue(Value), m_Robot);
                else if (Array.Count == 1) item = new DataArray(Array[0], Type, m_Robot);
                else if (Array.Count == 2)
                {
                    item = new DataArray(Array[0], Type, m_Robot);
                    for (int i = 1; i <= Array[0]; i++)
                    {
                        ((DataArray)item)[i] = new DataArray(Array[1], Type, m_Robot);
                    }
                }
                else throw new NotImplementedException("Array dim > 2");
                //DataType item = KUKA.DataTypes.MemorySelector.Get(Type, ParseValue(Value));
                //TODO: MemorySelector with basic types (enum,string,int,real,bool)
                if (item != null) item.IsGlobal = global;
                items.Add(Name, item);
            }
            return items;
        }
    }
}
