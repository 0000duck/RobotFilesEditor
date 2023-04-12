using CommunityToolkit.Mvvm.Messaging.Messages;
using ProgramTextFormat.Model.RobotInstructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramTextFormat.Messaging
{
    public class RemoveInstructionMessage : ValueChangedMessage<RobotInstructionBase>
    {
        public RemoveInstructionMessage(RobotInstructionBase value) : base(value)
        {
        }
    }
}
