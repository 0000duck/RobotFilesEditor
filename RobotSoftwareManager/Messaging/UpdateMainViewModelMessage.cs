using CommonLibrary.DataClasses;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotSoftwareManager.Messaging
{
    public class UpdateMainViewModelMessage : ValueChangedMessage<bool>
    {
        public UpdateMainViewModelMessage(bool value) : base(value)
        {
        }
    }
}
