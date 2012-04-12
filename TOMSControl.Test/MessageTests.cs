using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TOMSControl.Domain;

namespace TOMSControl.Test
{
    [TestClass]
    public class MessageTests
    {

        [TestMethod]
        public void MessageProducerCanSerializeToJson()
        {
            // Serialize
            var ex = new MessageProducer(null,null,null);
            var json = ex.SerializeToJson(new CommandMessage() 
            {  RoutingKey = "test.route", ExecuteFile = "net.exe" });
            Assert.IsTrue(json.Contains("CommandMessage"));
        }

        [TestMethod]
        public void MessageConsumerCanDeserializeFromJson()
        {
            // Serialize
            var ex = new MessageProducer(null,null,null);
            var json = ex.SerializeToJson(new CommandMessage() { RoutingKey = "test.route", ExecuteFile = "net.exe" });
            // DeSerialize
            var consumer = new MessageConsumer(null,null,null);
            var msg = consumer.DeserializeFromJson(json);
            Assert.IsTrue(msg is CommandMessage);
            Assert.AreEqual("net.exe", ((CommandMessage)msg).ExecuteFile);
        }

    }
}
