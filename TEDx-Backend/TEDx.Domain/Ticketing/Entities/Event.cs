using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Domain.Common.DomainInterfaces;
using TEDx.Domain.Common.Entities;
using TEDx.Domain.Common.Exceptions;
using TEDx.Domain.Ticketing.Enums;

namespace TEDx.Domain.Ticketing.Entities
{
    public class Event : AuditableEntity, ISoftDelete, IConcurrent
    {
        public Guid Id { get; set; }
        public string? TitleEn { get; set; }//nn 200
        public string? TitleAr { get; set; }// nn 200
        public string? DescriptionEn { get; set; }// nn max
        public string? DescriptionAr { get; set; }// nn max
        public string? Venue { get; set; }//nn 300
        public DateTime StartAtUtc { get; set; }//nn

        public DateTime EndAtUtc { get; set; }//nn
        public int Capacity { get; set; } // Check
        public decimal TicketPrice { get; set; }// (18,2) Check
        public int MaxIndividualQtyPerOrder { get; set; }
        public EventStatus eventStatus { get; private set; }// nn df — changes only via transition methods
        public string? ImageUrl { get; set; }// 500 nn
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAtUtc { get; set; }
        public byte[] RowVersion { get; set; } = null!;
        // nav (intra-context only)
        public List<Order>? Orders { get; set; }
        public List<Tickets>? Tickets { get; set; }
        public List<PromoCodes>? PromoCodes { get; set; }
        public List<Packages>? Packages { get; set; }

        // --- State machine (D:Q55) ---

        /// <summary>Draft → Published.</summary>
        public void Publish()
        {
            if (eventStatus != EventStatus.Draft)
                throw new InvalidStateTransitionException(nameof(Event), eventStatus, EventStatus.Published);

            eventStatus = EventStatus.Published;
        }

        /// <summary>Draft or Published → Cancelled.</summary>
        public void Cancel()
        {
            if (eventStatus != EventStatus.Draft && eventStatus != EventStatus.Published)
                throw new InvalidStateTransitionException(nameof(Event), eventStatus, EventStatus.Cancelled);

            eventStatus = EventStatus.Cancelled;
        }

        /// <summary>Published or Cancelled → Archived.</summary>
        public void Archive()
        {
            if (eventStatus != EventStatus.Published && eventStatus != EventStatus.Cancelled)
                throw new InvalidStateTransitionException(nameof(Event), eventStatus, EventStatus.Archived);

            eventStatus = EventStatus.Archived;
        }
    }
}
