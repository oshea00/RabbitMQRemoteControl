using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TOMSControl.Domain;

namespace TOMSControl.Test
{
    [TestClass]
    public class MessageExchangeTests
    {
        [TestMethod]
        public void ExchangeCanSerializeToJson()
        {
            // Serialize
            var ex = new MessageExchange();
            var json = ex.SerializeToJson(new CommandMessage() 
            {  RoutingKey = "test.route", ExecuteFile = "net.exe" });
            Assert.IsTrue(json.Contains("CommandMessage"));
        }

        [TestMethod]
        public void ExchangeCanDeserializeFromJson()
        {
            // Serialize
            var ex = new MessageExchange();
            var json = ex.SerializeToJson(new CommandMessage() { RoutingKey = "test.route", ExecuteFile = "net.exe" });
            // DeSerialize
            var msg = ex.DeserializeFromJson(json);
            Assert.IsTrue(msg is CommandMessage);
            Assert.AreEqual("net.exe", ((CommandMessage)msg).ExecuteFile);
        }

        [TestMethod]
        public void CanPeekAndGetMessagesOnExchange()
        {
            IMessageExchange e = new MessageExchange();
            var sent = new CommandMessage() { RoutingKey = "my.test.route", ExecuteFile = "command message", Ticket = 123 };
            e.Publish(sent);
            Assert.AreEqual(null,e.PeekMessage("my.bad.route"));
            Assert.AreEqual(sent.ExecuteFile, ((CommandMessage)e.PeekMessage("my.test.route")).ExecuteFile);
            Assert.AreEqual(sent.ExecuteFile, ((CommandMessage)e.NextMessage("my.test.route")).ExecuteFile);

        }

        [TestMethod]
        public void MessageExchangeConsoleCommandProcessorListensForCommand()
        {
            bool didSomething = false;
            var environment = new EnvironmentContext
            {
                Name = "prod",
                RootRouteKey = "admin",
                TicketAgent = new TestTicketAgent(),
                Exchange = new MessageExchange()
            };

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

            // Setup command processor to listen to a command request
            ICommandProcessor cp = new ConsoleCommandProcessor(environment, "listshares");

            // Listen for result from command processor
            environment.Exchange.OnPublish += (result) =>
            {
                if (result is CommandResultMessage)
                {
                    if (result.RoutingKey.Equals(cp.GetResultRoute()))
                    {
                        var msg = environment.Exchange.NextMessage(result.RoutingKey);
                        Console.WriteLine(((CommandResultMessage)msg).CommandResult);
                        didSomething = true; // a little trick here for testing 
                    }
                }
            };

            // Execute workflow
            wf.Execute();

            Assert.IsTrue(didSomething);
        }

    }
}
