using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOMSControl.Domain
{
    public interface ITicketAgent
    {
        int  GetTicket();
        bool IsValidTicket(int ticket);
        void PunchTicket(int ticket);
    }
}
