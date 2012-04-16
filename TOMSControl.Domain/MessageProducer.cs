using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQ.Client;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using RabbitMQ.Client.MessagePatterns;
using RabbitMQ.Client.Events;

namespace TOMSControl.Domain
{
    public interface IMessageProducer
    {
        void Publish(Message message);
    }

    public class MessageProducer : IMessageProducer
    {
        DataContractJsonSerializer _serializer;
        string _host;
        string _username;
        string _password;

        public MessageProducer(string host, string username, string password)
        {
            _host = host;
            _username = username;
            _password = password;
            _serializer = new DataContractJsonSerializer(typeof(Message));
        }

        public void Publish(Message message)
        {
            try
            {
                var connectionFactory = new ConnectionFactory { 
                     HostName = _host, 
                     UserName = _username, 
                     Password = _password, };
                var conn = connectionFactory.CreateConnection();
                var model = conn.CreateModel();
                model.QueueDeclare(message.RoutingKey, true, false, false, null);

                using (conn)
                {
                    using (model)
                    {
                        IBasicProperties props = model.CreateBasicProperties();
                        props.DeliveryMode = 2;
                        model.BasicPublish("", message.RoutingKey, props, 
                            Encoding.UTF8.GetBytes(SerializeToJson(message)));
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to connect to RabbitMQ");
            }
        }

        public string SerializeToJson(Message msg)
        {
            var mem = new MemoryStream(256);
            _serializer.WriteObject(mem, msg);
            mem.Position = 0;
            var json = (new StreamReader(mem)).ReadToEnd();
            return json;
        }
    }
}
