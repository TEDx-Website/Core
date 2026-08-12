using System;
using TEDx.Application.Ticketing.DTOs;

namespace TEDx.Api.Requests.Events
{
    public sealed record UpdateEventRequest(
        string TitleEN,
        string TitleAr,
        string DescriptionEN,
        string DescriptionAr,
        string Venue,
        DateTime StartsAtUtc,
        DateTime EndsAtUtc,
        int Capacity,
        MoneyDto TicketPrice,
        int? MaxIndividualQtyPerOrder,
        string RowVersion
    );
}
