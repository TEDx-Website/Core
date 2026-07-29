using System;
using System.Collections.Generic;
using System.Text;

namespace TEDx.Domain.Ticketing.Enums
{
    public enum TicketsStatus
    {
        Active,
        CheckedIn,
        Cancelled,
        Refunded,
        Voided
    }
}
