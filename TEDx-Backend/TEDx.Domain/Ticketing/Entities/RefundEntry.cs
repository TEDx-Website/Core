using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Domain.Common.Entities;

namespace TEDx.Domain.Ticketing.Entities
{
    public class RefundEntry : AuditableEntity
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public string Reason { get; set; } = null!;
        public int VoidedTicketsCount { get; set; }
        public int CheckedInTicketsRetained { get; set; }
        public int SeatsReleased { get; set; }
        public string RefundedBy { get; set; } = null!;
        public Order Order { get; set; } = null!;
    }
}
