using CommunityToolkit.Mvvm.Messaging.Messages;
using ProgramTextFormat.Model.RobotInstructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramTextFormat.Messaging
{
    public class UpdateInstructionMessage : ValueChangedMessage<KeyValuePair<string, string>>
    {
        public UpdateInstructionMessage(KeyValuePair<string, string> value) : base(value)
        {
        }
    }
}
