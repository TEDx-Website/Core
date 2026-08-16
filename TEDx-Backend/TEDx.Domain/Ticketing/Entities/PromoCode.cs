using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Domain.Common.Abstractions;
using TEDx.Domain.Common.Entities;
using TEDx.Domain.Ticketing.Enums;

namespace TEDx.Domain.Ticketing.Entities
{
    public class PromoCode : AuditableEntity, ISoftDeletable, IHasRowVersion
    {
        public Guid Id { get; set; }
        public string? Code { get; set; }
        public Guid EventId { get; set; }
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public bool IsActive { get; set; }
        public DateTime ValidFromUtc { get; set; }
        public DateTime ValidUntilUtc { get; set; }
        public int MaxTotalRedemption { get; set; }
        public int MaxPerUser { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAtUtc { get; set; }
        public byte[] RowVersion { get; set; } = null!;
        public Event Event { get; set; } = null!;
        public List<Order>? Orders { get; set; }
        public List<PromoRedemption>? PromoRedemptions { get; set; }
    }
}
