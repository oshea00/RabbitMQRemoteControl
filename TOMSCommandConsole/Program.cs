using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOMSControl.Domain;
using System.Threading.Tasks;

namespace TOMSCommandConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var environment = new EnvironmentContext();

            // Setup workflow with a job and a command
            var wf = new WorkFlow("Get Current Network Shares and Windows Directory", environment)
            {
                Jobs = new List<Job> { 
                         new Job {
                         Name = "List Shares",
                         Commands = new List<Command>() { 
                           new Command("NET Command","listshares") 
                           {  
                              ExecuteFile = @"c:\windows\system32\net.exe",
                              Arguments = "share",
                           },
                           new Command("NET Command","listdir") 
                           {  
                              ExecuteFile = @"c:\windows\system32\cmd.exe",
                              Arguments = @"/c dir",
                              WorkingDirectory = @"c:\windows",
                           },
                         }
                     }
                }
            };

            // Listen for results
            var workflowResultWatcher = new WorkFlowResultWatcher(wf);

            // Execute the workflow
            do
            {
                Console.WriteLine("Hit Enter To Execute Workflow");
                if (Console.ReadLine()=="q")
                    break;
                wf.Execute();
            } while (true);

            foreach (var line in workflowResultWatcher.GetAllResults())
                Console.WriteLine(line);
        }
    }
}
