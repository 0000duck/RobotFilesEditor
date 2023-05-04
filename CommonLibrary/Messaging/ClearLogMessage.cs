using CommonLibrary.DataClasses;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Messaging
{
    public class ClearLogMessage : ValueChangedMessage<bool>
    {
        public ClearLogMessage(bool value) : base(value)
        {
        }
    }
}
