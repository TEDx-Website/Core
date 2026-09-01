using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Domain.Common.Abstractions;
using TEDx.Domain.Common.Entities;
using TEDx.Domain.Common.Exceptions;
using TEDx.Domain.Ticketing.Enums;

namespace TEDx.Domain.Ticketing.Entities
{
    public class Ticket : AuditableEntity, IHasRowVersion
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public Guid OrderId { get; set; }
        public string? TicketReference { get; set; }
        public string? QrSecretHash { get; set; }
        public string? GuestName { get; set; }
        public string? CheckedInBy { get; private set; }
        public DateTime CheckedInAtUtc { get; private set; }
        public TicketStatus Status { get; private set; }
        public byte[] RowVersion { get; set; } = null!;
        // nav prop (intra-context only)
        public Event Event { get; set; } = null!;
        public Order Order { get; set; } = null!;

        // --- State machine (D:Q55) ---

        /// <summary>Issued → CheckedIn.</summary>
        public void CheckIn(string checkedInBy, DateTime utcNow)
        {
            if (Status != TicketStatus.Issued)
                throw new InvalidStateTransitionException(nameof(Ticket), Status, TicketStatus.CheckedIn);

            Status = TicketStatus.CheckedIn;
            CheckedInBy = checkedInBy;
            CheckedInAtUtc = utcNow;
        }

        /// <summary>Issued or CheckedIn → Voided.</summary>
        public void Void()
        {
            if (Status != TicketStatus.Voided)
                throw new InvalidStateTransitionException(nameof(Ticket), Status, TicketStatus.Voided);

            Status = TicketStatus.Voided;
        }
    }
}
