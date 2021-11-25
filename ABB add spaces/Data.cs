using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABB_add_spaces
{
    class Data
    {
        public int Line { get; set; }
        public string Content { get; set; }
        public string File { get; set; }
        public Data(int line, string content, string file)
        {
            Line = line;
            Content = content;
            File = file;
        }
    }
}
