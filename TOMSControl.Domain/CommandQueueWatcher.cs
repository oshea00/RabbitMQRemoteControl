using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOMSControl.Domain
{
    public interface ICommandQueueWatcher
    {
        Task AddWatchedQueue(string queueName);
        Task[] GetTasks();
    }

    public class CommandQueueWatcher : ICommandQueueWatcher
    {
        EnvironmentContext _environment;
        List<Task> _tasks;

        public CommandQueueWatcher(EnvironmentContext environment)
        {
            _environment = environment;
            _tasks = new List<Task>();
        }

        public Task AddWatchedQueue(string queueName)
        {
            var consoleProcessor = new ConsoleCommandProcessor(_environment, queueName);

            consoleProcessor.OnOutputLineReady += (line) =>
                _environment.MessageProducer.Publish(new CommandResultMessage
                {
                    RoutingKey = _environment.GetResultRoute(consoleProcessor.RouteKey),
                    Ticket = consoleProcessor.Ticket,
                    CommandResult = line,
                }, _environment.Credential);

            var task = consoleProcessor.ListenForCommand();
            _tasks.Add(task);
            return task;
        }

        public Task[] GetTasks()
        {
            return _tasks.ToArray();
        }
    }
}
