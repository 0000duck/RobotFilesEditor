using CommonLibrary.DataClasses;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Messaging
{
    public class AddLogMessage : ValueChangedMessage<LogResult>
    {
        public AddLogMessage(LogResult value) : base(value)
        {
        }
    }
}
