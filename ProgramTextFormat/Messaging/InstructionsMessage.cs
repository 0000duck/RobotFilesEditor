using CommunityToolkit.Mvvm.Messaging.Messages;
using ProgramTextFormat.Model.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramTextFormat.Messaging
{
    public class InstructionsMessage : ValueChangedMessage<Instructions>
    {
        public InstructionsMessage(Instructions value) : base(value)
        {
        }
    }
}
