using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ParseModuleFile
{
    public class RegexHelper
    {
        private static Dictionary<string, Regex> regexes = new Dictionary<string, Regex>();

        public RegexHelper()
        {
        }

        public static Regex Get(string key)
        {
            Regex.CacheSize = 150;
            if (!regexes.ContainsKey(key))
                regexes.Add(key, new Regex(key, RegexOptions.IgnoreCase | RegexOptions.Compiled));
            return regexes[key];
        }

        public static Match Match(string pattern, string input)
        {
            return Regex.Match(input, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }

        public static bool IsMatch(string pattern, string input)
        {
            return Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }
    }
}
