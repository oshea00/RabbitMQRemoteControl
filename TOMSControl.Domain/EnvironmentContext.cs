using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;
using System.Net;

namespace TOMSControl.Domain
{
    public class EnvironmentContext
    {
        public string Name { get; set; }
        public string RootRouteKey { get; set; }
        public NetworkCredential Credential { get; set; }
        public IMessageProducer MessageProducer { get; set; }
        public IMessageConsumer MessageConsumer { get; set; }
        public ITicketAgent TicketAgent { get; set; }
        public ITicketConsumer TicketConsumer { get; set; }

        public EnvironmentContext()
        {
            Name = ConfigurationManager.AppSettings.Get("name");
            RootRouteKey = ConfigurationManager.AppSettings.Get("rootroute");
            var Host = ConfigurationManager.AppSettings.Get("host");
            var secureAppSettings = (NameValueCollection) ConfigurationManager.GetSection("secureAppSettings");
            var Username = secureAppSettings["username"];
            var Password = secureAppSettings["password"];
            Credential = new NetworkCredential { 
                  Domain = Host,
                  UserName = Username,
                  Password = Password
            };

            MessageProducer = new MessageProducer();
            MessageConsumer = new MessageConsumer();
            TicketAgent = new TicketAgent();
            TicketConsumer = new TicketConsumer();
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
