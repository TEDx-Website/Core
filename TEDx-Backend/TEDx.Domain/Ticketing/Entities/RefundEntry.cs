using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Domain.Common.DomainInterfaces;

namespace TEDx.Domain.Ticketing.Entities
{
    public class RefundEntry : IRefreshTokenAudit
    {
        public Guid Id { get; set; }
        public Guid OrderId {  get; set; } // nn uq
        public string Reason { get; set; }//nnn
        public int VoidedTicketsCount { get; set; }//nnn
        public int CheckedInTicketsRetained { get; set; }//nn
        public int SeatsReleased { get; set; }//nn
        public string RefundedBy { get; set; }//nn

        public DateTime CreatedAtUtc { get; set; }//nn
        public Order Order { get; set; }

    }
}
