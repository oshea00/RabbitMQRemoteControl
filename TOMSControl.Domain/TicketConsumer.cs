using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using RabbitMQ.Client.MessagePatterns;
using System.Threading.Tasks;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Net;

namespace TOMSControl.Domain
{
    public interface ITicketConsumer
    {
        bool IsValidTicket(int ticket);
        void PunchTicket(int ticket);
        void ListenForTickets(NetworkCredential credential);
    }

    public class TicketConsumer : ITicketConsumer
    {
        Dictionary<int, bool> _tickets;
        protected const string _ticketExchange = "ticketExchange";
        protected const string _ticketKey = "ticket";

        public TicketConsumer()
        {
            _tickets = new Dictionary<int, bool>();
        }

        public void ListenForTickets(NetworkCredential credential)
        {
            Task.Factory.StartNew(() =>
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
                        model.ExchangeDeclare(_ticketExchange, "fanout");
                        model.QueueDeclare(_ticketKey, true, false, false, null);
                        model.QueueBind(_ticketKey, _ticketExchange, _ticketKey);
                        var queue = model.QueueDeclare("", false, true, true, null);
                        model.QueueBind(queue.QueueName, _ticketExchange, _ticketKey);
                        var sub = new Subscription(model, _ticketKey);

                        using (conn)
                        {
                            using (model)
                            {
                                foreach (BasicDeliverEventArgs e in sub)
                                {
                                    var msg = Int32.Parse(Encoding.UTF8.GetString(e.Body));
                                    lock (_tickets)
                                    {
                                        if (!_tickets.ContainsKey(msg))
                                        {
                                            _tickets[msg] = true;
                                        }
                                    }
                                    sub.Ack(e);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Thread.Sleep(1000);
                    }
                }
            });
        }

        public bool IsValidTicket(int ticket)
        {
            return _tickets.ContainsKey(ticket);
        }

        public void PunchTicket(int ticket)
        {
            if (IsValidTicket(ticket))
                _tickets.Remove(ticket);
        }

    }
}
