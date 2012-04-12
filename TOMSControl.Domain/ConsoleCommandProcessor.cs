using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TOMSControl.Domain
{
    public class ConsoleCommandProcessor : CommandProcessor
    {
        public ConsoleCommandProcessor(EnvironmentContext context, string route) : base(context,route)
        {
        }

        protected override void HandleMessage(Message m)
        {
            var msg = (CommandMessage) m;

            Process p = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    WorkingDirectory = msg.WorkingDirectory,
                    FileName = msg.ExecuteFile,
                    Arguments = msg.Arguments,
                }
            };

            p.OutputDataReceived += new DataReceivedEventHandler((o, e) =>
            {
                Console.WriteLine(e.Data);
                SendOutputLine(msg.Ticket, e.Data);
            });

            p.Start();
            p.BeginOutputReadLine();
            p.WaitForExit();
        }

        protected void SendOutputLine(int ticket, string line)
        {
            _environment.MessageProducer.Publish(new CommandResultMessage
            {
                RoutingKey = _environment.GetResultRoute(_routekey),
                Ticket = ticket,
                CommandResult = line,
            });
        }
    }
}
