using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Domain.Ticketing.Enums;

namespace TEDx.Application.Ticketing.DTOs
{
    public class CreateEventDTO
    {
        public Guid? Id { get; set; }
        public string? TitleEn { get; set; }
        public string? TitleAr { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public DateTime? StartsAtUtc { get; set; }
        public DateTime? EndsAtUtc { get; set; }
        public string? Location { get; set; }
        public int? Capacity { get; set; }
        public MoneyDto? TicketPrice { get; set; }
        public int? MaxIndividualQtyPerOrder { get; set; }
        public string? ImageUrl { get; set; }
        public EventStatus? Status { get; set; }
        public byte[]? RowVersion { get; set; }

    }
}
