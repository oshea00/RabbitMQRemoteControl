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
            if (Commands != null)
                foreach (var c in Commands)
                {
                    c.Ticket = ParentWorkFlow.TargetEnvironment.TicketAgent.GetTicket();
                    c.RoutingKey = BuildRouteKey(c);
                    ParentWorkFlow.TargetEnvironment.MessageProducer.Publish(c.CommandMessage());
                }
        }

        private string BuildRouteKey(Command c)
        {
            return ParentWorkFlow.TargetEnvironment.Name + "." + 
                   ParentWorkFlow.TargetEnvironment.RootRouteKey + "." + 
                   c.Tag;
        }
    
    }
}
