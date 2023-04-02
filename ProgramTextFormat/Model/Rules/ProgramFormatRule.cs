using CommunityToolkit.Mvvm.ComponentModel;
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
    public class ProgramFormatRule : ObservableObject, ICloneable
    {

        #region properties
        [XmlAttribute(AttributeName = "Number")]
        public string? Number { get; set; }

        [XmlAttribute(AttributeName = "Instruction")]
        public string? Instruction { get; set; }

        [XmlAttribute(AttributeName = "SelectedAction")]
        public string? SelectedAction { get; set; }

        [XmlAttribute(AttributeName = "GroupItems")]
        public bool GroupItems { get; set; }

        [XmlIgnore]
        public bool Editable { get { return m_Editable; } set { SetProperty(ref m_Editable, value); } }
        private bool m_Editable;

        #endregion properties

        #region constructor
        public ProgramFormatRule()
        {
            Editable = false;
        }

        public ProgramFormatRule(string? number, string? instruction, string? selectedAction, bool groupItems)
        {
            Number = number;
            Instruction = instruction;
            SelectedAction = selectedAction;
            GroupItems = groupItems;
            Editable = false;
        }


        #endregion constructor

        #region methods
        public void SetEditability(bool enable)
        {
            Editable = enable;
        }

        public object Clone()
        {
            var result = new ProgramFormatRule();
            result.Number = this.Number;
            result.Instruction = this.Instruction;
            result.SelectedAction = this.SelectedAction;
            result.GroupItems = this.GroupItems;
            return result;
        }
        #endregion methods
    }
}
