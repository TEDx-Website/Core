using TEDx.Application.Common.Dtos;
using TEDx.Application.Ticketing.Dtos;

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
        string? ImageUrl,
        string RowVersion
    );
}
