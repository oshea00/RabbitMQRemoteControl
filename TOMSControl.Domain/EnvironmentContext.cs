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

        public EnvironmentContext()
        {
            Name = ConfigurationManager.AppSettings.Get("name");
            RootRouteKey = ConfigurationManager.AppSettings.Get("rootroute");
            var secureAppSettings = (NameValueCollection) ConfigurationManager.GetSection("secureAppSettings");
            Credential = new NetworkCredential {
                  Domain = ConfigurationManager.AppSettings.Get("host"),
                  UserName = secureAppSettings["username"],
                  Password = secureAppSettings["password"]
            };
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
