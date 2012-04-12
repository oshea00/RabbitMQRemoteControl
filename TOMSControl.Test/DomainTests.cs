using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TOMSControl.Domain;

namespace TOMSControl.Test
{
    [TestClass]
    public class DomainTests
    {
        [TestMethod]
        public void CommandHasNameAndTag()
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
        public void CommandTagHasNoSpaces()
        {
            try
            {
                var cmd = new Command("Securities Import", "sec import");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Tag must not be empty or contain whitespace", ex.Message);
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
                Assert.AreEqual("Tag must not be empty or contain whitespace", ex.Message);
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

    }
}
