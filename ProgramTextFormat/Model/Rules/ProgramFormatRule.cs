﻿using CommunityToolkit.Mvvm.ComponentModel;
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
        [XmlAttribute(AttributeName = "Number")]
        public string Number { get { return m_Number; } set { SetProperty(ref m_Number, value); SendEditValidMsg(); } }
        private string m_Number;

        [XmlAttribute(AttributeName = "Instruction")]
        public string Instruction { get { return m_Instruction; } set { SetProperty(ref m_Instruction, value); SendEditValidMsg(); } }
        private string m_Instruction;

        [XmlAttribute(AttributeName = "SelectedAction")]
        public string? SelectedAction { get; set; }

        [XmlAttribute(AttributeName = "GroupItems")]
        public bool GroupItems { get { return m_GroupItems; } set { SetProperty(ref m_GroupItems, value); } }
        private bool m_GroupItems;

        [XmlIgnore]
        public RobotInstructionBase SelectedInstruction { get { return m_SelectedInstruction; } set { Instruction = value?.Name; SetProperty(ref m_SelectedInstruction, value); } }
        private RobotInstructionBase m_SelectedInstruction;

        #endregion properties

        #region constructor
        public ProgramFormatRule()
        {
            //Editable = false;
        }

        public ProgramFormatRule(string? number, string? instruction, string? selectedAction, bool groupItems)
        {
            Number = number;
            Instruction = instruction;
            SelectedAction = selectedAction;
            GroupItems = groupItems;
            //Editable = false;
        }


        #endregion constructor

        #region methods

        public object Clone()
        {
            var result = new ProgramFormatRule();
            result.Number = this.Number;
            result.Instruction = this.Instruction;
            result.SelectedAction = this.SelectedAction;
            result.GroupItems = this.GroupItems;
            result.SelectedInstruction = this.SelectedInstruction;
            return result;
        }
        #endregion methods
    }
}
