using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Domain.Common.Entities;

namespace TEDx.Domain.Ticketing.Entities
{
    public class RefundEntry : AuditableEntity
    {
        public Guid Id { get; set; }
        public Guid OrderId {  get; set; } // nn uq
        public string Reason { get; set; } = null!;//nnn
        public int VoidedTicketsCount { get; set; }//nnn
        public int CheckedInTicketsRetained { get; set; }//nn
        public int SeatsReleased { get; set; }//nn
        public string RefundedBy { get; set; } = null!;//nn
        public Order Order { get; set; } = null!;

    }
}
