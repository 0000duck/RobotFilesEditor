using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile
{
    public class Check
    {
        public Check(string input)
        {
            string a = EncodeStringData(input);
            string key = "";
            string value = "";

            if (GetStructure(a, out key, out value))
            {
                Console.WriteLine(key + ":");
                Console.WriteLine(value);
                Console.WriteLine("===============================================");
                Console.ReadKey();
            }
        }

        private string EncodeStringData(string input)
        {
            string o = input;
            int i = 0;
            while (i < o.Length - 1)
            {
                string sub = o.Substring(i);
                int startIndex, length;
                string output;
                Console.WriteLine("Input:" + sub);
                if (StringData(sub, out output, out startIndex, out length))
                {
                    string partA = o.Substring(0, i + startIndex + 1);
                    string partB = o.Substring(i + startIndex + length + 1);
                    Console.WriteLine("A:" + partA);
                    Console.WriteLine("F:" + output);
                    Console.WriteLine("B:" + partB);
                    o = String.Concat(partA, output, partB);
                    i += startIndex + output.Length + 2;
                    Console.WriteLine("Next I=" + i.ToString());
                    Console.WriteLine("===============================================");
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("===============================================");
                    Console.WriteLine(o);
                    Console.WriteLine("===============================================");
                    Console.ReadKey();
                    break;
                }
            }
            return o;
        }

        private bool StringData(string input, out string value, out int StartIndex, out int Length)
        {
            value = null;
            StartIndex = input.IndexOf('"');
            Length = 0;
            if (StartIndex == -1) return false;
            int val2 = input.IndexOf('"', StartIndex + 1);
            if (val2 == -1)
            {
                Console.WriteLine("=============WRONG======================");
                Console.ReadKey();
                throw new NotImplementedException();
            }
            Length = val2 - StartIndex - 1;
            value = input.Substring(StartIndex + 1, Length);
            if (value.IndexOf('%')>=0) value = value.Replace("%", "%%");
            if (value.IndexOf(',') >= 0) value = value.Replace(",", "%C");
            if (value.IndexOf('{') >= 0) value = value.Replace("{", "%A");
            if (value.IndexOf('}') >= 0) value = value.Replace("}", "%B");
            return true;
        }

        private bool GetStructure(string input, out string key, out string value)
        {
            key = "";
            value = "";
            int index1 = input.IndexOf('{');
            if (index1 > -1)
            {
                int index2 = input.LastIndexOf('}');
                if (index2 > -1)
                {
                    if (index1 > 0) key = input.Substring(0, index1 - 1);
                    value = input.Substring(index1 + 1, index2 - index1 - 2);
                    return true;
                }
                else
                {
                    Console.WriteLine("something went wrong!");
                    throw new NotImplementedException();
                }
            }
            else return false;
        }
    }
}
