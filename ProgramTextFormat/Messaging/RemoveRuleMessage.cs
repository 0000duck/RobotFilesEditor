using CommunityToolkit.Mvvm.Messaging.Messages;
using ProgramTextFormat.Model.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramTextFormat.Messaging
{
    public class RemoveRuleMessage : ValueChangedMessage<ProgramFormatRule>
    {
        public RemoveRuleMessage(ProgramFormatRule value) : base(value)
        {
        }
    }
}
