using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQ.Client;
using System.IO;
using System.Runtime.Serialization.Json;

namespace TOMSControl.Domain
{
    public class MessageExchange : IMessageExchange
    {
        DataContractJsonSerializer _serializer;
        IModel _model;
        IConnection _conn;
        ConnectionFactory _connectionFactory;

        public event Action<Message> OnPublish;

        public MessageExchange()
        {
            _serializer = new DataContractJsonSerializer(typeof(Message));
        }

        public void Publish(Message message)
        {
            try
            {
                ConnectToQueue(message.RoutingKey);

                IBasicProperties props = _model.CreateBasicProperties();
                props.DeliveryMode = 2;
                _model.BasicPublish("", message.RoutingKey, props, Encoding.UTF8.GetBytes(SerializeToJson(message)));

                CloseQueue();

                if (OnPublish != null)
                    OnPublish(message);
            }
            catch (Exception)
            {
            }
        }

        public Message NextMessage(string route)
        {
            try
            {
                ConnectToQueue(route);
                BasicGetResult result = _model.BasicGet(route, false);
                if (result != null)
                {
                    var msg = DeserializeFromJson(Encoding.UTF8.GetString(result.Body));
                    _model.BasicAck(result.DeliveryTag,false);
                    return (Message)msg;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                CloseQueue();
            }
        }

        public Message PeekMessage(string route)
        {
            try
            {
                ConnectToQueue(route);
                BasicGetResult result = _model.BasicGet(route, false);
                if (result != null)
                {
                    var msg = DeserializeFromJson(Encoding.UTF8.GetString(result.Body));
                    _model.BasicNack(result.DeliveryTag, false, true);
                    return (Message)msg;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                CloseQueue();
            }

        }

        private void ConnectToQueue(string queueName)
        {
            _connectionFactory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "Winter09" };
            _conn = _connectionFactory.CreateConnection();
            _model = _conn.CreateModel();
            _model.QueueDeclare(queueName, true, false, false, null);
        }

        private void CloseQueue()
        {
            if (_conn != null)
                _conn.Close();
            if (_model != null)
                _model.Abort();
        }

        public string SerializeToJson(Message msg)
        {
            var mem = new MemoryStream(256);
            _serializer.WriteObject(mem, msg);
            mem.Position = 0;
            var json = (new StreamReader(mem)).ReadToEnd();
            return json;
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
