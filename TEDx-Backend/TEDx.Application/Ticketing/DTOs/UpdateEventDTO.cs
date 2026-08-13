using TEDx.Domain.Ticketing.Enums;

namespace TEDx.Application.Ticketing.DTOs;

public sealed record UpdateEventDTO(
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
