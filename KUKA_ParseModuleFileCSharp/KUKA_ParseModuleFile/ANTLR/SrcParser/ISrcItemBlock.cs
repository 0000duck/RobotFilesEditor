using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser
{
    public interface ISrcItemBlock : ISrcItem
    {
        int EndLine { get; set; }
        //void AddChild(ISrcParser item);
    }
}
