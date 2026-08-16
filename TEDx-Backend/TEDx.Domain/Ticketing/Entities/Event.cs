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
        public string? TitleEn { get; set; }
        public string? TitleAr { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? Venue { get; set; }
        public DateTime StartAtUtc { get; set; }
        public DateTime EndAtUtc { get; set; }
        public int Capacity { get; set; }
        public decimal TicketPrice { get; set; }
        public int? MaxIndividualQtyPerOrder { get; set; }
        public EventStatus Status { get; private set; }
        public string? ImageUrl { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAtUtc { get; set; }
        public byte[] RowVersion { get; set; } = null!;

        // nav (intra-context only)
        public List<Order>? Orders { get; set; }
        public List<Ticket>? Tickets { get; set; }
        public List<PromoCode>? PromoCodes { get; set; }
        public List<Package>? Packages { get; set; }

        // --- State machine (D:Q55, D:Q56) ---

        public void Publish()
        {
            if (Status != EventStatus.Draft && Status != EventStatus.Archived)
                throw new InvalidStateTransitionException(nameof(Event), Status, EventStatus.Published);

            if (TicketPrice < 0)
                throw new EventNotPublishableException(
                    EventPublishBlock.InvalidTicketPrice, "TicketPrice must be ≥ 0.");

            if (Capacity <= 0)
                throw new EventNotPublishableException(
                    EventPublishBlock.InvalidCapacity, "Capacity must be > 0.");

            Status = EventStatus.Published;
        }

        public void Revert(int orderCount)
        {
            if (Status != EventStatus.Published)
                throw new InvalidStateTransitionException(nameof(Event), Status, EventStatus.Draft);

            if (orderCount > 0)
                throw new EventHasOrdersException();

            Status = EventStatus.Draft;
        }

        public void Archive()
        {
            if (Status != EventStatus.Published)
                throw new InvalidStateTransitionException(nameof(Event), Status, EventStatus.Archived);

            Status = EventStatus.Archived;
        }

        public void Cancel()
        {
            if (Status != EventStatus.Published && Status != EventStatus.Archived)
                throw new InvalidStateTransitionException(nameof(Event), Status, EventStatus.Cancelled);

            Status = EventStatus.Cancelled;
        }

        public static Event Create(
            string titleEn,
            string titleAr,
            string descriptionEn,
            string descriptionAr,
            DateTime startAtUtc,
            DateTime endAtUtc,
            string venue,
            int capacity,
            decimal ticketPrice,
            int? maxIndividualQtyPerOrder,
            string? imageUrl)
        {
            return new Event
            {
                Id = Guid.NewGuid(),
                TitleEn = titleEn,
                TitleAr = titleAr,
                DescriptionEn = descriptionEn,
                DescriptionAr = descriptionAr,
                StartAtUtc = startAtUtc,
                EndAtUtc = endAtUtc,
                Venue = venue,
                Capacity = capacity,
                TicketPrice = ticketPrice,
                MaxIndividualQtyPerOrder = maxIndividualQtyPerOrder,
                ImageUrl = imageUrl,
                Status = EventStatus.Draft
            };
        }
    }

}
