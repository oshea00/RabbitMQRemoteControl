using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;
using RabbitMQ.Client.Events;
using System.Net;

namespace TOMSControl.Domain
{
    public interface IMessageConsumer
    {
        event Action<Message> OnMessageReceived;
        void ListenToQueue(string route, NetworkCredential credential);
        void ListenToQueueAsync(string route, NetworkCredential credential);
    }

    public class MessageConsumer : IMessageConsumer
    {
        DataContractJsonSerializer _serializer;
        public event Action<Message> OnMessageReceived;

        public MessageConsumer()
        {
            _serializer = new DataContractJsonSerializer(typeof(Message));
        }

        public void ListenToQueueAsync(string route,NetworkCredential credential)
        {
            Task.Factory.StartNew(() => ListenToQueue(route,credential));
        }

        public void ListenToQueue(string route,NetworkCredential credential)
        {
            while (true)
            {
                try
                {
                    var connectionFactory = new ConnectionFactory
                    {
                        HostName = credential.Domain,
                        UserName = credential.UserName,
                        Password = credential.Password,
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
                                if (OnMessageReceived != null)
                                    OnMessageReceived(msg);
                                sub.Ack(e);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    Thread.Sleep(1000);
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
