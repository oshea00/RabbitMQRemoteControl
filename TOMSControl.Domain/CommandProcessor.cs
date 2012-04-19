using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOMSControl.Domain
{
    public interface ICommandProcessor
    {
        Task ListenForCommand();
        void HandleMessage(Message msg);
    }

    public class CommandProcessor : ICommandProcessor
    {
        protected EnvironmentContext _environment;
        protected IMessageConsumer MessageConsumer { get; set; }
        protected IMessageProducer MessageProducer { get; set; }
        public string RouteKey { get; set; }

        public CommandProcessor(EnvironmentContext context,string routekey)
        {
            MessageConsumer = new MessageConsumer();
            MessageProducer = new MessageProducer();

            if (String.IsNullOrEmpty(routekey))
                throw new Exception("Route key must not be null");

            if (context == null)
                throw new Exception("CommandProcessor environment context must not be null");

            _environment = context;
            RouteKey = routekey;
            MessageConsumer.OnMessageReceived += (msg) =>
            {
                if (msg is CommandMessage)
                {
                    try
                    {
                        if (msg.RoutingKey.Equals(_environment.GetRoute(RouteKey),StringComparison.CurrentCultureIgnoreCase))
                        {
                              HandleMessage(msg);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log(msg.Ticket,ex.Message);
                    }
                }
            };
        }

        public Task ListenForCommand()
        {
            return MessageConsumer.ListenToQueueAsync(
                _environment.GetRoute(RouteKey),
                _environment.Credential);
        }

        public void Log(Guid ticket, string message)
        {
            MessageProducer.Publish(
                new CommandResultMessage
                {
                    RoutingKey = _environment.GetLogRoute(RouteKey),
                    Ticket = ticket,
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
