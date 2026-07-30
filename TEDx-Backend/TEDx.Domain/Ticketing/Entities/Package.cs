using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Domain.Common.DomainInterfaces;
using TEDx.Domain.Common.Entities;

namespace TEDx.Domain.Ticketing.Entities
{
    public class Package : AuditableEntity, IConcurrent, ISoftDelete
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public string? NameEn { get; set; }
        public string? NameAr { get; set; }
        public decimal Price { get; set; }
        public int SeatsPerPackage { get; set; }
        public int MaxQuantityPerOrder { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAtUtc { get; set; }
        public byte[] RowVersion { get; set; } = null!;
        public Event Event { get; set; } = null!;
        public List<Order>? Orders { get; set; }
    }
}
