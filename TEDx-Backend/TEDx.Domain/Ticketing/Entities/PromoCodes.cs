using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Domain.Common.DomainInterfaces;
using TEDx.Domain.Common.Entities;
using TEDx.Domain.Ticketing.Enums;

namespace TEDx.Domain.Ticketing.Entities
{
    public class PromoCodes : AuditableEntity, ISoftDelete , IConcurrent
    {
        public Guid Id {  get; set; }
        public string? Code { get; set; } // NN UQ
        public Guid EventId {  get; set; }
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; } //  18,2
        public bool IsActive { get; set; } // NN Df
        public DateTime ValidFromUtc { get; set; }
        public DateTime ValidUnitUtc { get; set; }
        public int MaxTotalRedemption {  get; set; }
        public int MaxPerUser {  get; set; }
        public bool IsDeleted { get; set; } // nn df
        public DateTime? DeletedAtUtc { get; set; }
        public byte[] RowVersion { get; set; }//nn
        public Event Event { get; set; }
        public List<Order> Orders { get; set; }
        public List<PromoRedemption> PromoRedemptions { get; set; }

    }
}
