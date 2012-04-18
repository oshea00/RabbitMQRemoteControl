using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;
using RabbitMQ.Client.Events;
using System.Net;

namespace TOMSControl.Domain
{
    public interface ITicketAgent
    {
        int GetTicket(NetworkCredential credential);
    }

    public class TicketAgent : ITicketAgent
    {
        protected Random _rand;
        protected const string _ticketExchange = "ticketExchange";
        protected const string _ticketKey = "ticket";

        public TicketAgent ()
	    {
            _rand = new Random((int)DateTime.Now.Ticks);
	    }

        public int GetTicket(NetworkCredential credential)
        {
            try
            {
                int ticket;
                ticket = (int)(_rand.NextDouble() * (int.MaxValue - 1)) + 1;

                var connectionFactory = new ConnectionFactory
                {
                    HostName = credential.Domain,
                    UserName = credential.UserName,
                    Password = credential.Password,
                };
                var conn = connectionFactory.CreateConnection();
                var model = conn.CreateModel();
                model.ExchangeDeclare(_ticketExchange, "fanout");
                model.QueueDeclare(_ticketKey, true, false, false, null);
                model.QueueBind(_ticketKey, _ticketExchange, _ticketKey);
                var queue = model.QueueDeclare("", false, true, true, null);
                model.QueueBind(queue.QueueName, _ticketExchange, _ticketKey);

                using (conn)
                {
                    using (model)
                    {
                        IBasicProperties props = model.CreateBasicProperties();
                        props.DeliveryMode = 2;
                        model.BasicPublish(_ticketExchange, _ticketKey, props, Encoding.UTF8.GetBytes(ticket.ToString()));
                    }
                }

                return ticket;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
