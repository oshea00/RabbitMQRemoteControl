using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TOMSControl.Domain;
using NSubstitute;

namespace TOMSControl.Test
{
    [TestClass]
    public class DomainTests
    {
        [TestMethod]
        public void CommandHasNameAndQueue()
        {
            var cmd = new Command("Securities Import","secimport");
            Assert.IsNotNull(cmd.Name);
        }

        [TestMethod]
        public void CommandHasExecutableCommand()
        {
            var cmd = new Command("Securities Import","secimport") { ExecuteFile = "DoImport.bat BBH", Arguments= "-o -e 1302" };
            Assert.IsNotNull(cmd.ExecuteFile);
        }

        [TestMethod]
        public void CommandNameMustBeNonEmpty()
        {
            try
            {
                var cmd = new Command("", "sec import");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Name must not be empty", ex.Message);
            }
        }

        [TestMethod]
        public void CommandNameIsNotNull()
        {
            try
            {
                var cmd = new Command(null, "sec import");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Name must not be empty", ex.Message);
            }
        }

        [TestMethod]
        public void CommandQueueHasNoSpaces()
        {
            try
            {
                var cmd = new Command("Securities Import", "sec import");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Command queue must not be empty or contain whitespace", ex.Message);
            }
        }

        [TestMethod]
        public void CommandTagIsNotNull()
        {
            try
            {
                var cmd = new Command("Securities Import", null);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Command queue must not be empty or contain whitespace", ex.Message);
            }
        }

        [TestMethod]
        public void WorkflowNameNotNull()
        {
            try
            {
                var wf = new WorkFlow(null, 
                                      new EnvironmentContext { 
                                          Name = "prod",
                                          });
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Workflow must have a name.", ex.Message);
            }
        }

        [TestMethod]
        public void WorkflowMustHaveEnvironment()
        {
            try
            {
                var wf = new WorkFlow("MAT Security Import", null);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Workflow must have an environment.", ex.Message);
            }
        }

        [TestMethod]
        public void WorkflowHasTarget()
        {
            var wf = new WorkFlow("MAT Securities Import",new EnvironmentContext { Name = "prod" });
            Assert.IsNotNull(wf.TargetEnvironment);
        }

        [TestMethod]
        public void JobHasCommands()
        {
            var job = new Job { Name = "Securities Import Job" };
            job.Commands = new Command[] {
                new Command("Securities Import","secimport"),
            };
            Assert.AreEqual(1, job.Commands.Count());
        }

        [TestMethod]
        public void EnvironmentHasRootRoute()
        {
            var e = new EnvironmentContext { RootRouteKey = "tomi" };
            Assert.IsNotNull(e.RootRouteKey);
        }

        [TestMethod]
        public void EnvironmentHasUsername()
        {
            var e = new EnvironmentContext();
            Assert.IsNotNull(e.Username);
        }

        [TestMethod]
        public void EnvironmentHasPassword()
        {
            var e = new EnvironmentContext();
            Assert.IsNotNull(e.Password);
        }

        [TestMethod]
        public void EnvironmentBuildsRouteKey()
        {
            var env = new EnvironmentContext();
            Assert.AreEqual("dev.admin.somequeue",env.GetRoute("somequeue"));
        }

        [TestMethod]
        public void EnvironmentBuildsResultRouteKey()
        {
            var env = new EnvironmentContext();
            Assert.AreEqual("dev.admin.somequeue.result", env.GetResultRoute("somequeue"));
        }

        [TestMethod]
        public void EnvironmentBuildsLogKey()
        {
            var env = new EnvironmentContext();
            Assert.AreEqual("dev.admin.somequeue.log", env.GetLogRoute("somequeue"));
        }

        [TestMethod]
        public void MessageConsumerCallsOnMessageReceivedEventWithMessage()
        {
            bool eventWasraised = false;
            IMessageConsumer msgconsumer = Substitute.For<IMessageConsumer>();
            msgconsumer.OnMessageReceived += (msg) => eventWasraised = true;
            msgconsumer.OnMessageReceived += Raise.Event<Action<Message>>(new Message());
            Assert.IsTrue(eventWasraised);
        }

        [TestMethod]
        public void JobExecutesCommandUsingATicketAndMessageProducer()
        {
            var env = new EnvironmentContext
            {
                Name = "atest",
                RootRouteKey = "admin",
                TicketAgent = Substitute.For<ITicketAgent>(),
                MessageProducer = Substitute.For<IMessageProducer>(),
            };

            env.TicketAgent.GetTicket().Returns(1001);

            var wf = new WorkFlow("Test Workflow", env);
            var job = new Job { 
                ParentWorkFlow = wf,
                Commands = new Command[] { new Command("Test Command","testqueue") },
                };

            job.Execute();

            Assert.AreEqual(1001, job.Commands[0].Ticket);
            Assert.AreEqual("atest.admin.testqueue", job.Commands[0].RoutingKey);
            env.MessageProducer.Received().Publish(Arg.Any<Message>());
            
        }


    }
}
