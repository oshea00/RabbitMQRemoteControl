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
        IMessageProducer _messageProducer;

        public CommandQueueWatcher(EnvironmentContext environment, IList<string> queueNames)
        {
            _messageProducer = new MessageProducer();
            _environment = environment;
            _tasks = new List<Task>();
            foreach (var queueName in queueNames ?? new List<string>())
                AddWatchedQueue(queueName);
        }

        public Task AddWatchedQueue(string queueName)
        {
            var consoleProcessor = new ConsoleCommandProcessor(_environment, queueName);

            consoleProcessor.OnOutputLineReady += (ticket,line) =>
                _messageProducer.Publish(new CommandResultMessage
                {
                    RoutingKey = _environment.GetResultRoute(consoleProcessor.RouteKey),
                    Ticket = ticket,
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
