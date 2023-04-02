using ProgramTextFormat.Model.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramTextFormat.Model.Helpers
{
    public static class Statics
    {
        public static int GetHighest(IEnumerable<ProgramFormatRule> collection)
        {
            int result = 0;
            foreach (var item in collection)
            {
                if (int.TryParse(item.Number,out int num))
                    if (num > result)
                        result = num;
            }
            return result;
        }
    }
}
