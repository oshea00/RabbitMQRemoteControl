using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TOMSControl.Domain;

namespace TOMSControl.Test
{
    [TestClass]
    public class TestMessageExchangeTests
    {
        [TestMethod]
        public void CanPublishAndReceivePublishEventsOnExchange()
        {
            int count = 0;
            IMessageExchange e = new TestMessageExchange();
            var sent1 = new CommandMessage {RoutingKey = "my.test.route",  ExecuteFile = "command message", Ticket = 123 };
            var sent2 = new CommandMessage { RoutingKey = "my.other.route", ExecuteFile = "command message", Ticket = 123 };
            e.Publish(sent1);
            e.Publish(sent2);
            e.OnPublish += (msg) =>
            {
                var m = (CommandMessage) msg;
                count++;
                if (msg.RoutingKey.Contains("my.test.route"))
                {
                    Assert.AreEqual(sent1.ExecuteFile, e.NextMessage(msg.RoutingKey));
                    Assert.AreEqual(1, count);
                }

                if (msg.RoutingKey.Contains("my.other.route"))
                {
                    Assert.AreEqual(sent2.ExecuteFile, e.NextMessage(msg.RoutingKey));
                    Assert.AreEqual(1, count);
                }
            };
        }

        [TestMethod]
        public void CanPeekMessageOnExchange()
        {
            IMessageExchange e = new TestMessageExchange();
            var sent = new CommandMessage() 
            {RoutingKey = "my.test.route", ExecuteFile="command message", Ticket=123};
            e.Publish(sent);
            Assert.AreEqual(null,e.PeekMessage("my.bad.route"));
            Assert.AreEqual(sent,e.PeekMessage("my.test.route"));
        }

        [TestMethod]
        public void NextMessageRemovesFromQueue()
        {
            IMessageExchange e = new TestMessageExchange();
            var sent = new CommandMessage() { RoutingKey = "my.test.route", ExecuteFile = "command message", Ticket = 123 };
            e.Publish(sent);
            Assert.AreEqual(sent, e.NextMessage("my.test.route"));
            Assert.AreEqual(null, e.NextMessage("my.test.route"));
        }

    }
}
