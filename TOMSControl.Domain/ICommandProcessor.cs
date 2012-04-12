using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOMSControl.Domain
{
    public interface ICommandProcessor
    {
        event Action<CommandMessage> OnMessageReceived;
        string GetListeningRoute();
        string GetResultRoute();
        string GetLogRoute();
    }
}
