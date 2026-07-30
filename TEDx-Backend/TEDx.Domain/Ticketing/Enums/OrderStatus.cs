using System;
using System.Collections.Generic;
using System.Text;

namespace TEDx.Domain.Ticketing.Enums
{
    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }
}
