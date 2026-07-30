using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Domain.Common.DomainInterfaces;
using TEDx.Domain.Common.Entities;
using TEDx.Domain.Common.Exceptions;
using TEDx.Domain.Ticketing.Enums;
using TicketStatusEnum = TEDx.Domain.Ticketing.Enums.TicketsStatus;

namespace TEDx.Domain.Ticketing.Entities
{
    public class Tickets : AuditableEntity, IConcurrent
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; } // 1 Event -> M tickets  restrict
        public Guid OrderId { get; set; } // 1 Order -> M tickets restrict
        public string? TicketReference { get; set; } // 20  UQ
        public string? QrSecretHash { get; set; } // 88 UQ
        public string? GuestName { get; set; } // 200
        public string? CheckedInBy { get; private set; }
        public DateTime CheckedInAtUtc { get; private set; }
        public TicketsStatus TicketsStatus { get; private set; } // Df NN — changes only via transition methods
        public byte[] RowVersion { get; set; } = null!;
        // nav prop (intra-context only)
        public Event Event { get; set; } = null!;
        public Order Order { get; set; } = null!;

        // --- State machine (D:Q55) ---

        /// <summary>Issued → CheckedIn.</summary>
        public void CheckIn(string checkedInBy, DateTime utcNow)
        {
            if (TicketsStatus != TicketStatusEnum.Issued)
                throw new InvalidStateTransitionException(nameof(Tickets), TicketsStatus, TicketStatusEnum.CheckedIn);

            TicketsStatus = TicketStatusEnum.CheckedIn;
            CheckedInBy = checkedInBy;
            CheckedInAtUtc = utcNow;
        }

        /// <summary>Issued or CheckedIn → Voided.</summary>
        public void Void()
        {
            if (TicketsStatus == TicketStatusEnum.Voided)
                throw new InvalidStateTransitionException(nameof(Tickets), TicketsStatus, TicketStatusEnum.Voided);

            TicketsStatus = TicketStatusEnum.Voided;
        }
    }
}
