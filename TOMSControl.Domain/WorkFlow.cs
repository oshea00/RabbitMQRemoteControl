using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOMSControl.Domain
{
    public class WorkFlow
    {
        public string Name { get; private set; }
        public EnvironmentContext TargetEnvironment { get; private set; }

        private IList<Job> _jobs;
        public IList<Job> Jobs
        {
            get
            {
                return _jobs;
            }
            set
            {
                if (value != null)
                {
                    foreach (var j in value)
                    {
                        j.ParentWorkFlow = this;
                    }
                    _jobs = value;
                }
            }
        }

        public WorkFlow(string name, EnvironmentContext environment)
        {
            if (!String.IsNullOrWhiteSpace(name))
                Name = name;
            else
                throw new Exception("Workflow must have a name.");

            if (environment != null)
                TargetEnvironment = environment;
            else
                throw new Exception("Workflow must have an environment.");
        }

        public void Execute()
        {
            foreach (var j in Jobs)
            {
                j.Execute();
            }
        }
    }
}
