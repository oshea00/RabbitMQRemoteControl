using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace TOMSControl.Domain
{
    public class ConsoleCommandProcessor : CommandProcessor
    {
        public ConsoleCommandProcessor(EnvironmentContext context, string route) : base(context,route)
        {
        }

        public event Action<Guid,string> OnOutputLineReady;

        public override void HandleMessage(Message m)
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
                if (OnOutputLineReady != null)
                    OnOutputLineReady(msg.Ticket,e.Data);
            });

            p.Start();
            p.BeginOutputReadLine();
            p.WaitForExit();

        }
    }
}
