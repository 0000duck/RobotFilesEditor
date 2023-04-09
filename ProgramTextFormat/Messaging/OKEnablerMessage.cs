using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramTextFormat.Messaging
{
    public class OKEnablerMessage : ValueChangedMessage<bool>
    {
        public OKEnablerMessage(bool value) : base(value)
        {
        }
    }
}
