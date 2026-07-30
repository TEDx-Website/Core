using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Domain.Common.DomainInterfaces;
using TEDx.Domain.Common.Entities;

namespace TEDx.Domain.Ticketing.Entities
{
    public class Packages : AuditableEntity , IConcurrent , ISoftDelete
    {
        public Guid Id {  get; set; }
        public Guid EventId { get; set; }
        public string? NameEn { get; set; }//nn 200
        public string? NameAr { get; set; }//nn 200
        public decimal Price { get; set; } // nn 18,2 Check
        public int SeatsPerPackage { get; set; } // NN Check
        public int MaxQuantityPerOrder { get; set; }
        public bool IsActive { get; set; } // NN Df
        public bool IsDeleted { get; set; } // nn df
        public DateTime? DeletedAtUtc { get; set; }
        public byte[] RowVersion { get; set; }//nn
        public Event Event { get; set; }
        public List<Order> Orders { get; set; }
    }
}
