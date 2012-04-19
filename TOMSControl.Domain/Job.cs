using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOMSControl.Domain
{
    public class Job
    {
        public IMessageProducer MessageProducer {get; set;}   
        public WorkFlow ParentWorkFlow { get; set; }
        public string Name { get; set; }
        public IList<Command> Commands;

        public Job()
        {
            MessageProducer = new MessageProducer();
        }

        public void Execute()
        {
            var env = ParentWorkFlow.TargetEnvironment;
            var credential = env.Credential;
            if (Commands != null)
                foreach (var c in Commands)
                {
                    c.Ticket = Guid.NewGuid();
                    c.RoutingKey = env.GetRoute(c.CommandQueue);
                    MessageProducer.Publish(c.CommandMessage(),credential);
                }
        }
    }
}
