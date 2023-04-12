using ProgramTextFormat.Model.RobotInstructions;
using ProgramTextFormat.Model.Rules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.TextFormatting;
using System.Xml.Serialization;

namespace RobotFilesEditor.Model.Operations
{
    public static class KukaAddSpaces_v2
    {
        static ProgramFormatter rules;
        internal static IDictionary<string, string> AddSpaces(IDictionary<string, List<string>> inputSrcs)
        {
            ReadRules();
            IDictionary<string, string> result = new Dictionary<string, string>();
            IDictionary<string, List<string>> tempresult = new Dictionary<string, List<string>>();

            foreach (var file in inputSrcs)
            {
                tempresult.Add(file.Key, new List<string>());
                RobotInstructionBase previousInstruction = null;
                foreach (var command in file.Value)
                {
                    RobotInstructionBase instruction = TryMatchInstruction(command);
                    if (instruction != null)
                    {
                        var currentRule = rules.Rules.ProgramFormatRule.FirstOrDefault(x => x.SelectedInstruction == instruction);
                        if (currentRule != null)
                        {
                            switch (currentRule.SelectedAction)
                            {
                                case "EnterBeforeAfter":
                                case "EnterBefore":
                                    string addAfter = currentRule.SelectedAction.Equals("EnterBeforeAfter") ? "\r\n" : "";
                                    if (currentRule.GroupWithOther && previousInstruction != null && previousInstruction.Name.Equals(currentRule.GroupWithInstruction, StringComparison.OrdinalIgnoreCase))
                                    {
                                        tempresult[file.Key][tempresult[file.Key].Count - 1] = "\r\n" + tempresult[file.Key][tempresult[file.Key].Count - 1];
                                        tempresult[file.Key].Add(command + addAfter);
                                    }
                                    else
                                        tempresult[file.Key].Add("\r\n" + command + addAfter);
                                    break;
                                case "EnterAfter":
                                    tempresult[file.Key].Add(command + "\r\n");
                                    break;
                                case "RemoveCommand":
                                    break;
                            }
                        }
                        else
                            tempresult[file.Key].Add(command);
                    }
                    else
                        tempresult[file.Key].Add(command);
                    previousInstruction = instruction;
                }
            }
            foreach (var file in tempresult)
            {
                result.Add(file.Key, string.Empty);
                foreach (var command in tempresult[file.Key])
                    result[file.Key] += command;
                while (result[file.Key].Contains("\r\n\r\n\r\n"))
                    result[file.Key] = result[file.Key].Replace("\r\n\r\n\r\n", "\r\n\r\n");
            }
           
            return result;
        }

        #region private methods
        private static void ReadRules()
        {
            var dir = Path.GetDirectoryName(CommonLibrary.CommonMethods.GetApplicationConfig());
            var result = Path.Combine(dir, "ProgramFormatter.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(ProgramFormatter));

            using (Stream reader = new FileStream(result, FileMode.Open))
                rules = (ProgramFormatter)serializer.Deserialize(reader);
            rules.Initialize();
        }

        private static RobotInstructionBase TryMatchInstruction(string command)
        {
            RobotInstructionBase currentInstruction = null;
            foreach (var instruction in rules.Instructions.KukaInstructions)
            {
                if (instruction.IsComment && !command.Trim().StartsWith(instruction.CommentSign))
                    continue;
                if (instruction.IsFold && !command.Trim().ToLower().StartsWith(instruction.FoldStart))
                    continue;
                bool found = true;
                foreach (var keyword in instruction.KeyWordList)
                {
                    if (!command.ToLower().Contains(keyword.ToLower()))
                    {
                        found = false;
                        break;
                    }
                }
                if (!found)
                    continue;
                currentInstruction = instruction;
                break;
            }
            return currentInstruction;
        }
        #endregion private methods
    }
}
