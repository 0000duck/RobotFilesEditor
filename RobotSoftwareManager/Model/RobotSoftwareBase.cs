using CommonLibrary.DataClasses;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using RobotSoftwareManager.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RobotSoftwareManager.Model
{
    public abstract class RobotSoftwareBase : ObservableObject
    {

        public int Number { get { return m_Number; } set { SetProperty(ref m_Number, value); } }
        private int m_Number;


        public List<string> FoundNames { get { return m_FoundNames; } set { SetProperty(ref m_FoundNames, value); } }
        private List<string> m_FoundNames;

        public string SelectedName { get { return m_SelectedName; } set { SetProperty(ref m_SelectedName, value); CurrentName = value; } }
        private string m_SelectedName;


        public RobotSoftCheckState CheckState { get { return m_CheckState; } set { SetProperty(ref m_CheckState, value); } }
        private RobotSoftCheckState m_CheckState;


        public string Message { get { return m_Message; } set { SetProperty(ref m_Message, value); } }
        private string m_Message;

        public string CurrentName { get { return m_CurrentName; } set { SetProperty(ref m_CurrentName, value); CurrentNameUpdated(); } }
        private string m_CurrentName;

        public void UpdateSelectedValue(ObservableCollection<int> collection)
        {
            int oldNum = Number;
            Number = collection.FirstOrDefault(x => x == oldNum);
        }

        public abstract void Validate();

        public void CurrentNameUpdated()
        {
            Validate();
            WeakReferenceMessenger.Default.Send<UpdateMainViewModelMessage>(new UpdateMainViewModelMessage(true));
        }
    }
}
