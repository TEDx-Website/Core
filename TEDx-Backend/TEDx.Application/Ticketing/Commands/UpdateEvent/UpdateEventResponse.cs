using TEDx.Application.Common.Dtos;
using TEDx.Domain.Ticketing.Enums;

namespace TEDx.Application.Ticketing.Commands.UpdateEvent;

public sealed record UpdateEventResponse(
    Guid Id,
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
    EventStatus Status,
    string RowVersion);
