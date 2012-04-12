using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;
using RabbitMQ.Client.Events;

namespace TOMSControl.Domain
{
    public interface IMessageConsumer
    {
        event Action<Message> OnMessageReceived;
        void ListenToQueue(string route);
    }

    public class MessageConsumer : IMessageConsumer
    {
        DataContractJsonSerializer _serializer;
        string _host;
        string _username;
        string _password;
        public event Action<Message> OnMessageReceived;

        public MessageConsumer(string host, string username, string password)
        {
            _host = host;
            _username = username;
            _password = password;
            _serializer = new DataContractJsonSerializer(typeof(Message));
        }

        public void ListenToQueue(string route)
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = _host,
                UserName = _username,
                Password = _password,
            };
            var conn = connectionFactory.CreateConnection();
            var model = conn.CreateModel();
            var queue = model.QueueDeclare(route, true, false, false, null);
            var sub = new Subscription(model, route);

            using (conn)
            {
                using (model)
                {
                    foreach (BasicDeliverEventArgs e in sub)
                    {
                        var msg = DeserializeFromJson(Encoding.UTF8.GetString(e.Body));
                        sub.Ack(e);
                        if (OnMessageReceived != null)
                            OnMessageReceived(msg);
                    }
                }
            }
        }
        
        public Message DeserializeFromJson(string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            var mem = new MemoryStream(256);
            mem.Write(buffer, 0, buffer.Length);
            mem.Position = 0;
            var msg = _serializer.ReadObject(mem);
            return (Message)msg;
        }

    }
}
