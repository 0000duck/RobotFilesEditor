using WarningHelper;
using ParseModuleFile.KUKA.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ParseModuleFile.File
{
    public struct OLPData
    {
        public OLPType Name;
        public int Index;
        public string Value;
    }

    public class Olp : CFile
    {

        public List<OLPData> olp_data;
        public Olp(string fileName, Stream stream, Warnings warnings)
            : base(fileName, stream, warnings)
        {
        }

        protected override void ParseStream()
        {
            olp_data = new List<OLPData>();
            int i = 0;
            using (var reader = new StreamReader(stream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    OLPData var = ParseOLPLine(i, line, Path.GetFileNameWithoutExtension(base.fileName));
                    if (var.Name != OLPType.UNKNOWN)
                    {
                        olp_data.Add(var);
                        i += 1;
                    }
                }
            }
        }

        public OLPData ParseOLPLine(int line_number, string line, string module_name)
        {
            string trimmed_line = line.Trim();
            OLPData functionReturnValue = default(OLPData);
            functionReturnValue = new OLPData();
            functionReturnValue.Name = OLPType.UNKNOWN;
            current_line = line_number;
            if (trimmed_line.Length == 0)
                return functionReturnValue;
            if (RegexHelper.IsMatch(_comment, trimmed_line))
            {
                _warnings.Add(current_line, WarningType.Deep_Intern, Level.Information, "comment");
                return functionReturnValue;
            }
            Regex reBracket = default(Regex);
            if (trimmed_line.StartsWith("BASE_DATA["))
            {
                reBracket = new Regex("(?<name>BASE_DATA)\\[(?<item>\\d*)\\]=(?<value>.*)", RegexOptions.IgnoreCase);
                functionReturnValue.Name = OLPType.BASE_DATA;
            }
            else if (trimmed_line.StartsWith("BASE_NAME["))
            {
                reBracket = new Regex("(?<name>BASE_NAME)\\[(?<item>\\d*),\\]=(?<value>.*)", RegexOptions.IgnoreCase);
                functionReturnValue.Name = OLPType.BASE_NAME;
            }
            else if (trimmed_line.StartsWith("BASE_TYPE["))
            {
                reBracket = new Regex("(?<name>BASE_TYPE)\\[(?<item>\\d*)\\]=(?<value>.*)", RegexOptions.IgnoreCase);
                functionReturnValue.Name = OLPType.BASE_TYPE;
            }
            else if (trimmed_line.StartsWith("TOOL_DATA["))
            {
                reBracket = new Regex("(?<name>TOOL_DATA)\\[(?<item>\\d*)\\]=(?<value>.*)", RegexOptions.IgnoreCase);
                functionReturnValue.Name = OLPType.TOOL_DATA;
            }
            else if (trimmed_line.StartsWith("TOOL_TYPE["))
            {
                reBracket = new Regex("(?<name>TOOL_TYPE)\\[(?<item>\\d*)\\]=(?<value>.*)", RegexOptions.IgnoreCase);
                functionReturnValue.Name = OLPType.TOOL_TYPE;
            }
            else if (trimmed_line.StartsWith("TOOL_NAME["))
            {
                reBracket = new Regex("(?<name>TOOL_NAME)\\[(?<item>\\d*),\\]=(?<value>.*)", RegexOptions.IgnoreCase);
                functionReturnValue.Name = OLPType.TOOL_NAME;
            }
            else if (trimmed_line.StartsWith("LOAD_DATA["))
            {
                reBracket = new Regex("(?<name>LOAD_DATA)\\[(?<item>\\d*)\\]=(?<value>.*)", RegexOptions.IgnoreCase);
                functionReturnValue.Name = OLPType.LOAD_DATA;
            }
            else if (trimmed_line.StartsWith("DECL E6AXIS XHOME"))
            {
                reBracket = new Regex("DECL E6AXIS XHOME(?<item>\\d)=(?<value>.*)", RegexOptions.IgnoreCase);
                functionReturnValue.Name = OLPType.E6AXIS;
            }
            else
            {
                _warnings.Add(current_line, WarningType.Deep_Intern, Level.Failure, "Unknown data: " + trimmed_line);
                return functionReturnValue;
            }

            if (reBracket.IsMatch(trimmed_line))
            {
                Match match = reBracket.Match(trimmed_line);
                string _name = match.Groups["name"].Value;
                int _item = -1;
                if (!int.TryParse(match.Groups["item"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out _item))
                {
                    _warnings.Add(current_line, WarningType.Intern, Level.Failure, "Could not get integer from " + match.Groups["item"].Value);
                }
                functionReturnValue.Index = _item;
                if (functionReturnValue.Name == OLPType.E6AXIS)
                {
                    DynamicMemory mem = Dat.GetCorrectMemory("e6axis", _warnings);
                    Dat.AssignMemoryFromData(Dat.GetCorrectPattern("e6axis", _warnings), mem, match.Groups["value"].Value, _warnings);
                    functionReturnValue.Value = mem.ToString();
                }
                else
                {
                    functionReturnValue.Value = match.Groups["value"].Value;
                }
                _warnings.Add(current_line, WarningType.Deep_Intern, Level.Information, "Created variable " + _name);
            }
            else
            {
                throw new NotImplementedException();
            }
            return functionReturnValue;
        }

    }
}
