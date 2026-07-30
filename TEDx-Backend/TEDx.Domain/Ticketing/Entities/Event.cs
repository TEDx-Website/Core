using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Domain.Common.DomainInterfaces;
using TEDx.Domain.Ticketing.Enums;

namespace TEDx.Domain.Ticketing.Entities
{
    public class Event : IAuditable, ISoftDelete, IConcurrent
    {
        public Guid Id { get; set; }
        public string? TitleEn {  get; set; }//nn 200
        public string? TitleAr { get; set; }// nn 200
        public string? DescriptionEn { get; set; }// nn max
        public string? DescriptionAr { get; set; }// nn max
        public string? Venue {  get; set; }//nn 300
        public DateTime StartAtUtc { get; set; }//nn

        public DateTime EndAtUtc { get; set; }//nn
        public int Capacity { get; set; } // Check
        public decimal TicketPrice { get; set; }// (18,2) Check
        public int MaxIndividualQtyPerOrder { get; set; }
        public EventStatus eventStatus { get; set; }// nn df
        public string? ImageUrl { get; set; }// 500 nn
        public DateTime CreatedAtUtc { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAtUtc { get; set; }
        public byte[] RowVersion { get; set; }
        // nav
        public List<Order>? Orders {  get; set; }
        public List<Tickets>? Tickets { get; set; }
        public List<PromoCodes>? PromoCodes {  get; set; }
        public List<Packages>? Packages {  get; set; }
    }
}
