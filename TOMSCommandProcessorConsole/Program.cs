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

            CommandQueueWatcher commandWatcher;

            if (args.Count() == 0)
                commandWatcher = new CommandQueueWatcher(environment, new[] { "listshares", "listdir" });
            else
                commandWatcher = new CommandQueueWatcher(environment, new[] { args[0] });

            Task.WaitAll(commandWatcher.GetTasks());
        }
    }
}
