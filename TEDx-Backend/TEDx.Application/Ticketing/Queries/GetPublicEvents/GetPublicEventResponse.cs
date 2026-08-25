using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Application.Common.Dtos;

namespace TEDx.Application.Ticketing.Queries.GetPublicEvents
{
    public sealed record GetPublicEventResponse(
        Guid Id,
        string TitleEn,
        string TitleAr,
        string DescriptionEn,
        string DescriptionAr,
        DateTime StartsAtUtc,
        DateTime EndsAtUtc,
        string? Location,
        string? ImageUrl,
        int Capacity,
        int RemainingSeats,
        string Status,
        MoneyDto TicketPrice,
        MoneyDto PriceFrom);
  
}
