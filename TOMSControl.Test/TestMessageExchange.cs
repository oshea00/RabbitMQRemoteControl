using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOMSControl.Domain;

namespace TOMSControl.Test
{
    class TestMessageExchange : IMessageExchange
    {
        public event Action<Message> OnPublish;

        private Dictionary<string, IList<Message>> _messages = new Dictionary<string, IList<Message>>(); 

        public void Publish(Message message)
        {
            if (!_messages.ContainsKey(message.RoutingKey))
            {
                _messages[message.RoutingKey] = new List<Message>();
            }
            _messages[message.RoutingKey].Add(message);
            if (OnPublish != null)
                OnPublish(message);
        }

        public Message NextMessage(string route)
        {
            var msg = _messages[route].FirstOrDefault();
            _messages[route].Remove(msg);
            return msg;
        }

        public Message PeekMessage(string route)
        {
            try
            {
                var msg = _messages[route].FirstOrDefault();
                return msg;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
