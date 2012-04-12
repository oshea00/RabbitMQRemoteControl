using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOMSControl.Domain
{
    public class Command
    {
        public string RoutingKey { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }
        public string ExecuteFile { get; set; }
        public string WorkingDirectory { get; set; }
        public string Arguments { get; set; }
        public int    Ticket { get; set; }

        public Command(string name, string tag)
        {
            if (!String.IsNullOrWhiteSpace(name))
                Name = name;
            else
                throw new Exception("Name must not be empty");

            if (!String.IsNullOrWhiteSpace(tag) && !tag.Contains(" "))
                Tag = tag;
            else
                throw new Exception("Tag must not be empty or contain whitespace");
        }

        public CommandMessage CommandMessage()
        {
            return new CommandMessage() { 
                RoutingKey = RoutingKey, 
                Ticket = Ticket, 
                ExecuteFile = ExecuteFile,
                WorkingDirectory = WorkingDirectory,
                Arguments = Arguments,
            }; 
        }
    }
}
