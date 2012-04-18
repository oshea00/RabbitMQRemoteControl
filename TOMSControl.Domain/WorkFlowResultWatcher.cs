using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOMSControl.Domain
{
    public interface IWorkFlowResultWatcher
    {
        Action<Message> ResultAction { set; }
        void AddCommandQueue(string queueName);
    }

    public class WorkFlowResultWatcher : IWorkFlowResultWatcher
    {
        EnvironmentContext _environment;

        public WorkFlowResultWatcher(EnvironmentContext environment)
        {
            _environment = environment;
        }

        public Action<Message> ResultAction
        {
            set
            {
                _environment.MessageConsumer.OnMessageReceived += value;
            }
        }

        public void AddCommandQueue(string queueName)
        {
            _environment.MessageConsumer.ListenToQueueAsync(
                _environment.GetResultRoute(queueName),
                _environment.Credential);
        }
    }
}
