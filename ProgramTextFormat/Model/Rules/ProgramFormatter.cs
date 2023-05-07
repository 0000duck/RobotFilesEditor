using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProgramTextFormat.Model.Serialization.SerializationHelper;
using System.Xml.Serialization;
using System.Data;
using System.Reflection;
using ProgramTextFormat.Model.RobotInstructions;

namespace ProgramTextFormat.Model.Rules
{
    [XmlRoot(ElementName = "ProgramFormatter")]
    public class ProgramFormatter
    {
        [XmlElement(ElementName = "Rules")]
        public Rules Rules { get; set; }
        [XmlElement(ElementName = "Instructions")]
        public Instructions Instructions { get; set; }

        public void Initialize()
        {
            foreach (var rule in Rules.ProgramFormatRule)
            {
                if (Instructions.KukaInstructions.Any(x => x.Name.Equals(rule.Instruction, StringComparison.OrdinalIgnoreCase)))
                {
                    rule.SelectedInstruction = Instructions.KukaInstructions.FirstOrDefault(x => x.Name.Equals(rule.Instruction));
                    rule.SelectedInstructionToGroup = Instructions.KukaInstructions.FirstOrDefault(x => x.Name.Equals(rule?.GroupWithInstruction));
                    rule.SelectedInstructionToGroup2 = Instructions.KukaInstructions.FirstOrDefault(x => x.Name.Equals(rule?.GroupWithInstruction2));
                    rule.SelectedInstructionToGroup3 = Instructions.KukaInstructions.FirstOrDefault(x => x.Name.Equals(rule?.GroupWithInstruction3));
                    rule.SelectedInstructionToGroup4 = Instructions.KukaInstructions.FirstOrDefault(x => x.Name.Equals(rule?.GroupWithInstruction4));
                    rule.SelectedInstructionToGroup5 = Instructions.KukaInstructions.FirstOrDefault(x => x.Name.Equals(rule?.GroupWithInstruction5));
                }
            }
            foreach (var rule in Rules.ProgramFormatRule)
            {
                var props = rule.GetType().GetProperties().Where(x => x.CustomAttributes.Any(y => y.AttributeType.Name == "GroupItemAttribute"));

                foreach (PropertyInfo prop in props)
                {
                    RobotInstructionBase val = prop.GetValue(rule) as RobotInstructionBase;
                    if (val != null)
                    { 
                        if (!rule.CombinedGroupWithInstruction.Any(x => x.Name.Equals(val.Name)))
                        {
                            rule.CombinedGroupWithInstruction.Add(val);
                        }
                        var otherRule = Rules.ProgramFormatRule.FirstOrDefault(x => x.Instruction == val.Name);
                        if (otherRule != null && !otherRule.CombinedGroupWithInstruction.Any(x => x.Name == val.Name)) 
                        { 
                            otherRule.CombinedGroupWithInstruction.Add(rule.SelectedInstruction);
                        }
                    } 
                }              
            }
        }
    }
}
