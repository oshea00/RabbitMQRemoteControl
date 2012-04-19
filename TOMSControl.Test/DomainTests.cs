using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TOMSControl.Domain;
using NSubstitute;
using System.Net;
using System.Threading.Tasks;

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
            Assert.IsNotNull(e.Credential.UserName);
        }

        [TestMethod]
        public void EnvironmentHasPassword()
        {
            var e = new EnvironmentContext();
            Assert.IsNotNull(e.Credential.Password);
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
        public void JobExecutesCommandUsingAMessageProducer()
        {
            var env = new EnvironmentContext
            {
                Name = "atest",
                RootRouteKey = "admin",
            };

            var wf = new WorkFlow("Test Workflow", env);
            var job = new Job { 
                ParentWorkFlow = wf,
                Commands = new Command[] { new Command("Test Command","testqueue") },
                };

            job.MessageProducer = Substitute.For<IMessageProducer>();
            job.Execute();

            Assert.AreEqual("atest.admin.testqueue", job.Commands[0].RoutingKey);
            job.MessageProducer.Received().Publish(Arg.Any<Message>(),Arg.Any<NetworkCredential>());
            
        }

        [TestMethod]
        public void CommandWatcherWatchesCommands()
        {
            var watcher = Substitute.For<ICommandQueueWatcher>();
            watcher.AddWatchedQueue(Arg.Any<string>()).Returns<Task>(Arg.Any<Task>());
        }

        [TestMethod]
        public void CommandWatcherReturnsTaskWhenQueueIsAdded()
        {
            var env = new EnvironmentContext();
            var watcher = new CommandQueueWatcher(env,null);
            var task = watcher.AddWatchedQueue("testqueue");
            Assert.IsNotNull(task);
        }

        [TestMethod]
        public void CommandWatcherReturnsListOfWatchedQueueTasks()
        {
            var env = new EnvironmentContext();
            var watcher = new CommandQueueWatcher(env, new[] { "testqueue" });
            Assert.AreNotEqual(0, watcher.GetTasks().Count());
        }

        [TestMethod]
        public void WorkFlowResultWatcherCanWatchResultQueues()
        {
            var wfwatcher = Substitute.For<IWorkFlowResultWatcher>();
            wfwatcher.AddCommandResultQueue(Arg.Any<EnvironmentContext>(),Arg.Any<string>());
        }

        [TestMethod]
        public void WorkFlowResultWatcherMustHaveNonNullWorkflow()
        {
            try
            {
                var wf = new WorkFlowResultWatcher(null);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Workflow must not be null", ex.Message);
            }
        }

    }
}
