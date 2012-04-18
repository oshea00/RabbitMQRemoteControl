using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOMSControl.Domain
{
    public interface ICommandProcessor
    {
        void ListenForCommand();
    }

    public class CommandProcessor : ICommandProcessor
    {
        protected EnvironmentContext _environment;

        public string RouteKey { get; set; }
        public int Ticket { get; set; }

        public CommandProcessor(EnvironmentContext context,string routekey)
        {
            if (String.IsNullOrEmpty(routekey))
                throw new Exception("Route key must not be null");

            if (context == null)
                throw new Exception("CommandProcessor environment context must not be null");

            _environment = context;
            RouteKey = routekey;
            _environment.MessageConsumer.OnMessageReceived += (msg) =>
            {
                if (msg is CommandMessage)
                {
                    Ticket = msg.Ticket;
                    try
                    {
                        if (msg.RoutingKey.Equals(_environment.GetRoute(RouteKey),StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (_environment.TicketConsumer.IsValidTicket(Ticket))
                            {
                                HandleMessage(msg);
                            }
                            else
                            {
                                Log("Invalid Ticket on Command");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log(ex.Message);
                    }
                }
            };
        }

        public void ListenForCommand()
        {
            _environment.MessageConsumer.ListenToQueue(
                _environment.GetRoute(RouteKey),
                _environment.Credential);
        }

        public virtual void Log(string message)
        {
            _environment.MessageProducer.Publish(
                new CommandResultMessage
                {
                    RoutingKey = _environment.GetLogRoute(RouteKey),
                    Ticket = Ticket,
                    CommandResult = message,
                },
                _environment.Credential);
        }

        public virtual void HandleMessage(Message msg)
        {
            // default do nothing
        }
    }
}
