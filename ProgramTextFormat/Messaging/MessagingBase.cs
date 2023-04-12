using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramTextFormat.Messaging
{
    public abstract class MessagingBase : ObservableRecipient, IRecipient<RulesLoaded>, IRecipient<UpdateInstructionMessage>, IRecipient<OKEnablerMessage>, IRecipient<AddInstructionMessage>, IRecipient<AddRuleMessage>, IRecipient<RemoveInstructionMessage>, IRecipient<RemoveRuleMessage>
    {
        public abstract void Receive(RulesLoaded message);
        public abstract void Receive(UpdateInstructionMessage message);
        public abstract void Receive(OKEnablerMessage message);
        public abstract void Receive(AddInstructionMessage message);
        public abstract void Receive(AddRuleMessage message);
        public abstract void Receive(RemoveInstructionMessage message);
        public abstract void Receive(RemoveRuleMessage message);
    }
}
