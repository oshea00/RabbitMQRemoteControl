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

            environment.TicketConsumer.ListenForTickets(environment.Credential);

            var commandWatcher = new CommandQueueWatcher(environment);

            commandWatcher.AddWatchedQueue("listshares");
            commandWatcher.AddWatchedQueue("listdir");

            Task.WaitAll(commandWatcher.GetTasks());
        }
    }
}
