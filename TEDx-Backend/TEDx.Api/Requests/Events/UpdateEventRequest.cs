using TEDx.Application.Ticketing.DTOs;

namespace TEDx.Api.Requests.Events
{
    public sealed record UpdateEventRequest(
        string TitleEn,
        string TitleAr,
        string DescriptionEn,
        string DescriptionAr,
        DateTime StartsAtUtc,
        DateTime EndsAtUtc,
        string Location,
        int Capacity,
        MoneyDto TicketPrice,
        int? MaxIndividualQtyPerOrder,
        string RowVersion
    );
}
