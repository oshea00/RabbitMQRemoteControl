using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TOMSControl.Domain
{
    [DataContract]
    [KnownType(typeof(CommandMessage))]
    [KnownType(typeof(CommandResultMessage))]
    public class Message
    {
        [DataMember]
        public string RoutingKey { get; set; }
        [DataMember]
        public Guid Ticket { get; set; }
    }

    [DataContract]
    public class CommandMessage : Message
    {
        [DataMember]
        public string WorkingDirectory { get; set; }
        [DataMember]
        public string ExecuteFile { get; set; }
        [DataMember]
        public string Arguments { get; set; }
    }

    [DataContract]
    public class CommandResultMessage : Message
    {
        [DataMember]
        public string CommandResult { get; set; }
    }
}

