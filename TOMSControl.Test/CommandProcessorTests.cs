using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOMSControl.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using NSubstitute;

namespace TOMSControl.Test
{
    [TestClass]
    public class CommandProcessorTest
    {


        [TestMethod]
        public void ConsoleCommandProcessorRunsACommand()
        {
            int lines = 0;
            var consoleCmd = new ConsoleCommandProcessor(
                new EnvironmentContext { Name = "prod", RootRouteKey = "admin" }, "listshare");

            consoleCmd.OnOutputLineReady += (line) =>
            {
                lines++;
            };

            consoleCmd.HandleMessage(new CommandMessage { 
              ExecuteFile = @"c:\windows\system32\net.exe",
              Arguments = @"share",
              WorkingDirectory = @"c:\",
            });

            Assert.AreNotEqual(0, lines);
        }
    
    }
}
