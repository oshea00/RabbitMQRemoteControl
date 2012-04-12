using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOMSControl.Domain
{
    public interface IMessageExchange
    {
        event Action<Message> OnPublish;
        void Publish(Message message);
        Message NextMessage(string route);
        Message PeekMessage(string route);
    }
}
