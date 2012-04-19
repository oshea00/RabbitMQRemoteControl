using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOMSControl.Domain
{
    public interface IWorkFlowResultWatcher
    {
        void AddCommandResultQueue(EnvironmentContext environment, string queueName);
        IList<CommandResultMessage> GetAllResults();
        IList<CommandResultMessage> GetResultsByQueue(string queueName);
        IList<Task> Tasks { get; set; }
    }

    public class WorkFlowResultWatcher : IWorkFlowResultWatcher
    {
        object _locker = new object();
        Dictionary<string, List<CommandResultMessage>> _results;
        public IList<Task> Tasks { get; set; }

        public WorkFlowResultWatcher(WorkFlow wf)
        {
            if (wf == null)
                throw new Exception("Workflow must not be null");

            _results = new Dictionary<string, List<CommandResultMessage>>();
            Tasks = new List<Task>();

            var cmds = wf.Jobs.SelectMany(j => j.Commands).ToList();
            foreach (var c in cmds)
            {
                AddCommandResultQueue(wf.TargetEnvironment,c.CommandQueue);
            }
        }

        public void AddCommandResultQueue(EnvironmentContext environment, string queueName)
        {
            var messageConsumer = new MessageConsumer();
            messageConsumer.OnMessageReceived += (msg) =>
            {
                lock (_locker)
                {
                    var message = (CommandResultMessage) msg;
                    if (!_results.ContainsKey(queueName))
                    {
                        _results[queueName] = new List<CommandResultMessage>();
                    }
                    _results[queueName].Add(message);
                }
            };
 
            Tasks.Add(messageConsumer.ListenToQueueAsync(
                environment.GetResultRoute(queueName),
                environment.Credential));
        }

        public IList<CommandResultMessage> GetAllResults()
        {
            var all = new List<CommandResultMessage>();
            foreach (var key in _results.Keys)
            {
                foreach (var line in _results[key])
                {
                    all.Add(line);
                }
            }
            return all;
        }

        public IList<CommandResultMessage> GetResultsByQueue(string queueName)
        {
            if (_results.ContainsKey(queueName))
                return _results[queueName];
            return new List<CommandResultMessage>();
        }
    }
}
