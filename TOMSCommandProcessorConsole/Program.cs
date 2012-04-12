using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOMSControl.Domain;

namespace TOMSCommandProcessorConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var environment = new EnvironmentContext();
            environment.TicketConsumer.ListenToQueue();
            var consoleProcessor = new ConsoleCommandProcessor(environment, "listshares");
        }
    }
}
