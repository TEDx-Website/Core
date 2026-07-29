using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Domain.Common.DomainInterfaces;
using TEDx.Domain.Ticketing.Enums;

namespace TEDx.Domain.Ticketing.Entities
{
    public class Tickets : IAuditable , IConcurrent
    {
        public Guid Id {  get; set; }
        public Guid EventId { get; set; } // 1 Event -> M tickets  restrict
        public Guid OrderId { get; set; } // 1 Order -> M tickets restrict
        public string? TicketReference { get; set; } // 20  UQ
        public string? QrSecretHash { get; set; } // 88 UQ
        public string? GuestName { get; set; } // 200
        public string? CheckedInBy {  get; set; }
        public DateTime CheckedInAtUtc { get; set; }
        public TicketsStatus TicketsStatus { get; set; } // Df NN
        public DateTime CreatedAtUtc { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; }
        public Event Event { get; set; }
        public Order Order { get; set; }
    }
}
