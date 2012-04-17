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
            var wf = new WorkFlow("Get Current Network Shares", environment)
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
                         }
                     }
                }
            };

            // Listen and respond to command result messages
            environment.MessageConsumer.OnMessageReceived += (msg) =>
            {
                var r = (CommandResultMessage)msg;
                Console.WriteLine(r.CommandResult);
            };
            environment.MessageConsumer.ListenToQueueAsync(environment.GetResultRoute("listshares"));

            // Execute the workflow
            do
            {
                Console.WriteLine("Hit Enter To Send Command");
                Console.ReadLine();
                wf.Execute();
            } while (true);

        }
    }
}
