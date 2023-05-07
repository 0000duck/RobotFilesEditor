using CommunityToolkit.Mvvm.ComponentModel;
using ProgramTextFormat.Attributes;
using ProgramTextFormat.Model.RobotInstructions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace ProgramTextFormat.Model.Rules
{
    [XmlRoot(ElementName = "ProgramFormatRule")]
    public class ProgramFormatRule : DataGridElementBase, ICloneable
    {

        #region properties
        [XmlIgnore]
        public List<RobotInstructionBase> CombinedGroupWithInstruction { get; set; }

        [XmlAttribute(AttributeName = "Instruction")]
        public string Instruction { get { return m_Instruction; } set { SetProperty(ref m_Instruction, value); SendEditValidMsg(); } }
        private string m_Instruction;

        [XmlAttribute(AttributeName = "SelectedAction")]
        public string? SelectedAction { get; set; }

        [XmlAttribute(AttributeName = "GroupItems")]
        public bool GroupItems { get { return m_GroupItems; } set { SetProperty(ref m_GroupItems, value); } }
        private bool m_GroupItems;

        [XmlAttribute(AttributeName = "GroupWithOther")]
        public bool GroupWithOther { get { return m_GroupWithOther; } set { SetProperty(ref m_GroupWithOther, value); } }
        private bool m_GroupWithOther;

        [XmlAttribute(AttributeName = "GroupWithInstruction")]
        public string GroupWithInstruction { get { return m_GroupWithInstruction; } set { SetProperty(ref m_GroupWithInstruction, value); SendEditValidMsg(); } }
        private string m_GroupWithInstruction;

        [XmlAttribute(AttributeName = "GroupWithInstruction2")]
        public string GroupWithInstruction2 { get { return m_GroupWithInstruction2; } set { SetProperty(ref m_GroupWithInstruction2, value); SendEditValidMsg(); } }
        private string m_GroupWithInstruction2;

        [XmlAttribute(AttributeName = "GroupWithInstruction3")]
        public string GroupWithInstruction3 { get { return m_GroupWithInstruction3; } set { SetProperty(ref m_GroupWithInstruction3, value); SendEditValidMsg(); } }
        private string m_GroupWithInstruction3;

        [XmlAttribute(AttributeName = "GroupWithInstruction4")]
        public string GroupWithInstruction4 { get { return m_GroupWithInstruction4; } set { SetProperty(ref m_GroupWithInstruction4, value); SendEditValidMsg(); } }
        private string m_GroupWithInstruction4;

        [XmlAttribute(AttributeName = "GroupWithInstruction5")]
        public string GroupWithInstruction5 { get { return m_GroupWithInstruction5; } set { SetProperty(ref m_GroupWithInstruction5, value); SendEditValidMsg(); } }
        private string m_GroupWithInstruction5;
        [XmlIgnore]
        public RobotInstructionBase SelectedInstruction { get { return m_SelectedInstruction; } set { Instruction = value?.Name; SetProperty(ref m_SelectedInstruction, value); } }
        private RobotInstructionBase m_SelectedInstruction;

        [GroupItem]
        [XmlIgnore]
        public RobotInstructionBase SelectedInstructionToGroup { get { return m_SelectedInstructionToGroup; } set { GroupWithInstruction = value?.Name; SetProperty(ref m_SelectedInstructionToGroup, value); } }
        private RobotInstructionBase m_SelectedInstructionToGroup;

        [GroupItem]
        [XmlIgnore]
        public RobotInstructionBase SelectedInstructionToGroup2 { get { return m_SelectedInstructionToGroup2; } set { GroupWithInstruction2 = value?.Name; SetProperty(ref m_SelectedInstructionToGroup2, value); } }
        private RobotInstructionBase m_SelectedInstructionToGroup2;

        [GroupItem]
        [XmlIgnore]
        public RobotInstructionBase SelectedInstructionToGroup3 { get { return m_SelectedInstructionToGroup3; } set { GroupWithInstruction3 = value?.Name; SetProperty(ref m_SelectedInstructionToGroup3, value); } }
        private RobotInstructionBase m_SelectedInstructionToGroup3;

        [GroupItem]
        [XmlIgnore]
        public RobotInstructionBase SelectedInstructionToGroup4 { get { return m_SelectedInstructionToGroup4; } set { GroupWithInstruction4 = value?.Name; SetProperty(ref m_SelectedInstructionToGroup4, value); } }
        private RobotInstructionBase m_SelectedInstructionToGroup4;

        [GroupItem]
        [XmlIgnore]
        public RobotInstructionBase SelectedInstructionToGroup5 { get { return m_SelectedInstructionToGroup5; } set { GroupWithInstruction5 = value?.Name; SetProperty(ref m_SelectedInstructionToGroup5, value); } }
        private RobotInstructionBase m_SelectedInstructionToGroup5;

        #endregion properties

        #region constructor
        public ProgramFormatRule()
        {
            CombinedGroupWithInstruction = new List<RobotInstructionBase>();
        }

        public ProgramFormatRule(string? instruction, string? selectedAction, bool groupItems)
        {
            Instruction = instruction;
            SelectedAction = selectedAction;
            GroupItems = groupItems;
            CombinedGroupWithInstruction = new List<RobotInstructionBase>();
            //Editable = false;
        }


        #endregion constructor

        #region methods

        public object Clone()
        {
            var result = new ProgramFormatRule();
            result.Instruction = this.Instruction;
            result.SelectedAction = this.SelectedAction;
            result.GroupItems = this.GroupItems;
            result.SelectedInstruction = this.SelectedInstruction;
            result.SelectedInstructionToGroup = this.SelectedInstructionToGroup;
            result.SelectedInstructionToGroup2 = this.SelectedInstructionToGroup2;
            result.SelectedInstructionToGroup3 = this.SelectedInstructionToGroup3;
            result.SelectedInstructionToGroup4 = this.SelectedInstructionToGroup4;
            result.SelectedInstructionToGroup5 = this.SelectedInstructionToGroup5;
            result.GroupWithInstruction = this.GroupWithInstruction;
            result.GroupWithOther= this.GroupWithOther;
            return result;
        }
        #endregion methods
    }
}
