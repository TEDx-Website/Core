using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Domain.Common.DomainInterfaces;

namespace TEDx.Domain.Ticketing.Entities
{
    public class Packages : IAuditable , IConcurrent , ISoftDelete
    {
        public Guid Id {  get; set; }
        public Guid EventId { get; set; }
        public string? NameEn { get; set; }//nn 200
        public string? NameAr { get; set; }//nn 200
        public decimal Price { get; set; } // nn 18,2 Check
        public int SeatsPerPackage { get; set; } // NN Check
        public int MaxQuantityPerOrder { get; set; }
        public bool IsActive { get; set; } // NN Df
        public DateTime CreatedAtUtc { get; set; } // nn
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; } // nn df
        public DateTime? DeletedAtUtc { get; set; }
        public byte[] RowVersion { get; set; }//nn
        public Event Event { get; set; }
        public List<Order> Orders { get; set; }
    }
}
