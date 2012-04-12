using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace TOMSControl.Domain
{
    public class EnvironmentContext
    {
        public string Name { get; set; }
        public string RootRouteKey { get; set; }
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public IMessageProducer MessageProducer { get; set; }
        public IMessageConsumer MessageConsumer { get; set; }
        public ITicketAgent TicketAgent { get; set; }
        public ITicketConsumer TicketConsumer { get; set; }

        public EnvironmentContext()
        {
            Name = ConfigurationManager.AppSettings.Get("name");
            RootRouteKey = ConfigurationManager.AppSettings.Get("rootroute");
            Host = ConfigurationManager.AppSettings.Get("host");
            Username = ConfigurationManager.AppSettings.Get("username");
            Password = ConfigurationManager.AppSettings.Get("password");

            MessageProducer = new MessageProducer(Host, Username, Password);
            MessageConsumer = new MessageConsumer(Host, Username, Password);
            TicketAgent = new TicketAgent(Host, Username, Password);
            TicketConsumer = new TicketConsumer(Host, Username, Password);
        }

        public string GetRoute(string queue)
        {
            return Name + "." + RootRouteKey + "." + queue;
        }

        public string GetResultRoute(string queue)
        {
            return GetRoute(queue) + ".result";
        }

        public string GetLogRoute(string queue)
        {
            return GetRoute(queue) + ".log";
        }
    }
}
