using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOMSControl.Domain
{
    public class Job
    {
        
        public WorkFlow ParentWorkFlow { get; set; }
        public string Name { get; set; }
        public IList<Command> Commands;

        public void Execute()
        {
            var env = ParentWorkFlow.TargetEnvironment;
            var credential = env.Credential;
            if (Commands != null)
                foreach (var c in Commands)
                {
                    c.Ticket = env.TicketAgent.GetTicket(credential);
                    c.RoutingKey = env.GetRoute(c.CommandQueue);
                    env.MessageProducer.Publish(c.CommandMessage(),credential);
                }
        }
    }
}
