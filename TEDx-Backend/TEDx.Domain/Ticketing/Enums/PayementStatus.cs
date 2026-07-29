using System;
using System.Collections.Generic;
using System.Text;

namespace TEDx.Domain.Ticketing.Enums
{
    public enum PayementStatus
    {
        pending,
        Succeeded,
        Failed,
        Cancelled,
        Refunded
    }
}
