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

                List<SpaceItem> groups = DivideToGroups(file.Value);

                foreach (var command in groups)
                {
                    if (command.Rule != null)
                    {
                        switch (command.Rule.SelectedAction)
                        {
                            case "EnterBeforeAfter":
                            case "EnterBefore":
                                string addAfter = command.Rule.SelectedAction.Equals("EnterBeforeAfter") ? "\r\n" : "";
                                tempresult[file.Key].Add("\r\n" + command.Content + addAfter);
                                break;
                            case "EnterAfter":
                                tempresult[file.Key].Add(command.Content + "\r\n");
                                break;
                            case "RemoveCommand":
                                break;
                        }
                    }
                    else
                        tempresult[file.Key].Add(command.Content);
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

        private static List<SpaceItem> DivideToGroups(List<string> commands)
        {
            List<SpaceItem> result = new List<SpaceItem>();
            RobotInstructionBase previousInstruction = null;
            string currentGroup = string.Empty;
            ProgramFormatRule groupRule = null;
            foreach (var command in commands)
            {
                RobotInstructionBase instruction = TryMatchInstruction(command);
                if (instruction != null)
                {
                    var currentRule = rules.Rules.ProgramFormatRule.FirstOrDefault(x => x.SelectedInstruction == instruction);
                    if (currentRule != null)
                    {
                        if (groupRule != null && currentRule != groupRule && !groupRule.CombinedGroupWithInstruction.Any(x => x.Name.Equals(instruction.Name)))
                        {
                            if (!string.IsNullOrEmpty(currentGroup))
                            {
                                result.Add(new SpaceItem(currentGroup, groupRule));
                                currentGroup = string.Empty;
                                groupRule = null;
                            }
                        }
                        if (currentRule.GroupItems && instruction.Name == previousInstruction?.Name || previousInstruction == null && string.IsNullOrEmpty(currentGroup))
                        {
                            currentGroup += command;
                        }
                        foreach (var instr in currentRule.CombinedGroupWithInstruction)
                        {
                            if (currentRule.GroupWithOther && !(instr is EmptyInstuction) && (previousInstruction?.Name == instr.Name))
                            {
                                currentGroup += command;
                            }
                        }
                        groupRule = currentRule;

                    }
                    else
                    {
                        if (groupRule != null && !groupRule.CombinedGroupWithInstruction.Any(x => x.Name.Equals(instruction.Name)))
                        {
                            result.Add(new SpaceItem(currentGroup, groupRule));
                            currentGroup = string.Empty;
                            groupRule = null;
                        }
                        currentGroup += command;
                    }
                    previousInstruction = instruction;
                }
                else
                {
                    if (!string.IsNullOrEmpty(currentGroup))
                    {
                        result.Add(new SpaceItem(currentGroup, groupRule));
                        currentGroup = string.Empty;
                        groupRule = null;
                    }
                    result.Add(new SpaceItem(command,null));
                }
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

        private class SpaceItem
        {
            public string Content { get; set; }
            public ProgramFormatRule Rule { get; set; }

            public SpaceItem(string content, ProgramFormatRule rule)
            {
                Content = content;
                Rule = rule;
            }
        }
    }
}
