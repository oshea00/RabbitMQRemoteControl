using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOMSControl.Domain
{
    public interface ICommandProcessor
    {
    }

    public class CommandProcessor : ICommandProcessor
    {
        protected EnvironmentContext _environment;
        protected string _routekey;

        public CommandProcessor(EnvironmentContext context,string routekey)
        {
            if (String.IsNullOrEmpty(routekey))
                throw new Exception("Route key must not be null");

            if (context == null)
                throw new Exception("CommandProcessor environment context must not be null");

            _environment = context;
            _routekey = routekey;
            _environment.MessageConsumer.OnMessageReceived += (msg) =>
            {
                if (msg is CommandMessage)
                {
                    try
                    {
                        if (msg.RoutingKey.Equals(_environment.GetRoute(_routekey),StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (_environment.TicketConsumer.IsValidTicket(msg.Ticket))
                            {
                                HandleMessage(msg);
                            }
                            else
                            {
                                Log(msg.Ticket, "Invalid Ticket on Command");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log(msg.Ticket,ex.Message);
                    }
                }
            };
            _environment.MessageConsumer.ListenToQueue(_environment.GetRoute(_routekey));
        }


        public virtual void Log(int ticket, string message)
        {
            _environment.MessageProducer.Publish(
                new CommandResultMessage
                {
                    RoutingKey = _environment.GetLogRoute(_routekey),
                    Ticket = ticket,
                    CommandResult = message,
                });
        }

        protected virtual void HandleMessage(Message msg)
        {
            // default do nothing
        }
    }
}
