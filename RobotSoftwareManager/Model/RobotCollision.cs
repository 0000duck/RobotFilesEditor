using CommonLibrary.DataClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RobotSoftwareManager.Model
{
    public class RobotCollision : RobotSoftwareBase
    {
        public List<string> FoundNamesRel { get { return m_FoundNamesRel; } set { SetProperty(ref m_FoundNamesRel, value); } }
        private List<string> m_FoundNamesRel;
        
        public string SelectedNameRel { get { return m_SelectedNameRel; } set { SetProperty(ref m_SelectedNameRel, value); CurrentNameRel = value; } }
        private string m_SelectedNameRel;

        public string CurrentNameRel { get { return m_CurrentNameRel; } set { SetProperty(ref m_CurrentNameRel, value); ValidateRel(); } }
        private string m_CurrentNameRel;

        public RobotCollision(int number)
        {
            Number = number;
            FoundNames = new List<string>();
            FoundNamesRel = new List<string>();
        }

        internal void UpdateGui()
        {
            SelectedName = FoundNames.FirstOrDefault(x => x.Length > 0);
            SelectedNameRel = FoundNamesRel.FirstOrDefault(x => x.Length > 0);
            CurrentName = SelectedName;
            CurrentNameRel = SelectedNameRel;
        }

        public override void Validate()
        {
            if (CurrentName.Length > 40)
            {
                CheckState = RobotSoftCheckState.Error;
                Message = "Collision description longer than 40 chars.";
                return;
            }
            CheckState = RobotSoftCheckState.OK;
            Message = "OK";
            return;
        }

        private void ValidateRel()
        {
            if (CurrentNameRel.Length > 40)
            {
                CheckState = RobotSoftCheckState.Error;
                Message = "Collision description longer than 40 chars.";
                return;
            }
            CheckState = RobotSoftCheckState.OK;
            Message = "OK";
            return;
        }
    }
}
