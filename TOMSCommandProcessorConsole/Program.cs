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

            var consoleProcessor = new ConsoleCommandProcessor(environment, "listshares");

            consoleProcessor.OnOutputLineReady += (line) =>
                environment.MessageProducer.Publish(new CommandResultMessage
                {
                    RoutingKey = environment.GetResultRoute(consoleProcessor.RouteKey),
                    Ticket = consoleProcessor.Ticket,
                    CommandResult = line,
                },environment.Credential);

            consoleProcessor.ListenForCommand();
        }
    }
}
