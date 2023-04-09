using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using ProgramTextFormat.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProgramTextFormat.Model.Rules
{
    public abstract class DataGridElementBase : ObservableObject
    {
        [XmlIgnore]
        public bool Editable { get { return m_Editable; } set { SetProperty(ref m_Editable, value); } }
        private bool m_Editable;

        public void SetEditability(bool enable)
        {
            Editable = enable;
        }
        public void SendEditValidMsg()
        {
            WeakReferenceMessenger.Default.Send<EditValidMessage>(new EditValidMessage(true));
        }
    }
}
