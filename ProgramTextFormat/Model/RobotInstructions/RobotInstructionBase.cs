using CommunityToolkit.Mvvm.Messaging;
using ProgramTextFormat.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProgramTextFormat.Model.RobotInstructions
{
    [XmlRoot(ElementName = "RobotInstructionBase")]
    public abstract class RobotInstructionBase : ProgramTextFormat.Model.Rules.DataGridElementBase, ICloneable
    {
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get { return m_Name; } set { SetProperty(ref m_Name, value); SendEditValidMsg(); } }
        private string m_Name;

        [XmlAttribute(AttributeName = "IsComment")]
        public bool IsComment { get; set; }

        [XmlAttribute(AttributeName = "IsFold")]
        public bool IsFold { get; set; }

        [XmlIgnore]
        public abstract string CommentSign { get; }
        
        [XmlIgnore]
        public abstract bool UsesFold { get; }

        [XmlIgnore]
        public abstract string FoldStart { get; }

        [XmlIgnore]
        public abstract string FoldEnd { get; }

        [XmlIgnore]
        public abstract string RobotType { get; }

        [XmlAttribute(AttributeName = "KeyWordsString")]
        public string? KeyWordsString { get { return m_KeyWordsString; } set { SetProperty(ref m_KeyWordsString, value); SendEditValidMsg(); } }
        private string m_KeyWordsString;

        [XmlIgnore]
        public List<string> KeyWordList { get => KeyWordsString is null ? new List<string>() : KeyWordsString.Replace(" ", "").ToLower().Split(',').ToList(); }

        [XmlIgnore]
        public string SelectedNewRobotType 
        {
            get
            { if (this is KukaInstruction) return "KUKA"; return ""; }
            set 
            { } 
        }

        public abstract object Clone();

        public void FillBasicInfos(RobotInstructionBase input)
        {
            input.Name= this.Name;
            input.IsComment = this.IsComment;
            input.IsFold = this.IsFold;
            input.KeyWordsString = this.KeyWordsString;
            input.SelectedNewRobotType= this.SelectedNewRobotType;
        }
    }
}
