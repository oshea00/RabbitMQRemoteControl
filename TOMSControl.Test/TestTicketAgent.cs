using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOMSControl.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TOMSControl.Test
{
    public class TestTicketAgent : ITicketAgent
    {
        private Dictionary<int, int> _tickets = new Dictionary<int, int>();
        private Random rand = new Random((int)DateTime.Now.Ticks);
        public int GetTicket()
        {
            int ticket;

            do
            {
                ticket = (int)(rand.NextDouble() * (int.MaxValue - 1)) + 1;
            }
            while (IsValidTicket(ticket));
            _tickets[ticket] = ticket;

            return ticket;
        }

        public bool IsValidTicket(int ticket)
        {
            return _tickets.ContainsKey(ticket);
        }

        public void PunchTicket(int ticket)
        {
            _tickets.Remove(ticket);
        }

    }

    [TestClass]
    public class TicketAgentTests
    {
        [TestMethod]
        public void TicketAgentIssuesTickets()
        {
            ITicketAgent ta = new TestTicketAgent();
            Assert.AreNotEqual(0, ta.GetTicket());
        }

        [TestMethod]
        public void TicketAgentPunchesTickets()
        {
            ITicketAgent ta = new TestTicketAgent();
            var ticket = ta.GetTicket();
            Assert.AreEqual(true, ta.IsValidTicket(ticket));
            ta.PunchTicket(ticket);
            Assert.AreEqual(false, ta.IsValidTicket(ticket));
        }
    }
}
